using UnityEngine;
using System.Collections;
using System.IO;
using System;

/// <summary>
/// Log4Netのラッパークラス
/// </summary>

public class Log4Net : MonoBehaviour
{
	//singleton
	private static Log4Net instance;
	public static Log4Net Instance { get{ return instance; } }
	
	//設定ファイル
	public string configFile = ".\\Log\\Log4Net.config";
	
	//有効確認
	private bool isValid = false;
	public bool IsValid { get{ return isValid; } }
	
	//Loggerオブジェクト
	private log4net.ILog logger;
	public log4net.ILog Logger { get{ return logger; } }
	
	//ログイベントタイプ
	public enum LOG_EVENT_TYPE
	{
		STARTUP = 0,
		LOGIN_COMPLETE,
		LOGIN_FAILED,
		GETNODE_COMPLETE,
		GETNODE_FAILED,
		CHANGE_LANGUAGE,
		SELECT,
		DOWNLOAD
	}
	
	
	void Awake()
	{
		instance = this;
	}
	
	void Start()
	{
		FileInfo fileInfo = new FileInfo(configFile);
		if(fileInfo.Exists)
		{
			//設定ファイルの読み込み
			log4net.Config.XmlConfigurator.ConfigureAndWatch(fileInfo);
			
			//Loggerオブジェクトを作成
			logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
			
			isValid = true;
		}
	}
	
	
	/// <summary>
	/// ログ出力
	/// </summary>
	public void WriteLog(LOG_EVENT_TYPE type, string option = "")
	{	
		switch(type)
		{
			case LOG_EVENT_TYPE.STARTUP:
				logger.Info("[START]"+ "\t" + "River is started.");
				break;
			
			case LOG_EVENT_TYPE.LOGIN_COMPLETE:
				logger.Info("[LOGIN]"+ "\t" + "login complete.");
				break;
			
			case LOG_EVENT_TYPE.LOGIN_FAILED:
				logger.Error("[ERROR]"+ "\t" + "login failed.");
				break;
			
			case LOG_EVENT_TYPE.GETNODE_COMPLETE:
				logger.Info("[LOGIN]"+ "\t" + "get node complete.");
				break;
			
			case LOG_EVENT_TYPE.GETNODE_FAILED:
				logger.Error("[ERROR]"+ "\t" + "get node failed.");
				break;
			
			case LOG_EVENT_TYPE.CHANGE_LANGUAGE:
				logger.Info("[CHANGE_LANGUAGE]"+ "\t" + option);
				break;
			
			case LOG_EVENT_TYPE.SELECT:
				logger.Info("[SELECT]"+ "\t" + option);
				break;
			
			case LOG_EVENT_TYPE.DOWNLOAD:
				logger.Info("[DOWNLOAD]"+ "\t" + option);
				break;
			
			default: break;
		}
	}
}