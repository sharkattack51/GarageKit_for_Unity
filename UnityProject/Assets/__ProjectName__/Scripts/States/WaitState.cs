using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GarageKit;

public class WaitState : AsyncStateBase, ISequentialState
{
    [Header("WaitState")]
    public StageManagedObject view;


    public override void StateStart(object context)
    {
        base.StateStart(context);

        StageManagedObject.Alone(view);
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
        Debug.Log("Change State to [PLAY] with No Fade.");
        AppMain.Instance.sceneStateManager.ChangeState("PLAY");
    }

    public void ToPrevState()
    {
        // pass
    }
}
