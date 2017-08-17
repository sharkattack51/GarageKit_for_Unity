using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/*
 * マルチタッチ操作でのカメラのピンチズームコントロールクラス
 * マウス&タッチ対応
 */

// 最大最上の制限値
[Serializable]
public class LimitValue
{
	public float min;
	public float max;
	
	public LimitValue(float minValue, float maxValue)
	{
		min = minValue;
		max = maxValue;
	}
}

[RequireComponent(typeof(Camera))] 
public class PinchZoomCamera : MonoBehaviour
{
	public static bool winTouch = false;
	public static bool updateEnable = true;

	// カメラのズームタイプ
	public enum PINCH_ZOOM_TYPE
	{
		POSITION_Z = 0,
		FOV,
		ORTHOSIZE
	}
	
	public PINCH_ZOOM_TYPE zoomType = PINCH_ZOOM_TYPE.POSITION_Z;
	
	public float zoomBias = 1.0f;
	public float zoomSmoothTime = 0.1f;
	public bool invertZoom = false;
	public LimitValue limitMinMaxForRelativePosZ = new LimitValue(-1000.0f, 1000.0f); // Z位置の相対値で制限
	public LimitValue limitMinMaxForFOV = new LimitValue(30.0f, 80.0f); // FOV値で制限
	public LimitValue limitMinMaxForOrthoSize = new LimitValue(2.7f, 5.4f); // オルソサイズで制限
	public MonoBehaviour[] disableComponents; // ピンチズーム操作時に動作をOFFにする連携コンポーネント
	public bool zoomToPinchCenterFor2D = false;
	
	private GameObject pinchZoomRoot;

	private FlyThroughCamera flyThroughCamera;
	private OrbitCamera orbitCamera;

	private bool inputLock;
	public bool IsInputLock { get{ return inputLock; } }
	private object lockObject;
	
	private float defaultZoom;
	
	private float velocitySmoothZoom;
	private float dampZoomDelta = 0.0f;
	private float pushZoomDelta = 0.0f;
	
	private bool isFirstTouch = true;
	private float oldDistance;
	private float currentDistance;
	private float calcZoom;
	private Vector2 pinchCenter;
	private List<CameraShifter> cameraShifterListForZtoPC = new List<CameraShifter>();

	public float ratioForWheel = 1.0f;
	
	public float currentZoom
	{
		get
		{
			if(zoomType == PINCH_ZOOM_TYPE.POSITION_Z)
				return this.gameObject.transform.localPosition.z - defaultZoom;
			else if(zoomType == PINCH_ZOOM_TYPE.FOV)
				return this.gameObject.GetComponent<Camera>().fieldOfView;
			else if(zoomType == PINCH_ZOOM_TYPE.ORTHOSIZE)
				return this.gameObject.GetComponent<Camera>().orthographicSize;
			else
				return float.NegativeInfinity;
		}
	}
	
	
	void Awake()
	{

	}

