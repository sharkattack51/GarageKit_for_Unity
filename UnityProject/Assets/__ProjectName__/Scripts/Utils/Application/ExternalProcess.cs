using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

/// <summary>
/// 外部プロセスを起動する
/// </summary>
namespace GarageKit
{
    public class ExternalProcess : MonoBehaviour
    {
        public string exePath = "";
        public bool pathFromStreamingAssets = false;
        public string arguments = "";
        public bool startupOnStart = true;
        public bool showWindow = false;

        private int procId = -1;
        public int ProcId { get{ return procId; } }

        private bool isRunning = false;
        public bool IsRunning { get{ return isRunning; } }

        private Process proc;


        void Awake()
        {

        }

        void Start()
        {
            if(startupOnStart)
                StartProcess();
        }

        void Update()
        {
            if(proc == null || proc.HasExited)
                isRunning = false;
        }

        void OnApplicationQuit()
        {
            DisposeProcess();
        }


        public void StartProcess()
        {
            if(!isRunning)
            {
                string path = exePath;
                if(pathFromStreamingAssets)
                    path = Path.Combine(Application.streamingAssetsPath, path);
                path = Path.GetFullPath(path);

                if(File.Exists(path))
                {
                    try
                    {
                        UnityEngine.Debug.Log("ExternalProcess :: process start: " + path);

                        DisposeProcess();
                        proc = new Process();
                        proc.StartInfo.FileName = path;

                        // 引数設定
                        if(arguments != "")
                            proc.StartInfo.Arguments = arguments;

                        // ウィンドウスタイル設定
                        if(!showWindow)
                        {
                            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                            proc.StartInfo.CreateNoWindow = true;
                            proc.StartInfo.UseShellExecute = false;
                        }

                        proc.Start();

                        procId = proc.Id;
                        isRunning = true;
                    }
                    catch(Exception e)
                    {
                        UnityEngine.Debug.LogError("ExternalProcess :: process start error: " + e.Message);
                    }
                }
                else
                    UnityEngine.Debug.LogError("ExternalProcess :: file not found: " + path);
            }
        }

        private void DisposeProcess()
        {
            if(proc != null && !proc.HasExited)
                proc.Kill();
            proc = null;
        }
    }
}
