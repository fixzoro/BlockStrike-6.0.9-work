using System;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public byte[,,] map;

    public byte minX;

    public byte minY;

    public byte minZ;

    private MeshFilter meshFilter;

    private Mesh mesh;

    private BoxCollider[,,] boxColliders;

    private bool init;

    private List<Vector3> vertices = new List<Vector3>();

    private List<int> triangles = new List<int>();

    private List<Vector2> uvs = new List<Vector2>();

    private void Start()
	{
		this.Init();
	}

	public void Init()
	{
		if (!this.init)
		{
			ChunkManager.chunks.Add(this);
			this.meshFilter = base.GetComponent<MeshFilter>();
			this.mesh = new Mesh();
			this.meshFilter.sharedMesh = this.mesh;
			byte chunkSize = ChunkManager.chunkSize;
			this.map = new byte[(int)chunkSize, (int)chunkSize, (int)chunkSize];
			this.boxColliders = new BoxCollider[(int)chunkSize, (int)chunkSize, (int)chunkSize];
			this.init = true;
		}
	}

	public void AddCube(int x, int y, int z, byte tex)
	{
		x -= (int)this.minX;
		y -= (int)this.minY;
		z -= (int)this.minZ;
		if (this.map[x, y, z] != 0)
		{
			Debug.LogWarning("Chunk already exists");
			return;
		}
		this.map[x, y, z] = tex;
		if (this.boxColliders[x, y, z] != null)
		{
			this.boxColliders[x, y, z].enabled = true;
		}
		else
		{
			BoxCollider boxCollider = base.gameObject.AddComponent<BoxCollider>();
			boxCollider.center = new Vector3((float)x + 0.5f, (float)y + 0.5f, (float)z + 0.5f);
			boxCollider.size = Vector3.one;
			this.boxColliders[x, y, z] = boxCollider;
		}
		this.UpdateMesh();
		this.UpdateNextCube(x, y, z);
	}

	public void RemoveCube(int x, int y, int z, byte tex)
	{
		x -= (int)this.minX;
		y -= (int)this.minY;
		z -= (int)this.minZ;
		if (this.map[x, y, z] == 0)
		{
			Debug.LogWarning("Chunk no exists");
			return;
		}
		this.map[x, y, z] = 0;
		this.boxColliders[x, y, z].enabled = false;
		this.UpdateMesh();
		this.UpdateNextCube(x, y, z);
	}

	public bool IsTransparent(int x, int y, int z)
	{
		return this.GetByte(x, y, z) == 0;
	}

	public byte GetByte(int x, int y, int z)
	{
		if (x >= 0 && y >= 0 && z >= 0 && x < (int)ChunkManager.chunkSize && y < (int)ChunkManager.chunkSize && z < (int)ChunkManager.chunkSize)
		{
			return this.map[x, y, z];
		}
		Chunk chunk = ChunkManager.FindChunk(x + (int)this.minX, y + (int)this.minY, z + (int)this.minZ);
		if (chunk == this)
		{
			return 0;
		}
		if (chunk == null)
		{
			return 0;
		}
		return chunk.GetByte(x + (int)this.minX - (int)chunk.minX, y + (int)this.minY - (int)chunk.minY, z + (int)this.minZ - (int)chunk.minZ);
	}

	private void UpdateNextCube(int x, int y, int z)
	{
		if (x == 0)
		{
			Chunk chunk = ChunkManager.FindChunk(x - 1 + (int)this.minX, y + (int)this.minY, z + (int)this.minZ);
			if (chunk != null)
			{
				chunk.UpdateMesh();
			}
		}
		if (x == (int)(ChunkManager.chunkSize - 1))
		{
			Chunk chunk2 = ChunkManager.FindChunk(x + 1 + (int)this.minX, y + (int)this.minY, z + (int)this.minZ);
			if (chunk2 != null)
			{
				chunk2.UpdateMesh();
			}
		}
		if (y == 0)
		{
			Chunk chunk3 = ChunkManager.FindChunk(x + (int)this.minX, y - 1 + (int)this.minY, z + (int)this.minZ);
			if (chunk3 != null)
			{
				chunk3.UpdateMesh();
			}
		}
		if (y == (int)(ChunkManager.chunkSize - 1))
		{
			Chunk chunk4 = ChunkManager.FindChunk(x + (int)this.minX, y + 1 + (int)this.minY, z + (int)this.minZ);
			if (chunk4 != null)
			{
				chunk4.UpdateMesh();
			}
		}
		if (z == 0)
		{
			Chunk chunk5 = ChunkManager.FindChunk(x + (int)this.minX, y + (int)this.minY, z - 1 + (int)this.minZ);
			if (chunk5 != null)
			{
				chunk5.UpdateMesh();
			}
		}
		if (z == (int)(ChunkManager.chunkSize - 1))
		{
			Chunk chunk6 = ChunkManager.FindChunk(x + (int)this.minX, y + (int)this.minY, z + 1 + (int)this.minZ);
			if (chunk6 != null)
			{
				chunk6.UpdateMesh();
			}
		}
	}

	public void UpdateMesh()
	{
		this.vertices.Clear();
		this.triangles.Clear();
		this.uvs.Clear();
		for (int i = 0; i < (int)ChunkManager.chunkSize; i++)
		{
			for (int j = 0; j < (int)ChunkManager.chunkSize; j++)
			{
				for (int k = 0; k < (int)ChunkManager.chunkSize; k++)
				{
					if (this.map[i, j, k] != 0)
					{
						byte brick = this.map[i, j, k];
						if (this.IsTransparent(i + 1, j, k))
						{
							this.BuildFace(brick, new Vector3((float)(i + 1), (float)j, (float)k), Vector3.up, Vector3.forward, true);
						}
						if (this.IsTransparent(i - 1, j, k))
						{
							this.BuildFace(brick, new Vector3((float)i, (float)j, (float)k), Vector3.up, Vector3.forward, false);
						}
						if (this.IsTransparent(i, j + 1, k))
						{
							this.BuildFace(brick, new Vector3((float)i, (float)(j + 1), (float)k), Vector3.forward, Vector3.right, true);
						}
						if (this.IsTransparent(i, j - 1, k))
						{
							this.BuildFace(brick, new Vector3((float)i, (float)j, (float)k), Vector3.forward, Vector3.right, false);
						}
						if (this.IsTransparent(i, j, k - 1))
						{
							this.BuildFace(brick, new Vector3((float)i, (float)j, (float)k), Vector3.up, Vector3.right, true);
						}
						if (this.IsTransparent(i, j, k + 1))
						{
							this.BuildFace(brick, new Vector3((float)i, (float)j, (float)(k + 1)), Vector3.up, Vector3.right, false);
						}
					}
				}
			}
		}
		this.mesh.Clear();
		this.mesh.vertices = this.vertices.ToArray();
		this.mesh.triangles = this.triangles.ToArray();
		this.mesh.uv = this.uvs.ToArray();
		this.mesh.RecalculateNormals();
		this.mesh.RecalculateBounds();
	}

	public void UpdateColliders()
	{
		for (int i = 0; i < (int)ChunkManager.chunkSize; i++)
		{
			for (int j = 0; j < (int)ChunkManager.chunkSize; j++)
			{
				for (int k = 0; k < (int)ChunkManager.chunkSize; k++)
				{
					if (this.map[i, j, k] == 0)
					{
						if (this.boxColliders[i, j, k] != null)
						{
							this.boxColliders[i, j, k].enabled = false;
						}
					}
					else if (this.boxColliders[i, j, k] != null)
					{
						this.boxColliders[i, j, k].enabled = true;
					}
					else
					{
						BoxCollider boxCollider = base.gameObject.AddComponent<BoxCollider>();
						boxCollider.center = new Vector3((float)i + 0.5f, (float)j + 0.5f, (float)k + 0.5f);
						boxCollider.size = Vector3.one;
						this.boxColliders[i, j, k] = boxCollider;
					}
				}
			}
		}
	}

	public void BuildFace(byte brick, Vector3 corner, Vector3 up, Vector3 right, bool reversed)
	{
		int count = this.vertices.Count;
		this.vertices.Add(corner * ChunkManager.chunkScale);
		this.vertices.Add((corner + up) * ChunkManager.chunkScale);
		this.vertices.Add((corner + up + right) * ChunkManager.chunkScale);
		this.vertices.Add((corner + right) * ChunkManager.chunkScale);
		this.uvs.Add(new Vector2(0f, 0f));
		this.uvs.Add(new Vector2(0f, 0.0625f));
		this.uvs.Add(new Vector2(0.0625f, 0.0625f));
		this.uvs.Add(new Vector2(0.0625f, 0f));
		if (reversed)
		{
			this.triangles.Add(count);
			this.triangles.Add(count + 1);
			this.triangles.Add(count + 2);
			this.triangles.Add(count + 2);
			this.triangles.Add(count + 3);
			this.triangles.Add(count);
		}
		else
		{
			this.triangles.Add(count + 1);
			this.triangles.Add(count);
			this.triangles.Add(count + 2);
			this.triangles.Add(count + 3);
			this.triangles.Add(count + 2);
			this.triangles.Add(count);
		}
	}
}
