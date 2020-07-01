using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * マウスポインタの表示/非表示
 */
namespace GarageKit
{
    public class VisibleMouseCursor : MonoBehaviour
    {
        public static bool showCursor;

        void Update()
        {
            Cursor.visible = showCursor;
        }
    }
}
