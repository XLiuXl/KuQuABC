// Alloy Physical Shader Framework
// Copyright 2013-2017 RUST LLC.
// http://www.alloy.rustltd.com/

///////////////////////////////////////////////////////////////////////////////
/// @file Unity.cginc
/// @brief Code shared between Alloy shaders and Unity override headers.
///////////////////////////////////////////////////////////////////////////////

#ifndef ALLOY_SHADERS_FRAMEWORK_UNITY_CGINC
#define ALLOY_SHADERS_FRAMEWORK_UNITY_CGINC

// Headers both for this file, and for all Definition and Feature modules.
#include "Assets/Alloy/Shaders/Config.cginc"
#include "Assets/Alloy/Shaders/Framework/Direct.cginc"
#include "Assets/Alloy/Shaders/Framework/Surface.cginc"
#include "Assets/Alloy/Shaders/Framework/Utility.cginc"

#include "AutoLight.cginc"
#include "UnityCG.cginc"
#include "UnityLightingCommon.cginc"

/// Calculates light vectors per-vertex.
/// @param[in]  positionWorld       Position in world-space.
/// @param[out] lightVectorRange    XYZ: Vector to light center, W: Light volume range.
/// @param[out] lightCoord          Projection coordinates in light space.
void aLightVectorRangeCoord(
    float3 positionWorld,
    out float4 lightVectorRange,
    out unityShadowCoord4 lightCoord)
{
    lightVectorRange = UnityWorldSpaceLightDir(positionWorld).xyzz;

#ifdef DIRECTIONAL
    lightCoord = 0.0h;
#else
    lightCoord = mul(unity_WorldToLight, unityShadowCoord4(positionWorld, 1.0f));

    #if defined(POINT) || defined(POINT_COOKIE) || defined(SPOT)
        // Light range = |light-space light vector| / |world-space light vector|
        // This works because the light vector's length is the same in both world
        // and light space, but it's scaled by the light range in light space.
        // cf http://forum.unity3d.com/threads/get-the-range-of-a-point-light-in-forward-add-mode.213430/#post-1433291
        lightVectorRange.w = length(lightVectorRange.xyz) * rsqrt(dot(lightCoord.xyz, lightCoord.xyz));
    #endif
#endif
}

/// Calculates global illumination from UnityGI data.
/// @param  gi      UnityGI populated with data.
/// @param  s       Material surface data.
/// @return         Indirect illumination.
half3 aUnityIndirectLighting(
    UnityGI gi,
    ASurface s)
{
#ifdef A_HAS_LIGHTING_CALLBACKS
    return aIndirectLighting(gi.indirect, s);
#endif
}

/// Calculates forward direct illumination.
/// @param  s                   Material surface data.
/// @param  shadow              Shadow attenuation.
/// @param  lightVectorRange    XYZ: Vector to light center, W: Light volume range.
/// @param  lightCoord          Light projection texture coordinates.
/// @return                     Direct illumination.
half3 aUnityDirectLighting(
    ASurface s,
    half shadow,
    float4 lightVectorRange,
    unityShadowCoord4 lightCoord)
{
    half3 illum = 0.0h;

#ifdef A_HAS_LIGHTING_CALLBACKS
    ADirect d = aNewDirect();
    half3 lightAxis = 0.0h;

    d.color = _LightColor0.rgb;
    d.shadow = shadow;
        
    #if !defined(ALLOY_SUPPORT_REDLIGHTS) && defined(DIRECTIONAL_COOKIE)
        aLightCookie(d, tex2D(_LightTexture0, lightCoord.xy));
    #elif defined(POINT) || defined(POINT_COOKIE) || defined(SPOT)
        lightAxis = normalize(unity_WorldToLight[1].xyz);

        #if defined(POINT)
            A_UNITY_ATTENUATION(d, _LightTexture0, lightCoord.xyz, 1.0f)
        #elif defined(POINT_COOKIE)
            aLightCookie(d, texCUBE(_LightTexture0, lightCoord.xyz));
            A_UNITY_ATTENUATION(d, _LightTextureB0, lightCoord.xyz, 1.0f)
        #elif defined(SPOT)
            half4 cookie = tex2D(_LightTexture0, lightCoord.xy / lightCoord.w + 0.5);
        
            cookie.a *= (lightCoord.z > 0);
            aLightCookie(d, cookie);
            A_UNITY_ATTENUATION(d, _LightTextureB0, lightCoord.xyz, 1.0f)
        #endif
    #endif

    #if !defined(ALLOY_SUPPORT_REDLIGHTS) || !defined(DIRECTIONAL_COOKIE)
        aAreaLight(d, s, _LightColor0, lightAxis, lightVectorRange.xyz, lightVectorRange.w);
    #else
        d.direction = lightVectorRange.xyz;
        d.color *= redLightCalculateForward(_LightTexture0, s.positionWorld, s.normalWorld, s.viewDirWorld, d.direction);
        aDirectionalLight(d, s);
    #endif

    illum = aDirectLighting(d, s);
#endif

    return illum;
}

/// Forward illumination with Unity inputs.
/// @param s        Material surface data.
/// @param gi       Unity GI descriptor.
/// @param shadow   Shadow for the given direct light.
/// @return         Combined lighting, emission, etc.
half4 aUnityColor(
    ASurface s,
    UnityGI gi,
    half shadow)
{
    half4 c = 0.0h;
    float4 lightVectorRange = 0.0h;
    unityShadowCoord4 lightCoord = 0.0f;

#ifdef UNITY_PASS_FORWARDBASE
    c.rgb = aUnityIndirectLighting(gi, s);
#endif

    aLightVectorRangeCoord(s.positionWorld, lightVectorRange, lightCoord);
    c.rgb += aUnityDirectLighting(s, shadow, lightVectorRange, lightCoord);
    c.rgb = aHdrClamp(c.rgb);
    c.a = s.opacity;
    return c;
}

/// Populates the G-buffer with Unity-compatible material data.
/// @param[in]  s           Material surface data.
/// @param[in]  gi          Unity GI descriptor.
/// @param[out] outGBuffer0 RGB: albedo, A: specular occlusion.
/// @param[out] outGBuffer1 RGB: f0, A: 1-roughness.
/// @param[out] outGBuffer2 RGB: packed normal, A: 1-scattering mask.
/// @return                 RGB: emission, A: 1-transmission.
half4 aUnityGbuffer(
    ASurface s,
    UnityGI gi,
    out half4 outGBuffer0,
    out half4 outGBuffer1,
    out half4 outGBuffer2)
{
    half3 illum = aHdrClamp(aUnityIndirectLighting(gi, s) + s.emissiveColor);
    outGBuffer0 = half4(s.albedo, s.specularOcclusion);
    outGBuffer1 = half4(s.f0, 1.0h - s.roughness);
    outGBuffer2 = half4(s.normalWorld * 0.5h + 0.5h, s.materialType);
    return half4(illum, s.subsurface);
}

#endif // ALLOY_SHADERS_FRAMEWORK_UNITY_CGINC
