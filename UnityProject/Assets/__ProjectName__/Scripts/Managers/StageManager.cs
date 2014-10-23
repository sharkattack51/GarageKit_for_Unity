using UnityEngine;
using System.Collections;

public class StageManager : MonoBehaviour
{
	//ステージ管理用
	public enum StageState
	{
		STARTUP = 0,	//起動時
		WAIT,			//待機
		PLAY,			//プレイ
		RESULT			//結果表示
	}
	
	//singleton
	private static StageManager instance;
	public static StageManager Instance { get{ return instance; } }
	
	private StageState currentStage;
	public StageState CurrentStage { get{ return currentStage; } }
	
	
	void Awake()
	{
		instance = this;
	}

	void Start()
	{
		//コンテンツをスタート
		InitStage();
	}

	void Update()
	{
		//シーン名表示
		this.gameObject.name = "StageManager [" + currentStage.ToString() + "]";
	}
	
	
	#region シーンの管理
	
	/// <summary>
	/// コンテンツをスタートする
	/// </summary>
	private void InitStage()
	{
		//スタートアップシーン
		ChangeStartupStage();
	}
	
	
	/// <summary>
	/// ステージを変更する
	/// </summary>
	public void ChangeStage(StageState stage)
	{
		switch(stage)
		{
			case StageState.STARTUP:
				ChangeStartupStage();
				break;
			
			case StageState.WAIT:
				ChangeWaitStage();
				break;
			
			case StageState.PLAY:
				ChangePlayStage();
				break;
			
			case StageState.RESULT:
				ChangeResultStage();
				break;
			
			default: break;
		}
	}
	
	private void ChangeStartupStage()
	{
		currentStage = StageState.STARTUP;
		
		GUIManager.Instance.SetGUI(StageState.STARTUP);
	}
	
	private void ChangeWaitStage()
	{
		currentStage = StageState.WAIT;
		
		GUIManager.Instance.SetGUI(StageState.WAIT);
	}
	
	private void ChangePlayStage()
	{
		currentStage = StageState.PLAY;
		
		GUIManager.Instance.SetGUI(StageState.PLAY);
	}
	
	private void ChangeResultStage()
	{
		currentStage = StageState.RESULT;
		
		GUIManager.Instance.SetGUI(StageState.RESULT);
	}
	
	
	/// <summary>
	/// リセット処理
	/// </summary>
	public void ResetStage()
	{
		//強制GC
		System.GC.Collect();
		Resources.UnloadUnusedAssets();
		
		//Waitにステージを変更
		ChangeWaitStage();
	}
	
	#endregion
}
