﻿//#define USE_TOUCH_SCRIPT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

/*
 * ドラッグ操作でのカメラの移動コントロールクラス
 * マウス&タッチ対応
 */ 
namespace GarageKit
{
    [RequireComponent(typeof(Camera))]
    public class FlyThroughCamera : MonoBehaviour
    {
        public static bool winTouch = false;
        public static bool updateEnable = true;

        // フライスルーのコントロールタイプ
        public enum FLYTHROUGH_CONTROLL_TYPE
        {
            DRAG = 0,
            DRAG_HOLD
        }

        public FLYTHROUGH_CONTROLL_TYPE controllType;

        // 移動軸の方向
        public enum FLYTHROUGH_MOVE_TYPE
        {
            XZ = 0,
            XY
        }

        public FLYTHROUGH_MOVE_TYPE moveType;

        public Collider groundCollider;
        public Collider limitAreaCollider;
        public bool useLimitArea = false;
        public float moveBias = 1.0f;
        public float moveSmoothTime = 0.1f;
        public bool dragInvertX = false;
        public bool dragInvertY = false;
        public float rotateBias = 1.0f;
        public float rotateSmoothTime = 0.1f;
        public bool rotateInvert = false;
        public OrbitCamera combinationOrbitCamera;

        private GameObject flyThroughRoot;
        public GameObject FlyThroughRoot { get{ return flyThroughRoot; } }
        private GameObject shiftTransformRoot;
        public Transform ShiftTransform { get{ return shiftTransformRoot.transform; } }

        private bool inputLock;
        public bool IsInputLock { get{ return inputLock; } }
        private object lockObject;

        private Vector3 defaultPos;
        private Quaternion defaultRot;

        private bool isFirstTouch;
        private Vector3 oldScrTouchPos;

        private Vector3 dragDelta;
        private Vector3 velocitySmoothPos;
        private Vector3 dampDragDelta = Vector3.zero;
        private Vector3 pushMoveDelta = Vector3.zero;

        private float velocitySmoothRot;
        private float dampRotateDelta = 0.0f;
        private float pushRotateDelta = 0.0f;

        public Vector3 currentPos { get{ return flyThroughRoot.transform.position; } }
        public Quaternion currentRot { get{ return flyThroughRoot.transform.rotation; } }


        void Awake()
        {

        }

        void Start()
        {
            // 設定ファイルより入力タイプを取得
            if(!ApplicationSetting.Instance.GetBool("UseMouse"))
                winTouch = true;

            inputLock = false;

            // 地面上視点位置に回転ルートを設定する
            Ray ray = new Ray(this.transform.position, this.transform.forward);
            RaycastHit hitInfo;
            if(groundCollider.Raycast(ray, out hitInfo, float.PositiveInfinity))
            {
                flyThroughRoot = new GameObject(this.gameObject.name + " FlyThrough Root");
                flyThroughRoot.transform.SetParent(this.gameObject.transform.parent, false);
                flyThroughRoot.transform.position = hitInfo.point;
                flyThroughRoot.transform.rotation = Quaternion.identity;

                shiftTransformRoot = new GameObject(this.gameObject.name + " ShiftTransform Root");
                shiftTransformRoot.transform.SetParent(flyThroughRoot.transform, true);
                shiftTransformRoot.transform.localPosition = Vector3.zero;
                shiftTransformRoot.transform.localRotation = Quaternion.identity;

                this.gameObject.transform.SetParent(shiftTransformRoot.transform, true);
            }
            else
            {
                Debug.LogWarning("FlyThroughCamera :: not set the ground collider !!");
                return;
            }

            // 初期値を保存
            defaultPos = flyThroughRoot.transform.position;
            defaultRot = flyThroughRoot.transform.rotation;

            ResetInput();
        }

        void Update()
        {	
            if(!inputLock && ButtonObjectEvent.PressBtnsTotal == 0)
                GetInput();
            else
                ResetInput();

            UpdateFlyThrough();
            UpdateOrbitCombination();
        }

        private void ResetInput()
        {
            isFirstTouch = true;
            oldScrTouchPos = Vector3.zero;
            dragDelta = Vector3.zero;
        }

