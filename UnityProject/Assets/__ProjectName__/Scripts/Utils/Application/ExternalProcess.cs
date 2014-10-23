using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System;

/// <summary>
/// 外部プロセスを起動する
/// </summary>

//プロセス情報データ
[Serializable]
public class ProcessData
{
	public bool use = true;
	public string exePath = "";
	public string argument = "";
	public bool startupOnStart = true;
	
	private bool isRunning = false;
	public bool IsRunning { get{ return isRunning; } set{ isRunning = value; } }
}

public class ExternalProcess : MonoBehaviour
{
	//singleton
	private static ExternalProcess instance;
	public static ExternalProcess Instance { get{ return instance; } }
	
	//プロセス情報リスト
	public ProcessData[] processes;
	
	//実行プロセスリスト
	private List<Process> processList = new List<Process>();
	public List<Process> ProcessList { get{ return processList; } }
	
	
	void Awake()
	{
		instance = this;
	}
	
	void Start()
	{
		//スタート時自動起動
		foreach(ProcessData procData in processes)
		{
			if(procData.startupOnStart)
				StartProcess(procData);
		}
	}
	
	void OnApplicationQuit()
	{
		try
		{
			//プロセスの終了
			foreach(Process proc in processList)
			{
				proc.Kill();
				proc.Dispose();	
			}
			
			processList = new List<Process>();
		}
		catch(Exception e)
		{
			UnityEngine.Debug.Log("ExternalProcess :: process close error - " + e.Message);
		}
	}
	
	
	/// <summary>
	/// プロセスを実行
	/// </summary>
	public void StartProcess(ProcessData procData)
	{
		if(procData.use && !procData.IsRunning)
		{
			FileInfo fileInfo = new FileInfo(procData.exePath);
			if(fileInfo.Exists)
			{
				Process proc = new Process();
				proc.StartInfo.FileName = procData.exePath;
				
				//引数設定
				if(procData.argument != "")
					proc.StartInfo.Arguments = procData.argument;
				
				//ウィンドウスタイル設定
				if(!bool.Parse(ApplicationSetting.Instance.Data["IsDebug"]))
					proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
				
				try
				{
					proc.Start();
					
					processList.Add(proc);
					procData.IsRunning = true;
				}
				catch(Exception e)
				{
					UnityEngine.Debug.Log("ExternalProcess :: process start error - " + e.Message);
				}
			}
		}
	}
}