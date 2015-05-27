using UnityEngine;
using System.Collections;

public class PlayState : IState
{
	private GuiController gui;

	public void StateStart()
	{
		gui = GameObject.FindObjectOfType<GuiController>();
		gui.SetGUI(SceneStateManager.SceneState.PLAY);
	}
	
	public void StateUpdate()
	{
		TimeManager timeManager = AppMain.Instance.timeManager;
		if(timeManager.gameTimer.timer.IsRunning)
			gui.guiTimerText.GetComponent<TextMesh>().text = timeManager.gameTimer.timer.CurrentTime.ToString();
		else
			gui.guiTimerText.GetComponent<TextMesh>().text = "";
	}
	
	public void StateExit()
	{
		
	}
}