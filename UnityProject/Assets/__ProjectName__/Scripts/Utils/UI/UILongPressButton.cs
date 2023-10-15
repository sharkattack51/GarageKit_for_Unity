using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTrigger))]
public class UILongPressButton : MonoBehaviour
{
    public float holdTime = 3.0f;

    private EventTrigger evtTrig;
    private float holdInTime;

    private bool isHold = false;
    private bool isUpdate = true;

    public Action OnHoldButton;


    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {
        evtTrig = this.gameObject.GetComponent<EventTrigger>();

        EventTrigger.Entry onPointerDown = new EventTrigger.Entry();
        onPointerDown.eventID = EventTriggerType.PointerDown;
        onPointerDown.callback.AddListener((e) => {
            isHold = true;
            holdInTime = Time.timeSinceLevelLoad;
        });
        evtTrig.triggers.Add(onPointerDown);

        EventTrigger.Entry onPointerUp = new EventTrigger.Entry();
        onPointerUp.eventID = EventTriggerType.PointerUp;
        onPointerUp.callback.AddListener((e) => {
            isHold = false;
            isUpdate = true;
        });
        evtTrig.triggers.Add(onPointerUp);
    }

    protected virtual void Update()
    {
        if(isUpdate)
        {
            if(isHold)
            {
                if(Time.timeSinceLevelLoad - holdInTime >= holdTime)
                {
                    OnHoldButton?.Invoke();
                    isUpdate = false;
                }
            }
            else
                holdInTime = Time.timeSinceLevelLoad;
        }
    }
}
