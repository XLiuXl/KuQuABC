// Alloy Physical Shader Framework
// Copyright 2013-2017 RUST LLC.
// http://www.alloy.rustltd.com/

///////////////////////////////////////////////////////////////////////////////
/// @file DecalMultiplicative.cginc
/// @brief Multiplicative decal inputs and outputs.
///////////////////////////////////////////////////////////////////////////////

#ifndef ALLOY_SHADERS_TYPE_DECAL_MULTIPLICATIVE_CGINC
#define ALLOY_SHADERS_TYPE_DECAL_MULTIPLICATIVE_CGINC

#include "Assets/Alloy/Shaders/Framework/Decal.cginc"

void aVertex(
    inout AVertex v)
{
    aStandardVertex(v);
}

void aFinalColor(
    inout half4 color,
    ASurface s)
{
    color.rgb = s.baseColor.rgb;
    color.a = 1.0h;

    // Fog to white to allow underlying surface fog to bleed through.
    UNITY_APPLY_FOG_COLOR(s.fogCoord, color, A_WHITE4);
}

void aFinalGbuffer(
    inout AGbuffer gb,
    ASurface s)
{
    gb.target0.rgb = s.baseColor;
    gb.target0.a = 1.0h;
    gb.target1 = gb.target0;
    gb.target2 = A_ONE4;
    gb.target3 = gb.target0;
}

#endif // ALLOY_SHADERS_TYPE_DECAL_MULTIPLICATIVE_CGINC
