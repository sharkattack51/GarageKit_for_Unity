using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarageKit
{
    [RequireComponent(typeof(Camera))]
    public class OnRenderHandler : MonoBehaviour
    {
        public Action OnPreRenderAction;
        public Action OnPostRenderAction;

        void OnPreRender()
        {
            if(OnPreRenderAction != null)
                OnPreRenderAction();
        }

        void OnPostRender()
        {
            if(OnPostRenderAction != null)
                OnPostRenderAction();
        }
    }
}
