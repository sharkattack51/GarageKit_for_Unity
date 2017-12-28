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
	public string configFile = "Log4Net.config";
	public bool configFromStreamingAssets = true;
	
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
		FINISH,
		ERROR
	}
	
	
	void Awake()
	{
		instance = this;
	}
	
	void Start()
	{
		if(configFromStreamingAssets)
			configFile = Path.Combine(Application.streamingAssetsPath, configFile);

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
				logger.Info("[STARTUP]"+ "\t" + "application is started");
				break;
			
			case LOG_EVENT_TYPE.FINISH:
				logger.Info("[FINISH]"+ "\t" + "application is finished");
				break;
			
			case LOG_EVENT_TYPE.ERROR:
				logger.Error("[ERROR]"+ "\t" + option);
				break;
			
			default: break;
		}
	}
}