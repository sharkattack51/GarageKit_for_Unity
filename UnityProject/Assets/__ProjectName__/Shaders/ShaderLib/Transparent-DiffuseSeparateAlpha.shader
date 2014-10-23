Shader "Custom/ShaderLib/Transparent/Diffuse Separate Alpha"
{	
	Properties
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGBA)", 2D) = "black" {}
		_AlphaTex ("Trans (A)", 2D) = "black" {}
	}

	SubShader
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		LOD 200
	
		CGPROGRAM
		#pragma surface surf Lambert alpha

		fixed4 _Color;	
		sampler2D _MainTex;
		sampler2D _AlphaTex;
	
		struct Input
		{
			float2 uv_MainTex;
			float2 uv_AlphaTex;
		};

		void surf (Input IN, inout SurfaceOutput o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			fixed4 alp = tex2D(_MainTex, IN.uv_MainTex) * tex2D(_AlphaTex, IN.uv_AlphaTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = alp.a;
		}
		
		ENDCG
	}

	Fallback "Transparent/VertexLit"
}
