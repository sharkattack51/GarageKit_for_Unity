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

using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Tenebrous.EditorEnhancements
{
    [InitializeOnLoad]
    public static class Main
    {
	    private static List<EditorEnhancement> list;

        static Main()
        {
			list = new List<EditorEnhancement>();

			// find all classes which derive from EditorEnhancement
			// and create an instance of each one

			Type tEnhancement = typeof( EditorEnhancement );
			Assembly a = Assembly.GetAssembly( tEnhancement );
			foreach( Type type in a.GetTypes() )
				if( type.IsClass && !type.IsAbstract && type.IsSubclassOf( tEnhancement ) )
				{
					EditorEnhancement e = (EditorEnhancement)Activator.CreateInstance(type);
					list.Add( e );
				}


			// enable all those which are actually enabled

	        foreach (EditorEnhancement e in list)
		        if( EditorPrefs.GetBool( e.Prefix + "_Enabled", true ) )
			        e.OnEnable();
        }

        public static T Enhancement<T>() where T : EditorEnhancement
        {
            foreach( EditorEnhancement e in list )
                if (e is T)
                    return (T)e;

            return null;
        }

        [PreferenceItem("Enhancements")]
        public static void DrawPreferences()
        {
			// create a single preferences tab to manage all the enhancement's preferences

            foreach (EditorEnhancement e in list)
            {
                EditorGUILayout.BeginHorizontal();

                bool enabled = Bool[e,"Enabled",true];
                bool newEnabled = EditorGUILayout.Toggle( enabled, GUILayout.Width( 32 ) );
	            Bool[ e, "Enabled" ] = newEnabled;

                if( newEnabled != enabled )
                {
                    if( newEnabled )
                        e.OnEnable();
                    else
                        e.OnDisable();
                }

                bool expanded = Bool[e,"Expanded"];
                bool newExpanded;

	            if (e.HasPreferences)
		            newExpanded = EditorGUILayout.Foldout(expanded, "  " + e.Name);
	            else
	            {
		            GUILayout.Space(12);
					EditorGUILayout.LabelField("  " + e.Name);
					newExpanded = false;
				}

	            Bool[ e, "Expanded" ] = newExpanded;

                if (newExpanded != expanded)
                {
                    if( newExpanded )
                        foreach( EditorEnhancement e2 in list )
                            if (e != e2 && Bool[e2, "Expanded"])
                                Bool[e2, "Expanded"] = false;
                }

                EditorGUILayout.EndHorizontal();

                if( expanded )
                {
                    EditorGUILayout.Separator();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginVertical();
                    e.DrawPreferences();
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Separator();
                }

                EditorGUILayout.Space();
            }
        }

		public static Bools Bool = new Bools();
		public class Bools
		{
			public bool this[EditorEnhancement pFor, string pName, bool pDefault = false]
			{
				get { return ( EditorPrefs.GetBool( "EE_" + pFor.Prefix + "_" + pName, pDefault ) ); }
				set { EditorPrefs.SetBool( "EE_" + pFor.Prefix + "_" + pName, value ); }
			}
		}

		public static Ints Int = new Ints();
		public class Ints
		{
			public int this[EditorEnhancement pFor, string pName, int pDefault = 0]
			{
				get { return ( EditorPrefs.GetInt( "EE_" + pFor.Prefix + "_" + pName, pDefault ) ); }
				set { EditorPrefs.SetInt( "EE_" + pFor.Prefix + "_" + pName, value ); }
			}
		}
	}

    public class EditorEnhancement
    {
	    public virtual void OnEnable()
        {
        }

        public virtual void OnDisable()
        {
        }

	    public virtual bool HasPreferences
	    {
		    get { return false; }
	    }

        public virtual void DrawPreferences()
        {
        }

        public virtual string Name
        {
            get { return "Nothing"; }
        }

        public virtual string Prefix
        {
            get { return "nothing"; }
        }
    }
}