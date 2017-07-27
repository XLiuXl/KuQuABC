// Alloy Physical Shader Framework
// Copyright 2013-2017 RUST LLC.
// http://www.alloy.rustltd.com/

/////////////////////////////////////////////////////////////////////////////////
/// @file SkinTextures.cginc
/// @brief Main set of textures for Skin shaders.
/////////////////////////////////////////////////////////////////////////////////

#ifndef ALLOY_SHADERS_FEATURE_SKIN_TEXTURES_CGINC
#define ALLOY_SHADERS_FEATURE_SKIN_TEXTURES_CGINC

// Jon Moore recommended this value in his blog post.
#define A_SKIN_BUMP_BLUR_BIAS (3.0)

#ifdef A_SKIN_TEXTURES_ON
    #ifndef A_METALLIC_ON
        #define A_METALLIC_ON
    #endif

    #ifndef A_AMBIENT_OCCLUSION_ON
        #define A_AMBIENT_OCCLUSION_ON
    #endif

    #ifndef A_SUBSURFACE_ON
        #define A_SUBSURFACE_ON
    #endif

    #ifndef A_SCATTERING_ON
        #define A_SCATTERING_ON
    #endif
#endif

#include "Assets/Alloy/Shaders/Framework/Feature.cginc"

void aSkinTextures(
    inout ASurface s)
{
#ifdef A_SKIN_TEXTURES_ON
    half4 base = aBase(s);

    s.baseColor = base.rgb;
    s.subsurface = A_SS(s, base.a);
    s.opacity = 1.0h - base.a;
    aCutout(s);

    half4 material = aSampleMaterial(s);

    s.metallic = _Metal * material.A_METALLIC_CHANNEL;
    s.ambientOcclusion = aLerpOneTo(material.A_AO_CHANNEL, _Occlusion);
    s.specularity = _Specularity * material.A_SPECULARITY_CHANNEL;
    s.roughness = _Roughness * material.A_ROUGHNESS_CHANNEL;

    s.normalTangent = A_NT(s, aSampleBump(s));
    s.blurredNormalTangent = aSampleBumpBias(s, A_SKIN_BUMP_BLUR_BIAS);
#endif
}

#endif // ALLOY_SHADERS_FEATURE_SKIN_TEXTURES_CGINC
