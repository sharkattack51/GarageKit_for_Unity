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
        }
#endif
    }
}
