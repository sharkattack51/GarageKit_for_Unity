using UnityEngine;
using System.Collections;

/*
 * ターゲットオブジェクトのスクリーン位置に一致させる
 */

public class ScreenPositionFollower : MonoBehaviour
{
	public GameObject targetObject; 
	
	private Camera renderCamera;
	
	
	void Awake()
	{
		//レンダーカメラを取得
		renderCamera = Utils.CameraUtil.FindCameraForLayer(targetObject.layer);
		
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
			Vector3 screenPosition = renderCamera.WorldToScreenPoint(targetObject.transform.position);
			Vector3 halfScreen = new Vector3(1920 / 2.0f, 1080 / 2.0f, 0.0f);
			this.gameObject.transform.localPosition = (screenPosition - halfScreen) / 100.0f;
		}
	}
}
