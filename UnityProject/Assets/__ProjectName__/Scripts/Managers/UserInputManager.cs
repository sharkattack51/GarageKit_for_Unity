using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Manage all user input
 */
namespace GarageKit
{
	public class UserInputManager : ManagerBase
	{
		protected override void Awake()
		{
			base.Awake();
		}
		
		protected override void Start()
		{
			base.Start();
		}
		
		protected override void Update()
		{
			base.Update();

			// ESC
			if(Input.GetKeyDown(KeyCode.Escape))
			{
				Debug.Log("press key ESC : Application Quit");
				DebugConsole.Log("press key ESC : Application Quit");
				
				if(Application.platform != RuntimePlatform.WindowsEditor)			
					Application.Quit();
			}
			
			// D
			if(Input.GetKeyDown(KeyCode.D))
			{
				Debug.Log("press key D : Visible Debug View");
				DebugConsole.Log("press key D : Visible Debug View");
				
				DebugManager debugManager = AppMain.Instance.debugManager;
				debugManager.IsDebug = !debugManager.IsDebug;
				debugManager.ToggleShowDebugView();
			}
			
			// C
			if(Input.GetKeyDown(KeyCode.C))
			{
				Debug.Log("press key C : Clear DebugConsole");
				DebugConsole.Log("press key C : Clear DebugConsole");
				
				DebugConsole.Clear();
			}
			
			// G
			if(Input.GetKeyDown(KeyCode.G))
			{
				Debug.Log("press key G : System GC Collect");
				DebugConsole.Log("press key G : System GC Collect");
				
				System.GC.Collect();
			}
			
			// R
			if(Input.GetKeyDown(KeyCode.R))
			{
				Debug.Log("press key R : Reload ApplicationSetting");
				DebugConsole.Log("press key R : Reload ApplicationSetting");
				
				// Reload settings
				ApplicationSetting.Instance.LoadXML();
			}
			
			// Space
			if(Input.GetKeyDown(KeyCode.Space))
			{
				Debug.Log("press key Space : Change State");
				DebugConsole.Log("press key Space : Change State");

				// SE
				AppMain.Instance.soundManager.Play("SE", "CLICK");
				
				// change state
				SceneStateManager sceneStateManager = AppMain.Instance.sceneStateManager;
				TimeManager timeManager = AppMain.Instance.timeManager;
				if(sceneStateManager.CurrentState.StateName == "WAIT")
					sceneStateManager.ChangeAsyncState("PLAY");
				else if(sceneStateManager.CurrentState.StateName == "PLAY")
					timeManager.mainTimer.StartTimer(ApplicationSetting.Instance.GetInt("GameTime"));
				else if(sceneStateManager.CurrentState.StateName == "RESULT")
					sceneStateManager.ChangeAsyncState("WAIT");
			}
		}
		
		void OnGUI()
		{
			if(AppMain.Instance.debugManager.IsDebug)
				GUI.Window(0, new Rect(0.0f, 0.0f, 250.0f, 200.0f), DrawWindow, "- DEBUG KEYS -");
		}
		
		private void DrawWindow(int windowId)
		{
			GUILayout.BeginVertical();
			GUILayout.Label("ESC : Application Quit");
			GUILayout.Label("D : Visible Debug View");
			GUILayout.Label("C : Clear DebugConsole");
			GUILayout.Label("G : System GC Collect");
			GUILayout.Label("R : Reload ApplicationSetting");
			GUILayout.EndVertical();
		}
	}
}
