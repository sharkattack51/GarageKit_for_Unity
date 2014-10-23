using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System;

/// <summary>
/// ユーティリティクラス
/// </summary>
namespace Utils
{
	/// <summary>
	/// オブジェクト関係のユーティリティ
	/// </summary>
	public class ObjectUtil
	{	
		/// <summary>
		/// 子供のレイヤーを全て変更する
		/// </summary>
		public static void SetLayerChildren(GameObject rootObject, int layer)
		{
			for(int i = 0; i < rootObject.transform.childCount; i++)
			{
				GameObject child = rootObject.transform.GetChild(i).gameObject;
				child.layer = rootObject.layer;
				
				SetLayerChildren(child, child.layer);
				
				child = null;
			}
		}
		
		/// <summary>
		/// 階層のRenderBoundsを取得する
		/// </summary>
		public static Bounds GetRenderBoundsChildren(GameObject root)
		{
			Renderer[] renderers = root.GetComponentsInChildren<Renderer>();
		
			//rendererが無い場合は無効
			if(renderers.Length == 0)
				return new Bounds(Vector3.zero, Vector3.zero);
			
			Vector3 vertexPosMax = renderers[0].bounds.max;
			Vector3 vertexPosMin = renderers[0].bounds.min;
			
			foreach(Renderer rndr in renderers)
			{
				if(rndr.enabled)
				{
					//maxを比較
					vertexPosMax = Vector3.Max(vertexPosMax, rndr.bounds.max);
					
					//minを比較
					vertexPosMin = Vector3.Min(vertexPosMin, rndr.bounds.min);
				}
			}
			
			Vector3 center = (vertexPosMax + vertexPosMin) * 0.5f;
			Vector3 size = vertexPosMax - vertexPosMin;
			
			return new Bounds(center, size);
		}
		
		/// <summary>
		/// 階層の表示設定をまとめて変更する
		/// </summary>
		public static void SetRenderEnabledChildren(GameObject root, bool isRender)
		{
			Renderer[] renderers = root.GetComponentsInChildren<Renderer>();
			
			foreach(Renderer rndr in renderers)
				rndr.enabled = isRender;
		}
		
		/// <summary>
		/// リストをtransform.position.zでZソートする
		/// </summary>
		public static void ZSort(List<GameObject> objectList)
		{
			ZSortOrder order = new ZSortOrder();
			objectList.Sort(order);
		}
	}
	
	/// <summary>
	/// transform.position.zによるZソート用の比較関数
	/// </summary>
	public class ZSortOrder : IComparer<GameObject>
	{	
		public int Compare(GameObject objA, GameObject objB)
		{
			if((objA != null) && (objB != null))
			{
				if(objA.transform.position.z > objB.transform.position.z)
					return 1;
				else if(objA.transform.position.z < objB.transform.position.z)
					return -1;
				else
					return 0;
			}
			else
				return 0;
		}
	}
	
	
	/// <summary>
	/// カメラによる座標変換ユーティリティ
	/// </summary>
	public class CameraUtil
	{	
		/// <summary>
		/// レイヤーからカメラを取得する
		/// </summary>
		public static Camera FindCameraForLayer(int layer)
		{
			int layerMask = (1 << layer);
			Camera[] cameras = GameObject.FindObjectsOfType(typeof(Camera)) as Camera[];
			
			for(int i = 0; i < cameras.Length; i++)
			{
				if((cameras[i].cullingMask & layerMask) != 0)
					return cameras[i];
			}
			
			return null;
		}
		
		/// <summary>
		/// スクリーン座標をOrsoカメラ座標で正規化(-1.0～1.0)
		/// </summary>
		public static Vector3 NormalizeScreenPosition(Camera orthoCamera, Vector3 screenPosition)
		{
			float size = orthoCamera.orthographicSize;
			
			float v = size * 2;
			float h = v * ((float)Screen.width / (float)Screen.height);
			
			float x = ((screenPosition.x / Screen.width) - 0.5f) * h ;
			float y = ((screenPosition.y / Screen.height) - 0.5f) * v;
			
			Vector3 norm = new Vector3(x, y, 0.0f);
			
			return norm;
		}
		
