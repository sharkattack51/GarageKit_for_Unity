using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarageKit
{
    public interface ISceneRepository
    {
        T FindByVarName<T>(string varName);
    }
}
