using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

[CustomEditor(typeof(Hammer))]
public class HammerEditor : Editor
{
    private Hammer hammer;

    void OnEnable()
    {
        hammer = (Hammer)target;
    }


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Selected"))
        {
            hammer.OnSelected();
        }

        if (GUILayout.Button("Deselected"))
        {
            hammer.OnDeselected();
        }

        if (GUILayout.Button("Hit"))
        {
            hammer.OnHit();
        }

    }
}

#endif
