using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtensions
{
    public static bool IsInLayerMask(this GameObject obj, LayerMask layerMask)
    {
        return ((layerMask.value & (1 << obj.layer)) > 0);
    }

    public static void DestroyAllChildren(this Transform transform)
    {
        int num = transform.childCount;
        for (int i = 0; i < num; i++)
            Object.DestroyImmediate(transform.GetChild(0).gameObject);
    }
}
