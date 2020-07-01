using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarageKit
{
    public interface ITimelinedSceneState
    {
        void Pause();
        void Resume();
        void OnStateTimer(GameObject sender);
    }
}
