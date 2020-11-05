using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GarageKit
{
    public class SevenTapLikeAndroid : MonoBehaviour, IPointerDownHandler
    {
        public Action OnSevenTap;

        private static int TAP_COUNT = 7;

        private int currentCount = 0;
        private float countStartTime;


        void Update()
        {
            // カウント開始から3秒経過でリセット
            if(Time.timeSinceLevelLoad - countStartTime >= 3.0f)
            {
                currentCount = 0;
                return;
            }

            if(currentCount == TAP_COUNT)
            {
                if(OnSevenTap != null)
                    OnSevenTap();

                currentCount = 0;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (currentCount == 0)
                countStartTime = Time.timeSinceLevelLoad;

            currentCount++;
        }
    }
}
