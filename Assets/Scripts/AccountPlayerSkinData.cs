using System;
using System.Collections.Generic;
using FreeJSON;
using UnityEngine;

[Serializable]
public class AccountPlayerSkinData
{
    public List<CryptoInt> Head = new List<CryptoInt>();

    public List<CryptoInt> Body = new List<CryptoInt>();

    public List<CryptoInt> Legs = new List<CryptoInt>();

    public List<CryptoInt> Select = new List<CryptoInt> { 0, 0, 0 };

    [HideInInspector]
    public List<CryptoInt> LastSelect = new List<CryptoInt> { 0, 0, 0 };

    public void Deserialize(JsonObject json)
	{
		if (json == null)
		{
			return;
		}
		this.Head = this.JsonObjectToList(json.Get<JsonObject>("Head"));
		this.Body = this.JsonObjectToList(json.Get<JsonObject>("Body"));
		this.Legs = this.JsonObjectToList(json.Get<JsonObject>("Legs"));
		if (this.Head.Count == 0)
		{
			this.Head = this.JsonArrayToList(json.Get<JsonArray>("Head"));
		}
		if (this.Body.Count == 0)
		{
			this.Body = this.JsonArrayToList(json.Get<JsonArray>("Body"));
		}
		if (this.Legs.Count == 0)
		{
			this.Legs = this.JsonArrayToList(json.Get<JsonArray>("Legs"));
		}
		try
		{
			if (json.ContainsKey("Select"))
			{
				string text = json.Get<string>("Select");
				if (!string.IsNullOrEmpty(text))
				{
					string[] array = text.Split(new char[]
					{
						","[0]
					});
					int value = 0;
					for (int i = 0; i < array.Length; i++)
					{
						int.TryParse(array[i], out value);
						this.Select[i] = value;
						this.LastSelect[i] = value;
					}
				}
			}
		}
		catch
		{
			this.Select = new List<CryptoInt>
			{
				0,
				0,
				0
			};
			this.LastSelect = new List<CryptoInt>
			{
				0,
				0,
				0
			};
		}
	}

	private List<CryptoInt> JsonObjectToList(JsonObject json)
	{
		List<CryptoInt> list = new List<CryptoInt>();
		for (int i = 0; i < json.Length; i++)
		{
			list.Add(json.Get<int>(json.GetKey(i)));
		}
		return list;
	}

	private List<CryptoInt> JsonArrayToList(JsonArray json)
	{
		List<CryptoInt> list = new List<CryptoInt>();
		for (int i = 0; i < json.Length; i++)
		{
			list.Add(json.Get<int>(i));
		}
		return list;
	}
}
