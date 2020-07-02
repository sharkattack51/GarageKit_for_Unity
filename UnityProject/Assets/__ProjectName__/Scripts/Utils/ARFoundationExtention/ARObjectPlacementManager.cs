//#define USE_ARFOUNDATION

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if USE_ARFOUNDATION
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
#endif

namespace GarageKit.ARFoundationExtention
{
    public interface IPlacableObject
    {
        void OnPlace();
    }

    public class ARObjectPlacementManager : MonoBehaviour
    {
#if USE_ARFOUNDATION
        public ARRaycastManager raycastManager;

        [Header("instantiate or placement")]
        public bool isInstantiate = false;
        public GameObject instantiateRef;
        public GameObject placableObj;

        [Header("block input")]
        public RectTransform[] touchBlockers;

        private bool blocking = false;
        private bool rotStarted = false;
        private float preDeg;


        void Awake()
        {
        
        }

        void Start()
        {
            if(isInstantiate)
                placableObj = null;
        }

        void Update()
        {
            if(Input.touchCount == 0)
            {
                blocking = false;
                rotStarted = false;
            }
            else if(Input.touchCount == 1)
            {
                if(blocking)
                    return;

                if(Input.GetTouch(0).phase == TouchPhase.Began)
                    return;

                foreach(RectTransform rect in touchBlockers)
                {
                    if(RectTransformUtility.RectangleContainsScreenPoint(rect, Input.GetTouch(0).position))
                        return;
                }

                List<ARRaycastHit> hits = new List<ARRaycastHit>();
                if(raycastManager.Raycast(Input.GetTouch(0).position, hits, TrackableType.PlaneWithinPolygon))
                {
                    if(isInstantiate && instantiateRef != null && placableObj == null)
                        placableObj = Instantiate(instantiateRef) as GameObject;
                    
                    if(placableObj != null)
                    {
                        placableObj.transform.position = hits[0].pose.position;
                        placableObj.SendMessage("OnPlace");
                    }
                }
            }
            else if(Input.touchCount == 2)
            {
                blocking = true;

                Vector2 touch1Pos = Input.GetTouch(0).position;
                Vector2 touch2Pos = Input.GetTouch(1).position;

                if(!rotStarted)
                {
                    if(touch1Pos.x > touch2Pos.x)
                    {
                        Vector2 tmp = touch1Pos;
                        touch1Pos = touch2Pos;
                        touch2Pos = tmp;
                    }

                    rotStarted = true;
                }

                float dx = touch2Pos.x - touch1Pos.x;
                float dy = touch2Pos.y - touch1Pos.y;
                float deg = Mathf.Atan2(dy, dx)* Mathf.Rad2Deg;

                float rot = 0.0f;
                if(deg - preDeg > 0.0f)
                    rot = deg;
                else if(deg - preDeg < 0.0f)
                    rot = -deg;

                if(placableObj != null)
                    placableObj.transform.Rotate(Vector3.up, rot * 0.01f);

                preDeg = deg;
            }
        }
#endif
    }
}
