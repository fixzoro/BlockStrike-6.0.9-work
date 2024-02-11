using System;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public static List<Chunk> chunks = new List<Chunk>();

    public byte maxSizeX = 50;

    public byte maxSizeY = 50;

    public byte maxSizeZ = 50;

    public byte size = 5;

    public float scale = 1f;

    public byte texture;

    public Material material;

    public string save;

    private static ChunkManager instance;

    public static byte chunkSize
	{
		get
		{
			return ChunkManager.instance.size;
		}
	}


	public static float chunkScale
	{
		get
		{
			return ChunkManager.instance.scale;
		}
	}

	private void Awake()
	{
		ChunkManager.instance = this;
	}

	public static void CreatePlane(int x, int y, int z)
	{
		byte[,,] array = new byte[x, y, z];
		for (int i = 0; i < array.GetLength(0); i++)
		{
			for (int j = 0; j < array.GetLength(1); j++)
			{
				for (int k = 0; k < array.GetLength(2); k++)
				{
					array[i, j, k] = 1;
				}
			}
		}
		ChunkManager.SetMap(array);
	}

	public static void SetMap(byte[,,] map)
	{
		for (int i = 0; i < map.GetLength(0); i++)
		{
			for (int j = 0; j < map.GetLength(1); j++)
			{
				for (int k = 0; k < map.GetLength(2); k++)
				{
					Chunk chunk = ChunkManager.FindChunk(i, j, k);
					if (chunk == null)
					{
						chunk = ChunkManager.CreateChunk(i, j, k);
					}
					chunk.map[i - (int)chunk.minX, j - (int)chunk.minY, k - (int)chunk.minZ] = map[i, j, k];
				}
			}
		}
	}

	public static string SaveMap()
	{
		byte[] array = new byte[(int)(ChunkManager.instance.maxSizeX * ChunkManager.instance.maxSizeY * ChunkManager.instance.maxSizeZ + 5)];
		array[0] = ChunkManager.instance.maxSizeX;
		array[1] = ChunkManager.instance.maxSizeY;
		array[2] = ChunkManager.instance.maxSizeZ;
		array[3] = ChunkManager.instance.size;
		int num = 4;
		for (int i = 0; i < (int)ChunkManager.instance.maxSizeX; i++)
		{
			for (int j = 0; j < (int)ChunkManager.instance.maxSizeY; j++)
			{
				for (int k = 0; k < (int)ChunkManager.instance.maxSizeZ; k++)
				{
					Chunk chunk = ChunkManager.FindChunk(i, j, k);
					if (chunk != null)
					{
						array[num] = chunk.map[i - (int)chunk.minX, j - (int)chunk.minY, k - (int)chunk.minZ];
					}
					num++;
				}
			}
		}
		MonoBehaviour.print(array.Length);
		MonoBehaviour.print(null);
		return Convert.ToBase64String(null);
	}

	public static void LoadMap(byte[] list)
	{
		list = null;
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		ChunkManager.instance.maxSizeX = list[0];
		ChunkManager.instance.maxSizeY = list[1];
		ChunkManager.instance.maxSizeZ = list[2];
		ChunkManager.instance.size = list[3];
		for (int i = 4; i < list.Length; i++)
		{
			if (num3 > (int)(ChunkManager.instance.maxSizeZ - 1))
			{
				num3 = 0;
				if (num2 >= (int)(ChunkManager.instance.maxSizeY - 1))
				{
					num2 = 0;
					num++;
				}
				else
				{
					num2++;
				}
			}
			if (list[i] != 0)
			{
				Chunk chunk = ChunkManager.FindChunk(num, num2, num3);
				if (chunk == null)
				{
					chunk = ChunkManager.CreateChunk(num, num2, num3);
				}
				chunk.map[num - (int)chunk.minX, num2 - (int)chunk.minY, num3 - (int)chunk.minZ] = list[i];
			}
			num3++;
		}
		ChunkManager.UpdateMeshAll();
	}

	public static void AddCube(int x, int y, int z, byte tex)
	{
		if (x < 0 || y < 0 || z < 0 || x >= (int)ChunkManager.instance.maxSizeX || y >= (int)ChunkManager.instance.maxSizeY || z >= (int)ChunkManager.instance.maxSizeZ)
		{
			return;
		}
		Chunk chunk = ChunkManager.FindChunk(x, y, z);
		if (chunk == null)
		{
			chunk = ChunkManager.CreateChunk(x, y, z);
		}
		chunk.AddCube(x, y, z, tex);
	}

	public static void RemoveCube(int x, int y, int z, byte tex)
	{
		Chunk chunk = ChunkManager.FindChunk(x, y, z);
		if (chunk != null)
		{
			chunk.RemoveCube(x, y, z, tex);
		}
	}

	public static Chunk FindChunk(int x, int y, int z)
	{
		for (int i = 0; i < ChunkManager.chunks.Count; i++)
		{
			if (x >= (int)ChunkManager.chunks[i].minX && y >= (int)ChunkManager.chunks[i].minY && z >= (int)ChunkManager.chunks[i].minZ && x < (int)(ChunkManager.chunks[i].minX + ChunkManager.chunkSize) && y < (int)(ChunkManager.chunks[i].minY + ChunkManager.chunkSize) && z < (int)(ChunkManager.chunks[i].minZ + ChunkManager.chunkSize))
			{
				return ChunkManager.chunks[i];
			}
		}
		return null;
	}

	private static Chunk CreateChunk(int x, int y, int z)
	{
		GameObject gameObject = new GameObject(string.Concat(new object[]
		{
			"Chunk [",
			x / (int)ChunkManager.chunkSize * (int)ChunkManager.chunkSize,
			",",
			y / (int)ChunkManager.chunkSize * (int)ChunkManager.chunkSize,
			",",
			z / (int)ChunkManager.chunkSize * (int)ChunkManager.chunkSize,
			"]"
		}));
		gameObject.transform.SetParent(ChunkManager.instance.transform);
		gameObject.AddComponent<MeshFilter>();
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
		meshRenderer.sharedMaterial = ChunkManager.instance.material;
		byte minX = (byte)(x / (int)ChunkManager.chunkSize * (int)ChunkManager.chunkSize);
		byte minY = (byte)(y / (int)ChunkManager.chunkSize * (int)ChunkManager.chunkSize);
		byte minZ = (byte)(z / (int)ChunkManager.chunkSize * (int)ChunkManager.chunkSize);
		gameObject.transform.localPosition = new Vector3((float)(x / (int)ChunkManager.chunkSize * (int)ChunkManager.chunkSize) * ChunkManager.chunkScale, (float)(y / (int)ChunkManager.chunkSize * (int)ChunkManager.chunkSize) * ChunkManager.chunkScale, (float)(z / (int)ChunkManager.chunkSize * (int)ChunkManager.chunkSize) * ChunkManager.chunkScale);
		gameObject.tag = "ChunkBlock";
		Chunk chunk = gameObject.AddComponent<Chunk>();
		chunk.Init();
		chunk.minX = minX;
		chunk.minY = minY;
		chunk.minZ = minZ;
		return chunk;
	}

	public static void UpdateMeshAll()
	{
		for (int i = 0; i < ChunkManager.chunks.Count; i++)
		{
			ChunkManager.chunks[i].UpdateMesh();
			ChunkManager.chunks[i].UpdateColliders();
		}
	}
}
