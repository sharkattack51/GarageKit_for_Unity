using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarageKit
{
    public interface IVRSceneState
    {
        void ResetCurrentState();
        void SetStagingObjects();
    }
}
