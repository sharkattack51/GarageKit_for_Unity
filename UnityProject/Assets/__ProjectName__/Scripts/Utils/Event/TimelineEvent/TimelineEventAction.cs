using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarageKit
{
    public class TimelineEventAction
    {
        public float time;
        public Action action;
        private bool done = false;
        public bool IsDone { get{ return done; } }


        public TimelineEventAction(float time, Action action)
        {
            this.time = time;
            this.action = action;
        }

        public void Done()
        {
            done = true;
        }

        public void Reset()
        {
            done = false;
        }
    }
}
