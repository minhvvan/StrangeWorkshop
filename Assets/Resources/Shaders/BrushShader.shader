Shader "Custom/BrushShader"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _NoiseTex ("Brush Texture", 2D) = "white" {} 
        _LightThreshold ("Light Threshold", Range(0,1)) = 0.5
        _LightIntensity ("Light Intensity", Range(0,1)) = 0.01
        _ShadowThreshold ("Shadow Threshold", Range(0,1)) = 0.5 
        _ShadowIntensity ("Shadow Intensity", Range(0,1)) = 0.3 
    }
    SubShader
    {
        Tags 
        {
            "RenderType"="Opaque"
            "Queue"="Geometry"
            "LightMode" = "UniversalForward"
        }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD3;
                float4 shadowCoord : TEXCOORD4;
            };

            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            TEXTURE2D(_NoiseTex); SAMPLER(sampler_NoiseTex);
            float _LightThreshold;
            float _LightIntensity;
            float _ShadowThreshold;
            float _ShadowIntensity;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex);
                o.uv = v.uv;
                o.worldNormal = TransformObjectToWorldNormal(v.normal);
                o.viewDir = normalize(GetWorldSpaceViewDir(v.vertex));
                o.worldPos = TransformObjectToWorld(v.vertex);
                o.shadowCoord = TransformWorldToShadowCoord(o.worldPos);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                // 📌 메인 텍스처 적용
                half4 baseColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

                // 📌 붓 터치 효과 (노이즈 텍스처 적용)
                half4 noise = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, i.uv);
                baseColor.rgb *= lerp(1, noise.r, 0.5); 

                // 📌 유니티 URP의 메인 광원 정보 가져오기
                Light mainLight = GetMainLight();
                float3 lightDir = normalize(mainLight.direction);

                // 📌 URP 그림자 적용 (다른 오브젝트 그림자까지 포함)
                float shadowAtten = mainLight.shadowAttenuation;

                // 📌 광원 방향과 노멀 벡터를 이용한 기본 광원 계산
                float lightIntensity = saturate(dot(i.worldNormal, lightDir));

                i.shadowCoord = TransformWorldToShadowCoord(i.worldPos);
                //shadow
                float shadow = MainLightRealtimeShadow(i.shadowCoord);
                float shadowFactor = step(_ShadowThreshold, lightIntensity * shadow);

                // 📌 어두운 부분 그림자 효과
                half3 shadowColor = baseColor.rgb * (1.0 - _ShadowIntensity);
                half3 finalColor = lerp(shadowColor, baseColor.rgb, shadowFactor);

                // 📌 밝은 부분 Step 처리 (광원 강도가 임계값 이상이면 적용)
                float lightStep = step(_LightThreshold, lightIntensity);
                finalColor = lerp(finalColor, finalColor + _LightIntensity, lightStep);

                return half4(finalColor, 1.0);
            }
            ENDHLSL
        }

        Pass
        {
            Name "ShadowCaster"
            Tags
            {
                "LightMode" = "ShadowCaster"
            }

            // -------------------------------------
            // Render State Commands
            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull[_Cull]

            HLSLPROGRAM
            #pragma target 2.0

            // -------------------------------------
            // Shader Stages
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local _ALPHATEST_ON
            #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"

            // -------------------------------------
            // Universal Pipeline keywords

            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile _ LOD_FADE_CROSSFADE

            // This is used during shadow map generation to differentiate between directional and punctual light shadows, as they use different formulas to apply Normal Bias
            #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW

            // -------------------------------------
            // Includes
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
            ENDHLSL
        }
        

        // 📌 깊이 패스 (DepthOnly)
        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode" = "DepthOnly" }

            ZWrite On
            ColorMask 0
            Cull Front

            HLSLPROGRAM
            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
            ENDHLSL
        }
    }
}