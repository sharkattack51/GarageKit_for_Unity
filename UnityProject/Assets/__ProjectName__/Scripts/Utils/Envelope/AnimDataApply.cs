using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

using MiniJSON;

/*
 * Cinema4Dのキーフレームjsonデータをアニメーションカーブに変換する
 */

[Serializable]
public class AnimationTrack
{
	//C4Dでのアニメ―ショントラックタイプ
	public enum TRACK_TYPE
	{
		POS_X = 0,
		POS_Y,
		POS_Z,
		ROT_X,
		ROT_Y,
		ROT_Z
	};
	public TRACK_TYPE type = TRACK_TYPE.POS_X;

	//キーフレームから変換されるアニメカーブ
	public AnimationCurve curve;


	public AnimationTrack()
	{

	}


	//C4Dでのトラック名の取得
	public string GetTrackName()
	{
		string name = "";
		switch(this.type)
		{
			case TRACK_TYPE.POS_X: name = "Position . X"; break;
			case TRACK_TYPE.POS_Y: name = "Position . Y"; break;
			case TRACK_TYPE.POS_Z: name = "Position . Z"; break;
			case TRACK_TYPE.ROT_X: name = "Rotation . P"; break;
			case TRACK_TYPE.ROT_Y: name = "Rotation . H"; break;
			case TRACK_TYPE.ROT_Z: name = "Rotation . B"; break;
			default: break;
		}
		
		return name;
	}
}

public class AnimDataApply : MonoBehaviour
{
	//参照ファイル(json)
	public string filePath = "animdata.txt";

	//アニメーションデータ反映トラック
	public AnimationTrack[] animTracks;
	
	private object rawData;
	public object RawData { get{ return rawData; } }


	void Awake()
	{
		//ファイルの読み込み
		rawData = Json.Deserialize(File.ReadAllText(filePath));
	}

	void Start()
	{
		//アニメーションカーブを初期化
		InitAnimCurve();
	}

	void Update()
	{
	
	}


	//アニメーションカーブを初期化
	private void InitAnimCurve()
	{
		IDictionary data = rawData as IDictionary;
		
		foreach(AnimationTrack track in animTracks)
		{
			if(data.Contains(track.GetTrackName()))
			{
				List<float> keys = new List<float>();
				IList rawKeys = data[track.GetTrackName()] as IList;
				
				//C4Dの座標系を変換してキーフレームを取得
				foreach(object key in rawKeys)
					keys.Add(ConvertC4DSpace(track.type, Convert.ToSingle(key)));
				
				//アニメーションカーブに反映
				track.curve = new AnimationCurve();
				for(int i = 0; i < keys.Count; i++)
					track.curve.AddKey((float)i / (float)keys.Count, keys[i]);
			}
		}
	}

	//カーブを選択取得
	public AnimationCurve GetCurve(AnimationTrack.TRACK_TYPE type)
	{
		AnimationCurve curve = null;
		foreach(AnimationTrack track in animTracks)
		{
			if(track.type == type)
			{
				curve = track.curve;
				break;
			}
		}
		return curve;
	}
	
	//キーフレーム最大値を取得
	public int GetTotalFrame()
	{
		int totalFrame = 0;
		foreach(AnimationTrack track in animTracks)
		{
			if(totalFrame < track.curve.keys.Length)
				totalFrame = track.curve.keys.Length;
		}
		return totalFrame;
	}

	//C4Dの座標系を変換
	private float ConvertC4DSpace(AnimationTrack.TRACK_TYPE type, float keyValue)
	{
		switch(type)
		{
			case AnimationTrack.TRACK_TYPE.POS_X: keyValue = keyValue / 100.0f * -1.0f; break;
			case AnimationTrack.TRACK_TYPE.POS_Y: keyValue = keyValue / 100.0f; break;
			case AnimationTrack.TRACK_TYPE.POS_Z: keyValue = keyValue / 100.0f * -1.0f; break;
			case AnimationTrack.TRACK_TYPE.ROT_X: keyValue = keyValue * Mathf.Rad2Deg * -1.0f; break;
			case AnimationTrack.TRACK_TYPE.ROT_Y: keyValue = 180.0f - (keyValue * Mathf.Rad2Deg); break;
			case AnimationTrack.TRACK_TYPE.ROT_Z: keyValue = keyValue * Mathf.Rad2Deg * -1.0f; break;
			default: break;
		}

		return keyValue;
	}
}