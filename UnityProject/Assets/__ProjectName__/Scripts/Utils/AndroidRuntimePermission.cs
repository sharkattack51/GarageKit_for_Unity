using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://qiita.com/niusounds/items/2c10bbc08e602d0d575c

namespace GarageKit
{
    public class AndroidRuntimePermission : MonoBehaviour
    {
        public string[] permissions = new string[]{
            "android.permission.CAMERA",
            "android.permission.ACCESS_FINE_LOCATION"
        };

        public Action OnVerifiedPermission;
        public Action OnFailurePermission;

#if UNITY_ANDROID
        private bool permissionRequested = false;
#endif


        void Awake()
        {

        }

        void Start()
        {
#if UNITY_ANDROID
            if(Application.platform != RuntimePlatform.Android)
            {
                if(OnVerifiedPermission != null)
                    OnVerifiedPermission();
            }

            if(!IsAndroidMOrGreater())
            {
                if(OnVerifiedPermission != null)
                    OnVerifiedPermission();
            }

            List<string> requestPermissions = new List<string>();
            foreach(string permission in permissions)
            {
                if(!HasPermission(permission))
                {
                    if(!ShouldShowRequestPermissionRationale(permission))
                        requestPermissions.Add(permission);
                }
            }

            if(requestPermissions.Count == 0)
            {
                if(OnVerifiedPermission != null)
                    OnVerifiedPermission();
            }
            else
            {
                // パーミッションをリクエストして一旦Pause
                permissionRequested = true;
                RequestPermission(requestPermissions.ToArray());
            }
#else
            if(OnVerifiedPermission != null)
                OnVerifiedPermission();
#endif
        }

        void OnApplicationPause(bool pause)
        {
#if UNITY_ANDROID
            if(Application.platform != RuntimePlatform.Android)
                return;
            
            // パーミッションダイアログから復帰時
            if(!pause && permissionRequested)
            {
                int ok = 0;
                foreach(string permission in permissions)
                {
                    if(HasPermission(permission))
                        ok++;
                }

                if(ok == permissions.Length)
                {
                    if(OnVerifiedPermission != null)
                        OnVerifiedPermission();
                }
                else
                {
                    if(OnFailurePermission != null)
                        OnFailurePermission();
                }
            }
#endif
        }

#if UNITY_ANDROID
        public static AndroidJavaObject GetActivity()
        {
            using(var UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                return UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            }
        }

        public static bool IsAndroidMOrGreater()
        {
            using(var VERSION = new AndroidJavaClass("android.os.Build$VERSION"))
            {
                return VERSION.GetStatic<int>("SDK_INT") >= 23;
            }
        }

        public static bool HasPermission(string permission)
        {
            if(IsAndroidMOrGreater())
            {
                using(var activity = GetActivity())
                {
                    return activity.Call<int>("checkSelfPermission", permission) == 0;
                }
            }

            return true;
        }

        public static bool ShouldShowRequestPermissionRationale(string permission)
        {
            if(IsAndroidMOrGreater())
            {
                using(var activity = GetActivity())
                {
                    return activity.Call<bool>("shouldShowRequestPermissionRationale", permission);
                }
            }

            return false;
        }

        public static void RequestPermission(string[] permissions)
        {
            if(IsAndroidMOrGreater())
            {
                using(var activity = GetActivity())
                {
                    activity.Call("requestPermissions", permissions, 0);
                }
            }
        }
#endif
    }
}
