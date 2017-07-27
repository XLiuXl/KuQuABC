// Alloy Physical Shader Framework
// Copyright 2013-2017 RUST LLC.
// http://www.alloy.rustltd.com/

/////////////////////////////////////////////////////////////////////////////////
/// @file Dissolve.cginc
/// @brief Surface dissolve effects.
/////////////////////////////////////////////////////////////////////////////////

#ifndef ALLOY_SHADERS_FEATURE_DISSOLVE_CGINC
#define ALLOY_SHADERS_FEATURE_DISSOLVE_CGINC

#ifdef _DISSOLVE_ON
    #ifndef A_EMISSIVE_COLOR_ON
        #define A_EMISSIVE_COLOR_ON
    #endif
    
    #ifndef A_SHADOW_SURFACE_ON
        #define A_SHADOW_SURFACE_ON
    #endif
#endif

#include "Assets/Alloy/Shaders/Framework/Feature.cginc"

#ifdef _DISSOLVE_ON
    /// Dissolve glow tint color.
    /// Expects a linear HDR color with alpha.
    half4 _DissolveGlowColor; 
    
    /// Dissolve glow color with effect ramp in the alpha.
    /// Expects an RGBA map with sRGB sampling.
    A_SAMPLER_2D(_DissolveTex);
    
    /// The cutoff value for the dissolve effect in the ramp map.
    /// Expects values in the range [0,1].
    half _DissolveCutoff;

    #ifndef A_DISSOLVE_GLOW_OFF
        /// The weight of the dissolve glow effect.
        /// Expects linear space value in the range [0,1].
        half _DissolveGlowWeight;
    
        /// The width of the dissolve glow effect.
        /// Expects values in the range [0,1].
        half _DissolveEdgeWidth;
    #endif
#endif

void aDissolve(
    inout ASurface s) 
{
#ifdef _DISSOLVE_ON
    float2 dissolveUv = A_TEX_TRANSFORM_UV(s, _DissolveTex);
    half4 dissolveBase = _DissolveGlowColor * tex2D(_DissolveTex, dissolveUv);
    half dissolveCutoff = s.mask * _DissolveCutoff;
    half clipval = dissolveBase.a * 0.99h - dissolveCutoff;

    clip(clipval); // NOTE: Eliminates need for blend edge.
        
	#ifndef A_DISSOLVE_GLOW_OFF
		// Dissolve glow
        half3 glow = s.emissiveColor + dissolveBase.rgb * _DissolveGlowWeight;

        glow = clipval >= _DissolveEdgeWidth ? s.emissiveColor : glow; // Outer edge.
        s.emissiveColor = dissolveCutoff < A_EPSILON ? s.emissiveColor : glow; // Kill when cutoff is zero.
    #endif
#endif
} 

#endif // ALLOY_SHADERS_FEATURE_DISSOLVE_CGINC
