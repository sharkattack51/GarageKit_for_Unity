using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GarageKit;

public class PlayState : AsyncStateBase, ISequentialState
{
    [Header("PlayState")]
    public Text sceneText;
    public Text timerText;
    public Text messageText;


    void Start()
    {
        // タイマー設定
        AppMain.Instance.timeManager.mainTimer.OnCompleteTimer += OnCompleteGameTimer;
    }


    public override void StateStart(object context)
    {
        base.StateStart(context);
        
        sceneText.text = "this is [Play] state.";
        messageText.text = "push [Space] : timer start";
    }
    
    public override void StateUpdate()
    {
        base.StateUpdate();
        
        if(AppMain.Instance.timeManager.mainTimer.IsRunning)
            timerText.text = AppMain.Instance.timeManager.mainTimer.CurrentTime.ToString();
        else
            timerText.text = "";
    }

    public override void StateExit()
    {
        base.StateExit();
    }


    public void ToNextState()
    {
        Debug.Log("Change Async State to [RESULT] with Fade.");
        AppMain.Instance.sceneStateManager.ChangeAsyncState("RESULT");
    }

    public void ToPrevState()
    {
        // pass
    }


    // for TimerEvent
    public void StartTimer()
    {
        AppMain.Instance.timeManager.mainTimer.StartTimer(ApplicationSetting.Instance.GetInt("GameTime"));
    }

    private void OnCompleteGameTimer(GameObject sender)
    {
        ToNextState();
    }
}
