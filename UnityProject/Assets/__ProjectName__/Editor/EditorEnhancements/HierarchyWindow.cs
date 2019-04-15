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

#if UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_5 || UNITY_5_3_OR_NEWER
#define UNITY_4_3_PLUS
#endif

using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace Tenebrous.EditorEnhancements
{

    public class TeneHierarchyWindow : EditorEnhancement
    {
        private Dictionary<string, Color> _colorMap;
        private string _basePath;

        private bool _showAll;
        private int _shownLayers;

        private bool _setting_showHoverPreview;
        private bool _setting_showHoverPreviewShift;
        private bool _setting_showHoverPreviewCtrl;
        private bool _setting_showHoverPreviewAlt;

        private bool _setting_showHoverTooltip;
        private bool _setting_showHoverTooltipShift;
        private bool _setting_showHoverTooltipCtrl;
        private bool _setting_showHoverTooltipAlt;

        private bool _setting_showComponents;
        private bool _setting_showComponentsShift;
        private bool _setting_showComponentsCtrl;
        private bool _setting_showComponentsAlt;

        private bool _setting_showLock;
        private bool _setting_showLockLocked;
        private bool _setting_showLockShift;
        private bool _setting_showLockCtrl;
        private bool _setting_showLockAlt;
        private bool _setting_disallowSelection;

        private bool _setting_showEnabled;
        private bool _setting_showEnabledShift;
        private bool _setting_showEnabledCtrl;
        private bool _setting_showEnabledAlt;

        private bool _setting_showHoverDropWindow;

        private Object _hoverObject;
        private Object _lastHoverObject;

        private int _updateThrottle;
        private Vector2 _mousePosition;

        private GameObject _draggingHeldOver;
        private System.DateTime _draggingHeldStart;
        private bool _draggingShownQuickInspector;

        private Dictionary<Object, string> _tooltips = new Dictionary<Object, string>();

        private bool _wasDragging = false;

        public TeneHierarchyWindow()
        {
            ReadSettings();
        }

        public override void OnEnable()
        {
            EditorApplication.hierarchyWindowItemOnGUI += Draw;
            EditorApplication.update += Update;
            SceneView.onSceneGUIDelegate += Updated;
            EditorApplication.hierarchyChanged += ClearTooltipCache;
            EditorApplication.modifierKeysChanged += ModifierKeysChanged;
            if( Common.HierarchyWindow != null ) Common.HierarchyWindow.Repaint();
        }

        public override void OnDisable()
        {
            EditorApplication.hierarchyWindowItemOnGUI -= Draw;
            EditorApplication.update -= Update;
            SceneView.onSceneGUIDelegate -= Updated;
            EditorApplication.hierarchyChanged -= ClearTooltipCache;
            EditorApplication.modifierKeysChanged -= ModifierKeysChanged;
            Common.HierarchyWindow.Repaint();
        }

        public override string Name
        {
            get
            {
                return "Hierarchy Window";
            }
        }

        public override string Prefix
        {
            get
            {
                return "TeneHierarchyWindow";
            }
        }

        private void ModifierKeysChanged()
        {
            Common.HierarchyWindow.Repaint();
        }

        private void Update()
        {
            bool nowDragging = DragAndDrop.objectReferences.Length == 1;

            if (_wasDragging != nowDragging)
            {
                // detect when dragging state changes, so we force a refresh of the hierarchy
                _wasDragging = nowDragging;

                if( !nowDragging )
                {
                    _draggingHeldOver = null;
                    _draggingShownQuickInspector = false;
                    TeneDropTarget.Hide();
                }

                // doesn't appear to refresh when starting to drag stuff
                // but works ok when finishing
                Common.HierarchyWindow.Repaint();
            }

            if( _lastHoverObject != null || _hoverObject != null )
            {

                if( _lastHoverObject != _hoverObject )
                {
                    // don't currently support plain old gameobjects
                    if( _hoverObject is GameObject )
                        if( PrefabUtility.GetCorrespondingObjectFromSource( _hoverObject ) == null )
                            _hoverObject = null;
                }

                if( _lastHoverObject != _hoverObject )
                {
                    _lastHoverObject = _hoverObject;

                    TeneEnhPreviewWindow.Update(
                        Common.HierarchyWindow.position,
                        _mousePosition,
                        pAsset : _hoverObject
                        );
                }
                else
                {
                    _updateThrottle++;
                    if( _updateThrottle > 20 )
                    {
                        _hoverObject = null;
                        Common.HierarchyWindow.Repaint();
                        _updateThrottle = 0;
                    }
                }
            }

            if( _draggingHeldOver != null )
                if( !_draggingShownQuickInspector )
                    if( ( System.DateTime.Now - _draggingHeldStart ).TotalSeconds >= 0.5f )
                    {
                        // user held mouse over a game object, so we can show the
                        // quick-drop popup window

                        TeneDropTarget.Update(
                            Common.HierarchyWindow.position,
                            _mousePosition,
                            _draggingHeldOver
                        );

                        _draggingShownQuickInspector = true;
                    }
        }

        void Updated( SceneView pScene )
        {
            // triggered when the user changes anything
            // e.g. manually enables/disables components, etc

            if( Event.current.type == EventType.Repaint )
                Common.HierarchyWindow.Repaint();
        }

        private void Draw( int pInstanceID, Rect pDrawingRect )
        {
            // called per-line in the hierarchy window

            GameObject gameObject = EditorUtility.InstanceIDToObject( pInstanceID ) as GameObject;
            if( gameObject == null )
                return;

            bool currentLock = ( gameObject.hideFlags & HideFlags.NotEditable ) != 0;

            bool doPreview    = Common.Modifier( _setting_showHoverPreview, _setting_showHoverPreviewShift, _setting_showHoverPreviewCtrl, _setting_showHoverPreviewAlt );
            bool doTooltip    = Common.Modifier( _setting_showHoverTooltip, _setting_showHoverTooltipShift, _setting_showHoverTooltipCtrl, _setting_showHoverTooltipAlt );
            bool doComponents = Common.Modifier( _setting_showComponents,   _setting_showComponentsShift,   _setting_showComponentsCtrl,   _setting_showComponentsAlt   );
            bool doEnabled    = Common.Modifier( _setting_showEnabled,      _setting_showEnabledShift,      _setting_showEnabledCtrl,      _setting_showEnabledAlt);
            bool doLockIcon   = Common.Modifier( _setting_showLock,         _setting_showLockShift,         _setting_showLockCtrl,         _setting_showLockAlt         );
            doLockIcon |= _setting_showLock && _setting_showLockLocked && currentLock;

            Object dragging = DragAndDrop.objectReferences.Length == 1 ? DragAndDrop.objectReferences[0] : null;

            string tooltip = "";

            Color originalColor = GUI.color;

            if( doEnabled )
            {
                Rect r = pDrawingRect;
                r.x = r.x + r.width - 14;
                r.width = 12;

                bool bNewActive = GUI.Toggle( r, gameObject.activeSelf, "");
                if( bNewActive != gameObject.activeSelf )
                    gameObject.SetActive( bNewActive );

                pDrawingRect.width -= 14;
            }

            if( doLockIcon )
            {
                Rect r = pDrawingRect;
                r.x = r.x + r.width - 14;
                r.width = 12;

                GUI.color = currentLock ? new Color(1,1,1,1) : new Color(1,1,1,0.4f);

                bool newLock = GUI.Toggle( r, currentLock, "", (GUIStyle) "IN LockButton" );
                if( newLock != currentLock )
                {
                    if( newLock )
                        UpdateLock( gameObject, true );
                    else
                        UpdateLock( gameObject, false );
                }

                GUI.color = originalColor;

                pDrawingRect.width -= 14;
            }

            float width = EditorStyles.label.CalcSize( new GUIContent( gameObject.name ) ).x;

            if( ( ( 1 << gameObject.layer ) & Tools.visibleLayers ) == 0 )
            {
                Rect labelRect = pDrawingRect;
                labelRect.width = width;
                labelRect.x -= 2;
                labelRect.y -= 4;
                GUI.Label( labelRect, "".PadRight( gameObject.name.Length, '_' ) );
            }

            if( gameObject == dragging && _draggingShownQuickInspector )
            {
                Rect labelRect = pDrawingRect;

                labelRect.width = width;
                labelRect.x -= 2;
                labelRect.y += 1;

                GUI.color = Color.red;
                GUI.DrawTexture(new Rect(labelRect.x, labelRect.y, labelRect.width, 1), EditorGUIUtility.whiteTexture);
                GUI.DrawTexture(new Rect(labelRect.x, labelRect.y, 1, labelRect.height), EditorGUIUtility.whiteTexture);
                GUI.DrawTexture(new Rect(labelRect.x, labelRect.yMax, labelRect.width, 1), EditorGUIUtility.whiteTexture);
                GUI.DrawTexture(new Rect(labelRect.xMax, labelRect.y, 1, labelRect.height), EditorGUIUtility.whiteTexture);
            }

            if( doTooltip )
            {
                tooltip = GetTooltip( gameObject );
                if( tooltip.Length > 0 )
                    GUI.Label( pDrawingRect, new GUIContent( " ", tooltip ) );
            }

            if( doComponents )
            {
                Rect iconRect = pDrawingRect;
                iconRect.x = pDrawingRect.x + pDrawingRect.width - 16;
                iconRect.y--;
                iconRect.width = 16;
                iconRect.height = 16;

                _mousePosition = new Vector2( Event.current.mousePosition.x + Common.HierarchyWindow.position.x, Event.current.mousePosition.y + Common.HierarchyWindow.position.y );

                bool mouseIn = pDrawingRect.Contains( Event.current.mousePosition );

                if( doPreview && mouseIn )
                    _hoverObject = gameObject;

                if( DragAndDrop.objectReferences.Length == 1 && _setting_showHoverDropWindow && mouseIn )
                {
                    if( _draggingHeldOver == null )
                        _draggingHeldStart = System.DateTime.Now;

                    if( _draggingHeldOver != gameObject )
                        _draggingShownQuickInspector = false;

                    _draggingHeldOver = gameObject;
                }

                bool suitableDrop = false;
                bool drawnEtc = false;
                string tempNote = "";

                foreach( Component c in gameObject.GetComponents<Component>() )
                {
                    if( c is Transform )
                        continue;

                    if( c != null && !suitableDrop && dragging != null )
                    {
                        Type type = c.GetType();
                        foreach( FieldInfo f in TeneDropTarget.FieldsFor( type ) )
                            if (TeneDropTarget.IsCompatibleField(f, dragging, ref tempNote))
                            {
                                suitableDrop = true;
                                break;
                            }
                    }

                    if( c == null )
                    {
                        Rect rectX = new Rect( iconRect.x + 4, iconRect.y + 1, 14, iconRect.height );
                        GUI.color = new Color( 1.0f, 0.35f, 0.35f, 1.0f );
                        GUI.Label( rectX, new GUIContent( "X", "Missing Script" ), Common.ColorLabel( new Color( 1.0f, 0.35f, 0.35f, 1.0f ) ) );
                        iconRect.x -= 9;

                        if( rectX.Contains( Event.current.mousePosition ) )
                            _hoverObject = null;

                        continue;
                    }

                    GUI.color = c.GetEnabled() ? Color.white : new Color( 0.5f, 0.5f, 0.5f, 0.5f );

                    if( iconRect.x < pDrawingRect.x + width )
                    {
                        if( !drawnEtc )
                        {
                            GUI.Label( iconRect, " .." );
                            drawnEtc = true;
                        }
                        continue;
                    }

                    if( doTooltip )
                        tooltip = GetTooltip( c );

                    Texture iconTexture = null;

                    if( c is MonoBehaviour )
                    {
                        MonoScript ms = MonoScript.FromMonoBehaviour( c as MonoBehaviour );
                        iconTexture = AssetDatabase.GetCachedIcon( AssetDatabase.GetAssetPath( ms ) );
                    }

                    if( iconTexture == null )
                        iconTexture = Common.GetMiniThumbnail( c );

                    if( iconTexture != null )
                    {
                        if( doPreview )
                            if( iconRect.Contains( Event.current.mousePosition ) )
                                _hoverObject = c;

                        GUI.DrawTexture( iconRect, iconTexture, ScaleMode.ScaleToFit );
                        if( GUI.Button( iconRect, new GUIContent( "", tooltip ), EditorStyles.label ) )
                        {
                            c.SetEnabled( !c.GetEnabled() );
                            Common.HierarchyWindow.Repaint();
                            return;
                        }
                        iconRect.x -= iconRect.width;
                    }
                }

                if( suitableDrop )
                {
                    Rect labelRect = pDrawingRect;

                    labelRect.width = width;
                    labelRect.x -= 2;
                    labelRect.y += 1;

                    GUI.color = Color.white;
                    GUI.DrawTexture( new Rect( labelRect.x, labelRect.y, labelRect.width, 1 ), EditorGUIUtility.whiteTexture );
                    GUI.DrawTexture( new Rect( labelRect.x, labelRect.y, 1, labelRect.height ), EditorGUIUtility.whiteTexture );
                    GUI.DrawTexture( new Rect( labelRect.x, labelRect.yMax, labelRect.width, 1 ), EditorGUIUtility.whiteTexture );
                    GUI.DrawTexture( new Rect( labelRect.xMax, labelRect.y, 1, labelRect.height ), EditorGUIUtility.whiteTexture );

                    if( mouseIn )
                        DragAndDrop.visualMode = DragAndDropVisualMode.Generic;

                    if( _draggingHeldOver == gameObject )
                        GUI.DrawTexture(new Rect(0, labelRect.y + labelRect.height / 2.0f, labelRect.x, 1), EditorGUIUtility.whiteTexture);
                }
            }
    
            GUI.color = originalColor;
        }

        private void UpdateLock( GameObject pObject, bool pLocked )
        {
            if( pLocked )
            {
                pObject.hideFlags |= HideFlags.NotEditable;

                if( _setting_disallowSelection )
                    foreach( Component c in pObject.GetComponents( typeof( Component) ) )
                        if( !( c is Transform ) )
                            c.hideFlags |= HideFlags.NotEditable | HideFlags.HideInHierarchy;
            }
            else
            {
                pObject.hideFlags &= ~HideFlags.NotEditable;

                if( _setting_disallowSelection )
                    foreach( Component c in pObject.GetComponents( typeof( Component ) ) )
                        if( !(c is Transform) )
                            c.hideFlags &= ~(HideFlags.NotEditable | HideFlags.HideInHierarchy);
            }

            EditorUtility.SetDirty( pObject );
        }

        private string GetTooltip( UnityEngine.Object pObject )
        {
            string tooltip;

            if( _tooltips.TryGetValue( pObject, out tooltip ) )
                return ( tooltip );

            tooltip = pObject.GetPreviewInfo();

            _tooltips[pObject] = tooltip;

            return tooltip;
        }
        private void ClearTooltipCache()
        {
            _tooltips.Clear();
        }

        //////////////////////////////////////////////////////////////////////

        public override bool HasPreferences
        {
            get { return true; }
        }

        public override void DrawPreferences()
        {
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
                _setting_showComponents = GUILayout.Toggle( _setting_showComponents, "" );
                GUILayout.Label( "Show component icons", GUILayout.Width( 176 ) );

                if( _setting_showComponents )
                {
                    EditorGUILayout.Space();
                    _setting_showComponentsShift = GUILayout.Toggle( _setting_showComponentsShift, "shift" );
                    EditorGUILayout.Space();
                    _setting_showComponentsCtrl = GUILayout.Toggle( _setting_showComponentsCtrl, "ctrl" );
                    EditorGUILayout.Space();
                    _setting_showComponentsAlt = GUILayout.Toggle( _setting_showComponentsAlt, "alt" );
                }

                GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                _setting_showLock = GUILayout.Toggle( _setting_showLock, "" );
                GUILayout.Label( "Show lock icon", GUILayout.Width( 110 ) );

                if( _setting_showLock )
                {
                    EditorGUILayout.Space();
                    _setting_showLockLocked = GUILayout.Toggle( _setting_showLockLocked, "locked" );
                    EditorGUILayout.Space();
                    _setting_showLockShift = GUILayout.Toggle( _setting_showLockShift, "shift" );
                    EditorGUILayout.Space();
                    _setting_showLockCtrl = GUILayout.Toggle( _setting_showLockCtrl, "ctrl" );
                    EditorGUILayout.Space();
                    _setting_showLockAlt = GUILayout.Toggle( _setting_showLockAlt, "alt" );
                }

                GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                _setting_disallowSelection = GUILayout.Toggle( _setting_disallowSelection, "" );
                GUILayout.Label( "Locking an object also prevents it's selection" );
                GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                _setting_showHoverDropWindow = GUILayout.Toggle( _setting_showHoverDropWindow, "" );
                GUILayout.Label( "Quick-drop window", GUILayout.Width( 176 ) );
                GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            if( GUI.changed )
            {
                SaveSettings();
                Common.HierarchyWindow.Repaint();
            }
        }

        private void ConvertOldSettings()
        {
            // update old prefs to new format, whenever we change how we use them

            if( EditorPrefs.HasKey( "TeneHierarchyWindow_PreviewOnHover" ) )
            {
                Main.Bool[ this, "PreviewOnHover"      ] = EditorPrefs.GetBool( "TeneHierarchyWindow_PreviewOnHover",      Defaults.HierarchyWindowHoverPreview );
                Main.Bool[ this, "PreviewOnHoverShift" ] = EditorPrefs.GetBool( "TeneHierarchyWindow_PreviewOnHoverShift", Defaults.HierarchyWindowHoverPreviewShift );
                Main.Bool[ this, "PreviewOnHoverCtrl"  ] = EditorPrefs.GetBool( "TeneHierarchyWindow_PreviewOnHoverCtrl",  Defaults.HierarchyWindowHoverPreviewCtrl );
                Main.Bool[ this, "PreviewOnHoverAlt"   ] = EditorPrefs.GetBool( "TeneHierarchyWindow_PreviewOnHoverAlt",   Defaults.HierarchyWindowHoverPreviewAlt );

                Main.Bool[ this, "HoverTooltip"        ] = EditorPrefs.GetBool( "TeneHierarchyWindow_HoverTooltip",        Defaults.HierarchyWindowHoverTooltip );
                Main.Bool[ this, "HoverTooltipShift"   ] = EditorPrefs.GetBool( "TeneHierarchyWindow_HoverTooltipShift",   Defaults.HierarchyWindowHoverTooltipShift );
                Main.Bool[ this, "HoverTooltipCtrl"    ] = EditorPrefs.GetBool( "TeneHierarchyWindow_HoverTooltipCtrl",    Defaults.HierarchyWindowHoverTooltipCtrl );
                Main.Bool[ this, "HoverTooltipAlt"     ] = EditorPrefs.GetBool( "TeneHierarchyWindow_HoverTooltipAlt",     Defaults.HierarchyWindowHoverTooltipAlt );

                Main.Bool[ this, "Components"          ] = EditorPrefs.GetBool( "TeneHierarchyWindow_Components",          Defaults.HierarchyWindowComponents );
                Main.Bool[ this, "ComponentsShift"     ] = EditorPrefs.GetBool( "TeneHierarchyWindow_ComponentsShift",     Defaults.HierarchyWindowComponentsShift );
                Main.Bool[ this, "ComponentsCtrl"      ] = EditorPrefs.GetBool( "TeneHierarchyWindow_ComponentsCtrl",      Defaults.HierarchyWindowComponentsCtrl );
                Main.Bool[ this, "ComponentsAlt"       ] = EditorPrefs.GetBool( "TeneHierarchyWindow_ComponentsAlt",       Defaults.HierarchyWindowComponentsAlt );

                Main.Bool[ this, "Lock"                ] = EditorPrefs.GetBool( "TeneHierarchyWindow_Lock",                Defaults.HierarchyWindowLock );
                Main.Bool[ this, "LockLocked"          ] = EditorPrefs.GetBool( "TeneHierarchyWindow_LockLocked",          Defaults.HierarchyWindowLockLocked );
                Main.Bool[ this, "LockShift"           ] = EditorPrefs.GetBool( "TeneHierarchyWindow_LockShift",           Defaults.HierarchyWindowLockShift );
                Main.Bool[ this, "LockCtrl"            ] = EditorPrefs.GetBool( "TeneHierarchyWindow_LockCtrl",            Defaults.HierarchyWindowLockCtrl );
                Main.Bool[ this, "LockAlt"             ] = EditorPrefs.GetBool( "TeneHierarchyWindow_LockAlt",             Defaults.HierarchyWindowLockAlt );

                Main.Bool[ this, "HoverDropWindow"     ] = EditorPrefs.GetBool( "TeneHierarchyWindow_HoverDropWindow",     Defaults.HierarchyWindowHoverDropWindow );

                EditorPrefs.DeleteKey( "TeneHierarchyWindow_PreviewOnHover" );
            }
        }

        private void ReadSettings()
        {
            ConvertOldSettings();

            _setting_showHoverPreview      = Main.Bool[ this, "PreviewOnHover",      Defaults.HierarchyWindowHoverPreview ];
            _setting_showHoverPreviewShift = Main.Bool[ this, "PreviewOnHoverShift", Defaults.HierarchyWindowHoverPreviewShift ];
            _setting_showHoverPreviewCtrl  = Main.Bool[ this, "PreviewOnHoverCtrl",  Defaults.HierarchyWindowHoverPreviewCtrl ];
            _setting_showHoverPreviewAlt   = Main.Bool[ this, "PreviewOnHoverAlt",   Defaults.HierarchyWindowHoverPreviewAlt ];

            _setting_showHoverTooltip      = Main.Bool[ this, "HoverTooltip",        Defaults.HierarchyWindowHoverTooltip ];
            _setting_showHoverTooltipShift = Main.Bool[ this, "HoverTooltipShift",   Defaults.HierarchyWindowHoverTooltipShift ];
            _setting_showHoverTooltipCtrl  = Main.Bool[ this, "HoverTooltipCtrl",    Defaults.HierarchyWindowHoverTooltipCtrl ];
            _setting_showHoverTooltipAlt   = Main.Bool[ this, "HoverTooltipAlt",     Defaults.HierarchyWindowHoverTooltipAlt ];

            _setting_showComponents        = Main.Bool[ this, "Components",          Defaults.HierarchyWindowComponents ];
            _setting_showComponentsShift   = Main.Bool[ this, "ComponentsShift",     Defaults.HierarchyWindowComponentsShift ];
            _setting_showComponentsCtrl    = Main.Bool[ this, "ComponentsCtrl",      Defaults.HierarchyWindowComponentsCtrl ];
            _setting_showComponentsAlt     = Main.Bool[ this, "ComponentsAlt",       Defaults.HierarchyWindowComponentsAlt ];

            _setting_showLock              = Main.Bool[ this, "Lock",                Defaults.HierarchyWindowLock ];
            _setting_showLockLocked        = Main.Bool[ this, "LockLocked",          Defaults.HierarchyWindowLockLocked ];
            _setting_showLockShift         = Main.Bool[ this, "LockShift",           Defaults.HierarchyWindowLockShift ];
            _setting_showLockCtrl          = Main.Bool[ this, "LockCtrl",            Defaults.HierarchyWindowLockCtrl ];
            _setting_showLockAlt           = Main.Bool[ this, "LockAlt",             Defaults.HierarchyWindowLockAlt ];
            _setting_disallowSelection     = Main.Bool[ this, "LockUnselectable",    Defaults.HierarchyWindowLockUnselectable];

            _setting_showEnabled           = Main.Bool[ this, "Enabled",             Defaults.HierarchyWindowEnabled ];
            _setting_showEnabledShift      = Main.Bool[ this, "EnabledShift",        Defaults.HierarchyWindowEnabledShift ];
            _setting_showEnabledCtrl       = Main.Bool[ this, "EnabledCtrl",         Defaults.HierarchyWindowEnabledCtrl ];
            _setting_showEnabledAlt        = Main.Bool[ this, "EnabledAlt",          Defaults.HierarchyWindowEnabledAlt ];

            _setting_showHoverDropWindow   = Main.Bool[ this, "HoverDropWindow",     Defaults.HierarchyWindowHoverDropWindow ];
        }

        private void SaveSettings()
        {
            Main.Bool[ this, "PreviewOnHover"      ] = _setting_showHoverPreview;
            Main.Bool[ this, "PreviewOnHoverShift" ] = _setting_showHoverPreviewShift;
            Main.Bool[ this, "PreviewOnHoverCtrl"  ] = _setting_showHoverPreviewCtrl;
            Main.Bool[ this, "PreviewOnHoverAlt"   ] = _setting_showHoverPreviewAlt;

            Main.Bool[ this, "HoverTooltip"        ] = _setting_showHoverTooltip;
            Main.Bool[ this, "HoverTooltipShift"   ] = _setting_showHoverTooltipShift;
            Main.Bool[ this, "HoverTooltipCtrl"    ] = _setting_showHoverTooltipCtrl;
            Main.Bool[ this, "HoverTooltipAlt"     ] = _setting_showHoverTooltipAlt;
    
            Main.Bool[ this, "Components"          ] = _setting_showComponents;
            Main.Bool[ this, "ComponentsShift"     ] = _setting_showComponentsShift;
            Main.Bool[ this, "ComponentsCtrl"      ] = _setting_showComponentsCtrl;
            Main.Bool[ this, "ComponentsAlt"       ] = _setting_showComponentsAlt;
            
            Main.Bool[ this, "Lock"                ] = _setting_showLock;     
            Main.Bool[ this, "LockLocked"          ] = _setting_showLockLocked;     
            Main.Bool[ this, "LockShift"           ] = _setting_showLockShift;
            Main.Bool[ this, "LockCtrl"            ] = _setting_showLockCtrl;
            Main.Bool[ this, "LockAlt"             ] = _setting_showLockAlt;
            Main.Bool[ this, "LockUnselectable"    ] = _setting_disallowSelection;

            Main.Bool[ this, "Enabled"             ] = _setting_showEnabled;     
            Main.Bool[ this, "EnabledShift"        ] = _setting_showEnabledShift;
            Main.Bool[ this, "EnabledCtrl"         ] = _setting_showEnabledCtrl;
            Main.Bool[ this, "EnabledAlt"          ] = _setting_showEnabledAlt;     

            Main.Bool[ this, "HoverDropWindow"     ] = _setting_showHoverDropWindow;
        }
    }

    public static class ComponentExtensions
    {
        public static bool GetEnabled( this Component pComponent )
        {
            if( pComponent == null )
                return ( true );

            PropertyInfo p = pComponent.GetType().GetProperty( "enabled", typeof( bool ) );

            if( p != null )
                return ( (bool)p.GetValue( pComponent, null ) );

            return ( true );
        }
        public static void SetEnabled( this Component pComponent, bool pNewValue )
        {
            if( pComponent == null )
                return;

#if UNITY_4_3_PLUS
            Undo.RecordObject(pComponent, pNewValue ? "Enable Component" : "Disable Component");
#else
            Undo.RegisterUndo( pComponent, pNewValue ? "Enable Component" : "Disable Component" );
#endif

            PropertyInfo p = pComponent.GetType().GetProperty( "enabled", typeof( bool ) );

            if( p != null )
            {
                p.SetValue( pComponent, pNewValue, null );
                EditorUtility.SetDirty( pComponent.gameObject );
            }
        }        
    }
}