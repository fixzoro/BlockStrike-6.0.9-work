using System;
using System.Collections.Generic;
using Photon;
using UnityEngine;

public class mStorePlayerSkin : Photon.MonoBehaviour
{
    [Header("Player Skin")]
    public GameObject background;

    public BodyParts selectBodyParts;

    public UILabel selectSkinButtonLabel;

    public UILabel buySkinButtonLabel;

    public UITexture buySkinButtonTexture;

    public UISprite buySkinLine;

    public UITexture changeTeamButton;

    [Header("Others")]
    public Texture2D moneyTexture;

    public Texture2D goldTexture;

    public GameObject inAppPanel;

    public int headSkin;

    public int bodySkin;

    public int legsSkin;

    private Team team = Team.Blue;

    private PlayerStoreSkinData headData;

    private PlayerStoreSkinData bodyData;

    private PlayerStoreSkinData legsData;

    private List<int> headList = new List<int>();

    private List<int> bodyList = new List<int>();

    private List<int> legsList = new List<int>();

    private bool actived;

    private void Start()
	{
		UIEventListener uieventListener = UIEventListener.Get(this.background);
		uieventListener.onDrag = new UIEventListener.VectorDelegate(this.RotateWeapon);
	}

	private void OnEnable()
	{
		PhotonNetwork.onDisconnectedFromPhoton = (PhotonNetwork.VoidDelegate)Delegate.Combine(PhotonNetwork.onDisconnectedFromPhoton, new PhotonNetwork.VoidDelegate(this.OnDisconnectedFromPhoton));
	}

	private void OnDisable()
	{
		PhotonNetwork.onDisconnectedFromPhoton = (PhotonNetwork.VoidDelegate)Delegate.Remove(PhotonNetwork.onDisconnectedFromPhoton, new PhotonNetwork.VoidDelegate(this.OnDisconnectedFromPhoton));
	}

	private void OnDisconnectedFromPhoton()
	{
		this.Close();
	}

	private void RotateWeapon(GameObject go, Vector2 drag)
	{
		mPlayerCamera.Rotate(drag);
	}

	public void Show()
	{
		this.actived = true;
		mPanelManager.Show("PlayerSkin", true);
		this.GetSkinsList();
		this.headSkin = this.GetSkinIndex(AccountManager.GetPlayerSkinSelected(BodyParts.Head), BodyParts.Head);
		this.bodySkin = this.GetSkinIndex(AccountManager.GetPlayerSkinSelected(BodyParts.Body), BodyParts.Body);
		this.legsSkin = this.GetSkinIndex(AccountManager.GetPlayerSkinSelected(BodyParts.Legs), BodyParts.Legs);
		this.UpdateSkin();
	}

	public void Close()
	{
		if (!this.actived)
		{
			return;
		}
		mPanelManager.Show("Store", true);
		mPlayerCamera.Close();
		this.actived = false;
	}

	private void UpdateSkin()
	{
		this.GetPlayerSkinData();
		mPlayerCamera.Show();
		mPlayerCamera.SetSkin(this.team, this.headList[this.headSkin].ToString(), this.bodyList[this.bodySkin].ToString(), this.legsList[this.legsSkin].ToString());
		this.UpdateButtons();
	}

	private void UpdateButtons()
	{
		bool flag = AccountManager.GetPlayerSkin(this.GetSelectSkinData().ID, this.selectBodyParts);
		if (this.GetSelectSkinData().Price == 0)
		{
			flag = true;
		}
		if (flag)
		{
			this.selectSkinButtonLabel.cachedGameObject.SetActive(true);
			this.buySkinButtonLabel.cachedGameObject.SetActive(false);
			if (AccountManager.GetPlayerSkinSelected(this.selectBodyParts) == this.GetSelectSkinData().ID)
			{
				this.selectSkinButtonLabel.text = Localization.Get("Selected", true);
				this.selectSkinButtonLabel.alpha = 0.3f;
			}
			else
			{
				this.selectSkinButtonLabel.text = Localization.Get("Select", true);
				this.selectSkinButtonLabel.alpha = 1f;
			}
		}
		else
		{
			this.selectSkinButtonLabel.cachedGameObject.SetActive(false);
			this.buySkinButtonLabel.cachedGameObject.SetActive(true);
			this.buySkinButtonLabel.text = this.GetSelectSkinData().Price.ToString("n0");
			this.buySkinButtonTexture.mainTexture = ((this.GetSelectSkinData().Currency != GameCurrency.Money) ? this.goldTexture : this.moneyTexture);
			this.buySkinLine.color = ((this.GetSelectSkinData().Currency != GameCurrency.Money) ? new Color32(byte.MaxValue, 179, 0, byte.MaxValue) : new Color32(169, 174, 183, byte.MaxValue));
		}
	}

