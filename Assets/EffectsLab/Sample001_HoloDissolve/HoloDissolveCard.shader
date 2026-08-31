Shader "EffectsLab/HoloDissolveCard"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0.05, 0.12, 0.2, 1)
        _HoloColor ("Hologram Color", Color) = (0.2, 0.9, 1.0, 1)
        _EdgeColor ("Dissolve Edge", Color) = (1.0, 0.3, 0.8, 1)
        _Dissolve ("Dissolve", Range(0, 1)) = 0.35
        _EdgeWidth ("Edge Width", Range(0.001, 0.2)) = 0.055
        _NoiseScale ("Noise Scale", Range(1, 30)) = 9
        _NoiseSpeed ("Noise Speed", Range(0, 3)) = 0.55
        _ScanlineDensity ("Scanline Density", Range(5, 150)) = 55
        _ScanlineStrength ("Scanline Strength", Range(0, 1)) = 0.2
        _FresnelPower ("Fresnel Power", Range(0.5, 8)) = 3
        _Emission ("Emission", Range(0, 8)) = 2.2
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
            "RenderPipeline"="UniversalPipeline"
        }

        Pass
        {
            Name "ForwardUnlit"
            Tags { "LightMode"="UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float2 uv : TEXCOORD2;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _HoloColor;
                float4 _EdgeColor;
                float _Dissolve;
                float _EdgeWidth;
                float _NoiseScale;
                float _NoiseSpeed;
                float _ScanlineDensity;
                float _ScanlineStrength;
                float _FresnelPower;
                float _Emission;
            CBUFFER_END

            float hash21(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }

            float valueNoise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                f = f * f * (3.0 - 2.0 * f);

                float a = hash21(i);
                float b = hash21(i + float2(1, 0));
                float c = hash21(i + float2(0, 1));
                float d = hash21(i + float2(1, 1));

                return lerp(lerp(a, b, f.x), lerp(c, d, f.x), f.y);
            }

            float fbm(float2 p)
            {
                float n = 0.0;
                float amp = 0.5;
                [unroll] for (int i = 0; i < 4; i++)
                {
                    n += valueNoise(p) * amp;
                    p = p * 2.03 + 17.17;
                    amp *= 0.5;
                }
                return n;
            }

            Varyings vert(Attributes input)
            {
                Varyings output;
                VertexPositionInputs pos = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs nor = GetVertexNormalInputs(input.normalOS);
                output.positionHCS = pos.positionCS;
                output.positionWS = pos.positionWS;
                output.normalWS = nor.normalWS;
                output.uv = input.uv;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float2 noiseUV = input.uv * _NoiseScale;
                noiseUV += float2(_Time.y * _NoiseSpeed * 0.31, -_Time.y * _NoiseSpeed);
                float noise = fbm(noiseUV);

                float signedField = noise - _Dissolve;
                float alpha = smoothstep(-_EdgeWidth, 0.0, signedField);
                float edge = 1.0 - smoothstep(0.0, _EdgeWidth, abs(signedField));

                float3 viewDir = SafeNormalize(GetWorldSpaceViewDir(input.positionWS));
                float3 normalWS = SafeNormalize(input.normalWS);
                float fresnel = pow(1.0 - saturate(dot(normalWS, viewDir)), _FresnelPower);

                float scan = sin((input.uv.y + _Time.y * 0.16) * _ScanlineDensity * 6.2831853);
                scan = (scan * 0.5 + 0.5) * _ScanlineStrength;

                float hueShift = 0.5 + 0.5 * sin((input.uv.x + input.uv.y + _Time.y * 0.08) * 12.0);
                float3 holo = lerp(_HoloColor.rgb, _EdgeColor.rgb, hueShift * 0.28);
                float3 color = _BaseColor.rgb;
                color += holo * (fresnel * 1.2 + scan) * _Emission;
                color += _EdgeColor.rgb * edge * _Emission * 1.8;

                alpha *= _BaseColor.a;
                clip(alpha - 0.01);
                return half4(color, alpha);
            }
            ENDHLSL
        }
    }
}
