// Alloy Physical Shader Framework
// Copyright 2013-2017 RUST LLC.
// http://www.alloy.rustltd.com/

/////////////////////////////////////////////////////////////////////////////////
/// @file Lighting.cginc
/// @brief Lighting uber-header.
/////////////////////////////////////////////////////////////////////////////////

#ifndef ALLOY_SHADERS_FRAMEWORK_LIGHTING_CGINC
#define ALLOY_SHADERS_FRAMEWORK_LIGHTING_CGINC

// Headers both for this file, and for all Definition and Feature modules.
#include "Assets/Alloy/Shaders/Framework/Direct.cginc"
#include "Assets/Alloy/Shaders/Framework/Indirect.cginc"
#include "Assets/Alloy/Shaders/Framework/Surface.cginc"
#include "Assets/Alloy/Shaders/Framework/Utility.cginc"

#include "UnityCG.cginc"

#define A_HAS_LIGHTING_CALLBACKS

#define A_CULL_MODE_FRONT (1.0h)

#define A_OPAQUE_TYPE (1.0h)
#define A_SHADOWED_SUBSURFACE_TYPE (2.0h / 3.0h)
#define A_UNSHADOWED_SUBSURFACE_TYPE (1.0h / 3.0h)
#define A_SCATTERING_TYPE (0.0h)

#ifndef A_NORMAL_WORLD_ON
    #define A_NORMAL_WORLD_ON
#endif

#ifndef A_VIEW_DIR_WORLD_ON
    #define A_VIEW_DIR_WORLD_ON
#endif
    
#ifndef A_POSITION_WORLD_ON
    #define A_POSITION_WORLD_ON
#endif
        
#if !defined(A_REFLECTION_VECTOR_WORLD_ON) //&& (defined(A_DIRECT_ON) || defined(A_REFLECTION_PROBES_ON))
    #define A_REFLECTION_VECTOR_WORLD_ON
#endif

#if !defined(UNITY_PASS_DEFERRED) || !UNITY_ENABLE_REFLECTION_BUFFERS
    #define A_REFLECTION_PROBES_ON
#endif

#ifndef A_SCATTERING_ON
    #define A_SKIN_SUBSURFACE_WEIGHT 1.0h
#else
    #define A_SKIN_SUBSURFACE_WEIGHT _TransWeight

    #define _skinScatteringMask custom1.x
    #define _skinScattering custom1.y

    #ifdef A_FORWARD_ONLY
        #define A_SCATTERING_LUT _SssBrdfTex
        #define A_SCATTERING_WEIGHT _SssWeight
        #define A_SCATTERING_INV_MASK_CUTOFF 1.0h / _SssMaskCutoff
        #define A_SCATTERING_BIAS _SssBias
        #define A_SCATTERING_SCALE _SssScale
        #define A_SCATTERING_NORMAL_BLUR _SssBumpBlur
        #define A_SCATTERING_ABSORPTION _SssTransmissionAbsorption
        #define A_SCATTERING_AO_COLOR_BLEED _SssColorBleedAoWeights
    #else
        #define A_SCATTERING_LUT _DeferredSkinLut
        #define A_SCATTERING_WEIGHT _DeferredSkinParams.x
        #define A_SCATTERING_INV_MASK_CUTOFF _DeferredSkinParams.y
        #define A_SCATTERING_BIAS _DeferredSkinTransmissionAbsorption.w
        #define A_SCATTERING_SCALE _DeferredSkinColorBleedAoWeights.w
        #define A_SCATTERING_NORMAL_BLUR _DeferredSkinParams.z
        #define A_SCATTERING_ABSORPTION _DeferredSkinTransmissionAbsorption.xyz
        #define A_SCATTERING_AO_COLOR_BLEED _DeferredSkinColorBleedAoWeights.xyz
    #endif
#endif

#ifdef A_SUBSURFACE_ON
    #define _subsurfaceShadowWeight custom1.z

    #ifdef A_FORWARD_ONLY
        #define A_SUBSURFACE_WEIGHT A_SKIN_SUBSURFACE_WEIGHT
        #define A_SUBSURFACE_FALLOFF _TransPower
        #define A_SUBSURFACE_DISTORTION _TransDistortion
        #define A_SUBSURFACE_SHADOW _TransShadowWeight
    #else
        #define A_SUBSURFACE_WEIGHT _DeferredTransmissionParams.x
        #define A_SUBSURFACE_FALLOFF _DeferredTransmissionParams.y
        #define A_SUBSURFACE_DISTORTION _DeferredTransmissionParams.z
        #define A_SUBSURFACE_SHADOW _DeferredTransmissionParams.w
    #endif
#endif

