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

using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Object = UnityEngine.Object;
using System.Reflection;

namespace Tenebrous.EditorEnhancements
{
	public class TeneProjectWindow : EditorEnhancement
	{
		private Dictionary<string, Color> _colorMap;

		// caches for various things
		private Dictionary<string, int> _fileCount = new Dictionary<string, int>();
		private Dictionary<string, int> _folderFileCount = new Dictionary<string, int>();
		private Dictionary<string, string> _tooltips = new Dictionary<string, string>();
		//private Dictionary<string, string> _specialPaths = new Dictionary<string, string>();
		private Dictionary<string, FileAttributes> _fileAttrs = new Dictionary<string, FileAttributes>();

        public enum ShowExtensions
        {
            Never,
            Always,
            OnlyWhenConflicts
        }
        
		// settings
        private ShowExtensions _setting_showExtensionsWhen;
        private bool _setting_showExtensionsFilename;
		private bool _setting_showFileCount;

		private bool _setting_showHoverPreview;
		private bool _setting_showHoverPreviewShift;
		private bool _setting_showHoverPreviewCtrl;
		private bool _setting_showHoverPreviewAlt;
		private bool _setting_showHoverTooltip;
        private bool _setting_showHoverTooltipShift;
        private bool _setting_showHoverTooltipCtrl;
        private bool _setting_showHoverTooltipAlt;

        //private bool _setting_showFoldersFirst;
		private bool _setting_useDependencyChecker;

		private string _lastGUID;
		private string _currentGUID;
		private int _updateThrottle;

		// mouse position recorded during the projectWindowItemOnGUI
		private Vector2 _mousePosition;

		// any version-specific hacks
		private bool _needHackScrollbarWidthForDrawing;

        private HashSet<string> _GUIDsToIgnore = new HashSet<string>();

        public override void OnEnable()
        {
	        EditorApplication.projectWindowItemOnGUI -= Draw;
	        EditorApplication.projectWindowItemOnGUI += Draw;

	        EditorApplication.update -= Update;
	        EditorApplication.update += Update;

            if( EditorGUIUtility.isProSkin )
                _colorMap = new Dictionary<string, Color>()
				{
					{"png", new Color(0.8f, 0.8f, 1.0f)},
					{"psd", new Color(0.5f, 0.8f, 1.0f)},
					{"tga", new Color(0.8f, 0.5f, 1.0f)},

					{"cs",  new Color(0.5f, 1.0f, 0.5f)},
					{"js",  new Color(0.8f, 1.0f, 0.3f)},
					{"boo", new Color(0.3f, 1.0f, 0.8f)},

					{"mat", new Color(1.0f, 0.8f, 0.8f)},
					{"shader", new Color(1.0f, 0.5f, 0.5f)},

					{"wav", new Color(0.8f, 0.4f, 1.0f)},
					{"mp3", new Color(0.8f, 0.4f, 1.0f)},
					{"ogg", new Color(0.8f, 0.4f, 1.0f)},
				};
            else
                _colorMap = new Dictionary<string, Color>()
				{
					{"png", new Color(0.0f, 0.0f, 1.0f)},
					{"psd", new Color(0.7f, 0.2f, 1.0f)},
					{"tga", new Color(0.2f, 0.7f, 1.0f)},

					{"cs",  new Color(0.0f, 0.5f, 0.0f)},
					{"js",  new Color(0.5f, 0.5f, 0.0f)},
					{"boo", new Color(0.0f, 0.5f, 0.5f)},

					{"mat", new Color(0.2f, 0.8f, 0.8f)},
					{"shader", new Color(1.0f, 0.5f, 0.5f)},

					{"wav", new Color(0.8f, 0.4f, 1.0f)},
					{"mp3", new Color(0.8f, 0.4f, 1.0f)},
					{"ogg", new Color(0.8f, 0.4f, 1.0f)},
				};

            //_specialPaths = new Dictionary<string, string>()
            //{
            //    {"WebPlayerTemplates", "WebPlayerTemplates - excluded from build"}
            //};

            _needHackScrollbarWidthForDrawing = Common.UnityVersion() < Common.UnityVersion( "4.0.0b8" );

            ReadSettings();

            if( File.Exists( Common.TempRecompilationList ) )
            {
                CheckScriptInfo( File.ReadAllText( Common.TempRecompilationList ) );
                File.Delete( Common.TempRecompilationList );
            }

            //SetProjectWindowFoldersFirst( _setting_showFoldersFirst );

            if( Common.ProjectWindow != null ) Common.ProjectWindow.Repaint();

			if( _setting_useDependencyChecker )
				DependencyChecker.Refresh();
        }//

