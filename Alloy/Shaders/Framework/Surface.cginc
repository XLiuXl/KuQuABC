// Alloy Physical Shader Framework
// Copyright 2013-2017 RUST LLC.
// http://www.alloy.rustltd.com/

/////////////////////////////////////////////////////////////////////////////////
/// @file Surface.cginc
/// @brief ASurface structure, and related methods.
/////////////////////////////////////////////////////////////////////////////////

#ifndef ALLOY_SHADERS_FRAMEWORK_SURFACE_CGINC
#define ALLOY_SHADERS_FRAMEWORK_SURFACE_CGINC

#include "Assets/Alloy/Shaders/Config.cginc"

/// Tangent-space normal assignment and update of associated fields.
#define A_NT(s, n) n; aUpdateNormalTangent(s) 

/// World-space normal assignment and update of associated fields.
#define A_NW(s, n) n; aUpdateNormalWorld(s) 

/// Subsurface assignment and update of associated fields.
#define A_SS(s, ss) ss; aUpdateSubsurface(s) 

/// Subsurface color assignment and update of associated fields.
#define A_SSC(s, ssc) ssc; aUpdateSubsurfaceColor(s) 

/// Contains ALL data and state for rendering a surface.
/// Can set state to control how features are combined into the surface data.
struct ASurface {
    /////////////////////////////////////////////////////////////////////////////
    // Vertex inputs.
    /////////////////////////////////////////////////////////////////////////////

    /// Screen-space position.
    float4 screenPosition;
    
    /// Screen-space texture coordinates.
    float2 screenUv;

    /// Position in world space.
    float3 positionWorld;

    /// View direction in world space.
    /// Expects a normalized vector.
    half3 viewDirWorld;

    /// Distance from the camera to the given fragement.
    /// Expects values in the range [0,n].
    half viewDepth;

    /// Unity's fog data.
    float fogCoord;
        
    /// Tangent space to World space rotation matrix.
    half3x3 tangentToWorld;
        
    /// View direction in tangent space.
    /// Expects a normalized vector.
    half3 viewDirTangent;
    
    /// Vertex color.
    /// Expects linear-space LDR color values.
    half4 vertexColor;

    /// Vertex normal in world space.
    /// Expects normalized vectors in the range [-1,1].
    half3 vertexNormalWorld;

    /// Indicates via sign whether a triangle is front or back facing.
    /// Positive is front-facing, negative is back-facing. 
    half facingSign;


    /////////////////////////////////////////////////////////////////////////////
    // Feature layering inputs.
    /////////////////////////////////////////////////////////////////////////////
    
    /// Masks where the next feature layer will be applied.
    /// Expects values in the range [0,1].
    half mask;
        
    /// The base map's texture transform tiling amount.
    float2 baseTiling;
        
    /// Transformed texture coordinates for the base map.
    float2 baseUv;

#ifdef _VIRTUALTEXTURING_ON
    /// Transformed texture coordinates for the virtual base map.
    VirtualCoord baseVirtualCoord;
#endif

    /// The model's UV0 & UV1 texture coordinate data.
    /// Be aware that it can have parallax precombined with it.
    float4 uv01;


    /////////////////////////////////////////////////////////////////////////////
    // Surface inputs.
    /////////////////////////////////////////////////////////////////////////////

    /// Albedo and/or Metallic f0 based on settings. Used by Enlighten.
    /// Expects linear-space LDR color values.
    half3 baseColor;

    /// Controls opacity or cutout regions.
    /// Expects values in the range [0,1].
    half opacity;

    /// Interpolates material from dielectric to metal.
    /// Expects values in the range [0,1].
    half metallic;

    /// Diffuse ambient occlusion.
    /// Expects values in the range [0,1].
    half ambientOcclusion;

    /// Linear control of dielectric f0 from [0.00,0.08].
    /// Expects values in the range [0,1].
    half specularity;

    /// Tints the dielectric specularity by the base color chromaticity.
    /// Expects values in the range [0,1].
    half specularTint;

    /// Linear roughness value, where zero is smooth and one is rough.
    /// Expects values in the range [0,1].
    half roughness;

    /// Light emission by the material.
    /// Expects linear-space HDR color values.
    half3 emissiveColor;

    /// Color tint for transmission effect.
    /// Expects linear-space LDR color values.
    half3 subsurfaceColor;

    /// Monochrome transmission thickness.
    /// Expects gamma-space LDR values.
    half subsurface;

    /// Strength of clearcoat layer, used to apply masks.
    /// Expects values in the range [0,1].
    half clearCoat;

    /// Roughness of clearcoat layer.
    /// Expects values in the range [0,1].
    half clearCoatRoughness;

    /// Normal in world space.
    /// Expects a normalized vector.
    half3 normalWorld;

    /// Normal in tangent space.
    /// Expects a normalized vector.
    half3 normalTangent;

    /// Blurred normal in tangent space.
    /// Expects a normalized vector.
    half3 blurredNormalTangent;


    /////////////////////////////////////////////////////////////////////////////
    // BRDF inputs.
    /////////////////////////////////////////////////////////////////////////////
    
    /// Diffuse albedo.
    /// Expects linear-space LDR color values.
    half3 albedo;

    /// Fresnel reflectance at incidence zero.
    /// Expects linear-space LDR color values.
    half3 f0;

    /// Beckmann roughness.
    /// Expects values in the range [0,1].
    half beckmannRoughness;

    /// Specular occlusion.
    /// Expects values in the range [0,1].
    half specularOcclusion;

    /// Ambient diffuse normal in world space.
    /// Expects normalized vectors in the range [-1,1].
    half3 ambientNormalWorld;

    /// View reflection vector in world space.
    /// Expects a non-normalized vector.
    half3 reflectionVectorWorld;

    /// Clamped N.V.
    /// Expects values in the range [0,1].
    half NdotV;

    /// Fresnel weight of N.V.
    /// Expects values in the range [0,1].
    half FV;

    /// Deferred material lighting type.
    /// Expects the values 0, 1/3, 2/3, or 1.
    half materialType;

    half4 custom0;
    half4 custom1;
    half4 custom2;
    half4 custom3;
    half4 custom4;
    half4 custom5;
};

/// Constructor. 
/// @return Structure initialized with sane default values.
ASurface aNewSurface();

/// Sets the feature mask by using a gradient input mask.
/// @param[in,out]  s           Material surface data.
/// @param[in]      mask        Gradient where effect goes from black to white.
/// @param[in]      weight      Weight of the effect.
/// @param[in]      cutoff      Value below which the gradient becomes a mask.
/// @param[in]      blendRange  Range of smooth blend above cutoff.
/// @param[in]      vertexTint  Weight of vertex color alpha cutoff override.
void aBlendRangeMask(inout ASurface s, half mask, half weight, half cutoff, half blendRange, half vertexTint);

/// Transforms a normal from tangent-space to world-space.
half3 aTangentToWorld(ASurface s, half3 normalTangent);

/// Transforms a normal from world-space to tangent-space.
half3 aWorldToTangent(ASurface s, half3 normalWorld);

/// Calculates and sets normal and view-dependent data.
void aUpdateViewData(inout ASurface s);

/// Update values dependent on tangent-space normal.
void aUpdateNormalTangent(inout ASurface s);

/// Update values dependent on world-space normal.
void aUpdateNormalWorld(inout ASurface s);

/// Update values dependent on subsurface.
void aUpdateSubsurface(inout ASurface s);

/// Update values dependent on subsurface color.
void aUpdateSubsurfaceColor(inout ASurface s);

#endif // ALLOY_SHADERS_FRAMEWORK_SURFACE_CGINC
