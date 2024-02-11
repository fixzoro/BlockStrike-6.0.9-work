using System;
using System.Collections.Generic;
using FreeJSON;
using UnityEngine;

public class LevelManager
{
    public static bool customScene = false;

    private static Dictionary<GameMode, List<string>> gameModeScenes = new Dictionary<GameMode, List<string>>();

    private static List<string> scenes = new List<string>();

    public static void Init()
	{
		if (LevelManager.gameModeScenes.Count != 0)
		{
			return;
		}
		TextAsset textAsset = Resources.Load("Others/SceneManager") as TextAsset;
		JsonArray jsonArray = JsonArray.Parse(Utils.XOR(textAsset.text));
		for (int i = 0; i < jsonArray.Length; i++)
		{
			GameMode key = jsonArray.Get<JsonObject>(i).Get<GameMode>("GameMode");
			List<string> list = new List<string>();
			JsonArray jsonArray2 = jsonArray.Get<JsonObject>(i).Get<JsonArray>("Scenes");
			for (int j = 0; j < jsonArray2.Length; j++)
			{
				list.Add(jsonArray2.Get<string>(j));
				if (!LevelManager.scenes.Contains(jsonArray2.Get<string>(j)))
				{
					LevelManager.scenes.Add(jsonArray2.Get<string>(j));
				}
			}
			LevelManager.gameModeScenes.Add(key, list);
		}
	}

	public static List<string> GetGameModeScenes(GameMode mode)
	{
		LevelManager.Init();
		return LevelManager.gameModeScenes[mode];
	}

	public static List<string> GetAllScenes()
	{
		LevelManager.Init();
		return LevelManager.scenes;
	}

	public static bool HasScene(string scene)
	{
		LevelManager.Init();
		return LevelManager.scenes.Contains(scene);
	}

	public static string GetNextScene(GameMode mode)
	{
		return LevelManager.GetNextScene(mode, LevelManager.GetSceneName());
	}

	public static string GetNextScene(GameMode mode, string scene)
	{
		LevelManager.Init();
		if (LevelManager.customScene)
		{
			return scene;
		}
		List<string> list = LevelManager.gameModeScenes[mode];
		int i = 0;
		while (i < list.Count)
		{
			if (scene == list[i])
			{
				if (list.Count - 1 == i)
				{
					return list[0];
				}
				return list[i + 1];
			}
			else
			{
				i++;
			}
		}
		return string.Empty;
	}

	public static bool HasSceneInGameMode(GameMode mode)
	{
		return LevelManager.HasSceneInGameMode(mode, LevelManager.GetSceneName());
	}

	public static bool HasSceneInGameMode(GameMode mode, string scene)
	{
		List<string> list = LevelManager.GetGameModeScenes(mode);
		return list.Contains(scene);
	}

	public static string GetSceneName()
	{
		if (LevelManager.customScene)
		{
			return Application.loadedLevelName;
		}
		string text = Application.loadedLevelName;
		return text;
	}

	private static string GetEncryptSceneName(string name)
	{
		string text = Utils.XOR(name, Utils.test, true);
		return text.Replace("/", "#");
	}

	public static void LoadLevel(string name)
	{
		if (LevelManager.customScene)
		{
			Application.LoadLevel(name);
		}
		else
		{

            Application.LoadLevel(name);
        }
	}
}
