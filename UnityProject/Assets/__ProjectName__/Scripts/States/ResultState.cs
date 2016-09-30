using UnityEngine;
using System.Collections;

public class ResultState : AsyncStateBase
{
	public TextMesh sceneText;
	public TextMesh messageText;
	public TextMesh timerText;
	
	
	public override void StateStart(object context)
	{
		base.StateStart(context);
		
		sceneText.text = "this is [Result] scene.";
		messageText.text = "push [Space] return to [Wait] scene.";
		timerText.text = "timer is end";
	}
	
	public override void StateUpdate()
	{
		base.StateUpdate();
	}
	
	public override void StateExit()
	{
		// フェード機能をON
		Fader.EnableFade();

		base.StateExit();
	}
}
