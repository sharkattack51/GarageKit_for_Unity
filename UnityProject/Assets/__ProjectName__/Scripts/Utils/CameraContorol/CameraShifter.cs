using UnityEngine;
using System.Collections;

/*
 * カメラをシフトする
 * projectionMatrixが上書きされるのでFOVは無効になります
 */

public class CameraShifter : MonoBehaviour
{	
	public float shiftX = 0.0f;
	public float shiftY = 0.0f;
	
	public bool calcAlways = true;
	
	private Matrix4x4 orgProjM;
	
	
	void Awake()
	{
		//初期値を保存
		orgProjM = this.camera.projectionMatrix;
	}
	
	void Start()
	{			
		this.camera.projectionMatrix = CorrectedProjMat();
	}
	
	void Update()
	{	
		if(calcAlways)
			this.camera.projectionMatrix = CorrectedProjMat();
	}
	
	private Matrix4x4 CorrectedProjMat()
	{
		Matrix4x4 outMat;
		
		//回転をリセット
		//float camPitch = this.camera.transform.rotation.eulerAngles.x;				
		//Matrix4x4 resetRotMat = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(new Vector3(-camPitch,0.0f,0.0f)), Vector3.one);
		
		//シフト
		Vector3 shiftVec = new Vector3(shiftX, shiftY, 0.0f);
		Matrix4x4 shiftMat = Matrix4x4.TRS(shiftVec, Quaternion.Euler(Vector3.zero), Vector3.one);
		
		outMat = shiftMat * orgProjM;// * resetRotMat;
		
		return outMat;
	}
	
	public void ResetProjMat()
	{
		this.camera.projectionMatrix = orgProjM;
	}
}
