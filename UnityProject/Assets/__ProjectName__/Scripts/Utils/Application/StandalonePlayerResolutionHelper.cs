using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/*
 * StandalonePlayerの解像度設定の不具合を回避する。
 * レジストリから該当keyを削除
 */

/*
 * To use it, change ApiCompatibilityLevel in PlayerSettings to .NET Framework.
 */

namespace GarageKit
{
    [ExecuteInEditMode]
    public class StandalonePlayerResolutionHelper : MonoBehaviour
    {
        // ビルド設定のPlayerSettingsよりCompanyNameを入力してください
        public string playerSettingsCompanyName = "";

        // ビルド設定のPlayerSettingsよりProductNameを入力してください
        public string playerSettingsProductName = "";


        void Start()
        {
#if UNITY_EDITOR && UNITY_STANDALONE_WIN
            if(Application.platform == RuntimePlatform.WindowsEditor)
            {
                if(playerSettingsCompanyName == "" || playerSettingsCompanyName != PlayerSettings.companyName)
                    playerSettingsCompanyName = PlayerSettings.companyName;

                if(playerSettingsProductName == "" || playerSettingsProductName != PlayerSettings.productName)
                    playerSettingsProductName = PlayerSettings.productName;

#if UNITY_2021_1_OR_NEWER
                if(PlayerSettings.GetApiCompatibilityLevel(BuildTargetGroup.Standalone) != ApiCompatibilityLevel.NET_Unity_4_8)
                    Debug.LogError("StandalonePlayerResolutionHelper :: PlayerSettings.ApiCompatibilityLevel to .NET Framework");
#endif
            }
#endif

#if !UNITY_EDITOR
            Microsoft.Win32.SystemEvents.SessionEnding += new Microsoft.Win32.SessionEndingEventHandler((s, e) => {
                e.Cancel = true;
                RemoveFromRegistry();
            });
#endif
        }

        void OnApplicationQuit()
        {
#if !UNITY_EDITOR && UNITY_STANDALONE_WIN
            RemoveFromRegistry();
#endif
        }

#if !UNITY_EDITOR && UNITY_STANDALONE_WIN
        private void RemoveFromRegistry()
        {
            Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser;
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
                    key.DeleteValue("Screenmanager Fullscreen mode_h3630240806", false);
                    key.DeleteValue("Screenmanager Resolution Use Native_h1405027254", false);
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
