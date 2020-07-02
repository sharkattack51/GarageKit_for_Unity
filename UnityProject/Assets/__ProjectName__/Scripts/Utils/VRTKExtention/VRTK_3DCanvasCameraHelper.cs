//#define USE_VRTK

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace GarageKit.VRTKExtention
{
    public class VRTK_3DCanvasCameraHelper : MonoBehaviour
    {
#if USE_VRTK
        private Canvas ui3DCanvas;
        private VRTK_OnPreRenderActions onPreRenderActions;
        

        void Awake()
        {

        }

        void Start()
        {
            ui3DCanvas = this.gameObject.GetComponent<Canvas>();
        }

        void Update()
        {
            if(!VRTKUtil_Alias.Instance.AllDeviceIsReady())
                return;
            
            if(ui3DCanvas.worldCamera == null && Camera.main != null)
                ui3DCanvas.worldCamera = Camera.main;

            if(onPreRenderActions == null)
            {
                onPreRenderActions = Camera.main.GetComponent<VRTK_OnPreRenderActions>();
                onPreRenderActions.actions.Add(new Action(() => {
                    this.gameObject.transform.position = Camera.main.transform.position;
                }));
            }
        }
#endif
    }
}
