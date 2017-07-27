// Alloy Physical Shader Framework
// Copyright 2013-2017 RUST LLC.
// http://www.alloy.rustltd.com/

/////////////////////////////////////////////////////////////////////////////////
/// @file Meta.cginc
/// @brief Forward meta pass vertex & fragment shaders.
/////////////////////////////////////////////////////////////////////////////////

#ifndef ALLOY_SHADERS_FORWARD_META_CGINC
#define ALLOY_SHADERS_FORWARD_META_CGINC

#define A_BRDF_INPUTS_ON

#include "Assets/Alloy/Shaders/Framework/Forward.cginc"

#include "UnityMetaPass.cginc"

void aVertexShader(
    AVertex v,
    out AVertexToFragment o,
    out float4 opos : SV_POSITION)
{
    aForwardVertex(v, o, opos);
    opos = UnityMetaVertexPosition(v.vertex, v.uv1.xy, v.uv2.xy, unity_LightmapST, unity_DynamicLightmapST);
}

float4 aFragmentShader(
    AVertexToFragment i) : SV_Target
{
    UnityMetaInput o;
    ASurface s = aForwardSurfaceShader(i, 1.0h);

    UNITY_INITIALIZE_OUTPUT(UnityMetaInput, o);

#if defined(EDITOR_VISUALIZATION)
    o.Albedo = s.albedo;
#else
    o.Albedo = s.albedo + (s.f0 * (s.beckmannRoughness * 0.5h));
#endif

    o.SpecularColor = s.f0;

#ifndef A_EMISSIVE_COLOR_ON
    o.Emission = 0.0h;
#else
    o.Emission = aHdrClamp(s.emissiveColor);
#endif

    return UnityMetaFragment(o);
}
            
#endif // ALLOY_SHADERS_FORWARD_META_CGINC
