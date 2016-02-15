using UnityEngine;
using System.Collections;

public class ResultState : StateBase
{
	public TextMesh sceneText;
	public TextMesh messageText;
	public TextMesh timerText;
	
	
	public override void StateStart()
	{
		base.StateStart();
		
		sceneText.text = "this is [Result] scene.";
		messageText.text = "Timer End";
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
