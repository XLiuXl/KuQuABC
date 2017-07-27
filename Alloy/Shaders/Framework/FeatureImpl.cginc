// Alloy Physical Shader Framework
// Copyright 2013-2017 RUST LLC.
// http://www.alloy.rustltd.com/

/////////////////////////////////////////////////////////////////////////////////
/// @file Feature.cginc
/// @brief Feature method implementations to allow disabling of features.
/////////////////////////////////////////////////////////////////////////////////

#ifndef ALLOY_SHADERS_FRAMEWORK_FEATURE_IMPL_CGINC
#define ALLOY_SHADERS_FRAMEWORK_FEATURE_IMPL_CGINC

#include "Assets/Alloy/Shaders/Config.cginc"
#include "Assets/Alloy/Shaders/Framework/Feature.cginc"
#include "Assets/Alloy/Shaders/Framework/Surface.cginc"
#include "Assets/Alloy/Shaders/Framework/Utility.cginc"

#include "HLSLSupport.cginc"
#include "UnityCG.cginc"

void aBaseUvInit(
    inout ASurface s)
{
    s.baseUv = A_TEX_TRANSFORM_UV_SCROLL(s, _MainTex);
    s.baseTiling = _MainTex_ST.xy;

    // Initialize VirtualCoord here so subsequent calls can be cheaper updates.
#ifdef _VIRTUALTEXTURING_ON
    s.baseVirtualCoord = VTComputeVirtualCoord(s.baseUv);
#endif
}

void aUpdateBaseUv(
    inout ASurface s)
{
#ifdef _VIRTUALTEXTURING_ON
    s.baseVirtualCoord = VTUpdateVirtualCoord(s.baseVirtualCoord, s.baseUv);
#endif
}

void aTwoSided(
    inout ASurface s)
{
#ifdef A_TWO_SIDED_ON
    s.normalTangent.xy = A_NT(s, s.facingSign > 0.0h || _TransInvertBackNormal < 0.5f ? s.normalTangent.xy : -s.normalTangent.xy);
#endif
}

void aCutout(
    ASurface s)
{
#ifdef _ALPHATEST_ON
    clip(s.opacity - _Cutoff);
#endif
}

half4 aSampleBase(
    ASurface s) 
{
    half4 result = 0.0h;

#ifdef _VIRTUALTEXTURING_ON
    result = VTSampleBase(s.baseVirtualCoord);
#else
    result = tex2D(_MainTex, s.baseUv);
#endif
    
    return result;
}

half4 aSampleMaterial(
    ASurface s) 
{
    half4 result = 0.0h;

#ifndef A_EXPANDED_MATERIAL_MAPS
    #ifndef _VIRTUALTEXTURING_ON
        result = tex2D(_SpecTex, s.baseUv);
    #else
        result = VTSampleSpecular(s.baseVirtualCoord);
    #endif

    result.A_AO_CHANNEL = aGammaToLinear(result.A_AO_CHANNEL);
#else
    half3 channels;

    // Assuming sRGB texture sampling, undo it for all but AO.
    channels.x = tex2D(_MetallicMap, s.baseUv).g;
    channels.y = tex2D(_SpecularityMap, s.baseUv).g;
    channels.z = tex2D(_RoughnessMap, s.baseUv).g;
    channels = LinearToGammaSpace(channels);

    result.A_METALLIC_CHANNEL = channels.x;
    result.A_AO_CHANNEL = tex2D(_AoMap, s.baseUv).g;
    result.A_SPECULARITY_CHANNEL = channels.y;
    result.A_ROUGHNESS_CHANNEL = channels.z;
#endif

    return result;
}

half3 aSampleBumpScale(
    ASurface s,
    half scale)
{
    half4 result = 0.0h;

#ifdef _VIRTUALTEXTURING_ON
    result = VTSampleNormal(s.baseVirtualCoord);
#else
    result = tex2D(_BumpMap, s.baseUv);
#endif

    return UnpackScaleNormal(result, scale);
}

half3 aSampleBump(
    ASurface s) 
{
    return aSampleBumpScale(s, _BumpScale);
}

half3 aSampleBumpBias(
    ASurface s,
    float bias) 
{
    half4 result = 0.0h;

#ifdef _VIRTUALTEXTURING_ON
    result = VTSampleNormal(VTComputeVirtualCoord(s.baseUv, bias));
#else
    result = tex2Dbias(_BumpMap, float4(s.baseUv, 0.0h, bias));
#endif

    return UnpackScaleNormal(result, _BumpScale);  
}

half aSampleHeight(
    ASurface s)
{
    half result = 0.0h;

#ifdef _VIRTUALTEXTURING_ON
    result = VTSampleNormal(s.baseVirtualCoord).b;
#else
    result = tex2D(_ParallaxMap, s.baseUv).y;
#endif

    return result;
}

half3 aVertexColorTint(
    ASurface s,
    half strength)
{
#ifdef A_VERTEX_COLOR_IS_DATA
    return A_WHITE;
#else
    return aLerpWhiteTo(s.vertexColor.rgb, strength);
#endif
}

half3 aBaseVertexColorTint(
    ASurface s)
{
    return aVertexColorTint(s, _BaseColorVertexTint);
}

