using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarageKit
{
    public class IStateBehaviour : MonoBehaviour, IState
    {
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
