#define ENABLE_IOS_FILE_SHARING
//#define DISABLE_BITCODE

using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;
#endif

namespace GarageKit
{
    public class BuildProcessor : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        public int callbackOrder { get { return 99999; } }

        public void OnPreprocessBuild(BuildReport report)
        {

        }

        public void OnPostprocessBuild(BuildReport report)
        {
#if UNITY_IOS
            if(report.summary.platform == BuildTarget.iOS)
            {
                // modifi Info.plist
                {
                    string plistPath = report.summary.outputPath + "/Info.plist";
                    PlistDocument plist = new PlistDocument();
                    plist.ReadFromString(File.ReadAllText(plistPath));

#if ENABLE_IOS_FILE_SHARING
                    plist.root.SetBoolean("UIFileSharingEnabled", true);
#endif

                    File.WriteAllText(plistPath, plist.WriteToString());
                }

                // modifi Unity-iPhone.xcodeproj
                {
                    PBXProject pbx = new PBXProject();
                    string projectPath = report.summary.outputPath + "/Unity-iPhone.xcodeproj/project.pbxproj";
                    pbx.ReadFromString(File.ReadAllText(projectPath));
                    string target = pbx.GetUnityMainTargetGuid();

#if DISABLE_BITCODE
                    pbx.SetBuildProperty(target, "ENABLE_BITCODE", "NO");
#endif

                    File.WriteAllText(projectPath, pbx.WriteToString());
                }
            }
#endif
        }
    }
}