        private void GetInput()
        {
            // for Touch
            if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if(Input.touchCount == 1)
                {	
                    // ドラッグ量を計算
                    Vector3 currentScrTouchPos = Input.GetTouch(0).position;

                    if(isFirstTouch)
                    {
                        oldScrTouchPos = currentScrTouchPos;
                        isFirstTouch = false;
                        return;
                    }

                    dragDelta = currentScrTouchPos - oldScrTouchPos;

                    if(controllType == FLYTHROUGH_CONTROLL_TYPE.DRAG)
                        oldScrTouchPos = currentScrTouchPos;
                }
                else
                    ResetInput();
            }

#if UNITY_STANDALONE_WIN
            else if(Application.platform == RuntimePlatform.WindowsPlayer && winTouch)
            {
#if !USE_TOUCH_SCRIPT
                if(Input.touchCount == 1)
                {	
                    // ドラッグ量を計算
                    Vector3 currentScrTouchPos = Input.touches[0].position;
#else
                if(TouchScript.TouchManager.Instance.PressedPointersCount == 1)
                {	
                    // ドラッグ量を計算
                    Vector3 currentScrTouchPos = TouchScript.TouchManager.Instance.PressedPointers[0].Position;
#endif
                    if(isFirstTouch)
                    {
                        oldScrTouchPos = currentScrTouchPos;
                        isFirstTouch = false;
                        return;
                    }

                    dragDelta = currentScrTouchPos - oldScrTouchPos;

                    if(controllType == FLYTHROUGH_CONTROLL_TYPE.DRAG)
                        oldScrTouchPos = currentScrTouchPos;
                }
                else
                    ResetInput();
            }
#endif
            // for Mouse
            else
            {
                if(Input.GetMouseButton(0))
                {	
                    // ドラッグ量を計算
                    Vector3 currentScrTouchPos = Input.mousePosition;

                    if(isFirstTouch)
                    {
                        oldScrTouchPos = currentScrTouchPos;
                        isFirstTouch = false;
                        return;
                    }

                    dragDelta = currentScrTouchPos - oldScrTouchPos;

                    if(controllType == FLYTHROUGH_CONTROLL_TYPE.DRAG)
                        oldScrTouchPos = currentScrTouchPos;
                }
                else
                    ResetInput();	
            }
        }

        /// <summary>
        /// Input更新のLock
        /// </summary>
        public void LockInput(object sender)
        {
            if(!inputLock)
            {
                lockObject = sender;
                inputLock = true;
            }
        }

        /// <summary>
        /// Input更新のUnLock
        /// </summary>
        public void UnlockInput(object sender)
        {
            if(inputLock && lockObject == sender)
                inputLock = false;
        }

