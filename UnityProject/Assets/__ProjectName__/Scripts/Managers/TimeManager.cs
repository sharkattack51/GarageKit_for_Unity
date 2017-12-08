using UnityEngine;
using System;
using System.Collections;

/*
 * タイマーを管理する
 */
namespace GarageKit
{
	public class TimeManager : ManagerBase
	{
		// タイマーイベントオブジェクト設定
		public TimerEvent[] timerEvents;
		
		
		protected override void Awake()
		{
			base.Awake();
		}

		protected override void Start()
		{
			base.Start();

			// 各タイマー設定
			timerEvents[0].OnCompleteTimer += OnCompleteGameTimer;
		}

		protected override void Update()
		{
			base.Update();
		}
		

		// タイマー完了イベント
		private void OnCompleteGameTimer(GameObject senderObject)
		{
			timerEvents[0].OnCompleteTimer -= OnCompleteGameTimer;
			AppMain.Instance.sceneStateManager.ChangeState(SceneStateManager.SceneState.RESULT);
		}
	}
}