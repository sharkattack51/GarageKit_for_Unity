Shader "Custom/ShaderLib/Unlit/CombineAlpha"
{	
	Properties
	{
		_Color ("Main Color", Color) = (1, 1, 1, 0.5)
		_MainTex ("Base (RGBA)", 2D) = "white" {}
		_AlphaTex ("Trans. (Alpha)", 2D) = "white" {}
	}
	
	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
		
		LOD 100
		
		ZWrite On
		Blend SrcAlpha OneMinusSrcAlpha
		
		Pass
		{
			Lighting Off
			SetTexture [_MainTex]
			{
				constantColor [_Color]
				combine texture * constant, texture * constant
			}
			SetTexture [_AlphaTex]
			{
				combine previous, previous * texture
			}
		}
	}
}
