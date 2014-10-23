using UnityEngine;
using System.Collections;
using System;

/*
 * マウスポインタの表示/非表示
 */

public class VisibleMouseCursor : MonoBehaviour
{	
	public static bool showCursor;
	
	void Update()
	{
		Screen.showCursor = showCursor;
	}
}
