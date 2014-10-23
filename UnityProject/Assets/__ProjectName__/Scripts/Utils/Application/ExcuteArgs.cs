using UnityEngine;
using System.Collections;
using System;

/*
 * コンテンツ起動時の引数を受け取る
 */

public class ExcuteArgs : MonoBehaviour
{	
	private static ExcuteArgs instance;
	public static ExcuteArgs Instance { get{ return instance; } }
	
	private string[] args;
	public string[] Args { get{ return args; } }
	
	
	void Awake()
	{
		//参照用のインスタンス設定
		instance = this;
		
		//コマンドライン実行引数を取得する
		if(Application.platform != RuntimePlatform.WindowsEditor)
		{
			args = System.Environment.GetCommandLineArgs();
			
			for(int i=0; i<args.Length; i++)
			{
				//Debug.Log("excute args [" + i.ToString() + "] : " + args[i]);
				//DebugConsole.Log("excute args [" + i.ToString() + "] : " + args[i]);
			}
		}
	}
	
}
