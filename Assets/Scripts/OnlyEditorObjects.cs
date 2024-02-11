using System;
using UnityEngine;

public class OnlyEditorObjects : MonoBehaviour
{
	private void Start()
	{
		for (int i = 0; i < this.objs.Length; i++)
		{
			this.objs[i].SetActive(false);
		}
	}
    
	public GameObject[] objs;
}
