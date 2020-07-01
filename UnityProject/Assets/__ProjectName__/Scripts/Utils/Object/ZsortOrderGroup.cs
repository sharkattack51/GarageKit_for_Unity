using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarageKit
{
    public class ZsortOrderGroup : MonoBehaviour
    {
        public Transform distanceTarget;
        public bool isReverse = false;


        void Awake()
        {

        }

        void Start()
        {

        }

        void Update()
        {
            if(distanceTarget == null)
                distanceTarget = Camera.main.transform;

            List<Transform> children = new List<Transform>();
            for(int i = 0; i < this.gameObject.transform.childCount; i++)
                children.Add(this.gameObject.transform.GetChild(i));

            children.Sort((a, b) => {
                if(!isReverse)
                    return (int)(Vector3.Distance(distanceTarget.position, b.position) - Vector3.Distance(distanceTarget.position, a.position));
                else
                    return (int)(Vector3.Distance(distanceTarget.position, a.position) - Vector3.Distance(distanceTarget.position, b.position));
            });

            for(int i = 0; i < children.Count; i++)
                children[i].SetSiblingIndex(i);
        }
    }
}
