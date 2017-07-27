using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Rendering;

public class LightProbeParams : MonoBehaviour
{
   
    [SerializeField]
    public Vector3[] probePos;
    [Serializable]public class SphericalHarmonics
    {
        public float[] coefficients = new float[27];
    }

    [SerializeField]
    public SphericalHarmonics[] bakedProbes;
    
    [SerializeField]
    bool m_applied = false;

    void Awake()
    {
        ApplyLightProbeGroup();
    }

    void ApplyLightProbeGroup()
    {
        if (m_applied)
            return;

        if(GetComponent<LightProbeGroup>()==null)
            return;

        if (GetComponent<LightProbeGroup>() != null &&
            GetComponent<LightProbeParams>() != null)
        {
            GetComponent<LightProbeGroup>().probePositions = probePos;

            //bakedProbes
            var sphericalHarmonicsArray = new SphericalHarmonicsL2[bakedProbes.Length];

            for (int i = 0; i < bakedProbes.Length; i++)
            {
                var sphericalHarmonics = new SphericalHarmonicsL2();

                // j is coefficient
                for (int j = 0; j < 3; j++)
                {
                    //k is channel ( r g b )
                    for (int k = 0; k < 9; k++)
                    {
                        sphericalHarmonics[j, k] = bakedProbes[i].coefficients[j * 9 + k];
                    }
                }

                sphericalHarmonicsArray[i] = sphericalHarmonics;
            }

            LightmapSettings.lightProbes.bakedProbes = sphericalHarmonicsArray;

            m_applied = true;
        }
    }
}
