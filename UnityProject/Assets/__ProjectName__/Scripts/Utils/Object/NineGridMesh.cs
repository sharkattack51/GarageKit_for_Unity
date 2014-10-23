using UnityEngine;
using System.Collections;

/*
 * 9グリッドメッシュを設定する
 * Vertical方向の3*3メッシュが必要です
 */ 

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
public class NineGridMesh : MonoBehaviour
{
	//メッシュの更新を行う
	public bool isCalc = true;
	
	//Updateでメッシュ更新を行う
	public bool calcAlways = false;
	
	//メッシュをリセットする
	public bool asResetMesh = false;
	
	//コーナー部分のメッシュサイズ
	public Vector2 cornerSize = new Vector2(0.333333f, 0.333333f);
	
	private MeshFilter meshFilter;
	private Vector3[] defaultVertices;
	
	
	void Awake()
	{
		if(!isCalc)
			this.enabled = false;
			
		meshFilter = this.gameObject.GetComponent<MeshFilter>();
		defaultVertices = meshFilter.mesh.vertices;
	}
	
	void Start()
	{
		SetVertex();
	}
	
	void Update()
	{
		if(calcAlways)
			SetVertex();
	}
	
	
	/// <summary>
	/// Vertexの更新を行う
	/// </summary>
	private void SetVertex()
	{
		Vector3 scale = (asResetMesh) ? Vector3.one : this.gameObject.transform.localScale;
		Vector3[] calcVertices = defaultVertices;
		
		calcVertices[1] = new Vector3(defaultVertices[0].x + cornerSize.x / scale.x, defaultVertices[1].y, defaultVertices[1].z);
		calcVertices[5] = new Vector3(defaultVertices[4].x + cornerSize.x / scale.x, defaultVertices[5].y, defaultVertices[5].z);
		calcVertices[9] = new Vector3(defaultVertices[8].x + cornerSize.x / scale.x, defaultVertices[9].y, defaultVertices[9].z);
		calcVertices[13] = new Vector3(defaultVertices[12].x + cornerSize.x / scale.x, defaultVertices[13].y, defaultVertices[13].z);
		
		calcVertices[2] = new Vector3(defaultVertices[3].x - cornerSize.x / scale.x, defaultVertices[2].y, defaultVertices[2].z);
		calcVertices[6] = new Vector3(defaultVertices[7].x - cornerSize.x / scale.x, defaultVertices[6].y, defaultVertices[6].z);
		calcVertices[10] = new Vector3(defaultVertices[11].x - cornerSize.x / scale.x, defaultVertices[10].y, defaultVertices[10].z);
		calcVertices[14] = new Vector3(defaultVertices[15].x - cornerSize.x / scale.x, defaultVertices[14].y, defaultVertices[14].z);
		
		calcVertices[4] = new Vector3(defaultVertices[4].x, defaultVertices[0].y + cornerSize.y / scale.y, defaultVertices[4].z);
		calcVertices[5] = new Vector3(defaultVertices[5].x, defaultVertices[1].y + cornerSize.y / scale.y, defaultVertices[5].z);
		calcVertices[6] = new Vector3(defaultVertices[6].x, defaultVertices[2].y + cornerSize.y / scale.y, defaultVertices[6].z);
		calcVertices[7] = new Vector3(defaultVertices[7].x, defaultVertices[3].y + cornerSize.y / scale.y, defaultVertices[7].z);
		
		calcVertices[8] = new Vector3(defaultVertices[8].x, defaultVertices[12].y - cornerSize.y / scale.y, defaultVertices[8].z);
		calcVertices[9] = new Vector3(defaultVertices[9].x, defaultVertices[13].y - cornerSize.y / scale.y, defaultVertices[9].z);
		calcVertices[10] = new Vector3(defaultVertices[10].x, defaultVertices[14].y - cornerSize.y / scale.y, defaultVertices[10].z);
		calcVertices[11] = new Vector3(defaultVertices[11].x, defaultVertices[15].y - cornerSize.y / scale.y, defaultVertices[11].z);
		
		meshFilter.mesh.vertices = calcVertices;
		meshFilter.mesh.RecalculateBounds();
	}
}
