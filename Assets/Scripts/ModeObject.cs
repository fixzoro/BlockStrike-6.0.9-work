using System;
using System.Collections;
using UnityEngine;

public class ModeObject : MonoBehaviour
{
    public GameMode Mode;

    public GameObject[] Targets;

    private void Start()
	{
		if (PhotonNetwork.room.GetGameMode() == this.Mode)
		{
			base.StartCoroutine(this.LoadSync());
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private IEnumerator LoadSync()
	{
		for (int i = 0; i < this.Targets.Length; i++)
		{
			this.Targets[i].SetActive(true);
			yield return new WaitForSeconds(0.01f);
		}
		EventManager.Dispatch("ModeObject_Finish");
		yield break;
	}
}
