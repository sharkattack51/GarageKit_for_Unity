using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        void OnDestroy()
        {
            if(managedList.Contains(this))
                managedList.Remove(this);
        }

        public void On()
        {
            this.gameObject.SetActive(true);
        }

        public void Off()
        {
            this.gameObject.SetActive(false);
        }

        public static void AllOff()
        {
            foreach(StageManagedObject obj in managedList)
                obj.Off();
        }

        public static void ListOn(List<StageManagedObject> ons)
        {
            foreach(StageManagedObject obj in managedList)
            {
                if(ons.Contains(obj))
                    obj.On();
                else
                    obj.Off();
            }
        }

        public static void ListOn(StageManagedObject[] ons)
        {
            ListOn(ons.ToList<StageManagedObject>());
        }
    }
}