		/// <summary>
		/// Viewportでの画面はみ出しを修正する
		/// </summary>
		public static Vector3 ViewportProtrusion(Vector3 viewportPosition, Vector2 ratio)
		{
			float protX;
			if(viewportPosition.x < (1.0f - ratio.x) / 2.0f)
				protX = (1.0f - ratio.x) / 2.0f;
			else if(viewportPosition.x > 1.0f - ((1.0f - ratio.x) / 2.0f))
				protX = 1.0f - ((1.0f - ratio.x) / 2.0f);
			else
				protX = viewportPosition.x;
			
			float protY;
			if(viewportPosition.y < (1.0f - ratio.y) / 2.0f)
				protY = (1.0f - ratio.y) / 2.0f;
			else if(viewportPosition.y > 1.0f - ((1.0f - ratio.y) / 2.0f))
				protY = 1.0f - ((1.0f - ratio.y) / 2.0f);
			else
				protY = viewportPosition.y;
			
			return new Vector3(protX, protY, viewportPosition.z);
		}
		
	}


	/// <summary>
	/// テキスト整形処理関連のユーティリティ
	/// </summary>
	public class TextUtil
	{	
		/// <summary>
		/// テキストの文字数制限
		/// </summary>
		public static string CutoutText(string text, int maxCount)
		{
			//行端の不要文字を削除
			text = text.Trim();
			
			string cutOutText;
			if(text.Length > maxCount)
				cutOutText = text.Substring(0, maxCount);
			else
				cutOutText = text;
			
			return cutOutText;
		}
		
		/// <summary>
		/// テキストの行揃え
		/// </summary>
		public static string AlignText(string text, int lineCount)
		{
			//行端の不要文字を削除
			text = text.Trim();
			
			string alignText = "";
			
			for(int i = 0; i < text.Length; i++)
			{
				//改行を挿入
				if((i > 0) && ((i % lineCount) == 0))
					alignText += "\n";
				
				alignText += text.Substring(i, 1);
			}
			
			return alignText;
		}
		
		/// <summary>
		/// 改行入りテキストを行文字数及び行数指定で整形
		/// </summary>
		public static string CutoutLineAndLfText(string lfText, int maxLineCount, int maxLineStringCount)
		{
			string[] splitLines = lfText.Split('\n');
			
			string alignedText = "";
			for(int i = 0; i < splitLines.Length; i++)
			{
				string aligned = AlignText(splitLines[i], maxLineStringCount);
				alignedText += aligned;
				
				if(i < (splitLines.Length - 1))
					alignedText += "\n";
			}
			
			string[] alignedSplitLines = alignedText.Split('\n');
			
			string cutOutLineText = "";
			for(int j = 0; j < maxLineCount; j++)
			{
				cutOutLineText += alignedSplitLines[j];
				
				if(j < (maxLineCount - 1))
					cutOutLineText += "\n";
			}
			
			return cutOutLineText;
		}
		
		/// <summary>
		/// 改行入りテキストを行数指定で整形
		/// </summary>
		public static string CutoutLine(string lfText, int maxLineCount)
		{	
			string[] splitLines = lfText.Split('\n');
			
			string cutOutLineText = "";
			for(int i = 0; i < maxLineCount; i++)
			{
				cutOutLineText += splitLines[i];
				
				if(i < (maxLineCount - 1))
					cutOutLineText += "\n";
			}
			
			return cutOutLineText;
		}
		
		/// <summary>
		/// 改行を消す
		/// </summary>
		public static string RemoveEOL(string text)
		{
			return text.Replace("\n", "");
		}
		
		/// <summary>
		/// WWWクラスを使用時の半角#のエラーを回避する
		/// </summary>
		public static string EscapeSingleByteSharpForWWW(string text)
		{
			return text.Replace("#","%23");
		}
		
