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

        private GameObject ingameDebugConsole;
        private GameObject graphy;


        protected override void Awake()
        {
            base.Awake();
        }
        
        protected override void Start()
        {
            base.Start();

            isDebug = ApplicationSetting.Instance.GetBool("IsDebug");
            VisibleMouseCursor.showCursor = Application.isEditor || ApplicationSetting.Instance.GetBool("UseMouse");

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
                }

                if(useGraphy)
                {
                    if(graphy == null)
                    {
                        Debug.LogWarning(
                            "DebugManager :: package not found. recommend using the [Graphy]. please install with OpenUPM. and re-open unity.\n> openupm add com.tayx.graphy");
                    }
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
            if(ingameDebugConsole != null)
                ingameDebugConsole.SetActive(!ingameDebugConsole.activeSelf);

            if(graphy != null)
                graphy.SetActive(!graphy.activeSelf);
        }
    }
}
