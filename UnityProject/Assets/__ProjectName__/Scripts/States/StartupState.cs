using UnityEngine;
using System.Collections;
using GarageKit;

public class StartupState : StateBase
{
	public override void StateStart(object context)
	{
		base.StateStart(context);
		
		// to WAIT state
		AppMain.Instance.sceneStateManager.ChangeState("WAIT");
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