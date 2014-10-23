// Upgrade NOTE: replaced 'glstate.matrix.invtrans.modelview[0]' with 'UNITY_MATRIX_IT_MV'
// Upgrade NOTE: replaced 'glstate.matrix.mvp' with 'UNITY_MATRIX_MVP'

Shader "Custom/ShaderLib/CrossSection"
{
	Properties 
	{
		section_depth ("section depth (x, y, z, depth)", vector) = (0,0,0,0.15)
		section_color ("section color", color) = (0.5,0.1, 0.1, 1)
		
		color_map ("color map", 2D) = "white" {}	
	}

	SubShader
	{
		Pass
		{
			CULL OFF
			
			CGPROGRAM

			//#pragma target 3.0
			#pragma exclude_renderers gles
			#pragma vertex vertex_shader
			#pragma fragment fragment_shader

			#include "UnityCG.cginc"

			uniform float4 section_depth;
			uniform float4 section_color;
			uniform sampler2D color_map;	

			float4x4 rotate(float3 r) 
			{ 
				float3 c, s; 
				sincos(r.x, s.x, c.x); 
				sincos(r.y, s.y, c.y); 
				sincos(r.z, s.z, c.z);

				return float4x4(
					c.y * c.z, -s.z, s.y, 0, 
					s.z, c.x * c.z, -s.x, 0, 
					-s.y, s.x, c.x * c.y, 0, 
					0, 0, 0, 1);
			}

			struct a2v 
			{
				float4 vertex:POSITION;
				float4 color:COLOR;
				float2 texcoord:TEXCOORD;
				float3 normal:NORMAL;
			};

			struct v2f
			{
				float4 position:POSITION;
				float2 texcoord:TEXCOORD0;								
				float4 normal:TEXCOORD1; 
				float4 vertex:TEXCOORD2;
				float4 mask:TEXCOORD3;				
			};
			
			v2f vertex_shader( a2v IN )
			{													
				v2f OUT;
				
				float4x4 r = rotate(radians(section_depth.xyz));
				float4 c = float4(IN.vertex.xyz, 1);
				
				OUT.mask = mul(r, c);
				OUT.position = mul(UNITY_MATRIX_MVP, IN.vertex);
				OUT.texcoord = IN.texcoord;
				
				r *= float4x4(
					1,-1,-1, 0,
					-1, 1,-1, 0,
					-1,-1, 1, 0,
					0, 0, 0, 1); // the section_depth.xyz need to be inverted !
				
				OUT.normal = mul(r, float4(1, 0, 0, 1));
				OUT.vertex = IN.vertex;
				
				return OUT;
			}
			
			void fragment_shader(v2f IN, out float4 finalcolor:COLOR)				
			{
				if(IN.mask.x > section_depth.w)
					discard;
				
				float3 N = IN.normal.xyz;
				
				N = mul(UNITY_MATRIX_IT_MV, float4(N, 1));				
				float diffuse = saturate(dot(glstate.light[0].position, N));	
				
				finalcolor = float4(0, 0, 0, 1);		
				finalcolor.xyz = section_color * (diffuse * 0.6 + 0.4);
			}

			ENDCG
		}
		
		Pass
		{		
			CULL BACK
			
			CGPROGRAM

			#pragma exclude_renderers gles
			#pragma vertex vertex_shader
			#pragma fragment fragment_shader

			#include "UnityCG.cginc"

			uniform float4 section_depth;
			uniform float4 section_color;
			uniform sampler2D color_map;	

			float4x4 rotate(float3 r) 
			{ 
				float3 c, s; 
				sincos(r.x, s.x, c.x); 
				sincos(r.y, s.y, c.y); 
				sincos(r.z, s.z, c.z);

				return float4x4(
					c.y * c.z, -s.z, s.y, 0, 
					s.z, c.x * c.z,-s.x, 0, 
					-s.y, s.x, c.x*c.y, 0, 
					0, 0, 0, 1);
			}

			struct a2v 
			{
				float4 vertex:POSITION;
				float4 color:COLOR;
				float2 texcoord:TEXCOORD;
				float3 normal:NORMAL;
			};

			struct v2f
			{
				float4 position:POSITION;
				float2 texcoord:TEXCOORD0;								
				float3 normal:TEXCOORD1; 
				float4 vertex:TEXCOORD2;
				float4 mask:TEXCOORD3;				
			};
			
			v2f vertex_shader(a2v IN)
			{												
				v2f OUT;
				
				float4x4 r = rotate(radians(section_depth.xyz));
				float4 c = float4(IN.vertex.xyz, 1);
				
				OUT.mask = mul(r, c);
				OUT.position = mul(UNITY_MATRIX_MVP, IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.normal = IN.normal;
				OUT.vertex = IN.vertex;
				
				return OUT;
			}
			
			void fragment_shader(v2f IN, out float4 finalcolor:COLOR)	
			{
				if(IN.mask.x > section_depth.w)
					discard;
				
				float3 N = IN.normal;
					
				N = mul(UNITY_MATRIX_IT_MV, float4(N, 1));				
				float diffuse = saturate(dot(glstate.light[0].position, N));
				
				finalcolor = float4(0, 0, 0, 1);											
				finalcolor.xyz = tex2D(color_map, IN.texcoord).xyz * (diffuse * 0.6 + 0.4);
			}

			ENDCG
		}
	}

	Fallback "Diffuse"
}