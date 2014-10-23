using UnityEngine;
using System.Collections;

/*
 * オブジェクトの自動回転アニメーション
 */

public class AutoRotate : MonoBehaviour
{
	//回転スピード
	public float rotateSpeed;
	
	//回転軸
	public enum ROTATE_AXIS
	{
		X = 0,
		Y,
		Z,
		XY,
		XZ,
		YZ,
		XYZ
	}
	
	public ROTATE_AXIS axis;
	
	
	void Update()
	{
		float fixedRotate = rotateSpeed * (Time.deltaTime / (1.0f / 60.0f));
		
		//回転
		switch(axis)
		{
			case ROTATE_AXIS.X: this.transform.Rotate(new Vector3(fixedRotate, 0.0f, 0.0f));
				break;
			case ROTATE_AXIS.Y: this.transform.Rotate(new Vector3(0.0f, fixedRotate, 0.0f));
				break;
			case ROTATE_AXIS.Z: this.transform.Rotate(new Vector3(0.0f, 0.0f, fixedRotate));
				break;
			case ROTATE_AXIS.XY: this.transform.Rotate(new Vector3(fixedRotate, fixedRotate, 0.0f));
				break;
			case ROTATE_AXIS.XZ: this.transform.Rotate(new Vector3(fixedRotate, 0.0f, fixedRotate));
				break;
			case ROTATE_AXIS.YZ: this.transform.Rotate(new Vector3(0.0f, fixedRotate, fixedRotate));
				break;
			case ROTATE_AXIS.XYZ: this.transform.Rotate(new Vector3(fixedRotate, fixedRotate, fixedRotate));
				break;
			default: break;
		}
	}
}
