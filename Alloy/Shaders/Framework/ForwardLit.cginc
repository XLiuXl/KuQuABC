// Alloy Physical Shader Framework
// Copyright 2013-2017 RUST LLC.
// http://www.alloy.rustltd.com/

///////////////////////////////////////////////////////////////////////////////
/// @file ForwardLit.cginc
/// @brief Lit forward passes uber-header.
///////////////////////////////////////////////////////////////////////////////

#ifndef ALLOY_SHADERS_FRAMEWORK_FORWARD_LIT_CGINC
#define ALLOY_SHADERS_FRAMEWORK_FORWARD_LIT_CGINC

#include "AutoLight.cginc"

#ifdef A_HAS_LIGHTING_CALLBACKS
    #ifdef A_DIRECT_PASS
        #define A_DIRECT_ON
    #endif

    #ifdef A_INDIRECT_PASS
        #define A_INDIRECT_ON
    #endif

    #if defined(A_DIRECT_ON) || defined(A_INDIRECT_ON)
        #define A_LIGHTING_ON
    #endif
#endif

// NOTE: Custom macro to skip calculations and remove dependency on o.pos!
#if !defined(SHADOWS_SCREEN) || defined(UNITY_NO_SCREENSPACE_SHADOWS)
    #define A_FORWARD_TRANSFER_SHADOW(a, v) UNITY_TRANSFER_SHADOW(a, v.uv1)
#else
    #define A_COMPUTE_VERTEX_SCREEN_UV
    #define A_FORWARD_TRANSFER_SHADOW(a, v) a._ShadowCoord = unityShadowCoord4(a.fogCoord.y, a.fogCoord.z, 0.0, a.fogCoord.w);
#endif

#if defined(A_INDIRECT_ON) && defined(A_DIRECT_ON)
    #define A_FORWARD_TEXCOORD0 half4 giData : TEXCOORD0;
    #define A_FORWARD_TEXCOORD1 UNITY_SHADOW_COORDS(1)
#elif defined(A_INDIRECT_ON)
    #define A_FORWARD_TEXCOORD0 half4 giData : TEXCOORD0;

    #ifdef A_SHADOW_MASK_RT_ON
        #define A_FORWARD_TEXCOORD1 UNITY_SHADOW_COORDS(1)
    #endif
#elif defined(A_DIRECT_ON)
    #define A_FORWARD_TEXCOORD0 UNITY_SHADOW_COORDS(0)
    
    // Reduce v2f transfer cost for pure directional lights.
    #ifndef DIRECTIONAL
        #define A_FORWARD_TEXCOORD1 unityShadowCoord4 lightCoord : TEXCOORD1;

        #ifndef USING_DIRECTIONAL_LIGHT
            #define A_FORWARD_TEXCOORD2 float4 lightVectorRange : TEXCOORD2;
        #endif
    #endif
#endif

#include "Assets/Alloy/Shaders/Framework/Forward.cginc"
#include "Assets/Alloy/Shaders/Framework/Unity.cginc"
#include "Assets/Alloy/Shaders/Framework/Utility.cginc"

#include "HLSLSupport.cginc"
#include "UnityCG.cginc"
#include "UnityGlobalIllumination.cginc"
#include "UnityImageBasedLighting.cginc"
#include "UnityLightingCommon.cginc"
#include "UnityShaderVariables.cginc"
#include "UnityShadowLibrary.cginc"
#include "UnityStandardUtils.cginc"

