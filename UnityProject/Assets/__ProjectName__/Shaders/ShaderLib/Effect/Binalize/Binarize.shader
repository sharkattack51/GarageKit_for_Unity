Shader "Custom/ShaderLib/Effect/Binarize"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_Threshold("Threshold", Float) = 0.5
	}
	
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert 

		sampler2D _MainTex;
		float _Threshold;

		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutput o)
		{
			half c = tex2D(_MainTex, IN.uv_MainTex).r;
			
			if(c < _Threshold) c = 0.0;
			else c = 1.0;			
			
			o.Albedo = half3(c, c, c);
			o.Alpha = c;
		}
		
		ENDCG
	}
	
	FallBack "Diffuse"
}