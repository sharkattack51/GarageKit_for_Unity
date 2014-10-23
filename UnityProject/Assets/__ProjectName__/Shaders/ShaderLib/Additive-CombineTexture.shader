Shader "Custom/ShaderLib/Additive CombineTexture"
{	
	Properties
	{
		_Color ("Main Color", Color) = (1, 1, 1, 0.5)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_DetailTex ("Detail (RGB)", 2D) = "white" {}
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
		
		ZWrite Off
		Lighting Off
		Blend One One
		
		CGPROGRAM
		#pragma surface surf Lambert
		
		struct Input
		{
			float2 uv_MainTex;
			float2 uv_DetailTex;
		};
		
		float4 _Color;
		sampler2D _MainTex;
		sampler2D _DetailTex;
		
		void surf(Input IN, inout SurfaceOutput o)
		{
			float3 outColor = _Color.rgb;
			outColor *= tex2D (_MainTex, IN.uv_MainTex).rgb;
			outColor *= tex2D (_DetailTex, IN.uv_DetailTex).rgb;
			
			o.Albedo = outColor * _Color.a;
		}
		
		ENDCG
	}
}
