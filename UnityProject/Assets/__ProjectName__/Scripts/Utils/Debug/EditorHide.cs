using UnityEngine;
using System.Collections;

/*
 * Editor内で編集時は非表示にし実行時に表示させる
 */

[RequireComponent(typeof(Renderer))]
public class EditorHide : MonoBehaviour
{	
	void Awake()
	{	
		//実行時に表示
		this.renderer.enabled = true;
	}
}
