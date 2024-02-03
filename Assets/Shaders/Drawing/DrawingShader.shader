Shader "Hidden/TestShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex       : POSITION;
                float2 uv           : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv           : TEXCOORD0;
                float4 vertex       : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            sampler2D _BrushTex;
            float4 _MainTex_ST;
            float4 _PaintPos;
            float4 _PaintColor;
            float _BrushSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                float3 pixelPos = float3(i.uv * _MainTex_TexelSize.zw, 0.);
                _PaintPos.z = 0.;

                if (distance(_PaintPos, pixelPos) <= _BrushSize)
                    col = _PaintColor;

                return col;
            }
            ENDCG
        }
    }
}
