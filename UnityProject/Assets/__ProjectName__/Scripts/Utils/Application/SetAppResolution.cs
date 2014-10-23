using UnityEngine;
using System.Collections;

/*
 * コンテンツ起動後にウィンドウサイズを再設定する
 */

public class SetAppResolution : MonoBehaviour
{
	//解像度設定タイプ
	public enum APP_RESOLUTION
	{
		CURRENT_FULLSCREEN = 0,
		DISPLAY_SUPPORTED,
		CUSTOM_RESOLUTION,
		CUSTOM_RESOLUTION_POPUPWINDOW
	}
	
	public APP_RESOLUTION resolutionType = APP_RESOLUTION.CURRENT_FULLSCREEN;
	
	//フルスクリーン設定
	public bool fullScreen = true;
	
	//ディスプレイ対応解像度指定時のID
	public int resolutionID = 0;
	
	//カスタムタイプ時の指定解像度
	public int customW = 0;
	public int customH = 0;
	
	
	void Start()
	{
		if(Application.platform != RuntimePlatform.WindowsEditor)
		{
			int setWidth = 0;
			int setHeight = 0;
			
			switch(resolutionType)
			{
				case APP_RESOLUTION.CURRENT_FULLSCREEN:
					setWidth = Screen.currentResolution.width;
					setHeight = Screen.currentResolution.height;
					DebugConsole.Log("set CURRENT_FULLSCREEN resolution " + setWidth.ToString() + " : " + setHeight.ToString());
					break;
					
				case APP_RESOLUTION.DISPLAY_SUPPORTED:
					setWidth = Screen.resolutions[resolutionID].width;
					setHeight = Screen.resolutions[resolutionID].height;
					DebugConsole.Log("set DISPLAY_SUPPORTED resolution " + setWidth.ToString() + " : " + setHeight.ToString());
					break;
					
				case APP_RESOLUTION.CUSTOM_RESOLUTION:
					setWidth = customW;
					setHeight = customH;
					DebugConsole.Log("set CUSTOM_RESOLUTION resolution " + setWidth.ToString() + " : " + setHeight.ToString());
					break;

				case APP_RESOLUTION.CUSTOM_RESOLUTION_POPUPWINDOW:
					setWidth = customW;
					setHeight = customH;
					DebugConsole.Log("set CUSTOM_RESOLUTION_POPUPWINDOW resolution " + setWidth.ToString() + " : " + setHeight.ToString());
					break;
			}
			
			//解像度を変更
			if(resolutionType == APP_RESOLUTION.CUSTOM_RESOLUTION_POPUPWINDOW)
				Utils.WindowsUtil.SetPopupWindow(setWidth, setHeight);
			else
				Screen.SetResolution(setWidth, setHeight, fullScreen);
		}
	}
}