		/// <summary>
		/// テキストのエンコード変換
		/// </summary>
		public static string ConvertEncoding(string srcString, Encoding destEncording)
		{
			byte[] srcBytes = Encoding.ASCII.GetBytes(srcString);
			byte[] destBytes = Encoding.Convert(Encoding.ASCII, destEncording, srcBytes);
			
			return destEncording.GetString(destBytes);
		}
		
		/// <summary>
		/// バイト数を考慮してSubstringする
		/// </summary>
		public static string SubstringDoubleByte(string srcString, int startIndex, int endIndex, Encoding encoding)
		{
			char[] charArry = srcString.ToCharArray();
			List<List<char>> byteCheckedChars = new List<List<char>>(); 
			
			for(int i = 0; i < charArry.Length; i++)
			{
				List<char> doubleByteChar = new List<char>(); 
				
				if(isDoubleByteChar(charArry[i], encoding)) //全角2バイト文字
				{
					doubleByteChar.Add(charArry[i]);
				}
				else //半角1バイト文字
				{
					doubleByteChar.Add(charArry[i]);
					
					if((i + 1) < charArry.Length)
					{
						if(!isDoubleByteChar(charArry[i + 1], encoding)) //半角1バイト文字
						{
							doubleByteChar.Add(charArry[i + 1]);
							i++;
						}
					}
				}
				
				byteCheckedChars.Add(doubleByteChar);
			}
			
			string subString = "";
			for(int j = 0; j < byteCheckedChars.Count; j++)
			{
				if(j < startIndex)
					continue;
				
				if(j >= endIndex)
					break;
				
				for(int k = 0; k < byteCheckedChars[j].Count; k++)
					subString += byteCheckedChars[j][k];
			}
			
			return subString;
		}
		
		/// <summary>
		/// バイト数を考慮して文字数をカウント
		/// </summary>
		public static int DoubleByteLength(string srcString, Encoding encoding)
		{
			char[] charArry = srcString.ToCharArray();
			float ct = 0.0f;
			
			for(int i = 0; i < charArry.Length; i++)
			{
				if(isDoubleByteChar(charArry[i], encoding))
					ct += 1.0f;
				else
					ct += 0.5f;
			}
			
			return Mathf.CeilToInt(ct);
		}
		
		/// <summary>
		/// 2バイト文字のチェック
		/// </summary>
		private static bool isDoubleByteChar(char srcChar, Encoding encoding)
		{
			if(encoding.GetByteCount(srcChar.ToString()) > 1)
				return true;
			else
				return false;
		}
		
