using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GarageKit;

public class WaitState : AsyncStateBase, ISequentialState
{
	[Header("WaitState")]
	public Text sceneText;
	public Text timerText;
	public Text messageText;
	
	
	public override void StateStart(object context)
	{
		base.StateStart(context);
		
		sceneText.text = "this is [Wait] state.";
		timerText.text = "";
		messageText.text = "push [Space] : next state";
	}
	
	public override void StateUpdate()
	{
		base.StateUpdate();
	}
	
	public override void StateExit()
	{
		base.StateExit();
	}


	public void ToNextState()
	{
		AppMain.Instance.sceneStateManager.ChangeAsyncState("PLAY");
	}

	public void ToPrevState()
	{
		// pass
	}
}