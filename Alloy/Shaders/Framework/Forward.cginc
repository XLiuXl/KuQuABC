// Alloy Physical Shader Framework
// Copyright 2013-2017 RUST LLC.
// http://www.alloy.rustltd.com/

///////////////////////////////////////////////////////////////////////////////
/// @file Forward.cginc
/// @brief Forward passes uber-header.
///////////////////////////////////////////////////////////////////////////////

#ifndef ALLOY_SHADERS_FRAMEWORK_FORWARD_CGINC
#define ALLOY_SHADERS_FRAMEWORK_FORWARD_CGINC

// Headers both for this file, and for all Definition and Feature modules.
#include "Assets/Alloy/Shaders/Framework/Keywords.cginc"
#include "Assets/Alloy/Shaders/Framework/FeatureImpl.cginc"
#include "Assets/Alloy/Shaders/Framework/SplatImpl.cginc"
#include "Assets/Alloy/Shaders/Framework/SurfaceImpl.cginc"
#include "Assets/Alloy/Shaders/Framework/TypeImpl.cginc"
#include "Assets/Alloy/Shaders/Framework/Utility.cginc"

#include "HLSLSupport.cginc"
#include "UnityCG.cginc"
#include "UnityInstancing.cginc"
#include "UnityShaderUtilities.cginc"
#include "UnityStandardUtils.cginc"

#ifndef A_TWO_SIDED_ON
    #define A_FACING_TYPE
    #define A_FACING_SIGN 1.0h
#else
    #define A_FACING_TYPE ,half facingSign : VFACE
    #define A_FACING_SIGN facingSign
#endif

#if defined(VAPOR_TRANSLUCENT_FOG_ON) && defined(A_FORWARD_PASS)
    #ifndef A_POSITION_WORLD_ON
        #define A_POSITION_WORLD_ON
    #endif
#endif

#ifdef VTRANSPARENCY_ON
    #ifndef A_SCREEN_UV_ON
        #define A_SCREEN_UV_ON
    #endif
    
    #ifndef A_VIEW_DEPTH_ON
        #define A_VIEW_DEPTH_ON
    #endif
#endif

#if !defined(A_NORMAL_WORLD_ON) && defined(A_REFLECTION_VECTOR_WORLD_ON)
    #define A_NORMAL_WORLD_ON
#endif

#if !defined(A_VIEW_DIR_WORLD_ON) && (defined(A_VIEW_DIR_TANGENT_ON) || defined(A_REFLECTION_VECTOR_WORLD_ON))
    #define A_VIEW_DIR_WORLD_ON
#endif

#if !defined(A_POSITION_WORLD_ON) && defined(A_VIEW_DIR_WORLD_ON)
    #define A_POSITION_WORLD_ON
#endif

#if !defined(A_SCREEN_UV_ON) && defined(LOD_FADE_CROSSFADE)
    #define A_SCREEN_UV_ON
#endif

#if !defined(A_POSITION_TEXCOORD_ON) && (defined(A_POSITION_WORLD_ON) || defined(A_VIEW_DEPTH_ON))
    #define A_POSITION_TEXCOORD_ON
#endif

#if defined(A_FOG_ON) || defined(A_SCREEN_UV_ON) || defined(A_COMPUTE_VERTEX_SCREEN_UV)
    #define A_FOG_TEXCOORD_ON
#endif

#if defined(A_WORLD_TO_OBJECT_ON) && defined(A_INSTANCING_PASS)
    #define A_TRANSFER_INSTANCE_ID_ON
#endif

// Split vertex data conditions to reduce combinations.
// UV0-1, TBN, N.
#if defined(A_TANGENT_TO_WORLD_ON)    
    #define A_INNER_VERTEX_DATA2(A, B, C, D) \
        float4 texcoords : TEXCOORD##A; \
        half3 tangentWorld : TEXCOORD##B; \
        half3 bitangentWorld : TEXCOORD##C; \
        half3 normalWorld : TEXCOORD##D;
#elif defined(A_NORMAL_WORLD_ON)
    #define A_INNER_VERTEX_DATA2(A, B, C, D) \
        float4 texcoords : TEXCOORD##A; \
        half3 normalWorld : TEXCOORD##B;
#else
    #define A_INNER_VERTEX_DATA2(A, B, C, D) \
        float4 texcoords : TEXCOORD##A;
#endif

