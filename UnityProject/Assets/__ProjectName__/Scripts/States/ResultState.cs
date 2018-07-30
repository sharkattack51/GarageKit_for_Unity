using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GarageKit;

public class ResultState : AsyncStateBase
{
	public Text sceneText;
	public Text timerText;
	public Text messageText;
	
	
	public override void StateStart(object context)
	{
		base.StateStart(context);
		
		sceneText.text = "this is [Result] scene.";
		timerText.text = "timer is end";
		messageText.text = "push [Space] return to [Wait] scene.";
	}
	
	public override void StateUpdate()
	{
		base.StateUpdate();
	}
	
	public override void StateExit()
	{
		base.StateExit();
	}
}