        public override void OnDisable()
        {
            EditorApplication.projectWindowItemOnGUI -= Draw;
            EditorApplication.update -= Update;
            Common.ProjectWindow.Repaint();
        }

        public override string Name
        {
            get { return "Project Window"; }
        }

        public override string Prefix
        {
            get { return "TeneProjectWindow"; }
        }

		private void CheckScriptInfo( string pLines )
		{
			//string[] scripts = sLines.Split(new char[] {'\n'} ,StringSplitOptions.RemoveEmptyEntries);
			//foreach( string script in scripts )
			//{
			//	//Debug.Log(script);
			//}
		}

		public void RefreshDependencies()
		{
			if( _setting_useDependencyChecker )
				DependencyChecker.Refresh();
		}

		private void Update()
		{
			if (_setting_useDependencyChecker)
				if (DependencyChecker.Running)
					DependencyChecker.Continue();

			if( !_setting_showHoverPreview )
				return;

			if( _lastGUID == null && _currentGUID == null )
				return;

			if( _lastGUID != _currentGUID )
			{
				_lastGUID = _currentGUID;

				TeneEnhPreviewWindow.Update(
					Common.ProjectWindow.position, 
					_mousePosition,
					pGUID : _currentGUID
				);
			}
			else
			{
				_updateThrottle++;
				if( _updateThrottle > 20 )
				{
					_currentGUID = null;
					Common.ProjectWindow.Repaint();
					_updateThrottle = 0;
				}
			}
		}

	    private float _lastY;
        private void DrawingStarted()
        {
            _GUIDsToIgnore.Clear();
        }

