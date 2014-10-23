using UnityEngine;
using System.Collections;

/*
 * フェードアニメーションを設定する
 */

[RequireComponent(typeof(Renderer))]
public class FadeObject : MonoBehaviour
{
	public enum FADE_TYPE
	{
		IN = 0,
		OUT
	}
	
	public FADE_TYPE type = FADE_TYPE.IN;
	public bool useOnStart = false;
	public float fadeTime = 1.0f;
	public iTween.LoopType loopType = iTween.LoopType.none;
	
	private Color defaultColor;
	private Color startColor;
	
	
	void Awake()
	{
	
	}
	
	void Start()
	{
		//初期設定
		defaultColor = this.gameObject.renderer.material.color;
		if(type == FADE_TYPE.IN)
			this.gameObject.renderer.material.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 0.0f);
		else
			this.gameObject.renderer.material.color = defaultColor;
		startColor = this.gameObject.renderer.material.color;
		
		//自動スタート
		if(useOnStart)
			FadeStart();
	}
	
	void Update()
	{
	
	}
	
	
	//フェードの開始
	public void FadeStart()
	{
		float targetAlpha;
		if(type == FADE_TYPE.IN)
			targetAlpha = defaultColor.a;
		else
			targetAlpha = 0.0f;
		
		iTween.ColorTo(
			this.gameObject,
			iTween.Hash(
				"a", targetAlpha,
				"time", fadeTime,
				"looptype", loopType));
	}
	
	//フェードのリセット
	public void FadeReset()
	{
		iTween.Stop(this.gameObject);
		this.gameObject.renderer.material.color = startColor;
	}
}
