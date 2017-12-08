using UnityEngine;
using System.Collections;

/*
 * カメラをシフトする
 * projectionMatrixが上書きされるのでFOVは無効になります
 */
namespace GarageKit
{
	public class CameraShifter : MonoBehaviour
	{	
		public float shiftX = 0.0f;
		public float shiftY = 0.0f;
		
		public bool calcAlways = true;
		
		private Matrix4x4 orgProjM;
		
		
		void Awake()
		{
			
		}

		void Start()
		{
			// 初期値を保存
			orgProjM = this.GetComponent<Camera>().projectionMatrix;

			this.GetComponent<Camera>().projectionMatrix = CalcProjMat(shiftX, shiftY);
		}

		void LateUpdate()
		{
			if(calcAlways)
				this.GetComponent<Camera>().projectionMatrix = CalcProjMat(shiftX, shiftY);
		}

		private Matrix4x4 CalcProjMat(float shiftX, float shiftY)
		{
			Matrix4x4 m = orgProjM;

			m.m00 += 0.0f;
			m.m01 += 0.0f;
			m.m02 += -shiftX;
			m.m03 += 0.0f;

			m.m10 += 0.0f;
			m.m11 += 0.0f;
			m.m12 += -shiftY;
			m.m13 += 0.0f;

			m.m20 += 0.0f;
			m.m21 += 0.0f;
			m.m22 += 0.0f;
			m.m23 += 0.0f;

			m.m30 += 0.0f;
			m.m31 += 0.0f;
			m.m32 += 0.0f;
			m.m33 += 0.0f;

			return m;
		}

		public void ResetProjMat()
		{
			this.GetComponent<Camera>().projectionMatrix = orgProjM;
		}
	}
}
