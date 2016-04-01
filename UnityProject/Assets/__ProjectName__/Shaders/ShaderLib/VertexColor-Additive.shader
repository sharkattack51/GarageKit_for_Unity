Shader "Custom/ShaderLib/VertexColor Additive"
{
	Properties
	{
		_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_Multiply ("Multiply Value", Range(0.01,5.0)) = 2.0
	}

	Category
	{
		Tags
		{
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
		}
		
		Blend SrcAlpha One
		ColorMask RGB
		Cull Off
		Lighting Off
		ZWrite On
		
		SubShader
		{
			Pass
			{
				CGPROGRAM

				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"

				fixed4 _TintColor;
				float _Multiply;
			
				struct appdata_t
				{
					float4 vertex : POSITION;
			    float4 tangent : TANGENT;
			    float3 normal : NORMAL;
			    float4 texcoord : TEXCOORD0;
			    float4 color : COLOR0;
				};

				struct v2f
				{
					float4 vertex : SV_POSITION;
					fixed4 color : COLOR0;
				};
				
				v2f vert(appdata_t v)
				{
					v2f o;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					o.color = v.color;
					return o;
				}
			
				fixed4 frag(v2f i):COLOR
				{
					fixed4 col = _Multiply * i.color * _TintColor;
					return col;
				}

				ENDCG 
			}
		}	
	}
}
