using UnityEngine;
using System.Collections;
using System;

/*
 * Transformをターゲットオブジェクトと一致させる
 */

[Serializable]
public class FOLLOW_AXIS
{
	public bool x = true;
	public bool y = true;
	public bool z = true;
}

[ExecuteInEditMode]
public class TransformFollower : MonoBehaviour
{
	public GameObject targetObject; 
	
	public FOLLOW_AXIS followPosition;
	public FOLLOW_AXIS followRotation;
	public FOLLOW_AXIS followScale;
	
	
	void Awake()
	{
		SetFollow();
	}
	
	void Start()
	{
		
	}
	
	void Update()
	{
		SetFollow();
	}
	
	
	private void SetFollow()
	{
		if(targetObject != null)
		{
			if(followPosition.x || followPosition.y || followPosition.z)
				SetFollowPosition();
			
			if(followRotation.x || followRotation.y || followRotation.z)
				SetFollowRotation();
			
			if(followScale.x || followScale.y || followScale.z)
				SetFollowScale();
		}
	}
	
	//位置を設定
	private void SetFollowPosition()
	{
		float x = followPosition.x ? targetObject.transform.position.x : this.gameObject.transform.position.x;
		float y = followPosition.y ? targetObject.transform.position.y : this.gameObject.transform.position.y;
		float z = followPosition.z ? targetObject.transform.position.z : this.gameObject.transform.position.z;
		
		this.gameObject.transform.position = new Vector3(x, y, z);
	}
	
	//回転を設定
	private void SetFollowRotation()
	{
		float x = followRotation.x ? targetObject.transform.rotation.eulerAngles.x : this.gameObject.transform.rotation.eulerAngles.x;
		float y = followRotation.y ? targetObject.transform.rotation.eulerAngles.y : this.gameObject.transform.rotation.eulerAngles.y;
		float z = followRotation.z ? targetObject.transform.rotation.eulerAngles.z : this.gameObject.transform.rotation.eulerAngles.z;
		
		this.gameObject.transform.rotation = Quaternion.Euler(new Vector3(x, y, z));
	}
	
	//スケールを設定
	private void SetFollowScale()
	{
		float x = followScale.x ? targetObject.transform.localScale.x : this.gameObject.transform.localScale.x;
		float y = followScale.y ? targetObject.transform.localScale.y : this.gameObject.transform.localScale.y;
		float z = followScale.z ? targetObject.transform.localScale.z : this.gameObject.transform.localScale.z;
		
		this.gameObject.transform.localScale = new Vector3(x, y, z);
	}
}
