using System;
using UnityEngine;

public class nMonoBehaviour : MonoBehaviour
{
    private GameObject mGameObject;

    private Transform mTransform;

    private MeshFilter mMeshFilter;

    private MeshRenderer mMeshRenderer;

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


	public MeshFilter cachedMeshFilter
	{
		get
		{
			if (this.mMeshFilter == null)
			{
				this.mMeshFilter = base.GetComponent<MeshFilter>();
			}
			return this.mMeshFilter;
		}
	}


	public MeshRenderer cachedMeshRenderer
	{
		get
		{
			if (this.mMeshRenderer == null)
			{
				this.mMeshRenderer = base.GetComponent<MeshRenderer>();
			}
			return this.mMeshRenderer;
		}
	}

	public virtual void OnUpdate()
	{

	}

	public virtual void OnFixedUpdate()
	{

	}

	public virtual void OnLateUpdate()
	{

	}
}
