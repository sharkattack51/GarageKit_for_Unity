using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// ユーティリティクラス
/// </summary>
namespace GarageKit
{
    /// <summary>
    /// アプリケーション関連のユーティリティ
    /// </summary>
    public class WindowsUtil
    {
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindowByCaption(IntPtr zeroOnly, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hwnd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern IntPtr GetTopWindow(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsDelegate lpEnumFunc, IntPtr lParam);
        private static List<IntPtr> EnumWindowsList;
        private static List<IntPtr> GetEnumWindowsList()
        {
            EnumWindowsList = new List<IntPtr>();
            EnumWindows(EnumerateWindows, IntPtr.Zero);
            return EnumWindowsList;
        }
        private delegate bool EnumWindowsDelegate(IntPtr hWnd, IntPtr lParam);
        private static bool EnumerateWindows(IntPtr hWnd, IntPtr lParam)
        {
            EnumWindowsList.Add(hWnd);
            return true;            
        }

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, ref uint lpdwProcessId);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(HandleRef hwnd, out RECT lpRect);

        [DllImport( "user32.dll")]
        private static extern uint GetWindowLong(IntPtr hWnd, int index);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int index, uint dwNewLong); 

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        private static extern int MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int index);

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int ShowWindow(IntPtr hWnd, int nCmdShow); 


        private const int SM_CXFULLSCREEN = 0;
        private const int SM_CYFULLSCREEN = 1;

        private const int GW_HWNDFIRST = 0;
        private const int GW_HWNDLAST = 1;
        private const int GW_HWNDNEXT = 2;
        private const int GW_HWNDPREV = 3;
        private const int GW_OWNER = 4;
        private const int GW_CHILD = 5;

        private const int GWL_HWNDPARENT = -8;
        private const int GWL_STYLE = -16;

        private const uint WS_POPUP = 0x80000000;
        private const uint WS_BORDER = 0x00800000;
        private const uint WS_SYSMENU = 0x00080000;
        private const uint WS_DLGFRAME = 0x00400000;
        private const uint WS_CAPTION = 0x00C00000;
        private const uint WS_THICKFRAME = 0x00040000;
        private const uint WS_POPUPWINDOW = WS_BORDER | WS_POPUP | WS_SYSMENU;

        private static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
        private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        private static readonly IntPtr HWND_TOP = new IntPtr(0);
        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_FRAMECHANGED = 0x0020;

        private const int SW_MINIMIZE = 6;

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }


        /// <summary>
        /// アプリケーションのウィンドウハンドルを取得する
        /// </summary>
        private static IntPtr GetApplicationWindowHandle()
        {
            int currentProcID = Process.GetCurrentProcess().Id;

            foreach(IntPtr hWnd in GetEnumWindowsList())
            {
                uint procID = 0;
                GetWindowThreadProcessId(hWnd, ref procID);
                
                if(procID == currentProcID)
                    return hWnd;
            }

            return IntPtr.Zero;
        }

        /// <summary>
        /// アプリケーションのウィンドウRectを取得する
        /// </summary>
        public static Rect GetApplicationWindowRect()
        {
            IntPtr hWnd = GetApplicationWindowHandle();
            RECT rect = new RECT();
            GetWindowRect(new HandleRef(null, hWnd), out rect);

            return new Rect(rect.Left, rect.Bottom, rect.Right - rect.Left, rect.Top - rect.Bottom);
        }

        /// <summary>
        /// アプリケーションのウィンドウを前景にする
        /// </summary>
        public static void SetForeGroundApplicationWindow()
        {
            IntPtr hWnd = GetApplicationWindowHandle();

            if(hWnd != IntPtr.Zero)
            {
                SetForegroundWindow(hWnd);
                SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_FRAMECHANGED);
            }
        }

        /// <summary>
        /// ポップアップウィンドウを設定する
        /// </summary>
        public static void SetPopupWindow(int x, int y, int w, int h)
        {
            IntPtr hWnd = GetApplicationWindowHandle();

            MoveWindow(hWnd, x, y, w, h, false);

            Process currentProc = Process.GetCurrentProcess();
            currentProc.PriorityClass = ProcessPriorityClass.RealTime;

            uint old_style = GetWindowLong(hWnd, GWL_STYLE);
            uint new_style = (old_style & ~(WS_CAPTION | WS_BORDER | WS_DLGFRAME | WS_THICKFRAME));
            SetWindowLong(hWnd, GWL_STYLE, new_style);

            SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_FRAMECHANGED);
        }

        /// <summary>
        /// ウィンドウ位置の変更
        /// </summary>
        public static void SetWindowPos(int x, int y, int w, int h)
        {
            IntPtr hWnd = GetApplicationWindowHandle();
            MoveWindow(hWnd, x, y, w, h, false);
        }

        /// <summary>
        /// 指定ウィンドウを最小化する
        /// </summary>
        public static void MinimizeWindow(string className, string windowName)
        {
            IntPtr hWnd = FindWindow(className, windowName);
            ShowWindow(hWnd, SW_MINIMIZE);
        }

        public static void MinimizeWindow()
        {
            IntPtr hWnd = GetActiveWindow();
            ShowWindow(hWnd, SW_MINIMIZE);
        }
    }
}
