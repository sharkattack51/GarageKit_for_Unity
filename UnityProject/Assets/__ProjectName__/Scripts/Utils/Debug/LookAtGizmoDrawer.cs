using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarageKit
{
	public class LookAtGizmoDrawer : MonoBehaviour
	{
		public Color color = Color.red;
		public GameObject target;
		public Vector3 worldUp = Vector3.up;

		void OnDrawGizmos()
		{
			//色を設定
			Gizmos.color = color;
			
			Draw();
		}
		
		void OnDrawGizmosSelected()
		{
			//色を設定
			Gizmos.color = Color.white;
			
			Draw();
		}

		private void Draw()
		{
			if(target == null)
				return;

			this.gameObject.transform.LookAt(target.transform, worldUp);
			Gizmos.DrawLine(
				this.gameObject.transform.position,
				target.transform.position);
		}
	}
}
