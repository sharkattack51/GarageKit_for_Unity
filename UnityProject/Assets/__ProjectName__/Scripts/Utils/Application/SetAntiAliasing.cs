using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * アンチエイリアスの設定
 */ 
namespace GarageKit
{
    public class SetAntiAliasing : MonoBehaviour
    {
        public int AA_2x;

        void Awake()
        {
            QualitySettings.antiAliasing = AA_2x;
        }
    }
}
