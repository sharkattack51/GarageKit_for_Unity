using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

/*
 * 入力操作をエミュレーションする
 */
namespace GarageKit
{
    public class VirtualInput
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        // マウス操作やキーボード操作をシミュレーションするサンプル(C#.NET) 
        // http://homepage2.nifty.com/nonnon/SoftSample/CS.NET/SampleSendInput.html

# region input structs
        [StructLayout(LayoutKind.Sequential)]
        private struct MouseInput
        {
            public int dx;
            public int dy;
            public int mouseData;
            public int dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        };

        [StructLayout(LayoutKind.Sequential)]
        private struct KeyboardInput
        {
            public short wVk;
            public short wScan;
            public int dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        };

        [StructLayout(LayoutKind.Sequential)]
        private struct HardwareInput
        {
            public int uMsg;
            public short wParamL;
            public short wParamH;
        };

        [StructLayout(LayoutKind.Explicit)]
        private struct InputUnion
        {
            [FieldOffset(0)] public MouseInput mi;
            [FieldOffset(0)] public KeyboardInput ki;
            [FieldOffset(0)] public HardwareInput hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Input
        {
            public int type;
            public InputUnion ui;
        };
#endregion


        // キー操作、マウス操作をシミュレート(擬似的に操作する)
        [DllImport("user32.dll", SetLastError = true)]
        private extern static void SendInput(int nInputs, Input[] pInputs, int cbsize);

        // 仮想キーコードをスキャンコードに変換
        [DllImport("user32.dll", EntryPoint = "MapVirtualKeyA")]
        private extern static int MapVirtualKey(int wCode, int wMapType);

        private const int INPUT_MOUSE = 0;					// マウスイベント
        private const int INPUT_KEYBOARD = 1;				// キーボードイベント
        private const int INPUT_HARDWARE = 2;				// ハードウェアイベント

        private const int MOUSEEVENTF_MOVE = 0x0001;		// マウスを移動する
        private const int MOUSEEVENTF_ABSOLUTE = 0x8000;	// 絶対座標指定
        private const int MOUSEEVENTF_LEFTDOWN = 0x0002;	// 左ボタンを押す
        private const int MOUSEEVENTF_LEFTUP = 0x0004;		// 左ボタンを離す
        private const int MOUSEEVENTF_RIGHTDOWN = 0x0008;	// 右ボタンを押す
        private const int MOUSEEVENTF_RIGHTUP = 0x0010; 	// 右ボタンを離す
        private const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;	// 中央ボタンを押す
        private const int MOUSEEVENTF_MIDDLEUP = 0x0040;	// 中央ボタンを離す
        private const int MOUSEEVENTF_WHEEL = 0x0800;		// ホイールを回転する
        private const int WHEEL_DELTA = 120;				// ホイール回転値

        private const int KEYEVENTF_KEYDOWN = 0x0000;		// キーを押す
        private const int KEYEVENTF_KEYUP = 0x0002;			// キーを離す
        private const int KEYEVENTF_EXTENDEDKEY = 0x0001;	// 拡張コード

        public class KeyCode
        {
            // Virtual-Key Codes
            // http://msdn.microsoft.com/ja-jp/library/windows/desktop/dd375731(v=vs.85).aspx

            public const int VK_L_SHIFT = 0x00A0;
            public const int VK_R_SHIFT = 0x00A1;
            public const int VK_L_CONTROL = 0x00A2;
            public const int VK_R_CONTROL = 0x00A3;
            public const int VK_BACKSPACE = 0x0008;
            public const int VK_TAB = 0x0009;
            public const int VK_RETURN = 0x000D;
            public const int VK_ALT = 0x0012;
            public const int VK_ESCAPE = 0x001B;
            public const int VK_SPACE = 0x0020;
            public const int VK_LEFT_ARROW = 0x0025;
            public const int VK_UP_ARROW = 0x0026;
            public const int VK_RIGHT_ARROW = 0x0027;
            public const int VK_DOWN_ARROW = 0x0028;
            public const int VK_DELETE = 0x002E;

            public const int VK_0 = 0x0030;	
            public const int VK_1 = 0x0031;	
            public const int VK_2 = 0x0032;	
            public const int VK_3 = 0x0033;	
            public const int VK_4 = 0x0034;	
            public const int VK_5 = 0x0035;	
            public const int VK_6 = 0x0036;	
            public const int VK_7 = 0x0037;	
            public const int VK_8 = 0x0038;	
            public const int VK_9 = 0x0039;	

