Shader "Unlit/ComparisonShader"
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

                if (rc.a == 0.0)
                    discard;
                
                return oc == rc 
                    ? fixed4(1.0, 0.0, 0.0, 1.0)
                    : fixed4(0.0, 0.0, 1.0, 1.0);

                float normalized = sqrt(pow(oc.r - rc.r, 2) + pow(oc.g - rc.g, 2) + pow(oc.b - rc.b, 2));
                float normalized_ds = normalized / sqrt(3.0);

                return lerp(fixed4(1.0, 0.0, 0.0, 1.0), fixed4(0.0, 0.0, 1.0, 1.0), normalized_ds);
            }
            ENDCG
        }
    }
}