		private void Draw( string pGUID, Rect pDrawingRect )
		{
            // called per-line in the project window

            // if we're drawing at a higher position from the last time, then
            // assume we're drawing from the top again and clear stuff we might
            // need to clear
            if( pDrawingRect.y < _lastY )
                DrawingStarted();

		    _lastY = pDrawingRect.y;


            // now process the asset

			string assetpath = AssetDatabase.GUIDToAssetPath( pGUID );

            if( assetpath.Length == 0 )
                return;

            string extension = Path.GetExtension( assetpath );
			string filename = Path.GetFileNameWithoutExtension( assetpath );
			bool isFolder = false;

			bool icons = pDrawingRect.height > 20;

			string path = Path.GetDirectoryName( assetpath );

#if UNITY_2018
			// Exclude PackageManager "Packages" folder
			if(path.Contains("Packages"))
				return;
#endif

			isFolder = (GetFileAttr(assetpath) & FileAttributes.Directory) != 0;

			if (_setting_useDependencyChecker && !isFolder && !DependencyChecker.IsUsed(assetpath) )
			{
				Color c = GUI.color;
				GUI.color = new Color(1,0,0,0.1f);
				Rect r = pDrawingRect;
				r.width += r.x;
				r.x = 0;
				GUI.DrawTexture(r, EditorGUIUtility.whiteTexture);
				GUI.color = c;
			}

			bool doPreview = Common.Modifier( _setting_showHoverPreview, _setting_showHoverPreviewShift, _setting_showHoverPreviewCtrl, _setting_showHoverPreviewAlt );
			bool doTooltip = Common.Modifier( _setting_showHoverTooltip, _setting_showHoverTooltipShift, _setting_showHoverTooltipCtrl, _setting_showHoverTooltipAlt );

			if( doTooltip )
			{
				string tooltip = GetTooltip(assetpath);
				if (tooltip.Length > 0)
					GUI.Label(pDrawingRect, new GUIContent(" ", tooltip));
			}

			if( !_setting_showFileCount && isFolder )
				return;

			_mousePosition = new Vector2(Event.current.mousePosition.x + Common.ProjectWindow.position.x, Event.current.mousePosition.y + Common.ProjectWindow.position.y );

#if UNITY_4_PLUS
            if ( Event.current.mousePosition.x < pDrawingRect.width - 16 )
#endif
			if( doPreview )
				if( pDrawingRect.Contains( Event.current.mousePosition ) )
					_currentGUID = pGUID;

            if( !isFolder )
                if (_setting_showExtensionsWhen == ShowExtensions.Never)
                    return;
                else if( _setting_showExtensionsWhen == ShowExtensions.OnlyWhenConflicts )
				    if( GetExtensionsCount( extension, filename, path ) <= 1 )
					    return;

            GUIStyle labelstyle;

            if( _setting_showExtensionsFilename )
            {
                labelstyle = icons ? new GUIStyle( EditorStyles.miniLabel ) : new GUIStyle( EditorStyles.label );
                Rect extRect = pDrawingRect;
                extRect.x += labelstyle.CalcSize( new GUIContent( filename ) ).x + 15;
                extRect.y++;

                if( !isFolder )
                {
                    if( !_GUIDsToIgnore.Contains( pGUID ) )
                    {
                        GUI.Label( extRect, extension );
                        _GUIDsToIgnore.Add( pGUID );
                    }
                }

                return;
            }

            Color labelColor = Color.grey;
            string drawextension = "";

            if( !isFolder )
            {
				if (extension != "")
				{
                	extension = extension.Substring( 1 );
                	drawextension = extension;
				}

                if( !_colorMap.TryGetValue( extension.ToLower(), out labelColor ) )
                    labelColor = Color.grey;
            }
            else
            {
                labelColor = new Color( 0.75f, 0.75f, 0.75f, 1.0f );
                int files = GetFolderFilesCount( assetpath );
                if( files == 0 )
                    return;
                drawextension = "(" + files + ")";
            }

            labelstyle = icons ? Common.ColorMiniLabel( labelColor ) : Common.ColorLabel( labelColor );
            Rect newRect = pDrawingRect;
            Vector2 labelSize = labelstyle.CalcSize( new GUIContent( drawextension ) );

            if( icons )
            {
                labelSize = labelstyle.CalcSize( new GUIContent( drawextension ) );
                newRect.x += newRect.width - labelSize.x;
                newRect.width = labelSize.x;
                newRect.height = labelSize.y;
            }
            else
            {
#if UNITY_4_PLUS
                newRect.width += pDrawingRect.x - (_needHackScrollbarWidthForDrawing ? 16 : 0);
                newRect.x = 0;
#else
                newRect.width += pDrawingRect.x;
                newRect.x = 0;
#endif
                newRect.x = newRect.width - labelSize.x;
                if( !isFolder )
                {
                    newRect.x -= 4;
					
					if (drawextension != "")
                    	drawextension = "." + drawextension;
                }

                labelSize = labelstyle.CalcSize( new GUIContent( drawextension ) );

                newRect.width = labelSize.x + 1;

                if( isFolder )
                {
                    newRect = pDrawingRect;
                    newRect.x += labelstyle.CalcSize( new GUIContent( filename ) ).x + 20;
                }
            }

            Color color = GUI.color;

            if( !isFolder || icons )
            {
                // fill background
                Color bgColor = Common.DefaultBackgroundColor;
				if (drawextension != "")
                	bgColor.a = 1;
				else
					bgColor.a = 0;
                GUI.color = bgColor;
                GUI.DrawTexture( newRect, EditorGUIUtility.whiteTexture );
            }

            GUI.color = labelColor;
            GUI.Label( newRect, drawextension, labelstyle );
            GUI.color = color;
		}

		private FileAttributes GetFileAttr( string assetpath )
		{
			FileAttributes attrs;

			if( _fileAttrs.TryGetValue( assetpath, out attrs ) )
				return ( attrs );

			string searchpath = Common.FullPath( assetpath );

			attrs = File.GetAttributes( searchpath );

			_fileAttrs[ assetpath ] = attrs;

			return ( attrs );
		}

		private string GetTooltip( string assetpath )
		{
			string tooltip;

			if( _tooltips.TryGetValue( assetpath, out tooltip ) )
				return( tooltip );

			Object asset = AssetDatabase.LoadAssetAtPath( assetpath, typeof( Object ) );

			tooltip = asset.GetPreviewInfo();
			while( tooltip.StartsWith( "\n" ) )
				tooltip = tooltip.Substring("\n".Length);

			//foreach( KeyValuePair<string,string> kvp in _specialPaths )
			//    if( System.Text.RegularExpressions.Regex.IsMatch(assetpath,kvp.Key) )
			//        tooltip += "\n" + kvp.Value;

			_tooltips[assetpath] = tooltip;
			
			return tooltip;
		}