#if !A_USE_DEFERRED_MATERIAL_TYPE_BRANCHING || !defined(A_DEFERRED_SHADING_ON)
    #define A_DEFERRED_BRANCH(CONDITION)
#else
    #define A_DEFERRED_BRANCH(CONDITION) \
        UNITY_BRANCH \
        if (CONDITION)
#endif

#if defined(A_DEFERRED_SHADING_ON) && (defined(A_SCATTERING_ON) || defined(A_SUBSURFACE_ON))
    /// RGB=Blurred normals, A=Subsurface thickness.
    /// Expects value in the buffer alpha.
    sampler2D _DeferredPlusBuffer;
#endif

#ifdef A_SCATTERING_ON
    #ifdef A_FORWARD_ONLY
        /// Pre-Integrated scattering LUT.
        sampler2D _SssBrdfTex;

        /// Weight of the scattering effect.
        /// Expects values in the range [0,1].
        half _SssWeight;

        /// Cutoff value used to convert tranmission data to scattering mask.
        /// Expects values in the range [0.01,1].
        half _SssMaskCutoff;

        /// Biases the thickness value used to look up in the skin LUT.
        /// Expects values in the range [0,1].
        half _SssBias;

        /// Scales the thickness value used to look up in the skin LUT.
        /// Expects values in the range [0,1].
        half _SssScale;

        /// Increases the bluriness of the normal map for diffuse lighting.
        /// Expects values in the range [0,1].
        half _SssBumpBlur;

        /// Per-channel weights for thickness-based subsurface color absorption.
        half3 _SssTransmissionAbsorption;

        /// Per-channel RGB gamma weights for colored AO.
        /// Expects a vector of non-zero values.
        half3 _SssColorBleedAoWeights;

        /// Weight of the subsurface effect.
        /// Expects linear-space values in the range [0,1].
        half _TransWeight;
    #else
        /// Pre-Integrated scattering LUT.
        sampler2D _DeferredSkinLut;

        /// X=Scattering Weight, Y=1/Mask Cutoff, Z=Blur Weight.
        /// Expects a vector of non-zero values.
        half3 _DeferredSkinParams;

        /// Per-channel weights for thickness-based subsurface color absorption.
        /// LUT Bias in alpha.
        half4 _DeferredSkinTransmissionAbsorption;

        /// Per-channel RGB gamma weights for colored AO. LUT Scale in alpha.
        /// Expects a vector of non-zero values.
        half4 _DeferredSkinColorBleedAoWeights;
    #endif
#endif

#ifdef A_SUBSURFACE_ON
    float _ShadowCullMode;

    #ifdef A_FORWARD_ONLY
        /// Shadow influence on the subsurface effect.
        /// Expects values in the range [0,1].
        half _TransShadowWeight;

        /// Amount that the subsurface is distorted by surface normals.
        /// Expects values in the range [0,1].
        half _TransDistortion;

        /// Falloff of the subsurface effect.
        /// Expects values in the range [1,n).
        half _TransPower;
    #else
        /// X=Linear Weight, Y=Falloff, Z=Bump Distortion, W=Shadow Weight.
        /// Expects a vector of non-zero values.
        half4 _DeferredTransmissionParams;
    #endif
#endif

// Allows me to define the callbacks in the shader, after the include headers.
void aPreLighting(inout ASurface s);
half3 aDirectLighting(ADirect d, ASurface s);
half3 aIndirectLighting(AIndirect i, ASurface s);

