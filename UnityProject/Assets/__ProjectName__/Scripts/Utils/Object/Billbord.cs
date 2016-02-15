using UnityEngine;
using System.Collections;

/*
 * 	ターゲットに対してビルボード処理する
 */

public class Billbord : MonoBehaviour
{	
	public Transform target;
	public bool invertForward = false;
	public bool invertUp = false;	
	public bool lockPitch = false;
	public bool isZup = false;
	public bool asGroup = true;
	
	void Start()
	{
		if(target == null)
			target = Camera.main.gameObject.transform;
	}
	
	void Update()
	{
		if(asGroup)
		{
			for(int i = 0; i < this.gameObject.transform.childCount; i++)
				UpdateBillboard(this.gameObject.transform.GetChild(i));
		}
		else
			UpdateBillboard(this.gameObject.transform);
	}

	private void UpdateBillboard(Transform trns)
	{
		trns.LookAt(target.position, Vector3.up);
		
		//方向の反転設定
		if(invertForward)
			trns.localRotation *= Quaternion.AngleAxis(180.0f, Vector3.up);
		if(invertUp)
			trns.localRotation *= Quaternion.AngleAxis(180.0f, Vector3.forward);
		
		//ピッチ回転のロック
		if(lockPitch)
		{
			trns.localRotation = Quaternion.Euler(
				isZup ? -90.0f : 0.0f,
				trns.localRotation.eulerAngles.y,
				trns.localRotation.eulerAngles.z);
		}
	}
}
