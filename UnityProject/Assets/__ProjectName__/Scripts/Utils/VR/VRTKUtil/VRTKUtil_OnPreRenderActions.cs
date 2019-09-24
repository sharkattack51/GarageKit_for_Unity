//#define USE_VRTK

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace GarageKit
{
    public class VRTKUtil_OnPreRenderActions : MonoBehaviour
    {
#if USE_VRTK
        public List<Action> actions;


        void Awake()
        {
            actions = new List<Action>();
        }

        void Start()
        {
            
        }

        void OnPreRender()
        {
            foreach(Action action in actions)
            {
                if(action != null)
                    action();
            }
        }
#endif
    }
}
