using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarageKit
{
    public class PanoramaOrbit : MonoBehaviour
    {
        public enum OPERATION_MODE
        {
            ROTATE_Y = 0,
            ROTATE_XY
        };
        public OPERATION_MODE mode = OPERATION_MODE.ROTATE_Y;

        public float rotationSpeed = 0.1f;

        public bool invertY = true;
        public bool invertX = true;

        private float rotationX = 0.0f;
        private float rotationY = 0.0f;
        private Vector3 dragPrePosition = Vector3.zero;


        void Awake()
        {

        }

        void Start()
        {

        }

        void Update()
        {
            if(!Application.isEditor
                && (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android))
            {
                if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
                {
                    Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;

                    if(Input.deviceOrientation == DeviceOrientation.Portrait)
                    {
                        if(mode == OPERATION_MODE.ROTATE_XY)
                            rotationX += (touchDeltaPosition.y * rotationSpeed * (invertX ? -1.0f : 1.0f));
                        rotationY -= (touchDeltaPosition.x * rotationSpeed * (invertY ? -1.0f : 1.0f));
                    }
                    else if(Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown)
                    {
                        if(mode == OPERATION_MODE.ROTATE_XY)
                            rotationX -= (touchDeltaPosition.y * rotationSpeed * (invertX ? -1.0f : 1.0f));
                        rotationY += (touchDeltaPosition.x * rotationSpeed * (invertY ? -1.0f : 1.0f));
                    }
                    else if(Input.deviceOrientation == DeviceOrientation.LandscapeLeft)
                    {
                        if(mode == OPERATION_MODE.ROTATE_XY)
                            rotationX -= (touchDeltaPosition.x * rotationSpeed * (invertX ? -1.0f : 1.0f));
                        rotationY += (touchDeltaPosition.y * rotationSpeed * (invertY ? -1.0f : 1.0f));
                    }
                    else if(Input.deviceOrientation == DeviceOrientation.LandscapeRight)
                    {
                        if(mode == OPERATION_MODE.ROTATE_XY)
                            rotationX += (touchDeltaPosition.x * rotationSpeed * (invertX ? -1.0f : 1.0f));
                        rotationY -= (touchDeltaPosition.y * rotationSpeed * (invertY ? -1.0f : 1.0f));
                    }
                }
            }
            else
            {
                if(Input.GetMouseButtonDown(0))
                {
                    dragPrePosition = Input.mousePosition;
                    return;
                }
                else if(Input.GetMouseButton(0))
                {
                    Vector2 touchDeltaPosition = Input.mousePosition - dragPrePosition;
                    if(mode == OPERATION_MODE.ROTATE_XY)
                        rotationX += (touchDeltaPosition.y * rotationSpeed * (invertX ? -1.0f : 1.0f));
                    rotationY -= (touchDeltaPosition.x * rotationSpeed * (invertY ? -1.0f : 1.0f));

                    dragPrePosition = Input.mousePosition;
                }
            }

            rotationX = Mathf.Max(rotationX, -90.0f);
            rotationX = Mathf.Min(rotationX, 90.0f);
            this.gameObject.transform.localEulerAngles = new Vector3(rotationX, rotationY, 0.0f);
        }


        public void RotationReset()
        {
            rotationX = 0.0f;
            rotationY = 0.0f;
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
