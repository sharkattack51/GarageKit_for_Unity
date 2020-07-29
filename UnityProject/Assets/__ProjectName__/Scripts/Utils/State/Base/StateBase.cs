using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarageKit
{
    public class StateBase : MonoBehaviour, IState
    {
        protected bool updateEnable = true;
        public bool IsUpdateEnable { get{ return updateEnable; } }


        public virtual void StateStart(object context)
        {

        }

        public virtual void StateUpdate()
        {

        }

        public virtual void StateExit()
        {

        }
    }
}
