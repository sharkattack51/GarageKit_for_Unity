using UnityEngine;
using System.Collections;

/*
 * Debug用に使用メモリを表示する
 */

public class MemoryProfiler : MonoBehaviour
{	
	public static bool useHUD = true;
	
	private Rect displayRect;
	private Color color = Color.white;
	private GUIStyle style;
	
	private float useMem = 0.0f;
	
	
	void Awake()
	{
		//表示設定
		displayRect = new Rect(Screen.width - 200, Screen.height -100, 75, 50);
	}

	void Start()
	{
		//使用メモリ測定の開始
		StartCoroutine(MemoryCheck());
	}

	void Update()
	{
	
	}
	
	private IEnumerator MemoryCheck()
    {
		//Update FPS
		while( true )
		{
			//string
			useMem = Mathf.Floor((int)Profiler.usedHeapSize / 100000.0f) / 10.0f;
			
			//color
			if(useMem <= 600.0f)
				color = Color.green;
			else if(useMem >= 800.0f)
				color = Color.red;
			else
				color = Color.yellow;
			
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
			displayRect = GUI.Window(200, displayRect, DoWindow, "");
		}
	}

    void DoWindow(int windowID)
    {
        GUI.Label(new Rect(0, 0, displayRect.width, displayRect.height), useMem + " MB", style);
        GUI.DragWindow(new Rect(0, 0, Screen.width, Screen.height));
    }
}
