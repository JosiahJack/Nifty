using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// Helper script to put a button in the inspector for me to click and run tests.
[CustomEditor(typeof(Tests))]
public class TestsEditor : Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		Tests testScript = (Tests)target;
		if (GUILayout.Button(testScript.buttonLabel)) testScript.Run();
		EditorGUILayout.BeginVertical();
        GUILayout.Space(8f);
		Color lineColor = new Color(0f,0f,0f,0.8f);
        EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false,1f),lineColor);
        GUILayout.Space(8f);
		EditorGUILayout.EndVertical();
		if (GUILayout.Button("Run Unit Tests")) testScript.RunUnits();
	}
}
