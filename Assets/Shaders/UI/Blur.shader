Shader "UI/Blur"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Size("Blur Radius", Range(0, 6)) = 3
    }

    SubShader
    {
        Tags { "Queue"="Transparent" }
        
        HLSLINCLUDE
        #define SampleGrabTexture(posX, posY) tex2Dproj( _GrabTexture, float4(i.grab_uv.x + _GrabTexture_TexelSize.x * posX, i.grab_uv.y + _GrabTexture_TexelSize.y * posY, i.grab_uv.z, i.grab_uv.w))
        ENDHLSL

        Cull Off
        ZTest Always
        ZWrite On
        Blend SrcAlpha OneMinusSrcAlpha

        GrabPass { }
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 grab_uv : TEXCOORD0;
                float2 uv : TEXCOORD1;
                float4 color : COLOR0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _GrabTexture;
            float4 _GrabTexture_TexelSize;
            int _Size;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
#if UNITY_UV_STARTS_AT_TOP
                float scale = -1;
#else
                float scale = 1;
#endif

                o.grab_uv.xy = (float2(o.vertex.x, o.vertex.y * scale) + o.vertex.w) * 0.5;
                o.grab_uv.zw = o.vertex.zw;

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;

                return o;
            }

            half4 frag(v2f i) : SV_TARGET
            {
                half4 main = tex2D(_MainTex, i.uv);
                half4 result = SampleGrabTexture(0, 0);

                for (int range = 1; range <= _Size; range++)
                {
                    result += SampleGrabTexture(0, range);
                    result += SampleGrabTexture(0, -range);
                }
                result /= _Size * 2 + 1;
                result.a = main.a;
                return result * i.color;
            }
            ENDHLSL
        }

        GrabPass { }
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 grab_uv : TEXCOORD0;
                float2 uv : TEXCOORD1;
                float4 color : COLOR0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _GrabTexture;
            float4 _GrabTexture_TexelSize;
            int _Size;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
#if UNITY_UV_STARTS_AT_TOP
                float scale = -1;
#else
                float scale = 1;
#endif

                o.grab_uv.xy = (float2(o.vertex.x, o.vertex.y * scale) + o.vertex.w) * 0.5;
                o.grab_uv.zw = o.vertex.zw;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;

                return o;
            }

            half4 frag(v2f i) : SV_TARGET
            {
                half4 main = tex2D(_MainTex, i.uv);
                half4 result = SampleGrabTexture(0, 0);

                for (int range = 1; range <= _Size; range++)
                {
                    result += SampleGrabTexture(range, 0);
                    result += SampleGrabTexture(-range, 0);
                }
                result /= _Size * 2 + 1;
                result.a = main.a;
                return result * i.color;
            }
            ENDHLSL
        }
    }
}
