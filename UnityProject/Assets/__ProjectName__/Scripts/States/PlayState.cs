﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GarageKit;

public class PlayState : AsyncStateBase, ISequentialState
{
	[Header("PlayState")]
	public Text sceneText;
	public Text timerText;
	public Text messageText;
	

	void Start()
	{
		// タイマー設定
		AppMain.Instance.timeManager.mainTimer.OnCompleteTimer += OnCompleteGameTimer;
	}


	public override void StateStart(object context)
	{
		base.StateStart(context);
		
		sceneText.text = "this is [Play] state.";
		messageText.text = "push [Space] : timer start";
	}
	
	public override void StateUpdate()
	{
		base.StateUpdate();
		
		if(AppMain.Instance.timeManager.mainTimer.IsRunning)
			timerText.text = AppMain.Instance.timeManager.mainTimer.CurrentTime.ToString();
		else
			timerText.text = "";
	}
	
	public override void StateExit()
	{
		base.StateExit();
	}


	public void ToNextState()
	{
		AppMain.Instance.sceneStateManager.ChangeAsyncState("RESULT");
	}

	public void ToPrevState()
	{
		AppMain.Instance.sceneStateManager.ChangeAsyncState("WAIT");
	}


	// for TimerEvent
	public void StartTimer()
	{
		AppMain.Instance.timeManager.mainTimer.StartTimer(ApplicationSetting.Instance.GetInt("GameTime"));
	}

	private void OnCompleteGameTimer(GameObject sender)
	{
		ToNextState();
	}
}