	IEnumerator Start()
	{
		// 設定ファイルより入力タイプを取得
		if(!ApplicationSetting.Instance.GetBool("UseMouse"))
			winTouch = true;
		
		Vector3 temp_pos = this.gameObject.transform.position;
		Quaternion temp_rot = this.gameObject.transform.rotation;
		Vector3 temp_scale = this.gameObject.transform.localScale;

		yield return new WaitForEndOfFrame();

		// カメラコンポーネントの取得
		flyThroughCamera = this.gameObject.GetComponent<FlyThroughCamera>();
		orbitCamera = this.gameObject.GetComponent<OrbitCamera>();

		// ズーム位置のルートを設定する
		pinchZoomRoot = new GameObject(this.gameObject.name + " PinchZoom Root");
		pinchZoomRoot.transform.SetParent(this.gameObject.transform.parent, true);
		pinchZoomRoot.transform.position = temp_pos;
		pinchZoomRoot.transform.rotation = temp_rot;
		pinchZoomRoot.transform.localScale = temp_scale;
		this.gameObject.transform.SetParent(pinchZoomRoot.transform, true);
		this.gameObject.transform.localPosition = Vector3.zero;
		this.gameObject.transform.localRotation = Quaternion.identity;
		this.gameObject.transform.localScale = Vector3.one;

		// 初期値を保存
		switch(zoomType)
		{
			case PINCH_ZOOM_TYPE.POSITION_Z: defaultZoom = this.gameObject.transform.localPosition.z; break;
			case PINCH_ZOOM_TYPE.FOV: defaultZoom = this.gameObject.GetComponent<Camera>().fieldOfView; break;
			case PINCH_ZOOM_TYPE.ORTHOSIZE: defaultZoom = this.gameObject.GetComponent<Camera>().orthographicSize; break;
			default: break;
		}
		
		// ピンチセンターへのズーム設定
		if(zoomToPinchCenterFor2D)
		{
			if(zoomType != PINCH_ZOOM_TYPE.POSITION_Z)
				Debug.LogWarning("PinchZoomCamera :: [zoomToPinchCenter] only works for [PINCH_ZOOM_TYPE.POSITION_Z]");

			Camera[] cameras = this.gameObject.GetComponent<Camera>().GetComponentsInChildren<Camera>();
			foreach(Camera cam in cameras)
			{
				CameraShifter cameraShifter = cam.gameObject.GetComponent<CameraShifter>();
				if(cameraShifter == null)
					cameraShifter = cam.gameObject.AddComponent<CameraShifter>();
				cameraShifter.calcAlways = true;
				
				cameraShifterListForZtoPC.Add(cameraShifter);
			}
		}
		
		ResetInput();
	}
	
	void Update()
	{
		if(!inputLock && ButtonObjectBase.PressBtnsTotal == 0)
			GetInput();
		else
			ResetInput();
		
		UpdateZoom();
	}
	
	
	private void UpdateZoom()
	{
		if(!updateEnable)
			return;
		
		switch(zoomType)
		{
			case PINCH_ZOOM_TYPE.POSITION_Z: UpdatePinchZoomPositionZ(); break;
			case PINCH_ZOOM_TYPE.FOV: UpdatePinchZoomFOV(); break;
			case PINCH_ZOOM_TYPE.ORTHOSIZE: UpdatePinchZoomOrthoSize(); break;
			default: break;
		}
	}
	
	private void ResetInput()
	{
		// ピンチセンターへのズーム設定をリセット
		if(!isFirstTouch && zoomToPinchCenterFor2D)
		{
			if(flyThroughCamera != null)
			{
				Ray ray = this.gameObject.GetComponent<Camera>().ScreenPointToRay(new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, 0.0f));
				RaycastHit hitInfo;
				
				if(flyThroughCamera.groundCollider.Raycast(ray, out hitInfo, float.PositiveInfinity))
				{
					// カメラ位置
					flyThroughCamera.TranslateToFlyThrough(hitInfo.point - flyThroughCamera.currentPos);
					flyThroughCamera.ShiftTransform.localPosition = Vector3.zero;
					
					// カメラシフト
					foreach(CameraShifter cameraShifter in cameraShifterListForZtoPC)
					{
						cameraShifter.shiftX = 0.0f;
						cameraShifter.shiftY = 0.0f;
					}
				}
			}
		}
		
		isFirstTouch = true;
		oldDistance = 0.0f;
		currentDistance = 0.0f;
		calcZoom = 0.0f;
		velocitySmoothZoom = 0.0f;
		dampZoomDelta = 0.0f;
		pushZoomDelta = 0.0f;
		pinchCenter = Vector2.zero;
		
		// カメラ操作のアンロック
		if(flyThroughCamera != null) flyThroughCamera.UnlockInput(this.gameObject);
		if(orbitCamera != null) orbitCamera.UnlockInput(this.gameObject);
		