// Vertex Color, Position, View Depth, Fog, and Screen UV.
#if defined(A_POSITION_TEXCOORD_ON) && defined(A_FOG_TEXCOORD_ON)
    #define A_INNER_VERTEX_DATA1(A, B, C, D, E, F, G) \
        half4 color : TEXCOORD##A; \
        float4 positionWorldAndViewDepth : TEXCOORD##B; \
        UNITY_FOG_COORDS_PACKED(C, half4) \
        A_INNER_VERTEX_DATA2(D, E, F, G)
#elif defined(A_POSITION_TEXCOORD_ON)
    #define A_INNER_VERTEX_DATA1(A, B, C, D, E, F, G) \
        half4 color : TEXCOORD##A; \
        float4 positionWorldAndViewDepth : TEXCOORD##B; \
        A_INNER_VERTEX_DATA2(C, D, E, F)
#elif defined(A_FOG_TEXCOORD_ON)
    #define A_INNER_VERTEX_DATA1(A, B, C, D, E, F, G) \
        half4 color : TEXCOORD##A; \
        UNITY_FOG_COORDS_PACKED(C, half4) \
        A_INNER_VERTEX_DATA2(D, E, F, G)
#else
    #define A_INNER_VERTEX_DATA1(A, B, C, D, E, F, G) \
        half4 color : TEXCOORD##A; \
        A_INNER_VERTEX_DATA2(B, C, D, E)
#endif

// Instancing.
#if defined(A_TRANSFER_INSTANCE_ID_ON) && defined(A_STEREO_PASS)
    #define A_INSTANCING_VERTEX_DATA(A, B, C, D, E, F, G) A_INNER_VERTEX_DATA1(A, B, C, D, E, F, G) UNITY_VERTEX_INPUT_INSTANCE_ID UNITY_VERTEX_OUTPUT_STEREO
#elif defined(A_TRANSFER_INSTANCE_ID_ON)
    #define A_INSTANCING_VERTEX_DATA(A, B, C, D, E, F, G) A_INNER_VERTEX_DATA1(A, B, C, D, E, F, G) UNITY_VERTEX_INPUT_INSTANCE_ID
#elif defined(A_STEREO_PASS)
    #define A_INSTANCING_VERTEX_DATA(A, B, C, D, E, F, G) A_INNER_VERTEX_DATA1(A, B, C, D, E, F, G) UNITY_VERTEX_OUTPUT_STEREO
#else
    #define A_INSTANCING_VERTEX_DATA(A, B, C, D, E, F, G) A_INNER_VERTEX_DATA1(A, B, C, D, E, F, G)
#endif

// Surface shader off.
#if defined(A_SURFACE_SHADER_OFF)
    #define A_VERTEX_DATA(A, B, C, D, E, F, G) 
#else
    #define A_VERTEX_DATA(A, B, C, D, E, F, G) A_INSTANCING_VERTEX_DATA(A, B, C, D, E, F, G)
#endif

/// Extensible Vertex to Fragment data transfer structure.
struct AVertexToFragment {
#if defined(A_FORWARD_TEXCOORD0) && defined(A_FORWARD_TEXCOORD1) && defined(A_FORWARD_TEXCOORD2)
    A_FORWARD_TEXCOORD0
    A_FORWARD_TEXCOORD1
    A_FORWARD_TEXCOORD2
    A_VERTEX_DATA(3, 4, 5, 6, 7, 8, 9)
#elif defined(A_FORWARD_TEXCOORD0) && defined(A_FORWARD_TEXCOORD1)
    A_FORWARD_TEXCOORD0
    A_FORWARD_TEXCOORD1
    A_VERTEX_DATA(2, 3, 4, 5, 6, 7, 8)
#elif defined(A_FORWARD_TEXCOORD0)
    A_FORWARD_TEXCOORD0
    A_VERTEX_DATA(1, 2, 3, 4, 5, 6, 7)
#else
    A_VERTEX_DATA(0, 1, 2, 3, 4, 5, 6)
#endif
};

// TODO: Find a way to move this dependency!
#include "Assets/Alloy/Shaders/Framework/Tessellation.cginc"

