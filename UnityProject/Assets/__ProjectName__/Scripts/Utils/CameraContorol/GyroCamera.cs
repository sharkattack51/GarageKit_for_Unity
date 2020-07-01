using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarageKit
{
    public class GyroCamera : MonoBehaviour
    {
        public bool useCompassDirection = true;

        private Transform directionRoot;


        void Awake()
        {

        }

        void Start()
        {
            Input.gyro.enabled = true;

            if(useCompassDirection)
            {
                Input.compass.enabled = true;

                GameObject go = new GameObject("Direction Root");
                directionRoot = go.transform;
                directionRoot.position = this.gameObject.transform.position;
                directionRoot.rotation = this.gameObject.transform.rotation;
                directionRoot.parent = this.gameObject.transform.parent;
                this.gameObject.transform.parent = directionRoot;
            }

            Invoke("DelayedInit", 1.0f);		
        }

        private void DelayedInit()
        {
            if(useCompassDirection)
            {
#if UNITY_IOS
                directionRoot.rotation = Quaternion.Euler(new Vector3(0.0f, Input.compass.trueHeading, 0.0f));
#elif UNITY_ANDROID
                directionRoot.rotation = Quaternion.Euler(new Vector3(0.0f, Input.compass.trueHeading - 90.0f, 0.0f));
#endif
            }
        }

        void Update()
        {
            Quaternion gyro = Input.gyro.attitude;

#if UNITY_IOS
            this.transform.localRotation = new Quaternion(-gyro.x, -gyro.z, -gyro.y, gyro.w) * Quaternion.Euler(90.0f, 0.0f, 0.0f);
#elif UNITY_ANDROID
            this.transform.localRotation = Quaternion.Euler(90.0f, 0.0f, 0.0f) * new Quaternion(-gyro.x, -gyro.y, gyro.z, gyro.w);
#endif
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
