using UnityEngine;
using System.Collections;

/*
 * ターゲットを中心に回転するカメラコントロールクラスス
 * マウス&タッチ対応
 */

[RequireComponent(typeof(Camera))]
public class OrbitCamera : MonoBehaviour
{
	public static bool win7touch = false;

	//singleton
	private static OrbitCamera instance;
	public static OrbitCamera Instance { get{ return instance; } }

	public GameObject target;
	public float sensitivity = 0.1f;
	public float smoothTime = 2.0f;
	public float clampRotationX_Min = 0.0f;
	public float clampRotationX_Max = 60.0f;
	public bool invertDragX = false;
	public bool invertDragY = true;
	
	private bool inputLock;
	public bool IsInputLock { get{ return inputLock; } }
	private object lockObject;
	
	private Vector3 moveVector;
	
	private GameObject orbitRoot;
	private Vector3 defaultPos;
	private Quaternion defaultRot;
	private float dampRotX = 0.0f;
	private float dampRotY = 0.0f;
	private float velocityX = 0.0f;
	private float velocityY = 0.0f;

	
	void Awake()
	{
		instance = this;
	}

	void Start()
	{
		inputLock = false;

		//OrbitRootを設定
		orbitRoot = new GameObject("OrbitRoot");
		orbitRoot.transform.parent = this.gameObject.transform.parent;
		orbitRoot.transform.position = this.gameObject.transform.position;
		orbitRoot.transform.rotation = this.gameObject.transform.rotation;
		this.gameObject.transform.parent = orbitRoot.transform;

		//初期値設定
		defaultPos = orbitRoot.transform.position;
		defaultRot = orbitRoot.transform.rotation;
		dampRotX = defaultRot.eulerAngles.x;
		dampRotY = defaultRot.eulerAngles.y;
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
				}
				else
					ResetInput();
			}
			else if(Input.touchCount == 0)
				ResetInput();
		}

#if UNITY_STANDALONE_WIN
		else if(Application.platform == RuntimePlatform.WindowsPlayer && win7touch)
		{
			if(W7TouchManager.GetTouchCount() == 1)
			{
				if(W7TouchManager.GetTouch(0).Phase == TouchPhase.Moved)
				{
					moveVector = (Vector3)(W7TouchManager.GetTouch(0).DeltaPosition / 10.0f);
				}
				else
					ResetInput();
			}
			else if(W7TouchManager.GetTouchCount() == 0)
				ResetInput();
		}
#endif
		
		//for Mouse
		else
		{
			if(Input.GetMouseButton(0))
			{
				moveVector = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0.0f);
			}
			else
				ResetInput();
		}
	}
	
	private void UpdateOrbit()
	{
		if(target != null)
		{
			//回転を更新
			velocityX += moveVector.y * (invertDragY ? -1.0f : 1.0f) * sensitivity;
			velocityY += moveVector.x * (invertDragX ? -1.0f : 1.0f) * sensitivity;
			dampRotX += velocityX;
			dampRotY += velocityY;

			//回転を制限
			if (dampRotX < -360.0f)
				dampRotX += 360.0f;
			if (dampRotX > 360.0f)
				dampRotX -= 360.0f;
			dampRotX = Mathf.Clamp(dampRotX, clampRotationX_Min, clampRotationX_Max);

			//Transform反映
			orbitRoot.transform.rotation = Quaternion.Euler(dampRotX, dampRotY, 0.0f);
			orbitRoot.transform.position =
				(orbitRoot.transform.rotation * new Vector3(0.0f, 0.0f, -Vector3.Distance(orbitRoot.transform.position, target.transform.position))) + target.transform.position;

			//スムースダンプ
			velocityX = Mathf.Lerp(velocityX, 0.0f, Time.deltaTime * smoothTime);
			velocityY = Mathf.Lerp(velocityY, 0.0f, Time.deltaTime * smoothTime);
		}
	}

	/// <summary>
	/// Orbit回転をリセット
	/// </summary>
	public void ResetOrbit()
	{
		ResetInput();

		orbitRoot.transform.position = defaultPos;
		orbitRoot.transform.rotation = defaultRot;
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
}