using System;
using System.Collections.Generic;
using UnityEngine;

public class UIPlayerList : MonoBehaviour
{
    public UIGrid Grid;

    public GameObject Element;

    public UILabel PlayerNameLabel;

    public UILabel LevelLabel;

    public UILabel KillsLabel;

    public UILabel DeathsLabel;

    public UILabel PingLabel;

    private List<Transform> PlayerList = new List<Transform>();

    private List<Transform> PlayerListPool = new List<Transform>();

    public void Active()
	{
		this.PlayerNameLabel.gameObject.SetActive(false);
		PhotonPlayer[] playerList = PhotonNetwork.playerList;
		for (int i = 0; i < playerList.Length; i++)
		{
			Transform transform = this.GetGameObject().transform;
			transform.name = playerList[i].UserId;
			transform.GetComponent<UILabel>().text = playerList[i].UserId;
		}
		this.Grid.repositionNow = true;
	}

	public void SelectPlayer(Transform playerTransform)
	{
		PhotonPlayer player = this.GetPlayer(playerTransform.name);
		if (player == null)
		{
			return;
		}
		this.PlayerNameLabel.gameObject.SetActive(true);
		if (player.GetTeam() == Team.Blue)
		{
			this.PlayerNameLabel.text = "[00c5ff]" + player.UserId;
		}
		else if (player.GetTeam() == Team.Red)
		{
			this.PlayerNameLabel.text = "[ff0000]" + player.UserId;
		}
		this.KillsLabel.text = Localization.Get("Kills", true) + ": " + player.GetKills();
		this.DeathsLabel.text = Localization.Get("Deaths", true) + ": " + player.GetDeaths();
		this.PingLabel.text = Localization.Get("Ping", true) + ": " + player.GetPing();
	}

	private PhotonPlayer GetPlayer(string playerName)
	{
		for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
		{
			if (PhotonNetwork.playerList[i].UserId == playerName)
			{
				return PhotonNetwork.playerList[i];
			}
		}
		return null;
	}

	private GameObject GetGameObject()
	{
		GameObject gameObject;
		if (this.PlayerListPool.Count != 0)
		{
			gameObject = this.PlayerListPool[0].gameObject;
			this.PlayerListPool.RemoveAt(0);
		}
		else
		{
			gameObject = this.Grid.gameObject.AddChild(this.Element);
		}
		gameObject.SetActive(true);
		return gameObject;
	}

	private void ClearList()
	{
		if (this.PlayerList.Count != 0)
		{
			for (int i = 0; i < this.PlayerList.Count; i++)
			{
				this.PlayerList[i].gameObject.SetActive(false);
				this.PlayerListPool.Add(this.PlayerList[i]);
			}
			this.PlayerList.Clear();
		}
	}
}
