using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationSettingExample : MonoBehaviour
{
	void Awake()
	{
		
	}
	
	void Start()
	{
		Debug.LogWarning("ApplicationSetting.xml should be used in the same directory as .exe file.");
	}
	
	void Update()
	{
		
	}

	void OnGUI()
	{
		GUILayout.BeginVertical();

		GUILayout.Label("-- ApplicationSetting --");
		GUILayout.Label("IsDebug : " + ApplicationSetting.Instance.GetBool("IsDebug").ToString());
		GUILayout.Label("UseDebugConsole : " + ApplicationSetting.Instance.GetBool("UseDebugConsole").ToString());
		GUILayout.Label("DebugConsoleLogEnable : " + ApplicationSetting.Instance.GetBool("DebugConsoleLogEnable").ToString());
		GUILayout.Label("UseMouse : " + ApplicationSetting.Instance.GetBool("UseMouse").ToString());
		GUILayout.Label("UseSE : " + ApplicationSetting.Instance.GetBool("UseSE").ToString());
		GUILayout.Label("VolSE : " + ApplicationSetting.Instance.GetFloat("VolSE").ToString());
		GUILayout.Label("UseBGM : " + ApplicationSetting.Instance.GetBool("UseBGM").ToString());
		GUILayout.Label("VolBGM : " + ApplicationSetting.Instance.GetFloat("VolBGM").ToString());
		GUILayout.Label("GameTime : " + ApplicationSetting.Instance.GetInt("GameTime").ToString());

		GUILayout.EndVertical();
	}
}