            public const int VK_A = 0x0041;
            public const int VK_B = 0x0042;
            public const int VK_C = 0x0043;
            public const int VK_D = 0x0044;
            public const int VK_E = 0x0045;
            public const int VK_F = 0x0046;
            public const int VK_G = 0x0047;
            public const int VK_H = 0x0048;
            public const int VK_I = 0x0049;
            public const int VK_J = 0x004A;
            public const int VK_K = 0x004B;
            public const int VK_L = 0x004C;
            public const int VK_M = 0x004D;
            public const int VK_N = 0x004E;
            public const int VK_O = 0x004F;
            public const int VK_P = 0x0050;
            public const int VK_Q = 0x0051;
            public const int VK_R = 0x0052;
            public const int VK_S = 0x0053;
            public const int VK_T = 0x0054;
            public const int VK_U = 0x0055;
            public const int VK_V = 0x0056;
            public const int VK_W = 0x0057;
            public const int VK_X = 0x0058;
            public const int VK_Y = 0x0059;
            public const int VK_Z = 0x005A;

            public const int VK_NUM_0 = 0x0060;
            public const int VK_NUM_1 = 0x0061;
            public const int VK_NUM_2 = 0x0062;
            public const int VK_NUM_3 = 0x0063;
            public const int VK_NUM_4 = 0x0064;
            public const int VK_NUM_5 = 0x0065;
            public const int VK_NUM_6 = 0x0066;
            public const int VK_NUM_7 = 0x0067;
            public const int VK_NUM_8 = 0x0068;
            public const int VK_NUM_9 = 0x0069;

            public const int VK_F1 = 0x0070;
            public const int VK_F2 = 0x0071;
            public const int VK_F3 = 0x0072;
            public const int VK_F4 = 0x0073;
            public const int VK_F5 = 0x0074;
            public const int VK_F6 = 0x0075;
            public const int VK_F7 = 0x0076;
            public const int VK_F8 = 0x0077;
            public const int VK_F9 = 0x0078;
            public const int VK_F10 = 0x0079;
            public const int VK_F11 = 0x007A;
            public const int VK_F12 = 0x007B;

            public const int VK_L_WINDOWS = 0x005B;
            public const int VK_R_WINDOWS = 0x005C;
        }


#region Keyboard
        public static void KeyDown(int keyCode)
        {
            Input[] inputs = new Input[2];

            // key down
            inputs[0] = new Input();
            inputs[0].type = INPUT_KEYBOARD;
            inputs[0].ui.ki.wVk = (short)keyCode;
            inputs[0].ui.ki.wScan = (short)MapVirtualKey(keyCode, 0);
            inputs[0].ui.ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYDOWN;
            inputs[0].ui.ki.time = 0;
            inputs[0].ui.ki.dwExtraInfo = IntPtr.Zero;

            // key up
            inputs[1] = new Input();
            inputs[1].type = INPUT_KEYBOARD;
            inputs[1].ui.ki.wVk = (short)keyCode;
            inputs[1].ui.ki.wScan = (short)MapVirtualKey(keyCode, 0);
            inputs[1].ui.ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP;
            inputs[1].ui.ki.time = 0;
            inputs[1].ui.ki.dwExtraInfo = IntPtr.Zero;

            SendInput(inputs.Length, inputs, Marshal.SizeOf(inputs[0]));
            Debug.Log("VirtualInput :: keydown keycode: [ " + keyCode + " ]");
        }

        public static void KeyDown(int keyCode1, int keyCode2)
        {
            Input[] inputs = new Input[4];

            // key1 down
            inputs[0] = new Input();
            inputs[0].type = INPUT_KEYBOARD;
            inputs[0].ui.ki.wVk = (short)keyCode1;
            inputs[0].ui.ki.wScan = (short)MapVirtualKey(keyCode1, 0);
            inputs[0].ui.ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYDOWN;
            inputs[0].ui.ki.time = 0;
            inputs[0].ui.ki.dwExtraInfo = IntPtr.Zero;

            // key2 down
            inputs[1] = new Input();
            inputs[1].type = INPUT_KEYBOARD;
            inputs[1].ui.ki.wVk = (short)keyCode2;
            inputs[1].ui.ki.wScan = (short)MapVirtualKey(keyCode2, 0);
            inputs[1].ui.ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYDOWN;
            inputs[1].ui.ki.time = 0;
            inputs[1].ui.ki.dwExtraInfo = IntPtr.Zero;

            // key2 up
            inputs[2] = new Input();
            inputs[2].type = INPUT_KEYBOARD;
            inputs[2].ui.ki.wVk = (short)keyCode2;
            inputs[2].ui.ki.wScan = (short)MapVirtualKey(keyCode2, 0);
            inputs[2].ui.ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP;
            inputs[2].ui.ki.time = 0;
            inputs[2].ui.ki.dwExtraInfo = IntPtr.Zero;

            // key1 up
            inputs[3] = new Input();
            inputs[3].type = INPUT_KEYBOARD;
            inputs[3].ui.ki.wVk = (short)keyCode1;
            inputs[3].ui.ki.wScan = (short)MapVirtualKey(keyCode1, 0);
            inputs[3].ui.ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP;
            inputs[3].ui.ki.time = 0;
            inputs[3].ui.ki.dwExtraInfo = IntPtr.Zero;

            SendInput(inputs.Length, inputs, Marshal.SizeOf(inputs[0]));
            Debug.Log("VirtualInput :: keydown keycode: [ " + keyCode1 + " , " + keyCode2 + " ]");
        }

