// Alloy Physical Shader Framework
// Copyright 2013-2017 RUST LLC.
// http://www.alloy.rustltd.com/

///////////////////////////////////////////////////////////////////////////////
/// @file Splat.cginc
/// @brief Splat method implementations to allow disabling of features.
///////////////////////////////////////////////////////////////////////////////

#ifndef ALLOY_SHADERS_FRAMEWORK_SPLAT_IMPL_CGINC
#define ALLOY_SHADERS_FRAMEWORK_SPLAT_IMPL_CGINC

#include "Assets/Alloy/Shaders/Framework/Splat.cginc"
#include "Assets/Alloy/Shaders/Framework/Surface.cginc"
#include "Assets/Alloy/Shaders/Framework/Utility.cginc"

#include "HLSLSupport.cginc"
#include "UnityCG.cginc"
#include "UnityStandardUtils.cginc"

#define A_BASE_COLOR material0.rgb
#define A_OPACITY material0.a

#define A_METALLIC material1.A_METALLIC_CHANNEL
#define A_AMBIENT_OCCLUSION material1.A_AO_CHANNEL
#define A_SPECULARITY material1.A_SPECULARITY_CHANNEL
#define A_ROUGHNESS material1.A_ROUGHNESS_CHANNEL

#ifdef A_AMBIENT_OCCLUSION_ON
    #define A_SPECULAR_TINT material2.a
#else
    #define A_SPECULAR_TINT material1.g
#endif

#define A_EMISSIVE_COLOR material2.rgb

ASplat aNewSplat()
{
    ASplat sp;

    UNITY_INITIALIZE_OUTPUT(ASplat, sp);
    sp.material0 = 0.0h;
    sp.material1 = 0.0h;
    sp.material2 = 0.0h;
    sp.normal = 0.0h;
    return sp;
}

ASplatContext aNewSplatContext(
    ASurface s,
    half sharpness,
    float positionScale)
{
    ASplatContext sc;

    UNITY_INITIALIZE_OUTPUT(ASplatContext, sc);
    sc.uv01 = s.uv01;
    sc.vertexColor = s.vertexColor;

#ifdef A_TRIPLANAR_MAPPING_ON
    // Triplanar mapping
    // cf http://www.gamedev.net/blog/979/entry-2250761-triplanar-texturing-and-normal-mapping/
    #ifdef _TRIPLANARMODE_WORLD
        half3 geoNormal = s.vertexNormalWorld;
        sc.position = s.positionWorld;
    #else
        half3 geoNormal = UnityWorldToObjectDir(s.vertexNormalWorld);
        sc.position = mul(unity_WorldToObject, float4(s.positionWorld, 1.0f)).xyz;
    #endif

    // Unity uses a Left-handed axis, so it requires clumsy remapping.
    sc.xTangentToWorld = half3x3(A_AXIS_Z, A_AXIS_Y, geoNormal);
    sc.yTangentToWorld = half3x3(A_AXIS_X, A_AXIS_Z, geoNormal);
    sc.zTangentToWorld = half3x3(A_AXIS_X, A_AXIS_Y, geoNormal);

    half3 blending = abs(geoNormal);
    blending = normalize(max(blending, A_EPSILON));
    blending = pow(blending, sharpness);
    blending /= dot(blending, A_ONE);
    sc.blend = blending;

    sc.axisMasks = step(A_ZERO, geoNormal);
    sc.position *= positionScale;
#endif

    return sc;
}

void aApplySplat(
    inout ASurface s,
    ASplat sp)
{
    half3 normal = sp.normal;

    s.baseColor = sp.A_BASE_COLOR;
    s.opacity = sp.A_OPACITY;
    s.metallic = sp.A_METALLIC;
    s.specularity = sp.A_SPECULARITY;
    s.specularTint = sp.A_SPECULAR_TINT;
    s.roughness = sp.A_ROUGHNESS;

#ifdef A_AMBIENT_OCCLUSION_ON
    s.ambientOcclusion = sp.A_AMBIENT_OCCLUSION;
#endif

#ifdef A_EMISSIVE_COLOR_ON
    s.emissiveColor = sp.A_EMISSIVE_COLOR;
#endif

#ifndef A_TRIPLANAR_MAPPING_ON
    s.normalTangent = A_NT(s, normalize(normal));
#else
    #ifdef _TRIPLANARMODE_WORLD
        half3 normalWorld = normalize(normal);
    #else
        half3 normalWorld = UnityObjectToWorldNormal(normal);
    #endif    

    s.normalWorld = A_NW(s, normalWorld);
#endif
}

