using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// タイマーイベント設定用オブジェクト
/// </summary>
[Serializable]
public class TimerEventObject
{
	
	public TimerEvent timer;
	public int time;
	public float delayTime;
}

public class TimeManager : MonoBehaviour
{	
	//singleton
	private static TimeManager instance;
	public static TimeManager Instance { get{ return instance; } }
	
	//タイマーイベントオブジェクト設定　ここを追加設定する
	public TimerEventObject gameTimer;
	
	
	void Awake()
	{
		instance = this;
	}

	void Start()
	{
		//各タイマーごとに設定する
		
		//設定値を取得
		ApplicationSetting setting = ApplicationSetting.Instance;
		if(setting.IsValid)
		{
			if(setting.Data.ContainsKey("GameTime")) 
				gameTimer.time = int.Parse(setting.Data["GameTime"]);
		}
		
		//タイマーイベントを設定
		gameTimer.timer.OnCompleteTimer += GameTimer_OnCompleteTimer;
	}

	void Update()
	{
	
	}
	
	
	#region タイマー操作 各タイマーごとに設定する
	
	/// <summary>
	/// ゲームプレイタイマースタート
	/// </summary>
	public void GameTimer_Start()
	{
		gameTimer.timer.StartTimer(gameTimer.time, false);
		
		//スタートを一旦Delayする
		StartCoroutine(Delay_GameTimer_Start(gameTimer.delayTime));
	}
	
	private IEnumerator Delay_GameTimer_Start(float delay)
	{
		gameTimer.timer.StopTimer();
		
		yield return new WaitForSeconds(delay);
		
		gameTimer.timer.ResumeTimer();
	}
	
	/// <summary>
	/// ゲームプレイタイマーストップ
	/// </summary>
	public void GameTimer_Stop()
	{
		gameTimer.timer.StopTimer();
	}
	
	/// <summary>
	/// ゲームプレイタイマー完了イベント
	/// </summary>
	/// <param name="senderObject">
	/// A <see cref="GameObject"/>
	/// </param>
	private void GameTimer_OnCompleteTimer(GameObject senderObject)
	{
		StageManager.Instance.ChangeStage(StageManager.StageState.RESULT);
	}
	
	/// <summary>
	/// 現在のゲームプレイタイムを取得
	/// </summary>
	/// <returns>
	/// A <see cref="System.Int32"/>
	/// </returns>
	public int GetCurrentTime()
	{
		return gameTimer.timer.CurrentTime;
	}
	
	#endregion
}