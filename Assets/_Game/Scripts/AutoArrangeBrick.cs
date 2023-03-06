using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR

public static class AutoArrangeBrick
{
    [MenuItem("Auto/AutoArrangeBrickSelected")]
    private static void AutoArrangeInSelected()
    {
        // EditorUtility.CollectDeepHierarchy does not include inactive children
        var deeperSelection = Selection.gameObjects;

        int compCount = deeperSelection.Count();

        int unitAmount = Mathf.CeilToInt(Mathf.Sqrt(compCount));

        float unitSpace = 1f;

        int id = 0;

        foreach (var go in deeperSelection)
        {
            float i = id % 20 - 9.5f;
            float j = id / 20 - 10;

            go.transform.position = (Vector3.right * i + Vector3.forward * j) * unitSpace;

            id++;
        }

        Debug.Log($"AutoArrange {compCount} GameObjects");
    }

    // Prefabs can both be nested or variants, so best way to clean all is to go through them all
    // rather than jumping straight to the original prefab source.
    //private static void RecursivePrefabSource(GameObject instance, HashSet<Object> prefabs, ref int compCount,
    //    ref int goCount)
    //{
    //    var source = PrefabUtility.GetCorrespondingObjectFromSource(instance);
    //    // Only visit if source is valid, and hasn't been visited before
    //    if (source == null || !prefabs.Add(source))
    //        return;

    //    // go deep before removing, to differantiate local overrides from missing in source
    //    RecursivePrefabSource(source, prefabs, ref compCount, ref goCount);

    //    int count = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(source);
    //    if (count > 0)
    //    {
    //        Undo.RegisterCompleteObjectUndo(source, "Remove missing scripts");
    //        GameObjectUtility.RemoveMonoBehavioursWithMissingScript(source);
    //        compCount += count;
    //        goCount++;
    //    }
    //}
}
#endif