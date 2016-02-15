using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class FaderObject : MonoBehaviour
{
	public Image uiImage;
	private CanvasGroup uiGroup;

	// フェード完了イベント
	public delegate void OnFadedDelegate(GameObject sender);
	public event OnFadedDelegate OnFaded;
	protected void InvokeOnFaded()
	{
		if(OnFaded != null)
			OnFaded(this.gameObject);
	}

	// フェードタイプ
	public enum FADE_TYPE
	{
		FADE_IN = 0,
		FADE_OUT
	}

	// フェード処理中判定
	public bool IsFading { get{ return iTween.Count(this.gameObject) > 0; } }


	void Awake()
	{
		uiGroup = this.gameObject.GetComponent<CanvasGroup>();

		uiGroup.alpha = 0.0f;
		uiImage.enabled = false;
	}
	
	void Start()
	{

	}
	
	void Update()
	{
	
	}


	// フェード処理
	public void FadeStart(FADE_TYPE type, float time = 1.0f, float delay = 0.0f)
	{
		uiImage.enabled = true;

		float from = 0.0f;
		float to = 0.0f;
		if(type == FADE_TYPE.FADE_IN)
		{
			from = 1.0f;
			to = 0.0f;
		}
		else if(type == FADE_TYPE.FADE_OUT)
		{
			from = 0.0f;
			to = 1.0f;
		}

		iTween.ValueTo(this.gameObject,
			iTween.Hash(
				"time", time,
				"delay", delay,
				"from", from,
				"to", to,
				"onupdate", "fading",
				"onupdatetarget", this.gameObject,
				"oncomplete", "faded",
				"oncompletetarget", this.gameObject));
	}

	private void fading(float val)
	{
		uiGroup.alpha = val;
	}

	private void faded()
	{
		if(uiGroup.alpha == 0.0f)
			uiImage.enabled = false;

		InvokeOnFaded();
	}
}
