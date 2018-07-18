﻿//#define USE_TOUCH_SCRIPT
using UnityEngine;
using System.Collections;

/// <summary>
/// スクリーンセーバーオブジェクト
/// </summary>
namespace GarageKit
{
	public class ScreenSaverObject : MonoBehaviour
	{
		/// <summary>
		/// スクリーンセーバー終了イベント
		/// </summary>
		public delegate void OnEndScreenSaverDelegate();
		public event OnEndScreenSaverDelegate OnEndScreenSaver;
		protected void InvokeOnEndScreenSaver()
		{
			if(OnEndScreenSaver != null)
				OnEndScreenSaver();
		}
		
		// 入力タイプ
		public enum INPUT_TYPE
		{
			MOUSE = 0,
			INPUTTOUCH,
			WTOUCH,
			KINECT
		}
		public static INPUT_TYPE inputType = INPUT_TYPE.MOUSE;
		
		// 参照
		public GameObject fadePlane;
		
		private bool isValid = false;
		public bool IsValid { get{ return isValid; } }
		
		private Color defaultFadeColor = Color.white;
		
		
		void Awake()
		{

		}
		
		void Start()
		{
			// 設定ファイルから入力モードの取得
			string mode = ApplicationSetting.Instance.GetString("InputMode").ToLower();
			if(mode == "mouse")
				inputType = INPUT_TYPE.MOUSE;
			else if(mode == "touch")
				inputType = INPUT_TYPE.WTOUCH;
			else 
				inputType = INPUT_TYPE.KINECT;
			
			// 初期非表示
			fadePlane.gameObject.GetComponent<Renderer>().enabled = false;
		}
		
		void Update()
		{
			// 入力チェック
			CheckInput();
		}
		
		
		/// <summary>
		/// 入力チェック
		/// </summary>
		private void CheckInput()
		{
			if(!isValid)
			{
				// for Mouse
				if(Application.platform == RuntimePlatform.WindowsEditor || inputType == INPUT_TYPE.MOUSE)
				{
					if(Input.GetAxis("Mouse X") + Input.GetAxis("Mouse Y") > 0.0f)
						DisableScreenSaver();
				}
			
				// for Touch
				else if(inputType == INPUT_TYPE.INPUTTOUCH)
				{
					if(Input.touchCount > 0)
						DisableScreenSaver();
				}
			
				// for WTouch
				else if(inputType == INPUT_TYPE.WTOUCH)
				{
#if !USE_TOUCH_SCRIPT
					if(Input.touchCount > 0)					
#else
					if(TouchScript.TouchManager.Instance.PressedPointersCount > 0)
#endif	
						DisableScreenSaver();
				}
			
				// for Kinect
				else if(inputType == INPUT_TYPE.KINECT)
				{
					//TODO for Kinect
				}
			}
		}
		
		/// <summary>
		/// スクリーンセーバーを有効化
		/// </summary>
		public void EnableScreenSaver()
		{
			isValid = true;
			
			fadePlane.gameObject.GetComponent<Renderer>().enabled = true;
			
			// フェード
			iTween.ColorTo(
				fadePlane,
				iTween.Hash(
					"a", defaultFadeColor.a,
					"time", 0.5f));
		}
		
		/// <summary>
		/// スクリーンセーバーを無効化
		/// </summary>
		public void DisableScreenSaver()
		{
			isValid = false;
			
			// フェード
			iTween.ColorTo(
				fadePlane,
				iTween.Hash(
					"a", 0.0f,
					"time", 0.5f,
					"oncomplete", "faded",
					"oncompletetarget", this.gameObject));
		}
		
		private void faded()
		{
			fadePlane.gameObject.GetComponent<Renderer>().enabled = false;
		}
	}
}