	public void NextSkin()
	{
		switch (this.selectBodyParts)
		{
		case BodyParts.Head:
			this.headSkin++;
			if (this.headSkin >= this.headList.Count)
			{
				this.headSkin = 0;
			}
			break;
		case BodyParts.Body:
			this.bodySkin++;
			if (this.bodySkin >= this.bodyList.Count)
			{
				this.bodySkin = 0;
			}
			break;
		case BodyParts.Legs:
			this.legsSkin++;
			if (this.legsSkin >= this.legsList.Count)
			{
				this.legsSkin = 0;
			}
			break;
		}
		this.UpdateSkin();
	}

	public void LastSkin()
	{
		switch (this.selectBodyParts)
		{
		case BodyParts.Head:
			this.headSkin--;
			if (this.headSkin <= -1)
			{
				this.headSkin = this.headList.Count - 1;
			}
			break;
		case BodyParts.Body:
			this.bodySkin--;
			if (this.bodySkin <= -1)
			{
				this.bodySkin = this.bodyList.Count - 1;
			}
			break;
		case BodyParts.Legs:
			this.legsSkin--;
			if (this.legsSkin <= -1)
			{
				this.legsSkin = this.legsList.Count - 1;
			}
			break;
		}
		this.UpdateSkin();
	}

	public void ChangeTeam()
	{
		if (this.team == Team.Blue)
		{
			this.team = Team.Red;
			this.changeTeamButton.color = new Color32(237, 44, 45, byte.MaxValue);
		}
		else
		{
			this.team = Team.Blue;
			this.changeTeamButton.color = new Color32(70, 136, 231, byte.MaxValue);
		}
		this.UpdateSkin();
	}

	public void SelectBodyParts(int select)
	{
		this.selectBodyParts = (BodyParts)select;
		this.headSkin = this.GetSkinIndex(AccountManager.GetPlayerSkinSelected(BodyParts.Head), BodyParts.Head);
		this.bodySkin = this.GetSkinIndex(AccountManager.GetPlayerSkinSelected(BodyParts.Body), BodyParts.Body);
		this.legsSkin = this.GetSkinIndex(AccountManager.GetPlayerSkinSelected(BodyParts.Legs), BodyParts.Legs);
		this.UpdateSkin();
	}

	public void SelectSkin()
	{
		if (AccountManager.GetPlayerSkinSelected(this.selectBodyParts) == this.GetSelectSkinData().ID)
		{
			return;
		}
		AccountManager.SetPlayerSkinSelected(this.GetSelectSkinData().ID, this.selectBodyParts);
        AccountManager.UpdatePlayerSkin(null, null);
        this.UpdateButtons();
	}