        public static void KeyDown(int keyCode1, int keyCode2, int keyCode3)
        {
            Input[] inputs = new Input[6];

            // key1 down
            inputs[0] = new Input();
            inputs[0].type = INPUT_KEYBOARD;
            inputs[0].ui.ki.wVk = (short)keyCode1;
            inputs[0].ui.ki.wScan = (short)MapVirtualKey(keyCode1, 0);
            inputs[0].ui.ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYDOWN;
            inputs[0].ui.ki.time = 0;
            inputs[0].ui.ki.dwExtraInfo = IntPtr.Zero;

            // key2 down
            inputs[1] = new Input();
            inputs[1].type = INPUT_KEYBOARD;
            inputs[1].ui.ki.wVk = (short)keyCode2;
            inputs[1].ui.ki.wScan = (short)MapVirtualKey(keyCode2, 0);
            inputs[1].ui.ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYDOWN;
            inputs[1].ui.ki.time = 0;
            inputs[1].ui.ki.dwExtraInfo = IntPtr.Zero;

            // key3 down
            inputs[2] = new Input();
            inputs[2].type = INPUT_KEYBOARD;
            inputs[2].ui.ki.wVk = (short)keyCode3;
            inputs[2].ui.ki.wScan = (short)MapVirtualKey(keyCode3, 0);
            inputs[2].ui.ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYDOWN;
            inputs[2].ui.ki.time = 0;
            inputs[2].ui.ki.dwExtraInfo = IntPtr.Zero;

            // key3 up
            inputs[3] = new Input();
            inputs[3].type = INPUT_KEYBOARD;
            inputs[3].ui.ki.wVk = (short)keyCode3;
            inputs[3].ui.ki.wScan = (short)MapVirtualKey(keyCode3, 0);
            inputs[3].ui.ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP;
            inputs[3].ui.ki.time = 0;
            inputs[3].ui.ki.dwExtraInfo = IntPtr.Zero;

            // key2 up
            inputs[4] = new Input();
            inputs[4].type = INPUT_KEYBOARD;
            inputs[4].ui.ki.wVk = (short)keyCode2;
            inputs[4].ui.ki.wScan = (short)MapVirtualKey(keyCode2, 0);
            inputs[4].ui.ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP;
            inputs[4].ui.ki.time = 0;
            inputs[4].ui.ki.dwExtraInfo = IntPtr.Zero;

            // key1 up
            inputs[5] = new Input();
            inputs[5].type = INPUT_KEYBOARD;
            inputs[5].ui.ki.wVk = (short)keyCode1;
            inputs[5].ui.ki.wScan = (short)MapVirtualKey(keyCode1, 0);
            inputs[5].ui.ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP;
            inputs[5].ui.ki.time = 0;
            inputs[5].ui.ki.dwExtraInfo = IntPtr.Zero;

            SendInput(inputs.Length, inputs, Marshal.SizeOf(inputs[0]));
            Debug.Log("VirtualInput :: keydown keycode: [ " + keyCode1 + " , " + keyCode2 + " , " + keyCode3 + " ]");
        }
#endregion

#region Mouse
        public static void MouseMove(int posX, int posY)
        {
            Input[] inputs = new Input[1];

            // mouse move
            inputs[0] = new Input();
            inputs[0].type = INPUT_MOUSE;
            inputs[0].ui.mi.dx = posX * (65535 / Screen.width);
            inputs[0].ui.mi.dy = posY * (65535 / Screen.height);
            inputs[0].ui.mi.mouseData = 0;
            inputs[0].ui.mi.dwFlags = MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE;
            inputs[0].ui.mi.time = 0;
            inputs[0].ui.mi.dwExtraInfo = IntPtr.Zero;

            SendInput(inputs.Length, inputs, Marshal.SizeOf(inputs[0]));
            Debug.Log("VirtualInput :: mouse move [ " + posX + " , " + posY + " ]");
        }

