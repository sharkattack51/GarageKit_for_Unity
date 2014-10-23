Shader "Custom/ShaderLib/FresnelUnlit"
{	
	Properties 
	{
		_MainColor("_MainColor", Color) = (0.2, 0.5, 0.7, 0.5)
		_RimPower("_RimPower", Range(0, 2)) = 1
	}
	
	SubShader 
	{
		Tags
		{
			"Queue"="Transparent"
			"IgnoreProjector"="False"
			"RenderType"="Transparent"
		}
		
		Cull Back
		ZWrite Off
		ZTest LEqual
		ColorMask RGBA
		
		CGPROGRAM
		#pragma surface surf BlinnPhongEditor  noambient nolightmap alpha decal:blend vertex:vert
		#pragma target 2.0
		
		float4 _MainColor;
		float _RimPower;

		struct EditorSurfaceOutput
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half3 Gloss;
			half Specular;
			half Alpha;
			half4 Custom;
		};
		
		inline half4 LightingBlinnPhongEditor_PrePass(EditorSurfaceOutput s, half4 light)
		{
			half3 spec = light.a * s.Gloss;
			half4 c;
			c.rgb = (s.Albedo * light.rgb + light.rgb * spec);
			c.a = s.Alpha;
			
			return c;
		}

		inline half4 LightingBlinnPhongEditor(EditorSurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
		{
			half3 h = normalize (lightDir + viewDir);
			
			half diff = max (0, dot(lightDir, s.Normal));
			
			float nh = max(0, dot(s.Normal, h));
			float spec = pow(nh, s.Specular * 128.0);
			
			half4 res;
			res.rgb = _LightColor0.rgb * diff;
			res.w = spec * Luminance (_LightColor0.rgb);
			res *= atten * 2.0;

			return LightingBlinnPhongEditor_PrePass(s, res);
		}
		
		struct Input
		{
			float3 viewDir;
		};

		void vert(inout appdata_full v, out Input o)
		{
			float4 VertexOutputMaster0_0_NoInput = float4(0, 0, 0, 0);
			float4 VertexOutputMaster0_1_NoInput = float4(0, 0, 0, 0);
			float4 VertexOutputMaster0_2_NoInput = float4(0, 0, 0, 0);
			float4 VertexOutputMaster0_3_NoInput = float4(0, 0, 0, 0);
		}
			

		void surf(Input IN, inout EditorSurfaceOutput o)
		{
			o.Normal = float3(0.0, 0.0, 1.0);
			o.Alpha = 1.0;
			o.Albedo = 0.0;
			o.Emission = 0.0;
			o.Gloss = 0.0;
			o.Specular = 0.0;
			o.Custom = 0.0;
			
			float4 Split0 = _MainColor;
			float4 Fresnel0_1_NoInput = float4(0, 0, 1, 1);
			float4 Fresnel0 = (1.0 - dot(normalize(float4(IN.viewDir.x, IN.viewDir.y, IN.viewDir.z, 1.0).xyz), normalize(Fresnel0_1_NoInput.xyz))).xxxx;
			float4 Pow0 = pow(Fresnel0,_RimPower.xxxx);
			float4 Multiply0 = float4(Split0.w, Split0.w, Split0.w, Split0.w) * Pow0;
			float4 Master0_0_NoInput = float4(0, 0, 0, 0);
			float4 Master0_1_NoInput = float4(0, 0, 1, 1);
			float4 Master0_3_NoInput = float4(0, 0, 0, 0);
			float4 Master0_4_NoInput = float4(0, 0, 0, 0);
			float4 Master0_7_NoInput = float4(0, 0, 0, 0);
			float4 Master0_6_NoInput = float4(1, 1, 1, 1);
			
			o.Emission = _MainColor;
			o.Alpha = Multiply0;
			o.Normal = normalize(o.Normal);
		}
		
		ENDCG
	}
	
	Fallback "Diffuse"
}