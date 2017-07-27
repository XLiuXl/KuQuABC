// Alloy Physical Shader Framework
// Copyright 2013-2017 RUST LLC.
// http://www.alloy.rustltd.com/

///////////////////////////////////////////////////////////////////////////////
/// @file Standard.cginc
/// @brief Standard model inputs and outputs.
///////////////////////////////////////////////////////////////////////////////

#ifndef ALLOY_SHADERS_TYPE_STANDARD_CGINC
#define ALLOY_SHADERS_TYPE_STANDARD_CGINC

#include "Assets/Alloy/Shaders/Framework/Type.cginc"

void aVertex(
    inout AVertex v)
{
    aStandardVertex(v);
}

void aFinalColor(
    inout half4 color,
    ASurface s)
{
    UNITY_APPLY_FOG(s.fogCoord, color);
}

void aFinalGbuffer(
    inout AGbuffer gb,
    ASurface s)
{

}

#endif // ALLOY_SHADERS_TYPE_STANDARD_CGINC
