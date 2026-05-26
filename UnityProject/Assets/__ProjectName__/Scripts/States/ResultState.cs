using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GarageKit;

public class ResultState : AsyncStateBase, ISequentialState
{
    [Header("ResultState")]
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
        Debug.Log("Change Async State to [WAIT] with Fade.");
        AppMain.Instance.sceneStateManager.ChangeAsyncState("WAIT");
    }

    public void ToPrevState()
    {
        // pass
    }
}
