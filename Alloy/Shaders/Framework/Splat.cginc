// Alloy Physical Shader Framework
// Copyright 2013-2017 RUST LLC.
// http://www.alloy.rustltd.com/

///////////////////////////////////////////////////////////////////////////////
/// @file Splat.cginc
/// @brief Splat mapping with either UV or TriPlanar texture mapping.
///////////////////////////////////////////////////////////////////////////////

#ifndef ALLOY_SHADERS_FRAMEWORK_SPLAT_CGINC
#define ALLOY_SHADERS_FRAMEWORK_SPLAT_CGINC

#ifdef A_TRIPLANAR_MAPPING_ON
    #ifndef _TRIPLANARMODE_WORLD
        #define A_WORLD_TO_OBJECT_ON
    #endif   

    #ifndef A_NORMAL_WORLD_ON
        #define A_NORMAL_WORLD_ON
    #endif

    #ifndef A_POSITION_WORLD_ON
        #define A_POSITION_WORLD_ON
    #endif
#endif

#include "Assets/Alloy/Shaders/Framework/Surface.cginc"
#include "Assets/Alloy/Shaders/Framework/Utility.cginc"

/// Contains accumulated splat material data.
struct ASplat {
    /// Non-TriPlanar transformed base UVs.
    float2 baseUv;

    /// (RGB) Base Color, (A) Opacity.
    /// Expects linear-space LDR color values.
    half4 material0;

    /// (R) Metallic, (G) AO/Specular Tint, (B) Specularity, (A) Roughness.
    /// Expects values in the range [0,1].
    half4 material1;

    /// (RGB) Emission, (A) Specular Tint.
    /// Expects linear-space HDR color values.
    half4 material2;

    /// World, Object, or Tangent normals.
    /// Expects a normalized vector.
    half3 normal;
};

/// Contains shared state for all splat functions, including TriPlanar data.
struct ASplatContext {
    /// Vertex color.
    /// Expects linear-space LDR color values.
    half4 vertexColor;

    /// The model's UV0 & UV1 texture coordinate data.
    /// Be aware that it can have parallax precombined with it.
    float4 uv01;

    /// X-axis TriPlanar tangent to world matrix.
    half3x3 xTangentToWorld;

    /// Y-axis TriPlanar tangent to world matrix.
    half3x3 yTangentToWorld;

    /// Z-axis TriPlanar tangent to world matrix.
    half3x3 zTangentToWorld;

    /// Blend weights between the top, middle, and bottom TriPlanar axis.
    half3 blend;

    /// Position in either world or object-space.
    float3 position;

    /// Binary masks for the positive values in the vertex normals.
    half3 axisMasks;
};

/// Constructor. 
/// @return Structure initialized with sane default values.
ASplat aNewSplat();

/// Uses surface data to make shared splat data.
/// @param  s               Material surface data.
/// @param  sharpness       Sharpness of the blend between TriPlanar axis.
/// @param  positionScale   Scales the position used for TriPlanar UVs.
/// @return                 Splat context initialized with shared data.
ASplatContext aNewSplatContext(ASurface s, half sharpness, float positionScale);

/// Converts splat to material data and assigns it to a surface.
/// @param[in,out]  s   Material surface data.
/// @param[in]      sp  Combined splat data.
void aApplySplat(inout ASurface s, ASplat sp);

/// Uses mask to blend a splat into surface.
/// @param[in,out]  s       Material surface data.
/// @param[in]      sp      Combined splat data.
void aBlendSplat(inout ASurface s, ASplat sp);

/// Uses splat opacity and mask to blend a splat into surface.
/// @param[in,out]  s       Material surface data.
/// @param[in]      sp      Combined splat data.
void aBlendSplatWithOpacity(inout ASurface s, ASplat sp);

/// Combines two splats into one, accumulating into first splat.
/// @param[in,out]  sp0 Target for combined splat data ouput.
/// @param[in]      sp1 Second splat to be combined.
void aMergeSplats(inout ASplat sp0, ASplat sp1);

/// Applies constant material data to a splat.
/// @param[in,out]  sp              Splat being modified.
/// @param[in]      sc              Splat context.
/// @param[in]      tint            Base color tint.
/// @param[in]      vertexTint      Base color vertex color tint weight.
/// @param[in]      metallic        Metallic  weight.
/// @param[in]      specularity     Specularity.
/// @param[in]      specularTint    Specular tint weight.
/// @param[in]      roughness       Linear roughness.
void aSplatMaterial(inout ASplat sp, ASplatContext sc, half4 tint, half vertexTint, half metallic, half specularity, half specularTint, half roughness);

/// TriPlanar axis applied to a splat.
/// @param[in,out]  sp          Splat being modified.
/// @param[in]      mask        Masks where the effect is applied.
/// @param[in]      tbn         Local normal tangent to world matrix.
/// @param[in]      uv          Texture coordinates.
/// @param[in]      occlusion   Occlusion map weight.
/// @param[in]      bumpScale   Normal map XY scale.
/// @param[in]      base        Base color map.
/// @param[in]      material    Material map.
/// @param[in]      normal      Normal map.
void aTriPlanarAxis(inout ASplat sp, half mask, half3x3 tbn, float2 uv, half occlusion, half bumpScale, sampler2D base, sampler2D material, sampler2D normal);

