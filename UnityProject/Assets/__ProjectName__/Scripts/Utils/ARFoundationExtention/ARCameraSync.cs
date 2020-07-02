//#define USE_ARFOUNDATION

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if USE_ARFOUNDATION
using UnityEngine.XR.ARFoundation;
#endif

namespace GarageKit.ARFoundationExtention
{
    [RequireComponent(typeof(Camera))]
    public class ARCameraSync : MonoBehaviour
    {
#if USE_ARFOUNDATION
        public ARCameraManager arCameraManager;
        public bool syncFov = true;
        public bool syncPosition = true;
        public bool syncRotation = true;
        public bool asLocal = false;

        private Camera targetCamera;
        private Camera arCamera;


        void Awake()
        {

        }

        void Start()
        {
            targetCamera = this.gameObject.GetComponent<Camera>();
            arCamera = arCameraManager.GetComponent<Camera>();
        }

        void LateUpdate()
        {
            if(arCameraManager == null || targetCamera == null || arCamera == null)
                return;
            
            if(syncFov)
                targetCamera.projectionMatrix = arCamera.projectionMatrix;

            if(syncPosition)
            {
                if(asLocal)
                    targetCamera.transform.localPosition = arCamera.transform.localPosition;
                else
                    targetCamera.transform.position = arCamera.transform.position;
            }

            if(syncRotation)
            {
                if(asLocal)
                    targetCamera.transform.localRotation = arCamera.transform.localRotation;
                else
                    targetCamera.transform.rotation = arCamera.transform.rotation;
            }
        }
#endif
    }
}
