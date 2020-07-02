using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GarageKit;

public class MultipleScene2State : MultipleSceneAsyncStateBase, ISequentialState
{
    public override void StateStart(object context)
    {
        base.StateStart(context);
    }

    public override void SceneLoaded()
    {
        base.SceneLoaded();

        // access public member
        (this.SceneRepository as MultipleScene2Repository).messageText.text
            = string.Format("current scene is\n\"{0}\"", this.loadSceneName);

        // find by variable name
        GameObject testCapsule = this.SceneRepository?.FindByVarName<GameObject>("testCapsule");
        testCapsule.GetComponent<Renderer>().material.color = Color.yellow;
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        if(Input.GetKeyDown(KeyCode.Space))
            ToNextState();
    }

    public override void StateExit()
    {
        base.StateExit();
    }

    public void ToNextState()
    {
        AppMain.Instance.sceneStateManager.ChangeState("SCENE_0");
    }

    public void ToPrevState()
    {

    }
}
