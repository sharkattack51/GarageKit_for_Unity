using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace GarageKit
{
    public class SceneRepositoryBase : MonoBehaviour, ISceneRepository
    {
        public T FindByVarName<T>(string varName)
        {
            Type type = this.GetType();
            FieldInfo field = type.GetField(varName);
            if(field != null)
                return (T)field.GetValue(this);
            else
                return default(T);
        }
    }
}
