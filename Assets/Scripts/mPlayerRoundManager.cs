using System;
using UnityEngine;

public class mPlayerRoundManager : MonoBehaviour
{
    public GameObject panel;

    public UILabel modeLabel;

    public UILabel moneyLabel;

    public UILabel xpLabel;

    public UILabel killsLabel;

    public UILabel headshotLabel;

    public UILabel deathsLabel;

    public UILabel timeLabel;

    private static mPlayerRoundManager instance;

    private void Start()
	{
		mPlayerRoundManager.instance = this;
	}

	public static void Show()
	{
		mPlayerRoundManager.instance.panel.SetActive(true);
		mPlayerRoundManager.instance.modeLabel.text = Localization.Get(PlayerRoundManager.GetMode().ToString(), true);
		mPlayerRoundManager.instance.moneyLabel.text = PlayerRoundManager.GetMoney().ToString();
		mPlayerRoundManager.instance.xpLabel.text = PlayerRoundManager.GetXP().ToString();
		mPlayerRoundManager.instance.killsLabel.text = PlayerRoundManager.GetKills().ToString();
		mPlayerRoundManager.instance.headshotLabel.text = PlayerRoundManager.GetHeadshot().ToString();
		mPlayerRoundManager.instance.deathsLabel.text = PlayerRoundManager.GetDeaths().ToString();
		mPlayerRoundManager.instance.timeLabel.text = mPlayerRoundManager.ConvertTime(PlayerRoundManager.GetTime());
	}

	public void Close()
	{
		this.panel.SetActive(false);
		PlayerRoundManager.Clear();
		EventManager.Dispatch("AccountUpdate");
	}

	private static string ConvertTime(float time)
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds((double)time);
		return string.Format("{0:0}:{1:00}:{2:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
	}
}
