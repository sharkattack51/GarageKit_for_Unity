using UnityEngine;
using System.Collections;
using System;

/*
 * シェーダー内のテクスチャを更新する
 */ 

[Serializable]
public class LayerTexture
{
	public RenderScreenTexture renderScreenTexture;
	public string shaderTextureName = "_MainTex";
}

public class CompositLayer : MonoBehaviour
{
	public bool isUpdateShader = true;
	public LayerTexture[] layerTextures;
	
	
	void Awake()
	{
	
	}
	
	void Start()
	{
	
	}
	
	void Update()
	{
		//シェーダー内のテクスチャを更新
		if(isUpdateShader)
		{
			foreach(LayerTexture layerTex in layerTextures)
			{
				this.gameObject.renderer.material.SetTexture(
					layerTex.shaderTextureName,
					layerTex.renderScreenTexture.GetRenderTexture());
			}
		}
	}
}
