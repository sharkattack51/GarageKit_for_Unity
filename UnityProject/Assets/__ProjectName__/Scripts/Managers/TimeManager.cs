using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Manage all timer controll in scene
 */
namespace GarageKit
{
	public class TimeManager : ManagerBase
	{
		[Header("Scene timers")]
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