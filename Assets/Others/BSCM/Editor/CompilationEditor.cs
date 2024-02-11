using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using Object = UnityEngine.Object;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using BSCM;
using UnityEditor.SceneManagement;
using BSCM.Game;
using System.Text;

public class CompilationEditor : EditorWindow{

	public Object map;
	public List<GameMode> modes = new List<GameMode>();

	public static string version = "2.0.0";

	[MenuItem("BSCM/Build Map")]
	static void Init () {
		EditorWindow.GetWindow<CompilationEditor>(true, "Build Map", true);
	}

	void OnGUI(){
		EditorGUILayout.LabelField("Version: " + version);
		EditorGUI.BeginChangeCheck();
		map = EditorGUILayout.ObjectField("Map",map,typeof(Object),false);
		if(EditorGUI.EndChangeCheck()){
			if(Path.GetExtension(AssetDatabase.GetAssetPath(map)) != ".unity"){
				map = null;
			}
			if(map == null){
				modes = new List<GameMode>();
			}else{
				modes = new List<GameMode>();
				if(EditorPrefs.HasKey(map.name + "Modes")){
					string[] m = EditorPrefs.GetString(map.name + "Modes").Split("#"[0]);
					for(int i=0;i<m.Length;i++){
						modes.Add((GameMode)int.Parse(m[i]));
					}
				}
			}
		}

		if(map == null)
			return;

		EditorGUI.BeginChangeCheck();
		for(int i=0;i<modes.Count;i++){
			EditorGUILayout.BeginHorizontal();
			modes[i] = (GameMode)EditorGUILayout.EnumPopup(modes[i]);
			if(GUILayout.Button("X",GUILayout.MaxWidth(20))){
				modes.RemoveAt(i);
			}
			EditorGUILayout.EndHorizontal();
		}
		if(modes.Count < 6){
			if(GUILayout.Button("Add Mode")){
				modes.Add(GameMode.TeamDeathmatch);
			}
		}
		if(EditorGUI.EndChangeCheck()){
			for(int i=0;i<modes.Count - 1;i++){
				if(modes[i] == modes[modes.Count - 1]){
					if(((int)modes[modes.Count - 1]) == (Enum.GetNames(typeof(GameMode)).Length - 1)){
						modes[modes.Count - 1] = (GameMode)0;
					}else{
						modes[modes.Count - 1] = (GameMode)((int)modes[modes.Count - 1] + 1);
					}
					i = 0;
				}
			}
			string[] list = new string[modes.Count];
			for(int i=0;i<list.Length;i++){
				list[i] = ((int)modes[i]).ToString();
			}
			EditorPrefs.SetString(map.name + "Modes",string.Join("#",list));
		}


		if(GUILayout.Button("Build")){

			CheckScene();
			AssetBundleBuild[] buildMap = new AssetBundleBuild[1];
			buildMap[0].assetBundleName = map.name + ".bscm";
			buildMap[0].assetNames = new string[1]{AssetDatabase.GetAssetPath(map)};
			BuildPipeline.BuildAssetBundles("Assets/Others/BSCM/AssetBundles",buildMap,BuildAssetBundleOptions.None,BuildTarget.Android);
            //BuildPipeline.BuildAssetBundles("Assets/BSCM/AssetBundles", buildMap, BuildAssetBundleOptions.None, BuildTarget.Android);

            StringBuilder builder = new StringBuilder();
			string mode = "";
			for(int i=0;i<modes.Count;i++){
				if(i < modes.Count - 1)
					mode += (int)modes[i] + ",";
				else
					mode += ((int)modes[i]).ToString();
			}
			builder.AppendLine("mode=" + mode);

			int hash = (int)(xxHash.CalculateHash(File.ReadAllBytes(Application.dataPath + "/Others/BSCM/AssetBundles/" + map.name + ".bscm")) - int.MaxValue);
			builder.AppendLine("hash=" + hash);

			builder.Append("id=");

			File.WriteAllText(Application.dataPath + "/Others/BSCM/AssetBundles/" + map.name + ".txt",builder.ToString());

			if(File.Exists(Application.dataPath + "/Others/BSCM/AssetBundles/" + map.name.ToLower() + ".bscm")){
				File.Move(Application.dataPath + "/Others/BSCM/AssetBundles/" + map.name.ToLower() + ".bscm",Application.dataPath + "/Others/BSCM/AssetBundles/" + map.name + ".bscm");
			}
			AssetDatabase.Refresh();
		}
	}

	void CheckScene(){
		EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(map));
		GameObject[] go = Resources.FindObjectsOfTypeAll<GameObject>();
		
		for(int i=0;i<go.Length;i++){
			MeshRenderer mr = go[i].GetComponent<MeshRenderer>();
			if(mr != null){
				for(int j=0;j<mr.sharedMaterials.Length;j++){
					int id = DefaultMaterials.Contains(mr.sharedMaterials[j]);
					if(id >= 0){
						MaterialsID cmt = go[i].GetComponent<MaterialsID>();
						if(cmt == null){
							cmt = go[i].AddComponent<MaterialsID>();
						}
						if(!cmt.id.Contains(id)){
							cmt.id.Add(id);
						}
					}
				}
			}
		}
		
		Canvas[] canvas = GameObject.FindObjectsOfType<Canvas>();
		for(int i=0;i<canvas.Length;i++){
			canvas[i].gameObject.SetActive(false);
		}
		
		EventSystem[] es = GameObject.FindObjectsOfType<EventSystem>();
		for(int i=0;i<es.Length;i++){
			es[i].gameObject.SetActive(false);
		}
		
		AudioSource[] audioSource = GameObject.FindObjectsOfType<AudioSource>();
		for(int i=0;i<audioSource.Length;i++){
			audioSource[i].gameObject.SetActive(false);
		}
		
		AudioListener[] audioListener = GameObject.FindObjectsOfType<AudioListener>();
		for(int i=0;i<audioListener.Length;i++){
			audioListener[i].enabled = false;
		}

		Camera[] cam = GameObject.FindObjectsOfType<Camera>();
		for(int i=0;i<cam.Length;i++){
			cam[i].enabled = false;
		}

		TrailRenderer[] trailRenderer = GameObject.FindObjectsOfType<TrailRenderer>();
		for(int i=0;i<trailRenderer.Length;i++){
			trailRenderer[i].enabled = false;
		}
		
		LineRenderer[] lineRenderer = GameObject.FindObjectsOfType<LineRenderer>();
		for(int i=0;i<lineRenderer.Length;i++){
			lineRenderer[i].enabled = false;
		}
		
		LensFlare[] lensFlare = GameObject.FindObjectsOfType<LensFlare>();
		for(int i=0;i<lensFlare.Length;i++){
			lensFlare[i].enabled = false;
		}
		
		Projector[] projector = GameObject.FindObjectsOfType<Projector>();
		for(int i=0;i<projector.Length;i++){
			projector[i].enabled = false;
		}
		
		SpriteRenderer[] spriteRenderer = GameObject.FindObjectsOfType<SpriteRenderer>();
		for(int i=0;i<spriteRenderer.Length;i++){
			spriteRenderer[i].enabled = false;
		}

		EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
	}

}