void aBlendSplat(
    inout ASurface s,
    ASplat sp)
{
    half3 normal = sp.normal;
    half weight = s.mask;

    s.baseColor = lerp(s.baseColor, sp.A_BASE_COLOR, weight);
    s.opacity = lerp(s.opacity, sp.A_OPACITY, weight);
    s.metallic = lerp(s.metallic, sp.A_METALLIC, weight);
    s.specularity = lerp(s.specularity, sp.A_SPECULARITY, weight);
    s.specularTint = lerp(s.specularTint, sp.A_SPECULAR_TINT, weight);
    s.roughness = lerp(s.roughness, sp.A_ROUGHNESS, weight);    

#ifdef A_AMBIENT_OCCLUSION_ON
    s.ambientOcclusion = lerp(s.ambientOcclusion, sp.A_AMBIENT_OCCLUSION, weight);
#endif

#ifdef A_EMISSIVE_COLOR_ON
    s.emissiveColor = lerp(s.emissiveColor, sp.A_EMISSIVE_COLOR, weight);
#endif
    
#ifndef A_TRIPLANAR_MAPPING_ON
    s.normalTangent = A_NT(s, normalize(lerp(s.normalTangent, normal, weight)));
#else
    #ifdef _TRIPLANARMODE_WORLD
        half3 normalWorld = normalize(normal);
    #else
        half3 normalWorld = UnityObjectToWorldNormal(normal);
    #endif    

    s.normalWorld = A_NW(s, normalize(lerp(s.normalWorld, normalWorld, weight)));
#endif
}

void aBlendSplatWithOpacity(
    inout ASurface s,
    ASplat sp)
{
    s.mask *= sp.A_OPACITY;
    sp.A_OPACITY = 1.0h;

    aBlendSplat(s, sp);
}

void aMergeSplats(
    inout ASplat sp0,
    ASplat sp1)
{
    sp0.material0 += sp1.material0;
    sp0.material1 += sp1.material1;
    sp0.normal += sp1.normal;
    sp0.material2 += sp1.material2;
}

void aSplatMaterial(
    inout ASplat sp,
    ASplatContext sc,
    half4 tint,
    half vertexTint,
    half metallic,
    half specularity,
    half specularTint,
    half roughness)
{
    sp.material0 *= tint;

#ifndef A_VERTEX_COLOR_IS_DATA
    sp.A_BASE_COLOR *= aLerpWhiteTo(sc.vertexColor.rgb, vertexTint);
#endif

    sp.A_METALLIC *= metallic;
    sp.A_SPECULARITY *= specularity;
    sp.A_ROUGHNESS *= roughness;
    sp.A_SPECULAR_TINT = specularTint;
}

void aTriPlanarAxis(
    inout ASplat sp,
    half mask,
    half3x3 tbn,
    float2 uv,
    half occlusion,
    half bumpScale,
    sampler2D base,
    sampler2D material,
    sampler2D normal)
{
    ASplat sp0 = aNewSplat();

    sp0.material0 = tex2D(base, uv);
    sp0.normal = mul(UnpackScaleNormal(tex2D(normal, uv), bumpScale), tbn);

#ifndef A_ROUGHNESS_SOURCE_BASE_COLOR_ALPHA
    sp0.material1 = tex2D(material, uv);
    sp0.A_AMBIENT_OCCLUSION = aOcclusionStrength(sp0.A_AMBIENT_OCCLUSION, occlusion);
#else
    sp0.A_METALLIC = 1.0h;
    sp0.A_AMBIENT_OCCLUSION = 1.0h;
    sp0.A_SPECULARITY = 1.0h;
    sp0.A_ROUGHNESS = sp0.A_OPACITY;
    sp0.A_OPACITY = 1.0h;
#endif

    sp.material0 += mask * sp0.material0;
    sp.material1 += mask * sp0.material1;
    sp.normal += mask * sp0.normal;
}

void aTriPlanarX(
    inout ASplat sp,
    ASplatContext sc,
    A_SAMPLER_PARAM(base),
    sampler2D material,
    sampler2D normal,
    half occlusion,
    half bumpScale)
{
    aTriPlanarAxis(sp, sc.blend.x, sc.xTangentToWorld, A_TEX_TRANSFORM_SCROLL(base, sc.position.zy), occlusion, bumpScale, base, material, normal);
}

void aTriPlanarY(
    inout ASplat sp,
    ASplatContext sc,
    A_SAMPLER_PARAM(base),
    sampler2D material,
    sampler2D normal,
    half occlusion,
    half bumpScale)
{
    aTriPlanarAxis(sp, sc.blend.y, sc.yTangentToWorld, A_TEX_TRANSFORM_SCROLL(base, sc.position.xz), occlusion, bumpScale, base, material, normal);
}

