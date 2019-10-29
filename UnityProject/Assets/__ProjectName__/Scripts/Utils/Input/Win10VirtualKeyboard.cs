using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GarageKit
{
    public class Win10VirtualKeyboard : MonoBehaviour
    {
#if UNITY_STANDALONE_WIN
        [DllImport("user32")]
        static extern IntPtr FindWindow(String sClassName, String sAppName);

        [DllImport("user32")]
        static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        private static Process _onScreenKeyboardProcess = null;


        public static void ShowTouchKeyboard()
        {
            ExternalCall("C:\\Program Files\\Common Files\\Microsoft Shared\\ink\\tabtip.exe", null, false);
        }

        public static void HideTouchKeyboard()
        {
            uint WM_SYSCOMMAND = 274;
            int SC_CLOSE = 61536;
            IntPtr ptr = FindWindow("IPTip_Main_Window", null);
            PostMessage(ptr, WM_SYSCOMMAND, SC_CLOSE, 0);
        }

        public static void ShowOnScreenKeyboard()
        {
            if(_onScreenKeyboardProcess == null || _onScreenKeyboardProcess.HasExited)
                _onScreenKeyboardProcess = ExternalCall("OSK", null, false);
        }

        public static void HideOnScreenKeyboard()
        {
            if(_onScreenKeyboardProcess != null && !_onScreenKeyboardProcess.HasExited)
                _onScreenKeyboardProcess.Kill();
        }

        public static void RepositionOnScreenKeyboard(Rect rect)
        {
            ExternalCall("REG", @"ADD HKCU\Software\Microsoft\Osk /v WindowLeft /t REG_DWORD /d " + (int)rect.x + " /f", true);
            ExternalCall("REG", @"ADD HKCU\Software\Microsoft\Osk /v WindowTop /t REG_DWORD /d " + (int)rect.y + " /f", true);
            ExternalCall("REG", @"ADD HKCU\Software\Microsoft\Osk /v WindowWidth /t REG_DWORD /d " + (int)rect.width + " /f", true);
            ExternalCall("REG", @"ADD HKCU\Software\Microsoft\Osk /v WindowHeight /t REG_DWORD /d " + (int)rect.height + " /f", true);
        }

        private static Process ExternalCall(string filename, string arguments, bool hideWindow)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = filename;
            startInfo.Arguments = arguments;

            if(hideWindow)
            {
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;
            }

            Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();

            return process;
        }
#endif

        private GameObject currentSelectable;


        void Awake()
        {

        }

        void Start()
        {

        }

        void Update()
        {
#if UNITY_STANDALONE_WIN
            if(EventSystem.current != null)
            {
                if(EventSystem.current.currentSelectedGameObject == null)
                {
                    currentSelectable = null;
                    Win10VirtualKeyboard.HideTouchKeyboard();
                }
                else
                {
                    if(EventSystem.current.currentSelectedGameObject.Equals(currentSelectable))
                        return;
                    else
                    {
                        currentSelectable = EventSystem.current.currentSelectedGameObject;

                        if(currentSelectable.GetComponent<InputField>() != null)
                            Win10VirtualKeyboard.ShowTouchKeyboard();
                        else
                            Win10VirtualKeyboard.HideTouchKeyboard();
                    }
                }
            }
        }
#endif
    }
}
