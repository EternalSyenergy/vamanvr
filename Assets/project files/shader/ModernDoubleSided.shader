Shader "Custom/ModernDoubleSided"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        
        _Metallic("Metallic", Range(0,1)) = 0.0
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        
        [Normal] _BumpMap ("Normal Map", 2D) = "bump" {}
        _BumpScale ("Normal Scale", Float) = 1.0

        _OcclusionTex ("Occlusion (G)", 2D) = "white" {}
        _OcclusionStrength ("Occlusion Strength", Range(0.0, 1.0)) = 1.0

        [HDR] _EmissionColor ("Emission Color", Color) = (0,0,0)
        _EmissionMap ("Emission Map", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 300

        // Disables backface culling to make the mesh double-sided
        Cull Off

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0
        #pragma multi_compile_instancing

        sampler2D _MainTex;
        sampler2D _BumpMap;
        sampler2D _OcclusionTex;
        sampler2D _EmissionMap;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_BumpMap;
            float2 uv_OcclusionTex;
            float2 uv_EmissionMap;
            float facing : VFACE; // Handles normal direction for double-sided meshes
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        half _BumpScale;
        half _OcclusionStrength;
        fixed4 _EmissionColor;

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo & Color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;

            // Metallic & Smoothness
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;

            // Normal Map with Scale adjustment
            half3 normalTex = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
            normalTex.xy *= _BumpScale;
            o.Normal = normalTex;

            // Ambient Occlusion
            half occ = tex2D(_OcclusionTex, IN.uv_OcclusionTex).g;
            o.Occlusion = LerpOneTo(occ, _OcclusionStrength);

            // Emission
            fixed3 em = tex2D(_EmissionMap, IN.uv_EmissionMap).rgb * _EmissionColor.rgb;
            o.Emission = em;

            // Automatically flip normals for back-facing triangles so lighting doesn't look dark or inverted
            if (IN.facing < 0.5)
            {
                o.Normal = -o.Normal;
            }

            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Standard"
}