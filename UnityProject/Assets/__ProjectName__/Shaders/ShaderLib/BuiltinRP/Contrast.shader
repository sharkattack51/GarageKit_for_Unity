Shader "Custom/ShaderLib/Contrast"
{
    Properties
    {
        _MainTex("MainTex", 2D) = "" {}
        _InMin("InMin", Range(0.0, 1.0)) = 0.0
        _InMax("InMax", Range(0.0, 1.0)) = 1.0
        _OutMin("OutMin", Range(0.0, 1.0)) = 0.0
        _OutMax("OutMax", Range(0.0, 1.0)) = 1.0
        _Contrast("Contrast", Range(0.0, 2.0)) = 0.0
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

            sampler2D _MainTex;
            float _InMin;
            float _InMax;
            float _OutMin;
            float _OutMax;
            float _Contrast;

            struct appdata
            {
                float4 vertex : POSITION;
                float4 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float remap(float val, float inMin, float inMax, float outMin, float outMax)
            {
                return (val - inMin) * ((outMax - outMin) / (inMax - inMin)) + outMin;
            }

            float contrast(float val, float level)
            {
                return val * (1.0 - level) + (val * (val * level));
            }

            float4 frag(v2f i):COLOR
            {
                float4 c = tex2D(_MainTex, i.uv);

                c.r = contrast(remap(c.r, _InMin, _InMax, _OutMin, _OutMax), _Contrast);
                c.g = contrast(remap(c.g, _InMin, _InMax, _OutMin, _OutMax), _Contrast);
                c.b = contrast(remap(c.b, _InMin, _InMax, _OutMin, _OutMax), _Contrast);

                return c;
            }

            ENDCG
        }
    }
}