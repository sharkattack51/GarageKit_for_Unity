using UnityEngine;
using System.Collections;

/*
 * ゲームオブジェクトにギズモを設定する
 */

public class GizmoDrawer : MonoBehaviour
{
	//表示するGizmoのタイプ
	public enum GIZMO_TYPE
	{
		CUBE = 0,
		SPHERE
	}
	
	public GIZMO_TYPE gizmoType;
	
	public Color color = Color.red;
	
	public Vector3 gizmoSize = Vector3.one;
	public bool centerToCornerX = false;
	public bool centerToCornerY = false;
	public bool centerToCornerZ = false;
	public bool invertCornerX = false;
	public bool invertCornerY = false;
	public bool invertCornerZ = false;
	
	
	void OnDrawGizmos()
	{
		//色を設定
		Gizmos.color = color;
		
		Draw();
	}
	
	void OnDrawGizmosSelected()
	{
		//色を設定
		Gizmos.color = Color.white;
		
		Draw();
	}
	
	/// <summary>
	/// ギズモを表示
	/// </summary>
	private void Draw()
	{
		float centerX;
		float centerY;
		float centerZ;
		
		if(centerToCornerX)
			centerX = this.transform.position.x + ((gizmoSize.x / 2.0f) * (invertCornerX ? -1.0f : 1.0f));
		else
			centerX = this.transform.position.x;
		
		if(centerToCornerY)
			centerY = this.transform.position.y + ((gizmoSize.y / 2.0f) * (invertCornerY ? -1.0f : 1.0f));
		else
			centerY = this.transform.position.y;
		
		if(centerToCornerZ)
			centerZ = this.transform.position.z - ((gizmoSize.z / 2.0f) * (invertCornerZ ? -1.0f : 1.0f));
		else
			centerZ = this.transform.position.z;
		
		Matrix4x4 tempMat = Gizmos.matrix;
		Matrix4x4 mat = new Matrix4x4();
		mat.SetTRS(
			new Vector3(centerX, centerY, centerZ),
			this.gameObject.transform.localRotation,
			Vector3.one);
		Gizmos.matrix = mat;
		
		switch(gizmoType)
		{
			case GIZMO_TYPE.CUBE:
				Gizmos.DrawWireCube(Vector3.zero, gizmoSize);
				break;
			case GIZMO_TYPE.SPHERE:
				Gizmos.DrawWireSphere(Vector3.zero, gizmoSize.x / 2.0f);
				break;
			default: break;
		}
		
		Gizmos.matrix = tempMat;
	}
}