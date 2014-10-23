using UnityEngine;
using System.Collections;

/*
 * キー入力による表示のトグルコントロール
 */ 

[RequireComponent(typeof(Renderer))]
public class VisibleByKeyInput : MonoBehaviour
{
	//受け付けるキーコード
	public KeyCode key = KeyCode.D;
	
	//デバッグ設定の使用
	public bool useIsDebug = true;
	
	
	void Start()
	{
		if(useIsDebug)
			this.gameObject.renderer.enabled = bool.Parse(ApplicationSetting.Instance.Data["IsDebug"]);
	}
	
	void Update()
	{
		if(Input.GetKeyDown(key))
			this.renderer.enabled = !this.renderer.enabled;
	}
}
