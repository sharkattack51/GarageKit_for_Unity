using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/*
 * シーンStateを管理する
 */ 
public class SceneStateManager : ManagerBase
{
	//State管理用
	public enum SceneState
	{
		STARTUP = 0,
		WAIT,
		PLAY,
		RESULT
	}
	
	private Dictionary<SceneState, Type> stateTable = new Dictionary<SceneState, Type>()
	{
		{ SceneState.STARTUP, typeof(StartupState) },
		{ SceneState.WAIT, typeof(WaitState) },
		{ SceneState.PLAY, typeof(PlayState) },
		{ SceneState.RESULT, typeof(ResultState) }
	};

	private SceneState currentState;
	public SceneState CurrentState { get{ return currentState; } }

	private IState state = null;


	protected override void Awake()
	{
		base.Awake();
	}

	protected override void Start()
	{
		base.Start();
	}

	protected override void Update()
	{
		base.Update();

		//シーン名表示
		this.gameObject.name = "SceneStateManager [" + currentState.ToString() + "]";

		//Stateの更新処理
		if(state != null)
			state.StateUpdate();
	}
	
	
#region Stageの管理

	//アプリケーションをスタートする
	public void InitState()
	{
		//スタートアップシーン
		ChangeState(SceneState.STARTUP);
	}
	
	//Stateを変更する
	public void ChangeState(SceneState sceneState)
	{
		//前Stateの終了処理
		if(state != null)
			state.StateExit();

		currentState = sceneState;

		//新規Stateのセット
		state = Activator.CreateInstance(stateTable[sceneState]) as IState;

		//新規Stateの初期化処理
		state.StateStart();
	}

	//Stateをリセットする
	public void ResetState()
	{
		//強制GC
		System.GC.Collect();
		Resources.UnloadUnusedAssets();
		
		//StateをWaitに変更
		ChangeState(SceneState.WAIT);
	}
	
#endregion
}