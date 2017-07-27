// Alloy Physical Shader Framework
// Copyright 2013-2017 RUST LLC.
// http://www.alloy.rustltd.com/

///////////////////////////////////////////////////////////////////////////////
/// @file DecalAlpha.cginc
/// @brief Alpha decal inputs and outputs.
///////////////////////////////////////////////////////////////////////////////

#ifndef ALLOY_SHADERS_TYPE_DECAL_ALPHA_CGINC
#define ALLOY_SHADERS_TYPE_DECAL_ALPHA_CGINC

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
    UNITY_APPLY_FOG(s.fogCoord, color);
}

void aFinalGbuffer(
    inout AGbuffer gb,
    ASurface s)
{
    // Deferred alpha decal two-pass solution.
    // cf http://forum.unity3d.com/threads/how-do-i-write-a-normal-decal-shader-using-a-newly-added-unity-5-2-finalgbuffer-modifier.356644/page-2
#ifdef A_DECAL_ALPHA_FIRSTPASS
    gb.target0.a = s.opacity;
    gb.target1.a = s.opacity;
    gb.target2.a = s.opacity;
    gb.target3.a = s.opacity;
#else
    gb.target0.a *= s.opacity;
    gb.target1.a *= s.opacity;
    gb.target2.a *= s.opacity;
    gb.target3.a *= s.opacity;
#endif
}

#endif // ALLOY_SHADERS_TYPE_DECAL_ALPHA_CGINC
