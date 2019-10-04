using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace GarageKit
{
    [RequireComponent(typeof(Collider))]
    public class OnCollisionHandler : MonoBehaviour
    {
        public Action<Collision> OnCollisionEnterAction;
        public Action<Collision> OnCollisionExitAction;
        public Action<Collision> OnCollisionStayAction;
        public Action<Collider> OnTriggerEnterAction;
        public Action<Collider> OnTriggerExitAction;
        public Action<Collider> OnTriggerStayAction;

        void OnCollisionEnter(Collision collision)
        {
            if(OnCollisionEnterAction != null)
                OnCollisionEnterAction(collision);
        }

        void OnCollisionExit(Collision collision)
        {
            if(OnCollisionExitAction != null)
                OnCollisionExitAction(collision);
        }

        void OnCollisionStay(Collision collision)
        {
            if(OnCollisionStayAction != null)
                OnCollisionStayAction(collision);
        }

        void OnTriggerEnter(Collider collider)
        {
            if(OnTriggerEnterAction != null)
                OnTriggerEnterAction(collider);
        }

        void OnTriggerExit(Collider collider)
        {
            if(OnTriggerExitAction != null)
                OnTriggerExitAction(collider);
        }

        void OnTriggerStay(Collider collider)
        {
            if(OnTriggerStayAction != null)
                OnTriggerStayAction(collider);
        }
    }
}
