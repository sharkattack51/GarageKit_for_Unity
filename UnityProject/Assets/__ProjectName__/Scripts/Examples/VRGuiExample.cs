using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GarageKit;

using Cysharp.Threading.Tasks;

public class VRGuiExample : MonoBehaviour
{
    public UIFadeTelop telop;


    async void Start()
    {
        telop.StartTelop("Drag to change camera direction.");

        await UniTask.Delay(TimeSpan.FromSeconds(6));

        telop.StartTelop("Drag to change camera direction.");
    }
}
