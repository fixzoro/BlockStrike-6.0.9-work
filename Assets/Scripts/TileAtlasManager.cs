using System;
using UnityEngine;

public class TileAtlasManager : ScriptableObject
{
    public Material mat;

    public TileAtlasManager.TextureData[] data;

    private static TileAtlasManager Instance;

    public static TileAtlasManager instance
	{
		get
		{
			if (TileAtlasManager.Instance == null)
			{
				TileAtlasManager.Instance = (Resources.Load("TileAtlasManager") as TileAtlasManager);
			}
			return TileAtlasManager.Instance;
		}
	}
    
	public static TileAtlasManager.TextureData GetData(Material material)
	{
		for (int i = 0; i < TileAtlasManager.instance.data.Length; i++)
		{
			if (TileAtlasManager.instance.data[i].mat == material)
			{
				return TileAtlasManager.instance.data[i];
			}
		}
		return default(TileAtlasManager.TextureData);
	}
    
	public static TileAtlasManager.TextureData GetData(byte x, byte y)
	{
		for (int i = 0; i < TileAtlasManager.instance.data.Length; i++)
		{
			if (TileAtlasManager.instance.data[i].x == x && TileAtlasManager.instance.data[i].y == y)
			{
				return TileAtlasManager.instance.data[i];
			}
		}
		return default(TileAtlasManager.TextureData);
	}
    
	public static Vector2 GetCordinates(byte index)
	{
		return new Vector2((float)TileAtlasManager.instance.data[(int)index].x, (float)TileAtlasManager.instance.data[(int)index].y);
	}
    
	public static bool Contains(Material material)
	{
		for (int i = 0; i < TileAtlasManager.instance.data.Length; i++)
		{
			if (TileAtlasManager.instance.data[i].mat == material)
			{
				return true;
			}
		}
		return false;
	}
   
	[Serializable]
	public struct TextureData
	{
		public Material mat;
        
		public byte x;
        
		public byte y;
        
		public byte width;
        
		public byte height;
	}
}