/// X-axis triplanar material applied to a splat.
/// @param[in,out]  sp          Splat being modified.
/// @param[in]      sc          Splat context.
/// @param[in]      base        Base color map.
/// @param[in]      material    Material map.
/// @param[in]      normal      Normal map.
/// @param[in]      occlusion   Occlusion map weight.
/// @param[in]      bumpScale   Normal map XY scale.
void aTriPlanarX(inout ASplat sp, ASplatContext sc, A_SAMPLER_PARAM(base), sampler2D material, sampler2D normal, half occlusion, half bumpScale);

/// Y-axis triplanar material applied to a splat.
/// @param[in,out]  sp          Splat being modified.
/// @param[in]      sc          Splat context.
/// @param[in]      base        Base color map.
/// @param[in]      material    Material map.
/// @param[in]      normal      Normal map.
/// @param[in]      occlusion   Occlusion map weight.
/// @param[in]      bumpScale   Normal map XY scale.
void aTriPlanarY(inout ASplat sp, ASplatContext sc, A_SAMPLER_PARAM(base), sampler2D material, sampler2D normal, half occlusion, half bumpScale);

/// Z-axis triplanar material applied to a splat.
/// @param[in,out]  sp          Splat being modified.
/// @param[in]      sc          Splat context.
/// @param[in]      base        Base color map.
/// @param[in]      material    Material map.
/// @param[in]      normal      Normal map.
/// @param[in]      occlusion   Occlusion map weight.
/// @param[in]      bumpScale   Normal map XY scale.
void aTriPlanarZ(inout ASplat sp, ASplatContext sc, A_SAMPLER_PARAM(base), sampler2D material, sampler2D normal, half occlusion, half bumpScale);

/// Positive Y-axis triplanar material applied to a splat.
/// @param[in,out]  sp          Splat being modified.
/// @param[in]      sc          Splat context.
/// @param[in]      base        Base color map.
/// @param[in]      material    Material map.
/// @param[in]      normal      Normal map.
/// @param[in]      occlusion   Occlusion map weight.
/// @param[in]      bumpScale   Normal map XY scale.
void aTriPlanarPositiveY(inout ASplat sp, ASplatContext sc, A_SAMPLER_PARAM(base), sampler2D material, sampler2D normal, half occlusion, half bumpScale);

/// Negative Y-axis triplanar material applied to a splat.
/// @param[in,out]  sp          Splat being modified.
/// @param[in]      sc          Splat context.
/// @param[in]      base        Base color map.
/// @param[in]      material    Material map.
/// @param[in]      normal      Normal map.
/// @param[in]      occlusion   Occlusion map weight.
/// @param[in]      bumpScale   Normal map XY scale.
void aTriPlanarNegativeY(inout ASplat sp, ASplatContext sc, A_SAMPLER_PARAM(base), sampler2D material, sampler2D normal, half occlusion, half bumpScale);

/// Applies constant material data to a splat.
/// @param  sc              Splat context.
/// @param  base            Base color map.
/// @param  material        Material map.
/// @param  normal          Normal map.
/// @param  tint            Base color tint.
/// @param  vertexTint      Base vertex color tint.
/// @param  metallic        Metallic  weight.
/// @param  specularity     Specularity.
/// @param  specularTint    Specular tint weight.
/// @param  roughness       Linear roughness.
/// @param  occlusion       Occlusion map weight.
/// @param  bumpScale       Normal map XY scale.
/// @return                 Splat populated with terrain data.
ASplat aNewSplat(ASplatContext sc, A_SAMPLER_PARAM(base), sampler2D material, sampler2D normal, half4 tint, half vertexTint, half metallic, half specularity, half specularTint, half roughness, half occlusion, half bumpScale);

/// Combine splats and convert to material data to assign to a surface.
/// @param[in,out]  s       Material surface data.
/// @param[in]      weights Weights masking where splats are combined.
/// @param[in]      sp0     Splat data.
/// @param[in]      sp1     Splat data.
/// @param[in]      sp2     Splat data.
void aApplyTerrainSplats(inout ASurface s, half3 weights, ASplat sp0, ASplat sp1, ASplat sp2);

/// Combine splats and convert to material data to assign to a surface.
/// @param[in,out]  s       Material surface data.
/// @param[in]      weights Weights masking where splats are combined.
/// @param[in]      sp0     Splat data.
/// @param[in]      sp1     Splat data.
/// @param[in]      sp2     Splat data.
/// @param[in]      sp3     Splat data.
void aApplyTerrainSplats(inout ASurface s, half4 weights, ASplat sp0, ASplat sp1, ASplat sp2, ASplat sp3);

#endif // ALLOY_SHADERS_FRAMEWORK_SPLAT_CGINC
