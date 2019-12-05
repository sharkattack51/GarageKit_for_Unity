using UnityEngine;
using System.Collections;
using GarageKit;

public class StartupState : StateBase
{
	public override void StateStart(object context)
	{
		base.StateStart(context);

		// In this state, you can initialize the application.
		// You can start the application safely.
		// Even if it is unnecessary, it is better to leave it better.
		
		// srart app
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