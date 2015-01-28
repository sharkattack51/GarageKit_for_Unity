using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

/*
 * 入力操作をエミュレーションする
 */

public class VirtualInput
{
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN

	//マウス操作やキーボード操作をシミュレーションするサンプル(C#.NET) 
	//http://homepage2.nifty.com/nonnon/SoftSample/CS.NET/SampleSendInput.html

	// マウスイベント(mouse_eventの引数と同様のデータ)
	[StructLayout(LayoutKind.Sequential)]
	private struct MOUSEINPUT
	{
		public int dx;
		public int dy;
		public int mouseData;
		public int dwFlags;
		public int time;
		public int dwExtraInfo;
	};
	
	// キーボードイベント(keybd_eventの引数と同様のデータ)
	[StructLayout(LayoutKind.Sequential)]
	private struct KEYBDINPUT
	{
		public short wVk;
		public short wScan;
		public int dwFlags;
		public int time;
		public int dwExtraInfo;
	};
	
	// ハードウェアイベント
	[StructLayout(LayoutKind.Sequential)]
	private struct HARDWAREINPUT
	{
		public int uMsg;
		public short wParamL;
		public short wParamH;
	};
	
	// 各種イベント(SendInputの引数データ)
	[StructLayout(LayoutKind.Explicit)]
	private struct INPUT
	{
		[FieldOffset(0)] public int type;
		[FieldOffset(4)] public MOUSEINPUT mi;
		[FieldOffset(4)] public KEYBDINPUT ki;
		[FieldOffset(4)] public HARDWAREINPUT hi;
	};
	
	// キー操作、マウス操作をシミュレート(擬似的に操作する)
	[DllImport("user32.dll")]
	private extern static void SendInput(int nInputs, ref INPUT pInputs, int cbsize);
	
	// 仮想キーコードをスキャンコードに変換
	[DllImport("user32.dll", EntryPoint = "MapVirtualKeyA")]
	private extern static int MapVirtualKey(int wCode, int wMapType);
	
	private const int INPUT_MOUSE = 0;					// マウスイベント
	private const int INPUT_KEYBOARD = 1;				// キーボードイベント
	private const int INPUT_HARDWARE = 2;				// ハードウェアイベント
	
	private const int MOUSEEVENTF_MOVE = 0x1;			// マウスを移動する
	private const int MOUSEEVENTF_ABSOLUTE = 0x8000;	// 絶対座標指定
	private const int MOUSEEVENTF_LEFTDOWN = 0x2;		// 左ボタンを押す
	private const int MOUSEEVENTF_LEFTUP = 0x4;		// 左ボタンを離す
	private const int MOUSEEVENTF_RIGHTDOWN = 0x8;		// 右ボタンを押す
	private const int MOUSEEVENTF_RIGHTUP = 0x10; 		// 右ボタンを離す
	private const int MOUSEEVENTF_MIDDLEDOWN = 0x20;	// 中央ボタンを押す
	private const int MOUSEEVENTF_MIDDLEUP = 0x40;		// 中央ボタンを離す
	private const int MOUSEEVENTF_WHEEL = 0x800;		// ホイールを回転する
	private const int WHEEL_DELTA = 120;				// ホイール回転値
	
	private const int KEYEVENTF_KEYDOWN = 0x0;			// キーを押す
	private const int KEYEVENTF_KEYUP = 0x2;			// キーを離す
	private const int KEYEVENTF_EXTENDEDKEY = 0x1;		// 拡張コード

	//Virtual-Key Codes
	//http://msdn.microsoft.com/ja-jp/library/windows/desktop/dd375731(v=vs.85).aspx
	
	private const int VK_L_SHIFT = 0xA0;
	private const int VK_R_SHIFT = 0xA1;
	private const int VK_L_CONTROL = 0xA2;
	private const int VK_R_CONTROL = 0xA3;
	private const int VK_BACKSPACE = 0x08;
	private const int 	VK_TAB = 0x09;
	private const int VK_RETURN = 0x0D;
	private const int VK_ALT = 0x12;
	private const int VK_ESCAPE = 0x1B;
	private const int VK_SPACE = 0x20;
	private const int VK_LEFT_ARROW = 0x25;
	private const int VK_UP_ARROW = 0x26;
	private const int VK_RIGHT_ARROW = 0x27;
	private const int VK_DOWN_ARROW = 0x28;
	private const int VK_DELETE = 0x2E;
			
