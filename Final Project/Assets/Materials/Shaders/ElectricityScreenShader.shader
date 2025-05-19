Shader "UI/ElectricityEffectURP"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Speed ("Scroll Speed", Float) = 1.0
        _DistortionStrength ("Distortion Strength", Range(0,0.1)) = 0.03
        _FlickerSpeed ("Flicker Speed", Float) = 10.0
        _FlickerIntensity ("Flicker Intensity", Range(0,1)) = 0.5
        _Alpha ("Alpha", Range(0,1)) = 1.0
        _Tint ("Tint Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            Name "ElectricityEffectUI"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Speed;
            float _DistortionStrength;
            float _FlickerSpeed;
            float _FlickerIntensity;
            float _Alpha;
            float4 _Tint;

            float rand(float2 co)
            {
                return frac(sin(dot(co.xy ,float2(12.9898,78.233))) * 43758.5453);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.position = TransformObjectToHClip(v.vertex.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                float time = _Time.y * _Speed;

                // Distort UVs to simulate flickering arcs
                float distortion = sin(i.uv.y * 50 + time * 20) * _DistortionStrength;
                distortion += (rand(i.uv * 1000 + time) - 0.5) * _DistortionStrength;

                float2 uvDistorted = i.uv + float2(distortion, 0);

                // Sample texture, includes alpha channel
                half4 texColor = tex2D(_MainTex, uvDistorted);

                // Apply tint and flicker
                half flicker = (sin(time * _FlickerSpeed + i.uv.x * 100) * 0.5 + 0.5) * _FlickerIntensity;

                // Final alpha is texture alpha * flicker * global alpha
                texColor.a *= lerp(1.0, flicker, flicker) * _Alpha;

                // Apply tint color (including alpha)
                texColor.rgb *= _Tint.rgb;
                texColor.a *= _Tint.a;

                // Return final color
                return texColor;
            }
            ENDHLSL
        }
    }
}
