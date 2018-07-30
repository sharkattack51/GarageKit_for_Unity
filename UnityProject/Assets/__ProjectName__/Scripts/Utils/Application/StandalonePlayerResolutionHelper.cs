using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

/*
 * StandalonePlayerの解像度設定の不具合を回避する。
 * レジストリから該当keyを削除
 */

#if UNITY_STANDALONE_WIN
using Microsoft.Win32;
#endif

namespace GarageKit
{
	[ExecuteInEditMode]
	public class StandalonePlayerResolutionHelper : MonoBehaviour
	{
		/*
		* ビルド設定のPlayerSettingsよりCompanyNameを入力してください
		*/
		public string playerSettingsCompanyName = "";
		
		/*
		*　ビルド設定のPlayerSettingsよりProductNameを入力してください
		*/
		public string playerSettingsProductName = "";
		
		
		void Start()
		{
#if UNITY_EDITOR
			if(Application.platform == RuntimePlatform.WindowsEditor)
			{
				if(playerSettingsCompanyName == "" || playerSettingsCompanyName != PlayerSettings.companyName)
					playerSettingsCompanyName = PlayerSettings.companyName;

				if(playerSettingsProductName == "" || playerSettingsProductName != PlayerSettings.productName)
					playerSettingsProductName = PlayerSettings.productName;
			}
#endif
		}
		
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
		void OnApplicationQuit()
		{
			RegistryKey key = Registry.CurrentUser;
			key = key.OpenSubKey("Software" + @"\" + playerSettingsCompanyName + @"\" + playerSettingsProductName, true);
			key.DeleteValue("Screenmanager Is Fullscreen mode_h3981298716", false);
			key.DeleteValue("Screenmanager Resolution Height_h2627697771", false);
			key.DeleteValue("Screenmanager Resolution Width_h182942802", false);
			key.DeleteValue("UnityGraphicsQuality_h1669003810", false);
			key.DeleteValue("UnitySelectMonitor_h17969598", false);
			key.Close();
		}
#endif
	}
}
