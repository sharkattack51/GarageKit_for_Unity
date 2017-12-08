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

		static public string FileDir()
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

		static public string CacheDir()
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

		static public string ExternalFilesDir()
		{
			string _ExternalFilesDir = "";

#if !UNITY_EDITOR && UNITY_ANDROID

			using(AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				using(AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
				{
					using(AndroidJavaObject externalFilesDir = currentActivity.Call<AndroidJavaObject>("getExternalFilesDir", null))
					{
						_ExternalFilesDir = externalFilesDir.Call<string>("getCanonicalPath");
					}
				}
			}

#endif

			return _ExternalFilesDir;
		}

		static public string ExternalCacheDir()
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
	}
}