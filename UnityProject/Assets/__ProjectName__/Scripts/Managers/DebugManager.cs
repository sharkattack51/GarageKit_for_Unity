using UnityEngine;
using System.Collections;

[RequireComponent(typeof(VisibleMouseCursor))]
[RequireComponent(typeof(FrameRateUtil))]
[RequireComponent(typeof(MemoryProfiler))]
public class DebugManager : MonoBehaviour
{	
	//singleton
	private static DebugManager instance;
	public static DebugManager Instance { get{ return instance; } }
	
	public bool IsDebug = true;
	public bool UseDebugConsole = false;
	public bool DebugConsoleLogEnable = true;
	
	
	void Awake()
	{
		instance = this;
	}
	
	void Start()
	{		
		//設定値を取得
		IsDebug = bool.Parse(ApplicationSetting.Instance.Data["IsDebug"]);
		
		//デバッグ用コンソール初期設定
		UseDebugConsole = bool.Parse(ApplicationSetting.Instance.Data["UseDebugConsole"]);
		DebugConsole.IsOpen = UseDebugConsole;
		
		DebugConsoleLogEnable = bool.Parse(ApplicationSetting.Instance.Data["DebugConsoleLogEnable"]);
		DebugConsole.Instance._enable = DebugConsoleLogEnable;
		
		//FPS表示設定
		FrameRateUtil.useHUD = IsDebug;
		
		//メモリチェックの表紙設定
		MemoryProfiler.useHUD = IsDebug;
		
		//マウスカーソル表示設定
		if(Application.platform == RuntimePlatform.WindowsEditor)
			VisibleMouseCursor.showCursor = true;
		else
			VisibleMouseCursor.showCursor = bool.Parse(ApplicationSetting.Instance.Data["UseMouse"]);
	}

	void Update()
	{
	
	}
	
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
