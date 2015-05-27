using UnityEngine;
using System.Collections;

public class StartupState : IState
{
	private GuiController gui;

	public void StateStart()
	{
		gui = GameObject.FindObjectOfType<GuiController>();
		gui.SetGUI(SceneStateManager.SceneState.STARTUP);

		//StateをWAITに変更
		AppMain.Instance.sceneStateManager.ChangeState(SceneStateManager.SceneState.WAIT);
	}

	public void StateUpdate()
	{
		gui.guiTimerText.GetComponent<TextMesh>().text = "";
	}

	public void StateExit()
	{

	}
}