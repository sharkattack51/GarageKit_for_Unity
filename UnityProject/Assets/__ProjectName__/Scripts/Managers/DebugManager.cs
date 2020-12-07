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
        public bool isDebug = true;
        public bool useIngameDebugConsole = true;

        private GameObject ingameDebugConsole;

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

            isDebug = ApplicationSetting.Instance.GetBool("IsDebug");
            
            if(isDebug && useIngameDebugConsole)
            {
                ingameDebugConsole = GameObject.Find("IngameDebugConsole");
                if(ingameDebugConsole == null)
                {
                    Debug.LogWarning(
                        "DebugManager :: package not found. recommend using the [IngameDebugConsole]. please install with OpenUPM. and re-open unity.\n> openupm add com.yasirkula.ingamedebugconsole");
                }
            }

#if USE_STATSMONITOR
            // Display StatesMonitor
            statsMonitor = FindObjectOfType<StatsMonitor.StatsMonitor>();
            if(statsMonitor != null)
                statsMonitor.gameObject.SetActive(isDebug);
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

            if(isDebug)
                this.gameObject.name = "DebugManager [DEBUG]";
        }


        // Toggle debug infomation
        public void ToggleShowDebugView()
        {
            // Display IngameDebugConsole
            if(ingameDebugConsole != null)
                ingameDebugConsole.SetActive(!ingameDebugConsole.activeSelf);

#if USE_STATSMONITOR
            // Display StatesMonitor
            if(statsMonitor != null)
                statsMonitor.gameObject.SetActive(IsDebug);
#endif
        }
    }
}
