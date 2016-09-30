using UnityEngine;
using System.Collections;

public class StartupState : StateBase
{
	public override void StateStart(object context)
	{
		base.StateStart(context);
		
		// StateをWAITに変更
		AppMain.Instance.sceneStateManager.ChangeState(SceneStateManager.SceneState.WAIT);
	}
	
	public override void StateUpdate()
	{
		base.StateUpdate();
	}

	public override void StateExit()
	{
		// フェード機能をOFF
		Fader.DisableFade();

		base.StateExit();
	}
}