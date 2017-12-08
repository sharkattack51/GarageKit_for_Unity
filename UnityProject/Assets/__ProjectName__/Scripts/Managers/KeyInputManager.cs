using UnityEngine;
using System.Collections;
using System;

/*
 * キーボード入力を管理する
 */
namespace GarageKit
{
	public class KeyInputManager : ManagerBase
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
				{				
					// アプリケーション終了
					Application.Quit();
				}
			}
			
			// D
			if(Input.GetKeyDown(KeyCode.D))
			{
				Debug.Log("press key D : Visible Debug View");
				DebugConsole.Log("press key D : Visible Debug View");
				
				// デバッグ表示のトグル
				DebugManager debugManager = AppMain.Instance.debugManager;
				debugManager.IsDebug = !debugManager.IsDebug;
				debugManager.ToggleShowDebugView();
			}
			
			// C
			if(Input.GetKeyDown(KeyCode.C))
			{
				Debug.Log("press key C : Clear DebugConsole");
				DebugConsole.Log("press key C : Clear DebugConsole");
				
				// デバッグコンソールのクリア
				DebugConsole.Clear();
			}
			
			// G
			if(Input.GetKeyDown(KeyCode.G))
			{
				Debug.Log("press key G : System GC Collect");
				DebugConsole.Log("press key G : System GC Collect");
				
				// 強制CG
				System.GC.Collect();
			}
			
			// R
			if(Input.GetKeyDown(KeyCode.R))
			{
				Debug.Log("press key R : Reload ApplicationSetting");
				DebugConsole.Log("press key R : Reload ApplicationSetting");
				
				// 設定ファイルの再読み込み
				ApplicationSetting.Instance.LoadXML();
			}
			
			// Space
			if(Input.GetKeyDown(KeyCode.Space))
			{
				Debug.Log("press key Space : Change Stage");
				DebugConsole.Log("press key Space : Change Stage");
				
				// ステージの変更
				SceneStateManager sceneStateManager = AppMain.Instance.sceneStateManager;
				TimeManager timeManager = AppMain.Instance.timeManager;
				if(sceneStateManager.CurrentState == SceneStateManager.SceneState.STARTUP)
					sceneStateManager.ChangeState(SceneStateManager.SceneState.WAIT);
				else if(sceneStateManager.CurrentState == SceneStateManager.SceneState.WAIT)
					sceneStateManager.ChangeState(SceneStateManager.SceneState.PLAY);
				else if(sceneStateManager.CurrentState == SceneStateManager.SceneState.PLAY)
					timeManager.timerEvents[0].StartTimer(ApplicationSetting.Instance.GetInt("GameTime"));
				else if(sceneStateManager.CurrentState == SceneStateManager.SceneState.RESULT)
					sceneStateManager.ChangeAsyncState(SceneStateManager.SceneState.WAIT);
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
