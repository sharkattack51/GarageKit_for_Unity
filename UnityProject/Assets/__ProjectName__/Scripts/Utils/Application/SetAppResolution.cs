using UnityEngine;
using System.Collections;

/*
 * コンテンツ起動後にウィンドウサイズを再設定する
 */
namespace GarageKit
{
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

		// アスペクト比固定設定
		public bool useFixAspect = false; 
		public float aspectRatio = 1.77777778f; // 16:9
		private Vector2 lastResolution;
		private bool changing = false;
		
		
		void Start()
		{
			if(Application.platform == RuntimePlatform.WindowsPlayer
			|| Application.platform == RuntimePlatform.OSXPlayer)
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
					GarageKit.WindowsUtil.SetPopupWindow(setWidth, setHeight);
				else
					Screen.SetResolution(setWidth, setHeight, fullScreen);
			}

			lastResolution = new Vector2(Screen.width, Screen.height);
		}

		void LateUpdate()
		{
			if(Application.platform == RuntimePlatform.WindowsPlayer
			|| Application.platform == RuntimePlatform.OSXPlayer)
			{
				if(!fullScreen && useFixAspect && !changing)
				{
					if(Screen.width != (int)lastResolution.x
						|| Screen.height != (int)lastResolution.y
						|| Camera.main.aspect != aspectRatio)
					{
						StartCoroutine(SetAspectResolution());
						lastResolution = new Vector2(Screen.width, Screen.height);
					}
				}
			}
		}

		private IEnumerator SetAspectResolution()
		{
			changing = true;
			Screen.SetResolution(Screen.width, (int)((float)Screen.width / aspectRatio), false);

			yield return new WaitForSeconds(0.5F);

			changing = false;
		}
	}
}
