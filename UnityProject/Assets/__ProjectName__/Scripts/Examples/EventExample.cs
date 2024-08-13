using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GarageKit;

public class EventExample : MonoBehaviour
{
    // TimelineActionExample
    private float elapsedTime;
    private TimelineEventActionList actionList;

    // TimerEventExample
    public TimerEvent testTimer;

    // ButtonObjectEventExample
    public ButtonObjectEvent btnObj;

    // SevenTapLikeAndroidExample
    public SevenTapLikeAndroid sevenTap;


    void Awake()
    {

    }

    void Start()
    {
        // TimelineActionExample - init
        actionList = new TimelineEventActionList();
        actionList.Add(1.0f, () => {
            Debug.Log("action 1 sec");
        });
        actionList.Add(2.0f, () => {
            Debug.Log("action 2 sec");
        });
        actionList.Add(3.0f, () => {
            Debug.Log("action 3 sec");
        });

        // TimerEventExample - init
        testTimer.OnTimer += (sender, sec) => {
            if(sec == 5)
                Debug.Log("timer 5 sec");
        };
        testTimer.OnCompleteTimer += (sender) => {
            Debug.Log("timer complete 10 sec");
        };
        testTimer.StartTimer(10);

        // ButtonObjectEventExample - init
        btnObj.OnButton = () => {
            Debug.Log("click button object");
        };
        btnObj.OnPressButton = () => {
            btnObj.GetComponent<Renderer>().material.color = Color.red;
        };
        btnObj.OnReleaseButton = () => {
            btnObj.GetComponent<Renderer>().material.color = Color.white;
        };

        // SevenTapLikeAndroidExample - init
        sevenTap.OnSevenTap = () => {
            Debug.Log("seven click !!!!!!!");
        };
    }

    void Update()
    {
        // TimelineActionExample - time update
        elapsedTime += Time.deltaTime;
        actionList.Update(elapsedTime);
    }
}
