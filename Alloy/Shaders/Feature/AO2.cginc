// Alloy Physical Shader Framework
// Copyright 2013-2017 RUST LLC.
// http://www.alloy.rustltd.com/

/////////////////////////////////////////////////////////////////////////////////
/// @file AO2.cginc
/// @brief Secondary Ambient Occlusion, possibly on a different UV.
/////////////////////////////////////////////////////////////////////////////////

#ifndef ALLOY_SHADERS_FEATURE_AO2_CGINC
#define ALLOY_SHADERS_FEATURE_AO2_CGINC

#ifdef _AO2_ON
    #ifndef A_AMBIENT_OCCLUSION_ON
        #define A_AMBIENT_OCCLUSION_ON
    #endif
#endif

#include "Assets/Alloy/Shaders/Framework/Feature.cginc"

#ifdef _AO2_ON
    /// Secondary Ambient Occlusion map.
    /// Expects an RGB map with sRGB sampling
    A_SAMPLER_2D(_Ao2Map);

    /// Ambient Occlusion strength.
    /// Expects values in the range [0,1].
    half _Ao2Occlusion;
#endif

void aAo2(
    inout ASurface s) 
{
#ifdef _AO2_ON
    float2 ao2Uv = A_TEX_TRANSFORM_UV(s, _Ao2Map);
    s.ambientOcclusion *= aLerpOneTo(tex2D(_Ao2Map, ao2Uv).g, _Ao2Occlusion * s.mask);
#endif
} 

#endif // ALLOY_SHADERS_FEATURE_AO2_CGINC
