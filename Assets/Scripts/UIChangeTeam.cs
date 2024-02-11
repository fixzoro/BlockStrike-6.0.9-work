using System;
using System.Collections.Generic;
using UnityEngine;

public class UIChangeTeam : MonoBehaviour
{
    public UIWidget changeTeamSprite;

    private bool isChangeTeam;

    private float time;

    private bool changeOnlyDead;

    private static UIChangeTeam instance;

    private void Awake()
	{
		UIChangeTeam.instance = this;
	}

	private void OnEnable()
	{
		UIOthers.pauseEvent += this.OnPause;
	}

	private void OnDisable()
	{
		UIOthers.pauseEvent -= this.OnPause;
	}

	public static void SetChangeTeam(bool active)
	{
		UIChangeTeam.SetChangeTeam(active, false);
	}

	public static void SetChangeTeam(bool active, bool onlyDead)
	{
		if (PhotonNetwork.room.isOfficialServer())
		{
			return;
		}
		UIChangeTeam.instance.isChangeTeam = active;
		UIChangeTeam.instance.changeOnlyDead = onlyDead;
	}

	private void OnPause()
	{
		if (PhotonNetwork.offlineMode || !this.isChangeTeam)
		{
			return;
		}
		if (this.changeOnlyDead && !PhotonNetwork.player.GetDead())
		{
			this.changeTeamSprite.cachedGameObject.SetActive(true);
			return;
		}
		if (this.time > Time.time)
		{
			this.changeTeamSprite.cachedGameObject.SetActive(false);
			return;
		}
		PhotonPlayer[] playerList = PhotonNetwork.playerList;
		byte b = 0;
		byte b2 = 0;
		for (int i = 0; i < playerList.Length; i++)
		{
			if (playerList[i].GetTeam() == Team.Blue)
			{
				b += 1;
			}
			else if (playerList[i].GetTeam() == Team.Red)
			{
				b2 += 1;
			}
		}
		if (PhotonNetwork.player.GetTeam() == Team.Blue)
		{
			this.changeTeamSprite.cachedGameObject.SetActive(b >= b2);
		}
		else if (PhotonNetwork.player.GetTeam() == Team.Red)
		{
			this.changeTeamSprite.cachedGameObject.SetActive(b2 >= b);
		}
	}

	public void ChangeTeam()
	{
		if (PhotonNetwork.offlineMode || !this.isChangeTeam || this.time > Time.time)
		{
			return;
		}
		if (this.changeOnlyDead && !PhotonNetwork.player.GetDead())
		{
			this.changeTeamSprite.cachedGameObject.SetActive(false);
			return;
		}
		Team team = PhotonNetwork.player.GetTeam();
		PhotonPlayer[] playerList = PhotonNetwork.playerList;
		List<PhotonPlayer> list = new List<PhotonPlayer>();
		List<PhotonPlayer> list2 = new List<PhotonPlayer>();
		for (int i = 0; i < playerList.Length; i++)
		{
			if (playerList[i].GetTeam() == Team.Blue)
			{
				list.Add(playerList[i]);
			}
		}
		for (int j = 0; j < playerList.Length; j++)
		{
			if (playerList[j].GetTeam() == Team.Red)
			{
				list2.Add(playerList[j]);
			}
		}
		if (team == Team.Blue)
		{
			if (list.Count >= list2.Count)
			{
				this.time = Time.time + 60f;
				GameManager.team = Team.Red;
				EventManager.Dispatch<Team>("AutoBalance", Team.Red);
				this.OnPause();
			}
		}
		else if (team == Team.Red && list2.Count >= list.Count)
		{
			this.time = Time.time + 60f;
			GameManager.team = Team.Blue;
			EventManager.Dispatch<Team>("AutoBalance", Team.Blue);
			this.OnPause();
		}
	}
}
