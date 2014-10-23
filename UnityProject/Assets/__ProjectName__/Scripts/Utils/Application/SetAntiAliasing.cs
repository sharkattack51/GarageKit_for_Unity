using UnityEngine;
using System.Collections;

/*
 * アンチエイリアスの設定
 */ 

public class SetAntiAliasing : MonoBehaviour
{	
	public int AA_2x;
	
	void Awake()
	{
		QualitySettings.antiAliasing = AA_2x;
	}
}
