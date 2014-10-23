using UnityEngine;
using System.Collections;

/*
 * Debug用にフレームレートを表示する
 */

public class FrameRateUtil : MonoBehaviour
{
	public static bool useHUD = true;
	public int targetFPS = 60;
	
	private Rect displayRect;
	private float accum   = 0.0f;
	private int frames  = 0;
	private Color color = Color.white;
	private string sFPS = "";
	private GUIStyle style;
	
	
	void Awake()
	{
		//フレームレートの固定
		Application.targetFrameRate = targetFPS;
		
		//表示設定
		displayRect = new Rect(Screen.width - 100, Screen.height -100, 75, 50);
	}

	void Start()
	{
		//FPS測定の開始
		StartCoroutine(FPSCheck());
	}

	void Update()
	{
		accum += Time.timeScale / Time.deltaTime;
		frames++;
	}
	
	private IEnumerator FPSCheck()
    {
		//Update FPS
		while( true )
		{
			float fps = accum/frames;
			
			//string
			sFPS = fps.ToString("f" + Mathf.Clamp(1, 0, 10));
			
			//color
			if(fps >= 30)
				color = Color.green;
			else if(fps > 10)
				color = Color.red;
			else
				color = Color.yellow;
			
			//reset count
			accum = 0.0f;
			frames = 0;
			
            yield return new WaitForSeconds(0.5f);
        }
    }
	
	void OnGUI()
	{
		if(useHUD)
		{
			//スタイルの設定
			if( style == null )
			{
				style = new GUIStyle(GUI.skin.label);
				style.normal.textColor = Color.white;
				style.alignment = TextAnchor.MiddleCenter;
			}
	        
			//表示を更新
			GUI.color = color;
			displayRect = GUI.Window(100, displayRect, DoWindow, "");
		}
	}

    void DoWindow(int windowID)
    {
        GUI.Label(new Rect(0, 0, displayRect.width, displayRect.height), sFPS + " FPS", style);
        GUI.DragWindow(new Rect(0, 0, Screen.width, Screen.height));
    }
}
