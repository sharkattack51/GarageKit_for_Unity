using UnityEngine;
using System.Collections;

/*
 * メーター表示オブジェクト
 */

[ExecuteInEditMode]
[RequireComponent(typeof(Renderer))]
public class MeterObject : MonoBehaviour
{
#if UNITY_4_2
	[Range(0.0f, 1.0f)]
#endif
	public float fillAmount = 1.0f;
	public bool invert = false;
		
	
	void Awake()
	{
	
	}
	
	void Start()
	{
	
	}
	
	void Update()
	{
		//uvオフセットにメーター値を反映
		if(invert)
		{
			this.GetComponent<Renderer>().material.mainTextureScale = new Vector2(1.0f, 1.0f);
			this.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(-fillAmount + 0.5f, 0.0f);
		}
		else
		{
			this.GetComponent<Renderer>().material.mainTextureScale = new Vector2(-1.0f, 1.0f);
			this.GetComponent<Renderer>().material.mainTextureOffset = new Vector2((1.0f - fillAmount) + 0.5f, 0.0f);
		}
	}
}
