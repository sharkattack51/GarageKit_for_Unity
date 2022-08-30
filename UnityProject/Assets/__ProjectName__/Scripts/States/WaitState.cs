using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
        Debug.Log("Change State to [PLAY] with No Fade.");
        AppMain.Instance.sceneStateManager.ChangeState("PLAY");
    }

    public void ToPrevState()
    {
        // pass
    }
}
