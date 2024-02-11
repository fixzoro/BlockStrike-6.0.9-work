using UnityEngine;
using System.Collections;
using UnityEditor;
using BSCM;
using BSCM.Game;
using BSCM.Game.Others;
using BSCM.Game.Modes.BunnyHop;
using BSCM.Game.Modes.DeathRun;

public class FunctionsEditor : EditorWindow {
	

	//================================================================

	[MenuItem("BSCM/Create/Others/Climb",false,0)]
	static void CreateOthersClimb(){
		GameObject go = new GameObject("Climb");
		go.AddComponent<Climb>();
		BoxCollider collider = go.AddComponent<BoxCollider>();
		collider.isTrigger = true;
		Selection.activeGameObject = go;
	}

	[MenuItem("BSCM/Create/Others/KillTrigger",false,0)]
	static void CreateOthersDeadTrigger(){
		GameObject go = new GameObject("KillTrigger");
		go.AddComponent<KillTrigger>();
		BoxCollider collider = go.AddComponent<BoxCollider>();
		collider.isTrigger = true;
		Selection.activeGameObject = go;
	}

	[MenuItem("BSCM/Create/Others/Teleport",false,0)]
	static void CreateOthersTeleport(){
		GameObject go = new GameObject("Teleport");
		go.AddComponent<Teleport>();
		BoxCollider collider = go.AddComponent<BoxCollider>();
		collider.isTrigger = true;
		Selection.activeGameObject = go;
	}

	[MenuItem("BSCM/Create/Others/Water",false,0)]
	static void CreateOthersWater(){
		GameObject go = new GameObject("Water");
		go.AddComponent<Water>();
		BoxCollider collider = go.AddComponent<BoxCollider>();
		collider.isTrigger = true;
		Selection.activeGameObject = go;
	}

	[MenuItem("BSCM/Create/Others/Ice",false,0)]
	static void CreateOthersIce(){
		GameObject go = new GameObject("Ice");
		go.AddComponent<IceTrigger>();
		BoxCollider collider = go.AddComponent<BoxCollider>();
		collider.isTrigger = true;
		Selection.activeGameObject = go;
	}

	[MenuItem("BSCM/Create/Others/EventsTrigger",false,0)]
	static void CreateOthersEventsTrigger(){
		GameObject go = new GameObject("EventsTrigger");
		go.AddComponent<EventsTrigger>();
		BoxCollider collider = go.AddComponent<BoxCollider>();
		collider.isTrigger = true;
		Selection.activeGameObject = go;
	}

	//================================================================

	[MenuItem("BSCM/Add/Others/Move Object",false,0)]
	static void AddOthersMoveObject(){
		if(Selection.activeGameObject != null){
			Selection.activeGameObject.AddComponent<MoveObject>();
		}
	}

	[MenuItem("BSCM/Add/Others/Animate Material",false,0)]
	static void AddOthersAnimateMaterial(){
		if(Selection.activeGameObject != null){
			Selection.activeGameObject.AddComponent<AnimateMaterial>();
		}
	}

	//================================================================

//	[MenuItem("BSCM/Add/Modes/DeathRun/SetActive",false,0)]
//	static void AddModesDeathRunSetActive(){
//		if(Selection.activeGameObject != null){
//			Selection.activeGameObject.AddComponent<SetActive>();
//		}
//	}

	//================================================================

	[MenuItem("BSCM/Create/Modes/BunnyHop/Checkpoint",false,0)]
	static void CreateModesBunnyHopCheckpoint(){
		GameObject go = new GameObject("Checkpoint");
		go.AddComponent<Checkpoint>();
		BoxCollider collider = go.AddComponent<BoxCollider>();
		collider.isTrigger = true;
		Selection.activeGameObject = go;
	}

	[MenuItem("BSCM/Create/Modes/BunnyHop/FinishTrigger",false,0)]
	static void CreateModesBunnyHopFinishTrigger(){
		GameObject go = new GameObject("FinishTrigger");
		go.AddComponent<FinishTrigger>();
		go.AddComponent<BoxCollider>();
		Selection.activeGameObject = go;
	}

	[MenuItem("BSCM/Create/Modes/BunnyHop/JumpTrigger",false,0)]
	static void CreateModesBunnyHopJumpTrigger(){
		GameObject go = new GameObject("JumpTrigger");
		go.AddComponent<JumpTrigger>();
		BoxCollider collider = go.AddComponent<BoxCollider>();
		collider.isTrigger = true;
		Selection.activeGameObject = go;
	}

	[MenuItem("BSCM/Create/Scene Settings",false,120)]
	static void CreateSceneSettings(){
		GameObject go = new GameObject("SceneSettings");
		go.AddComponent<SceneSettings>();
		Selection.activeGameObject = go;
	}

	//================================================================

//	[MenuItem("BSCM/Create/Modes/DeathRun/Button",false,0)]
//	static void CreateModesBunnyDeathRunButton(){
//		GameObject go = (GameObject)Instantiate(AssetDatabase.LoadAssetAtPath("Assets/BlockStrikeMapCreator/Prefabs/Modes/DeathRun/Button.prefab",typeof(GameObject)));
//		go.name = "Button";
//		Selection.activeGameObject = go;
//	}

