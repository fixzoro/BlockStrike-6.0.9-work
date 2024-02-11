using System;
using System.Collections.Generic;
using BestHTTP;
using UnityEngine;

public class AvatarManager
{
    private static Dictionary<string, Texture2D> avatars = new Dictionary<string, Texture2D>();

    public static Texture2D avatarsAtlas;

    public static Rect[] avatarsAtlasRect;

    public static bool Contains(string url)
	{
		return !string.IsNullOrEmpty(url) && CacheManager.Exists(url, "Avatars", true);
	}

	public static void Get(string url, Action<Texture2D> callback)
	{
		if (string.IsNullOrEmpty(url))
		{
			return;
		}
		if (!AvatarManager.Contains(url))
		{
			new HTTPRequest(new Uri(url), delegate(HTTPRequest req, HTTPResponse res)
			{
				if (res.IsSuccess)
				{
					Texture2D texture2D2 = new Texture2D(96, 96);
					try
					{
						texture2D2.LoadImage(res.Data);
						texture2D2.Apply();
						AvatarManager.avatars.Add(url, texture2D2);
						CacheManager.Save<byte[]>(url, "Avatars", res.Data, true);
						callback(texture2D2);
					}
					catch
					{
						callback(null);
					}
				}
			}).Send();
			return;
		}
		if (AvatarManager.avatars.ContainsKey(url))
		{
			callback(AvatarManager.avatars[url]);
			return;
		}
		Texture2D texture2D = new Texture2D(96, 96);
		try
		{
			texture2D.LoadImage(CacheManager.Load<byte[]>(url, "Avatars", true));
			texture2D.Apply();
			AvatarManager.avatars.Add(url, texture2D);
			callback(texture2D);
		}
		catch
		{
			callback(null);
		}
	}

	public static Texture2D Get(string url)
	{
		if (AvatarManager.avatars.ContainsKey(url))
		{
			return AvatarManager.avatars[url];
		}
		if (string.IsNullOrEmpty(url))
		{
			return GameSettings.instance.NoAvatarTexture;
		}
		if (AvatarManager.Contains(url))
		{
			try
			{
				Texture2D texture2D = new Texture2D(96, 96);
				texture2D.LoadImage(CacheManager.Load<byte[]>(url, "Avatars", true));
				texture2D.Apply();
				AvatarManager.avatars.Add(url, texture2D);
				return texture2D;
			}
			catch
			{
			}
		}
		return GameSettings.instance.NoAvatarTexture;
	}

	public static void Load(string url)
	{
		if (string.IsNullOrEmpty(url))
		{
			return;
		}
		if (AvatarManager.Contains(url))
		{
			return;
		}
		new HTTPRequest(new Uri(url), delegate(HTTPRequest req, HTTPResponse res)
		{
			if (res.IsSuccess)
			{
				CacheManager.Save<byte[]>(url, "Avatars", res.Data, true);
			}
		}).Send();
	}
}
