using UnityEngine;
using System.Collections;

/*
 * Debug用にフレームレートを表示する
 */
namespace GarageKit
{
	public class FrameRateUtil : MonoBehaviour
	{
		public static bool useHUD = true;
		private static float fps = 0.0f;
		public static float Fps { get{ return fps; } }

		private Rect displayRect;
		private GUIStyle style;

		
		void Awake()
		{
			// 初期表示位置
			displayRect = new Rect(Screen.width - 100.0f, Screen.height -100.0f, 75.0f, 50.0f);
		}

		void Start()
		{
			// FPS測定の開始
			StartCoroutine(FPSCheck());
		}

		void Update()
		{

		}
		
		private IEnumerator FPSCheck()
	    {
			// Update FPS
			while(true)
			{
				int lastFrameCount = Time.frameCount;
				float lastTime = Time.realtimeSinceStartup;

				yield return new WaitForSeconds(0.5f);
 
				FrameRateUtil.fps = Mathf.RoundToInt(
					(Time.frameCount - lastFrameCount) / (Time.realtimeSinceStartup - lastTime));
	        }
	    }
		
		void OnGUI()
		{
			if(useHUD)
			{
				if(style == null)
				{
					style = new GUIStyle(GUI.skin.label);
					style.normal.textColor = Color.white;
					style.alignment = TextAnchor.MiddleCenter;
				}

				// 表示を更新
				if(fps >= 30.0f)
					GUI.color = Color.green;
				else if(fps > 10.0f)
					GUI.color = Color.red;
				else
					GUI.color = Color.yellow;
				
				displayRect = GUI.Window(100, displayRect, DoWindow, "");
			}
		}

	    private void DoWindow(int windowID)
	    {
	        GUI.Label(
				new Rect(0.0f, 0.0f, displayRect.width, displayRect.height),
				FrameRateUtil.fps.ToString("f1") + " FPS", style);

	        GUI.DragWindow(new Rect(0.0f, 0.0f, Screen.width, Screen.height));
	    }
	}
}
