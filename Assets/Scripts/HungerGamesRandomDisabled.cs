using System;
using System.Collections.Generic;
using UnityEngine;

public class HungerGamesRandomDisabled : MonoBehaviour
{
    public List<HungerGamesBox> Boxes = new List<HungerGamesBox>();

    public int boxDisabled;

    private void Start()
	{
		EventManager.AddListener("StartRound", new EventManager.Callback(this.StartRound));
	}

	private void StartRound()
	{
		if (PhotonNetwork.isMasterClient)
		{
			TimerManager.In(0.5f, delegate()
			{
				List<byte> list = new List<byte>();
				List<HungerGamesBox> list2 = new List<HungerGamesBox>();
				for (int i = 0; i < this.boxDisabled; i++)
				{
					HungerGamesBox hungerGamesBox = this.Boxes[UnityEngine.Random.Range(0, this.Boxes.Count)];
					list.Add((byte)hungerGamesBox.ID);
					this.Boxes.Remove(hungerGamesBox);
					list2.Add(hungerGamesBox);
				}
				for (int j = 0; j < list2.Count; j++)
				{
					this.Boxes.Add(list2[j]);
				}
				list2.Clear();
				HungerGames.HideBoxes(list.ToArray());
			});
		}
	}
}
