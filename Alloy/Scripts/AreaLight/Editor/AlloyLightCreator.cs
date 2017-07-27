// Alloy Physical Shader Framework
// Copyright 2013-2017 RUST LLC.
// http://www.alloy.rustltd.com/

using UnityEditor;
using UnityEngine;

public static class AlloyLightCreator {
    const string c_lightMenuPath = "GameObject/Light/";
    const string c_undoMessage = "Created ";
    const string c_directionalLight = "Alloy Directional Light";
    const string c_pointLight = "Alloy Point Light";
    const string c_spotLight = "Alloy Spotlight";

    [MenuItem(c_lightMenuPath + c_directionalLight)]
	static void CreateDirectionalLight() {
		var go = new GameObject();
		go.transform.position = SceneView.lastActiveSceneView.pivot;

		Undo.RegisterCreatedObjectUndo(go, c_undoMessage + c_directionalLight);
		go.name = c_directionalLight;

		var light = go.AddComponent<Light>();
		light.type = LightType.Directional;

		go.AddComponent<AlloyAreaLight>();
		Selection.activeGameObject = go;
	}
    
	[MenuItem(c_lightMenuPath + c_pointLight)]
	static void CreateSphereAreaLight() {
		var go = new GameObject();

		Undo.RegisterCreatedObjectUndo(go, c_undoMessage + c_pointLight);
		go.name = c_pointLight;

		var light = go.AddComponent<Light>();
		light.type = LightType.Point;

		go.AddComponent<AlloyAreaLight>();
		go.transform.position = SceneView.lastActiveSceneView.pivot;
		Selection.activeGameObject = go;
	}

	[MenuItem(c_lightMenuPath + c_spotLight)]
	static void CreateSpotSphereAreaLight() {
		var go = new GameObject();

		Undo.RegisterCreatedObjectUndo(go, c_undoMessage + c_spotLight);
		go.name = c_spotLight;

		var light = go.AddComponent<Light>();
		light.type = LightType.Spot;

		go.AddComponent<AlloyAreaLight>();
		go.transform.position = SceneView.lastActiveSceneView.pivot;
		Selection.activeGameObject = go;
	}
}