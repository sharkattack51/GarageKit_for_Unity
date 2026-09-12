Shader "Graphy/Graph Mobile"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap("Pixel snap", Float) = 0

        _GoodColor("Good Color", Color) = (1,1,1,1)
        _CautionColor("Caution Color", Color) = (1,1,1,1)
        _CriticalColor("Critical Color", Color) = (1,1,1,1)

        _GoodThreshold("Good Threshold", Float) = 0.5
        _CautionThreshold("Caution Threshold", Float) = 0.25
    }

    SubShader
    {
        PackageRequirements
        {
            "com.unity.render-pipelines.universal": "14.0"
        }

        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
            "RenderPipeline"="UniversalPipeline"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            Name "Default"
            Tags { "LightMode"="SRPDefaultUnlit" }

            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ PIXELSNAP_ON
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                half4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                half4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_AlphaTex);
            SAMPLER(sampler_AlphaTex);

            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
                half4 _GoodColor;
                half4 _CautionColor;
                half4 _CriticalColor;
                float _GoodThreshold;
                float _CautionThreshold;
                float Average;
                float GraphValues_Length;
                float GraphValueWidth;
            CBUFFER_END

            float _AlphaSplitEnabled;
            float GraphValues[128];

            static const half ThresholdLineAlpha = 0.55;
            static const half ThresholdLineWidth = 0.02;

            float4 GraphyPixelSnap(float4 positionCS)
            {
                float2 halfScreen = _ScreenParams.xy * 0.5;
                float2 pixelPosition = round((positionCS.xy / positionCS.w) * halfScreen);
                positionCS.xy = pixelPosition / halfScreen * positionCS.w;
                return positionCS;
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT = (Varyings)0;

                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;

                #ifdef PIXELSNAP_ON
                OUT.positionCS = GraphyPixelSnap(OUT.positionCS);
                #endif

                return OUT;
            }

            half4 SampleSpriteTexture(float2 uv)
            {
                half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);

                #if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
                if (_AlphaSplitEnabled)
                    color.a = SAMPLE_TEXTURE2D(_AlphaTex, sampler_AlphaTex, uv).r;
                #endif

                return color;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

                half4 color = IN.color;

                float xCoord = IN.texcoord.x;
                float yCoord = IN.texcoord.y;

                int graphValueIndex = (int) min(floor(xCoord * GraphValues_Length), GraphValues_Length - 1);
                float graphValue = GraphValues[graphValueIndex];

                if (graphValue > _GoodThreshold)
                {
                    color *= _GoodColor;
                }
                else if (graphValue > _CautionThreshold)
                {
                    color *= _CautionColor;
                }
                else
                {
                    color *= _CriticalColor;
                }

                if (graphValue - yCoord > GraphValueWidth)
                {
                    color.a *= yCoord * 0.3 / graphValue;
                }

                if (yCoord > graphValue)
                {
                    color.a = 0;
                }

                if (yCoord < Average && yCoord > Average - 0.02)
                {
                    color = half4(1, 1, 1, 1);
                }

                if (yCoord < _CautionThreshold && yCoord > _CautionThreshold - ThresholdLineWidth)
                {
                    color = _CautionColor;
                    color.a *= ThresholdLineAlpha;
                }

                if (yCoord < _GoodThreshold && yCoord > _GoodThreshold - ThresholdLineWidth)
                {
                    color = _GoodColor;
                    color.a *= ThresholdLineAlpha;
                }

                color.a *= saturate(min(xCoord, 1 - xCoord) * 33.333333);

                half4 result = SampleSpriteTexture(IN.texcoord) * color;
                result.rgb *= result.a;

                return result;
            }
            ENDHLSL
        }
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
            "RenderPipeline"=""
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            Name "Default"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ PIXELSNAP_ON

            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;

                UNITY_VERTEX_OUTPUT_STEREO
            };

            fixed4 _Color;

            v2f vert(appdata_t IN)
            {
                v2f OUT;

                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_OUTPUT(v2f, OUT);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap(OUT.vertex);
                #endif

                return OUT;
            }

            sampler2D _MainTex;
            sampler2D _AlphaTex;
            float _AlphaSplitEnabled;

            fixed4 SampleSpriteTexture(float2 uv)
            {
                fixed4 color = tex2D(_MainTex, uv);

                #if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
                if (_AlphaSplitEnabled)
                    color.a = tex2D(_AlphaTex, uv).r;
                #endif //UNITY_TEXTURE_ALPHASPLIT_ALLOWED

                return color;
            }

            fixed4 _GoodColor;
            fixed4 _CautionColor;
            fixed4 _CriticalColor;

            fixed _GoodThreshold;
            fixed _CautionThreshold;

            uniform float Average;

            // NOTE: The size of this array can break compatibility with some older GPUs
            // This shader is pretty much equal to GraphStandard.shader but with a smaller Array size.
            uniform float GraphValues[128];

            uniform float GraphValues_Length;
            uniform float GraphValueWidth;

            static const fixed ThresholdLineAlpha = 0.55;
            static const fixed ThresholdLineWidth = 0.02;

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 color = IN.color;

                fixed xCoord = IN.texcoord.x;
                fixed yCoord = IN.texcoord.y;

                int graphValueIndex = (int) min(floor(xCoord * GraphValues_Length), GraphValues_Length - 1);
                float graphValue = GraphValues[graphValueIndex];

                // Assign the corresponding color
                if (graphValue > _GoodThreshold)
                {
                    color *= _GoodColor;
                }
                else if (graphValue > _CautionThreshold)
                {
                    color *= _CautionColor;
                }
                else
                {
                    color *= _CriticalColor;
                }

                // Point coloring
                if (graphValue - yCoord > GraphValueWidth)
                {
                    //color.a = yCoord * graphValue * 0.3;
                    color.a *= yCoord * 0.3 / graphValue;
                }

                // Set as transparent the part on top of the current point value
                if (yCoord > graphValue)
                {
                    color.a = 0;
                }

                // Average white bar
                if (yCoord < Average && yCoord > Average - 0.02)
                {
                    color = fixed4(1, 1, 1, 1);
                }

                // CautionColor bar
                if (yCoord < _CautionThreshold && yCoord > _CautionThreshold - ThresholdLineWidth)
                {
                    color = _CautionColor;
                    color.a *= ThresholdLineAlpha;
                }

                // GoodColor bar
                if (yCoord < _GoodThreshold && yCoord > _GoodThreshold - ThresholdLineWidth)
                {
                    color = _GoodColor;
                    color.a *= ThresholdLineAlpha;
                }

                // Fade the alpha of the sides of the graph
                color.a *= saturate(min(xCoord, 1 - xCoord) * 33.333333);

                fixed4 c = SampleSpriteTexture(IN.texcoord) * color;

                c.rgb *= c.a;

                return c;
            }
            ENDCG
        }
    }
}