/// Pre-calculate material data shared between direct & indirect lighting.
/// @param  s   Material surface data.
void aStandardPreLighting(
    inout ASurface s)
{
    // In Forward mode, set material type.
#ifndef A_DEFERRED_SHADING_ON
    #if defined(A_SCATTERING_ON)
        s.materialType = A_SCATTERING_TYPE;
        s._subsurfaceShadowWeight = 0.0h;
        s._skinScatteringMask = 1.0h;
        s.ambientNormalWorld = aTangentToWorld(s, s.blurredNormalTangent);
    #elif defined(A_SUBSURFACE_ON)
        #ifdef A_TWO_SIDED_ON
            s.materialType = A_SHADOWED_SUBSURFACE_TYPE;
            s._subsurfaceShadowWeight = A_SUBSURFACE_SHADOW;
        #else
            s.materialType = _ShadowCullMode == A_CULL_MODE_FRONT ? A_SHADOWED_SUBSURFACE_TYPE : A_UNSHADOWED_SUBSURFACE_TYPE;
            s._subsurfaceShadowWeight = _ShadowCullMode == A_CULL_MODE_FRONT ? A_SUBSURFACE_SHADOW : 0.0h;
        #endif
    #else
        s.materialType = A_OPAQUE_TYPE;
    #endif
#endif

#if defined(A_SCATTERING_ON) || defined(A_SUBSURFACE_ON)
    A_DEFERRED_BRANCH(s.materialType != A_OPAQUE_TYPE)
    {
    // In Deferred mode, determine material type and sample extra G-buffer data.
    #if defined(A_DEFERRED_SHADING_ON)
        half4 buffer = tex2Dlod(_DeferredPlusBuffer, float4(s.screenUv, 0.0f, 0.0f));

        s.subsurface = buffer.a;
        s._subsurfaceShadowWeight = s.materialType == A_SHADOWED_SUBSURFACE_TYPE ? A_SUBSURFACE_SHADOW : 0.0h;

        #if defined(A_SCATTERING_ON)
            s._skinScatteringMask = s.materialType == A_SCATTERING_TYPE ? 1.0h : 0.0h;
            s.ambientNormalWorld = normalize(buffer.xyz * 2.0h - 1.0h);
        #endif
    #endif

    // Subsurface color calculation.
    #ifdef A_SUBSURFACE_ON
        #ifdef A_FORWARD_ONLY
            s.subsurfaceColor *= s.albedo;
        #else
            s.subsurfaceColor = s.albedo * aGammaToLinear(s.subsurface);
        #endif
    #endif

    // Scattering input data.
    #ifdef A_SCATTERING_ON
        A_DEFERRED_BRANCH(s.materialType == A_SCATTERING_TYPE)
        {
            // Scattering mask.
            s._skinScatteringMask *= A_SCATTERING_WEIGHT * saturate(A_SCATTERING_INV_MASK_CUTOFF * s.subsurface);
            s._skinScattering = saturate(s.subsurface * A_SCATTERING_SCALE + A_SCATTERING_BIAS);

            // Skin subsurface depth absorption tint.
            // cf http://www.crytek.com/download/2014_03_25_CRYENGINE_GDC_Schultz.pdf pg 35
            half3 absorption = exp((1.0h - s.subsurface) * A_SCATTERING_ABSORPTION);

            // Albedo scale for absorption assumes ~0.5 luminance for Caucasian skin.
            absorption *= saturate(s.albedo * unity_ColorSpaceDouble.rgb);
            s.subsurfaceColor = lerp(s.subsurfaceColor, absorption, s._skinScatteringMask);

            // Blurred normals for indirect diffuse and direct scattering.
            s.ambientNormalWorld = normalize(lerp(s.normalWorld, s.ambientNormalWorld, A_SCATTERING_NORMAL_BLUR * s._skinScatteringMask));
        }
    #endif

    // Subsurface color weight.
    #ifdef A_SUBSURFACE_ON
        s.subsurfaceColor *= A_SUBSURFACE_WEIGHT;
    #endif
    }
#endif
}

