Shader "Custom/ShaderLib/DotPlane"
{
    properties
    {
        _Color("Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _Scale("Scale", float) = 100.0
        _DotSize("DotSize", float) = 1.0
        [MaterialToggle] _IsWorldCoord("IsWorldCoord", int) = 1
    }

    SubShader
    {
        Blend SrcAlpha OneMinusSrcAlpha

        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
        }

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
                half3 worldNormal : TEXCOORD1;
            };

            v2f vert(appdata input)
            {
                v2f output;
                output.position = UnityObjectToClipPos(input.vertex);
                output.uv = input.uv;
                output.worldNormal = mul(unity_ObjectToWorld, input.vertex).xyz;

                return output;
            }

            float4 _Color;
            float _Scale;
            float _DotSize;
            int _IsWorldCoord;

            fixed4 frag(v2f input) : SV_Target
            {
                float2 v = (_IsWorldCoord == 1) ? float2(input.worldNormal.x, input.worldNormal.z) : input.uv;
                float f = 10.0 * ((sin(v.x * _Scale) * 0.5 + 0.5) + (sin(v.y * _Scale) * 0.5 + 0.5));
                float a = step(f, _DotSize);

                return fixed4(_Color.r, _Color.g, _Color.b, _Color.a * a); 
            }

            ENDCG
        }
    }
}
