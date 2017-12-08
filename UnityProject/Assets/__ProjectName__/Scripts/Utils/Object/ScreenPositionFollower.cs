using UnityEngine;
using System.Collections;

/*
 * ターゲットオブジェクトのスクリーン位置に一致させる
 */
namespace GarageKit
{
	public class ScreenPositionFollower : MonoBehaviour
	{
		public GameObject targetObject;
		public int screenWidth = 1920;
		public int screenHeight = 1080;
		public float uiScale = 1.0f;
		public Camera rayCamera;
		
		
		void Awake()
		{
			SetFollowScreenPosition();
		}
		
		void Start()
		{
			
		}
		
		void Update()
		{
			SetFollowScreenPosition();
		}
		
		
		//スクリーン位置を設定
		private void SetFollowScreenPosition()
		{
			if(targetObject != null)
			{
				Vector3 viewportPoint = rayCamera.WorldToViewportPoint(targetObject.transform.position);
				Vector3 screenPosition = new Vector3(
					screenWidth / 2.0f * (viewportPoint.x - 0.5f) * uiScale,
					screenHeight / 2.0f * (viewportPoint.y - 0.5f) * uiScale,
					0.0f);
				this.gameObject.transform.localPosition = screenPosition / 100.0f;
			}
		}
	}
}