		private int GetExtensionsCount( string extension, string filename, string path )
		{
			string searchpath = Common.FullPath( path );

			int files = 0;
			string pathnoext = path + Path.AltDirectorySeparatorChar + filename;

			if( !_fileCount.TryGetValue( pathnoext, out files ) )
			{
				files = 1;
				string[] otherFilenames = Directory.GetFiles( searchpath, filename + ".*" );
				foreach( string otherFilename in otherFilenames )
				{
					if( otherFilename.EndsWith( filename + extension ) )
						continue;

					if( otherFilename.EndsWith( ".meta" ) )
						continue;

					files++;
					break;
				}

				_fileCount[pathnoext] = files;
			}
			return files;
		}

		private int GetFolderFilesCount( string assetpath )
		{
			string searchpath = Common.FullPath( assetpath );
			int files;

			if( _folderFileCount.TryGetValue( assetpath, out files ) )
				return ( files );

			string[] otherFilenames = Directory.GetFiles( searchpath );
			files = 0;

			searchpath += Path.DirectorySeparatorChar + ".";

			foreach( string otherFilename in otherFilenames )
			{
				if( otherFilename.StartsWith( searchpath ) )
					continue;

				if( otherFilename.EndsWith( ".meta" ) )
					continue;

				files++;
			}

			_folderFileCount[ assetpath ] = files;

			return files;
		}

		public void ClearCache( string sAsset )
		{
			// remove cached count of number of files with alternate extensions
			_fileCount.Remove( Path.GetDirectoryName( sAsset ) 
							 + Path.AltDirectorySeparatorChar
							 + Path.GetFileNameWithoutExtension( sAsset ) );

			// remove cached tooltips for this path
			_tooltips.Remove( sAsset );

			// remove cached file attributes for this path
			_fileAttrs.Remove( sAsset );

			// removed cached folder file count for this path's folder
			_fileAttrs.Remove( Path.GetDirectoryName(sAsset) );
		}

        public void SetProjectWindowFoldersFirst( bool pValue )
        {
            // trying to get to:
            // projectWindow (UnityEditor.ProjectBrowser)
            //   .m_AssetTree (UnityEditor.TreeView field)
            //     .data (UnityEditor.AssetsTreeViewDataSource property)
            //       .foldersFirst (bool property)

            //try
            {
                // experimental, so at the moment don't catch any specific issues

                EditorWindow e = Common.ProjectWindow;

                if (e == null)
                    return;

                // get projectWindow.m_AssetTree
                Type projectWindowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.ProjectBrowser");
                FieldInfo projectWindow_AssetTree_Field = projectWindowType.GetField("m_AssetTree", BindingFlags.NonPublic | BindingFlags.Instance);
                object assetTree = projectWindow_AssetTree_Field.GetValue(e);

                // get projectWindow.m_AssetTree.data
                Type treeViewType = typeof(EditorWindow).Assembly.GetType("UnityEditor.TreeView");
                PropertyInfo treeView_Data_Property = treeViewType.GetProperty("data");
                MethodInfo treeView_ReloadData_Method = treeViewType.GetMethod("ReloadData");
                object data = treeView_Data_Property.GetValue(assetTree, new object[] { });

                // get projectWindow.m_AssetTree.data.foldersFirst
                Type assetsTreeViewDataSourceType = typeof(EditorWindow).Assembly.GetType("UnityEditor.AssetsTreeViewDataSource");
                PropertyInfo assetsTreeViewDS_FoldersFirst_Property = assetsTreeViewDataSourceType.GetProperty("foldersFirst", typeof(bool));

                // finally get the actual value and change it if required
                bool value = (bool)assetsTreeViewDS_FoldersFirst_Property.GetValue(data, new object[] { });

                if (value != pValue )
                {
                    assetsTreeViewDS_FoldersFirst_Property.SetValue(data, pValue, new object[] { });
                    treeView_ReloadData_Method.Invoke( assetTree, new object[] { } );
                }   
            }
            //catch( Exception e )
            //{
                //Debug.Log(e);
            //}
        }

