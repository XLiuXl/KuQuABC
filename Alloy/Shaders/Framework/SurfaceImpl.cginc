// Alloy Physical Shader Framework
// Copyright 2013-2017 RUST LLC.
// http://www.alloy.rustltd.com/

///////////////////////////////////////////////////////////////////////////////
/// @file Surface.cginc
/// @brief Surface method implementations to allow disabling of features.
///////////////////////////////////////////////////////////////////////////////

#ifndef ALLOY_SHADERS_FRAMEWORK_SURFACE_IMPL_CGINC
#define ALLOY_SHADERS_FRAMEWORK_SURFACE_IMPL_CGINC

#include "Assets/Alloy/Shaders/Framework/Surface.cginc"
#include "Assets/Alloy/Shaders/Framework/Utility.cginc"

#include "HLSLSupport.cginc"
#include "UnityCG.cginc"

ASurface aNewSurface() {
    ASurface s;

    UNITY_INITIALIZE_OUTPUT(ASurface, s);
    s.vertexNormalWorld = A_FLAT_NORMAL;
    s.normalWorld = A_FLAT_NORMAL;
    s.viewDirWorld = A_AXIS_X;
    s.viewDirTangent = A_AXIS_X;
    s.reflectionVectorWorld = A_AXIS_X;
    s.tangentToWorld = 0.0h;
    s.normalTangent = A_FLAT_NORMAL;
    s.blurredNormalTangent = A_FLAT_NORMAL;
    s.facingSign = 1.0h;
    s.fogCoord = 0.0f;
    s.NdotV = 0.0h;
    s.FV = 0.0h;
    s.materialType = 1.0h;
    s.mask = 1.0h;

    s.baseColor = 0.0h;
    s.opacity = 1.0h;
    s.metallic = 0.0h;
    s.ambientOcclusion = 1.0h;
    s.specularity = 0.5h;
    s.specularTint = 0.0h;
    s.roughness = 0.0h;
    s.emissiveColor = 0.0h;
    s.subsurfaceColor = 0.0h;
    s.subsurface = 0.0h;
    s.clearCoat = 0.0h;
    s.clearCoatRoughness = 0.0h;

    return s;
}

void aBlendRangeMask(
    inout ASurface s,
    half mask,
    half weight,
    half cutoff,
    half blendRange,
    half vertexTint)
{
    cutoff = lerp(cutoff, 1.0h, s.vertexColor.a * vertexTint);
    mask = 1.0h - saturate((mask - cutoff) / blendRange);
    s.mask *= weight * mask;
}

half3 aTangentToWorld(
    ASurface s,
    half3 normalTangent)
{
#ifndef A_TANGENT_TO_WORLD_ON
    return s.vertexNormalWorld;
#else
    return normalize(mul(normalTangent, s.tangentToWorld));
#endif
}

half3 aWorldToTangent(
    ASurface s,
    half3 normalWorld)
{
#ifndef A_TANGENT_TO_WORLD_ON
    return A_FLAT_NORMAL;
#else
    return normalize(mul(s.tangentToWorld, normalWorld));
#endif
}

void aUpdateViewData(
    inout ASurface s)
{
#if defined(A_NORMAL_WORLD_ON) && defined(A_VIEW_DIR_WORLD_ON)
    // Area lights need this to be per-pixel.
    #ifdef A_REFLECTION_VECTOR_WORLD_ON
        s.reflectionVectorWorld = reflect(-s.viewDirWorld, s.normalWorld);
    #endif

    // Skip re-calculating world normals in some cases.
    #ifndef A_VIEW_DIR_TANGENT_ON
        s.NdotV = aDotClamp(s.normalWorld, s.viewDirWorld);
    #else
        s.NdotV = aDotClamp(s.normalTangent, s.viewDirTangent);
    #endif

    s.FV = aFresnel(s.NdotV);
#endif
}

void aUpdateNormalTangent(
    inout ASurface s)
{
#ifndef A_TANGENT_TO_WORLD_ON
    s.normalTangent = A_FLAT_NORMAL;
#else
    s.normalWorld = aTangentToWorld(s, s.normalTangent);
    s.ambientNormalWorld = s.normalWorld;
    aUpdateViewData(s);
#endif
}

void aUpdateNormalWorld(
    inout ASurface s)
{
#ifndef A_TANGENT_TO_WORLD_ON
    s.normalWorld = s.vertexNormalWorld;
#else
    s.normalTangent = aWorldToTangent(s, s.normalWorld);
    s.ambientNormalWorld = s.normalWorld;
    aUpdateViewData(s);
#endif
}

void aUpdateSubsurface(
    inout ASurface s)
{
    s.subsurfaceColor = aGammaToLinear(s.subsurface).rrr;
}

void aUpdateSubsurfaceColor(
    inout ASurface s)
{
    s.subsurface = LinearToGammaSpace(s.subsurfaceColor).g;
}

#endif // ALLOY_SHADERS_FRAMEWORK_SURFACE_IMPL_CGINC
