using UnityEngine;
using System.Collections;

/*
 * デバッグ情報を管理する
 */
[RequireComponent(typeof(VisibleMouseCursor))]
[RequireComponent(typeof(FrameRateUtil))]
[RequireComponent(typeof(MemoryProfiler))]
public class DebugManager : ManagerBase
{
	public bool IsDebug = true;
	public bool UseDebugConsole = false;
	public bool DebugConsoleLogEnable = true;
	
	
	protected override void Awake()
	{
		base.Awake();
	}
	
	protected override void Start()
	{
		base.Start();

		//設定値を取得
		IsDebug = ApplicationSetting.Instance.GetBool("IsDebug");
		
		//デバッグ用コンソール初期設定
		UseDebugConsole = ApplicationSetting.Instance.GetBool("UseDebugConsole");
		DebugConsole.IsOpen = UseDebugConsole;
		
		DebugConsoleLogEnable = ApplicationSetting.Instance.GetBool("DebugConsoleLogEnable");
		DebugConsole.Instance._enable = DebugConsoleLogEnable;
		
		//FPS表示設定
		FrameRateUtil.useHUD = IsDebug;
		
		//メモリチェックの表紙設定
		MemoryProfiler.useHUD = IsDebug;
		
		//マウスカーソル表示設定
		if(Application.platform == RuntimePlatform.WindowsEditor)
			VisibleMouseCursor.showCursor = true;
		else
			VisibleMouseCursor.showCursor = ApplicationSetting.Instance.GetBool("UseMouse");
	}

	protected override void Update()
	{
		base.Update();
	}


	//デバッグ情報のトグル
	public void ToggleShowDebugView()
	{
		//デバッグコンソールの表示
		if(UseDebugConsole)
			DebugConsole.IsOpen = !DebugConsole.IsOpen;
		
		//FPS表示
		FrameRateUtil.useHUD = !FrameRateUtil.useHUD;
		
		//メモリ表示
		MemoryProfiler.useHUD = !MemoryProfiler.useHUD;
	}
}
