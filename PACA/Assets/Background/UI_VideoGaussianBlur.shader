Shader "UI/VideoGaussianBlur"
{
    Properties
    {
        [PerRendererData] _MainTex ("视频贴图", 2D) = "white" {}
        _BlurSize ("模糊强度", Range(0, 10)) = 0
        _SampleCount ("采样次数", Range(1, 16)) = 6
        [HideInInspector] _StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector] _Stencil ("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp ("Stencil Operation", Float) = 0
        [HideInInspector] _StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector] _StencilReadMask ("Stencil Read Mask", Float) = 255
        [HideInInspector] _ColorMask ("Color Mask", Float) = 15
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
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            float _BlurSize;
            int _SampleCount;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.texcoord);
                float weightSum = 1;

                // 高斯模糊采样，权重随距离衰减
                for(int j = 1; j <= _SampleCount; j++)
                {
                    float weight = 1.0 / (j + 1);
                    col += tex2D(_MainTex, i.texcoord + float2(_MainTex_TexelSize.x * j * _BlurSize, 0)) * weight;
                    col += tex2D(_MainTex, i.texcoord - float2(_MainTex_TexelSize.x * j * _BlurSize, 0)) * weight;
                    col += tex2D(_MainTex, i.texcoord + float2(0, _MainTex_TexelSize.y * j * _BlurSize)) * weight;
                    col += tex2D(_MainTex, i.texcoord - float2(0, _MainTex_TexelSize.y * j * _BlurSize)) * weight;
                    weightSum += 4 * weight;
                }

                col /= weightSum;
                return col * i.color;
            }
            ENDCG
        }
    }
    FallBack "UI/Default"
}