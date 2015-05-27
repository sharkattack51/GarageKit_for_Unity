using UnityEngine;
using System.Collections;

public class ResultState : IState
{
	private GuiController gui;

	public void StateStart()
	{
		gui = GameObject.FindObjectOfType<GuiController>();
		gui.SetGUI(SceneStateManager.SceneState.RESULT);
	}
	
	public void StateUpdate()
	{
		gui.guiTimerText.GetComponent<TextMesh>().text = "";
	}
	
	public void StateExit()
	{
		
	}
}
