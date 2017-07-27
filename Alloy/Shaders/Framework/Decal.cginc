// Alloy Physical Shader Framework
// Copyright 2013-2017 RUST LLC.
// http://www.alloy.rustltd.com/

///////////////////////////////////////////////////////////////////////////////
/// @file Decal.cginc
/// @brief Decal model type uber-header.
///////////////////////////////////////////////////////////////////////////////

#ifndef ALLOY_SHADERS_FRAMEWORK_DECAL_CGINC
#define ALLOY_SHADERS_FRAMEWORK_DECAL_CGINC

#ifdef A_PROJECTIVE_DECAL_ON
    #ifndef A_TEX_UV_OFF
        #define A_TEX_UV_OFF
    #endif

    #ifndef A_VERTEX_COLOR_IS_DATA
        #define A_VERTEX_COLOR_IS_DATA
    #endif
#endif

#include "Assets/Alloy/Shaders/Framework/Type.cginc"

#endif // ALLOY_SHADERS_FRAMEWORK_DECAL_CGINC
