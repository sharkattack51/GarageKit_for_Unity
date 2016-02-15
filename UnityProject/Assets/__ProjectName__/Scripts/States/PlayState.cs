using UnityEngine;
using System.Collections;

public class PlayState : StateBase
{
	public TextMesh sceneText;
	public TextMesh messageText;
	public TextMesh timerText;
	
	
	public override void StateStart()
	{
		base.StateStart();
		
		sceneText.text = "this is [Play] scene.";
		messageText.text = "[Space] : timer start";
	}
	
	public override void StateUpdate()
	{
		base.StateUpdate();
		
		if(AppMain.Instance.timeManager.gameTimer.timer.IsRunning)
			timerText.text = AppMain.Instance.timeManager.gameTimer.timer.CurrentTime.ToString();
		else
			timerText.text = "";
	}
	
	public override void StateExit()
	{
		base.StateExit();
	}
}