using UnityEngine;
using System.Collections;

/*
 * Unity5の解像度設定の不具合を回避する。
 * レジストリから該当keyを削除
 */

#if UNITY_STANDALONE_WIN
using Microsoft.Win32;
#endif

public class Unity5ResolutionHelper : MonoBehaviour
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
		
	}
	
#if UNITY_STANDALONE_WIN
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
