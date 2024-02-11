using System;
using UnityEngine;

public class SpleefBlock : MonoBehaviour
{
    public int id;

    public GameObject cachedGameObject;

    public Renderer cachedRenderer;

    public void Damage()
	{
		SpleefMode.instance.Damage(this.id);
	}
}
