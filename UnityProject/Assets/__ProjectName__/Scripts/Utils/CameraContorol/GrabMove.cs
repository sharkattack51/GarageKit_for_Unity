using UnityEngine;
using System.Collections;

/*
 * マルチタッチのグラブ操作でのオブジェクト移動コントロールクラス
 * マウス&タッチ対応
 */

public class GrabMove : MonoBehaviour
{
	public static bool win7touch = false;
	
	//singleton
	private static GrabMove instance;
	public static GrabMove Instance { get{ return instance; } }
	
	public Camera renderCamera;
	public int grabTouchNum = 3;
	public float moveBias = 1.0f;
	public float smoothTime = 0.1f;
	public MonoBehaviour[] disableComponents; //グラブムーブ操作時に動作をOFFにするコンポーネント
	
	private bool inputLock;
	public bool IsInputLock { get{ return inputLock; } }
	private object lockObject;
	
	private Vector3 defaultPos;
	private Vector3 velocitySmoothPos;
	
	private bool isFirstTouch;
	private Vector3 oldWorldTouchPos;
	private Vector3 dragDelta;
	
	
	void Awake()
	{
		instance = this;
	}
	
	void Start()
	{
		inputLock = false;
		
		//初期値を保存
		defaultPos = this.gameObject.transform.position;
			
		ResetInput();
	}
	
	void Update()
	{
		if(!inputLock && ButtonObjectBase.PressBtnsTotal == 0)
			GetInput();
		else
			ResetInput();
		
		UpdateMove();
	}
	
	private void ResetInput()
	{
		isFirstTouch = true;
		oldWorldTouchPos = Vector3.zero;
		dragDelta = Vector3.zero;
		
		//カメラ操作のアンロック
		if(FlyThroughCamera.Instance != null) FlyThroughCamera.Instance.UnlockInput(this.gameObject);
		if(PinchZoomCamera.Instance != null) PinchZoomCamera.Instance.UnlockInput(this.gameObject);
		if(OrbitCamera.Instance != null) OrbitCamera.Instance.UnlockInput(this.gameObject);

		//コンポーネントをON
		foreach(MonoBehaviour component in disableComponents)
		{
			if(component != null)
				component.enabled = true;
		}
	}
	
	private void GetInput()
	{
		//for Touch
		if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
		{
			if(Input.touchCount >= grabTouchNum)
			{
				//カメラ操作のロック
				if(FlyThroughCamera.Instance != null) FlyThroughCamera.Instance.LockInput(this.gameObject);
				if(PinchZoomCamera.Instance != null) PinchZoomCamera.Instance.LockInput(this.gameObject);
				if(OrbitCamera.Instance != null) OrbitCamera.Instance.LockInput(this.gameObject);
				
				//コンポーネントをOFF
				foreach(MonoBehaviour component in disableComponents)
				{
					if(component != null)
						component.enabled = false;
				}
				
				//ドラッグ量を計算
				float touchesPosX = (Input.GetTouch(0).position.x + Input.GetTouch(1).position.x + Input.GetTouch(2).position.x) / 3.0f;
				float touchesPosY = (Input.GetTouch(0).position.y + Input.GetTouch(1).position.y + Input.GetTouch(2).position.y) / 3.0f;
				Vector3 currentWorldTouchPos = renderCamera.ScreenToWorldPoint(new Vector3(touchesPosX, touchesPosY, 1000.0f));
				
				if(isFirstTouch)
				{
					oldWorldTouchPos = currentWorldTouchPos;
					isFirstTouch = false;
					return;
				}
				
				dragDelta = currentWorldTouchPos - oldWorldTouchPos;
				oldWorldTouchPos = currentWorldTouchPos;
			}
			else
				ResetInput();
		}

#if UNITY_STANDALONE_WIN
		else if(Application.platform == RuntimePlatform.WindowsPlayer && win7touch)
		{
			if(W7TouchManager.GetTouchCount() >= grabTouchNum)
			{
				//カメラ操作のロック
				if(FlyThroughCamera.Instance != null) FlyThroughCamera.Instance.LockInput(this.gameObject);
				if(PinchZoomCamera.Instance != null) PinchZoomCamera.Instance.LockInput(this.gameObject);
				if(OrbitCamera.Instance != null) OrbitCamera.Instance.LockInput(this.gameObject);

				//コンポーネントをOFF
				foreach(MonoBehaviour component in disableComponents)
				{
					if(component != null)
						component.enabled = false;
				}
				
				//ドラッグ量を計算
				float touchesPosX = (W7TouchManager.GetTouch(0).Position.x + W7TouchManager.GetTouch(1).Position.x + W7TouchManager.GetTouch(2).Position.x) / 3.0f;
				float touchesPosY = (W7TouchManager.GetTouch(0).Position.y + W7TouchManager.GetTouch(1).Position.y + W7TouchManager.GetTouch(2).Position.y) / 3.0f;
				Vector3 currentWorldTouchPos = renderCamera.ScreenToWorldPoint(new Vector3(touchesPosX, touchesPosY, 1000.0f));
				
				if(isFirstTouch)
				{
					oldWorldTouchPos = currentWorldTouchPos;
					isFirstTouch = false;
					return;
				}
				
				dragDelta = currentWorldTouchPos - oldWorldTouchPos;
				oldWorldTouchPos = currentWorldTouchPos;
			}
			else
				ResetInput();
		}
#endif

		//for Mouse
		else
		{
			if(Input.GetMouseButton(0)
				&& (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
				&& (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
			{
				//カメラ操作のロック
				if(FlyThroughCamera.Instance != null) FlyThroughCamera.Instance.LockInput(this.gameObject);
				if(PinchZoomCamera.Instance != null) PinchZoomCamera.Instance.LockInput(this.gameObject);
				if(OrbitCamera.Instance != null) OrbitCamera.Instance.LockInput(this.gameObject);

				//コンポーネントをOFF
				foreach(MonoBehaviour component in disableComponents)
				{
					if(component != null)
						component.enabled = false;
				}
				
				//ドラッグ量を計算
				Vector3 currentWorldTouchPos = renderCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1000.0f));
				
				if(isFirstTouch)
				{
					oldWorldTouchPos = currentWorldTouchPos;
					isFirstTouch = false;
					return;
				}
				
				dragDelta = currentWorldTouchPos - oldWorldTouchPos;
				oldWorldTouchPos = currentWorldTouchPos;
			}
			else
				ResetInput();	
		}
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
	/// 位置を更新
	/// </summary>
	private void UpdateMove()
	{
		Vector3 targetPos = this.gameObject.transform.position + (dragDelta * moveBias);
		Vector3 calcPos = Vector3.SmoothDamp(this.gameObject.transform.position, targetPos, ref velocitySmoothPos, smoothTime);
		
		this.gameObject.transform.position = calcPos;
	}
	
	/// <summary>
	/// 位置を初期化
	/// </summary>
	public void ResetGrabMove()
	{
		ResetInput();
		
		this.gameObject.transform.position = defaultPos;
	}
}
