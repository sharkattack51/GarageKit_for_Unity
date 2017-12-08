using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/*
 * シーンStateを管理する
 */
namespace GarageKit
{
	public class SceneStateManager : ManagerBase
	{
		// State管理用
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

		private IState stateObj = null;
		public IState CurrentStateObj { get{ return stateObj; } }

		private bool stateChanging = false;
		public bool StateChanging { get{ return stateChanging; } }

		private bool isAsync = false;


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

			// シーン名表示
			this.gameObject.name = "SceneStateManager [" + currentState.ToString() + "]";

			// Stateの更新処理
			if(stateObj != null)
				stateObj.StateUpdate();
		}
		
		
#region Stageの管理

		// アプリケーションをスタートする
		public void InitState()
		{
			// スタートアップシーン
			ChangeState(SceneState.STARTUP);
		}

		// Stateを変更する
		public void ChangeState(SceneState sceneState, object context = null)
		{
			isAsync = false;

			StartCoroutine(ChangeStateCoroutine(sceneState, context));
		}

		// Stateを変更する
		public void ChangeAsyncState(SceneState sceneState, object context = null)
		{
			if(!stateChanging)
			{
				isAsync = true;
				stateChanging = true;

				StartCoroutine(ChangeStateCoroutine(sceneState, context));
			}
		}

		private IEnumerator ChangeStateCoroutine(SceneState sceneState, object context)
		{
			// 前Stateの終了処理
			if(stateObj != null)
				stateObj.StateExit();
			
			while(isAsync)
				yield return null;
			
			currentState = sceneState;

			// 新規Stateのセット
			stateObj = this.gameObject.GetComponentInChildren(stateTable[sceneState]) as IState;

			// 新規Stateの初期化処理
			stateObj.StateStart(context);

			stateChanging = false;
		}

		// Stateをリセットする
		public void ResetState()
		{
			// 強制GC
			System.GC.Collect();
			Resources.UnloadUnusedAssets();
			
			//StateをWaitに変更
			ChangeState(SceneState.WAIT);
		}
		
#endregion


		// 非同期でのステート変更用
		public void SyncState()
		{
			isAsync = false;
		}
	}
}
