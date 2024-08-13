using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogExample : MonoBehaviour
{
    IEnumerator Start()
    {
        // wait Log4Net initialize
        yield return new WaitForEndOfFrame();

        // Logs are output to ./Log directory with timestamp
        Log4Net.Logger.Log(Log4Net.LOG_LEVEL.INFO, "log example start...");
    }

    void OnApplicationQuit()
    {
        Log4Net.Logger.Log(Log4Net.LOG_LEVEL.INFO, "log example quit...");
    }
}
