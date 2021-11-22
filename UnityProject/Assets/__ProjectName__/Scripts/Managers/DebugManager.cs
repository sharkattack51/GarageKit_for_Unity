using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Manage debug information
 */
namespace GarageKit
{
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

            if(isDebug || (!Application.isEditor && Debug.isDebugBuild))
            {
                if(useIngameDebugConsole)
                {
                    ingameDebugConsole = GameObject.Find("IngameDebugConsole");
                    if(ingameDebugConsole == null)
                    {
                        Debug.LogWarning(
                            "DebugManager :: package not found. recommend using the [IngameDebugConsole]. please install with OpenUPM. and re-open unity.\n> openupm add com.yasirkula.ingamedebugconsole");
                    }
                }

                if(useGraphy)
                {
                    graphy = GameObject.Find("[Graphy]");
                    if(graphy == null)
                    {
                        Debug.LogWarning(
                            "DebugManager :: package not found. recommend using the [Graphy]. please install with OpenUPM. and re-open unity.\n> openupm add com.tayx.graphy");
                    }
                }
            }
        }

        protected override void Update()
        {
            base.Update();

            if(isDebug || (!Application.isEditor && Debug.isDebugBuild))
                this.gameObject.name = "DebugManager [DEBUG]";
        }


        // Toggle debug infomation
        public void ToggleShowDebugView()
        {
            // Display IngameDebugConsole
            if(ingameDebugConsole != null)
                ingameDebugConsole.SetActive(!ingameDebugConsole.activeSelf);

            // Display Graphy
            if(graphy != null)
                graphy.SetActive(!graphy.activeSelf);
        }
    }
}
