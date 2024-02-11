using System;
using FreeJSON;

public static class ClanManager
{
    public static int admin;

    public static string name;

    public static string tag;

    public static int[] players;

    private static bool loaded;

    public static void Save()
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject.Add("a", ClanManager.admin);
		jsonObject.Add("n", ClanManager.name);
		jsonObject.Add("t", ClanManager.tag);
		jsonObject.Add("p", ClanManager.players);
		CryptoPrefs.SetString("ClanData", jsonObject.ToString());
	}

	public static void Load()
	{
        if (ClanManager.loaded)
		{
			return;
		}
		if (!CryptoPrefs.HasKey("ClanData"))
		{
			return;
		}
		JsonObject jsonObject = JsonObject.Parse(CryptoPrefs.GetString("ClanData"));
		ClanManager.admin = jsonObject.Get<int>("a");
		ClanManager.name = jsonObject.Get<string>("n");
		ClanManager.tag = jsonObject.Get<string>("t");
		ClanManager.players = jsonObject.Get<int[]>("p");
		ClanManager.loaded = true;
	}
}
