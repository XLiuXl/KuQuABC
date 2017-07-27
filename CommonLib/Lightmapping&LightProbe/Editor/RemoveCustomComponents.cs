using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RemoveCustomComponents : Editor
{
    [MenuItem("GameObject/光影图处理工具/添加光影图组件", false, 0)]
    static void AddLightMapComponent()
    {
        PrefabLightmapData.AddLightMapCmp();
    }


    [MenuItem("GameObject/光影图处理工具/清除所有光影图相关组件", false, 0)]
    static void RemoveAllLightMapComponent()
    {
        RemoveLightMapComponent();
        RemoveEnvRefComponent();
        RemoveRefProbeComponent();
    }

    [MenuItem("GameObject/光影图处理工具/清除光影图组件", false, 0)]
    static void RemoveLightMapComponent()
    {
        foreach (PrefabLightmapData p in FindObjectsOfType<PrefabLightmapData>())
            DestroyImmediate(p);
    }

    [MenuItem("GameObject/光影图处理工具/清除环境反射球组件", false, 0)]
    static void RemoveEnvRefComponent()
    {
        foreach (ReflectionProbeParams p in FindObjectsOfType<ReflectionProbeParams>())
            DestroyImmediate(p);
    }

    [MenuItem("GameObject/光影图处理工具/清除光照探针组件", false, 0)]
    static void RemoveRefProbeComponent()
    {
        foreach (LightProbeParams p in FindObjectsOfType<LightProbeParams>())
            DestroyImmediate(p);
    }
}