        public static void MouseClick(int posX, int posY)
        {
            Input[] inputs = new Input[3];

            // mouse move
            inputs[0] = new Input();
            inputs[0].type = INPUT_MOUSE;
            inputs[0].ui.mi.dx = posX * (65535 / Screen.width);
            inputs[0].ui.mi.dy = posY * (65535 / Screen.height);
            inputs[0].ui.mi.mouseData = 0;
            inputs[0].ui.mi.dwFlags = MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE;
            inputs[0].ui.mi.time = 0;
            inputs[0].ui.mi.dwExtraInfo = IntPtr.Zero;

            // mouse left button down
            inputs[1] = new Input();
            inputs[1].type = INPUT_MOUSE;
            inputs[1].ui.mi.dx = 0;
            inputs[1].ui.mi.dy = 0;
            inputs[1].ui.mi.mouseData = 0;
            inputs[1].ui.mi.dwFlags = MOUSEEVENTF_LEFTDOWN;
            inputs[1].ui.mi.time = 0;
            inputs[1].ui.mi.dwExtraInfo = IntPtr.Zero;

            // mouse left button up
            inputs[2] = new Input();
            inputs[2].type = INPUT_MOUSE;
            inputs[2].ui.mi.dx = 0;
            inputs[2].ui.mi.dy = 0;
            inputs[2].ui.mi.mouseData = 0;
            inputs[2].ui.mi.dwFlags = MOUSEEVENTF_LEFTUP;
            inputs[2].ui.mi.time = 0;
            inputs[2].ui.mi.dwExtraInfo = IntPtr.Zero;

            SendInput(inputs.Length, inputs, Marshal.SizeOf(inputs[0]));
            Debug.Log("VirtualInput :: mouse click [ " + posX + " , " + posY + " ]");
        }

        public static void MouseDoubleClick(int posX, int posY)
        {
            Input[] inputs = new Input[5];

            // mouse move
            inputs[0] = new Input();
            inputs[0].type = INPUT_MOUSE;
            inputs[0].ui.mi.dx = posX * (65535 / Screen.width);
            inputs[0].ui.mi.dy = posY * (65535 / Screen.height);
            inputs[0].ui.mi.mouseData = 0;
            inputs[0].ui.mi.dwFlags = MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE;
            inputs[0].ui.mi.time = 0;
            inputs[0].ui.mi.dwExtraInfo = IntPtr.Zero;

            // mouse left button down
            inputs[1] = new Input();
            inputs[1].type = INPUT_MOUSE;
            inputs[1].ui.mi.dx = 0;
            inputs[1].ui.mi.dy = 0;
            inputs[1].ui.mi.mouseData = 0;
            inputs[1].ui.mi.dwFlags = MOUSEEVENTF_LEFTDOWN;
            inputs[1].ui.mi.time = 0;
            inputs[1].ui.mi.dwExtraInfo = IntPtr.Zero;

            // mouse left button up
            inputs[2] = new Input();
            inputs[2].type = INPUT_MOUSE;
            inputs[2].ui.mi.dx = 0;
            inputs[2].ui.mi.dy = 0;
            inputs[2].ui.mi.mouseData = 0;
            inputs[2].ui.mi.dwFlags = MOUSEEVENTF_LEFTUP;
            inputs[2].ui.mi.time = 0;
            inputs[2].ui.mi.dwExtraInfo = IntPtr.Zero;

            // mouse left button down
            inputs[3] = new Input();
            inputs[3].type = INPUT_MOUSE;
            inputs[3].ui.mi.dx = 0;
            inputs[3].ui.mi.dy = 0;
            inputs[3].ui.mi.mouseData = 0;
            inputs[3].ui.mi.dwFlags = MOUSEEVENTF_LEFTDOWN;
            inputs[3].ui.mi.time = 0;
            inputs[3].ui.mi.dwExtraInfo = IntPtr.Zero;

            // mouse left button up
            inputs[4] = new Input();
            inputs[4].type = INPUT_MOUSE;
            inputs[4].ui.mi.dx = 0;
            inputs[4].ui.mi.dy = 0;
            inputs[4].ui.mi.mouseData = 0;
            inputs[4].ui.mi.dwFlags = MOUSEEVENTF_LEFTUP;
            inputs[4].ui.mi.time = 0;
            inputs[4].ui.mi.dwExtraInfo = IntPtr.Zero;

            SendInput(inputs.Length, inputs, Marshal.SizeOf(inputs[0]));
            Debug.Log("VirtualInput :: mouse double click [ " + posX + " , " + posY + " ]");
        }
#endregion

#endif
    }
}
