using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * タイマーイベントを設定する
 */ 

/*	How To TimerEvent Script
 * 
 *	GameObject obj = new GameObject("TimerEventObject"); 
 * 	obj.AddComponent<TimerEvent>();
 * 	obj.GetComponent<TimerEvent>().OnTimer += (sender, sec) => {};
 * 	obj.GetComponent<TimerEvent>().OnCompleteTimer += (sender) => {};
 * 	obj.GetComponent<TimerEvent>().StartTimer(10);
 */
namespace GarageKit
{
    public class TimerEvent : MonoBehaviour
    {
        private bool isStarted = false;
        public bool IsStarted { get{ return isStarted; } }

        private bool isRunning = false;
        public bool IsRunning { get{ return isRunning; } }

        // 残り時間
        private int currentTime = 0;
        public int CurrentTime { get{ return currentTime; } }

        // 経過時間
        private float elapsedTime = 0.0f;
        public float ElapsedTime { get{ return elapsedTime; } }

        private int startTime;
        private bool autoDestroy;

        private string timerName;

#region Event
        /// <summary>
        /// OnTimerイベント
        /// </summary>
        public delegate void OnTimerDelegate(GameObject sender, int sec);
        public event OnTimerDelegate OnTimer;
        protected void InvokeOnTimer()
        {
            if(OnTimer != null)
                OnTimer(this.gameObject, currentTime);
        }

        /// <summary>
        /// OnCompleteTimerイベント
        /// </summary>
        public delegate void OnCompleteTimerDelegate(GameObject sender);
        public event OnCompleteTimerDelegate OnCompleteTimer;
        protected void InvokeOnCompleteTimer()
        {
            isRunning = false;

            if(OnCompleteTimer != null)
                OnCompleteTimer(this.gameObject);

            //自動削除
            if(autoDestroy)
                Destroy(this.gameObject);
        }
    #endregion


        void Awake()
        {
            timerName = this.gameObject.name;
        }

        void Start()
        {	

        }

        void Update()
        {
            if(isRunning)
            {
                elapsedTime += Time.deltaTime;

                if(startTime - Mathf.FloorToInt(elapsedTime) != currentTime)
                {
                    // onTimerイベント
                    InvokeOnTimer();
                }

                if(startTime - Mathf.FloorToInt(elapsedTime) == -1)
                {
                    isRunning = false;
                    currentTime = 0;

                    // onCompleteTimerイベント
                    InvokeOnCompleteTimer();
                }
                else
                    currentTime = startTime - Mathf.FloorToInt(elapsedTime);
            }

            // オブジェクト名に現在時間を設定
            this.gameObject.name = timerName + " ["+ currentTime.ToString() + "]";
        }


#region タイマーの操作
        public void StartTimer(int countTime, float delayTime = 0.0f, bool autoDestroy = false)
        {
            StartTimer(countTime, autoDestroy);

            if(delayTime > 0.0f)
                StartCoroutine(DelayStart(delayTime));
        }

        private IEnumerator DelayStart(float delay)
        {
            StopTimer(); // 一旦停止

            yield return new WaitForSeconds(delay);

            ResumeTimer();
        }

        private void StartTimer(int countTime, bool autoDestroy)
        {
            this.elapsedTime = 0.0f;

            this.startTime = countTime;
            this.currentTime = countTime + 1;
            this.autoDestroy = autoDestroy;

            this.isRunning = true;
            this.isStarted = true;
        }

        public void StopTimer()
        {
            isRunning = false;
        }

        public void ResumeTimer()
        {
            isRunning = true;
        }

        public void ResetTimer(bool andStart)
        {
            isRunning = false;
            elapsedTime = 0.0f;

            if(andStart)
                StartTimer(this.startTime, this.autoDestroy);
        }
#endregion
    }
}
