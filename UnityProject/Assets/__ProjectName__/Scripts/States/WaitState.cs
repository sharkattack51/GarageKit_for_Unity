using UnityEngine;
using System.Collections;

public class WaitState : StateBase
{
	public TextMesh sceneText;
	public TextMesh messageText;
	public TextMesh timerText;
	
	
	public override void StateStart()
	{
		base.StateStart();
		
		sceneText.text = "this is [Wait] scene.";
		messageText.text = "[Space] : next scene";
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