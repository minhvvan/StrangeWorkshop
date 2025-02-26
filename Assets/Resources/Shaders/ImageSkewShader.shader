Shader "UI/SkewShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}   // UI의 기본 텍스처
        _SkewX ("Skew X", Range(-1, 1)) = 0.5   // X축 Skew 값 (-1 ~ 1)
        _SkewY ("Skew Y", Range(-1, 1)) = 0.0   // Y축 Skew 값 (-1 ~ 1)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 100
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _SkewX;
            float _SkewY;

            v2f vert (appdata_t v)
            {
                v2f o;
                // Skew 적용
                float skewOffsetX = v.vertex.y * _SkewX;
                float skewOffsetY = v.vertex.x * _SkewY;
                v.vertex.x += skewOffsetX;
                v.vertex.y += skewOffsetY;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;
                return col;
            }
            ENDCG
        }
    }
}