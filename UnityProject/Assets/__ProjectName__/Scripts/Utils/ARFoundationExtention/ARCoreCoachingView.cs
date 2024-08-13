//#define USE_ARFOUNDATION

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if USE_ARFOUNDATION
using UnityEngine.XR.ARFoundation;
#endif

namespace GarageKit.ARFoundationExtention
{
    public class ARCoreCoachingView : MonoBehaviour
    {
#if USE_ARFOUNDATION
        public ARPlaneManager planeManager;
        public CanvasGroup view;

        private float targetAlpha = 0.0f;


        void Awake()
        {

        }

        void Start()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            ActivateCoaching();
#else
            DisableCoaching();
#endif
        }

        void Update()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            targetAlpha = (planeManager.trackables.count == 0) ? 1.0f : 0.0f;
            view.alpha += (targetAlpha - view.alpha) * 5.0f * Time.deltaTime;
#endif
        }


        public void ActivateCoaching()
        {
            view.gameObject.SetActive(true);
        }

        public void DisableCoaching()
        {
            view.gameObject.SetActive(false);
        }
#endif
    }
}
