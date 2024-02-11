using System;
using UnityEngine;

public class AnimMaterial : MonoBehaviour
{
    public Material material;

    public Vector2 speed;

    private bool visible;

    private void Reset()
	{
		if (this.material != null)
		{
			this.material.mainTextureOffset = Vector2.zero;
		}
	}

	private void OnDisable()
	{
		this.Reset();
	}

	private void OnBecameVisible()
	{
		this.visible = true;
	}

	private void OnBecameInvisible()
	{
		this.visible = false;
	}

	private void Update()
	{
		if (this.visible)
		{
			this.material.mainTextureOffset = this.speed * Time.time;
		}
	}
}
