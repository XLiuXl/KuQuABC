// Alloy Physical Shader Framework
// Copyright 2013-2017 RUST LLC.
// http://www.alloy.rustltd.com/

///////////////////////////////////////////////////////////////////////////////
/// @file Type.cginc
/// @brief Shader type uber-header.
///////////////////////////////////////////////////////////////////////////////

#ifndef ALLOY_SHADERS_FRAMEWORK_TYPE_CGINC
#define ALLOY_SHADERS_FRAMEWORK_TYPE_CGINC

// Headers both for this file, and for all Definition and Feature modules.
#include "Assets/Alloy/Shaders/Config.cginc"
#include "Assets/Alloy/Shaders/Framework/Keywords.cginc"

#include "Assets/Alloy/Shaders/Framework/Feature.cginc"
#include "Assets/Alloy/Shaders/Framework/Surface.cginc"
#include "Assets/Alloy/Shaders/Framework/Utility.cginc"

#include "HLSLSupport.cginc"
#include "UnityCG.cginc"
#include "UnityInstancing.cginc"

// Features
#include "Assets/Alloy/Shaders/Feature/AO2.cginc"
#include "Assets/Alloy/Shaders/Feature/CarPaint.cginc"
#include "Assets/Alloy/Shaders/Feature/Decal.cginc"
#include "Assets/Alloy/Shaders/Feature/Detail.cginc"
#include "Assets/Alloy/Shaders/Feature/DirectionalBlend.cginc"
#include "Assets/Alloy/Shaders/Feature/Dissolve.cginc"
#include "Assets/Alloy/Shaders/Feature/Emission.cginc"
#include "Assets/Alloy/Shaders/Feature/Emission2.cginc"
#include "Assets/Alloy/Shaders/Feature/Eye.cginc"
#include "Assets/Alloy/Shaders/Feature/MainTextures.cginc"
#include "Assets/Alloy/Shaders/Feature/OrientedTextures.cginc"
#include "Assets/Alloy/Shaders/Feature/Parallax.cginc"
#include "Assets/Alloy/Shaders/Feature/Puddles.cginc"
#include "Assets/Alloy/Shaders/Feature/Rim.cginc"
#include "Assets/Alloy/Shaders/Feature/Rim2.cginc"
#include "Assets/Alloy/Shaders/Feature/SecondaryTextures.cginc"
#include "Assets/Alloy/Shaders/Feature/SkinTextures.cginc"
#include "Assets/Alloy/Shaders/Feature/SpeedTree.cginc"
#include "Assets/Alloy/Shaders/Feature/TeamColor.cginc"
#include "Assets/Alloy/Shaders/Feature/Terrain.cginc"
#include "Assets/Alloy/Shaders/Feature/TransitionBlend.cginc"
#include "Assets/Alloy/Shaders/Feature/Transmission.cginc"
#include "Assets/Alloy/Shaders/Feature/TriPlanar.cginc"
#include "Assets/Alloy/Shaders/Feature/VertexBlend.cginc"
#include "Assets/Alloy/Shaders/Feature/WeightedBlend.cginc"
#include "Assets/Alloy/Shaders/Feature/Wetness.cginc"

#if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
    #define A_SHADOW_MASK_RT_ON
#endif

#if !defined(A_UV2_ON) && (defined(DYNAMICLIGHTMAP_ON) || defined(UNITY_PASS_META))
    #define A_UV2_ON
#endif

// NOTE: This block is here to enable the vertex Tangent!
#if !defined(A_TANGENT_TO_WORLD_ON) && !defined(A_SHADOW_META_PASS) && (defined(A_NORMAL_MAPPING_ON) || defined(A_VIEW_DIR_TANGENT_ON))
    #define A_TANGENT_TO_WORLD_ON

    #ifndef A_TANGENT_ON
        #define A_TANGENT_ON
    #endif

    #ifndef A_NORMAL_WORLD_ON
        #define A_NORMAL_WORLD_ON
    #endif
#endif

/// Deferred geometry buffer representation of surface data.
struct AGbuffer {
    /// RGB: Albedo, A: Specular occlusion.
    half4 target0 : SV_Target0;

    /// RGB: f0, A: 1-Roughness.
    half4 target1 : SV_Target1;

    /// RGB: Packed normal, A: Material type.
    half4 target2 : SV_Target2;

    /// RGB: Emission, A: 1-Transmission.
    half4 target3 : SV_Target3;

#ifdef A_SHADOW_MASK_RT_ON
    /// RGBA: Shadow Masks.
    half4 target4 : SV_Target4;
#endif
};

/// Vertex input from the model data.
struct AVertex 
{
    float4 vertex : POSITION;
    float4 uv0 : TEXCOORD0;
    float4 uv1 : TEXCOORD1;
    half3 normal : NORMAL;
#ifdef A_UV2_ON
    float4 uv2 : TEXCOORD2;
#endif
#ifdef A_UV3_ON
    float4 uv3 : TEXCOORD3;
#endif
#ifdef A_TANGENT_ON
    half4 tangent : TANGENT;
#endif
    half4 color : COLOR;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

void aVertex(inout AVertex v);
void aFinalColor(inout half4 color, ASurface s);
void aFinalGbuffer(inout AGbuffer gb, ASurface s);

/// Applies standard vertex transformations.
/// @param  v   Input vertex data.
void aStandardVertex(inout AVertex v);

#endif // ALLOY_SHADERS_FRAMEWORK_TYPE_CGINC
