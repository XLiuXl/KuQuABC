using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectionProbeParams : MonoBehaviour {

    [SerializeField]
    public Texture bakedTexture;
    [SerializeField]
    bool m_applied = false;
    
    void Awake()
    {
        ApplyRelectionProbe();
    }

    void ApplyRelectionProbe()
    {
        if (m_applied)
            return;

        if (GetComponent<ReflectionProbe>() == null)
            return;

        if (GetComponent<ReflectionProbe>() != null &&
            GetComponent<ReflectionProbeParams>() != null)
        {
            GetComponent<ReflectionProbe>().bakedTexture = bakedTexture;
            m_applied = true;
        }
    }
    
}
