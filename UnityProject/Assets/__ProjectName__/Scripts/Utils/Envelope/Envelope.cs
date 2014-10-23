using UnityEngine;
using System.Collections;
using System;

/*
 * AnimationCurveによってエンベロープ処理する
 */

[Serializable]
public class EnvelopeKey
{
	public float time = 0.0f;
	public float value = 0.0f;
	public AnimationCurveUtil.TangentMode leftTangent = AnimationCurveUtil.TangentMode.Editable;
	public AnimationCurveUtil.TangentMode rightTangent = AnimationCurveUtil.TangentMode.Editable;
}

public class Envelope : MonoBehaviour
{
	//カーブ生成
	public bool generateCurveOnStartEditor = false;

	//プロパティ名
	public string propertyName = "";

	//ラップモード
	public WrapMode preWrapMode = WrapMode.Default;
	public WrapMode postWrapMode = WrapMode.Default;

	//カーブポイント配列
	public EnvelopeKey[] envelopeKeys;
	
	//カーブ
	[SerializeField]
	private AnimationCurve envelopeCurve;
	public AnimationCurve GetCurve() { return envelopeCurve; }


	void Start()
	{
		//カーブを設定
		if(Application.platform == RuntimePlatform.WindowsEditor && generateCurveOnStartEditor)
		{
			Keyframe[] keys = new Keyframe[envelopeKeys.Length];
			for(int i = 0; i < envelopeKeys.Length; i++)
			{
				EnvelopeKey envKey = envelopeKeys[i];
				keys[i] = AnimationCurveUtil.GetNewKey(envKey.time, envKey.value, envKey.leftTangent, envKey.rightTangent);
			}
			
			envelopeCurve = new AnimationCurve(keys);
			envelopeCurve.preWrapMode = preWrapMode;
			envelopeCurve.postWrapMode = postWrapMode;
			AnimationCurveUtil.UpdateAllLinearTangents(envelopeCurve);
		}
	}
}
