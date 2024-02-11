using System;
using UnityEngine;

public class UIPlayerStatisticsElement : MonoBehaviour
{
    public UILabel PlayerNameLabel;

    public UITexture AvatarTexture;

    public UILabel LevelLabel;

    public UILabel ClanTagLabel;

    public UILabel KillsLabel;

    public UILabel DeathsLabel;

    public UILabel PingLabel;

    public UIWidget Widget;

    public UIDragScrollView DragScroll;

    private static Color32 AdminColor = new Color32(60, 181, 232, byte.MaxValue);

    private static Color32 LocalPlayerColor = Color.green;

    public PhotonPlayer PlayerInfo;

    private int Timer;

    private Transform mTransform;

    public Transform cachedTransform
	{
		get
		{
			if (this.mTransform == null)
			{
				this.mTransform = base.transform;
			}
			return this.mTransform;
		}
	}

	private void OnDisable()
	{
		TimerManager.Cancel(this.Timer);
	}

	public void SetData(PhotonPlayer playerInfo)
	{
		this.PlayerInfo = playerInfo;
		string text = playerInfo.GetClan().ToUpper();
		this.ClanTagLabel.text = text;
		if (Settings.ShowAvatars)
		{
			this.AvatarTexture.mainTexture = AvatarManager.Get(playerInfo.GetAvatarUrl());
		}
		else
		{
			this.AvatarTexture.mainTexture = GameSettings.instance.NoAvatarTexture;
		}
		this.PlayerNameLabel.text = this.UpdatePlayerName(playerInfo);
		this.LevelLabel.text = StringCache.Get(playerInfo.GetLevel());
		this.KillsLabel.text = StringCache.Get(this.PlayerInfo.GetKills());
		this.DeathsLabel.text = StringCache.Get(this.PlayerInfo.GetDeaths());
		this.PingLabel.text = StringCache.Get(this.PlayerInfo.GetPing());
		base.name = this.PlayerNameLabel.text;
		if (playerInfo.GetDead())
		{
			this.Widget.alpha = 0.5f;
		}
		else
		{
			this.Widget.alpha = 1f;
		}
		this.Widget.UpdateWidget();
		if (playerInfo.IsLocal)
		{
			this.PlayerNameLabel.color = UIPlayerStatisticsElement.LocalPlayerColor;
			this.LevelLabel.color = UIPlayerStatisticsElement.LocalPlayerColor;
			this.KillsLabel.color = UIPlayerStatisticsElement.LocalPlayerColor;
			this.DeathsLabel.color = UIPlayerStatisticsElement.LocalPlayerColor;
			this.PingLabel.color = UIPlayerStatisticsElement.LocalPlayerColor;
		}
		else if (playerInfo.IsMasterClient)
		{
			this.PlayerNameLabel.color = UIPlayerStatisticsElement.AdminColor;
			this.LevelLabel.color = UIPlayerStatisticsElement.AdminColor;
			this.KillsLabel.color = UIPlayerStatisticsElement.AdminColor;
			this.DeathsLabel.color = UIPlayerStatisticsElement.AdminColor;
			this.PingLabel.color = UIPlayerStatisticsElement.AdminColor;
		}
		else
		{
			this.PlayerNameLabel.color = Color.white;
			this.LevelLabel.color = Color.white;
			this.KillsLabel.color = Color.white;
			this.DeathsLabel.color = Color.white;
			this.PingLabel.color = Color.white;
		}
		if (!TimerManager.IsActive(this.Timer))
		{
			this.Timer = TimerManager.In(3f, -1, 3f, new TimerManager.Callback(this.UpdateData));
		}
	}

	private string UpdatePlayerName(PhotonPlayer player)
	{
		if ((PhotonNetwork.room.GetGameMode() == GameMode.Bomb || PhotonNetwork.room.GetGameMode() == GameMode.Bomb2) && PhotonNetwork.player.GetTeam() != Team.Blue && BombManager.GetPlayerBombID() != -1 && player.ID == BombManager.GetPlayerBombID())
		{
			return player.UserId + "   " + UIStatus.GetSpecialSymbol(98);
		}
		return player.UserId;
	}

	private void UpdateData()
	{
		this.SetData(this.PlayerInfo);
	}
}