	private const int VK_0 = 0x30;	
	private const int VK_1 = 0x31;	
	private const int VK_2 = 0x32;	
	private const int VK_3 = 0x33;	
	private const int VK_4 = 0x34;	
	private const int VK_5 = 0x35;	
	private const int VK_6 = 0x36;	
	private const int VK_7 = 0x37;	
	private const int VK_8 = 0x38;	
	private const int VK_9 = 0x39;	

	private const int VK_A = 0x41;
	private const int VK_B = 0x42;
	private const int VK_C = 0x43;
	private const int VK_D = 0x44;
	private const int VK_E = 0x45;
	private const int VK_F = 0x46;
	private const int VK_G = 0x47;
	private const int VK_H = 0x48;
	private const int VK_I = 0x49;
	private const int VK_J = 0x4A;
	private const int VK_K = 0x4B;
	private const int VK_L = 0x4C;
	private const int VK_M = 0x4D;
	private const int VK_N = 0x4E;
	private const int VK_O = 0x4F;
	private const int VK_P = 0x50;
	private const int VK_Q = 0x51;
	private const int VK_R = 0x52;
	private const int VK_S = 0x53;
	private const int VK_T = 0x54;
	private const int VK_U = 0x55;
	private const int VK_V = 0x56;
	private const int VK_W = 0x57;
	private const int VK_X = 0x58;
	private const int VK_Y = 0x59;
	private const int VK_Z = 0x5A;

	private const int VK_NUM_0 = 0x60;
	private const int VK_NUM_1 = 0x61;
	private const int VK_NUM_2 = 0x62;
	private const int VK_NUM_3 = 0x63;
	private const int VK_NUM_4 = 0x64;
	private const int VK_NUM_5 = 0x65;
	private const int VK_NUM_6 = 0x66;
	private const int VK_NUM_7 = 0x67;
	private const int VK_NUM_8 = 0x68;
	private const int VK_NUM_9 = 0x69;

	private const int VK_F1 = 0x70;
	private const int VK_F2 = 0x71;
	private const int VK_F3 = 0x72;
	private const int VK_F4 = 0x73;
	private const int VK_F5 = 0x74;
	private const int VK_F6 = 0x75;
	private const int VK_F7 = 0x76;
	private const int VK_F8 = 0x77;
	private const int VK_F9 = 0x78;
	private const int VK_F10 = 0x79;
	private const int VK_F11 = 0x7A;
	private const int VK_F12 = 0x7B;


#region Keyboard

	// キーを押す
	public static void KeyDown(int code)
	{
		// キーボード操作実行用のデータ
		const int num = 2;
		INPUT[] inp = new INPUT[num];
		
		// キーを押す
		inp[0].type = INPUT_KEYBOARD;
		inp[0].ki.wVk = (short)code;
		inp[0].ki.wScan = (short)MapVirtualKey(inp[0].ki.wVk, 0);
		inp[0].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYDOWN;
		inp[0].ki.dwExtraInfo = 0;
		inp[0].ki.time = 0;
		
		//  キーを離す
		inp[1].type = INPUT_KEYBOARD;
		inp[1].ki.wVk = (short)code;
		inp[1].ki.wScan = (short)MapVirtualKey(inp[1].ki.wVk, 0);
		inp[1].ki.dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP;
		inp[1].ki.dwExtraInfo = 0;
		inp[1].ki.time = 0;
		
		// キーボード操作実行
		SendInput(num, ref inp[0], Marshal.SizeOf(inp[0]));

		Debug.Log("VirtualInput :: KeyDown [" + code + "]");
	}

#endregion

#region Mouse

	public static void Move(int pos_x, int pos_y)
	{
		// マウス操作実行用のデータ
		const int num = 1;
		INPUT[] inp = new INPUT[num];
		
		// マウスカーソルを移動する
		inp[0].type = INPUT_MOUSE;
		inp[0].mi.dwFlags = MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE;
		inp[0].mi.dx = pos_x * (65535 / Screen.width);
		inp[0].mi.dy = pos_y * (65535 / Screen.height);
		inp[0].mi.mouseData = 0;
		inp[0].mi.dwExtraInfo = 0;
		inp[0].mi.time = 0;
		
		// マウス操作実行
		SendInput(num, ref inp[0], Marshal.SizeOf(inp[0]));

		Debug.Log("VirtualInput :: Mouse Move [ " + pos_x.ToString() + " , " + pos_y.ToString() + " ]");
	}

