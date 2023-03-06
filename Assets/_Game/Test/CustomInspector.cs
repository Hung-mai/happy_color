#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LoadMainGameData))]
public class CustomInspector : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		LoadMainGameData loadData = (LoadMainGameData)target;
		if (GUILayout.Button("Read CSV"))
		{
			loadData.ReadData();
		}
	}
}
#endif