void aTriPlanarZ(
    inout ASplat sp,
    ASplatContext sc,
    A_SAMPLER_PARAM(base),
    sampler2D material,
    sampler2D normal,
    half occlusion,
    half bumpScale)
{
    aTriPlanarAxis(sp, sc.blend.z, sc.zTangentToWorld, A_TEX_TRANSFORM_SCROLL(base, sc.position.xy), occlusion, bumpScale, base, material, normal);
}

void aTriPlanarPositiveY(
    inout ASplat sp,
    ASplatContext sc,
    A_SAMPLER_PARAM(base),
    sampler2D material,
    sampler2D normal,
    half occlusion,
    half bumpScale)
{
    aTriPlanarAxis(sp, sc.axisMasks.y * sc.blend.y, sc.yTangentToWorld, A_TEX_TRANSFORM_SCROLL(base, sc.position.xz), occlusion, bumpScale, base, material, normal);
}

void aTriPlanarNegativeY(
    inout ASplat sp,
    ASplatContext sc,
    A_SAMPLER_PARAM(base),
    sampler2D material,
    sampler2D normal,
    half occlusion,
    half bumpScale)
{
    aTriPlanarAxis(sp, (1.0h - sc.axisMasks.y) * sc.blend.y, sc.yTangentToWorld, A_TEX_TRANSFORM_SCROLL(base, sc.position.xz), occlusion, bumpScale, base, material, normal);
}

ASplat aNewSplat(
    ASplatContext sc,
    A_SAMPLER_PARAM(base),
    sampler2D material,
    sampler2D normal,
    half4 tint,
    half vertexTint,
    half metallic,
    half specularity,
    half specularTint,
    half roughness,
    half occlusion,
    half bumpScale)
{
    ASplat sp = aNewSplat();

#ifdef A_TRIPLANAR_MAPPING_ON
    aTriPlanarX(sp, sc, A_SAMPLER_INPUT(base), material, normal, occlusion, bumpScale);
    aTriPlanarY(sp, sc, A_SAMPLER_INPUT(base), material, normal, occlusion, bumpScale);
    aTriPlanarZ(sp, sc, A_SAMPLER_INPUT(base), material, normal, occlusion, bumpScale);
#else
    sp.baseUv = A_TEX_TRANSFORM_UV_SCROLL(sc, base);
    sp.material0 = tex2D(base, sp.baseUv);
    sp.normal = UnpackScaleNormal(tex2D(normal, sp.baseUv), bumpScale);

    #ifndef A_ROUGHNESS_SOURCE_BASE_COLOR_ALPHA
        sp.material1 = tex2D(material, sp.baseUv);
        sp.A_AMBIENT_OCCLUSION = aOcclusionStrength(sp.A_AMBIENT_OCCLUSION, occlusion);
    #else
        sp.A_METALLIC = 1.0h;
        sp.A_AMBIENT_OCCLUSION = 1.0h;
        sp.A_SPECULARITY = 1.0h;
        sp.A_ROUGHNESS = sp.A_OPACITY;
        sp.A_OPACITY = 1.0h;
    #endif
#endif

    aSplatMaterial(sp, sc, tint, vertexTint, metallic, specularity, specularTint, roughness);
    return sp;
}

void aApplyTerrainSplats(
    inout ASurface s,
    half3 weights,
    ASplat sp0,
    ASplat sp1,
    ASplat sp2)
{
    ASplat sp = aNewSplat();
    sp.material0 = weights.r * sp0.material0 + weights.g * sp1.material0 + weights.b * sp2.material0;
    sp.material1 = weights.r * sp0.material1 + weights.g * sp1.material1 + weights.b * sp2.material1;
    sp.material2 = weights.r * sp0.material2 + weights.g * sp1.material2 + weights.b * sp2.material2;
    sp.normal = weights.r * sp0.normal + weights.g * sp1.normal + weights.b * sp2.normal;
    aApplySplat(s, sp);
}

void aApplyTerrainSplats(
    inout ASurface s,
    half4 weights,
    ASplat sp0,
    ASplat sp1,
    ASplat sp2,
    ASplat sp3)
{
    ASplat sp = aNewSplat();
    sp.material0 = weights.r * sp0.material0 + weights.g * sp1.material0 + weights.b * sp2.material0 + weights.a * sp3.material0;
    sp.material1 = weights.r * sp0.material1 + weights.g * sp1.material1 + weights.b * sp2.material1 + weights.a * sp3.material1;
    sp.material2 = weights.r * sp0.material2 + weights.g * sp1.material2 + weights.b * sp2.material2 + weights.a * sp3.material2;
    sp.normal = weights.r * sp0.normal + weights.g * sp1.normal + weights.b * sp2.normal + weights.a * sp3.normal;
    aApplySplat(s, sp);
}

#endif // ALLOY_SHADERS_FRAMEWORK_SPLAT_IMPL_CGINC