		// 連携コンポーネントをON
		foreach(MonoBehaviour component in disableComponents)
		{
			if(component != null)
				component.enabled = true;
		}
	}
	
	private void GetInput()
	{
		// for Touch
		if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
		{
			if(Input.touchCount == 2)
			{
				// カメラ操作のロック
				if(flyThroughCamera != null) flyThroughCamera.LockInput(this.gameObject);
				if(orbitCamera != null) orbitCamera.LockInput(this.gameObject);

				// 連携コンポーネントをOFF
				foreach(MonoBehaviour component in disableComponents)
				{
					if(component != null)
						component.enabled = false;
				}
				
				// ピンチセンターを設定
				pinchCenter = (Input.GetTouch(0).position + Input.GetTouch(1).position) / 2.0f;
				
				// ピンチ距離を計算
				currentDistance = Vector3.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
				
				if(isFirstTouch)
				{
					// ピンチセンターへのズーム開始
					if(zoomToPinchCenterFor2D)
						StartZoomToPinchCenter();
					
					oldDistance = currentDistance;
					isFirstTouch = false;
					return;
				}
				
				calcZoom = currentDistance - oldDistance;
				oldDistance = currentDistance;
			}
			else
				ResetInput();
		}

#if UNITY_STANDALONE_WIN
		else if(Application.platform == RuntimePlatform.WindowsPlayer && winTouch)
		{
			if(TouchScript.TouchManager.Instance.PressedPointersCount == 2)
			{
				// カメラ操作のロック
				if(flyThroughCamera != null) flyThroughCamera.LockInput(this.gameObject);
				if(orbitCamera != null) orbitCamera.LockInput(this.gameObject);

				// 連携コンポーネントをOFF
				foreach(MonoBehaviour component in disableComponents)
				{
					if(component != null)
						component.enabled = false;
				}
				
				// ピンチセンターを設定
				pinchCenter = (
					TouchScript.TouchManager.Instance.PressedPointers[0].Position + TouchScript.TouchManager.Instance.PressedPointers[1].Position) / 2.0f;
				
				// ピンチ距離を計算
				currentDistance = Vector3.Distance(
					TouchScript.TouchManager.Instance.PressedPointers[0].Position,
					TouchScript.TouchManager.Instance.PressedPointers[1].Position);
				
				if(isFirstTouch)
				{
					// ピンチセンターへのズーム開始
					if(zoomToPinchCenterFor2D)
						StartZoomToPinchCenter();
					
					oldDistance = currentDistance;
					isFirstTouch = false;
					return;
				}
				
				calcZoom = currentDistance - oldDistance;
				oldDistance = currentDistance;
			}
			else
				ResetInput();
		}
#endif

		// for Mouse
		else
		{
			if((Input.GetMouseButton(0) && (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)))
				|| Input.mouseScrollDelta.y != 0.0f)
			{
				// カメラ操作のロック
				if(flyThroughCamera != null) flyThroughCamera.LockInput(this.gameObject);
				if(orbitCamera != null) orbitCamera.LockInput(this.gameObject);
				
				// 連携コンポーネントをOFF
				foreach(MonoBehaviour component in disableComponents)
				{
					if(component != null)
						component.enabled = false;
				}

				if(Input.mouseScrollDelta.y == 0.0f)
				{
					// ドラッグでの操作
					
					// ピンチセンターを設定
					if(isFirstTouch) pinchCenter = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
					
					// ピンチ距離を計算
					Vector2 orgPoint = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
					Vector2 diff = orgPoint - pinchCenter;
					Vector2 mirrorPoint = new Vector2(
						(diff.x * Mathf.Cos(180 * Mathf.Deg2Rad)) - (diff.y * Mathf.Sin(180 * Mathf.Deg2Rad)),
						(diff.x * Mathf.Sin(180 * Mathf.Deg2Rad)) + (diff.y * Mathf.Cos(180 * Mathf.Deg2Rad))
					);
					currentDistance = Vector3.Distance(orgPoint, mirrorPoint);
					
					if(isFirstTouch)
					{
						// ピンチセンターへのズーム開始
						if(zoomToPinchCenterFor2D)
							StartZoomToPinchCenter();
						
						oldDistance = currentDistance;
						isFirstTouch = false;
						return;
					}
					
					calcZoom = currentDistance - oldDistance;
					oldDistance = currentDistance;
				}
				else
				{
					// ホイールでの操作
					calcZoom = Input.mouseScrollDelta.y * ratioForWheel;
				}
			}
			else
				ResetInput();	
		}
	}
	
	/// <summary>
	/// ピンチセンターへのズーム開始
	/// </summary>
	private void StartZoomToPinchCenter()
	{
		if(flyThroughCamera != null)
		{
			Ray ray = this.GetComponent<Camera>().ScreenPointToRay(pinchCenter);
			RaycastHit hitInfo;
			if(flyThroughCamera.groundCollider.Raycast(ray, out hitInfo, float.PositiveInfinity))
			{
				// カメラ位置
				Vector3 shift = hitInfo.point - flyThroughCamera.FlyThroughRoot.transform.position;
				flyThroughCamera.ShiftTransform.localPosition = shift;
				
				// カメラシフト
				foreach(CameraShifter cameraShifter in cameraShifterListForZtoPC)
				{
					cameraShifter.shiftX = ((pinchCenter.x / (float)Screen.width) - 0.5f) * 2.0f;
					cameraShifter.shiftY = ((pinchCenter.y / (float)Screen.height) - 0.5f) * 2.0f;
				}
			}
		}
		else
			Debug.LogWarning("PinchiZoomCamera::UpdateZoomToCenter() is need a FlyThroughCamera in this scene.");
	}
	
	
	/// <summary>
	/// Input更新のLock
	/// </summary>
	public void LockInput(object sender)
	{
		if(!inputLock)
		{
			lockObject = sender;
			inputLock = true;
		}
	}
	
	/// <summary>
	/// Input更新のUnLock
	/// </summary>
	public void UnlockInput(object sender)
	{
		if(inputLock && lockObject == sender)
			inputLock = false;
	}
	
	/// <summary>
	/// カメラのZ位置を更新
	/// </summary>
	private void UpdatePinchZoomPositionZ()
	{
		float zoomDelta = (calcZoom + pushZoomDelta) * (invertZoom ? -1.0f : 1.0f) * zoomBias;
		dampZoomDelta = Mathf.SmoothDamp(dampZoomDelta, zoomDelta, ref velocitySmoothZoom, zoomSmoothTime);
		this.gameObject.transform.Translate(new Vector3(0.0f, 0.0f, dampZoomDelta), Space.Self);
		
		// 位置制限
		if(this.gameObject.transform.localPosition.z <= defaultZoom + limitMinMaxForRelativePosZ.min)
			this.gameObject.transform.localPosition = new Vector3(0.0f, 0.0f, defaultZoom + limitMinMaxForRelativePosZ.min);
		if(this.gameObject.transform.localPosition.z >= defaultZoom + limitMinMaxForRelativePosZ.max)
			this.gameObject.transform.localPosition = new Vector3(0.0f, 0.0f, defaultZoom + limitMinMaxForRelativePosZ.max);
		
		pushZoomDelta = 0.0f;
	}
	
	/// <summary>
	/// カメラのFOVを更新
	/// </summary>
	private void UpdatePinchZoomFOV()
	{
		float zoomDelta = (calcZoom + pushZoomDelta) * (invertZoom ? -1.0f : 1.0f) * zoomBias;
		dampZoomDelta = Mathf.SmoothDamp(dampZoomDelta, zoomDelta, ref velocitySmoothZoom, zoomSmoothTime);
		this.gameObject.GetComponent<Camera>().fieldOfView += dampZoomDelta;
		
		// 画角制限
		if(this.gameObject.GetComponent<Camera>().fieldOfView <= limitMinMaxForFOV.min)
			this.gameObject.GetComponent<Camera>().fieldOfView = limitMinMaxForFOV.min;
		if(this.gameObject.GetComponent<Camera>().fieldOfView >= limitMinMaxForFOV.max)
			this.gameObject.GetComponent<Camera>().fieldOfView = limitMinMaxForFOV.max;
		
		pushZoomDelta = 0.0f;
	}
	
	/// <summary>
	/// カメラのOrthoSizeを更新
	/// </summary>
	private void UpdatePinchZoomOrthoSize()
	{
		float zoomDelta = (calcZoom + pushZoomDelta) * (invertZoom ? -1.0f : 1.0f) * zoomBias;
		dampZoomDelta = Mathf.SmoothDamp(dampZoomDelta, zoomDelta, ref velocitySmoothZoom, zoomSmoothTime);
		this.gameObject.GetComponent<Camera>().orthographicSize += dampZoomDelta;
		
		// Orthoサイズ制限
		if(this.gameObject.GetComponent<Camera>().orthographicSize <= limitMinMaxForOrthoSize.min)
			this.gameObject.GetComponent<Camera>().orthographicSize = limitMinMaxForOrthoSize.min;
		if(this.gameObject.GetComponent<Camera>().orthographicSize >= limitMinMaxForOrthoSize.max)
			this.gameObject.GetComponent<Camera>().orthographicSize = limitMinMaxForOrthoSize.max;
		
		pushZoomDelta = 0.0f;
	}
	
	/// <summary>
	/// 外部トリガーでズームする
	/// </summary>
	public void PushZoom(float zoomDelta)
	{
		pushZoomDelta = zoomDelta;
	}
	
	/// <summary>
	/// 目標値にズームする
	/// </summary>
	public void SetToPinchZoom(float zoom, float time = 1.0f)
	{
		dampZoomDelta = 0.0f;
		
		if(zoomType == PINCH_ZOOM_TYPE.POSITION_Z)
		{
			// 位置制限
			if(this.gameObject.transform.localPosition.z + zoom <= defaultZoom + limitMinMaxForRelativePosZ.min)
				zoom = (defaultZoom + limitMinMaxForRelativePosZ.min) - this.gameObject.transform.localPosition.z;
			if(this.gameObject.transform.localPosition.z + zoom >= defaultZoom + limitMinMaxForRelativePosZ.max)
				zoom = (defaultZoom + limitMinMaxForRelativePosZ.max) - this.gameObject.transform.localPosition.z;
			
			iTween.MoveTo(
				this.gameObject,
				iTween.Hash(
					"z", zoom,
					"islocal", true,
					"time", time,
					"easetype", iTween.EaseType.easeOutCubic
				)
			);		
		}
		else if(zoomType == PINCH_ZOOM_TYPE.FOV)
		{
			// 画角制限
			if(zoom <= limitMinMaxForFOV.min)
				zoom = limitMinMaxForFOV.min;
			if(zoom >= limitMinMaxForFOV.max)
				zoom = limitMinMaxForFOV.max;
			
			iTween.ValueTo(
				this.gameObject,
				iTween.Hash(
					"from", this.gameObject.GetComponent<Camera>().fieldOfView,
					"to", zoom,
					"time", time,
					"easetype", iTween.EaseType.easeOutCubic,
					"onupdate", "updateFov"
				)
			);
		}
		else if(zoomType == PINCH_ZOOM_TYPE.ORTHOSIZE)
		{
			// Orthoサイズ制限
			if(zoom <= limitMinMaxForOrthoSize.min)
				zoom = limitMinMaxForOrthoSize.min;
			if(zoom >= limitMinMaxForOrthoSize.max)
				zoom = limitMinMaxForOrthoSize.max;
			
			iTween.ValueTo(
				this.gameObject,
				iTween.Hash(
					"from", this.gameObject.GetComponent<Camera>().orthographicSize,
					"to", zoom,
					"time", time,
					"easetype", iTween.EaseType.easeOutCubic,
					"onupdate", "updateOrthosize"
				)
			);
		}
	}
	
	private void updateFov(float fov)
	{
		this.gameObject.GetComponent<Camera>().fieldOfView = fov;
	}
	
	private void updateOrthosize(float size)
	{
		this.gameObject.GetComponent<Camera>().orthographicSize = size;
	}
	
	/// <summary>
	/// ズームを初期化
	/// </summary>
	public void ResetPinchZoom()
	{
		ResetInput();
		
		SetToPinchZoom(defaultZoom);
	}
}