/// Transfers the per-vertex lightmapping or SH data to the fragment shader.
/// @param[in,out]  i   Vertex to fragment transfer data.
/// @param[in]      v   Vertex input data.
void aVertexGi(
    inout AVertexToFragment i,
    AVertex v)
{
#ifdef A_INDIRECT_ON
    #ifdef LIGHTMAP_ON
        i.giData.xy = v.uv1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
        i.giData.zw = 0.0h;
    #elif UNITY_SHOULD_SAMPLE_SH
        // Add approximated illumination from non-important point lights
        half3 normalWorld = i.normalWorld.xyz;

        #ifdef VERTEXLIGHT_ON
            i.giData.rgb = aShade4PointLights(i.positionWorldAndViewDepth.xyz, normalWorld);
        #endif

        i.giData.rgb = ShadeSHPerVertex(normalWorld, i.giData.rgb);
    #endif

    #ifdef DYNAMICLIGHTMAP_ON
        i.giData.zw = v.uv2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
    #endif
#endif
}

/// Populates a UnityGI descriptor in the fragment shader.
/// @param  i       Vertex to fragment transfer data.
/// @param  s       Material surface data.
/// @param  shadow  Forward Base directional light shadow.
/// @return         Initialized UnityGI descriptor.
UnityGI aFragmentGi(
    AVertexToFragment i,
    ASurface s,
    half shadow)
{
    UnityGI gi;
    UNITY_INITIALIZE_OUTPUT(UnityGI, gi);

#ifdef A_INDIRECT_ON
    UnityGIInput d;

    UNITY_INITIALIZE_OUTPUT(UnityGIInput, d);
    d.worldPos = s.positionWorld;
    d.worldViewDir = s.viewDirWorld; // ???
    d.atten = shadow;

    #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
        d.ambient = 0;
        d.lightmapUV = i.giData;
    #else
        d.ambient = i.giData.rgb;
        d.lightmapUV = 0;
    #endif

    d.probeHDR[0] = unity_SpecCube0_HDR;
    d.probeHDR[1] = unity_SpecCube1_HDR;

    #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
        d.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
    #endif

    #ifdef UNITY_SPECCUBE_BOX_PROJECTION
        d.boxMax[0] = unity_SpecCube0_BoxMax;
        d.probePosition[0] = unity_SpecCube0_ProbePosition;
        d.boxMax[1] = unity_SpecCube1_BoxMax;
        d.boxMin[1] = unity_SpecCube1_BoxMin;
        d.probePosition[1] = unity_SpecCube1_ProbePosition;
    #endif

    // So we can extract shadow with baked occlusion.
    #ifdef HANDLE_SHADOWS_BLENDING_IN_GI
        d.light.color = A_WHITE;
    #endif

    // Pass 1.0 for occlusion so we can apply it later in indirect().  
    gi = UnityGI_Base(d, 1.0h, s.ambientNormalWorld);

    #ifdef A_REFLECTION_PROBES_ON
        Unity_GlossyEnvironmentData g;

        g.reflUVW = s.reflectionVectorWorld;
        g.roughness = s.roughness;
        gi.indirect.specular = UnityGI_IndirectSpecular(d, 1.0h, s.normalWorld, g);
    #endif
#endif

    return gi;
}

/// Transfers the per-vertex surface data to the pixel shader.
/// @param[in,out]  v       Vertex input data.
/// @param[out]     o       Vertex to fragment transfer data.
/// @param[out]     opos    Clip space position.
void aForwardLitVertex(
    inout AVertex v,
    out AVertexToFragment o,
    out float4 opos)
{
    aForwardVertex(v, o, opos);

#ifdef A_INDIRECT_ON
    aVertexGi(o, v);
#endif

#ifdef A_DIRECT_ON
    A_FORWARD_TRANSFER_SHADOW(o, v);

    #ifndef DIRECTIONAL
        o.lightCoord = mul(unity_WorldToLight, unityShadowCoord4(o.positionWorldAndViewDepth.xyz, 1.0f));

        #ifndef USING_DIRECTIONAL_LIGHT
            o.lightVectorRange = UnityWorldSpaceLightDir(o.positionWorldAndViewDepth.xyz).xyzz;

            // Light range = |light-space light vector| / |world-space light vector|
            // This works because the light vector's length is the same in both world
            // and light space, but it's scaled by the light range in light space.
            // cf http://forum.unity3d.com/threads/get-the-range-of-a-point-light-in-forward-add-mode.213430/#post-1433291
            o.lightVectorRange.w = length(o.lightVectorRange.xyz) * rsqrt(dot(o.lightCoord.xyz, o.lightCoord.xyz));
        #endif
    #endif
#endif
}

