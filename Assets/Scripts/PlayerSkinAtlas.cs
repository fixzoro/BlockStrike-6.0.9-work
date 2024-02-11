using System;
using UnityEngine;

[RequireComponent(typeof(SkinnedMeshRenderer))]
[ExecuteInEditMode]
public class PlayerSkinAtlas : MonoBehaviour
{
    public static UIAtlas lastUsedAtlas;

    public Mesh originalMesh;

    private SkinnedMeshRenderer _skinnedMeshRenderer;

    private Mesh mesh;

    public Material originalMaterial;

    public Material atlasMaterial;

    public UIAtlas mAtlas;

    public string headSprite;

    public string headSpriteLast;

    public string bodySprite;

    public string bodySpriteLast;

    public string legsSprite;

    public string legsSpriteLast;

    private void OnEnable()
	{
		this.UpdateMesh();
	}

	private void Reset()
	{
		this.DisableMesh();
		if (Application.isEditor)
		{
			this.UpdateMesh();
		}
	}


	public SkinnedMeshRenderer skinnedMeshRenderer
	{
		get
		{
			if (this._skinnedMeshRenderer == null)
			{
				this._skinnedMeshRenderer = base.gameObject.GetComponent<SkinnedMeshRenderer>();
			}
			return this._skinnedMeshRenderer;
		}
	}

	public void UpdateMesh()
	{
		if (this.originalMesh == null)
		{
			this.originalMesh = base.gameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh;
			this.originalMaterial = this.skinnedMeshRenderer.sharedMaterial;
			if (this.atlas == null && PlayerSkinAtlas.lastUsedAtlas != null)
			{
				this.atlas = PlayerSkinAtlas.lastUsedAtlas;
			}
		}
		if (base.enabled)
		{
			this.EnableMesh();
		}
		else
		{
			this.DisableMesh();
		}
	}

	public void EnableMesh()
	{
		if (this.mesh == null)
		{
			this.mesh = (Mesh)UnityEngine.Object.Instantiate(this.originalMesh);
			this.mesh.name = this.originalMesh.name + "_Atlas";
			this.UpdateUVs();
			if (this.atlasMaterial != null)
			{
				this.skinnedMeshRenderer.sharedMaterial = this.atlasMaterial;
			}
			this.skinnedMeshRenderer.sharedMesh = this.mesh;
		}
	}

	public void DisableMesh()
	{
		if (this.mesh != null)
		{
			this.atlasMaterial = this.skinnedMeshRenderer.sharedMaterial;
			UnityEngine.Object.DestroyImmediate(this.mesh);
			this.mesh = null;
		}
		if (this.originalMesh != null)
		{
			this.skinnedMeshRenderer.sharedMesh = this.originalMesh;
		}
		if (this.originalMaterial != null)
		{
			this.skinnedMeshRenderer.sharedMaterial = this.originalMaterial;
		}
	}

	public void UpdateUVs()
	{
		if (this.mesh == null)
		{
			return;
		}
		if (this.atlas == null)
		{
			return;
		}
		Vector2[] array = this.originalMesh.uv;
		array = this.UpdatePlayerUV(PlayerSkinMember.Face, this.headSprite, array);
		array = this.UpdatePlayerUV(PlayerSkinMember.Body, this.bodySprite, array);
		array = this.UpdatePlayerUV(PlayerSkinMember.Legs, this.legsSprite, array);
		this.mesh.uv = array;
	}

