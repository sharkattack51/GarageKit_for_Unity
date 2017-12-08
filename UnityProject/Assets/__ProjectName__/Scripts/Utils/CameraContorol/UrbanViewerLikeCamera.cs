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
				float PosY = ( dragY / 100 ) * Time.deltaTime * moveSpeed;
				Vector3 up = characterController.gameObject.transform.TransformDirection(Vector3.up);
				characterController.Move(up * PosY);
			}
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
