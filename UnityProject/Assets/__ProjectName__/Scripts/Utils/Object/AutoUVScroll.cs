using UnityEngine;
using System.Collections;

/*
 * UVの自動スクロールアニメーション
 */ 
public class AutoUVScroll : MonoBehaviour
{
	public int materialID = 0;
	public string texturePropName = "_MainTex";
	
	public float scrollTime = 1.0f;
	public Vector2 startUV = Vector2.zero;
	public Vector2 endUV = Vector2.zero;
	public iTween.EaseType easeType = iTween.EaseType.linear;
	public iTween.LoopType loopType = iTween.LoopType.loop;
	
	private Material targetMat;
	
	
	void Start()
	{
		targetMat = this.renderer.materials[materialID];
		
		iTween.ValueTo(
			this.gameObject,
			iTween.Hash(
				"time", scrollTime,
				"from", 0.0f,
				"to", 1.0f,
				"easetype", easeType,
				"looptype", loopType,
				"onupdate", "updated"));
	}
	
	private void updated(float val)
	{
		targetMat.SetTextureOffset(texturePropName, Vector2.Lerp(startUV, endUV, val));
	}
}