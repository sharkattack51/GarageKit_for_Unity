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
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class TeneDropTarget : EditorWindow
{
	private static TeneDropTarget _window;

	private GameObject _target;
	//private Rect _anchoredTo;
	private Rect _desiredPosition;
	private bool _repositioned;
	public bool HadMouseOver;

	public static TeneDropTarget Window
	{
		get { return _window; }
	}

	public static void Update( Rect pAnchorTo, Vector2 pMousePosition, GameObject pObject )
	{
		if( _window == null )
			_window = ScriptableObject.CreateInstance<TeneDropTarget>();

		_window.ShowPopup();

		_window._close = false;
		_window._repositioned = false;
		//_window._anchoredTo = pAnchorTo;
		_window.HadMouseOver = false;
#if (UNITY_5 || UNITY_5_3_OR_NEWER) && !UNITY_5_0
        _window.titleContent = new GUIContent(pObject.name);
#else
        _window.title = pObject.name;
#endif
        _window.SetTarget( pObject );
		_window.SetPosition( pAnchorTo, pMousePosition );
	}

	public static void Hide()
	{
		if( _window != null )
		{
			DestroyImmediate( _window );
			_window.Close();
			_window = null;
		}
	}

	private void SetPosition( Rect pWindow, Vector2 pMouse )
	{
		//Rect newPos = new Rect( pWindow.x - 410, pMouse.y, 400, 0 );

		//if( newPos.x < 0 )
		//	newPos.x = pWindow.x + pWindow.width;

		_desiredPosition = pWindow;
		_desiredPosition.y = pMouse.y;
		_repositioned = false;
		Repaint();
	}

	private void SetTarget( GameObject pObject )
	{
		if( pObject == null )
			return;

		_target = pObject;

		_window.Repaint();

		if( EditorWindow.mouseOverWindow != null )
			EditorWindow.mouseOverWindow.Focus();
	}

	private bool _close;
	private System.DateTime _closeTime;
	void Update()
	{
		if( _close )
		{
			if( ( System.DateTime.Now - _closeTime ).TotalSeconds > 0.5f )
				Hide();
		}
		else if( DragAndDrop.objectReferences.Length == 0 )
			Hide();
	}

	void OnGUI()
	{
		if( DragAndDrop.objectReferences.Length == 0 )
			return;

		if( Event.current.type == EventType.MouseMove || Event.current.type == EventType.DragUpdated )
			HadMouseOver = true;

		int items = 0;
		//EditorGUIUtility.LookLikeInspector();

		Object dragging = DragAndDrop.objectReferences[0];
	    string note;

		foreach( Component component in _target.GetComponents<Component>() )
		{
			bool drawnHeader = false;
			Type type = component.GetType();

			foreach( FieldInfo f in FieldsFor( type ) )
			{
			    note = "";
				if( !IsCompatibleField(f,dragging, ref note) )
					continue;

				if( !drawnHeader )
				{
					MonoScript ms = MonoScript.FromMonoBehaviour( component as MonoBehaviour );
					string s = Path.GetFileName( AssetDatabase.GetAssetPath( ms ) );
					EditorGUILayout.LabelField( s, EditorStyles.boldLabel );
					drawnHeader = true;
				}

				UnityEngine.Object oldValue = (UnityEngine.Object)f.GetValue( component );
				EditorGUILayout.BeginHorizontal( GUILayout.Width( 330.0f ) );
                GUILayout.Label("    " + f.Name, GUILayout.Width(150.0f));

				Object newValue;
				newValue = EditorGUILayout.ObjectField( oldValue, f.FieldType, true, GUILayout.Height( 20 ), GUILayout.Width(250) );

				if( newValue != oldValue )
				{
#if UNITY_4_3_PLUS
                    Undo.RecordObject( component, "Change field" );
#else
                    Undo.RegisterUndo( component, "Change field" );
#endif
                    f.SetValue( component, newValue );
					EditorUtility.SetDirty( component.gameObject );
					_close = true;
					_closeTime = System.DateTime.Now;
				}

                GUILayout.Label( note );

				EditorGUILayout.EndHorizontal();

				items++;
			}
		}

		if( items == 0 )
		{
			GUILayout.Label( "No suitable variables for " + dragging.GetType().ToString() );
		}

		if( !_repositioned && Event.current.type == EventType.Repaint )
		{
			Rect r = GUILayoutUtility.GetLastRect();
			r.height += r.y + 4.0f;
			r.height = Mathf.Max( r.height, 50.0f );

			float newY = _desiredPosition.y - r.height / 2;
			float newX = _desiredPosition.x - 500.0f;

			if( newY < 0 )
				newY = 0;
			else if( newY + r.height > Screen.currentResolution.height - 32 )
				newY = Screen.currentResolution.height - r.height - 32;

			position = new Rect( newX, newY, 500.0f, r.height );

			_repositioned = true;
		}

		GUI.DrawTexture( new Rect( 0, 0, position.width, 1 ), EditorGUIUtility.whiteTexture );
		GUI.DrawTexture( new Rect( 0, 0, 1, position.height ), EditorGUIUtility.whiteTexture );
		GUI.DrawTexture( new Rect( 0, position.height - 1, position.width, 1 ), EditorGUIUtility.whiteTexture );
		GUI.DrawTexture( new Rect( position.width - 1, 0, 1, position.height ), EditorGUIUtility.whiteTexture );
	}

	public static FieldInfo[] FieldsFor( Type pType )
	{
		return pType.GetFields( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic );
	}

	public static bool IsCompatibleField( FieldInfo pField, Object pDesiredValue, ref string pNote )
	{
        if (pField.IsStatic)
            return false;

		if( Attribute.IsDefined( pField, typeof( System.NonSerializedAttribute ) ) )
			return false;

        if (!pField.IsPublic && !Attribute.IsDefined(pField, typeof(SerializeField)))
            return false;

        if( pField.FieldType.IsSubclassOf( typeof(Component) ))
            if( pDesiredValue is GameObject )
                if( ( (GameObject)pDesiredValue ).GetComponent( pField.FieldType ) != null )
                {
                    pNote = "(" + pField.FieldType.ToString().Replace("UnityEngine.","") + ")";
                    return true;
                }

	    if (!pField.FieldType.IsInstanceOfType(pDesiredValue))
            return false;

	    return true;
	}
}
