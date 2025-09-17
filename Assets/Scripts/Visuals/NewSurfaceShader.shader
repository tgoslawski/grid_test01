Shader "Custom/WaterReflection"
{
    Properties
    {
        _ReflectionTex ("Reflection Texture", 2D) = "white" {}
        _NoiseTex1 ("Noise Texture 1", 2D) = "white" {}
        _NoiseTex2 ("Noise Texture 2", 2D) = "white" {}
        _TintColor ("Water Tint", Color) = (0.2,0.5,0.7, 1)
        _TintAmount ("Tint Amount", Range(0,1)) = 0.5
        _DistortionStrength ("Distortion Strength", Range(0,1)) = 0.05
        _SpecThreshold ("Specular Threshold", Range(0,1)) = 0.9
        _EdgeMask ("Edge Mask Texture", 2D) = "white" {}
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

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                // If you want world position for edge masks, etc.
                o.worldPos = v.vertex.xy; // or some other mapping
                return o;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                float2 uv = IN.uv;

                // Distortion via noise
                float2 n1 = tex2D(_NoiseTex1, uv * 1 + _Time.yz * 0.1).xy;
                float2 n2 = tex2D(_NoiseTex2, uv * 1 + _Time.zw * 0.1).xy;
                float2 noiseCombined = (n1 - 0.5 + n2 - 0.5); // center around zero
                float2 offset = noiseCombined * _DistortionStrength;
                // Maybe only apply on X
                offset.y = 0;

                // Sample reflection with distortion
                float2 reflectUV = uv + offset;
                fixed4 refl = tex2D(_ReflectionTex, reflectUV);

                // Tint
                fixed4 tinted = lerp(refl, _TintColor, _TintAmount);

                // Specular highlights
                float noiseSum = (n1 + n2).r; // maybe use one channel
                float spec = step(_SpecThreshold, noiseSum);
                // Soft spec
                float soft = smoothstep(0.0, _SpecThreshold, noiseSum);
                tinted.rgb += spec * 1.0 + soft * 0.15; // tweak

                // Edge ripples
                fixed4 edge = tex2D(_EdgeMask, uv);
                // Let’s suppose edge.r is mask
                float ripple = sin(IN.worldPos.x * _RippleFreq + _Time.y * _RippleSpeed);
                float fade = saturate(1.0 - (/*distance to edge*/ 0.5) / _RippleFadeDist);
                float rippleEffect = ripple * edge.r * fade;
                tinted.rgb += rippleEffect * 0.2;

                return tinted;
            }
            ENDCG
        }
    }
}
