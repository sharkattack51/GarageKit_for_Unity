using UnityEngine;
using System.Collections;
using GarageKit;

public class WaitState : AsyncStateBase
{
	public TextMesh sceneText;
	public TextMesh messageText;
	public TextMesh timerText;
	
	
	public override void StateStart(object context)
	{
		base.StateStart(context);
		
		sceneText.text = "this is [Wait] scene.";
		messageText.text = "push [Space] : next scene";
		timerText.text = "";
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