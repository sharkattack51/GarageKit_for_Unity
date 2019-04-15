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

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace Tenebrous.EditorEnhancements
{
	public static class DependencyChecker
	{
		private static HashSet<string> _used = new HashSet<string>();
		private static IEnumerator<string> _assetEnumerator;

		public static void Refresh()
		{
			_used.Clear();
			_assetEnumerator = EnumerateProjectFiles();
		}

		public static bool Running
		{
			get { return _assetEnumerator != null; }
		}

		public static void Continue()
		{
			if (_assetEnumerator != null)
			{
				if (_assetEnumerator.MoveNext())
				{
					string assetpath = _assetEnumerator.Current;
					string[] deps = AssetDatabase.GetDependencies(new string[] { assetpath });

					foreach (string dep in deps)
						if( dep != assetpath )
							_used.Add( dep );

					Common.ProjectWindow.Repaint();
				}
				else
				{
					_assetEnumerator = null;
				}
			}			
		}

		static IEnumerator<string> EnumerateProjectFiles()
		{
			Queue<DirectoryInfo> folders = new Queue<DirectoryInfo>();

			folders.Enqueue(new DirectoryInfo(Application.dataPath));

			while (folders.Count > 0)
			{
				DirectoryInfo current = folders.Dequeue();

				foreach (FileInfo file in current.GetFiles())
				{
					if( !CanFileBeDependant( file.Name ) )
						continue;

					yield return "Assets"
								 + file.FullName
								   .Substring(Application.dataPath.Length)
								   .Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
				}

				foreach (DirectoryInfo folder in current.GetDirectories())
				{
					if( !CanFolderContainDependants(folder.Name) )
						continue;

					folders.Enqueue(folder);
				}
			}
		}

		public static bool IsUsed( string pPath )
		{
			return !CanFileBeDependant(pPath) 
				|| !CanPathContainDependants(pPath) 
				|| !CanFileBeDependency( pPath )
				|| _used.Contains( pPath );
		}

		static bool CanFolderContainDependants(string pPath)
		{
			return !pPath.StartsWith( "." ) 
				&& pPath != "Editor";
		}

		static bool CanPathContainDependants(string pPath)
		{
			return !pPath.Contains( "/Editor/" );
		}

		static bool CanFileBeDependant( string pPath )
		{
			return !pPath.StartsWith( "." )
			    && !pPath.EndsWith( ".meta" );
		}

		static bool CanFileBeDependency( string pPath )
		{
			return !pPath.EndsWith( ".unity" );
		}
	}
}