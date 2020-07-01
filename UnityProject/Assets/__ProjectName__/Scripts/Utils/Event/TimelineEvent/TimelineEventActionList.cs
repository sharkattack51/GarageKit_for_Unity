using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarageKit
{
    public class TimelineEventActionList
    {
        private List<TimelineEventAction> actionList;


        public TimelineEventActionList()
        {
            actionList = new List<TimelineEventAction>();
        }

        public void Update(float time)
        {
            // イベントAction実行
            foreach(TimelineEventAction action in actionList)
            {
                if(!action.IsDone && time >= action.time)
                {
                    if(action.action != null)
                        action.action();

                    action.Done();
                }
            }
        }

        public void Add(float time, Action action)
        {
            actionList.Add(new TimelineEventAction(time, action));
        }

        public void Clear()
        {
            // イベントActionリセット
            foreach(TimelineEventAction action in actionList)
                action.Reset();
            actionList = new List<TimelineEventAction>();
        }
    }
}
