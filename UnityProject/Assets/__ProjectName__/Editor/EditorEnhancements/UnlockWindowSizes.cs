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

using UnityEngine;
using UnityEditor;

namespace Tenebrous.EditorEnhancements
{
	public class UnlockWindowSizes : EditorEnhancement
	{
		[System.NonSerialized] private static int numWindows = 0;

		public override void OnEnable()
		{
            UnlockSizes();
			EditorApplication.update += Update;
            EditorApplication.playModeStateChanged += UnlockSizes;
		}

		public override void OnDisable()
		{                          
			EditorApplication.update -= Update;
            EditorApplication.playModeStateChanged -= UnlockSizes;
		}

		public override string Name
		{
			get
			{
				return "Unlock Window Sizes";
			}
		}

		public override string Prefix
		{
			get
			{
				return "TeneUnlockWindowSizes";
			}
		}

		private static void UnlockSizes(PlayModeStateChange change)
        {
			UnlockSizes();
		}

        private static void UnlockSizes()
        {
			EditorWindow[] windows = (EditorWindow[]) Resources.FindObjectsOfTypeAll(typeof (EditorWindow));
			foreach (EditorWindow window in windows)
				window.minSize = new Vector2(10, 10);

			numWindows = windows.Length;
		}

		private static int _update;
		private static void Update()
		{
			if (_update++ > 100)
			{
                _update = 0;

                EditorWindow[] windows = (EditorWindow[])Resources.FindObjectsOfTypeAll( typeof( EditorWindow ) );
                if( windows.Length != numWindows )
				    UnlockSizes();
			}
		}
	}
}