Shader "Custom/WaterReflectionNatural"
{
    Properties
    {
        _ReflectionTex ("Reflection Texture", 2D) = "white" {}
        _NoiseTex1 ("Noise Texture 1", 2D) = "white" {}
        _NoiseTex2 ("Noise Texture 2", 2D) = "white" {}
        _EdgeMask ("Edge Mask Texture", 2D) = "white" {}
        _TintColor ("Water Tint", Color) = (0.2,0.5,0.7, 1)
        _TintAmount ("Tint Amount", Range(0,1)) = 0.5
        _DistortionStrength ("Distortion Strength", Range(0,1)) = 0.05
        _SpecThreshold ("Specular Threshold", Range(0,1)) = 0.9
        _RippleSpeed ("Ripple Speed", Float) = 1.0
        _RippleFreq ("Ripple Frequency", Float) = 10.0
        _RippleFadeDist ("Ripple Fade Distance", Float) = 0.5
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _ReflectionTex;
            sampler2D _NoiseTex1;
            sampler2D _NoiseTex2;
            sampler2D _EdgeMask;
            float4 _TintColor;
            float _TintAmount;
            float _DistortionStrength;
            float _SpecThreshold;
            float _RippleSpeed;
            float _RippleFreq;
            float _RippleFadeDist;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float2 worldPos : TEXCOORD1;
            };

            // Small UV rotation helper
            float2 RotateUV(float2 uv, float angle)
            {
                float s = sin(angle);
                float c = cos(angle);
                float2x2 rot = float2x2(c, -s, s, c);
                return mul(rot, uv - 0.5) + 0.5;
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldPos = v.vertex.xy; 
                return o;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                float2 uv = IN.uv;

                // --- Noise layer 1 ---
                float2 uv1 = uv * float2(1.2, 0.9) + float2(_Time.y * 0.1, 0);
                uv1 = RotateUV(uv1, 0.3);
                float2 n1 = tex2D(_NoiseTex1, uv1).rg - 0.5;

                // --- Noise layer 2 ---
                float2 uv2 = uv * float2(0.8, 1.1) + float2(0, _Time.y * 0.07);
                uv2 = RotateUV(uv2, 1.0);
                float2 n2 = tex2D(_NoiseTex2, uv2).rg - 0.5;

                // Combine noise layers
                float2 noiseCombined = n1 + n2;
                float2 offset = noiseCombined * _DistortionStrength;
                offset.y *= 0.5; // reduce vertical distortion

                // Reflection sample with distortion
                float2 reflectUV = uv + offset;
                fixed4 refl = tex2D(_ReflectionTex, reflectUV);

                // Tint
                fixed4 tinted = lerp(refl, _TintColor, _TintAmount);

                // Specular highlights
                float noiseSum = (n1.x + n2.x + n1.y + n2.y) * 0.25 + 0.5;
                float spec = step(_SpecThreshold, noiseSum);
                float soft = smoothstep(0.0, _SpecThreshold, noiseSum);
                tinted.rgb += spec * 0.6 + soft * 0.15;

                // Edge ripples
                fixed4 edge = tex2D(_EdgeMask, uv);
                float ripple = sin(IN.worldPos.x * _RippleFreq + _Time.y * _RippleSpeed);
                float fade = saturate(1.0 - (0.5 / _RippleFadeDist));
                float rippleEffect = ripple * edge.r * fade;
                tinted.rgb += rippleEffect * 0.2;

                return tinted;
            }
            ENDCG
        }
    }
}