		//////////////////////////////////////////////////////////////////////

		//		private static Vector2 _scroll;
		//		private static string _editingName = "";
		//		private static Color _editingColor;

		public override bool HasPreferences
		{
			get { return true; }
		}

		public override void DrawPreferences()
		{

            _setting_showExtensionsWhen = (ShowExtensions)EditorGUILayout.EnumPopup("Show extensions", (Enum)_setting_showExtensionsWhen);

            if( _setting_showExtensionsWhen != ShowExtensions.Never )
                _setting_showExtensionsFilename = EditorGUILayout.Toggle( "   as part of filename", _setting_showExtensionsFilename );

            //EditorGUILayout.BeginHorizontal();
            //EditorGUILayout.Space();
            //_setting_showFoldersFirst = GUILayout.Toggle(_setting_showFoldersFirst, "");
            //GUILayout.Label("Show folders first", GUILayout.Width(176));
            //GUILayout.FlexibleSpace();
            //EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
				EditorGUILayout.Space();
				_setting_showFileCount = GUILayout.Toggle( _setting_showFileCount, "" );
				GUILayout.Label( "Show folder file count", GUILayout.Width( 176 ) );
				GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
				EditorGUILayout.Space();
				_setting_showHoverPreview = GUILayout.Toggle( _setting_showHoverPreview, "" );
				GUILayout.Label( "Asset preview on hover", GUILayout.Width( 176 ) );

				if( _setting_showHoverPreview )
				{
					EditorGUILayout.Space();
					_setting_showHoverPreviewShift = GUILayout.Toggle( _setting_showHoverPreviewShift, "shift" );
					EditorGUILayout.Space();
					_setting_showHoverPreviewCtrl = GUILayout.Toggle( _setting_showHoverPreviewCtrl, "ctrl" );
					EditorGUILayout.Space();
					_setting_showHoverPreviewAlt = GUILayout.Toggle( _setting_showHoverPreviewAlt, "alt" );
				}

				GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
				EditorGUILayout.Space();
				_setting_showHoverTooltip = GUILayout.Toggle( _setting_showHoverTooltip, "" );
				GUILayout.Label( "Asset tooltip on hover", GUILayout.Width( 176 ) );

				if( _setting_showHoverTooltip )
				{
					EditorGUILayout.Space();
					_setting_showHoverTooltipShift = GUILayout.Toggle( _setting_showHoverTooltipShift, "shift" );
					EditorGUILayout.Space();
					_setting_showHoverTooltipCtrl = GUILayout.Toggle( _setting_showHoverTooltipCtrl, "ctrl" );
					EditorGUILayout.Space();
					_setting_showHoverTooltipAlt = GUILayout.Toggle( _setting_showHoverTooltipAlt, "alt" );
				}

				GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space();
				_setting_useDependencyChecker = GUILayout.Toggle(_setting_useDependencyChecker, "");
			GUILayout.Label("Highlight unused assets (experimental!)");
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();

			/*
						string removeExtension = null;
						string changeExtension = null;
						Color changeColor = Color.black;

						EditorGUILayout.Space();
						foreach (KeyValuePair<string, Color> ext in _colorMap)
						{
							EditorGUILayout.BeginHorizontal(GUILayout.Width(300));
							EditorGUILayout.SelectableLabel(ext.Key, GUILayout.Width(80), GUILayout.Height(16));

							Color c = EditorGUILayout.ColorField(ext.Value);
							if (c != ext.Value)
							{
								changeExtension = ext.Key;
								changeColor = c;
							}

							if (GUILayout.Button("del", GUILayout.Width(42)))
							{
								_editingName = ext.Key;
								_editingColor = ext.Value;
								removeExtension = ext.Key;
							}

							EditorGUILayout.EndHorizontal();
						}
						//GUILayout.Label("", GUILayout.Width(32));
						//EditorGUILayout.EndScrollView();

						if (removeExtension != null)
							_colorMap.Remove(removeExtension);

						if (changeExtension != null)
							_colorMap[changeExtension] = changeColor;

						EditorGUILayout.BeginHorizontal();
						GUILayout.Label("", GUILayout.Width(32));
						_editingName = EditorGUILayout.TextField(_editingName, GUILayout.Width(80));
						_editingColor = EditorGUILayout.ColorField(_editingColor);
						if (GUILayout.Button("add", GUILayout.Width(42)))
						{
						}
						EditorGUILayout.EndHorizontal();
			*/

			if( GUI.changed )
			{
				SaveSettings();
                //SetProjectWindowFoldersFirst( _setting_showFoldersFirst );
				Common.ProjectWindow.Repaint();
			}
		}

