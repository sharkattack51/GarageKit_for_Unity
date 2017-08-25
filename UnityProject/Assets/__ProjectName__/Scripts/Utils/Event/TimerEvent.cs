using UnityEngine;
using System.Collections;

/*
 * タイマーイベントを設定する
 */ 

/*	How To TimerEvent Script
 * 
 *	GameObject obj = new GameObject("TimerEventObject"); 
 * 	obj.AddComponent<TimerEvent>();
 * 	obj.GetComponent<TimerEvent>().OnTimerEvent += new TimerEvent.OnTimerDelegate(OnTimerFunction);
 * 	obj.GetComponent<TimerEvent>().OnCompleteTimerEvent += new TimerEvent.OnCompleteTimerDelegate(OnCompFunction);
 * 	obj.GetComponent<TimerEvent>().StartTimer(10);
 * 
 * 	---
 * 	OnTimerFunction(GameObject senderObject,int fixedTime){ Debug.Log("on timer"); }
 * 	OnCompFunction(GameObject senderObject){ Debug.Log("on complete timer"); }
 */

public class TimerEvent : MonoBehaviour
{
	private bool isStarted = false;
	public bool IsStarted { get{ return isStarted; } }
	
	private bool isRunning = false;
	public bool IsRunning { get{ return isRunning; } }
	
	private int currentTime = 0;
	public int CurrentTime { get{ return currentTime; } }

	public int ElapsedTime { get{ return startTime - currentTime; } }
	public float ElapsedTimeRaw { get{ return ElapsedTime + Mathf.Min(decimalSec, 0.999999999f); } }

	private int startTime;
	private bool autoDestroy;
	//private int repeatCount;
	private float decimalSec = 0.0f;
	
	private string timerName;
	
	//TODO : OnTimerを秒数指定に変更してリピートできるようにする
	
	
#region Event
	
	/// <summary>
	/// OnTimerイベント
	/// </summary>
	public delegate void OnTimerDelegate(GameObject senderObject, int fixedTime);
	public event OnTimerDelegate OnTimer;
	protected void InvokeOnTimer()
	{
		if(OnTimer != null)
			OnTimer(this.gameObject, currentTime);
	}
	
	/// <summary>
	/// OnCompleteTimerイベント
	/// </summary>
	public delegate void OnCompleteTimerDelegate(GameObject senderObject);
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
		//オブジェクト名に現在時間を設定
		this.gameObject.name = timerName + " ["+ currentTime.ToString() + "]";

		// 経過時間少数点以下を加算
		decimalSec += Time.fixedDeltaTime;
	}
	
	
#region タイマーの操作

	/// <summary>
	/// タイマーをスタートする
	/// </summary>
	public void StartTimer(int countTime, float delayTime = 0.0f, bool autoDestroy = false)
	{
		StartTimer(countTime, autoDestroy);

		StartCoroutine(DelayStart(delayTime));
	}

	private void StartTimer(int countTime, bool autoDestroy)
	{
		this.startTime = countTime;
		this.currentTime = this.startTime;
		this.autoDestroy = autoDestroy;
		
		if(!isRunning)
		{
			isRunning = true;
			StartCoroutine("CountTimer");
		}
		
		isStarted = true;
	}

	private IEnumerator DelayStart(float delay)
	{
		StopTimer(); // 一旦停止

		yield return new WaitForSeconds(delay);

		ResumeTimer();
	}
	
	/// <summary>
	/// タイマーを再開する
	/// </summary>
	public void ResumeTimer()
	{
		StartTimer(currentTime, autoDestroy);
	}
	
	/// <summary>
	/// タイマーを停止する
	/// </summary>
	public void StopTimer()
	{
		if(isRunning)
		{
			isRunning = false;
			StopCoroutine("CountTimer");
		}
	}
	
	/// <summary>
	/// タイマーをリセットする
	/// </summary>
	public void ResetTimer()
	{
		StopTimer();
		
		if(isStarted)
			StartTimer(this.startTime, 0.0f, this.autoDestroy);
	}
	
#endregion
	
	
	/// <summary>
	/// カウント処理
	/// </summary>
	/// <returns>
	/// A <see cref="IEnumerator"/>
	/// </returns>
	IEnumerator CountTimer()
	{	
		while(true)
		{
			yield return new WaitForSeconds(1.0f);
			
			//onTimerイベント
			InvokeOnTimer();
			
			currentTime--;
			decimalSec = 0.0f;
		
			if(currentTime < 0)
				break;
		}
		
		//時間を0にする
		currentTime = 0;
		
		//onCompleteTimerイベント
		InvokeOnCompleteTimer();
	}
}