	public static void Click(int pos_x, int pos_y)
	{
		// マウス操作実行用のデータ
		const int num = 3;
		INPUT[] inp = new INPUT[num];
		
		// マウスカーソルを移動する
		inp[0].type = INPUT_MOUSE;
		inp[0].mi.dwFlags = MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE;
		inp[0].mi.dx = pos_x * (65535 / Screen.width);
		inp[0].mi.dy = pos_y * (65535 / Screen.height);
		inp[0].mi.mouseData = 0;
		inp[0].mi.dwExtraInfo = 0;
		inp[0].mi.time = 0;
		
		// マウスの左ボタンを押す
		inp[1].type = INPUT_MOUSE;
		inp[1].mi.dwFlags = MOUSEEVENTF_LEFTDOWN;
		inp[1].mi.dx = 0;
		inp[1].mi.dy = 0;
		inp[1].mi.mouseData = 0;
		inp[1].mi.dwExtraInfo = 0;
		inp[1].mi.time = 0;
		
		// マウスの左ボタンを離す
		inp[2].type = INPUT_MOUSE;
		inp[2].mi.dwFlags = MOUSEEVENTF_LEFTUP;
		inp[2].mi.dx = 0;
		inp[2].mi.dy = 0;
		inp[2].mi.mouseData = 0;
		inp[2].mi.dwExtraInfo = 0;
		inp[2].mi.time = 0;
		
		// マウス操作実行
		SendInput(num, ref inp[0], Marshal.SizeOf(inp[0]));

		Debug.Log("VirtualInput :: Mouse Click [ " + pos_x.ToString() + " , " + pos_y.ToString() + " ]");
	}

	public static void DoubleClick(int pos_x, int pos_y)
	{
		// マウス操作実行用のデータ
		const int num = 5;
		INPUT[] inp = new INPUT[num];
		
		// マウスカーソルを移動する
		inp[0].type = INPUT_MOUSE;
		inp[0].mi.dwFlags = MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE;
		inp[0].mi.dx = pos_x * (65535 / Screen.width);
		inp[0].mi.dy = pos_y * (65535 / Screen.height);
		inp[0].mi.mouseData = 0;
		inp[0].mi.dwExtraInfo = 0;
		inp[0].mi.time = 0;
		
		// マウスの左ボタンを押す
		inp[1].type = INPUT_MOUSE;
		inp[1].mi.dwFlags = MOUSEEVENTF_LEFTDOWN;
		inp[1].mi.dx = 0;
		inp[1].mi.dy = 0;
		inp[1].mi.mouseData = 0;
		inp[1].mi.dwExtraInfo = 0;
		inp[1].mi.time = 0;
		
		// マウスの左ボタンを離す
		inp[2].type = INPUT_MOUSE;
		inp[2].mi.dwFlags = MOUSEEVENTF_LEFTUP;
		inp[2].mi.dx = 0;
		inp[2].mi.dy = 0;
		inp[2].mi.mouseData = 0;
		inp[2].mi.dwExtraInfo = 0;
		inp[2].mi.time = 0;

		// マウスの左ボタンを押す
		inp[3].type = INPUT_MOUSE;
		inp[3].mi.dwFlags = MOUSEEVENTF_LEFTDOWN;
		inp[3].mi.dx = 0;
		inp[3].mi.dy = 0;
		inp[3].mi.mouseData = 0;
		inp[3].mi.dwExtraInfo = 0;
		inp[3].mi.time = 0;
		
		// マウスの左ボタンを離す
		inp[4].type = INPUT_MOUSE;
		inp[4].mi.dwFlags = MOUSEEVENTF_LEFTUP;
		inp[4].mi.dx = 0;
		inp[4].mi.dy = 0;
		inp[4].mi.mouseData = 0;
		inp[4].mi.dwExtraInfo = 0;
		inp[4].mi.time = 0;
		
		// マウス操作実行
		SendInput(num, ref inp[0], Marshal.SizeOf(inp[0]));

		Debug.Log("VirtualInput :: Mouse Double Click [ " + pos_x.ToString() + " , " + pos_y.ToString() + " ]");
	}

