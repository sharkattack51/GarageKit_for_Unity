/*
 * Copyright (c) 2016 Tenebrous
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 *
 * Latest version: https://bitbucket.org/Tenebrous/unityeditorenhancements/wiki/Home
*/

#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_5 || UNITY_5_3_OR_NEWER
#define UNITY_4_PLUS
#endif

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Tenebrous.EditorEnhancements
{
    public class TeneProjectSettings : EditorEnhancement
    {
        public override void OnEnable()
        {
            List<string> settings = new List<string>();

#if UNITY_2020_3_OR_NEWER
            if( VersionControlSettings.mode == "Hidden Meta Files" )
#else
            if( EditorSettings.externalVersionControl == "Hidden Meta Files" )
#endif
                settings.Add( "· Meta files: 'Hidden Meta Files' to 'Visible Meta Files'"  );

            if( EditorSettings.serializationMode == SerializationMode.Mixed )
                settings.Add( "· Serialization mode: 'Mixed' to 'Force Text'" );

            if( settings.Count > 0 )
            {
                bool update = EditorUtility.DisplayDialog(
                    "Update settings",
                    "Would you like to make the following changes?\n\n"
                    + string.Join( "\n\n", settings.ToArray() ),
                    "OK",
                    "No thanks"
                );

                if( update )
                {
#if UNITY_2020_3_OR_NEWER
                    if( VersionControlSettings.mode == "Hidden Meta Files" )
                        VersionControlSettings.mode = "Visible Meta Files";
#else
                    if( EditorSettings.externalVersionControl == "Hidden Meta Files" )
                        EditorSettings.externalVersionControl = "Visible Meta Files";
#endif

                    if( EditorSettings.serializationMode == SerializationMode.Mixed )
                        EditorSettings.serializationMode = SerializationMode.ForceText;
                }
            }
        }

        public override void OnDisable()
        {
        }

        public override string Name
        {
            get
            {
                return "Project Settings";
            }
        }

        public override string Prefix
        {
            get
            {
                return "TeneProjectSettings";
            }
        }

        public override bool HasPreferences
        {
            get
            {
                return false;
            }
        }

        public override void DrawPreferences()
        {
        }
    }
}