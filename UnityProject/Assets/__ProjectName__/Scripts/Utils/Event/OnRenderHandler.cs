using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class OnPostRenderActions : MonoBehaviour
{
    public Action OnPreRenderAction;
    public Action OnPostRenderAction;

    void OnPreRender()
    {
        if(OnPreRenderAction != null)
            OnPreRenderAction();
    }

    void OnPostRender()
    {
        if(OnPostRenderAction != null)
            OnPostRenderAction();
    }
}
