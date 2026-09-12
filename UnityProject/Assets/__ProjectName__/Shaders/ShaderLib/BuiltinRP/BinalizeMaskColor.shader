Shader "Custom/ShaderLib/BinalizeMaskColor"
{
    properties
    {
        _BaseMap("BaseMap", 2D) = "white" {}
        _Threshold ("Threshold", Float) = 0.5
        _Color ("Main Color", Color) = (1, 1, 1, 0.5)
    }

    SubShader
    {
        //Blend SrcAlpha OneMinusSrcAlpha // alpha blend
        Blend SrcAlpha One // additive

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
            };

            v2f vert(appdata input)
            {
                v2f output;
                output.position = UnityObjectToClipPos(input.vertex);
                output.uv = input.uv;

                return output;
            }

            sampler2D _BaseMap;
            float _Threshold;
            float4 _Color;

            fixed4 frag(v2f input) : SV_Target
            {
                float a = (tex2D(_BaseMap, input.uv).r >= _Threshold) ? 1.0 : 0.0;
                return fixed4(_Color.r, _Color.g, _Color.b, _Color.a * a);
            }

            ENDCG
        }
    }
}
