using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarageKit
{
    public interface ISequentialState
    {
        void ToNextState();
        void ToPrevState();
    }
}
