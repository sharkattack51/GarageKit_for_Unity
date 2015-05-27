using UnityEngine;
using System;
using System.Collections;

/*
 * タイマーを管理する
 */
public class TimeManager : ManagerBase
{
	//タイマーイベントオブジェクト設定
	public TimerEventObject gameTimer;
	
	
	protected override void Awake()
	{
		base.Awake();
	}

	protected override void Start()
	{
		base.Start();

		//各タイマー設定
		gameTimer.time = ApplicationSetting.Instance.GetInt("GameTime");
		gameTimer.timer.OnCompleteTimer += OnCompleteGameTimer;
	}

	protected override void Update()
	{
		base.Update();
	}
	

	//ゲームタイマー完了イベント
	private void OnCompleteGameTimer(GameObject senderObject)
	{
		AppMain.Instance.sceneStateManager.ChangeState(SceneStateManager.SceneState.RESULT);
	}
}