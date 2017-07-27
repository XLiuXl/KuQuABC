// Alloy Physical Shader Framework
// Copyright 2013-2017 RUST LLC.
// http://www.alloy.rustltd.com/

/////////////////////////////////////////////////////////////////////////////////
/// @file Indirect.cginc
/// @brief AIndirect structure, and related methods.
/////////////////////////////////////////////////////////////////////////////////

#ifndef ALLOY_SHADERS_FRAMEWORK_INDIRECT_CGINC
#define ALLOY_SHADERS_FRAMEWORK_INDIRECT_CGINC

#include "HLSLSupport.cginc"
#include "UnityLightingCommon.cginc"

// Use Unity's struct directly to avoid copying since the fields are the same.
#define AIndirect UnityIndirect

/// Constructor. 
/// @return Structure initialized with sane default values.
AIndirect aNewIndirect() {
    AIndirect i;

    UNITY_INITIALIZE_OUTPUT(AIndirect, i);
    i.diffuse = 0.0h;
    i.specular = 0.0h;

    return i;
}

#endif // ALLOY_SHADERS_FRAMEWORK_INDIRECT_CGINC
