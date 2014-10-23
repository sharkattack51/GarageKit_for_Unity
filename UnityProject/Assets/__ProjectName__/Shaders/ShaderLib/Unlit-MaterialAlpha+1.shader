Shader "Custom/ShaderLib/Unlit/MaterialAlpha +1"
{	
	Properties
	{
		_Color ("Main Color", Color) = (1, 1, 1, 0.5)
		_MainTex ("Base (RGBA)", 2D) = "white" {}
	}
	
	SubShader
	{
		Tags
		{
			"Queue" = "Transparent+1"
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
		}
	}
}
