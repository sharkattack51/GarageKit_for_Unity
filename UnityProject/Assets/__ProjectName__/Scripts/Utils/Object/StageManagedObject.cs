using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarageKit
{
    public class StageManagedObject : MonoBehaviour
    {
        private static List<StageManagedObject> managedList = new List<StageManagedObject>();


        void Awake()
        {
            managedList.Add(this);
        }

        void Start()
        {

        }

        void Update()
        {

        }


        public void On()
        {
            this.gameObject.SetActive(true);
        }

        public static void AllOff()
        {
            foreach(StageManagedObject obj in managedList)
                obj.gameObject.SetActive(false);
        }
    }
}
