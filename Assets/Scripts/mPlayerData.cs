using UnityEngine;

public class mPlayerData : MonoBehaviour
{
    public UILabel PlayerNameLabel;

    public UILabel PlayerLevelLabel;

    public UIProgressBar PlayerXP;

    public UILabel MoneyLabel;

    public UILabel GoldLabel;

    public UITexture AvatarTexture;

    private void Start()
	{
		EventManager.AddListener("AccountUpdate", new EventManager.Callback(this.AccountUpdate));
		EventManager.AddListener("AvatarUpdate", new EventManager.Callback(this.AvatarUpdate));
		this.AccountUpdate();
		this.AvatarUpdate();
	}

	private void AccountUpdate()
	{
		if (string.IsNullOrEmpty(AccountManager.instance.Data.Clan.ToString()))
		{
			this.PlayerNameLabel.text = AccountManager.instance.Data.AccountName;
		}
		else
		{
			this.PlayerNameLabel.text = AccountManager.instance.Data.AccountName + " - " + AccountManager.instance.Data.Clan;
		}
		this.PlayerLevelLabel.text = Localization.Get("Level", true) + " - " + AccountManager.GetLevel().ToString();
		this.PlayerXP.value = (float)AccountManager.GetXP() / (float)AccountManager.GetMaxXP();
		this.GoldLabel.text = AccountManager.GetGold().ToString("n0");
		this.MoneyLabel.text = AccountManager.GetMoney().ToString("n0");
	}

	private void AvatarUpdate()
	{
		this.AvatarTexture.mainTexture = AccountManager.instance.Data.Avatar;
	}
}