half4 aBaseTint(
    ASurface s)
{
    half4 result = _Color;

#ifndef A_VERTEX_COLOR_IS_DATA
    result.rgb *= aBaseVertexColorTint(s);
#endif

    return result;
}

half4 aBase(
    ASurface s)
{
    return aBaseTint(s) * aSampleBase(s);
}

void aParallaxOffset(
    inout ASurface s,
    float2 offset)
{
    offset *= s.mask;
    
    // To apply the parallax offset to secondary textures without causing swimming,
    // we must normalize it by removing the implicitly multiplied base map tiling. 
    s.uv01 += (offset / s.baseTiling).xyxy;
    s.baseUv += A_BV(s, offset);
}

void aOffsetBumpMapping(
    inout ASurface s)
{
    // NOTE: Prevents NaN compiler errors in DX9 mode for shadow pass.
#if defined(A_TANGENT_TO_WORLD_ON) && defined(A_VIEW_DIR_TANGENT_ON)
    half h = aSampleHeight(s) * _Parallax - _Parallax * 0.5h;
    half3 v = s.viewDirTangent;

    v.z += 0.42h;
    aParallaxOffset(s, h * (v.xy / v.z));
#endif
}

void aParallaxOcclusionMapping(
    inout ASurface s,
    float minSamples,
    float maxSamples)
{
    // NOTE: Prevents NaN compiler errors in DX9 mode for shadow pass.
#if defined(A_TANGENT_TO_WORLD_ON) && defined(A_VIEW_DIR_TANGENT_ON)
    // Parallax Occlusion Mapping
    // Subject to GameDev.net Open License
    // cf http://www.gamedev.net/page/resources/_/technical/graphics-programming-and-theory/a-closer-look-at-parallax-occlusion-mapping-r3262
    float2 offset = float2(0.0f, 0.0f);

    // Calculate the parallax offset vector max length.
    // This is equivalent to the tangent of the angle between the
    // viewer position and the fragment location.
    float parallaxLimit = -length(s.viewDirTangent.xy) / s.viewDirTangent.z;

    // Scale the parallax limit according to heightmap scale.
    parallaxLimit *= _Parallax;

    // Calculate the parallax offset vector direction and maximum offset.
    float2 offsetDirTangent = normalize(s.viewDirTangent.xy);
    float2 maxOffset = offsetDirTangent * parallaxLimit;
    
    // Calculate how many samples should be taken along the view ray
    // to find the surface intersection.  This is based on the angle
    // between the surface normal and the view vector.
    int numSamples = (int)lerp(maxSamples, minSamples, s.NdotV);
    int currentSample = 0;
    
    // Specify the view ray step size.  Each sample will shift the current
    // view ray by this amount.
    float stepSize = 1.0f / (float)numSamples;

    // Initialize the starting view ray height and the texture offsets.
    float currentRayHeight = 1.0f;	
    float2 lastOffset = float2(0.0f, 0.0f);
    
    float lastSampledHeight = 1.0f;
    float currentSampledHeight = 1.0f;

    #ifdef _VIRTUALTEXTURING_ON
        VirtualCoord vcoord = s.baseVirtualCoord;
    #else
        // Calculate the texture coordinate partial derivatives in screen
        // space for the tex2Dgrad texture sampling instruction.
        float2 dx = ddx(s.baseUv);
        float2 dy = ddy(s.baseUv);
    #endif

    while (currentSample < numSamples) {
        #ifdef _VIRTUALTEXTURING_ON
            vcoord = VTUpdateVirtualCoord(vcoord, s.baseUv + offset);
            currentSampledHeight = VTSampleNormal(vcoord).b;
        #else
            // Sample the heightmap at the current texcoord offset.
            currentSampledHeight = tex2Dgrad(_ParallaxMap, s.baseUv + offset, dx, dy).y;
        #endif

        // Test if the view ray has intersected the surface.
        UNITY_BRANCH
        if (currentSampledHeight > currentRayHeight) {
            // Find the relative height delta before and after the intersection.
            // This provides a measure of how close the intersection is to 
            // the final sample location.
            float delta1 = currentSampledHeight - currentRayHeight;
            float delta2 = (currentRayHeight + stepSize) - lastSampledHeight;
            float ratio = delta1 / (delta1 + delta2);

            // Interpolate between the final two segments to 
            // find the true intersection point offset.
            offset = lerp(offset, lastOffset, ratio);
            
            // Force the exit of the while loop
            currentSample = numSamples + 1;	
        }
        else {
            // The intersection was not found.  Now set up the loop for the next
            // iteration by incrementing the sample count,
            currentSample++;

            // take the next view ray height step,
            currentRayHeight -= stepSize;
            
            // save the current texture coordinate offset and increment
            // to the next sample location, 
            lastOffset = offset;
            offset += stepSize * maxOffset;

            // and finally save the current heightmap height.
            lastSampledHeight = currentSampledHeight;
        }
    }

    aParallaxOffset(s, offset);
#endif
}

#endif // ALLOY_SHADERS_FRAMEWORK_FEATURE_IMPL_CGINC
