using System;
using System.Text;
using UnityEngine;

public class UISelectTeam : MonoBehaviour
{
    public UILabel BluePlayersLabel;

    public UILabel RedPlayersLabel;

    public UILabel BlueCountLabel;

    public UILabel RedCountLabel;

    public GameObject SpectatorsButton;

    private bool isSpectator;

    private int redPlayersCount;

    private int bluePlayersCount;

    private int timeID;

    private Action<Team> selectCallback;

    private static UISelectTeam instance;

    private void Awake()
	{
		UISelectTeam.instance = this;
	}

	public static void OnStart(Action<Team> callback)
	{
		GUI.enabled = false;
		UIPanelManager.ShowPanel("SelectTeam");
		UISelectTeam.instance.UpdateList();
		UISelectTeam.instance.timeID = TimerManager.In(0.1f, -1, 0.1f, new TimerManager.Callback(UISelectTeam.instance.UpdateList));
		UISelectTeam.instance.selectCallback = callback;
	}

	public static void OnSpectator()
	{
		UISelectTeam.instance.isSpectator = true;
	}

	private void UpdateList()
	{
		PhotonPlayer[] otherPlayers = PhotonNetwork.otherPlayers;
		this.redPlayersCount = 0;
		this.bluePlayersCount = 0;
		StringBuilder stringBuilder = new StringBuilder();
		StringBuilder stringBuilder2 = new StringBuilder();
		for (int i = 0; i < otherPlayers.Length; i++)
		{
			if (otherPlayers[i].GetTeam() == Team.Blue)
			{
				stringBuilder2.AppendLine(otherPlayers[i].UserId);
				this.bluePlayersCount++;
			}
			else if (otherPlayers[i].GetTeam() == Team.Red)
			{
				stringBuilder.AppendLine(otherPlayers[i].UserId);
				this.redPlayersCount++;
			}
		}
		this.BluePlayersLabel.text = stringBuilder2.ToString();
		this.RedPlayersLabel.text = stringBuilder.ToString();
		this.BlueCountLabel.text = this.bluePlayersCount + "/" + PhotonNetwork.room.MaxPlayers / 2;
		this.RedCountLabel.text = this.redPlayersCount + "/" + PhotonNetwork.room.MaxPlayers / 2;
		if (this.isSpectator)
		{
			if (this.bluePlayersCount > 1 && this.redPlayersCount > 0 && !PhotonNetwork.isMasterClient)
			{
				UISelectTeam.instance.SpectatorsButton.SetActive(true);
			}
		}
		else
		{
			UISelectTeam.instance.SpectatorsButton.SetActive(false);
		}
	}

	public void SelectTeam(int team)
	{
		if (!this.HasConnectTeam((Team)team) && team != 0)
		{
			return;
		}
		GameManager.team = (Team)team;
		if (this.selectCallback != null)
		{
			this.selectCallback(GameManager.team);
		}
		TimerManager.Cancel(this.timeID);
	}

	private bool HasConnectTeam(Team team)
	{
		if (team == Team.Blue)
		{
			return this.bluePlayersCount - this.redPlayersCount < 1 && PhotonNetwork.room.MaxPlayers / 2 != this.bluePlayersCount;
		}
		return team == Team.Red && this.redPlayersCount - this.bluePlayersCount < 1 && PhotonNetwork.room.MaxPlayers / 2 != this.redPlayersCount;
	}
}
