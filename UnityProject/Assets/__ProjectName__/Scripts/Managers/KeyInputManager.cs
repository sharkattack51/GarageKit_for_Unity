using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// キーボード入力を管理する
/// </summary>

public class KeyInputManager : MonoBehaviour
{
	void Awake()
	{
		
	}
	
	void Start()
	{
		
	}
	
	void Update()
	{
		//ESC
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			Debug.Log("press key ESC : Application Quit");
			DebugConsole.Log("press key ESC : Application Quit");
			
			if(Application.platform != RuntimePlatform.WindowsEditor)
			{				
				//アプリケーション終了
				Application.Quit();
			}
		}
		
		//D
		if(Input.GetKeyDown(KeyCode.D))
		{
			Debug.Log("press key D : Visible Debug View");
			DebugConsole.Log("press key D : Visible Debug View");
			
			//デバッグ表示のトグル
			DebugManager.Instance.IsDebug = !DebugManager.Instance.IsDebug;
			DebugManager.Instance.ToggleShowDebugView();
		}
		
		//C
		if(Input.GetKeyDown(KeyCode.C))
		{
			Debug.Log("press key C : Clear DebugConsole");
			DebugConsole.Log("press key C : Clear DebugConsole");
			
			//デバッグコンソールのクリア
			DebugConsole.Clear();
		}
		
		//W
		if(Input.GetKeyDown(KeyCode.W))
		{
			Debug.Log("press key W : Change DebugConsole Mode");
			DebugConsole.Log("press key W : Change DebugConsole Mode");
			
			//デバッグコンソールのモードを切り替える
			if(DebugConsole.IsOpen)
			{
				if(DebugConsole.Instance.mode == DebugConsole.Mode.Log)
					DebugConsole.Instance.mode = DebugConsole.Mode.WatchVars;
				else
					DebugConsole.Instance.mode = DebugConsole.Mode.Log;
			}
		}
		
		//G
		if(Input.GetKeyDown(KeyCode.G))
		{
			Debug.Log("press key G : System GC Collect");
			DebugConsole.Log("press key G : System GC Collect");
			
			//強制CG
			System.GC.Collect();
		}
		
		//R
		if(Input.GetKeyDown(KeyCode.R))
		{
			Debug.Log("press key R : Reload ApplicationSetting");
			DebugConsole.Log("press key R : Reload ApplicationSetting");
			
			//設定ファイルの再読み込み
			ApplicationSetting.Instance.LoadXML();
		}
		
		//Space
		if(Input.GetKeyDown(KeyCode.Space))
		{
			Debug.Log("press key Space : Change Stage");
			DebugConsole.Log("press key Space : Change Stage");
			
			//ステージの変更
			if(StageManager.Instance.CurrentStage == StageManager.StageState.STARTUP)
				StageManager.Instance.ChangeStage(StageManager.StageState.WAIT);
			else if(StageManager.Instance.CurrentStage == StageManager.StageState.WAIT)
				StageManager.Instance.ChangeStage(StageManager.StageState.PLAY);
			else if(StageManager.Instance.CurrentStage == StageManager.StageState.PLAY)
				TimeManager.Instance.GameTimer_Start();
			else if(StageManager.Instance.CurrentStage == StageManager.StageState.RESULT)
				StageManager.Instance.ChangeStage(StageManager.StageState.WAIT);
		}
	}
	
	void OnGUI()
	{
		if(DebugManager.Instance.IsDebug)
			GUI.Window(0, new Rect(0.0f, 0.0f, 250.0f, 200.0f), DrawWindow, "- DEBUG KEYS -");
	}
	
	private void DrawWindow(int windowId)
	{
		GUILayout.BeginVertical();
		GUILayout.Label("ESC : Application Quit");
		GUILayout.Label("D : Visible Debug View");
		GUILayout.Label("C : Clear DebugConsole");
		GUILayout.Label("W : Change DebugConsole Mode");
		GUILayout.Label("G : System GC Collect");
		GUILayout.Label("R : Reload ApplicationSetting");
		GUILayout.EndVertical();
	}
}
