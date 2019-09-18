using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace GarageKit
{
    public class ColliderHandler : MonoBehaviour
    {
        public Action<Collision> OnCollisionEnterHandler;
        public Action<Collision> OnCollisionExitHandler;
        public Action<Collision> OnCollisionStayHandler;
        public Action<Collider> OnTriggerEnterHandler;
        public Action<Collider> OnTriggerExitHandler;
        public Action<Collider> OnTriggerStayHandler;

        void OnCollisionEnter(Collision collision)
        {
            if(OnCollisionEnterHandler != null)
                OnCollisionEnterHandler(collision);
        }

        void OnCollisionExit(Collision collision)
        {
            if(OnCollisionExitHandler != null)
                OnCollisionExitHandler(collision);
        }

        void OnCollisionStay(Collision collision)
        {
            if(OnCollisionStayHandler != null)
                OnCollisionStayHandler(collision);
        }

        void OnTriggerEnter(Collider collider)
        {
            if(OnTriggerEnterHandler != null)
                OnTriggerEnterHandler(collider);
        }

        void OnTriggerExit(Collider collider)
        {
            if(OnTriggerExitHandler != null)
                OnTriggerExitHandler(collider);
        }

        void OnTriggerStay(Collider collider)
        {
            if(OnTriggerStayHandler != null)
                OnTriggerStayHandler(collider);
        }
    }
}