	public static void LeftDown(int pos_x, int pos_y)
	{
		// マウス操作実行用のデータ
		const int num = 2;
		INPUT[] inp = new INPUT[num];
		
		// マウスカーソルを移動する
		inp[0].type = INPUT_MOUSE;
		inp[0].mi.dwFlags = MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE;
		inp[0].mi.dx = pos_x * (65535 / Screen.width);
		inp[0].mi.dy = pos_y * (65535 / Screen.height);
		inp[0].mi.mouseData = 0;
		inp[0].mi.dwExtraInfo = 0;
		inp[0].mi.time = 0;
		
		// マウスの中ボタンを押す
		inp[1].type = INPUT_MOUSE;
		inp[1].mi.dwFlags = MOUSEEVENTF_LEFTDOWN;
		inp[1].mi.dx = 0;
		inp[1].mi.dy = 0;
		inp[1].mi.mouseData = 0;
		inp[1].mi.dwExtraInfo = 0;
		inp[1].mi.time = 0;
		
		// マウス操作実行
		SendInput(num, ref inp[0], Marshal.SizeOf(inp[0]));

		Debug.Log("VirtualInput :: Mouse Left Down [ " + pos_x.ToString() + " , " + pos_y.ToString() + " ]");
	}

	public static void LeftUp()
	{
		// マウス操作実行用のデータ
		const int num = 1;
		INPUT[] inp = new INPUT[num];
		
		// マウスの中ボタンを離す
		inp[0].type = INPUT_MOUSE;
		inp[0].mi.dwFlags = MOUSEEVENTF_LEFTUP;
		inp[0].mi.dx = 0;
		inp[0].mi.dy = 0;
		inp[0].mi.mouseData = 0;
		inp[0].mi.dwExtraInfo = 0;
		inp[0].mi.time = 0;
		
		// マウス操作実行
		SendInput(num, ref inp[0], Marshal.SizeOf(inp[0]));

		Debug.Log("VirtualInput :: Mouse Left Up");
	}

	public static void MiddleDown(int pos_x, int pos_y)
	{
		// マウス操作実行用のデータ
		const int num = 2;
		INPUT[] inp = new INPUT[num];
		
		// マウスカーソルを移動する
		inp[0].type = INPUT_MOUSE;
		inp[0].mi.dwFlags = MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE;
		inp[0].mi.dx = pos_x * (65535 / Screen.width);
		inp[0].mi.dy = pos_y * (65535 / Screen.height);
		inp[0].mi.mouseData = 0;
		inp[0].mi.dwExtraInfo = 0;
		inp[0].mi.time = 0;
		
		// マウスの中ボタンを押す
		inp[1].type = INPUT_MOUSE;
		inp[1].mi.dwFlags = MOUSEEVENTF_MIDDLEDOWN;
		inp[1].mi.dx = 0;
		inp[1].mi.dy = 0;
		inp[1].mi.mouseData = 0;
		inp[1].mi.dwExtraInfo = 0;
		inp[1].mi.time = 0;
		
		// マウス操作実行
		SendInput(num, ref inp[0], Marshal.SizeOf(inp[0]));

		Debug.Log("VirtualInput :: Mouse Middle Down [ " + pos_x.ToString() + " , " + pos_y.ToString() + " ]");
	}

	public static void MiddleUp()
	{
		// マウス操作実行用のデータ
		const int num = 1;
		INPUT[] inp = new INPUT[num];
		
		// マウスの中ボタンを離す
		inp[0].type = INPUT_MOUSE;
		inp[0].mi.dwFlags = MOUSEEVENTF_MIDDLEUP;
		inp[0].mi.dx = 0;
		inp[0].mi.dy = 0;
		inp[0].mi.mouseData = 0;
		inp[0].mi.dwExtraInfo = 0;
		inp[0].mi.time = 0;
		
		// マウス操作実行
		SendInput(num, ref inp[0], Marshal.SizeOf(inp[0]));

		Debug.Log("VirtualInput :: Mouse Middle Up");
	}

#endregion

#endif
}