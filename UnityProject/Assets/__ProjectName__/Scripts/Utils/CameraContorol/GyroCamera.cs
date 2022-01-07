//#define USE_GYRODOROID

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_ANDROID && USE_GYRODOROID
using GyroDroid;
#endif

namespace GarageKit
{
    public class GyroCamera : MonoBehaviour
    {
        public enum NORTH_DIRECTION
        {
            NONE = 0,
            COMPASS_NORTH,
            GYRO_HEAD_DIRECTION
        }
        public NORTH_DIRECTION northDirection = NORTH_DIRECTION.GYRO_HEAD_DIRECTION;

        private bool ready = false;
        public bool IsReady { get{ return ready; } }

        public Action OnReady;

        private Transform directionRoot;


        void Awake()
        {

        }

        void Start()
        {
            Input.gyro.enabled = false;
            Input.gyro.enabled = true;

            if(northDirection == NORTH_DIRECTION.COMPASS_NORTH)
                Input.compass.enabled = true;

#if UNITY_ANDROID && USE_GYRODOROID
            SensorHelper.DeactivateRotation();
            SensorHelper.ActivateRotation();
#endif

            // 基準方向用に親を設定
            if(northDirection != NORTH_DIRECTION.NONE)
            {
                GameObject go = new GameObject("Direction Root");
                directionRoot = go.transform;
                directionRoot.position = this.gameObject.transform.position;
                directionRoot.rotation = this.gameObject.transform.rotation;
                directionRoot.parent = this.gameObject.transform.parent;
                this.gameObject.transform.SetParent(directionRoot);
            }

            // センサーの初期化完了を待つために遅延実行
            Invoke("LazyInit", 1.0f);		
        }

        private void LazyInit()
        {
            // コンパス北方向で初期化
            if(northDirection == NORTH_DIRECTION.COMPASS_NORTH)
            {
#if UNITY_IOS
                directionRoot.rotation = Quaternion.Euler(0.0f, Input.compass.trueHeading, 0.0f);
#else
                directionRoot.rotation = Quaternion.Euler(0.0f, Input.compass.trueHeading - 90.0f, 0.0f);
#endif
            }

            // ジャイロ初期方向で初期化
            else if(northDirection == NORTH_DIRECTION.GYRO_HEAD_DIRECTION)
            {
                Quaternion gyro = Input.gyro.attitude;
                Quaternion quat = Quaternion.identity;

#if UNITY_IOS
                quat = new Quaternion(-gyro.x, -gyro.z, -gyro.y, gyro.w) * Quaternion.Euler(90.0f, 0.0f, 0.0f);
#elif UNITY_ANDROID && USE_GYRODOROID
                quat = SensorHelper.rotation;
#else
                quat = Quaternion.Euler(90.0f, 0.0f, 0.0f) * new Quaternion(-gyro.x, -gyro.y, gyro.z, gyro.w);
#endif

                directionRoot.rotation = Quaternion.Euler(0.0f, -quat.eulerAngles.y, 0.0f);
            }

            ready = true;
            OnReady?.Invoke();
        }

        void Update()
        {
            Quaternion gyro = Input.gyro.attitude;
            Quaternion quat = Quaternion.identity;

#if UNITY_IOS
            quat = new Quaternion(-gyro.x, -gyro.z, -gyro.y, gyro.w) * Quaternion.Euler(90.0f, 0.0f, 0.0f);
#elif UNITY_ANDROID && USE_GYRODOROID
            quat = SensorHelper.rotation;
#else
            quat = Quaternion.Euler(90.0f, 0.0f, 0.0f) * new Quaternion(-gyro.x, -gyro.y, gyro.z, gyro.w);
#endif

            this.transform.localRotation = quat;
        }
        

        public void CalibrateNorth()
        {
            directionRoot.rotation = Quaternion.Euler(new Vector3(0.0f, -this.transform.localRotation.eulerAngles.y, 0.0f));
        }

        public void SetFov(float fov)
        {
            this.gameObject.GetComponent<Camera>().fieldOfView = fov;
        }
    }
}
