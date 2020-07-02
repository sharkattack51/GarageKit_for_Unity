using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GarageKit;

public class MultipleScene0State : MultipleSceneStateBase, ISequentialState
{
    public override void StateStart(object context)
    {
        base.StateStart(context);
    }

    public override void SceneLoaded()
    {
        base.SceneLoaded();

        // access public member
        (this.SceneRepository as MultipleScene0Repository).messageText.text
            = string.Format("current scene is\n\"{0}\"", this.loadSceneName);

        // find by variable name
        GameObject testCube = this.SceneRepository?.FindByVarName<GameObject>("testCube");
        testCube.GetComponent<Renderer>().material.color = Color.red;
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
        AppMain.Instance.sceneStateManager.ChangeState("SCENE_1");
    }

    public void ToPrevState()
    {

    }
}
