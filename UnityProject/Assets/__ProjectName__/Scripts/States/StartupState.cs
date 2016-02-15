using UnityEngine;
using System.Collections;

public class StartupState : StateBase
{
	public override void StateStart()
	{
		base.StateStart();
		
		// StateをWAITに変更
		AppMain.Instance.sceneStateManager.ChangeState(SceneStateManager.SceneState.WAIT);
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