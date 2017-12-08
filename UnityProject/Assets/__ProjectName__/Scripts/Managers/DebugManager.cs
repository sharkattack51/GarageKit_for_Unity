//#define USE_STATSMONITOR

using UnityEngine;
using System.Collections;

/*
 * デバッグ情報を管理する
 */
namespace GarageKit
{
	[RequireComponent(typeof(VisibleMouseCursor))]
	public class DebugManager : ManagerBase
	{
		public bool IsDebug = true;
		public bool UseDebugConsole = false;

#if USE_STATSMONITOR
		private StatsMonitor.StatsMonitor statsMonitor;
#endif

		
		protected override void Awake()
		{
			base.Awake();
		}
		
		protected override void Start()
		{
			base.Start();

			// 設定値を取得
			IsDebug = ApplicationSetting.Instance.GetBool("IsDebug");
			
			// デバッグ用コンソール初期設定
			UseDebugConsole = ApplicationSetting.Instance.GetBool("UseDebugConsole");
			DebugConsole.IsOpen = UseDebugConsole;

#if USE_STATSMONITOR
			// StatesMonitor
			statsMonitor = FindObjectOfType<StatsMonitor.StatsMonitor>();
			if(statsMonitor != null)
				statsMonitor.gameObject.SetActive(IsDebug);
#else
			this.gameObject.AddComponent<MemoryProfiler>();
			MemoryProfiler.useHUD = IsDebug;
			this.gameObject.AddComponent<FrameRateUtil>();
			FrameRateUtil.useHUD = IsDebug;
#endif
			
			// マウスカーソル表示設定
			if(Application.platform == RuntimePlatform.WindowsEditor)
				VisibleMouseCursor.showCursor = true;
			else
				VisibleMouseCursor.showCursor = ApplicationSetting.Instance.GetBool("UseMouse");
		}

		protected override void Update()
		{
			base.Update();
		}


		// デバッグ情報のトグル
		public void ToggleShowDebugView()
		{
			// デバッグコンソールの表示
			if(UseDebugConsole)
				DebugConsole.IsOpen = !DebugConsole.IsOpen;

#if USE_STATSMONITOR
			// StatesMonitor
			if(statsMonitor != null)
				statsMonitor.gameObject.SetActive(IsDebug);
#else
			MemoryProfiler.useHUD = IsDebug;
			FrameRateUtil.useHUD = IsDebug;
#endif
		}
	}
}
