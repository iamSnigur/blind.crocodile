Shader "Hidden/ComparisonShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
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

            float4 _MainTex_ST;
            sampler2D _MainTex;
            sampler2D _originalTex;

            // 0                  0.5                1    
            // rgb(253, 231, 37), rgb(33, 145, 140), rgb(68, 1, 84) | viridis

            static float4 _lowColor = float4(5, 235, 89, 255) / float4(255, 255, 255, 255);
            static float4 _midColor = float4(239, 238, 92, 255) / float4(255, 255, 255, 255);
            static float4 _hightColor = float4(252, 78, 77, 255) / float4(255, 255, 255, 255);

            fixed4 ToHeatmap(float diff)
            {
                diff = clamp(diff, 0, 1);

                return diff < 0.5
                    ? lerp(_lowColor, _midColor, diff * 2.0)
                    : lerp(_midColor, _hightColor, (diff - 0.5) * 2.0);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 oc = tex2D(_originalTex, i.uv);
                fixed4 rc = tex2D(_MainTex, i.uv);

                if (oc.a == 0.0 || rc.a == 0.0)
                    discard;

                float diff = length(rc.rgb - oc.rgb);
                
                return ToHeatmap(diff);               
            }
            ENDCG
        }
    }
}
