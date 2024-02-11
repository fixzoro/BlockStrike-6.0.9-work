using System;
using UnityEngine;

public class CreativeObject : MonoBehaviour
{
    public byte id;

    public MeshAtlas[] meshAtlases;

    private Transform mTransform;

    private GameObject mGameObject;

    public Transform cachedTransform
	{
		get
		{
			if (this.mTransform == null)
			{
				this.mTransform = base.transform;
			}
			return this.mTransform;
		}
	}


	public GameObject cachedGameObject
	{
		get
		{
			if (this.mGameObject == null)
			{
				this.mGameObject = base.gameObject;
			}
			return this.mGameObject;
		}
	}

	public void UpdateFaces(byte spriteID)
	{
		this.UpdateFaces(spriteID, true);
	}

	public void UpdateFaces(byte spriteID, bool checkFace)
	{
		this.id = spriteID;
		for (int i = 0; i < this.meshAtlases.Length; i++)
		{
			if (checkFace)
			{
				RaycastHit raycastHit;
				if (Physics.Raycast(this.cachedTransform.position, -this.meshAtlases[i].cachedTransform.forward, out raycastHit, 1.1f))
				{
					if (raycastHit.transform.CompareTag("CreativeObject"))
					{
						if (raycastHit.transform.name != "Plane")
						{
							raycastHit.transform.GetComponent<Renderer>().enabled = false;
						}
						this.meshAtlases[i].meshRenderer.enabled = false;
					}
					else
					{
						this.meshAtlases[i].meshRenderer.enabled = true;
					}
				}
				else
				{
					this.meshAtlases[i].meshRenderer.enabled = true;
				}
			}
			this.meshAtlases[i].spriteName = spriteID.ToString();
		}
	}

	public void Delete()
	{
		this.Delete(true);
	}

	public void Delete(bool check)
	{
		if (check)
		{
			for (int i = 0; i < this.meshAtlases.Length; i++)
			{
				RaycastHit raycastHit;
				if (Physics.Raycast(this.cachedTransform.position, -this.meshAtlases[i].cachedTransform.forward, out raycastHit, 1.1f) && raycastHit.transform.CompareTag("CreativeObject") && raycastHit.transform.name != "Plane")
				{
					raycastHit.transform.GetComponent<Renderer>().enabled = true;
				}
			}
		}
		PoolManager.Despawn("Block ", this.cachedGameObject);
	}
}