/// Transfers the per-vertex surface data to the pixel shader.
/// @param[in,out]  v       Vertex input data.
/// @param[out]     o       Vertex to fragment transfer data.
/// @param[out]     opos    Clip space position.
void aForwardVertex(
    inout AVertex v,
    out AVertexToFragment o, 
    out float4 opos)
{
#ifdef A_SURFACE_SHADER_OFF
    opos = 0.0h;
#else
    UNITY_INITIALIZE_OUTPUT(AVertexToFragment, o);

    #ifdef A_INSTANCING_PASS
        UNITY_SETUP_INSTANCE_ID(v);

        #ifdef A_TRANSFER_INSTANCE_ID_ON
            UNITY_TRANSFER_INSTANCE_ID(v, o);
        #endif

        #ifdef A_STEREO_PASS
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
        #endif
    #endif

    aVertex(v);

    o.color = v.color; // Gamma-space vertex color, unless modified.
    o.texcoords.xy = v.uv0.xy;
    o.texcoords.zw = v.uv1.xy;
    opos = UnityObjectToClipPos(v.vertex.xyz);
    
    #ifdef A_POSITION_TEXCOORD_ON
        #ifdef A_POSITION_WORLD_ON
            o.positionWorldAndViewDepth.xyz = mul(unity_ObjectToWorld, v.vertex).xyz;
        #endif

        #ifdef A_VIEW_DEPTH_ON
            COMPUTE_EYEDEPTH(o.positionWorldAndViewDepth.w);
        #endif
    #endif
        
    #ifdef A_NORMAL_WORLD_ON
        float3 normalWorld = UnityObjectToWorldNormal(v.normal);
    #endif

    #ifndef A_TANGENT_TO_WORLD_ON
        #ifdef A_NORMAL_WORLD_ON
            o.normalWorld = normalWorld;
        #endif
    #else
        float3 tangentWorld = UnityObjectToWorldDir(v.tangent.xyz);
        float3x3 tangentToWorld = CreateTangentToWorldPerVertex(normalWorld, tangentWorld, v.tangent.w);

        o.tangentWorld = tangentToWorld[0];
        o.bitangentWorld = tangentToWorld[1];
        o.normalWorld = tangentToWorld[2];
    #endif
        
    #ifdef A_FOG_TEXCOORD_ON
        #if defined(A_SCREEN_UV_ON) || defined(A_COMPUTE_VERTEX_SCREEN_UV)
            o.fogCoord.yzw = ComputeScreenPos(opos).xyw;
        #else
            o.fogCoord.yzw = A_AXIS_Z;
        #endif

        UNITY_TRANSFER_FOG(o, opos);
    #endif
#endif
}

