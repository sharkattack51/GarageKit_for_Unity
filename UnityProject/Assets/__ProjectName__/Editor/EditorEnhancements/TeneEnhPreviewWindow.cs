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

using Tenebrous.EditorEnhancements;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class TeneEnhPreviewWindow : EditorWindow
{
	private static TeneEnhPreviewWindow _window;

	private Object _asset;
	private Texture2D _tex;
	private double _timeStart;
	private bool _noPreview;
	private string _info;

	private bool _hasAlpha;

	private bool _anchoredToRight;

	public static void Update( Rect pAnchorTo, Vector2 pMousePosition, string pGUID = null, Object pAsset = null )
	{
		if( ( pGUID == null && pAsset == null )
			|| !CanShowPreviewFor(pGUID:pGUID, pAsset:pAsset)
		)
		{
			if( _window != null )
			{
				_window.Close();
				_window = null;
			}

			return;
		}

		if( _window == null )
			_window = EditorWindow.GetWindow<TeneEnhPreviewWindow>( true );

		_window.SetPosition( pAnchorTo, pMousePosition );
		_window.SetPreview( pGUID: pGUID, pAsset: pAsset );
	}

	static void Resolve( ref string pGUID, ref string pPath, ref Object pAsset )
	{
		if( pGUID != null )
			pPath = AssetDatabase.GUIDToAssetPath( pGUID );

		if( pPath != null )
			pAsset = AssetDatabase.LoadAssetAtPath( pPath, typeof( Object ) );

		if( pAsset is MeshFilter )
			pAsset = ( (MeshFilter)pAsset ).sharedMesh;
		else if( pAsset is AudioSource )
			pAsset = ( (AudioSource)pAsset ).clip;
		else if( pAsset is GameObject )
		{
			GameObject pBaseAsset = PrefabUtility.GetCorrespondingObjectFromSource(pAsset) as GameObject;
			if( pBaseAsset != null )
				pAsset = pBaseAsset;
		}
	}


	static bool CanShowPreviewFor( string pGUID = null, string pPath = null, Object pAsset = null )
	{
		Resolve( ref pGUID, ref pPath, ref pAsset );

		// include specific things
		if( pAsset is Camera )
			return (true);

		if( pAsset is GameObject )
			return ((GameObject) pAsset).HasAnyRenderers();

		// exclude specific things
		return(
			pAsset != null
			&& !( pAsset is MonoScript )
			&& !( pAsset is TextAsset )
			&& !( pAsset is MonoBehaviour )
			&& !( pAsset is Behaviour )
			&& !( pAsset is Collider )
			&& !( pAsset is Component )
			&& pAsset.GetType().ToString() != "UnityEngine.Object"
		);
	}

	private void SetPosition( Rect pWindow, Vector2 pMouse )
	{
		Rect newPos = new Rect( pWindow.x - 210, pMouse.y - 90, 200, 200 );

		_anchoredToRight = false;

		if( newPos.x < 0 )
		{
			newPos.x = pWindow.x + pWindow.width;
			_anchoredToRight = true;
		}

		newPos.y = Mathf.Clamp( newPos.y, 0, Screen.currentResolution.height - 250 );

		position = newPos;
	}

	private void SetPreview( string pGUID = null, string pPath = null, Object pAsset = null )
	{
		Resolve( ref pGUID, ref pPath, ref pAsset );
//
        string newTitle = pAsset.name + " (" + pAsset.GetType().ToString().Replace("UnityEngine.", "") + ")";

#if (UNITY_5 || UNITY_5_3_OR_NEWER) && !UNITY_5_0
        titleContent = new GUIContent(newTitle);
#else
        title = newTitle;
#endif

		_asset = pAsset;
		_noPreview = false;
		_hasAlpha = false;
		_info = "";

		if( pAsset == null )
			return;

		if( _asset is Texture2D )
		{
			_tex = (Texture2D)_asset;
			_hasAlpha = true;

			if( _anchoredToRight )
				position = new Rect( position.x, position.y, position.width + 200, position.height );
			else
				position = new Rect( position.x - 200, position.y, position.width + 200, position.height );
		}
		else
			_tex = Common.GetAssetPreview( _asset );

		_info += _asset.GetPreviewInfo();// +"\n" + _asset.GetType().ToString();

		_timeStart = EditorApplication.timeSinceStartup;

		_window.Repaint();

		if( EditorWindow.mouseOverWindow != null )
			EditorWindow.mouseOverWindow.Focus();
	}

	void Update()
	{
		if( _tex == null && _asset != null )
		{
			_tex = Common.GetAssetPreview( _asset );
			if( _tex != null )
				Repaint();
			else if( EditorApplication.timeSinceStartup - _timeStart > 3.0f )
			{
				_noPreview = true;
				Repaint();
			}
		}
	}

	void OnGUI()
	{
		Rect pos = position;
		pos.x = 0;
		pos.y = 0;

		if( _tex != null )
			ShowTexture( pos );
		if( _asset is Camera )
			ShowCamera(pos, (Camera)_asset);
		else if( _asset is MonoBehaviour || _asset is Component )
			ShowMonoBehaviour( pos );

		pos.y = pos.height - EditorStyles.boldLabel.CalcHeight( new GUIContent( _info ), pos.width );
		pos.height -= pos.y;

		GUIStyle s = new GUIStyle( EditorStyles.boldLabel );
		s.normal.textColor = Color.white;
		GUI.color = Color.white;
		EditorGUI.DropShadowLabel( pos, _info, s );
	}

	void ShowCamera( Rect pPos, Camera pCamera )
	{
		Rect oldViewport = pCamera.rect;
		Handles.DrawCamera(pPos, pCamera);
		pCamera.rect = oldViewport;
	}

	void ShowMonoBehaviour(Rect pPos)
	{
		//MonoScript ms = MonoScript.FromMonoBehaviour( _asset as MonoBehaviour );
		//GUI.Label(pPos,ms.GetPreviewInfo());
	}

	void ShowTexture(Rect pPos)
	{
		if( _tex == null )
		{
			if( _noPreview )
				GUI.Label( pPos, "No preview\n\n" + _info );
			else
				GUI.Label( pPos, "Loading...\n\n" + _info );
		}
		else
		{
			GUI.color = Color.white;

			if( _hasAlpha )
			{
				Rect half = pPos;
				half.width /= 2;
				EditorGUI.DrawTextureAlpha( half, _tex );

				half.x += half.width;
				EditorGUI.DrawPreviewTexture( half, _tex );
			}
			else
			{
				//GUI.DrawTexture(pPos, EditorGUIUtility.whiteTexture);
				GUI.DrawTexture(pPos, _tex, ScaleMode.StretchToFill, true);
			}
		}		
	}
}
