using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Manage all user input
 */
namespace GarageKit
{
    public class UserInputManager : ManagerBase
    {
        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();

            // ESC
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log("press key ESC : Application Quit");
                
                if(Application.platform != RuntimePlatform.WindowsEditor)			
                    Application.Quit();
            }

            // D
            if(Input.GetKeyDown(KeyCode.D))
            {
                Debug.Log("press key D : Visible Debug View");
                
                DebugManager debugManager = AppMain.Instance.debugManager;
                debugManager.isDebug = !debugManager.isDebug;
                debugManager.ToggleShowDebugView();
            }

            // G
            if(Input.GetKeyDown(KeyCode.G))
            {
                Debug.Log("press key G : System GC Collect");

                System.GC.Collect();
            }

            // R
            if(Input.GetKeyDown(KeyCode.R))
            {
                Debug.Log("press key R : Reload ApplicationSetting");

                // Reload settings
                ApplicationSetting.Instance.LoadXML();
            }

            // Space
            if(Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("press key Space : Change State");

                // SE
                AppMain.Instance.soundManager.Play("SE", "CLICK");

                // change State or Timer start
                PlayState state = AppMain.Instance.sceneStateManager.CurrentState.StateObj as PlayState;
                if(state != null)
                    state.StartTimer(); // PLAY state
                else
                {
                    ISequentialState seqState = AppMain.Instance.sceneStateManager.CurrentState.StateObj as ISequentialState;
                    if(seqState != null)
                        seqState.ToNextState(); // WAIT or RESULT state
                }
            }
        }
    }
}
