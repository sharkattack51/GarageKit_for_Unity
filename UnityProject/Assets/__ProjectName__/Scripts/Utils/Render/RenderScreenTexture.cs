using UnityEngine;
using System.Collections;

/*
 * カメラにRenderTextureを設定して外部から取得可能にする
 */

[RequireComponent(typeof(Camera))]
public class RenderScreenTexture : MonoBehaviour
{
	private RenderTexture renderScreenTexture;
	public RenderTexture GetRenderTexture(){ return renderScreenTexture; }
	
	public bool asScreenSize = true;
	public Vector2 textureSize = new Vector2(640, 480);
	
	
	void Awake()
	{
		
	}
	
	void Start()
	{
		//スクリーンサイズでの自動設定
		if(asScreenSize)
			textureSize = new Vector2(Screen.width, Screen.height);
		
		//RenderTextureの作成
		renderScreenTexture = new RenderTexture((int)textureSize.x, (int)textureSize.y, 8, RenderTextureFormat.ARGB32);
		renderScreenTexture.anisoLevel = 9;
		renderScreenTexture.filterMode = FilterMode.Point;
		renderScreenTexture.wrapMode = TextureWrapMode.Clamp;
		
		//TargetTextureの設定
		this.gameObject.camera.targetTexture = renderScreenTexture;
	}
	
	void Update()
	{
	
	}
	
	void OnDisable()
	{
		//レンダーテクスチャをリリース
		this.gameObject.camera.targetTexture = null;
		renderScreenTexture.Release();
		RenderTexture.DestroyImmediate(renderScreenTexture);
		renderScreenTexture = null;
	}
}
