// Alloy Physical Shader Framework
// Copyright 2013-2017 RUST LLC.
// http://www.alloy.rustltd.com/

///////////////////////////////////////////////////////////////////////////////
/// @file Keywords.cginc
/// @brief Remaps Unity keywords to be more clear and save typing.
///////////////////////////////////////////////////////////////////////////////

#ifndef ALLOY_SHADERS_FRAMEWORK_KEYWORDS_CGINC
#define ALLOY_SHADERS_FRAMEWORK_KEYWORDS_CGINC

// Needed to prevent z-fighting on Forward Add when using instancing.
#if !defined(UNITY_USE_PREMULTIPLIED_MATRICES) && defined(INSTANCING_ON) && defined(UNITY_PASS_FORWARDADD)
    #define UNITY_USE_PREMULTIPLIED_MATRICES 
#endif

#if defined(UNITY_PASS_FORWARDBASE) || defined(UNITY_PASS_FORWARDADD) || defined(A_PASS_DISTORT)
    #define A_FORWARD_PASS
#endif

#if defined(UNITY_PASS_SHADOWCASTER) || defined(UNITY_PASS_META)
    #define A_SHADOW_META_PASS
#endif

#if defined(_ALPHABLEND_ON) || defined(_ALPHAPREMULTIPLY_ON)
    #define A_ALPHA_BLENDING_ON 
#endif	

#if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
    #define A_FOG_ON
#endif

#if defined(EFFECT_BUMP) || defined(_TERRAIN_NORMAL_MAP)
    #define A_NORMAL_MAPPING_ON
#endif

#ifdef _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
    #define A_ROUGHNESS_SOURCE_BASE_COLOR_ALPHA
#endif

#ifdef _METALLICGLOSSMAP
    #define A_TRIPLANAR_MAPPING_ON
#endif

#ifdef _SPECGLOSSMAP
    #define A_ALPHA_SPLAT_ON
#endif

#ifdef _DETAIL_MULX2
    #define A_DETAIL_ON
#endif

#ifdef _NORMALMAP
    #define A_DETAIL_MASK_VERTEX_COLOR_ALPHA_ON
#endif

#ifdef _PARALLAXMAP
    #define A_PARALLAX_ON
#endif

#ifdef _EMISSION
    #define A_EMISSION_ON
#endif

#endif // ALLOY_SHADERS_FRAMEWORK_KEYWORDS_CGINC
