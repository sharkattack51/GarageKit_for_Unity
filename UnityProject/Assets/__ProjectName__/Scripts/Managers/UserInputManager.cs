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
                Debug.Log("Press Key [ESC]: Application Quit");
                
                if(Application.platform != RuntimePlatform.WindowsEditor)			
                    Application.Quit();
            }

            // D
            if(Input.GetKeyDown(KeyCode.D))
            {
                Debug.Log("Press Key [D]: Visible Debug View");
                
                DebugManager debugManager = AppMain.Instance.debugManager;
                debugManager.isDebug = !debugManager.isDebug;
                debugManager.ToggleShowDebugView();
            }

            // R
            if(Input.GetKeyDown(KeyCode.R))
            {
                Debug.Log("Press Key [R]: Reload ApplicationSetting");

                // Reload settings
                ApplicationSetting.Instance.LoadXML();
            }

            // Space
            if(Input.GetKeyDown(KeyCode.Space))
            {
                // SE
                AppMain.Instance.soundManager.Play("SE", "CLICK");

                // change State or Timer start
                PlayState state = AppMain.Instance.sceneStateManager.CurrentState.StateObj as PlayState;
                if(state != null)
                {
                    // PLAY state
                    Debug.Log("Press Key [SPACE]: Start Timer");
                    state.StartTimer();
                }
                else
                {
                    // WAIT or RESULT state
                    Debug.Log("Press Key [SPACE]: Change State");
                    ISequentialState seqState = AppMain.Instance.sceneStateManager.CurrentState.StateObj as ISequentialState;
                    if(seqState != null)
                        seqState.ToNextState();
                }
            }

            // Backspace
            if(Input.GetKeyDown(KeyCode.Backspace))
            {
                Debug.Log("Press Key [Backspace]: Screen Capture");

                // Screen Capture
                StartCoroutine(CaptureUtil.CaptureRect("capture.png", true));
            }
        }
    }
}
