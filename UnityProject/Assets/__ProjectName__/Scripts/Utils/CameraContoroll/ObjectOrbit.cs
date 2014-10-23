using UnityEngine;
using System.Collections;

/*
 * ドラッグ操作でのカメラのオービット回転コントロールクラス
 * マウス&タッチ対応
 */

public class ObjectOrbit : MonoBehaviour
{
	public static bool win7touch = false;
	
	//singleton
	private static ObjectOrbit instance;
	public static ObjectOrbit Instance { get{ return instance; } }
	
	public float sensitivity = 10.0f;
	public float speed = 1.0f;
	public float smoothTime = 0.5f;
	public bool clampRotationX = true;
	public float clampRotationX_Min = -80.0f;
	public float clampRotationX_Max = 80.0f;
	public bool invertDragX = false;
	public bool invertDragY = false;
	
	private bool inputLock;
	public bool IsInputLock { get{ return inputLock; } }
	private object lockObject;
	
	private Vector3 moveVector;
	private bool isClicked;
	
	private bool isInvert = false;
	private bool oldState = false;
	private float dampRotX = 0.0f;
	private float dampRotY = 0.0f;
	private float velocityX = 0.0f;
	private float velocityY = 0.0f;
	
	private GameObject orbitAxisX;
	private GameObject orbitAxisY;
	
	
	void Awake()
	{
		instance = this;
	}
	
	void Start()
	{
		inputLock = false;
		
		//Orbit回転軸を設定
		orbitAxisX = new GameObject("OrbitAxisX");
		orbitAxisY = new GameObject("OrbitAxisY");
		if(this.transform.parent != null)
			orbitAxisX.transform.parent = this.transform.parent;
		orbitAxisY.transform.parent = orbitAxisX.transform;
		this.transform.parent = orbitAxisY.transform;
	}
	
	void Update()
	{
		if(!inputLock && ButtonObjectBase.PressBtnsTotal == 0)
			GetInput();
		else
			ResetInput();
		
		UpdateOrbit();
	}
	
	private void ResetInput()
	{	
		moveVector = Vector3.zero;
		isClicked = false;
	}
	
	private void GetInput()
	{
		//for Touch
		if(Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
		{
			if(Input.touchCount == 1)
			{
				if(Input.GetTouch(0).phase == TouchPhase.Moved)
				{
					moveVector = (Vector3)(Input.GetTouch(0).deltaPosition / 10.0f);
					isClicked = true;
				}
				else
					ResetInput();
			}
			else if(Input.touchCount == 0)
				ResetInput();
		}
		else if(Application.platform == RuntimePlatform.WindowsPlayer && win7touch)
		{
			if(W7TouchManager.GetTouchCount() == 1)
			{
				if(W7TouchManager.GetTouch(0).Phase == TouchPhase.Moved)
				{
					moveVector = (Vector3)(W7TouchManager.GetTouch(0).DeltaPosition / 10.0f);
					isClicked = true;
				}
				else
					ResetInput();
			}
			else if(W7TouchManager.GetTouchCount() == 0)
				ResetInput();
		}
		
		//for Mouse
		else
		{
			if(Input.GetMouseButton(0))
			{
				moveVector = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0.0f);
				isClicked = true;
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
	/// Orbit回転を更新
	/// </summary>
	private void UpdateOrbit()
	{
		//スムースダンプ
		dampRotX = Mathf.SmoothDampAngle(dampRotX, isClicked?(moveVector.y * sensitivity):0.0f, ref velocityX, smoothTime);
		dampRotY = Mathf.SmoothDampAngle(dampRotY, isClicked?(moveVector.x * sensitivity):0.0f, ref velocityY, smoothTime);
		
		//回転反転処理
		if(!oldState && isClicked)
		{
			if(orbitAxisX.transform.localRotation.eulerAngles.y < 180.0f)
				isInvert = true;
			else
				isInvert = false;
		}
		
		//回転を更新
		orbitAxisX.transform.localRotation *= Quaternion.AngleAxis(dampRotX * speed, Vector3.right * (invertDragY ? -1.0f : 1.0f));
		orbitAxisY.transform.localRotation *= Quaternion.AngleAxis(dampRotY * speed, (isInvert ? Vector3.down : Vector3.up) * (invertDragX ? -1.0f : 1.0f));
			
		//回転を制限
		if(clampRotationX)
		{
			Vector3 clampRot = orbitAxisX.transform.localRotation.eulerAngles;
			if((clampRot.x > 180.0f) && (clampRot.x < (360.0f + clampRotationX_Min))) clampRot.x = (360.0f + clampRotationX_Min);
			else if((clampRot.x < 180.0f) && (clampRot.x > clampRotationX_Max)) clampRot.x = clampRotationX_Max;
			orbitAxisX.transform.localRotation = Quaternion.Euler(new Vector3(clampRot.x, 0.0f, 0.0f));
		}
		
		//回転反転処理のためのステートチェック
		oldState = isClicked;
	}
	
	/// <summary>
	/// Orbit回転をリセット
	/// </summary>
	public void ResetOrbit()
	{
		ResetInput();
		
		orbitAxisX.transform.localRotation = Quaternion.identity;
		orbitAxisY.transform.localRotation = Quaternion.identity;
	}
}
