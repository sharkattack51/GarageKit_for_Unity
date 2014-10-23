using UnityEngine;
using System.Collections;

/*
 * 	ターゲットに対してビルボード処理する
 */

public class Billbord : MonoBehaviour
{	
	public GameObject targetObj;
	public bool invertForward = false;
	public bool invertUp = false;	
	public bool lockPitch = false;
	public bool isZup = false;
	
	
	void Start()
	{
		if(targetObj == null)
			targetObj = Camera.main.gameObject;
	}
	
	void Update()
	{
		this.gameObject.transform.LookAt(targetObj.transform.position, Vector3.up);
		
		//方向の反転設定
		if(invertForward)
			this.gameObject.transform.localRotation *= Quaternion.AngleAxis(180.0f, Vector3.up);
		if(invertUp)
			this.gameObject.transform.localRotation *= Quaternion.AngleAxis(180.0f, Vector3.forward);
		
		//ピッチ回転のロック
		if(lockPitch)
		{
			this.gameObject.transform.localRotation = Quaternion.Euler(
				new Vector3(
					isZup ? -90.0f : 0.0f,
					this.gameObject.transform.localRotation.eulerAngles.y,
					this.gameObject.transform.localRotation.eulerAngles.z
				)
			);
		}
	}
}