/// Calculate direct illumination from a light and a surface.
/// @param  d   Direct lighting data.
/// @param  s   Material surface data.
/// @return     Direct illumination.
half3 aStandardDirectLighting(
    ADirect d,
    ASurface s)
{
    half3 illum = 0.0h;
    half3 specular = 0.0h;

#if defined(A_SCATTERING_ON) || defined(A_SUBSURFACE_ON)
    A_DEFERRED_BRANCH(s.materialType != A_OPAQUE_TYPE)
    {
    #ifdef A_SUBSURFACE_ON
        // Subsurface transmission.
        // cf http://www.farfarer.com/blog/2012/09/11/translucent-shader-unity3d/
        half3 transLightDir = d.direction + s.normalWorld * A_SUBSURFACE_DISTORTION;
        half transLight = pow(aDotClamp(s.viewDirWorld, -transLightDir), A_SUBSURFACE_FALLOFF);
        half shadow = aLerpOneTo(d.shadow, s._subsurfaceShadowWeight);

        illum += s.subsurfaceColor * (shadow * transLight);
    #endif

    #ifdef A_SCATTERING_ON
        A_DEFERRED_BRANCH(s.materialType == A_SCATTERING_TYPE)
        {
            // Pre-Integrated Skin Shading.
            // cf http://www.farfarer.com/blog/2013/02/11/pre-integrated-skin-shader-unity-3d/
            float ndlBlur = dot(s.ambientNormalWorld, d.direction) * 0.5h + 0.5h;
            float4 sssLookupUv = float4(ndlBlur, s._skinScattering * aLuminance(d.color), 0.0f, 0.0f);
            half3 sss = (s._skinScatteringMask * d.shadow) * tex2Dlod(A_SCATTERING_LUT, sssLookupUv).rgb;

            illum += s.albedo * sss;
            s.albedo *= 1.0h - s._skinScatteringMask;
        }
    #endif
    }
#endif

    // Cook-Torrance microfacet model.
    // cf http://graphicrants.blogspot.com/2013/08/specular-brdf-reference.html
    half LdotH2 = d.LdotH * d.LdotH;

    // Brent Burley diffuse BRDF.
    // cf https://disney-animation.s3.amazonaws.com/library/s2012_pbs_disney_brdf_notes_v2.pdf pg14
    half FL = aFresnel(d.NdotL);
    half Fd90 = 0.5h + (2.0h * LdotH2 * s.roughness);
    half Fd = aLerpOneTo(Fd90, FL) * aLerpOneTo(Fd90, s.FV);
    half3 diffuse = s.albedo * Fd;

#ifndef _SPECULARHIGHLIGHTS_OFF
    // Schlick's Fresnel approximation.
    half3 F = lerp(s.f0, A_WHITE, aFresnel(d.LdotH));

    // John Hable's Visibility function.
    // cf http://www.filmicworlds.com/2014/04/21/optimizing-ggx-shaders-with-dotlh/
    half a2 = s.beckmannRoughness * s.beckmannRoughness;
    half k2 = a2 * 0.25h; // k = a/2; k*k = (a*a)/(2*2) = (a^2)/4.
    half invV = lerp(k2, 1.0h, LdotH2);

    // GGX (Trowbridge-Reitz) NDF.
    // cf http://graphicrants.blogspot.com/2013/08/specular-brdf-reference.html
    half denom = aLerpOneTo(a2, d.NdotH * d.NdotH);
    half mDV = k2 / (invV * denom * denom); // k2 is GGX a^2 and microfacet 1/4.

    specular = F * (mDV * s.specularOcclusion * d.specularIntensity);
#endif

    // Punctual lighting equation.
    // cf http://seblagarde.wordpress.com/2012/01/08/pi-or-not-to-pi-in-game-lighting-equation/
    illum += (d.shadow * d.NdotL) * (diffuse + specular);

    return d.color * illum;
}

/// Calculate indirect illumination from a light and a surface.
/// @param  i   Indirect lighting data.
/// @param  s   Material surface data.
/// @return     Indirect illumination.
half3 aStandardIndirectLighting(
    AIndirect i,
    ASurface s)
{
    half3 illum = 0.0h;

#if defined(A_DEFERRED_SHADING_ON) || defined(A_REFLECTION_PROBES_ON)
    // Brian Karis' modification of Dimitar Lazarov's Environment BRDF.
    // cf https://www.unrealengine.com/blog/physically-based-shading-on-mobile
    const half4 c0 = half4(-1.0h, -0.0275h, -0.572h, 0.022h);
    const half4 c1 = half4(1.0h, 0.0425h, 1.04h, -0.04h);
    half4 r = s.roughness * c0 + c1;
    half a004 = min(r.x * r.x, exp2(-9.28h * s.NdotV)) * r.x + r.y;
    half2 AB = half2(-1.04h, 1.04h) * a004 + r.zw;
    half3 specular = i.specular * (s.f0 * AB.x + AB.yyy);
#endif

#ifdef A_DEFERRED_SHADING_ON
    illum = specular * s.specularOcclusion;
#else
    #ifndef A_AMBIENT_OCCLUSION_ON
        half3 diffuse = s.albedo * i.diffuse;

        #ifndef A_REFLECTION_PROBES_ON
            illum = diffuse;
        #else
            illum = diffuse + specular;
        #endif
    #else
        #ifndef A_SCATTERING_ON
            half ao = s.ambientOcclusion;
        #else
            // Color Bleed AO.
            // cf http://www.iryoku.com/downloads/Next-Generation-Character-Rendering-v6.pptx pg113
            half3 ao = pow(s.ambientOcclusion.rrr, A_ONE - (A_SCATTERING_AO_COLOR_BLEED * s._skinScatteringMask));
        #endif

        // Yoshiharu Gotanda's fake interreflection for specular occlusion.
        // Modified to better account for surface f0.
        // cf http://research.tri-ace.com/Data/cedec2011_RealtimePBR_Implementation_e.pptx pg65
        half3 ambient = i.diffuse * ao;

        #ifndef A_REFLECTION_PROBES_ON
            // Diffuse and fake interreflection only.
            illum = ambient * (s.albedo + s.f0 * (1.0h - s.specularOcclusion));
        #else
            // Full equation.
            illum = ambient * s.albedo
                + lerp(ambient * s.f0, specular, s.specularOcclusion);
        #endif
    #endif
#endif

    return illum;
}

#endif // ALLOY_SHADERS_FRAMEWORK_LIGHTING_CGINC
