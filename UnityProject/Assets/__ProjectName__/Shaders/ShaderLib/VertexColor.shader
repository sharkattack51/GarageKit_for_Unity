Shader "Custom/ShaderLib/VertexColor"
{	
	Properties
	{
	
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
		Cull Off
		
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			struct appdata
			{
			    float4 vertex : POSITION;
			    float4 tangent : TANGENT;
			    float3 normal : NORMAL;
			    float4 texcoord : TEXCOORD0;
			    float4 color : COLOR0;			
			};

			struct v2f
			{
				float4 pos:SV_POSITION;
				float3 color:COLOR0;
			};
			
			v2f vert(appdata v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color;
				return o;
			}
			
			half4 frag(v2f i):COLOR
			{
				return half4(i.color, 1);
			}
			
			ENDCG
		}
	}
}
