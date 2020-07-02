using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Log4Netのラッパークラス
public class Log4Net : MonoBehaviour
{
    private static Log4Net logger;
    public static Log4Net Logger { get{ return logger; } }

    public string outputLogDir = "./Log";

    // Loggerオブジェクト
    private log4net.ILog nativeLogger;
    public log4net.ILog NativeLogger { get{ return nativeLogger; } }

    // ログイベントタイプ
    public enum LOG_EVENT_TYPE
    {
        STARTUP = 0,
        INFO,
        QUIT,
        ERROR
    }


    void Awake()
    {
        logger = this;
    }

    void Start()
    {
        InitializeLog4Net();
    }

    void Update()
    {

    }

    void OnDestroy()
    {
        log4net.LogManager.Shutdown();
        nativeLogger = null;
    }


    private void InitializeLog4Net()
    {
        // ログローテーション設定
        nativeLogger = log4net.LogManager.GetLogger("Logger");

        log4net.Appender.RollingFileAppender rollingFileAppender = new log4net.Appender.RollingFileAppender();
        rollingFileAppender.RollingStyle = log4net.Appender.RollingFileAppender.RollingMode.Date;
        rollingFileAppender.AppendToFile = true;
        rollingFileAppender.StaticLogFileName = false;
        rollingFileAppender.File = Path.GetFullPath(outputLogDir) + "/";
        rollingFileAppender.DatePattern = "yyyyMMdd\".log\"";
        rollingFileAppender.Layout = new log4net.Layout.PatternLayout("%date{yyyy/MM/dd  HH:mm:ss} %message%newline");
        log4net.Filter.LevelRangeFilter filter = new log4net.Filter.LevelRangeFilter();
        filter.LevelMin = log4net.Core.Level.Info;
        filter.LevelMax = log4net.Core.Level.Fatal;
        rollingFileAppender.AddFilter(filter);

        rollingFileAppender.ActivateOptions();
        ((log4net.Repository.Hierarchy.Logger)nativeLogger.Logger).AddAppender(rollingFileAppender);
        ((log4net.Repository.Hierarchy.Logger)nativeLogger.Logger).Hierarchy.Configured = true;
    }

    // ログ出力
    public void Log(LOG_EVENT_TYPE type, string message = "")
    {	
        switch(type)
        {
            case LOG_EVENT_TYPE.STARTUP:
                nativeLogger.Info("[STARTUP]"+ "\t" + message);
                break;

            case LOG_EVENT_TYPE.INFO:
                nativeLogger.Info("[INFO]"+ "\t" + message);
                break;

            case LOG_EVENT_TYPE.QUIT:
                nativeLogger.Info("[QUIT]"+ "\t" + message);
                break;

            case LOG_EVENT_TYPE.ERROR:
                nativeLogger.Error("[ERROR]"+ "\t" + message);
                break;

            default: break;
        }
    }

    public void Log(string message)
    {
        Log(LOG_EVENT_TYPE.INFO, message);
    }
}
