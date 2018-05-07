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
		
		public TimerEvent mainTimer { get{ return (timerEvents.Length > 0) ? timerEvents[0] : null; } }
		
		
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
		}
	}
}