/// Calculates forward lighting from surface and vertex data.
/// @param  i           Vertex to fragment transfer data.
/// @param  facingSign  Sign of front/back facing direction.
/// @return             Forward direct and indirect lighting.
half4 aForwardLitColor(
    AVertexToFragment i,
    half facingSign)
{
    ASurface s = aForwardSurfaceShader(i, facingSign);
    half3 illum = 0.0h;

#ifdef A_LIGHTING_ON
    half shadow = UNITY_SHADOW_ATTENUATION(i, s.positionWorld);

    #ifdef A_INDIRECT_ON
        UnityGI gi = aFragmentGi(i, s, shadow);

        illum = aUnityIndirectLighting(gi, s);

        // Extract shadow with combined baked occlusion.
        #ifdef HANDLE_SHADOWS_BLENDING_IN_GI
            shadow = gi.light.color.g;
        #endif
    #endif

    #if defined(A_DIRECT_ON)
        // Reduce v2f transfer cost for pure directional lights.
        #if defined(DIRECTIONAL)
            illum += aUnityDirectLighting(s, shadow, _WorldSpaceLightPos0, A_ZERO4);
        #elif defined(DIRECTIONAL_COOKIE)
            illum += aUnityDirectLighting(s, shadow, _WorldSpaceLightPos0, i.lightCoord);
        #else
            illum += aUnityDirectLighting(s, shadow, i.lightVectorRange, i.lightCoord);
        #endif
    #endif
#endif

#if defined(A_BASE_PASS) && defined(A_EMISSIVE_COLOR_ON)
    illum += s.emissiveColor;
#endif

    return aForwardColor(s, illum);
}

/// Creates a G-Buffer from the provided surface data.
/// @param  i           Vertex to fragment transfer data.
/// @param  facingSign  Sign of front/back facing direction.
/// @return             G-buffer with surface data and ambient illumination.
AGbuffer aForwardLitGbuffer(
    AVertexToFragment i,
    half facingSign)
{
    AGbuffer gb;
    ASurface s = aForwardSurfaceShader(i, facingSign);

    UNITY_INITIALIZE_OUTPUT(AGbuffer, gb);

    half3 illum = 0.0h;

#ifdef A_INDIRECT_ON
    illum += aUnityIndirectLighting(aFragmentGi(i, s, 1.0h), s);
#endif

#ifdef A_EMISSIVE_COLOR_ON
    illum += s.emissiveColor;
#endif

#if defined(A_INDIRECT_ON) || defined(A_EMISSIVE_COLOR_ON)
    illum = aHdrClamp(illum);
#endif

    gb.target0 = half4(s.albedo, s.specularOcclusion);
    gb.target1 = half4(s.f0, 1.0h - s.roughness);
    gb.target2 = half4(s.normalWorld * 0.5h + 0.5h, s.materialType);
    gb.target3 = half4(illum, s.subsurface);
    
#ifndef UNITY_HDR_ON
    gb.target3.rgb = exp2(-gb.target3.rgb);
#endif

    // Baked direct lighting occlusion if any.
#ifdef A_SHADOW_MASK_RT_ON
    #ifndef A_LIGHTING_ON
        gb.target4 = A_ZERO4;
    #elif defined(A_INDIRECT_ON)
        gb.target4 = UnityGetRawBakedOcclusions(i.giData.xy, s.positionWorld);
    #endif
#endif

    aFinalGbuffer(gb, s);
    return gb;
}

#endif // ALLOY_SHADERS_FRAMEWORK_FORWARD_LIT_CGINC
