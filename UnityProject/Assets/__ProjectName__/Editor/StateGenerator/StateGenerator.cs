using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace GarageKit
{
    [Serializable]
    public class GenerateStateInfo
    {
        public string className = "NewState";
        
        public enum BaseClass
        {
            StateBase = 0,
            AsyncStateBase,
            TimelinedSceneStateBase,
            VRSceneStateBase
        }
        public GenerateStateInfo.BaseClass baseClass = GenerateStateInfo.BaseClass.StateBase;
    }

    public class StateGenerator : EditorWindow
    { 
        private static StateGenerator window;
        private static GenerateStateInfo stateInfo;

        private static Dictionary<GenerateStateInfo.BaseClass, string> templates = new Dictionary<GenerateStateInfo.BaseClass, string>() {
            { GenerateStateInfo.BaseClass.StateBase, "NewState" },
            { GenerateStateInfo.BaseClass.AsyncStateBase, "NewAsyncState" },
            { GenerateStateInfo.BaseClass.TimelinedSceneStateBase, "NewTimelinedSceneState" },
            { GenerateStateInfo.BaseClass.VRSceneStateBase, "NewVRSceneState" }
        };

        [MenuItem("EditorScript/GarageKit/StateGenerator")]
        public static void WindowOpen()
        {
            stateInfo = new GenerateStateInfo();

            window = EditorWindow.GetWindow<StateGenerator>("StateGenerator");
        }

        void OnGUI()
        {
            EditorWindow.GetWindow<StateGenerator>().Focus();
            EditorWindow.GetWindow<StateGenerator>().maximized = false;
            EditorWindow.GetWindow<StateGenerator>().minSize = new Vector2(400, 70);

            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.LabelField("Genarate the New Scene State.");
                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("New State Class", GUILayout.Width(150.0f));
                    EditorGUILayout.LabelField("Base State Class", GUILayout.Width(150.0f));
                    EditorGUILayout.Space();
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal(GUI.skin.box);
                {
                    stateInfo.className = EditorGUILayout.TextField(stateInfo.className, GUILayout.Width(150.0f));
                    stateInfo.baseClass = (GenerateStateInfo.BaseClass)EditorGUILayout.EnumPopup(stateInfo.baseClass, GUILayout.Width(150.0f));

                    if(GUILayout.Button("Generate", GUILayout.Width(70.0f)))
                    {
                        if(stateInfo.className != "")
                            AddSourceProcess(stateInfo);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }

        private static void AddSourceProcess(GenerateStateInfo stateInfo)
        {
            stateInfo.className = stateInfo.className.Substring(0, 1).ToUpper() + stateInfo.className.Substring(1, stateInfo.className.Length - 1);
            if(stateInfo.className.Length > 5)
            {
                if(stateInfo.className.Substring(stateInfo.className.Length - 5, 5) != "State")
                    stateInfo.className += "State";
            }
            else
                stateInfo.className += "State";

            string statesFolderPath = Path.GetFullPath(
                AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("StateBase")[0])).Split(new string[]{"Scripts"}, StringSplitOptions.None)[0] + "Scripts/States";
            string sourcePath = Path.Combine(statesFolderPath, stateInfo.className + ".cs");

            using(FileStream fs = File.Create(sourcePath))
            {
                using(StreamWriter sw = new StreamWriter(fs))
                {
                    string tmplatePath = Path.GetFullPath(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets(templates[stateInfo.baseClass])[0]));
                    string src = File.ReadAllText(tmplatePath).Replace("{className}", stateInfo.className);
                    sw.Write(src);
                }
            }

            Debug.LogWarning("generated: " + sourcePath);
            AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);
            window.Close();
        }
    }
}
