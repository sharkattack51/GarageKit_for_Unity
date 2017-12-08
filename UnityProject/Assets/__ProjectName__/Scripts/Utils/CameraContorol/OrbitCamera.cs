using UnityEngine;
using System.Collections;

/*
 * ターゲットを中心に回転するカメラコントロールクラスス
 * マウス&タッチ対応
 */
namespace GarageKit
{
	[RequireComponent(typeof(Camera))]
	public class OrbitCamera : MonoBehaviour
	{
		public static bool winTouch = false;
		public static bool updateEnable = true;

		// 入力方法のタイプ
		public enum ORBIT_INPUT_TYPE
		{
			PRIMARY = 0,
			SECONDARY
		}

		public ORBIT_INPUT_TYPE orbitInputType = ORBIT_INPUT_TYPE.PRIMARY;

		public GameObject target;
		public float sensitivity = 0.1f;
		public float smoothTime = 2.0f;
		public float clampRotationX_Min = 0.0f;
		public float clampRotationX_Max = 60.0f;
		public bool invertDragX = false;
		public bool invertDragY = true;
		public float ratioForMouse = 1.0f;
		public FlyThroughCamera combinationFlyThroughCamera;

		private bool inputLock;
		public bool IsInputLock { get{ return inputLock; } }
		private object lockObject;

		private GameObject orbitRoot;
		public GameObject OrbitRoot { get{ return orbitRoot; } }

		private Vector3 moveVector;
		private Vector3 defaultPos;
		private Quaternion defaultRot;
		private float dampRotX = 0.0f;
		private float dampRotY = 0.0f;
		private float velocityX = 0.0f;
		private float velocityY = 0.0f;

		private Vector3 lastFingerVec = Vector3.zero;
		private Vector3 lastFingerPos = Vector3.zero;
		private Vector3 oldMousePosition = Vector3.zero;

		
		void Awake()
		{

		}

		IEnumerator Start()
		{
			Vector3 temp_pos = this.gameObject.transform.position;
			Quaternion temp_rot = this.gameObject.transform.rotation;

			yield return null;

			inputLock = false;

			// OrbitRootを設定
			orbitRoot = new GameObject(this.name + " Orbit Root");
			orbitRoot.transform.SetParent(this.gameObject.transform.parent, true);
			orbitRoot.transform.position = temp_pos;
			orbitRoot.transform.rotation = temp_rot;
			this.gameObject.transform.SetParent(orbitRoot.transform, true);

			// 初期値設定
			defaultPos = orbitRoot.transform.position;
			defaultRot = orbitRoot.transform.rotation;
			dampRotX = defaultRot.eulerAngles.x;
			dampRotY = defaultRot.eulerAngles.y;
		}
		
		void Update()
		{
			if(!inputLock && ButtonObjectBase.PressBtnsTotal == 0)
			{
				if(orbitInputType == ORBIT_INPUT_TYPE.PRIMARY)
					GetInput();
				else if(orbitInputType == ORBIT_INPUT_TYPE.SECONDARY)
					GetInput2();
			}
			else
				ResetInput();

			UpdateOrbit();
		}

		private void ResetInput()
		{
			moveVector = Vector3.zero;
			lastFingerVec = Vector3.zero;
			lastFingerPos = Vector3.zero;
		}

		private void GetInput()
		{
			// for Touch
			if(Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
			{
				if(Input.touchCount == 1)
				{
					if(Input.GetTouch(0).phase == TouchPhase.Moved)
						moveVector = (Vector3)(Input.GetTouch(0).deltaPosition / 10.0f);
					else
						ResetInput();
				}
				else if(Input.touchCount == 0)
					ResetInput();
			}

#if UNITY_STANDALONE_WIN
			else if(Application.platform == RuntimePlatform.WindowsPlayer && winTouch)
			{
				if(TouchScript.TouchManager.Instance.PressedPointersCount == 1)
				{
					TouchScript.Pointers.Pointer tp = TouchScript.TouchManager.Instance.PressedPointers[0];
					if(tp.Position != tp.PreviousPosition)
						moveVector = (Vector3)((tp.PreviousPosition - tp.Position) / 10.0f);
					else
						ResetInput();
				}
				else if(TouchScript.TouchManager.Instance.PressedPointersCount == 0)
					ResetInput();
			}
#endif
			
			// for Mouse
			else
			{
				if(Input.GetMouseButton(0))
					moveVector = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0.0f);
				else
					ResetInput();
			}
		}

