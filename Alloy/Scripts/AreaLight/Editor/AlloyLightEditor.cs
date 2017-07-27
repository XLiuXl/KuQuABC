// Alloy Physical Shader Framework
// Copyright 2013-2017 RUST LLC.
// http://www.alloy.rustltd.com/
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Light))]
[CanEditMultipleObjects]
public class AlloyLightEditor : Editor {
	Editor m_editor;

	Type GetTypeGlobal(string typeName) {
		return AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).FirstOrDefault(t => t.Name == typeName);
	}

	void OnEnable() {
		m_editor = CreateEditor(targets, GetTypeGlobal("LightEditor"));
		Undo.undoRedoPerformed += RebindAreaLights;
		RebindAreaLights();
    }

    void OnDisable() {
        Undo.undoRedoPerformed -= RebindAreaLights;
        DestroyImmediate(m_editor);
    }

    public override void OnInspectorGUI() {
		m_editor.OnInspectorGUI();
		bool anyMissing = targets.Any(l => ((Light)l).GetComponent<Light>().type != LightType.Area 
                                        && ((Light)l).GetComponent<AlloyAreaLight>() == null);

		if (anyMissing) {
			if (GUILayout.Button("Convert to Alloy area light", EditorStyles.toolbarButton)) {
				foreach (Light light in targets) {
					Undo.AddComponent<AlloyAreaLight>(light.gameObject);
				}
			}
		}

		if (GUI.changed) {
			RebindAreaLights();
		}
	}

	void RebindAreaLights() {
		var lights = targets.Select(l => ((Light)l).GetComponent<AlloyAreaLight>()).Where(a => a != null);
		
        foreach (AlloyAreaLight ar in lights) {
			ar.UpdateBinding();
		}
	}
}