Shader "Custom/SpriteFlashWhite"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FlashAmount ("Flash Amount", Range(0,1)) = 0
        _FlashColor ("Flash Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags 
        { 
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        Lighting Off
        ZWrite Off

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
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _FlashAmount;
            float4 _FlashColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                // 核心：原图 → 白色 插值
                col.rgb = lerp(col.rgb, _FlashColor.rgb, _FlashAmount);

                return col;
            }
            ENDCG
        }
    }
}