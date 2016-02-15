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
			this.gameObject.GetComponent<Renderer>().enabled = ApplicationSetting.Instance.GetBool("IsDebug");
	}
	
	void Update()
	{
		if(Input.GetKeyDown(key))
			this.GetComponent<Renderer>().enabled = !this.GetComponent<Renderer>().enabled;
	}
}
