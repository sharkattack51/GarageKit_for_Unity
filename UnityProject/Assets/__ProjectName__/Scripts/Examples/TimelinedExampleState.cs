using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GarageKit;

public class TimelinedExampleState : TimelinedSceneStateBase
{
	public Text sceneText;


	public override void StateStart(object context)
	{
		base.StateStart(context);

		// set duration
		this.durationSec = 30;
		
		// timeline action sample
		this.actionList = new List<TimelineEventAction>();
		this.actionList.Add(new TimelineEventAction(0.0f, () => {
			sceneText.text = "this is [TimelinedExampleState] scene.\nstart.";
		}));
		this.actionList.Add(new TimelineEventAction(5.0f, () => {
			sceneText.text = "this is [TimelinedExampleState] scene.\n5 seconds elapsed";
		}));
		this.actionList.Add(new TimelineEventAction(10.0f, () => {
			sceneText.text = "this is [TimelinedExampleState] scene.\n10 seconds elapsed";
		}));
		this.actionList.Add(new TimelineEventAction(20.0f, () => {
			sceneText.text = "this is [TimelinedExampleState] scene.\n20 seconds elapsed";
		}));
	}
	
	public override void StateUpdate()
	{
		base.StateUpdate();
	}
	
	public override void StateExit()
	{
		base.StateExit();
	}

	public override void OnStateTimer(GameObject sender)
	{
		base.OnStateTimer(sender);

		sceneText.text = "this is [TimelinedExampleState] scene.\nfinish.";
	}
}
