using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GuiController : MonoBehaviour
{
	public GameObject guiStartup;
	public GameObject guiWait;
	public GameObject guiPlay;
	public GameObject guiResult;
	public GameObject guiTimerText;

	private List<GameObject> showHideGuis;


	void Awake()
	{

	}

	void Start()
	{
		showHideGuis = new List<GameObject>();
		showHideGuis.Add(guiStartup);
		showHideGuis.Add(guiWait);
		showHideGuis.Add(guiPlay);
		showHideGuis.Add(guiResult);
	}
	
	void Update()
	{
	
	}


	public void SetGUI(SceneStateManager.SceneState state)
	{
		foreach(GameObject gui in showHideGuis)
			gui.SetActive(false);

		showHideGuis[(int)state].SetActive(true);
	}
}
