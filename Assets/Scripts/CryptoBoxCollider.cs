using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[ExecuteInEditMode]
public class CryptoBoxCollider : MonoBehaviour
{
    public CryptoVector3 center;

    public CryptoVector3 size;

    private BoxCollider mCollider;

    public BoxCollider cachedBoxCollider
	{
		get
		{
			if (this.mCollider == null)
			{
				this.mCollider = base.GetComponent<BoxCollider>();
			}
			return this.mCollider;
		}
	}

	private void OnEnable()
	{
		this.Check();
	}

	private void OnDisable()
	{
		this.Check();
	}

	private void OnApplicationFocus(bool focus)
	{
		if (!focus)
		{
			return;
		}
		this.Check();
	}

	private void Check()
	{
		if (this.cachedBoxCollider.size != this.size)
		{
			CheckManager.Detected();
		}
		if (this.cachedBoxCollider.center != this.center)
		{
			CheckManager.Detected();
		}
	}
}
