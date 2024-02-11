using System;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
[ExecuteInEditMode]
public class CryptoCapsuleCollider : MonoBehaviour
{
    public CryptoVector3 center;

    public CryptoFloat radius;

    public CryptoFloat height;

    private CapsuleCollider mCollider;

    public CapsuleCollider cachedCapsuleCollider
	{
		get
		{
			if (this.mCollider == null)
			{
				this.mCollider = base.GetComponent<CapsuleCollider>();
			}
			return this.mCollider;
		}
	}

	private void OnDisable()
	{
		this.Check();
	}

	private void OnApplicationFocus(bool focus)
	{
		this.Check();
	}

	private void Check()
	{
		if (this.cachedCapsuleCollider.center != this.center || this.cachedCapsuleCollider.radius != this.radius || this.cachedCapsuleCollider.height != this.height)
		{
			CheckManager.Detected();
		}
	}
}