		private void GetInput2()
		{
			// for Touch
			if(Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
			{
				if(Input.touchCount == 2)
				{
					if(lastFingerVec == Vector3.zero)
					{
						lastFingerVec = (Input.GetTouch(0).position - Input.GetTouch(1).position).normalized;
						lastFingerPos = (Input.GetTouch(0).position + Input.GetTouch(1).position) / 2.0f;
					}
					else
					{
						moveVector = Vector3.zero;

						// 回転
						Vector3 now_vec = (Input.GetTouch(0).position - Input.GetTouch(1).position).normalized;
						moveVector.x += Mathf.Rad2Deg * (Mathf.Atan2(lastFingerVec.y, lastFingerVec.x) - Mathf.Atan2(now_vec.y, now_vec.x));

						float rot_x_max = 10.0f;
						if(moveVector.x < rot_x_max) moveVector.x = -rot_x_max;
						if(rot_x_max < moveVector.x) moveVector.x = rot_x_max;
						lastFingerVec = now_vec;

						// 傾き
						Vector3 now_pos = (Input.GetTouch(0).position + Input.GetTouch(1).position) / 2.0f;
						moveVector.y += (-100.0f * ((now_pos - lastFingerPos).y / Screen.height));
						lastFingerPos = now_pos;
					}
				}
				else
					ResetInput();
			}

#if UNITY_STANDALONE_WIN
			else if(Application.platform == RuntimePlatform.WindowsPlayer && winTouch)
			{
				if(TouchScript.TouchManager.Instance.PressedPointersCount == 2)
				{
					TouchScript.Pointers.Pointer pt0 = TouchScript.TouchManager.Instance.PressedPointers[0];
					TouchScript.Pointers.Pointer pt1 = TouchScript.TouchManager.Instance.PressedPointers[1];

					if(lastFingerVec == Vector3.zero)
					{
						lastFingerVec = (pt0.Position - pt1.Position).normalized;
						lastFingerPos = (pt0.Position + pt1.Position) / 2.0f;
					}
					else
					{
						moveVector = Vector3.zero;

						// 回転
						Vector3 now_vec = (pt0.Position - pt1.Position).normalized;
						moveVector.x += Mathf.Rad2Deg * (Mathf.Atan2(lastFingerVec.y, lastFingerVec.x) - Mathf.Atan2(now_vec.y, now_vec.x));

						float rot_x_max = 10.0f;
						if(moveVector.x < rot_x_max) moveVector.x = -rot_x_max;
						if(rot_x_max < moveVector.x) moveVector.x = rot_x_max;
						lastFingerVec = now_vec;

						// 傾き
						Vector3 now_pos = (pt0.Position + pt1.Position) / 2.0f;
						moveVector.y += (-100.0f * ((now_pos - lastFingerPos).y / Screen.height));
						lastFingerPos = now_pos;
					}
				}
				else
					ResetInput();
			}
#endif

			// for Mouse
			else
			{
				moveVector = Vector3.zero;

				if(Input.GetMouseButton(1))
				{
					// 回転
					moveVector.x = (oldMousePosition.x - Input.mousePosition.x) * ratioForMouse;

					// 傾き
					moveVector.y = (oldMousePosition.y - Input.mousePosition.y) * ratioForMouse;
				}
				else
					ResetInput();

				oldMousePosition = Input.mousePosition;
			}
		}
		
		private void UpdateOrbit()
		{
			if(!OrbitCamera.updateEnable)
				return;

			if(combinationFlyThroughCamera != null && target == null)
				target = combinationFlyThroughCamera.FlyThroughRoot;

			if(target != null && orbitRoot != null)
			{
				// 回転を更新
				velocityX += moveVector.y * (invertDragY ? -1.0f : 1.0f) * sensitivity;
				velocityY += moveVector.x * (invertDragX ? -1.0f : 1.0f) * sensitivity;
				dampRotX += velocityX;
				dampRotY += velocityY;

				// 回転を制限
				if (dampRotX < -360.0f)
					dampRotX += 360.0f;
				if (dampRotX > 360.0f)
					dampRotX -= 360.0f;
				dampRotX = Mathf.Clamp(dampRotX, clampRotationX_Min, clampRotationX_Max);

				// Transform反映
				orbitRoot.transform.rotation = Quaternion.Euler(dampRotX, dampRotY, 0.0f);
				orbitRoot.transform.position =
					(orbitRoot.transform.rotation * new Vector3(0.0f, 0.0f, -Vector3.Distance(orbitRoot.transform.position, target.transform.position))) + target.transform.position;

				// スムースダンプ
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
}