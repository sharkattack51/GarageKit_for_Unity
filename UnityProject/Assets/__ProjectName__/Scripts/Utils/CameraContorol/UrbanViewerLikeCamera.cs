using UnityEngine;
using System.Collections;

namespace GarageKit
{
	public class UrbanViewerLikeCamera : MonoBehaviour
	{
		public Camera childCamera;
		public float moveSpeed = 5.0f;
		public float rotateSpeed = 20.0f;
		public bool updateEnable = true;

		[Header("for GamePad")]
		public float camMoveSpeedForPad = 0.5f;
		public float camRotSpeedForPad = 0.5f;
		public bool invertCamRotXForPad = false;
		public bool invertCamRotYForPad = false;
		public enum PadType
		{
			DEFAULT = 0,
			XBOXONE
		}
		public PadType padType = PadType.DEFAULT;
		
		private CharacterController characterController;
		private bool isFirstTouch = true;
		private Vector3 dragDelta;
		private Vector3 oldMousePos;
		private float dragX;
		private float dragY;


		void Awake()
		{
			
		}

		void Start()
		{
			if(this.GetComponent<Camera>() != null)
			{
				Debug.LogWarning("UrbanViewerLikeCamera :: This script attaches to the parent object of the camera.");
				this.enabled = false;
				return;
			}

			if(childCamera == null)
			{
				Camera cam = this.gameObject.GetComponentInChildren<Camera>();
				if(cam != null)
					childCamera = cam;
			}

			characterController = this.gameObject.AddComponent<CharacterController>();
		}
		
		void Update () 
		{
			if(!updateEnable)
				return;
			
			GetInputMouseDrag();
			UpdateFlythrough();
		}
		
		
		private void GetInputMouseDrag()
		{	
			if(Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2))
			{
				// ドラッグ量を計算
				Vector3 currentMousePos = Input.mousePosition;
				
				if(isFirstTouch)
				{
					oldMousePos = currentMousePos;
					isFirstTouch = false;

					return;
				}
				
				dragDelta = currentMousePos - oldMousePos;
				dragX = Mathf.Clamp(dragDelta.x, -100.0f, 100.0f);
				dragY = Mathf.Clamp(dragDelta.y, -100.0f, 100.0f);
			}
			else
				ResetInput();
		}
		
		private void UpdateFlythrough()
		{
#region for Mouse

			if(Input.GetMouseButton(0))
			{
				// 前進後退
				float pos_z = (dragY / 100.0f) * Time.deltaTime * moveSpeed;
				Vector3 forward = this.gameObject.transform.TransformDirection(Vector3.forward);
				characterController.Move(forward * pos_z);

				// 左右旋回
				float rot_y = (dragX / 100.0f) * Time.deltaTime * rotateSpeed;
				this.gameObject.transform.Rotate(Vector3.up, rot_y, Space.Self);
			}
			else if(Input.GetMouseButton(2))
			{
				// 左右旋回
				float rot_y = (dragX / 100.0f) * Time.deltaTime * rotateSpeed;
				this.gameObject.transform.Rotate(Vector3.up, rot_y, Space.Self);

				// 上下旋回
				float rot_x = (dragY / 100.0f)  * Time.deltaTime * rotateSpeed;
				childCamera.transform.Rotate(Vector3.right, -rot_x, Space.Self);
			}
			else if(Input.GetMouseButton(1))
			{
				// 左右平行移動
				float pos_x = (dragX / 100.0f) * Time.deltaTime * moveSpeed;
				Vector3 right = characterController.gameObject.transform.TransformDirection(Vector3.right);
				characterController.Move(right * pos_x);

				// 上昇下降
				float pos_y = (dragY / 100.0f) * Time.deltaTime * moveSpeed;
				Vector3 up = characterController.gameObject.transform.TransformDirection(Vector3.up);
				characterController.Move(up * pos_y);
			}

#endregion

#region for GamePad

			if((Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt)))
				return;
			
			float camMoveSpeedForPadOnFire = camMoveSpeedForPad * ((Input.GetButton("Fire1") || Input.GetButton("Fire2")) ? 5.0f : 1.0f);

			// 前進後退
			float pad_pos_z = Input.GetAxisRaw("Vertical") * Time.deltaTime * moveSpeed * camMoveSpeedForPadOnFire;
			Vector3 pad_forward = this.gameObject.transform.TransformDirection(Vector3.forward);
			characterController.Move(pad_forward * pad_pos_z);

			// 左右平行移動
			float pad_pos_x = Input.GetAxisRaw("Horizontal") * Time.deltaTime * moveSpeed * camMoveSpeedForPadOnFire;
			Vector3 pad_right = characterController.gameObject.transform.TransformDirection(Vector3.right);
			characterController.Move(pad_right * pad_pos_x);

			// 上昇下降
			if(padType == PadType.DEFAULT)
			{
				float pad_pos_y = Input.GetAxisRaw("Trigger") * Time.deltaTime * moveSpeed * camMoveSpeedForPadOnFire;
				Vector3 pad_up = characterController.gameObject.transform.TransformDirection(Vector3.up);
				characterController.Move(pad_up * pad_pos_y);
			}
			else if(padType == PadType.XBOXONE)
			{
				float pad_pos_y1 = Input.GetAxisRaw("Trigger") * Time.deltaTime * moveSpeed * camMoveSpeedForPadOnFire * 0.5f;
				Vector3 pad_up1 = characterController.gameObject.transform.TransformDirection(Vector3.up);
				characterController.Move(pad_up1 * pad_pos_y1);

				float pad_pos_y2 = Input.GetAxisRaw("Trigger2") * Time.deltaTime * moveSpeed * camMoveSpeedForPadOnFire * 0.5f;
				Vector3 pad_up2 = characterController.gameObject.transform.TransformDirection(Vector3.up);
				characterController.Move(pad_up2 * pad_pos_y2);
			}

			// 左右旋回
			float pad_rot_y = Input.GetAxisRaw("Horizontal2") * Time.deltaTime * rotateSpeed * camRotSpeedForPad * (invertCamRotYForPad ? -1.0f : 1.0f);
			this.gameObject.transform.Rotate(Vector3.up, pad_rot_y, Space.Self);

			// 上下旋回
			float pad_rot_x = Input.GetAxisRaw("Vertical2") * Time.deltaTime * rotateSpeed * camRotSpeedForPad * (invertCamRotXForPad ? -1.0f : 1.0f);
			childCamera.transform.Rotate(Vector3.right, -pad_rot_x, Space.Self);

#endregion
		}
		
		private void ResetInput()
		{
			isFirstTouch = true;
			dragDelta = Vector3.zero;
			dragX = 0.0f;
			dragY = 0.0f;
		}
	}
}
