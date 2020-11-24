//#define USE_TOUCH_SCRIPT

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarageKit
{
    // Press&Releaseの連携処理コンポーネント格納用
    [Serializable]
    public class RelationalComponentData
    {
        public MonoBehaviour component = null;
        public string pressFunctionName = "";
        public string releaseFunctionName = "";
    }

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

        public enum INPUT_PHASE
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
        public bool asFirstResponder = true; // Hitチェックの重なりを考慮

        [Header("button view")]
        public Renderer targetRenderer;
        public Color defaultButtonColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        public Color disableButtonColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        public Texture2D offButtonTexture;
        public Texture2D onButtonTexture;

        [Header("toggle button")]
        public bool asToggle = false;
        private bool toggleState = false;
        public bool ToggleState { get{ return toggleState; } }

        [Header("relational call")]
        public RelationalComponentData[] relationalComponents;

        private bool isEnableButton = true;
        public bool IsEnableButton { get{ return isEnableButton; } }
        private bool inputEnable = true;
        public bool InputEnable { get{ return inputEnable; } }

        private Camera rayCamera;
        private Collider collid;

        private bool isTouch = true; // タッチしてるかどうか
        private bool isPressed = false; // Pressタイプのとき、Pressできたかどうか
        private int touchCount;
        private Vector3 touchPosition; 
        public Vector3 TouchPosition { get{ return touchPosition; } }
        private INPUT_PHASE phase = INPUT_PHASE.NONE;

        static public int PressBtnsTotal = 0;

        public Action OnButton;
        public Action<bool> OnToggleButton;


        public static void SetInputType(INPUT_TYPE type)
        {
            ButtonObjectEvent[] btns = FindObjectsOfType<ButtonObjectEvent>();
            foreach(ButtonObjectEvent btn in btns)
                btn.inputType = type;
        }


        void Awake()
        {
            if(rayCamera == null)
                rayCamera = CameraUtil.FindCameraForLayer(this.gameObject.layer);
            if(rayCamera == null)
                rayCamera = Camera.main;

            collid = this.GetComponent<Collider>();
            if(collid == null)
                Debug.LogWarning("collider is null. a collider component is required for button processing.");
        }

        void Start()
        {
            // ボタンテクスチャの設定
            ChangeTexture(false);
        }

        void Update()
        {
            // デバイス入力
            InputDevice();

            // ボタン判定
            CheckTouchButton();

            // トグルボタン時にテクスチャを上書き設定
            if(asToggle && !isPressed)
                ChangeTexture(toggleState);
        }


        private void InputDevice()
        {
            isTouch = true;
            touchCount = 0;

            if(inputType == INPUT_TYPE.TOUCH)
            {
#if UNITY_STANDALONE_WIN
                // for Windows Player
#if !USE_TOUCH_SCRIPT
                Debug.LogWarning("Input.Touch is not work in windows. please use TouchScript.");
#else
                // as TouchScript
                touchCount = TouchScript.TouchManager.Instance.PressedPointersCount;

                // 1点目を入力として受付
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

        private void CheckTouchButton()
        {
            if(!isTouch || !inputEnable)
            {
                Unhit();
                return;
            }

            // タッチ判定を行う
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
            if(phase == INPUT_PHASE.BEGAN) // Pressした瞬間
            {
                isPressed = true;
                
                OnPressButton(true);
            }
            else if(phase == INPUT_PHASE.MOVED) // 押している間
            {
                if(isPressed)
                    OnPressHoldButton();
            }
            else if(phase == INPUT_PHASE.ENDED) // 離した瞬間
            {
                if(isPressed)
                {
                    OnPressButton(false);	
                    OnClickButton();
                }

                isPressed = false;
            }
            else if(phase == INPUT_PHASE.NONE) // オーバー
                isPressed = false;
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
                    OnPressButton(false);
                
                    isPressed = false;
                }
            }
            else
                isPressed = false;
        }

#region Button Event Function
        private void OnPressButton(bool pressed)
        {
            if(pressed)
            {
                // Pressされたボタンのカウント
                PressBtnsTotal++;

                // Pressのコンポーネント連携
                RelationalComponentFunc(true);

                // テクスチャを切り替え
                ChangeTexture(true);

                // ボタン処理
                if(buttonType == BUTTON_TYPE.PRESS)
                    InvokeOnButton();
            }
            else
            {
                // Pressされたボタンのカウント
                PressBtnsTotal--;
                if(PressBtnsTotal < 0)
                    PressBtnsTotal = 0;

                // Releaseのコンポーネント連携
                RelationalComponentFunc(false);

                // テクスチャを切り替え
                if(!asToggle)
                    ChangeTexture(false);

                // ボタン処理
                if(buttonType == BUTTON_TYPE.RELEASE)
                    InvokeOnButton();
            }
        }

        private void OnPressHoldButton()
        {
            if(buttonType == BUTTON_TYPE.PRESSHOLD)
                InvokeOnButton();
        }

        private void OnClickButton()
        {
            if(buttonType == BUTTON_TYPE.CLICK)
                InvokeOnButton();
        }

        private void InvokeOnButton()
        {
            // トグルボタン
            if(asToggle)
            {
                toggleState = !toggleState;

                this.OnToggleButton?.Invoke(toggleState);
            }

            this.OnButton?.Invoke();
        }
#endregion

        private void RelationalComponentFunc(bool state)
        {
            foreach(RelationalComponentData componentData in relationalComponents)
            {
                if(componentData != null)
                {
                    if(state)
                        componentData.component.SendMessage(componentData.pressFunctionName, SendMessageOptions.DontRequireReceiver);
                    else
                        componentData.component.SendMessage(componentData.releaseFunctionName, SendMessageOptions.DontRequireReceiver);
                }
            }
        }

        private void ChangeTexture(bool pressed)
        {
            if(pressed)
            {
                if(targetRenderer != null && onButtonTexture != null)
                    targetRenderer.material.mainTexture = onButtonTexture;
            }
            else
            {
                if(targetRenderer != null && offButtonTexture != null)
                    targetRenderer.material.mainTexture = offButtonTexture;
            }
        }

# region Enable/Disable
        // ボタンの有効化
        public void EnableButton()
        {
            isEnableButton = true;

            if(targetRenderer != null)
                targetRenderer.material.color = defaultButtonColor;

            if(collid != null)
                collid.enabled = true;
        }

        // ボタンの無効化
        public void DisableButton()
        {
            isEnableButton = false;

            if(targetRenderer != null)
                targetRenderer.material.color = disableButtonColor;

            if(collid != null)
                collid.enabled = false;
        }

        // ボタンのリセット
        public void ResetButton()
        {
            toggleState = false;
            ChangeTexture(false);
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
