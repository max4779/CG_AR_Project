Shader "Custom/HologramURP"
{
    Properties
    {
        _BaseColor     ("Base Color", Color) = (1, 0.6, 0.1, 1)
        _RimColor      ("Rim Color",  Color) = (1, 0.8, 0.4, 1)
        _RimPower      ("Rim Power",  Range(0.5, 8)) = 3
        _RimIntensity  ("Rim Intensity", Range(0, 5)) = 2

        _LineColor     ("Line Color", Color) = (1, 0.7, 0.2, 1)
        _LineTiling    ("Line Tiling", Range(1, 200)) = 40
        _LineSpeed     ("Line Speed", Range(-5, 5)) = 1

        _Alpha         ("Alpha", Range(0, 1)) = 0.6

        _MainTex       ("MainTex (optional)", 2D) = "white" {}
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Back
        ZWrite Off

        Pass
        {
            Name "ForwardUnlit"
            Tags{"LightMode" = "UniversalForward"}

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 normalWS    : TEXCOORD0;
                float2 uv          : TEXCOORD1;
                float3 viewDirWS   : TEXCOORD2;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _RimColor;
                float  _RimPower;
                float  _RimIntensity;

                float4 _LineColor;
                float  _LineTiling;
                float  _LineSpeed;

                float  _Alpha;

                float4 _MainTex_ST;
            CBUFFER_END

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            Varyings vert (Attributes IN)
            {
                Varyings OUT;

                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                float3 positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);

                float3 cameraPosWS = GetCameraPositionWS();
                OUT.viewDirWS = normalize(cameraPosWS - positionWS);

                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                float3 N = normalize(IN.normalWS);
                float3 V = normalize(IN.viewDirWS);

                // ----- Rim (Fresnel) -----
                float NdotV = saturate(dot(N, V));
                float fresnel = pow(1.0 - NdotV, _RimPower);
                float rimTerm = fresnel * _RimIntensity;

                // ----- Scan Line -----
                // 세로 방향으로 움직이는 밴드
                float t = _Time.y * _LineSpeed;
                float linePhase = frac(IN.uv.y * _LineTiling + t);
                float lineMask  = smoothstep(0.45, 0.55, linePhase);

                // ----- 기본 색상 (원하면 텍스처 사용) -----
                float4 baseTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                float3 baseCol = _BaseColor.rgb * baseTex.rgb;

                // Emission 색 섞기
                float3 rimCol   = _RimColor.rgb  * rimTerm;
                float3 lineCol  = _LineColor.rgb * lineMask;

                float3 emission = baseCol * 0.25 + rimCol + lineCol;

                // Alpha
                float alpha = saturate(_Alpha * (fresnel * 0.6 + lineMask * 0.8));

                return half4(emission, alpha);
            }
            ENDHLSL
        }
    }

    FallBack Off
}