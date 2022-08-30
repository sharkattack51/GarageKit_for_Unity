using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GarageKit;

using DG.Tweening;

public class MultipleScene1State : MultipleSceneStateBase, ISequentialState
{
    public override void StateStart(object context)
    {
        base.StateStart(context);
    }

    public override void SceneLoaded()
    {
        base.SceneLoaded();

        // access public member
        (this.SceneRepository as MultipleScene1Repository).messageText.text
            = string.Format("current scene is\n\"{0}\"", this.loadSceneName);

        // find by variable name
        GameObject testSphere = this.SceneRepository?.FindByVarName<GameObject>("testSphere");
        testSphere.GetComponent<Renderer>().material.color = Color.blue;
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

        DOTween.KillAll();
    }

    public void ToNextState()
    {
        AppMain.Instance.sceneStateManager.ChangeState("SCENE_2");
    }

    public void ToPrevState()
    {

    }
}
