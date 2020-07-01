using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarageKit
{
    public interface IState
    {
        void StateStart(object context);
        void StateUpdate();
        void StateExit();
    }
}
