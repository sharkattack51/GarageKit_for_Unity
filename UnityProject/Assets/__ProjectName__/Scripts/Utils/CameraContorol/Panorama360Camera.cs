using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarageKit
{
    public class Panorama360Camera : MonoBehaviour
    {
        public enum OPERATION_MODE
        {
            GYRO = 0,
            DRAG_MOUSE,
            DRAG_TOUCH,
            GAME_PAD
        };
        public	OPERATION_MODE mode = OPERATION_MODE.GYRO;

        public float rotationSpeed = 0.1f;
        private float rotationX = 0.0f;
        private float rotationY = 0.0f;

        [Header("Rotate Limit")]
        public bool useLimit = false;
        public float limitRotL = 0.0f;
        public float limitRotR = 0.0f;

        [Header("Gamepad")]
        public bool invertRotH = false;
        public bool invertRotV = false;

        private	float appliedGyroYAngle = 0.0f;
        private	float calibrationYAngle = 0.0f;
        private	bool calibrated = false;

        private Vector3 dragPrePosition = Vector3.zero;

        [Header("Smooth")]
        public bool useSmooth = false;
        public float smoothTime = 0.1f;
        private float smoothVelX;
        private float smoothVelY;


        void Awake()
        {

        }

        void Start()
        {
            if(mode == OPERATION_MODE.GYRO)
            {
                if(SystemInfo.supportsGyroscope)
                    Input.gyro.enabled = true;
                else
                    Debug.LogWarning("Panorama360Camera :: not supported gyro.");
            }
        }

        void Update()
        {
            float nowY = this.gameObject.transform.localEulerAngles.y;
            float nowTY = rotationY;

            if(mode == OPERATION_MODE.GYRO)
            {
                float gyroMagnitude = new Vector3(Input.gyro.attitude.x, Input.gyro.attitude.y, Input.gyro.attitude.z).magnitude;
                if(gyroMagnitude < 0.0001f)
                    return;

                this.gameObject.transform.rotation = Input.gyro.attitude;
                this.gameObject.transform.Rotate(0f, 0f, 180f, Space.Self);
                this.gameObject.transform.Rotate(90f, 180f, 0f, Space.World);

                appliedGyroYAngle = this.gameObject.transform.eulerAngles.y;

                if(!calibrated)
                    CalibrateYAngle();

                this.gameObject.transform.Rotate(0.0f, -this.calibrationYAngle, 0.0f, Space.World);
                this.gameObject.transform.localEulerAngles = this.gameObject.transform.eulerAngles;
            }
            else if(mode == OPERATION_MODE.DRAG_MOUSE)
            {
                if(Input.GetMouseButtonDown(0))
                {
                    dragPrePosition = Input.mousePosition;
                    return;
                }
                else if(Input.GetMouseButton(0))
                {
                    Vector2 touchDeltaPosition = Input.mousePosition - dragPrePosition;
                    rotationX += (touchDeltaPosition.y * rotationSpeed);
                    rotationY -= (touchDeltaPosition.x * rotationSpeed);

                    rotationX = Mathf.Max(rotationX, -90.0f);
                    rotationX = Mathf.Min(rotationX, 90.0f);

                    if(!useSmooth)
                        this.gameObject.transform.localEulerAngles = new Vector3(rotationX, rotationY, 0.0f);

                    dragPrePosition = Input.mousePosition;
                }
            }
            else if(mode == OPERATION_MODE.DRAG_TOUCH)
            {
                if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
                {
                    Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
                    rotationX += (touchDeltaPosition.y * rotationSpeed);
                    rotationY -= (touchDeltaPosition.x * rotationSpeed);

                    rotationX = Mathf.Max(rotationX, -90.0f);
                    rotationX = Mathf.Min(rotationX, 90.0f);

                    if(!useSmooth)
                        this.gameObject.transform.localEulerAngles = new Vector3(rotationX, rotationY, 0.0f);
                }
            }
            else if(mode == OPERATION_MODE.GAME_PAD)
            {
                float rotH = Input.GetAxis("Horizontal") * (invertRotH ? -1.0f : 1.0f);
                float rotV = Input.GetAxis("Vertical") * (invertRotV ? -1.0f : 1.0f);
                SetRotate(rotH, rotV, rotationSpeed);
            }

            if(mode != OPERATION_MODE.GYRO && useSmooth)
            {
                this.gameObject.transform.localEulerAngles = new Vector3(
                    Mathf.SmoothDampAngle(this.gameObject.transform.localEulerAngles.x, rotationX, ref smoothVelX, smoothTime),
                    Mathf.SmoothDampAngle(this.gameObject.transform.localEulerAngles.y, rotationY, ref smoothVelY, smoothTime),
                    0.0f);
            }

            if(useLimit)
            {
                float nxtY = this.gameObject.transform.localEulerAngles.y;
                float rotVal = Mathf.DeltaAngle(nowY, nxtY);

                if(rotVal != 0.0f)
                {
                    if(0.0f < rotVal)
                    {
                        // 右回り中
                        if(0.0f < nowY && nowY < 180.0f)
                            nxtY = Mathf.Min(nxtY, limitRotR);
                        if(180.0f <= nowY && nowY < limitRotL)
                            nxtY = limitRotL;
                    }
                    else
                    {
                        // 左回り中
                        if(180.0f < nowY && nowY < 360.0f)
                            nxtY = Mathf.Max(nxtY, limitRotL);
                        if(nowY <= 180.0f && limitRotR < nowY)
                            nxtY = limitRotR;
                    }

                    if(mode != OPERATION_MODE.GYRO)
                    {
                        if(nxtY != this.gameObject.transform.localEulerAngles.y)
                            rotationY = nowTY;
                    }

                    this.gameObject.transform.localEulerAngles = new Vector3(
                        this.gameObject.transform.localEulerAngles.x, nxtY, 0.0f);
                }
            }
        }


        private void CalibrateYAngle()
        {
            calibrationYAngle = appliedGyroYAngle;
            calibrated = true;
        }

        public void ResetGyro()
        {
            rotationX = 0.0f;
            rotationY = 0.0f;

            this.gameObject.transform.localEulerAngles = new Vector3(rotationX, rotationY, 0.0f);

            CalibrateYAngle();
        }

        public void ResetRotation(bool force = false)
        {
            appliedGyroYAngle = 0.0f;
            calibrationYAngle = 0.0f;
            calibrated = false;

            rotationX = 0.0f;
            rotationY = 0.0f;

            if(force)
                this.gameObject.transform.localRotation = Quaternion.identity;
        }

        public void SetRotate(float h, float v, float speed)
        {
            rotationX += (v * rotationSpeed * speed);
            rotationY -= (h * rotationSpeed * speed);

            rotationX = Mathf.Max(rotationX, -90.0f);
            rotationX = Mathf.Min(rotationX, 90.0f);

            this.gameObject.transform.localEulerAngles = new Vector3(rotationX, rotationY, 0.0f);
        }
    }
}
