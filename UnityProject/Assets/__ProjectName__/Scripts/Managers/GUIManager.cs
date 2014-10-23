using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// GUI要素オブジェクト
/// </summary>
[Serializable]
public class GuiObject
{	
	public GameObject gameObject;
	
	//非表示位置
	public Vector3 hidePosition = new Vector3(0.0f, -20.0f, 0.0f);
	
	public void SetShow()
	{
		if(gameObject != null)
			gameObject.transform.localPosition = Vector3.zero;
	}
	
	public void SetHide()
	{
		if(gameObject != null)
			gameObject.transform.localPosition = hidePosition;
	}
}

public class GUIManager : MonoBehaviour
{	
	//singleton
	private static GUIManager instance;
	public static GUIManager Instance { get{ return instance; } }
	
	//GUI要素オブジェクト設定　ここを追加設定する
	public GuiObject guiStartupScene;
	public GuiObject guiWaitScene;
	public GuiObject guiPlayScene;
	public GuiObject guiResultScene;
	
	
	void Awake()
	{
		instance = this;
	}

	void Start()
	{		
	
	}
	
	void Update()
	{
		if(StageManager.Instance.CurrentStage == StageManager.StageState.PLAY)
		{
			if(TimeManager.Instance.gameTimer.timer.IsRunning)
				GameObject.Find("TimerText").GetComponent<TextMesh>().text = TimeManager.Instance.gameTimer.timer.CurrentTime.ToString();
			else
				GameObject.Find("TimerText").GetComponent<TextMesh>().text = "";
		}
		else
			GameObject.Find("TimerText").GetComponent<TextMesh>().text = "";
	}
	
	
	#region GUIの管理 ここを編集する
	
	/// <summary>
	/// シーン切り替え時にGUIを変更する
	/// </summary>
	/// <param name="sceneState">
	/// A <see cref="SceneState"/>
	/// </param>
	public void SetGUI(StageManager.StageState stageState)
	{
		switch(stageState)
		{
			case StageManager.StageState.STARTUP:
				guiStartupScene.SetShow();
				guiWaitScene.SetHide();
				guiPlayScene.SetHide();
				guiResultScene.SetHide();
				break;
			
			case StageManager.StageState.WAIT:
				guiStartupScene.SetHide();
				guiWaitScene.SetShow();
				guiPlayScene.SetHide();
				guiResultScene.SetHide();
				break;
			
			case StageManager.StageState.PLAY:
				guiStartupScene.SetHide();
				guiWaitScene.SetHide();
				guiPlayScene.SetShow();
				guiResultScene.SetHide();
				break;
			
			case StageManager.StageState.RESULT:
				guiStartupScene.SetHide();
				guiWaitScene.SetHide();
				guiPlayScene.SetHide();
				guiResultScene.SetShow();
				break;
				
			default: break;
		}
	}
	
	#endregion
}