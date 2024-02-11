using System;
using UnityEngine;

public class PaintObject : MonoBehaviour
{
    public int ID;

    public int ColorID;

    public Transform[] faces;

    public Renderer[] facesRenderer;

    public MeshFilter[] facesMesh;

    private Transform mTransform;

    private Transform CachedTransform
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

	public void Init(int id, int colorID)
	{
		this.ID = id;
		this.ColorID = colorID;
		for (int i = 0; i < this.faces.Length; i++)
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(this.CachedTransform.position, -this.faces[i].forward, out raycastHit, 1.1f))
			{
				if (raycastHit.transform.CompareTag("PaintObject"))
				{
					if (raycastHit.transform.name != "Plane")
					{
						raycastHit.transform.GetComponent<Renderer>().enabled = false;
					}
					this.facesRenderer[i].GetComponent<Renderer>().enabled = false;
				}
				else
				{
					this.facesRenderer[i].GetComponent<Renderer>().enabled = true;
				}
			}
			else
			{
				this.facesRenderer[i].GetComponent<Renderer>().enabled = true;
			}
			Color32 color = PaintManager.GetColor(colorID);
			Color32[] colors = new Color32[]
			{
				color,
				color,
				color,
				color
			};
			this.facesMesh[i].mesh.colors32 = colors;
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
			for (int i = 0; i < this.faces.Length; i++)
			{
				RaycastHit raycastHit;
				if (Physics.Raycast(this.CachedTransform.position, -this.faces[i].forward, out raycastHit, 1.1f) && raycastHit.transform.CompareTag("PaintObject") && raycastHit.transform.name != "Plane")
				{
					raycastHit.transform.GetComponent<Renderer>().enabled = true;
				}
			}
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public byte[] GetData()
	{
		return new byte[]
		{
			(byte)((int)this.mTransform.localPosition.x),
			(byte)((int)this.mTransform.localPosition.y),
			(byte)((int)this.mTransform.localPosition.z),
			(byte)this.ColorID
		};
	}
}
