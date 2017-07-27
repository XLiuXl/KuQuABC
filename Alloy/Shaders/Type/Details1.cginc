// Alloy Physical Shader Framework
// Copyright 2013-2017 RUST LLC.
// http://www.alloy.rustltd.com/

///////////////////////////////////////////////////////////////////////////////
/// @file Details1.cginc
/// @brief Unity Details1 model inputs and outputs.
///////////////////////////////////////////////////////////////////////////////

#ifndef ALLOY_SHADERS_TYPE_DETAILS1_CGINC
#define ALLOY_SHADERS_TYPE_DETAILS1_CGINC

#ifndef A_TEX_UV_OFF
    #define A_TEX_UV_OFF
#endif

#ifndef A_TEX_SCROLL_OFF
    #define A_TEX_SCROLL_OFF
#endif

#ifndef A_SHADOW_SURFACE_ON
    #define A_SHADOW_SURFACE_ON
#endif

#include "Assets/Alloy/Shaders/Framework/Type.cginc"

#include "TerrainEngine.cginc"

void aVertex(
    inout AVertex v)
{
    aStandardVertex(v);

    // Adapt vertex data so we can reuse wind code.
    appdata_full IN;

    UNITY_INITIALIZE_OUTPUT(appdata_full, IN);
    IN.vertex = v.vertex;
    IN.color = v.color;

    WavingGrassVert(IN);
    v.vertex = IN.vertex;
    v.color = IN.color;
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

#endif // ALLOY_SHADERS_TYPE_DETAILS1_CGINC
