using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GarageKit;

public class TimelinedExampleState : TimelinedSceneStateBase
{
    public Text sceneText;


    public override void StateStart(object context)
    {
        // set duration
        this.durationSec = 30;

        base.StateStart(context);

        // timeline action sample
        this.actionList = new TimelineEventActionList();
        this.actionList.Add(0.0f, () => {
            sceneText.text = "this is [TimelinedExampleState] scene.\nstart.";
        });
        this.actionList.Add(5.0f, () => {
            sceneText.text = "this is [TimelinedExampleState] scene.\n5 seconds elapsed";
        });
        this.actionList.Add(10.0f, () => {
            sceneText.text = "this is [TimelinedExampleState] scene.\n10 seconds elapsed";
        });
        this.actionList.Add(20.0f, () => {
            sceneText.text = "this is [TimelinedExampleState] scene.\n20 seconds elapsed";
        });
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
    }

    public override void StateExit()
    {
        base.StateExit();
    }

    public override void OnStateTimer(GameObject sender)
    {
        base.OnStateTimer(sender);

        sceneText.text = "this is [TimelinedExampleState] scene.\nfinish.";
    }
}
