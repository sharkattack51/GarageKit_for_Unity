//#define USE_STATSMONITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Manage debug information
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

            IsDebug = ApplicationSetting.Instance.GetBool("IsDebug");
            
            UseDebugConsole = ApplicationSetting.Instance.GetBool("UseDebugConsole");
            DebugConsole.IsOpen = UseDebugConsole;

#if USE_STATSMONITOR
            // Display StatesMonitor
            statsMonitor = FindObjectOfType<StatsMonitor.StatsMonitor>();
            if(statsMonitor != null)
                statsMonitor.gameObject.SetActive(IsDebug);
#else
            this.gameObject.AddComponent<MemoryProfiler>();
            MemoryProfiler.useHUD = IsDebug;
            this.gameObject.AddComponent<FrameRateUtil>();
            FrameRateUtil.useHUD = IsDebug;
#endif
            
            // Display mouse cursor
            if(Application.platform == RuntimePlatform.WindowsEditor)
                VisibleMouseCursor.showCursor = true;
            else
                VisibleMouseCursor.showCursor = ApplicationSetting.Instance.GetBool("UseMouse");
        }

        protected override void Update()
        {
            base.Update();
        }


        // Toggle debug infomation
        public void ToggleShowDebugView()
        {
            // Display DebugConsole
            if(UseDebugConsole)
                DebugConsole.IsOpen = !DebugConsole.IsOpen;

#if USE_STATSMONITOR
            // Display StatesMonitor
            if(statsMonitor != null)
                statsMonitor.gameObject.SetActive(IsDebug);
#else
            MemoryProfiler.useHUD = IsDebug;
            FrameRateUtil.useHUD = IsDebug;
#endif
        }
    }
}