	//================================================================

	[MenuItem("BSCM/Create/Modes/Bomb/Settings",false,0)]
	static void CreateModesBombSettings(){
		GameObject go = (GameObject)Instantiate(AssetDatabase.LoadAssetAtPath("Assets/BlockStrikeMapCreator/Prefabs/Modes/Bomb/BombSettings.prefab",typeof(GameObject)));
		Selection.activeGameObject = go;
	}

	//================================================================
	
	[MenuItem("BSCM/Settings/Lightmap/Default Lightmap Value",false,1)]
	static void SetDefaultLigthmapValue() {
		LightmapSettings.lightmapsMode = LightmapsMode.NonDirectional;
		LightmapEditorSettings.quality = LightmapBakeQuality.High;
		LightmapEditorSettings.bounces = 4;
		LightmapEditorSettings.skyLightColor = new Color32(219,237,255,255);
		LightmapEditorSettings.skyLightIntensity = 0.2f;
		LightmapEditorSettings.bounceBoost = 1;
		LightmapEditorSettings.bounceIntensity = 1;
		LightmapEditorSettings.finalGatherRays = 1000;
		LightmapEditorSettings.finalGatherContrastThreshold = 0.2f;
		LightmapEditorSettings.finalGatherGradientThreshold = 0.3f;
		LightmapEditorSettings.finalGatherInterpolationPoints = 15;
		LightmapEditorSettings.aoAmount = 0.5f;
		LightmapEditorSettings.aoMaxDistance = 0.1f;
		LightmapEditorSettings.aoContrast = 1;
		LightmapEditorSettings.lockAtlas = false;
		LightmapEditorSettings.realtimeResolution = 8;
		LightmapEditorSettings.padding = 0;
		LightmapEditorSettings.textureCompression = false;
	}

	[MenuItem("BSCM/Settings/Lightmap/Set Lightmap Ambient",false,1)]
	static void SetLightmapAmbient () {
		RenderSettings.ambientLight = new Color32(70,78,90,255);
	}
	[MenuItem("BSCM/Settings/Lightmap/Set Realtime Ambient",false,1)]
	static void SetRealtimeAmbient () {
		RenderSettings.ambientLight = new Color32(156,174,200,255);
	}


	[MenuItem("BSCM/Create/Others/Light",false,1)]
	static void AddLight () {
		GameObject go = (GameObject)Instantiate(AssetDatabase.LoadAssetAtPath("Assets/BlockStrikeMapCreator/Prefabs/Others/Light.prefab",typeof(GameObject)));
		go.name = "Light";
		Selection.activeGameObject = go;
	}

	[MenuItem("BSCM/Settings/Lightmap/Auto Lightmap",false,120)]
	static void AutoLightmap() {
		if(GameObject.Find("Light") == null){
			AddLight();
		}
		SetDefaultLigthmapValue();
		SetLightmapAmbient();
		EditorApplication.update += AutoLightmapUpdate;
		Lightmapping.BakeAsync();
	}
	
	static void AutoLightmapUpdate(){
		if(!Lightmapping.isRunning){
			SetRealtimeAmbient();
			EditorApplication.update -= AutoLightmapUpdate;
		}
	}

	//================================================================

	[MenuItem("BSCM/Help/Tutorial 1",false,100)]
	static void HelpTutorial1(){
		Application.OpenURL("https://docs.google.com/document/d/1ciomnzOWNfy3QOV3YtImdr-yWq6pTU1DW_415JGVb5Q/pub?embedded=true");
	}

	[MenuItem("BSCM/Help/Tutorial 2",false,100)]
	static void HelpTutorial2(){
		Application.OpenURL("https://docs.google.com/document/d/1KT3Kj7aRYrZDE3LMeRM2Gcwuwhk0Avdzqgo0phIWjzI/pub?embedded=true");
	}

	[MenuItem("BSCM/Help/Tutorial 3",false,100)]
	static void HelpTutorial3(){
		Application.OpenURL("https://docs.google.com/document/d/1tllTSo_lM1hafNjflK9z0pLRW0K7kkRLs9DWUmXIXjM/pub?embedded=true");
	}

	[MenuItem("BSCM/Help/Tutorial 4",false,100)]
	static void HelpTutorial4(){
		Application.OpenURL("https://docs.google.com/document/d/1Je8I_qAV-IlEUpK236qiLt-9aKGCwaUIAAWxj3XBFec/pub?embedded=true");
	}

	[MenuItem("BSCM/Help/Tutorial 5",false,100)]
	static void HelpTutorial5(){
		Application.OpenURL(" https://docs.google.com/document/d/1AR4y6gNnAo2TnZuc_I8CKSyfGvORVhAyZR7dL5EfDOg/pub?embedded=true");
	}

	[MenuItem("BSCM/Help/API",false,120)]
	static void HelpApi(){
		Application.OpenURL("https://docs.google.com/document/d/1cdrE66lrUs08ChFOGnN6zjSqE1L-_fFkhFmh6EMGv-8/pub?embedded=true");
	}

	[MenuItem("BSCM/Help/Check Update",false,140)]
	static void HelpCheckUpdate(){
		Application.OpenURL("http://rexetstudio.com/blockstrike/bsmc");
	}

	//================================================================
}
