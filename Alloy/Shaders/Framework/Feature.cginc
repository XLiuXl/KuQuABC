// Alloy Physical Shader Framework
// Copyright 2013-2017 RUST LLC.
// http://www.alloy.rustltd.com/

/////////////////////////////////////////////////////////////////////////////////
/// @file Feature.cginc
/// @brief Features uber-header. Holds methods that rely on uniforms.
/////////////////////////////////////////////////////////////////////////////////

#ifndef ALLOY_SHADERS_FRAMEWORK_FEATURE_CGINC
#define ALLOY_SHADERS_FRAMEWORK_FEATURE_CGINC

// Headers both for this file, and for all Definition and Feature modules.
#include "Assets/Alloy/Shaders/Config.cginc"
#include "Assets/Alloy/Shaders/Framework/Keywords.cginc"
#include "Assets/Alloy/Shaders/Framework/Splat.cginc"
#include "Assets/Alloy/Shaders/Framework/Surface.cginc"
#include "Assets/Alloy/Shaders/Framework/Utility.cginc"

#include "UnityCG.cginc"
#include "UnityStandardUtils.cginc"

/// Picks either UV0 or UV1.
#ifdef A_TEX_UV_OFF
    #define A_TEX_UV(s, name) (s.uv01.xy)
#else
    #define A_TEX_UV(s, name) (name##UV < 0.5f ? s.uv01.xy : s.uv01.zw)
#endif

/// Applies Unity texture transforms plus UV0.
#define A_TEX_TRANSFORM(s, name) (TRANSFORM_TEX(s.uv01.xy, name))

/// Applies Unity texture transforms plus UV-switching effect.
#define A_TEX_TRANSFORM_UV(s, name) (TRANSFORM_TEX(A_TEX_UV(s, name), name))

/// Applies Unity texture transforms plus UV-switching and our scrolling effects.
#define A_TEX_TRANSFORM_UV_SCROLL(s, name) (A_TEX_TRANSFORM_SCROLL(name, A_TEX_UV(s, name)))

/// Base UV assignment and update of associated fields.
#define A_BV(s, UV) UV; aUpdateBaseUv(s)

/// Cutoff value that controls where cutout occurs over opacity.
/// Expects values in the range [0,1].
half _Cutoff;

/// Toggles inverting the backface normals.
/// Expects the values 0 or 1.
float _TransInvertBackNormal;

/// The base tint color.
/// Expects a linear LDR color with alpha.
half4 _Color;

/// Base color map.
/// Expects an RGB(A) map with sRGB sampling.
A_SAMPLER_2D(_MainTex);

/// Base packed material map.
/// Expects an RGBA data map.
A_SAMPLER_2D(_SpecTex);

/// Metallic map.
/// Expects an RGB map with sRGB sampling
sampler2D _MetallicMap;

/// Ambient Occlusion map.
/// Expects an RGB map with sRGB sampling
sampler2D _AoMap;

/// Specularity map.
/// Expects an RGB map with sRGB sampling
sampler2D _SpecularityMap;

/// Roughness map.
/// Expects an RGB map with sRGB sampling
sampler2D _RoughnessMap;

/// Base normal map.
/// Expects a compressed normal map.
sampler2D _BumpMap;

/// Height map.
/// Expects an RGBA data map.
sampler2D _ParallaxMap;

/// Toggles tinting the base color by the vertex color.
/// Expects values in the range [0,1].
half _BaseColorVertexTint;

/// The base metallic scale.
/// Expects values in the range [0,1].
half _Metal; 

/// The base specularity scale.
/// Expects values in the range [0,1].
half _Specularity;

// Amount that f0 is tinted by the base color.
/// Expects values in the range [0,1].
half _SpecularTint;

/// The base roughness scale.
/// Expects values in the range [0,1].
half _Roughness;

/// Ambient Occlusion strength.
/// Expects values in the range [0,1].
half _Occlusion;

/// Normal map XY scale.
half _BumpScale;

/// Height scale of the heightmap.
/// Expects values in the range [0,0.08].
float _Parallax;

/// Sets up base UV for the first time.
/// @param[in,out] s Material surface data.
void aBaseUvInit(inout ASurface s);

/// Update values dependent on base UV.
void aUpdateBaseUv(inout ASurface s);

/// Sets whether backface normals are inverted.
/// @param[in,out] s Material surface data.
void aTwoSided(inout ASurface s);

/// Applies cutout effect.
/// @param s Material surface data.
void aCutout(ASurface s);

/// Samples the base color map.
/// @param  s   Material surface data.
/// @return     Base Color with alpha.
half4 aSampleBase(ASurface s);

/// Samples the base material map.
/// @param  s   Material surface data.
/// @return     Packed material.
half4 aSampleMaterial(ASurface s);

/// Samples  and scales the base bump map.
/// @param  s       Material surface data.
/// @param  scale   Normal XY scale factor.
/// @return         Normalized tangent-space normal.
half3 aSampleBumpScale(ASurface s, half scale);

/// Samples the base bump map.
/// @param  s   Material surface data.
/// @return     Normalized tangent-space normal.
half3 aSampleBump(ASurface s);

/// Samples the base bump map biasing the mipmap level sampled.
/// @param  s       Material surface data.
/// @param  bias    Mipmap level bias.
/// @return         Normalized tangent-space normal.
half3 aSampleBumpBias(ASurface s, float bias);

/// Samples the base bump map biasing the mipmap level sampled.
/// @param  s   Material surface data.
/// @return     Normalized tangent-space normal.
half aSampleHeight(ASurface s);

/// Applies color based on weight parameter.
/// @param  s           Material surface data.
/// @param  strength    Amount to blend in vertex color.
/// @return             Vertex color tint.
half3 aVertexColorTint(ASurface s, half strength);

/// Applies base vertex color.
/// @param  s   Material surface data.
/// @return     Vertex color tint.
half3 aBaseVertexColorTint(ASurface s);

/// Gets combined base color tint from uniform and vertex color.
/// @param  s   Material surface data.
/// @return     Base Color with alpha.
half4 aBaseTint(ASurface s);

/// Gets combined base color from main channels.
/// @param  s   Material surface data.
/// @return     Base Color with alpha.
half4 aBase(ASurface s);

/// Applies texture coordinate offsets to surface data.
/// @param[in,out]  s       Material surface data.
/// @param[in]      offset  Texture coordinate offset.
void aParallaxOffset(inout ASurface s, float2 offset);

/// Calculates Offset Bump Mapping texture offsets.
/// @param[in,out]  s   Material surface data.
void aOffsetBumpMapping(inout ASurface s);

/// Calculates Parallax Occlusion Mapping texture offsets.
/// @param[in,out]  s           Material surface data.
/// @param[in]      minSamples  Minimum number of samples for POM effect [1,n].
/// @param[in]      maxSamples  Maximum number of samples for POM effect [1,n].
void aParallaxOcclusionMapping(inout ASurface s, float minSamples, float maxSamples);

#endif // ALLOY_SHADERS_FRAMEWORK_FEATURE_CGINC
