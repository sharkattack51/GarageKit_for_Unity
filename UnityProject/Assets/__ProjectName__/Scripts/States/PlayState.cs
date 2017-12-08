using UnityEngine;
using System.Collections;
using GarageKit;

public class PlayState : StateBase
{
	public TextMesh sceneText;
	public TextMesh messageText;
	public TextMesh timerText;
	
	
	public override void StateStart(object context)
	{
		base.StateStart(context);
		
		sceneText.text = "this is [Play] scene.";
		messageText.text = "push [Space] : timer start";
	}
	
	public override void StateUpdate()
	{
		base.StateUpdate();
		
		if(AppMain.Instance.timeManager.timerEvents[0].IsRunning)
			timerText.text = AppMain.Instance.timeManager.timerEvents[0].CurrentTime.ToString();
		else
			timerText.text = "";
	}
	
	public override void StateExit()
	{
		base.StateExit();
	}
}