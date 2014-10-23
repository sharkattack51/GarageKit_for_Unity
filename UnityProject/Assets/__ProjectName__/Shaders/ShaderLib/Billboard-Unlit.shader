Shader "Custom/ShaderLib/Billboard-Unlit"
{
    /*
     * オブジェクト座標の回転を(0,180,0)にした状態でビルボード処理をします
     */
    
    Properties
    {
        _Color("Main Color", Color) = (1, 1, 1, 1)
        _MainTex("Base (RGBA)", 2D) = "white" {}
    }

    Category 
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector" = "True"
            "RenderType"="Transparent"
        }

        LOD 100

        ZWrite on
        Blend SrcAlpha OneMinusSrcAlpha

        SubShader
        {     
            Pass
            {   
                Cull Back

                CGPROGRAM         

                    #pragma vertex vert  
                    #pragma fragment frag 
                    #pragma only_renderers d3d9

                    #include "UnityCG.cginc"

                    uniform sampler2D _MainTex;    
                    float4 _MainTex_ST;
                    fixed4 _Color;

                    struct vertexInput
                    {
                        float4 vertex : POSITION;
                        float4 texcoord : TEXCOORD0;
                        fixed4 color : COLOR;
                    };

                    struct vertexOutput
                    {
                        float4 pos : SV_POSITION;
                        float2 tex : TEXCOORD0;
                        float4 uv : TEXCOORD0;
                        fixed4 color : COLOR;
                    };

                    vertexOutput vert(vertexInput input)
                    {
                        vertexOutput output;

                        float2 billboardDirection_local_xy = normalize(_WorldSpaceCameraPos.xz - float2(_World2Object[1].w, _World2Object[2].w)); 

                        float2x2 billboardRotation = float2x2(
                            billboardDirection_local_xy.y, 
                            billboardDirection_local_xy.x,
                            -billboardDirection_local_xy.x,
                            billboardDirection_local_xy.y
                        );  

                        output.pos.xz = mul(billboardRotation, input.vertex.xz); 
                        output.pos.yw = input.vertex.yw;
                        output.pos = mul(UNITY_MATRIX_MVP, output.pos);

                        output.tex = float2(input.texcoord.x, input.texcoord.y);                        
                        output.tex = TRANSFORM_TEX(input.texcoord, _MainTex);

                        return output;
                    }

                    fixed4 frag(vertexOutput input) : COLOR
                    {
                        return tex2D(_MainTex, float2(input.tex.xy)) * _Color;
                    }

                ENDCG
            }
        } 
    }
}