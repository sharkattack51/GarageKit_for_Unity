using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

// Log4Net warapper for unity
public class Log4Net : MonoBehaviour
{
    private static Log4Net logger;
    public static Log4Net Logger { get{ return logger; } }

    public string outputLogDir = "./Log";

    // native logger instance
    private log4net.ILog nativeLogger;
    public log4net.ILog NativeLogger { get{ return nativeLogger; } }

    public enum FILTER_LEVEL
    {
        VERBOSE = 0,
        DEBUG,
        INFO,
        WARN,
        ERROR
    }
    public FILTER_LEVEL filterLevel = FILTER_LEVEL.VERBOSE;

    public enum LOG_LEVEL
    {
        DEBUG = 0,
        INFO,
        WARN,
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
        nativeLogger = log4net.LogManager.GetLogger("UnityLog4NetLogger");

        // set log rotate to daily
        log4net.Appender.RollingFileAppender rollingFileAppender = new log4net.Appender.RollingFileAppender();
        rollingFileAppender.RollingStyle = log4net.Appender.RollingFileAppender.RollingMode.Date;
        rollingFileAppender.AppendToFile = true;
        rollingFileAppender.StaticLogFileName = false;
        rollingFileAppender.File = Path.GetFullPath(outputLogDir) + "/";
        rollingFileAppender.DatePattern = "yyyyMMdd\".log\"";
        rollingFileAppender.Layout = new log4net.Layout.PatternLayout("%date{yyyy/MM/dd  HH:mm:ss} %message%newline");
        rollingFileAppender.Encoding = new UTF8Encoding(true);

        log4net.Filter.LevelRangeFilter filter = new log4net.Filter.LevelRangeFilter();
        switch(filterLevel)
        {
            case FILTER_LEVEL.VERBOSE: filter.LevelMin = log4net.Core.Level.Verbose; break;
            case FILTER_LEVEL.DEBUG: filter.LevelMin = log4net.Core.Level.Debug; break;
            case FILTER_LEVEL.INFO: filter.LevelMin = log4net.Core.Level.Info; break;
            case FILTER_LEVEL.WARN: filter.LevelMin = log4net.Core.Level.Warn; break;
            case FILTER_LEVEL.ERROR: filter.LevelMin = log4net.Core.Level.Error; break;
        }
        filter.LevelMax = log4net.Core.Level.Fatal;
        rollingFileAppender.AddFilter(filter);

        rollingFileAppender.ActivateOptions();
        ((log4net.Repository.Hierarchy.Logger)nativeLogger.Logger).AddAppender(rollingFileAppender);
        ((log4net.Repository.Hierarchy.Logger)nativeLogger.Logger).Hierarchy.Configured = true;
    }

    // output log
    public void Log(LOG_LEVEL lv, string msg, bool withDebugLog = true)
    {
        if(nativeLogger == null)
            return;

        switch(lv)
        {
            case LOG_LEVEL.DEBUG: nativeLogger.Debug(msg); break;
            case LOG_LEVEL.INFO: nativeLogger.Info(msg); break;
            case LOG_LEVEL.WARN: nativeLogger.Warn(msg); break;
            case LOG_LEVEL.ERROR: nativeLogger.Error(msg); break;
        }

        if(withDebugLog)
        {
            switch(lv)
            {
                case LOG_LEVEL.DEBUG:
                case LOG_LEVEL.INFO:
                    Debug.Log(msg);
                    break;

                case LOG_LEVEL.WARN:
                    Debug.LogWarning(msg);
                    break;

                case LOG_LEVEL.ERROR:
                    Debug.LogError(msg);
                    break;
            }
        }
    }

    public void Log(LOG_LEVEL lv, string tag, string msg)
    {
        Log(lv, tag + "\t" + msg);
    }
}
