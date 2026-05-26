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
        public bool useGraphy = false;

        private bool isShowDebug;
        public bool IsShowDebug { get{ return isShowDebug; } }

        private GameObject ingameDebugConsole;
        private GameObject graphy;


        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();

            VisibleMouseCursor.showCursor = Application.isEditor || ApplicationSetting.Instance.GetBool("UseMouse");

            isDebug = ApplicationSetting.Instance.GetBool("IsDebug");
            isShowDebug = isDebug;

            ingameDebugConsole = GameObject.Find("IngameDebugConsole");
            graphy = GameObject.Find("[Graphy]");

            if((Application.isEditor && isDebug) || (!Application.isEditor && (Debug.isDebugBuild || isDebug)))
            {
                if(useIngameDebugConsole)
                {
                    if(ingameDebugConsole == null)
                    {
                        Debug.LogWarning(
                            "DebugManager :: package not found. recommend using the [IngameDebugConsole]. please install with OpenUPM. and re-open unity.\n> openupm add com.yasirkula.ingamedebugconsole");
                    }
                    else
                        ingameDebugConsole.SetActive(isShowDebug);
                }

                if(useGraphy)
                {
                    if(graphy == null)
                    {
                        Debug.LogWarning(
                            "DebugManager :: package not found. recommend using the [Graphy]. please install with OpenUPM. and re-open unity.\n> openupm add com.tayx.graphy");
                    }
                    else
                        graphy.SetActive(isShowDebug);
                }
            }
            else
            {
                if(ingameDebugConsole != null)
                    ingameDebugConsole.SetActive(false);

                if(graphy != null)
                    graphy.SetActive(false);
            }
        }

        protected override void Update()
        {
            base.Update();

            if((Application.isEditor && isDebug) || (!Application.isEditor && (Debug.isDebugBuild || isDebug)))
                this.gameObject.name = "DebugManager [DEBUG]";
        }


        // Toggle debug infomation
        public void ToggleShowDebugView()
        {
            isShowDebug = !isShowDebug;

            if(ingameDebugConsole != null)
                ingameDebugConsole.SetActive(isShowDebug);

            if(graphy != null)
                graphy.SetActive(isShowDebug);
        }
    }
}