/// Create a ASurface populated with data from the vertex shader.
/// @param  i           Vertex to fragment transfer data.
/// @param  facingSign  Sign of front/back facing direction.
/// @return             Initialized surface data object.
ASurface aForwardSurfaceShader(
    AVertexToFragment i,
    half facingSign)
{
    ASurface s = aNewSurface();

#ifndef A_SURFACE_SHADER_OFF
    #ifdef A_TRANSFER_INSTANCE_ID_ON
        UNITY_SETUP_INSTANCE_ID(i);
    #endif

    s.uv01 = i.texcoords;
    s.vertexColor = i.color;
    s.facingSign = facingSign;

    #ifdef A_POSITION_TEXCOORD_ON
        #ifdef A_POSITION_WORLD_ON
            s.positionWorld = i.positionWorldAndViewDepth.xyz;
        #endif
        
        #ifdef A_VIEW_DEPTH_ON
            s.viewDepth = i.positionWorldAndViewDepth.w;
        #endif
    #endif

    #ifdef A_NORMAL_WORLD_ON
        #ifdef A_TWO_SIDED_ON
            i.normalWorld.xyz *= facingSign;
        #endif

        // Give these sane defaults in case the surface shader doesn't set them.
        s.vertexNormalWorld = normalize(i.normalWorld);
        s.normalWorld = s.vertexNormalWorld;
        s.ambientNormalWorld = s.vertexNormalWorld;
    #endif

    #ifdef A_VIEW_DIR_WORLD_ON
        // Cheaper to calculate in PS than to unpack from vertex, while also
        // preventing distortion in POM and area light specular highlights.
        s.viewDirWorld = normalize(UnityWorldSpaceViewDir(s.positionWorld));
    #endif

    #ifdef A_TANGENT_TO_WORLD_ON
        half3 t = i.tangentWorld;
        half3 b = i.bitangentWorld;
        half3 n = i.normalWorld;
        
        #if UNITY_TANGENT_ORTHONORMALIZE
            n = normalize(n);
    
            // ortho-normalize Tangent
            t = normalize (t - n * dot(t, n));

            // recalculate Binormal
            half3 newB = cross(n, t);
            b = newB * sign (dot (newB, b));
        #endif

        s.tangentToWorld = half3x3(t, b, n);

        #if defined(A_VIEW_DIR_WORLD_ON) && defined(A_VIEW_DIR_TANGENT_ON)
            s.viewDirTangent = normalize(mul(s.tangentToWorld, s.viewDirWorld));
        #endif
    #endif
        
    #ifdef A_FOG_TEXCOORD_ON
        #ifdef A_FOG_ON
            s.fogCoord = i.fogCoord;
        #endif

        #ifdef A_SCREEN_UV_ON
            s.screenPosition.xyw = i.fogCoord.yzw;
            s.screenPosition.z = 0.0h;
            s.screenUv.xy = s.screenPosition.xy / s.screenPosition.w;

            #ifdef LOD_FADE_CROSSFADE
                half2 projUV = s.screenUv.xy * _ScreenParams.xy * 0.25h;

                projUV.y = frac(projUV.y) * 0.0625h /* 1/16 */ + unity_LODFade.y; // quantized lod fade by 16 levels
                clip(tex2D(_DitherMaskLOD2D, projUV).a - 0.5f);
            #endif
        #endif
    #endif

    // Runs the shader and lighting type's surface code.
    aBaseUvInit(s);
    aUpdateViewData(s);

    aSurfaceShader(s);

    #if !defined(A_LIGHTING_ON) && !defined(A_BRDF_INPUTS_ON)
        // Unlit (kill everything except normals and emission).
        s.albedo = 0.0h;
        s.specularOcclusion = 0.0h;
        s.f0 = 0.0h;
        s.roughness = 1.0h;
        s.materialType = 1.0h;
        s.subsurface = 0.0h;
    #else
        s.baseColor = saturate(s.baseColor);
        s.albedo = s.baseColor;
        s.f0 = aSpecularityToF0(s.specularity);

        #ifdef A_SPECULAR_TINT_ON
            s.f0 *= aLerpWhiteTo(aChromaticity(s.baseColor), s.specularTint);
        #endif

        #ifdef A_METALLIC_ON
            half metallicInv = 1.0h - s.metallic;

            s.albedo *= metallicInv; // Affects transmission through albedo.
            s.f0 = lerp(s.f0, s.baseColor, s.metallic);

            #ifdef _ALPHAPREMULTIPLY_ON
                // Interpolate from a translucent dielectric to an opaque metal.
                s.opacity = s.metallic + metallicInv * s.opacity;
            #endif
        #endif

        #ifdef A_CLEARCOAT_ON
            // f0 of 0.04 gives us a polyurethane-like coating.
            half clearCoat = s.clearCoat * lerp(0.04h, 1.0h, s.FV);

            s.albedo *= aLerpOneTo(0.0h, clearCoat);
            s.f0 = lerp(s.f0, A_WHITE, clearCoat);
            s.roughness = lerp(s.roughness, s.clearCoatRoughness, clearCoat);
        #endif

        #ifdef _ALPHAPREMULTIPLY_ON
            // Premultiply opacity with albedo for translucent shaders.
            s.albedo *= s.opacity;
        #endif

        s.beckmannRoughness = aLinearToBeckmannRoughness(s.roughness);

        #ifndef A_AMBIENT_OCCLUSION_ON
            s.specularOcclusion = 1.0h;
        #else
            s.specularOcclusion = aSpecularOcclusion(s.ambientOcclusion, s.NdotV);
        #endif
    #endif

    #ifdef A_LIGHTING_ON
        aPreLighting(s);
    #endif
#endif

    return s;
}

/// Final processing of the forward color before output.
/// @param  s       Material surface data.
/// @param  color   Lighting + Emission + Fog + etc.
/// @return         Final HDR output color with alpha opacity.
half4 aForwardColor(
    ASurface s,
    half3 color)
{
    half4 output;
    output.rgb = color;

#if defined(A_ALPHA_BLENDING_ON) && !defined(A_PASS_DISTORT)
    output.a = s.opacity;
#else
    UNITY_OPAQUE_ALPHA(output.a);
#endif

#if defined(VAPOR_TRANSLUCENT_FOG_ON)
    #if defined(UNITY_PASS_FORWARDBASE) || defined(A_PASS_DISTORT)
        output = VaporApplyFog(s.positionWorld, output);
    #else
        output = VaporApplyFogAdd(s.positionWorld, output);
    #endif
#elif defined(VTRANSPARENCY_ON)
    float4 data = s.screenPosition;

    data.z = s.viewDepth;

    #if defined(UNITY_PASS_FORWARDBASE) || defined(A_PASS_DISTORT)
        output = VolumetricTransparencyBase(output, data);
    #else
        output = VolumetricTransparencyAdd(output, data);
    #endif
#endif

    aFinalColor(output, s);
    return aHdrClamp(output);
}

#endif // ALLOY_SHADERS_FRAMEWORK_FORWARD_CGINC
