using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GarageKit
{
    public class IOSUtil
    {
        public static void NoBackupDocumentsFolder()
        {
#if UNITY_IOS
            UnityEngine.iOS.Device.SetNoBackupFlag(Application.persistentDataPath);
#endif
        }
    }
}
