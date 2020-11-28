//#define USE_TOUCH_SCRIPT

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarageKit
{
    public class ButtonObjectEvent : MonoBehaviour
    {
        public enum INPUT_TYPE
        {
            MOUSE = 0,
            TOUCH
        }

        public enum BUTTON_TYPE
        {
            CLICK = 0,
            PRESS,
            RELEASE,
            PRESSHOLD
        }

        private enum INPUT_PHASE
        {
            NONE = 0,
            BEGAN,
            CANCELED,
            ENDED,
            MOVED,
            STATIONARY
        }

        public INPUT_TYPE inputType = INPUT_TYPE.MOUSE;
        public BUTTON_TYPE buttonType = BUTTON_TYPE.CLICK;
        public bool asFirstResponder = true;
        public bool asToggle = false;

        private bool isEnableButton = true;
        public bool IsEnableButton { get{ return isEnableButton; } }
        private bool inputEnable = true;
        public bool InputEnable { get{ return inputEnable; } }
        private bool toggleState = false;
        public bool ToggleState { get{ return toggleState; } }
        private Vector3 touchPosition; 
        public Vector3 TouchPosition { get{ return touchPosition; } }

        private Camera rayCamera;
        private Collider collid;

        private bool isTouch = true;
        private bool isPressed = false;
        private bool isHover = false;
        private int touchCount;
        
        private INPUT_PHASE phase = INPUT_PHASE.NONE;

        static public int PressBtnsTotal = 0;

        // Button Actions
        public Action OnButton;
        public Action<bool> OnToggleButton;
        public Action OnPressButton;
        public Action OnReleaseButton;
        public Action OnHoverInButton;
        public Action OnHoverExitButton;


        public static void SetAllInputType(INPUT_TYPE type)
        {
            ButtonObjectEvent[] btns = FindObjectsOfType<ButtonObjectEvent>();
            foreach(ButtonObjectEvent btn in btns)
                btn.inputType = type;
        }


        void Awake()
        {

        }

        void Start()
        {
            if(rayCamera == null)
                rayCamera = CameraUtil.FindCameraForLayer(this.gameObject.layer);
            if(rayCamera == null)
                rayCamera = Camera.main;

            collid = this.GetComponent<Collider>();
            if(collid == null)
                Debug.LogWarning("collider is null. a collider component is required for button processing.");
        }

        void Update()
        {
            InputByDevice();

            ButtonProcess();
        }


        private void InputByDevice()
        {
            isTouch = true;
            touchCount = 0;

            if(inputType == INPUT_TYPE.TOUCH)
            {
#if UNITY_STANDALONE_WIN
                // for Windows Player
#if !USE_TOUCH_SCRIPT
                Debug.LogWarning("Input.Touch is not work in windows. please use TouchScript. and then enable the define macro on the first line of this script.");
#else
                // as TouchScript
                touchCount = TouchScript.TouchManager.Instance.PressedPointersCount;

                if(touchCount >= 1)
                {
                    TouchScript.Pointers.Pointer tp = TouchScript.TouchManager.Instance.PressedPointers[0];
                    Rect wrect = WindowsUtil.GetApplicationWindowRect(); // not work in Editor.
                    touchPosition = new Vector2(
                        (int)(((tp.Position.x / Screen.width) * Screen.currentResolution.width) - wrect.x),
                        Screen.height + (int)(((tp.Position.y / Screen.height) * Screen.currentResolution.height) - wrect.y));

                    if(tp.Position == tp.PreviousPosition)
                    {
                        if(isPressed)
                            phase = INPUT_PHASE.Moved;
                        else
                            phase = INPUT_PHASE.Began;
                    }
                    else
                        phase = INPUT_PHASE.Moved;
                }
                else
#endif
                {
                    // Pressされた後にここにきたらReleaseとする
                    if(isPressed)
                        phase = INPUT_PHASE.ENDED;
                    else
                    {
                        phase = INPUT_PHASE.NONE;
                        touchPosition = Vector3.zero;
                        isTouch = false;
                    }
                }

#elif UNITY_IOS || UNITY_ANDROID
                // for Mobile
                touchCount = Input.touchCount;
                
                if(touchCount >= 1)
                {
                    touchPosition = Input.GetTouch(0).position;	
                    TouchPhase tphase = Input.GetTouch(0).phase;
                    
                    if(tphase == TouchPhase.Began)
                        phase = INPUT_PHASE.BEGAN;
                    else if(tphase == TouchPhase.Ended || tphase == TouchPhase.Canceled)
                        phase = INPUT_PHASE.ENDED;
                    else if(tphase == TouchPhase.Moved || tphase == TouchPhase.Stationary)
                        phase = INPUT_PHASE.MOVED;
                }
                else
                {
                    phase = INPUT_PHASE.NONE;
                    touchPosition = Vector3.zero;
                    isTouch = false;
                }
#endif
            }

            // for Mouse
            else if(inputType == INPUT_TYPE.MOUSE)
            {
                touchPosition = Input.mousePosition;

                if(Input.GetMouseButton(0))
                {
                    if(Input.GetMouseButtonDown(0))
                        phase = INPUT_PHASE.BEGAN;
                    else
                        phase = INPUT_PHASE.MOVED;

                    touchCount = 1;
                }
                else if(Input.GetMouseButtonUp(0))
                    phase = INPUT_PHASE.ENDED;
                else
                    phase = INPUT_PHASE.NONE;
            }

            if(touchCount < 0)
                touchCount = 0;
        }

        private void ButtonProcess()
        {
            if(!isTouch || !inputEnable)
            {
                Unhit();
                return;
            }

            RaycastHit hit = new RaycastHit();
            Ray ray = rayCamera.ScreenPointToRay(touchPosition);
            if(asFirstResponder)
            {
                // 重なりを考慮してチェック
                if(Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    if(hit.collider.gameObject == this.gameObject)
                        Hit();
                    else
                        Unhit();
                }
                else
                    Unhit();
            }
            else
            {
                // 重なりは無視してチェック
                if(collid.Raycast(ray, out hit, Mathf.Infinity))
                    Hit();
                else
                    Unhit();
            }
        }

        private void Hit()
        {
            if(phase == INPUT_PHASE.BEGAN) // 押した瞬間
            {
                if(!isHover)
                    this.OnHoverInButton?.Invoke();
                isHover = true;

                isPressed = true;
                this.OnPressButton?.Invoke();

                PressButton(true);
            }
            else if(phase == INPUT_PHASE.MOVED) // 押している間
            {
                if(isPressed)
                    PressHoldButton();
            }
            else if(phase == INPUT_PHASE.ENDED) // 離した瞬間
            {
                if(isPressed)
                {
                    isPressed = false;
                    this.OnReleaseButton?.Invoke();
                    PressButton(false);

                    ClickButton();

                    if(inputType == INPUT_TYPE.TOUCH)
                    {
                        isHover = false;
                        this.OnHoverExitButton?.Invoke();
                    }
                }
            }
            else if(phase == INPUT_PHASE.NONE) // オーバー
            {
                if(!isHover)
                    this.OnHoverInButton?.Invoke();
                isHover = true;

                isPressed = false;
            }
        }

        private void Unhit()
        {
            if(buttonType == BUTTON_TYPE.PRESS
                || buttonType == BUTTON_TYPE.PRESSHOLD
                || buttonType == BUTTON_TYPE.CLICK)
            {
                // Began時にOnPressしていれば処理
                if(phase == INPUT_PHASE.ENDED && isPressed)
                {
                    isPressed = false;
                    this.OnReleaseButton?.Invoke();
                    PressButton(false);
                }
            }
            else
                isPressed = false;

            if(isHover)
                this.OnHoverExitButton?.Invoke();
            isHover = false;
        }

#region Button Event Function
        private void PressButton(bool pressed)
        {
            if(pressed)
            {
                PressBtnsTotal++;

                if(buttonType == BUTTON_TYPE.PRESS)
                    InvokeOnButton();
            }
            else
            {
                PressBtnsTotal--;
                if(PressBtnsTotal < 0)
                    PressBtnsTotal = 0;

                if(buttonType == BUTTON_TYPE.RELEASE)
                    InvokeOnButton();
            }
        }

        private void PressHoldButton()
        {
            if(buttonType == BUTTON_TYPE.PRESSHOLD)
                InvokeOnButton();
        }

        private void ClickButton()
        {
            if(buttonType == BUTTON_TYPE.CLICK)
                InvokeOnButton();
        }

        private void InvokeOnButton()
        {
            this.OnButton?.Invoke();

            // トグルボタン
            if(asToggle)
            {
                toggleState = !toggleState;
                this.OnToggleButton?.Invoke(toggleState);
            }
        }
#endregion

# region Enable/Disable
        // ボタンの有効化
        public void EnableButton()
        {
            isEnableButton = true;

            if(collid != null)
                collid.enabled = true;
        }

        // ボタンの無効化
        public void DisableButton()
        {
            isEnableButton = false;

            if(collid != null)
                collid.enabled = false;
        }

        // ボタンのリセット
        public void ResetButton()
        {
            toggleState = false;
        }

        // 入力の有効化
        public void EnableInput()
        {
            inputEnable = true;
        }

        // 入力の無効化
        public void DisableInput()
        {
            inputEnable = false;
        }
#endregion
    }
}
