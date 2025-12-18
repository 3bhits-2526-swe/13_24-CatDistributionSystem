Shader "Hidden/Acerola/ASCIIEffect"
{
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

        #define AFX_PI 3.14159265359 

        // Properties set from C#
        float _Zoom;
        float2 _Offset;
        int _KernelSize;
        float _Sigma, _SigmaScale, _Tau, _Threshold, _DepthThreshold, _NormalThreshold, _DepthCutoff;
        bool _UseDepth, _UseNormals;

        // Textures
        TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
        TEXTURE2D(_LuminanceAsciiTex); SAMPLER(sampler_LuminanceAsciiTex);
        TEXTURE2D(_AsciiPingTex); SAMPLER(sampler_AsciiPingTex);
        TEXTURE2D(_AsciiDogTex); SAMPLER(sampler_AsciiDogTex);
        TEXTURE2D(_AsciiEdgesTex); SAMPLER(sampler_AsciiEdgesTex);
        TEXTURE2D(_NormalsDepthTex); SAMPLER(sampler_NormalsDepthTex);
        TEXTURE2D(_FinalASCIITex); SAMPLER(sampler_FinalASCIITex);

        struct Attributes {
            uint vertexID : SV_VertexID;
        };

        struct Varyings {
            float4 positionCS : SV_POSITION;
            float2 uv : TEXCOORD0;
        };

        // Standard URP Fullscreen Vertex Shader
        Varyings FullScreenVert(Attributes input) {
            Varyings output;
            output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);
            output.uv = GetFullScreenTriangleTexCoord(input.vertexID);
            return output;
        }

        float2 transformUV(float2 uv) {
            float2 zoomUV = uv * 2 - 1;
            zoomUV += float2(-_Offset.x, _Offset.y) * 2;
            zoomUV *= _Zoom;
            return zoomUV * 0.5f + 0.5f;
        }

        float gaussian(float sigma, float pos) {
            return exp(-(pos * pos) / (2.0f * sigma * sigma));
        }

        // --- Fragment Functions ---

        float PS_Luminance(Varyings input) : SV_TARGET {
            float3 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, transformUV(input.uv)).rgb;
            return Luminance(saturate(color));
        }

        float4 PS_Downscale(Varyings input) : SV_TARGET {
            float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, transformUV(input.uv));
            return float4(col.rgb, Luminance(col.rgb));
        }

        float4 PS_HorizontalBlur(Varyings input) : SV_TARGET {
            float2 texelSize = 1.0 / _ScreenParams.xy;
            float2 blur = 0;
            float2 kernelSum = 0;
            for (int x = -_KernelSize; x <= _KernelSize; ++x) {
                float luminance = SAMPLE_TEXTURE2D(_LuminanceAsciiTex, sampler_LuminanceAsciiTex, input.uv + float2(x, 0) * texelSize).r;
                float2 gauss = float2(gaussian(_Sigma, x), gaussian(_Sigma * _SigmaScale, x));
                blur += luminance * gauss;
                kernelSum += gauss;
            }
            return float4(blur / kernelSum, 0, 0);
        }

        float PS_VerticalBlurAndDifference(Varyings input) : SV_TARGET {
            float2 texelSize = 1.0 / _ScreenParams.xy;
            float2 blur = 0;
            float2 kernelSum = 0;
            for (int y = -_KernelSize; y <= _KernelSize; ++y) {
                float2 luminance = SAMPLE_TEXTURE2D(_AsciiPingTex, sampler_AsciiPingTex, input.uv + float2(0, y) * texelSize).rg;
                float2 gauss = float2(gaussian(_Sigma, y), gaussian(_Sigma * _SigmaScale, y));
                blur += luminance * gauss;
                kernelSum += gauss;
            }
            blur /= kernelSum;
            float D = (blur.x - _Tau * blur.y);
            return (D >= _Threshold) ? 1 : 0;
        }

        float4 PS_CalculateNormals(Varyings input) : SV_TARGET {
            float3 texelSize = float3(1.0 / _ScreenParams.x, 1.0 / _ScreenParams.y, 0.0);
            float centerDepth = LinearEyeDepth(SampleSceneDepth(transformUV(input.uv)), _ZBufferParams);
            float northDepth = LinearEyeDepth(SampleSceneDepth(transformUV(input.uv - texelSize.zy)), _ZBufferParams);
            float eastDepth = LinearEyeDepth(SampleSceneDepth(transformUV(input.uv + texelSize.xz)), _ZBufferParams);
            float3 vertCenter = float3(input.uv - 0.5, 1) * centerDepth;
            float3 vertNorth = float3((input.uv - texelSize.zy) - 0.5, 1) * northDepth;
            float3 vertEast = float3((input.uv + texelSize.xz) - 0.5, 1) * eastDepth;
            return float4(normalize(cross(vertCenter - vertNorth, vertCenter - vertEast)), centerDepth);
        }

        float PS_EdgeDetect(Varyings input) : SV_TARGET {
            float2 texelSize = 1.0 / _ScreenParams.xy;
            float4 c = SAMPLE_TEXTURE2D(_NormalsDepthTex, sampler_NormalsDepthTex, input.uv);
            float4 w = SAMPLE_TEXTURE2D(_NormalsDepthTex, sampler_NormalsDepthTex, input.uv + float2(-1, 0) * texelSize);
            float4 e = SAMPLE_TEXTURE2D(_NormalsDepthTex, sampler_NormalsDepthTex, input.uv + float2(1, 0) * texelSize);
            float4 n = SAMPLE_TEXTURE2D(_NormalsDepthTex, sampler_NormalsDepthTex, input.uv + float2(0, -1) * texelSize);
            float4 s = SAMPLE_TEXTURE2D(_NormalsDepthTex, sampler_NormalsDepthTex, input.uv + float2(0, 1) * texelSize);

            float depthSum = abs(w.w - c.w) + abs(e.w - c.w) + abs(n.w - c.w) + abs(s.w - c.w);
            float output = (_UseDepth && depthSum > _DepthThreshold) ? 1.0 : 0.0;
            float3 normalSum = abs(w.rgb - c.rgb) + abs(e.rgb - c.rgb) + abs(n.rgb - c.rgb) + abs(s.rgb - c.rgb);
            if (_UseNormals && dot(normalSum, 1) > _NormalThreshold) output = 1.0;

            float D = SAMPLE_TEXTURE2D(_AsciiDogTex, sampler_AsciiDogTex, input.uv).r;
            return saturate(abs(D - output));
        }

        float4 PS_HorizontalSobel(Varyings input) : SV_TARGET {
            float2 texelSize = 1.0 / _ScreenParams.xy;
            float lum1 = SAMPLE_TEXTURE2D(_AsciiEdgesTex, sampler_AsciiEdgesTex, input.uv - float2(1, 0) * texelSize).r;
            float lum2 = SAMPLE_TEXTURE2D(_AsciiEdgesTex, sampler_AsciiEdgesTex, input.uv).r;
            float lum3 = SAMPLE_TEXTURE2D(_AsciiEdgesTex, sampler_AsciiEdgesTex, input.uv + float2(1, 0) * texelSize).r;
            return float4(3 * lum1 - 3 * lum3, 3 * lum1 + 10 * lum2 + 3 * lum3, 0, 0);
        }

        float2 PS_VerticalSobel(Varyings input) : SV_TARGET {
            float2 texelSize = 1.0 / _ScreenParams.xy;
            float2 grad1 = SAMPLE_TEXTURE2D(_AsciiPingTex, sampler_AsciiPingTex, input.uv - float2(0, 1) * texelSize).rg;
            float2 grad2 = SAMPLE_TEXTURE2D(_AsciiPingTex, sampler_AsciiPingTex, input.uv).rg;
            float2 grad3 = SAMPLE_TEXTURE2D(_AsciiPingTex, sampler_AsciiPingTex, input.uv + float2(0, 1) * texelSize).rg;
            float2 G = float2(3 * grad1.x + 10 * grad2.x + 3 * grad3.x, 3 * grad1.y - 3 * grad3.y);
            float theta = atan2(normalize(G).y, normalize(G).x);
            float depth = LinearEyeDepth(SampleSceneDepth(transformUV(input.uv)), _ZBufferParams);
            if (_DepthCutoff > 0.0f && depth > _DepthCutoff) return float2(0, 0);
            return float2(theta, 1.0);
        }

        float4 PS_EndPass(Varyings input) : SV_TARGET {
            return SAMPLE_TEXTURE2D(_FinalASCIITex, sampler_FinalASCIITex, input.uv);
        }
        ENDHLSL

        Pass { Name "Luminance" HLSLPROGRAM #pragma vertex FullScreenVert #pragma fragment PS_Luminance ENDHLSL }
        Pass { Name "Downscale" HLSLPROGRAM #pragma vertex FullScreenVert #pragma fragment PS_Downscale ENDHLSL }
        Pass { Name "HBlur" HLSLPROGRAM #pragma vertex FullScreenVert #pragma fragment PS_HorizontalBlur ENDHLSL }
        Pass { Name "VBlurDiff" HLSLPROGRAM #pragma vertex FullScreenVert #pragma fragment PS_VerticalBlurAndDifference ENDHLSL }
        Pass { Name "Normals" HLSLPROGRAM #pragma vertex FullScreenVert #pragma fragment PS_CalculateNormals ENDHLSL }
        Pass { Name "EdgeDetect" HLSLPROGRAM #pragma vertex FullScreenVert #pragma fragment PS_EdgeDetect ENDHLSL }
        Pass { Name "HSobel" HLSLPROGRAM #pragma vertex FullScreenVert #pragma fragment PS_HorizontalSobel ENDHLSL }
        Pass { Name "VSobel" HLSLPROGRAM #pragma vertex FullScreenVert #pragma fragment PS_VerticalSobel ENDHLSL }
        Pass { Name "FinalBlit" HLSLPROGRAM #pragma vertex FullScreenVert #pragma fragment PS_EndPass ENDHLSL }
    }
}