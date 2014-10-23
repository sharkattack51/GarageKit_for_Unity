using UnityEngine;
using System.Collections;

public class Binalize : MonoBehaviour
{
	public Material material = null;
	public float threshold = 0.05f;
	
	void Update()
	{
		material.SetFloat("_Threshold", threshold);
	}
	
	void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		//PostEffectシェーダーを適用
		Graphics.Blit(source, destination, material);
	}
}
