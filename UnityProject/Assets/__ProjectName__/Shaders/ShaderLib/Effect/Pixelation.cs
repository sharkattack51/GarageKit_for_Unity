using UnityEngine;
using System.Collections;

[AddComponentMenu("Image Effects/Pixelation")]
public class Pixelation : MonoBehaviour
{
	//ピクセル分割
	public int scale = 8;
	
	void OnRenderImage(RenderTexture source, RenderTexture destination)
	{	
		RenderTexture small = RenderTexture.GetTemporary(source.width / scale, source.height / scale, 0);
		
		Graphics.Blit(source, small);
		
		small.filterMode = FilterMode.Point;
		Graphics.Blit(small, destination);
		
		RenderTexture.ReleaseTemporary(small);           
	}
}