	public Vector2[] UpdatePlayerUV(PlayerSkinMember element, string sprite, Vector2[] playerUV)
	{
		if (string.IsNullOrEmpty(sprite))
		{
			return playerUV;
		}
		UISpriteData sprite2 = this.atlas.GetSprite(sprite);
		if (sprite2 == null)
		{
			return playerUV;
		}
		if (this.atlas.texture == null)
		{
			return playerUV;
		}
		Rect rect = new Rect((float)sprite2.x, (float)sprite2.y, (float)sprite2.width, (float)sprite2.height);
		Rect rect2 = NGUIMath.ConvertToTexCoords(rect, this.atlas.texture.width, this.atlas.texture.height);
		for (int i = 0; i < playerUV.Length; i++)
		{
			switch (element)
			{
			case PlayerSkinMember.Face:
				if (i >= 428 && i <= 451)
				{
					playerUV[i].x = playerUV[i].x * rect2.width + rect2.x;
					playerUV[i].y = playerUV[i].y * rect2.height + rect2.y;
				}
				break;
			case PlayerSkinMember.Body:
				if (i >= 196 && i <= 427)
				{
					playerUV[i].x = playerUV[i].x * rect2.width + rect2.x;
					playerUV[i].y = playerUV[i].y * rect2.height + rect2.y;
				}
				break;
			case PlayerSkinMember.Legs:
				if ((i >= 0 && i <= 195) || (i >= 452 && i <= 527))
				{
					playerUV[i].x = playerUV[i].x * rect2.width + rect2.x;
					playerUV[i].y = playerUV[i].y * rect2.height + rect2.y;
				}
				break;
			}
		}
		return playerUV;
	}

	public UIAtlas atlas
	{
		get
		{
			return this.mAtlas;
		}
		set
		{
			if (this.mAtlas != value)
			{
				this.mAtlas = value;
				if (this.mAtlas != null)
				{
					PlayerSkinAtlas.lastUsedAtlas = value;
					this.atlasMaterial = this.mAtlas.spriteMaterial;
					this.skinnedMeshRenderer.sharedMaterial = this.atlasMaterial;
					if (string.IsNullOrEmpty(this.headSprite) || string.IsNullOrEmpty(this.bodySprite) || string.IsNullOrEmpty(this.legsSprite))
					{
						if (string.IsNullOrEmpty(this.headSprite))
						{
							this.SetSprite(PlayerSkinMember.Face, this.headSpriteLast);
						}
						if (string.IsNullOrEmpty(this.bodySprite))
						{
							this.SetSprite(PlayerSkinMember.Body, this.bodySpriteLast);
						}
						if (string.IsNullOrEmpty(this.legsSprite))
						{
							this.SetSprite(PlayerSkinMember.Legs, this.legsSpriteLast);
						}
					}
					else
					{
						this.UpdateUVs();
					}
				}
			}
		}
	}

	public string GetSprite(PlayerSkinMember member)
	{
		switch (member)
		{
		case PlayerSkinMember.Face:
			return this.headSprite;
		case PlayerSkinMember.Body:
			return this.bodySprite;
		case PlayerSkinMember.Legs:
			return this.legsSprite;
		}
		return string.Empty;
	}

	public void SetSprite(PlayerSkinMember member, string value)
	{
		string sprite = this.GetSprite(member);
		if (sprite != value)
		{
			switch (member)
			{
			case PlayerSkinMember.Face:
				this.headSprite = value;
				this.headSpriteLast = value;
				break;
			case PlayerSkinMember.Body:
				this.bodySprite = value;
				this.bodySpriteLast = value;
				break;
			case PlayerSkinMember.Legs:
				this.legsSprite = value;
				this.legsSpriteLast = value;
				break;
			}
			this.UpdateUVs();
		}
	}

	public void SetSprite(string head, string body, string legs)
	{
		if (this.headSprite != head)
		{
			this.headSprite = head;
			this.headSpriteLast = head;
		}
		if (this.bodySprite != body)
		{
			this.bodySprite = body;
			this.bodySpriteLast = body;
		}
		if (this.legsSprite != legs)
		{
			this.legsSprite = legs;
			this.legsSpriteLast = legs;
		}
		this.UpdateUVs();
	}

	public void UpdateAllMeshes()
	{
		PlayerSkinAtlas[] array = NGUITools.FindActive<PlayerSkinAtlas>();
		int i = 0;
		int num = array.Length;
		while (i < num)
		{
			PlayerSkinAtlas playerSkinAtlas = array[i];
			if (playerSkinAtlas.enabled && this.originalMesh == playerSkinAtlas.originalMesh)
			{
				playerSkinAtlas.UpdateMesh();
			}
			i++;
		}
	}

	public void UpdateMeshTextures()
	{

	}

	[Serializable]
	public class SpriteData
	{
		public string sprite;

		public string lastSpriteName;
	}
}