		private void ConvertOldSettings()
		{
			// update old prefs to new format, whenever we change how we use them

            if( EditorPrefs.HasKey( "TeneProjectWindow_All" ) )
            {
				EditorPrefs.SetInt( "TeneProjectWindow_WhenExtensions", 
								    (int)(EditorPrefs.GetBool( "TeneProjectWindow_All", true ) ? ShowExtensions.Always : ShowExtensions.OnlyWhenConflicts) );
                EditorPrefs.DeleteKey( "TeneProjectWindow_All" );
            }

			if( EditorPrefs.HasKey( "TeneProjectWindow_WhenExtensions" ) )
			{
				Main.Int [ this, "WhenExtensions"     ] = EditorPrefs.GetInt ( "TeneProjectWindow_WhenExtensions",      (int)Defaults.ProjectWindowExtensionsWhen );
				Main.Bool[ this, "FileCount"          ] = EditorPrefs.GetBool( "TeneProjectWindow_FileCount",           Defaults.ProjectWindowFileCount );
				Main.Bool[ this, "PreviewOnHover"     ] = EditorPrefs.GetBool( "TeneProjectWindow_PreviewOnHover",      Defaults.ProjectWindowHoverPreview );
				Main.Bool[ this, "PreviewOnHoverShift"] = EditorPrefs.GetBool( "TeneProjectWindow_PreviewOnHoverShift", Defaults.ProjectWindowHoverPreviewShift );
				Main.Bool[ this, "PreviewOnHoverCtrl" ] = EditorPrefs.GetBool( "TeneProjectWindow_PreviewOnHoverCtrl",  Defaults.ProjectWindowHoverPreviewCtrl );
				Main.Bool[ this, "PreviewOnHoverAlt"  ] = EditorPrefs.GetBool( "TeneProjectWindow_PreviewOnHoverAlt",   Defaults.ProjectWindowHoverPreviewAlt );
				Main.Bool[ this, "HoverTooltip"       ] = EditorPrefs.GetBool( "TeneProjectWindow_HoverTooltip",        Defaults.ProjectWindowHoverTooltip );
				Main.Bool[ this, "HoverTooltipShift"  ] = EditorPrefs.GetBool( "TeneProjectWindow_HoverTooltipShift",   Defaults.ProjectWindowHoverTooltipShift );
				Main.Bool[ this, "HoverTooltipCtrl"   ] = EditorPrefs.GetBool( "TeneProjectWindow_HoverTooltipCtrl",    Defaults.ProjectWindowHoverTooltipCtrl );
				Main.Bool[ this, "HoverTooltipAlt"    ] = EditorPrefs.GetBool( "TeneProjectWindow_HoverTooltipAlt",     Defaults.ProjectWindowHoverTooltipAlt );
				EditorPrefs.DeleteKey( "TeneProjectWindow_WhenExtensions" );
			}
		}

