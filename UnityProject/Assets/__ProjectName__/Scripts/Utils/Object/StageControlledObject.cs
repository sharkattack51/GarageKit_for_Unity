using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GarageKit
{
	public class StageControlledObject : MonoBehaviour
	{
		private static List<StageControlledObject> controlledList = new List<StageControlledObject>();


		void Awake()
		{
			controlledList.Add(this);
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
			foreach(StageControlledObject obj in controlledList)
				obj.gameObject.SetActive(false);
		}
	}
}
