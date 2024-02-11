using System;
using UnityEngine;

public class StickersManager : MonoBehaviour
{
    public GameObject StickersParent;

    public MeshAtlas[] Stickers;

    private void OnEnable()
	{
		if (UnityEngine.Random.Range(0, 100) == 56)
		{
			this.StickersParent.SetActive(true);
			for (int i = 0; i < this.Stickers.Length; i++)
			{
				this.Stickers[i].spriteName = UnityEngine.Random.Range(0, 10).ToString();
			}
		}
	}
    
	private void OnDisable()
	{
		this.StickersParent.SetActive(false);
	}
}