		private void ReadSettings()
		{
			//string colourinfo;
			ConvertOldSettings();

			_setting_showExtensionsWhen     = (ShowExtensions)Main.Int[this, "WhenExtensions", (int)Defaults.ProjectWindowExtensionsWhen];
            _setting_showExtensionsFilename = Main.Bool[this, "ShowExtensionsFilename", Defaults.ProjectWindowExtensionsFilename];
			_setting_showFileCount          = Main.Bool[this, "FileCount",           Defaults.ProjectWindowFileCount         ];

			_setting_showHoverPreview       = Main.Bool[this, "PreviewOnHover",      Defaults.ProjectWindowHoverPreview      ];
            _setting_showHoverPreviewShift  = Main.Bool[this, "PreviewOnHoverShift", Defaults.ProjectWindowHoverPreviewShift ];
            _setting_showHoverPreviewCtrl   = Main.Bool[this, "PreviewOnHoverCtrl",  Defaults.ProjectWindowHoverPreviewCtrl  ];
            _setting_showHoverPreviewAlt    = Main.Bool[this, "PreviewOnHoverAlt",   Defaults.ProjectWindowHoverPreviewAlt   ];

            _setting_showHoverTooltip       = Main.Bool[this, "HoverTooltip",        Defaults.ProjectWindowHoverTooltip      ];
            _setting_showHoverTooltipShift  = Main.Bool[this, "HoverTooltipShift",   Defaults.ProjectWindowHoverTooltipShift ];
            _setting_showHoverTooltipCtrl   = Main.Bool[this, "HoverTooltipCtrl",    Defaults.ProjectWindowHoverTooltipCtrl  ];
            _setting_showHoverTooltipAlt    = Main.Bool[this, "HoverTooltipAlt",     Defaults.ProjectWindowHoverTooltipAlt   ];

            //_setting_showFoldersFirst      = Main.Bool[this, "ShowFoldersFirst",    Application.platform != RuntimePlatform.OSXEditor];

			_setting_useDependencyChecker  = Main.Bool[this, "DependencyChecker",    Defaults.ProjectWindowUseDependencyChceker ];

			//string colormap = Common.GetLongPref("TeneProjectWindow_ColorMap");
        }

		private void SaveSettings()
		{
			Main.Int[  this, "WhenExtensions"         ] = (int)_setting_showExtensionsWhen;
            Main.Bool[ this, "ShowExtensionsFilename" ] = _setting_showExtensionsFilename;

			string colormap = "";
			foreach( KeyValuePair<string, Color> entry in _colorMap )
				colormap += entry.Key + ":" + Common.ColorToString( entry.Value ) + "|";

			Common.SetLongPref( "TeneProjectWindow_ColorMap", colormap );

			Main.Bool[ this, "FileCount"           ] = _setting_showFileCount;
			Main.Bool[ this, "PreviewOnHover"      ] = _setting_showHoverPreview;
			Main.Bool[ this, "PreviewOnHoverShift" ] = _setting_showHoverPreviewShift;
			Main.Bool[ this, "PreviewOnHoverCtrl"  ] = _setting_showHoverPreviewCtrl;
			Main.Bool[ this, "PreviewOnHoverAlt"   ] = _setting_showHoverPreviewAlt;
			Main.Bool[ this, "HoverTooltip"        ] = _setting_showHoverTooltip;
            Main.Bool[ this, "HoverTooltipShift"   ] = _setting_showHoverTooltipShift;
            Main.Bool[ this, "HoverTooltipCtrl"    ] = _setting_showHoverTooltipCtrl;
            Main.Bool[ this, "HoverTooltipAlt"     ] = _setting_showHoverTooltipAlt;
            //Main.Bool[ this, "ShowFoldersFirst"    ] = _setting_showFoldersFirst;

            Main.Bool[ this, "DependencyChecker"   ] = _setting_useDependencyChecker;
		}
	}

    public class ProjectWindowExtensionsClass : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets( string[] pImported, string[] pDeleted, string[] pMoved, string[] pMoveFrom )
        {
            TeneProjectWindow proj = Main.Enhancement<TeneProjectWindow>();
            if (proj == null )
                return;

            string compilationList = "";
            foreach( string file in pImported )
            {
                string lower = file.ToLower();
                if( lower.EndsWith( ".cs" ) || lower.EndsWith( ".boo" ) || lower.EndsWith( ".js" ) )
                    compilationList += file + "\n";

                proj.ClearCache( file );
            }

            if( compilationList.Length > 0 )
                File.WriteAllText( Common.TempRecompilationList, compilationList );

            foreach( string file in pDeleted )
                proj.ClearCache( file );

            foreach( string file in pMoved )
                proj.ClearCache( file );

            foreach( string file in pMoveFrom )
                proj.ClearCache( file );
		}
    }


#if UNITY_5 || UNITY_5_3_OR_NEWER
    public class ProjectWindowAMP : UnityEditor.AssetModificationProcessor
#else
	public class ProjectWindowAMP : AssetModificationProcessor
#endif
	{
		private static void OnWillSaveAssets(string[] pPaths)
		{
			TeneProjectWindow proj = Main.Enhancement<TeneProjectWindow>();
			if (proj != null)
				proj.RefreshDependencies();
		}
	}
}