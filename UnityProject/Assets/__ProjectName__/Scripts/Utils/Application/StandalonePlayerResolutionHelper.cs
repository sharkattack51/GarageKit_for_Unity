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
/*
 * Unity2018 or Newer
 * 1) Download nuget package
 *     https://www.nuget.org/packages/Microsoft.Win32.Registry/4.5.0
 * 2) Rename .zip and unzip
 * 3) Copy /lib/net461/Microsoft.Win32.Registry.dll to Plugins
 * 4) Player settings ScriptingRuntimeVersion to [.NET 4.x Equivalent]
 * 5) Player settings ApiCompatibilityLevel to [.NET 4.x]
 */
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

#if UNITY_2018_3_OR_NEWER
				if(PlayerSettings.scriptingRuntimeVersion != ScriptingRuntimeVersion.Latest)
					Debug.LogError("StandalonePlayerResolutionHelper :: PlayerSettings.scriptingRuntimeVersion is Lagacy");

				if(PlayerSettings.GetApiCompatibilityLevel(BuildTargetGroup.Standalone) != ApiCompatibilityLevel.NET_4_6)
					Debug.LogError("StandalonePlayerResolutionHelper :: PlayerSettings.ApiCompatibilityLevel is Low");
#endif
			}
#endif
		}
		
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
		void OnApplicationQuit()
		{
			RegistryKey key = Registry.CurrentUser;
			if(key == null)
				Debug.LogError("StandalonePlayerResolutionHelper :: Registry.CurrentUser is Null");
			else
			{
				string subkey = "Software" + @"\" + playerSettingsCompanyName + @"\" + playerSettingsProductName;
				key = key.OpenSubKey(subkey, true);
				if(key == null)
					Debug.LogError("StandalonePlayerResolutionHelper :: don't open sub key [" + subkey + "]");
				else
				{
#if UNITY_2018_3_OR_NEWER
					key.DeleteValue("Screenmanager Fullscreen mode_h3630240806", false);
					key.DeleteValue("Screenmanager Resolution Use Native_h1405027254", false);
#else
					key.DeleteValue("Screenmanager Is Fullscreen mode_h3981298716", false);
#endif
					key.DeleteValue("Screenmanager Resolution Height_h2627697771", false);
					key.DeleteValue("Screenmanager Resolution Width_h182942802", false);
					key.DeleteValue("UnityGraphicsQuality_h1669003810", false);
					key.DeleteValue("UnitySelectMonitor_h17969598", false);
					key.Close();
				}
			}
		}
#endif
	}
}
