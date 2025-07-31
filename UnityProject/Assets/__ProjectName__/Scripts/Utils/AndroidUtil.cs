using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

namespace GarageKit
{
    public class AndroidUtil
    {
#region ANDROID_PATH
        public static string FileDir()
        {
            string _FilesDir = "";

#if !UNITY_EDITOR && UNITY_ANDROID
            using(AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using(AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    using(AndroidJavaObject filesDir = currentActivity.Call<AndroidJavaObject>("getFilesDir"))
                    {
                        _FilesDir = filesDir.Call<string>("getCanonicalPath");
                    }
                }
            }
#endif
            return _FilesDir;
        }

        public static string CacheDir()
        {
            string _CacheDir = "";

#if !UNITY_EDITOR && UNITY_ANDROID
            using(AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using(AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    using(AndroidJavaObject cacheDir = currentActivity.Call<AndroidJavaObject>("getCacheDir"))
                    {
                        _CacheDir = cacheDir.Call<string>("getCanonicalPath");
                    }
                }
            }
#endif
            return _CacheDir;
        }

        public static string ExternalFilesDir()
        {
            string _ExternalFilesDir = "";

#if !UNITY_EDITOR && UNITY_ANDROID
            using(AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using(AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    using(AndroidJavaObject externalFilesDir = currentActivity.Call<AndroidJavaObject>("getExternalFilesDir"))
                    {
                        _ExternalFilesDir = externalFilesDir.Call<string>("getCanonicalPath");
                    }
                }
            }
#endif
            return _ExternalFilesDir;
        }

        public static string ExternalCacheDir()
        {
            string _ExternalCacheDir = "";

#if !UNITY_EDITOR && UNITY_ANDROID
            using(AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using(AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    using(AndroidJavaObject externalCacheDir = currentActivity.Call<AndroidJavaObject>("getExternalCacheDir"))
                    {
                        _ExternalCacheDir = externalCacheDir.Call<string>("getCanonicalPath");
                    }
                }
            }
#endif
            return _ExternalCacheDir;
        }

        public static string ExternalStorageDir()
        {
            string _ExternalStorageDir = "";

#if !UNITY_EDITOR && UNITY_ANDROID
            using(AndroidJavaClass environment = new AndroidJavaClass("android.os.Environment"))
            {
                using(AndroidJavaObject externalStorageDir = environment.CallStatic<AndroidJavaObject>("getExternalStorageDirectory"))
                {
                    _ExternalStorageDir = externalStorageDir.Call<string>("getCanonicalPath");
                }
            }
#endif
            return _ExternalStorageDir;
        }

        public static string ExternalStoragePublicDir()
        {
            string _ExternalStoragePublicDir = "";

#if !UNITY_EDITOR && UNITY_ANDROID
            using(AndroidJavaClass environment = new AndroidJavaClass("android.os.Environment"))
            {
                using(AndroidJavaObject externalStoragePublicDir = environment.CallStatic<AndroidJavaObject>("getExternalStoragePublicDirectory", ""))
                {
                    _ExternalStoragePublicDir = externalStoragePublicDir.Call<string>("getCanonicalPath");
                }
            }
#endif
            return _ExternalStoragePublicDir;
        }

        public static string DownloadDir()
        {
            string _DownloadDir = "";

#if !UNITY_EDITOR && UNITY_ANDROID
            string[] paths = (Application.persistentDataPath.Replace("Android","")).Split(new string[]{ "//" }, StringSplitOptions.None);
            _DownloadDir = Path.Combine(paths[0], "Download");
#endif
            return _DownloadDir;
        }
#endregion

#region EXTERNAL_ACTIVITY
        public static void OpenActivity(string packageName, string className, bool asNewTask)
        {
#if !UNITY_EDITOR && UNITY_ANDROID
            AndroidJavaClass cUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject oCurrentActivity = cUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject oIntent = new AndroidJavaObject("android.content.Intent");

            oIntent.Call<AndroidJavaObject>("setAction", "android.intent.action.VIEW");
            if(asNewTask)
                oIntent.Call<AndroidJavaObject>("setFlags", 0x10000000); // FLAG_ACTIVITY_NEW_TASK
            oIntent.Call<AndroidJavaObject>("setClassName", packageName, packageName + "." + className);
            oCurrentActivity.Call("startActivity", oIntent);

            oIntent.Dispose();
            oCurrentActivity.Dispose();
#endif
        }
#endregion

#region BROADCAST
        public static void SendBroadcast(string action)
        {
#if !UNITY_EDITOR && UNITY_ANDROID
            AndroidJavaClass cUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject oCurrentActivity = cUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject oIntent = new AndroidJavaObject("android.content.Intent", action);

            oCurrentActivity.Call("sendBroadcast", oIntent);

            oIntent.Dispose();
            oCurrentActivity.Dispose();
#endif
        }
#endregion

        public static int GetApiLevel()
        {
            int apiLevel = 0;

#if !UNITY_EDITOR && UNITY_ANDROID
            using(AndroidJavaClass cVersion = new AndroidJavaClass("android.os.Build$VERSION"))
            {
                apiLevel = cVersion.GetStatic<int>("SDK_INT");
            }
#endif
            return apiLevel;
        }

#region SCOPED STRAGE ACCESS
        public static void RequestAllFilesAccessPermission()
        {
#if UNITY_EDITOR
            Debug.LogWarning("don't forget add to AndroidManifest.xml: <uses-permission android:name=\"android.permission.MANAGE_EXTERNAL_STORAGE\" />");
#endif

#if !UNITY_EDITOR && UNITY_ANDROID
            using(AndroidJavaClass cUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using(AndroidJavaObject oCurrentActivity = cUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                string packageName = oCurrentActivity.Call<string>("getPackageName");

                if(GetApiLevel() >= 30) // Android11
                {
                    using(AndroidJavaClass cEnvironment = new AndroidJavaClass("android.os.Environment"))
                    {
                        bool isExternalStorageManager = cEnvironment.CallStatic<bool>("isExternalStorageManager");
                        if(isExternalStorageManager)
                            return;
                    }

                    using(AndroidJavaObject oIntent = new AndroidJavaObject("android.content.Intent", "android.settings.MANAGE_APP_ALL_FILES_ACCESS_PERMISSION"))
                    using(AndroidJavaClass cUri = new AndroidJavaClass("android.net.Uri"))
                    using(AndroidJavaObject uri = cUri.CallStatic<AndroidJavaObject>("parse", "package:" + packageName))
                    {
                        oIntent.Call<AndroidJavaObject>("addCategory", "android.intent.category.DEFAULT");
                        oIntent.Call<AndroidJavaObject>("setData", uri);
                        oCurrentActivity.Call("startActivity", oIntent);
                    }

                    // restart app
                    Debug.LogWarning("restart app for set all files access permission.");
                    Application.Quit();
                }
            }
#endif
        }
#endregion
    }
}