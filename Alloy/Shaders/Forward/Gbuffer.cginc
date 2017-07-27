// Alloy Physical Shader Framework
// Copyright 2013-2017 RUST LLC.
// http://www.alloy.rustltd.com/

/////////////////////////////////////////////////////////////////////////////////
/// @file Gbuffer.cginc
/// @brief Forward g-buffer fill pass vertex & fragment shaders.
/////////////////////////////////////////////////////////////////////////////////

#ifndef ALLOY_SHADERS_FORWARD_GBUFFER_CGINC
#define ALLOY_SHADERS_FORWARD_GBUFFER_CGINC

#define A_BASE_PASS
#define A_INSTANCING_PASS
#define A_STEREO_PASS
#define A_INDIRECT_PASS

#include "Assets/Alloy/Shaders/Framework/ForwardLit.cginc"

void aVertexShader(
    AVertex v,
    out AVertexToFragment o,
    out float4 opos : SV_POSITION)
{
    aForwardLitVertex(v, o, opos);
}

AGbuffer aFragmentShader(
    AVertexToFragment i
    A_FACING_TYPE)
{
    return aForwardLitGbuffer(i, A_FACING_SIGN);
}					
            
#endif // ALLOY_SHADERS_FORWARD_GBUFFER_CGINC
