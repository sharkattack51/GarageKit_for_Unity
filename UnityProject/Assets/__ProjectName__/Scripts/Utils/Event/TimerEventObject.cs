using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// タイマーイベント設定用オブジェクト
/// </summary>
[Serializable]
public class TimerEventObject
{
	public TimerEvent timer;
	public int time;
	public float delayTime;
	
	//タイマースタート
	public void Start()
	{
		timer.StartTimer(time, false);
		
		//スタートを一旦Delayする
		AppMain.Instance.timeManager.StartCoroutine(DelayStart(delayTime));
	}
	
	//ディレイスタート
	private IEnumerator DelayStart(float delay)
	{
		timer.StopTimer();
		
		yield return new WaitForSeconds(delay);
		
		timer.ResumeTimer();
	}
	
	//タイマーストップ
	public void Stop()
	{
		timer.StopTimer();
	}
	
	//現在の時間を取得
	public int GetCurrentTime()
	{
		return timer.CurrentTime;
	}
}