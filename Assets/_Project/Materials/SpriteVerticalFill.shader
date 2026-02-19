Shader "Custom/SpriteVerticalFill"
{
        Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _Fill ("Fill Amount", Range(0,1)) = 1

        _FillStart ("Fill Start (UV Y)", Range(0,1)) = 0
        _FillEnd ("Fill End (UV Y)", Range(0,1)) = 1

        _Edge ("Smooth Edge Size", Range(0,0.1)) = 0.01
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "CanUseSpriteAtlas"="True"
        }

        Blend One OneMinusSrcAlpha
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
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;

            float _Fill;
            float _FillStart;
            float _FillEnd;
            float _Edge;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Normalize UV.y into 0–1 range inside the fill window
                float t = saturate((i.uv.y - _FillStart) / (_FillEnd - _FillStart));

                // Smooth fade at the fill boundary
                float mask = smoothstep(_Fill - _Edge, _Fill + _Edge, t);

                fixed4 col = tex2D(_MainTex, i.uv) * i.color;

                // Apply mask: 0 = fully visible, 1 = fully invisible
                col.a *= (1 - mask);

                // Optional: clip tiny alpha to avoid fuzzy borders
                clip(col.a - 0.001);

                return col;
            }
            ENDCG
        }
    }

}