	public void BuySkin()
	{
        if (!AccountManager.isConnect)
        {
            UIToast.Show(Localization.Get("Connection account"));
            return;
        }
        if (this.GetSelectSkinData().Currency == GameCurrency.Gold && this.GetSelectSkinData().Price > AccountManager.GetGold())
        {
            this.inAppPanel.SetActive(true);
            UIToast.Show(Localization.Get("Not enough money"));
            return;
        }
        if (this.GetSelectSkinData().Currency == GameCurrency.Money && this.GetSelectSkinData().Price > AccountManager.GetMoney())
        {
            this.inAppPanel.SetActive(true);
            UIToast.Show(Localization.Get("Not enough money"));
            return;
        }
        mPopUp.ShowPopup(Localization.Get("Do you really want to buy?"), Localization.Get("Player Skin"), Localization.Get("Yes"), delegate ()
        {
            mPopUp.HideAll();
            mPopUp.SetActiveWaitPanel(true, Localization.Get("Please wait", true));
            AccountManager.SetPlayerSkin(this.GetSelectSkinData().ID, this.selectBodyParts);
            AccountManager.SetPlayerSkinSelected(this.GetSelectSkinData().ID, this.selectBodyParts);
            AccountManager.UpdatePlayerSkin(delegate(string result) 
            {
                mPopUp.SetActiveWaitPanel(false);
                if (this.GetSelectSkinData().Currency == GameCurrency.Money)
                {
                    AccountManager.UpdateMoney(AccountManager.GetMoney() - this.GetSelectSkinData().Price, null, null);
                    AccountManager.SetMoney(AccountManager.GetMoney() - this.GetSelectSkinData().Price, false);
                }
                if (this.GetSelectSkinData().Currency == GameCurrency.Gold)
                {
                    AccountManager.UpdateGold(AccountManager.GetGold() - this.GetSelectSkinData().Price, null, null);
                    AccountManager.SetGold(AccountManager.GetGold() - this.GetSelectSkinData().Price, false);
                }
                this.UpdateButtons();
                EventManager.Dispatch("AccountUpdate");
            }, delegate (string error) 
            {
                mPopUp.SetActiveWaitPanel(false);
                UIToast.Show(error);
            });
        }, Localization.Get("No"), delegate ()
        {
            mPopUp.HideAll();
        });
    }

	private void GetSkinsList()
	{
		this.headList.Clear();
		this.bodyList.Clear();
		this.legsList.Clear();
		for (int i = 0; i < GameSettings.instance.PlayerStoreHead.Count; i++)
		{
			this.headList.Add(GameSettings.instance.PlayerStoreHead[i].ID);
		}
		for (int j = 0; j < GameSettings.instance.PlayerStoreBody.Count; j++)
		{
			this.bodyList.Add(GameSettings.instance.PlayerStoreBody[j].ID);
		}
		for (int k = 0; k < GameSettings.instance.PlayerStoreLegs.Count; k++)
		{
			this.legsList.Add(GameSettings.instance.PlayerStoreLegs[k].ID);
		}
	}

	private void GetPlayerSkinData()
	{
		switch (this.selectBodyParts)
		{
		case BodyParts.Head:
			for (int i = 0; i < GameSettings.instance.PlayerStoreHead.Count; i++)
			{
				if (this.headList[this.headSkin] == GameSettings.instance.PlayerStoreHead[i].ID)
				{
					this.headData = GameSettings.instance.PlayerStoreHead[i];
					break;
				}
			}
			break;
		case BodyParts.Body:
			for (int j = 0; j < GameSettings.instance.PlayerStoreBody.Count; j++)
			{
				if (this.bodyList[this.bodySkin] == GameSettings.instance.PlayerStoreBody[j].ID)
				{
					this.bodyData = GameSettings.instance.PlayerStoreBody[j];
					break;
				}
			}
			break;
		case BodyParts.Legs:
			for (int k = 0; k < GameSettings.instance.PlayerStoreLegs.Count; k++)
			{
				if (this.legsList[this.legsSkin] == GameSettings.instance.PlayerStoreLegs[k].ID)
				{
					this.legsData = GameSettings.instance.PlayerStoreLegs[k];
					break;
				}
			}
			break;
		}
	}

	private PlayerStoreSkinData GetSelectSkinData()
	{
		switch (this.selectBodyParts)
		{
		case BodyParts.Head:
			return this.headData;
		case BodyParts.Body:
			return this.bodyData;
		case BodyParts.Legs:
			return this.legsData;
		default:
			return this.bodyData;
		}
	}

	private int GetSkinIndex(int skinID, BodyParts part)
	{
		switch (part)
		{
		case BodyParts.Head:
			for (int i = 0; i < this.headList.Count; i++)
			{
				if (skinID == this.headList[i])
				{
					return i;
				}
			}
			break;
		case BodyParts.Body:
			for (int j = 0; j < this.bodyList.Count; j++)
			{
				if (skinID == this.bodyList[j])
				{
					return j;
				}
			}
			break;
		case BodyParts.Legs:
			for (int k = 0; k < this.legsList.Count; k++)
			{
				if (skinID == this.legsList[k])
				{
					return k;
				}
			}
			break;
		}
		return 0;
	}
}