		/// <summary>
		/// 文字列が指定した幅に収まるかチェックし収まらない場合は改行する
		/// </summary>
		public static void CalcTextBox(TextMesh textMesh, float width = 1000.0f)
		{
			//既に収まっている場合
			if(textMesh.renderer.bounds.size.x / textMesh.transform.lossyScale.x <= width)
				return;
			
			//スケール0は無効
			if(textMesh.transform.lossyScale.x <= 0.0f || textMesh.transform.lossyScale.y <= 0.0f || textMesh.transform.lossyScale.z <= 0.0f)
				return;
			
			//サイズが0の場合
			if(width <= 0.0f)
				return;
			
			string processedText = "";
			string tempText = textMesh.text;
			
			//幅設定
			int start = 0;
			int mid = 0;
			int min = 0;
			int max = tempText.Length;
			while(mid < tempText.Length)
			{
				if(min != 0)
					processedText += "\n";
				
				start = mid;
				min = mid;
				max = tempText.Length;
				
				while(min <= max)
				{
					mid = (min + max) / 2;
					
					//midで文字幅を比較
					textMesh.text = tempText.Substring(start, mid - start);
					
					float size = textMesh.renderer.bounds.size.x / textMesh.transform.lossyScale.x;
					if(size < width)
					{
						min = mid + 1;
					}
					else if(size >= width)
					{
						max = mid - 1;
						mid--; //はみ出して終了を防ぐ
					}
					else
						break;
				}
				
				//改行後の先頭文字をチェック
				int length = start + (mid - start + 1);
				if(length <= tempText.Length)
				{
					string tmpChar = tempText.Substring(mid, 1);
					string[] notKaigyouArry = new string[]{",", ".", ";", ":", "、", "。"};
					
					foreach(string checkChar in notKaigyouArry)
					{
						if(tmpChar == checkChar)
						{
							mid++;
							break;
						}
					}
				}
				
				processedText += tempText.Substring(start, mid - start);
			}
			
			textMesh.text = processedText;
		}
	}
	
	
	/// <summary>
	/// スクリーンキャプチャ関連のユーティリティ
	/// </summary>
	public class CaptureUtil : MonoBehaviour
	{	
		/// <summary>
		/// 範囲指定をしてPNGで保存
		/// </summary>
		public static IEnumerator CaptureRangeJPG(string dirPath, string fileName, Rect range)
		{
			yield return new WaitForEndOfFrame();
			
			//範囲でキャプチャをする
			Texture2D screenShot = new Texture2D((int)range.width, (int)range.height);
			screenShot.ReadPixels(range, 0, 0);
			
			//保存先の確認
			if(!Directory.Exists(dirPath))
				Directory.CreateDirectory(dirPath);
			
			//拡張子の確認
			if(Path.GetExtension(fileName).ToUpper() != ".JPG")
				fileName = Path.GetFileNameWithoutExtension(fileName) + ".jpg";
			
			//JPEGエンコード
			JPGEncoder encoder = new JPGEncoder(screenShot, 100.0f);
			encoder.doEncoding();
			while(!encoder.isDone)
				yield return 0;
			
			//保存
			File.WriteAllBytes(Path.Combine(dirPath, fileName), encoder.GetBytes());
			
			encoder = null;
			Texture2D.Destroy(screenShot);
		}
	}
	
	
	/// <summary>
	/// アプリケーション関連のユーティリティ
	/// </summary>
	public class WindowsUtil : MonoBehaviour
	{
		[DllImport("user32.dll")]
		private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
		
		[DllImport("user32.dll", EntryPoint = "FindWindow")]
		private static extern IntPtr FindWindowByCaption(IntPtr zeroOnly, string lpWindowName);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool SetForegroundWindow(IntPtr hwnd);

		[DllImport("user32.dll")]
		private static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll")]
		private static extern IntPtr GetTopWindow(IntPtr hwnd);

		[DllImport("user32.dll")]
		private static extern IntPtr GetNextWindow(IntPtr hwnd, uint wCmd);

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
		
		//private static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
		//private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
		//private static readonly IntPtr HWND_TOP = new IntPtr(0);
		private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

		private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;
		private const uint SWP_FRAMECHANGED = 0x0020;
		private const uint TOPMOST_FLAGS = (SWP_NOSIZE | SWP_NOMOVE);
		
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

			//IntPtr hWnd = GetTopWindow(IntPtr.Zero);
			IntPtr hWnd = GetForegroundWindow();
			do
			{
				if((GetWindowLong(hWnd, GWL_HWNDPARENT) != 0) || !IsWindowVisible(hWnd))
					continue;

				uint procID = 0;
				GetWindowThreadProcessId(hWnd, ref procID);
				if(procID == currentProcID)
					return hWnd;

				hWnd = GetNextWindow(hWnd, GW_HWNDNEXT);
			}
			while(hWnd != IntPtr.Zero);

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
			SetForegroundWindow(hWnd);
		}

		/// <summary>
		/// ポップアップウィンドウを設定する
		/// </summary>
		public static void SetPopupWindow(int cx, int cy)
		{
			IntPtr hWnd = GetApplicationWindowHandle();

			MoveWindow(hWnd, 0, 0, cx, cy, false);
			
			Process currentProc = Process.GetCurrentProcess();
			currentProc.PriorityClass = ProcessPriorityClass.RealTime;
			
			uint old_style = GetWindowLong(hWnd, GWL_STYLE);
			uint new_style = (old_style & ~(WS_CAPTION | WS_BORDER | WS_DLGFRAME | WS_THICKFRAME));
			SetWindowLong(hWnd, GWL_STYLE, new_style);
			
			SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_FRAMECHANGED);
		}
	}
}
