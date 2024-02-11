using System;
using System.Collections;
using UnityEngine;

[AddComponentMenu("Simple Mesh Combine")]
public class SimpleMeshCombine : MonoBehaviour
{
    public GameObject[] combinedGameOjects;

    public GameObject combined;

    public string meshName = "Combined_Meshes";

    public bool _advanced;

    public bool _savedPrefab;

    public bool _canGenerateLightmapUV;

    public int vCount;

    public void EnableRenderers(bool e)
	{
		for (int i = 0; i < this.combinedGameOjects.Length; i++)
		{
			this.combinedGameOjects[i].GetComponent<Renderer>().enabled = e;
		}
	}

	public MeshFilter[] FindEnabledMeshes()
	{
		int num = 0;
		MeshFilter[] componentsInChildren = base.transform.GetComponentsInChildren<MeshFilter>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].GetComponent<MeshRenderer>() != null && componentsInChildren[i].GetComponent<MeshRenderer>().enabled)
			{
				num++;
			}
		}
		MeshFilter[] array = new MeshFilter[num];
		num = 0;
		for (int j = 0; j < componentsInChildren.Length; j++)
		{
			if (componentsInChildren[j].GetComponent<MeshRenderer>() != null && componentsInChildren[j].GetComponent<MeshRenderer>().enabled)
			{
				array[num] = componentsInChildren[j];
				num++;
			}
		}
		return array;
	}

	public void CombineMeshes()
	{
		GameObject gameObject = new GameObject();
		gameObject.name = "_Combined Mesh [" + base.name + "]";
		gameObject.gameObject.AddComponent<MeshFilter>();
		gameObject.gameObject.AddComponent<MeshRenderer>();
		MeshFilter[] array = this.FindEnabledMeshes();
		ArrayList arrayList = new ArrayList();
		ArrayList arrayList2 = new ArrayList();
		this.combinedGameOjects = new GameObject[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			MeshFilter[] componentsInChildren = array[i].GetComponentsInChildren<MeshFilter>();
			this.combinedGameOjects[i] = array[i].gameObject;
			foreach (MeshFilter meshFilter in componentsInChildren)
			{
				MeshRenderer component = meshFilter.GetComponent<MeshRenderer>();
				array[i].transform.gameObject.GetComponent<Renderer>().enabled = false;
				for (int k = 0; k < meshFilter.sharedMesh.subMeshCount; k++)
				{
					if (k < component.sharedMaterials.Length && k < meshFilter.sharedMesh.subMeshCount)
					{
						int num = this.Contains(arrayList, component.sharedMaterials[k]);
						if (num == -1)
						{
							arrayList.Add(component.sharedMaterials[k]);
							num = arrayList.Count - 1;
						}
						arrayList2.Add(new ArrayList());
						CombineInstance combineInstance = default(CombineInstance);
						combineInstance.transform = component.transform.localToWorldMatrix;
						combineInstance.subMeshIndex = k;
						combineInstance.mesh = meshFilter.sharedMesh;
						(arrayList2[num] as ArrayList).Add(combineInstance);
					}
				}
			}
		}
		Mesh[] array3 = new Mesh[arrayList.Count];
		CombineInstance[] array4 = new CombineInstance[arrayList.Count];
		for (int l = 0; l < arrayList.Count; l++)
		{
			CombineInstance[] combine = (arrayList2[l] as ArrayList).ToArray(typeof(CombineInstance)) as CombineInstance[];
			array3[l] = new Mesh();
			array3[l].CombineMeshes(combine, true, true);
			array4[l] = default(CombineInstance);
			array4[l].mesh = array3[l];
			array4[l].subMeshIndex = 0;
		}
		gameObject.GetComponent<MeshFilter>().sharedMesh = new Mesh();
		gameObject.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(array4, false, false);
		foreach (Mesh mesh in array3)
		{
			mesh.Clear();
			UnityEngine.Object.DestroyImmediate(mesh);
		}
		MeshRenderer meshRenderer = gameObject.GetComponent<MeshFilter>().GetComponent<MeshRenderer>();
		if (meshRenderer == null)
		{
			meshRenderer = base.gameObject.AddComponent<MeshRenderer>();
		}
		Material[] materials = arrayList.ToArray(typeof(Material)) as Material[];
		meshRenderer.materials = materials;
		this.combined = gameObject.gameObject;
		this.EnableRenderers(false);
		gameObject.transform.parent = base.transform;
		gameObject.GetComponent<MeshFilter>().sharedMesh.uv2 = null;
		this.vCount = gameObject.GetComponent<MeshFilter>().sharedMesh.vertexCount;
		if (this.vCount > 65536)
		{
			Debug.LogWarning("Vertex Count: " + this.vCount + "- Vertex Count too high, please divide mesh combine into more groups. Max 65536 for each mesh");
			this._canGenerateLightmapUV = false;
		}
		else
		{
			this._canGenerateLightmapUV = true;
		}
	}

	public void GenerateLightmapUV()
	{
	}

	public int Contains(ArrayList l, Material n)
	{
		for (int i = 0; i < l.Count; i++)
		{
			if (l[i] as Material == n)
			{
				return i;
			}
		}
		return -1;
	}
}
