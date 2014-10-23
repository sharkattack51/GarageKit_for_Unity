using UnityEngine;
using System.Collections;

/*
 * 平面方向を設定しターゲットまでのラインを引く
 * XY平面の場合はX軸が、XZ平面の場合はZ軸がターゲット方向を向く
 */ 

[ExecuteInEditMode]
public class LineObject : MonoBehaviour
{
	public enum DIMENSION
	{
		XY = 0,
		XZ
	}
	
	public DIMENSION dimension;
	public GameObject target;
	public float thickness = 0.1f;
	
	private GameObject linePlate;
	
	
	void Awake()
	{
		//ラインプレートを設定する
		if(linePlate != null)
		{
			linePlate = this.gameObject.transform.GetChild(0).gameObject;
			linePlate.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
			if(dimension == DIMENSION.XY)
				linePlate.transform.localPosition = new Vector3(0.5f, 0.0f, 0.0f);
			else if(dimension == DIMENSION.XZ)
				linePlate.transform.localPosition = new Vector3(0.0f, 0.0f, 0.5f);
		}
		
		SetLine();
	}
	
	void Start()
	{
	
	}
	
	void Update()
	{
		SetLine();
	}
	
	private void SetLine()
	{
		if(target != null)
		{
			if(dimension == DIMENSION.XY)
			{
				//方向
				Vector3 dir = target.transform.position - this.gameObject.transform.position;
				this.transform.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg));
				
				//長さ&太さ
				float scale = Vector3.Distance(target.transform.position, this.gameObject.transform.position);
				this.transform.localScale = new Vector3(scale / this.gameObject.transform.parent.localScale.x, thickness, 1.0f);
			}
			else if(dimension == DIMENSION.XZ)
			{
				//方向
				Vector3 dir = target.transform.position - this.gameObject.transform.position;
				this.transform.localRotation = Quaternion.Euler(new Vector3(0.0f, Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg, 0.0f));
				
				//長さ&太さ
				float scale = Vector3.Distance(target.transform.position, this.gameObject.transform.position);
				this.transform.localScale = new Vector3(thickness, 1.0f, scale / this.gameObject.transform.parent.localScale.x);
			}
		}
	}
}
