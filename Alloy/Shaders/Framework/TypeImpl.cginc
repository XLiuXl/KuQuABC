// Alloy Physical Shader Framework
// Copyright 2013-2017 RUST LLC.
// http://www.alloy.rustltd.com/

///////////////////////////////////////////////////////////////////////////////
/// @file Type.cginc
/// @brief Shader type method implementations to allow disabling of features.
///////////////////////////////////////////////////////////////////////////////

#ifndef ALLOY_SHADERS_FRAMEWORK_MODEL_IMPL_CGINC
#define ALLOY_SHADERS_FRAMEWORK_MODEL_IMPL_CGINC

#ifdef A_USE_VERTEX_MOTION
    #ifndef A_VERTEX_COLOR_IS_DATA
        #define A_VERTEX_COLOR_IS_DATA
    #endif
#endif

#include "Assets/Alloy/Shaders/Framework/Type.cginc"

void aStandardVertex(
    inout AVertex v)
{
#ifdef A_USE_VERTEX_MOTION
    v.vertex = VertExmotion(v.vertex, v.color);
#elif !defined(A_VERTEX_COLOR_IS_DATA)
    /// Convert in vertex shader to interpolate in linear space.
    v.color.rgb = aGammaToLinear(v.color.rgb);
#endif
}

#endif // ALLOY_SHADERS_FRAMEWORK_MODEL_IMPL_CGINC
