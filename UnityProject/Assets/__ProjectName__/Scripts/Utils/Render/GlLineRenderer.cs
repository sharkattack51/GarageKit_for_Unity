using UnityEngine;
using System.Collections;
using System;

/*
 * OnPostRenderでラインを描画する
 */ 

//描画ラインのデータオブジェクト
[Serializable]
public class LineData
{
	public Vector2 start = Vector2.zero;
	public Vector2 end = Vector2.zero;
}

[RequireComponent(typeof(Camera))]
public class GlLineRenderer : MonoBehaviour
{
	//ラインの色
	public Color lineColor = Color.red;
	
	//描画ライン
	public LineData[] lines;
	
	private Material lineMaterial;
	
	
	void Start()
	{
		//ライン用のマテリアルを作成
		lineMaterial = new Material(Shader.Find("Self-Illumin/Diffuse"));
		lineMaterial.color = lineColor;
		lineMaterial.hideFlags = HideFlags.HideAndDontSave;
		lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
	}
	
	void OnPostRender()
	{
		GL.PushMatrix();
	    
		lineMaterial.SetPass(0);
	    GL.LoadOrtho();
		
		//ラインの描画
		foreach(LineData line in lines)
		{
		    GL.Begin(GL.LINES);
			
		    GL.Vertex(line.start);
		    GL.Vertex(line.end);
			
	    	GL.End();
		}
		
	    GL.PopMatrix();
	}
	
	/// <summary>
	/// ラインデータの更新
	/// </summary>
	public void UpdateLine(int id, Vector2 start, Vector2 end)
	{
		if(id < lines.Length && lines[id] != null)
		{
			lines[id].start = start;
			lines[id].end = end;
		}
	}
}