        /// <summary>
        /// フライスルーを更新
        /// </summary>
        private void UpdateFlyThrough()
        {
            if(!FlyThroughCamera.updateEnable || flyThroughRoot == null)
                return;	

            // 位置
            float dragX = dragDelta.x * (dragInvertX ? -1.0f : 1.0f);
            float dragY = dragDelta.y * (dragInvertY ? -1.0f : 1.0f);

            if(moveType == FLYTHROUGH_MOVE_TYPE.XZ)
                dampDragDelta = Vector3.SmoothDamp(dampDragDelta, new Vector3(dragX, 0.0f, dragY) * moveBias + pushMoveDelta, ref velocitySmoothPos, moveSmoothTime);
            else if(moveType == FLYTHROUGH_MOVE_TYPE.XY)
                dampDragDelta = Vector3.SmoothDamp(dampDragDelta, new Vector3(dragX, dragY, 0.0f) * moveBias + pushMoveDelta, ref velocitySmoothPos, moveSmoothTime);

            flyThroughRoot.transform.Translate(dampDragDelta, Space.Self);
            pushMoveDelta = Vector3.zero;

            if(useLimitArea)
            {
                // 移動範囲限界を設定
                if(limitAreaCollider != null)
                {	
                    Vector3 movingLimitMin = limitAreaCollider.bounds.min + limitAreaCollider.bounds.center - limitAreaCollider.gameObject.transform.position;
                    Vector3 movingLimitMax = limitAreaCollider.bounds.max + limitAreaCollider.bounds.center - limitAreaCollider.gameObject.transform.position;
                    
                    if(moveType == FLYTHROUGH_MOVE_TYPE.XZ)
                    {
                        float limitX = Mathf.Max(Mathf.Min(flyThroughRoot.transform.position.x, movingLimitMax.x), movingLimitMin.x);
                        float limitZ = Mathf.Max(Mathf.Min(flyThroughRoot.transform.position.z, movingLimitMax.z), movingLimitMin.z);
                        flyThroughRoot.transform.position = new Vector3(limitX, flyThroughRoot.transform.position.y, limitZ);
                    }
                    else if(moveType == FLYTHROUGH_MOVE_TYPE.XY)
                    {
                        float limitX = Mathf.Max(Mathf.Min(flyThroughRoot.transform.position.x, movingLimitMax.x), movingLimitMin.x);
                        float limitY = Mathf.Max(Mathf.Min(flyThroughRoot.transform.position.y, movingLimitMax.y), movingLimitMin.y);
                        flyThroughRoot.transform.position = new Vector3(limitX, limitY, flyThroughRoot.transform.position.z);
                    }
                }
            }

            // 方向
            dampRotateDelta = Mathf.SmoothDamp(dampRotateDelta, pushRotateDelta * rotateBias, ref velocitySmoothRot, rotateSmoothTime);
            if(moveType == FLYTHROUGH_MOVE_TYPE.XZ)
                flyThroughRoot.transform.Rotate(Vector3.up, dampRotateDelta, Space.Self);
            else if(moveType == FLYTHROUGH_MOVE_TYPE.XY)
                flyThroughRoot.transform.Rotate(Vector3.forward, dampRotateDelta, Space.Self);
            pushRotateDelta = 0.0f;
        }

        private void UpdateOrbitCombination()
        {
            // 連携機能
            if(combinationOrbitCamera != null && combinationOrbitCamera.OrbitRoot != null)
            {
                Vector3 lookPoint = combinationOrbitCamera.OrbitRoot.transform.position;
                Transform orbitParent = combinationOrbitCamera.OrbitRoot.transform.parent;
                combinationOrbitCamera.OrbitRoot.transform.parent = null;
                flyThroughRoot.transform.LookAt(lookPoint, Vector3.up);
                flyThroughRoot.transform.rotation = Quaternion.Euler(
                    0.0f, flyThroughRoot.transform.rotation.eulerAngles.y + 180.0f, 0.0f);
                combinationOrbitCamera.OrbitRoot.transform.parent = orbitParent;
            }
        }

        /// <summary>
        /// 目標位置にカメラを移動させる
        /// </summary>
        public void MoveToFlyThrough(Vector3 targetPosition, float time = 1.0f)
        {
            dampDragDelta = Vector3.zero;

            flyThroughRoot.transform.DOMove(targetPosition, time)
                .SetEase(Ease.OutCubic)
                .Play();
        }

        /// <summary>
        /// 指定値でカメラを移動させる
        /// </summary>
        public void TranslateToFlyThrough(Vector3 move)
        {
            dampDragDelta = Vector3.zero;
            flyThroughRoot.transform.Translate(move, Space.Self);
        }

        /// <summary>
        /// 目標方向にカメラを回転させる
        /// </summary>
        public void RotateToFlyThrough(float targetAngle, float time = 1.0f)
        {
            dampRotateDelta = 0.0f;

            Vector3 targetEulerAngles = flyThroughRoot.transform.rotation.eulerAngles;
            targetEulerAngles.y = targetAngle;
            flyThroughRoot.transform.DORotate(targetEulerAngles, time)
                .SetEase(Ease.OutCubic)
                .Play();
        }

        /// <summary>
        /// 外部トリガーで移動させる
        /// </summary>
        public void PushMove(Vector3 move)
        {
            pushMoveDelta = move;
        }

        /// <summary>
        /// 外部トリガーでカメラ方向を回転させる
        /// </summary>
        public void PushRotate(float rotate)
        {
            pushRotateDelta = rotate;
        }

        /// <summary>
        /// フライスルーを初期化
        /// </summary>
        public void ResetFlyThrough()
        {
            ResetInput();

            MoveToFlyThrough(defaultPos);
            RotateToFlyThrough(defaultRot.eulerAngles.y);
        }
    }
}
