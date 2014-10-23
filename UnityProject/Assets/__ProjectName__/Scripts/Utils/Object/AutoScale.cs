using UnityEngine;
using System.Collections;

/*
 * オブジェクトの自動スケールアニメーション
 */

public class AutoScale : MonoBehaviour
{	
	public bool isLoop = true;
	public float scaleTime = 1.0f;
	public Vector3 startScale = Vector3.zero;
	public Vector3 endScale = Vector3.one;
	
	
	void Start()
	{	
		StartTween();
	}
	
	private void StartTween()
	{
		this.gameObject.transform.localScale = startScale;
		
		iTween.LoopType loopType;
		if(isLoop)
			loopType = iTween.LoopType.loop;
		else
			loopType = iTween.LoopType.none;
		
		iTween.ScaleTo(
			this.gameObject,
			iTween.Hash(
				"time", scaleTime,
				"scale", endScale,
				"easetype", iTween.EaseType.linear,
				"looptype", loopType
			)
		);
	}
}
