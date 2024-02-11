using System;
using System.Collections.Generic;
using System.Text;
using Photon;
using UnityEngine;

public class mStoreWeapons : Photon.MonoBehaviour
{
    [Header("Weapon Info")]
    public mStoreWeapons.WeaponInfo weaponInfo;

    [Header("Weapon Panel")]
    public UIWidget background;

    public UIWidget[] weaponButtonsBackground;

    public UILabel selectWeaponButtonLabel;

    public UILabel weaponBuyButtonLabel;

    public UITexture weaponBuyIcon;

    public UISprite weaponBuyLine;

    public UILabel fireStatLabel;

    private List<string> weaponsList = new List<string>();

    private int Weapon;

    private int WeaponSkin;

    private WeaponType weaponType;

    private WeaponData weaponData;

    private WeaponStoreData weaponStoreData;

    [Header("Weapon Skin")]
    public Color normalColor = new Color(255f, 255f, 255f, 255f);

    public Color baseColor = new Color(33f, 153f, 255f, 255f);

    public Color professionalColor = new Color(255f, 57f, 57f, 255f);

    public Color legendaryColor = new Color(255f, 32f, 147f, 255f);

    public UILabel skinNameLabel;

    public UILabel skinRarityLabel;

    public UILabel selectSkinButtonLabel;

    public UILabel selectSkinCountLabel;

    public GameObject skinDropInCase;

    public UILabel skinTemporarySkin;

    [Header("Weapon Stickers")]
    public UITexture stickersButton;

    public BoxCollider stickersButtonCollider;

    public UIScrollView stickerScrollView;

    public UISprite stickerElement;

    public List<UISprite> stickerElementList = new List<UISprite>();

    public List<UISprite> stickerElementPool = new List<UISprite>();

    private int stickerPos = 1;

    public UILabel stickerName;

    public UIWidget selectStickerButton;

    public UIWidget deleteStickerButton;

    public UISprite previewStickerSprite;

    private int weaponStickersCount;

    private int[] stickers;

    private int selectSticker;

    [Header("Others")]
    public GameObject weaponPanel;

    public GameObject skinPanel;

    public GameObject stickersPanel;

    public UISprite lineSprite;

    public GameObject inAppPanel;

    public Texture2D moneyTexture;

    public Texture2D goldTexture;

    private mStoreWeapons.SelectPanel selectPanel;

    private bool Active;

    public enum SelectPanel
    {
        Weapon,
        Skins,
        Stickers
    }

    private void Start()
	{
		UIEventListener.Get(this.background.cachedGameObject).onDrag = new UIEventListener.VectorDelegate(this.RotateWeapon);
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
		mWeaponCamera.Rotate(drag, this.selectPanel == mStoreWeapons.SelectPanel.Weapon);
	}

	public void Show(int type)
	{
		this.Active = true;
		this.background.cachedGameObject.SetActive(true);
		this.Weapon = 0;
		this.weaponType = (WeaponType)type;
		if (type > 0)
		{
			type--;
		}
		for (int i = 0; i < this.weaponButtonsBackground.Length; i++)
		{
			this.weaponButtonsBackground[i].color = new Color(1f, 1f, 1f, 0f);
		}
		this.weaponButtonsBackground[type].color = Color.white;
		this.GetWeaponList();
		this.UpdateWeapon();
	}

	public void Close()
	{
		if (!this.Active)
		{
			return;
		}
		this.background.cachedGameObject.SetActive(false);
		mPanelManager.Show("Store", true);
		mWeaponCamera.ResetRotateX(false);
		mWeaponCamera.SetViewportRect(new Rect(0f, 0f, 1f, 1f), 0f);
		mWeaponCamera.Close();
		this.selectPanel = mStoreWeapons.SelectPanel.Weapon;
		this.Active = false;
	}

	public void ShowHideWeaponInfoPanel()
	{
		this.weaponInfo.panel.SetActive(!this.weaponInfo.panel.activeSelf);
	}

	public void NextWeapon()
	{
		if (this.selectPanel != mStoreWeapons.SelectPanel.Weapon)
		{
			return;
		}
		this.Weapon++;
		if (this.Weapon >= this.weaponsList.Count)
		{
			this.Weapon = 0;
		}
		this.UpdateWeapon();
	}

	public void LastWeapon()
	{
		if (this.selectPanel != mStoreWeapons.SelectPanel.Weapon)
		{
			return;
		}
		this.Weapon--;
		if (this.Weapon <= -1)
		{
			this.Weapon = this.weaponsList.Count - 1;
		}
		this.UpdateWeapon();
	}

	private void UpdateWeapon()
	{
		this.GetWeaponData();
		this.GetWeaponStoreData();
		this.WeaponSkin = AccountManager.GetWeaponSkinSelected(this.weaponData.ID);
		mWeaponCamera.SetViewportRect(new Rect(0f, 0f, 1f, 1f), 0f);
		mWeaponCamera.Show(this.weaponsList[this.Weapon]);
		bool flag = AccountManager.GetWeapon(this.weaponData.ID);
		if (this.weaponStoreData.Price == 0)
		{
			flag = true;
		}
		if (flag)
		{
			this.SelectedWeaponPanel();
		}
		else
		{
			this.BuyWeaponPanel();
		}
		this.stickersButtonCollider.enabled = mWeaponCamera.HasStickers();
		if (mWeaponCamera.HasStickers())
		{
			this.stickersButton.color = Color.white;
		}
		else
		{
			this.stickersButton.color = new Color(1f, 1f, 1f, 0.4f);
		}
		this.UpdateWeaponData();
		this.UpdateWeaponSkin();
	}

	private void SelectedWeaponPanel()
	{
		this.selectWeaponButtonLabel.cachedGameObject.SetActive(true);
		this.weaponBuyButtonLabel.cachedGameObject.SetActive(false);
		if (AccountManager.GetWeaponSelected(this.weaponType) == this.weaponData.ID)
		{
			this.selectWeaponButtonLabel.text = Localization.Get("Selected", true);
			this.selectWeaponButtonLabel.alpha = 0.3f;
			return;
		}
		this.selectWeaponButtonLabel.text = Localization.Get("Select", true);
		this.selectWeaponButtonLabel.alpha = 1f;
	}

	public void SelectWeapon()
	{
		if (AccountManager.GetWeaponSelected(this.weaponType) == this.weaponData.ID)
		{
			return;
		}
		if ((this.weaponData.Secret && AccountManager.GetGold() < 0) || (this.weaponData.Secret && AccountManager.GetMoney() < 0))
		{
			return;
		}
		AccountManager.SetWeaponSelected(this.weaponType, this.weaponData.ID);
		this.SelectedWeaponPanel();
		WeaponManager.UpdateData();
		this.UpdateWeaponSkin();
		AccountManager.SetFirebaseWeaponsSelected(null, null);
	}

	private void BuyWeaponPanel()
	{
		this.selectWeaponButtonLabel.cachedGameObject.SetActive(false);
		this.selectWeaponButtonLabel.alpha = 1f;
		this.weaponBuyButtonLabel.cachedGameObject.SetActive(true);
		this.weaponBuyButtonLabel.text = this.weaponStoreData.Price.ToString("n0");
		this.weaponBuyIcon.mainTexture = ((this.weaponStoreData.Currency != GameCurrency.Gold) ? this.moneyTexture : this.goldTexture);
		this.weaponBuyLine.color = ((this.weaponStoreData.Currency != GameCurrency.Gold) ? new Color32(169, 174, 183, byte.MaxValue) : new Color32(byte.MaxValue, 179, 0, byte.MaxValue));
	}

	public void BuyWeapon()
	{
		if (!AccountManager.isConnect)
		{
			UIToast.Show(Localization.Get("Connection account", true));
			return;
		}
		int num = (this.weaponStoreData.Currency != GameCurrency.Gold) ? AccountManager.GetMoney() : AccountManager.GetGold();
		if (this.weaponStoreData.Price > num)
		{
			this.inAppPanel.SetActive(true);
			UIToast.Show(Localization.Get("Not enough money", true));
			return;
		}
		mPopUp.ShowPopup(Localization.Get("Do you really want to buy?", true), Localization.Get("Weapon", true), Localization.Get("Yes", true), delegate()
		{
			mPopUp.HideAll();
			mPopUp.SetActiveWaitPanel(true, Localization.Get("Please wait", true));
			AccountManager.SetFirebaseWeaponsBuy(this.weaponData.ID, delegate
			{
                AccountManager.SetWeapon(this.weaponData.ID);
                mPopUp.SetActiveWaitPanel(false);
                AccountManager.SetWeaponSelected(this.weaponType, this.weaponData.ID);
                AccountManager.SetFirebaseWeaponsSelected(null, null);
                if (this.weaponStoreData.Currency == GameCurrency.Gold)
                {
                    this.MoneySet(true);
                }
                else
                {
                    this.MoneySet(false);
                }
                this.SelectedWeaponPanel();
                EventManager.Dispatch("AccountUpdate");
                WeaponManager.UpdateData();
            }, delegate(string e)
			{
				mPopUp.SetActiveWaitPanel(false);
				UIToast.Show(e);
			});
		}, Localization.Get("No", true), delegate()
		{
			mPopUp.HideAll();
		});
	}

	private void UpdateWeaponData()
	{
		this.weaponInfo.nameLabel.text = this.weaponData.Name;
		this.SetDamage();
		this.SetFireRate();
		this.SetAccuracy();
		this.SetAmmo();
		this.SetMaxAmmo();
		this.SetMobility();
	}

	private void UpdateWeaponSkin()
	{
		this.skinNameLabel.text = this.weaponData.Name + " | " + this.weaponStoreData.Skins[this.WeaponSkin].Name;
		this.fireStatLabel.cachedTransform.parent.gameObject.SetActive(AccountManager.GetFireStat(this.weaponData.ID, this.weaponStoreData.Skins[this.WeaponSkin].ID) && this.weaponData.Type != WeaponType.Knife);
		if (this.fireStatLabel.cachedTransform.parent.gameObject.activeSelf)
		{
			this.fireStatLabel.text = Localization.Get("Kills", true) + ": " + AccountManager.GetFireStatCounter(this.weaponData.ID, this.weaponStoreData.Skins[this.WeaponSkin].ID);
		}
		mWeaponCamera.SetSkin(this.weaponData.ID, this.weaponStoreData.Skins[this.WeaponSkin].ID);
		if (AccountManager.GetWeaponSkin(this.weaponData.ID, this.weaponStoreData.Skins[this.WeaponSkin].ID))
		{
			this.selectSkinButtonLabel.cachedGameObject.SetActive(true);
			if (AccountManager.GetWeaponSkinSelected(this.weaponData.ID) == this.weaponStoreData.Skins[this.WeaponSkin].ID)
			{
				this.selectSkinButtonLabel.text = Localization.Get("Selected", true);
				this.selectSkinButtonLabel.alpha = 0.5f;
			}
			else
			{
				this.selectSkinButtonLabel.text = Localization.Get("Select", true);
				this.selectSkinButtonLabel.alpha = 1f;
			}
			this.skinDropInCase.SetActive(false);
		}
		else
		{
			this.skinDropInCase.SetActive(true);
			this.selectSkinButtonLabel.cachedGameObject.SetActive(false);
		}
		if (string.IsNullOrEmpty(this.weaponStoreData.Skins[this.WeaponSkin].Temporary))
		{
			this.skinTemporarySkin.cachedGameObject.SetActive(false);
		}
		else
		{
			this.skinTemporarySkin.cachedGameObject.SetActive(true);
			this.skinTemporarySkin.text = Localization.Get("You can get to", true) + ": " + this.weaponStoreData.Skins[this.WeaponSkin].Temporary;
		}
		this.selectSkinCountLabel.text = (this.WeaponSkin + 1).ToString() + "/" + this.weaponStoreData.Skins.Count;
		switch (this.weaponStoreData.Skins[this.WeaponSkin].Quality)
		{
		case WeaponSkinQuality.Default:
			this.skinRarityLabel.text = Localization.Get("Normal quality", true);
			TweenColor.Begin(this.lineSprite.cachedGameObject, 0.5f, this.normalColor);
			break;
		case WeaponSkinQuality.Normal:
			this.skinRarityLabel.text = Localization.Get("Normal quality", true);
			TweenColor.Begin(this.lineSprite.cachedGameObject, 0.5f, this.normalColor);
			break;
		case WeaponSkinQuality.Basic:
			this.skinRarityLabel.text = Localization.Get("Basic quality", true);
			TweenColor.Begin(this.lineSprite.cachedGameObject, 0.5f, this.baseColor);
			break;
		case WeaponSkinQuality.Professional:
			this.skinRarityLabel.text = Localization.Get("Professional quality", true);
			TweenColor.Begin(this.lineSprite.cachedGameObject, 0.5f, this.professionalColor);
			break;
		case WeaponSkinQuality.Legendary:
			this.skinRarityLabel.text = Localization.Get("Legendary quality", true);
			TweenColor.Begin(this.lineSprite.cachedGameObject, 0.5f, this.legendaryColor);
			break;
		}
		this.UpdateWeaponStickers();
	}

	public void NextSkin()
	{
		if (this.selectPanel != mStoreWeapons.SelectPanel.Skins)
		{
			return;
		}
		if (this.weaponData.Secret)
		{
			List<int> list = new List<int>();
			for (int i = 0; i < this.weaponStoreData.Skins.Count; i++)
			{
				if (AccountManager.GetWeaponSkin(this.weaponData.ID, this.weaponStoreData.Skins[i].ID))
				{
					list.Add(i);
				}
			}
			int j = 0;
			while (j < list.Count)
			{
				if (this.WeaponSkin == list[j])
				{
					if (this.WeaponSkin == list[list.Count - 1])
					{
						this.WeaponSkin = list[0];
						break;
					}
					this.WeaponSkin = list[j + 1];
					break;
				}
				else
				{
					j++;
				}
			}
		}
		else
		{
			this.WeaponSkin++;
			if (this.WeaponSkin >= this.weaponStoreData.Skins.Count)
			{
				this.WeaponSkin = 0;
			}
			if (!string.IsNullOrEmpty(this.weaponStoreData.Skins[this.WeaponSkin].Temporary) && !AccountManager.GetWeaponSkin(this.weaponData.ID, this.weaponStoreData.Skins[this.WeaponSkin].ID))
			{
				string[] array = this.weaponStoreData.Skins[this.WeaponSkin].Temporary.Split(new char[]
				{
					"."[0]
				});
				if (array.Length != 3)
				{
					UnityEngine.MonoBehaviour.print("Error Temporary: " + this.weaponStoreData.Skins[this.WeaponSkin].Name + " " + this.weaponStoreData.Skins[this.WeaponSkin].Temporary);
					this.NextSkin();
					return;
				}
				if (new DateTime(2000 + int.Parse(array[2]), int.Parse(array[1]), int.Parse(array[0])) < DateTime.UtcNow)
				{
					this.NextSkin();
					return;
				}
			}
		}
		this.UpdateWeaponSkin();
	}

	public void LastSkin()
	{
		if (this.selectPanel != mStoreWeapons.SelectPanel.Skins)
		{
			return;
		}
		if (this.weaponData.Secret)
		{
			List<int> list = new List<int>();
			for (int i = 0; i < this.weaponStoreData.Skins.Count; i++)
			{
				if (AccountManager.GetWeaponSkin(this.weaponData.ID, this.weaponStoreData.Skins[i].ID))
				{
					list.Add(i);
				}
			}
			int j = 0;
			while (j < list.Count)
			{
				if (this.WeaponSkin == list[j])
				{
					if (this.WeaponSkin == list[0])
					{
						this.WeaponSkin = list[list.Count - 1];
						break;
					}
					this.WeaponSkin = list[j - 1];
					break;
				}
				else
				{
					j++;
				}
			}
		}
		else
		{
			this.WeaponSkin--;
			if (this.WeaponSkin < 0)
			{
				this.WeaponSkin = this.weaponStoreData.Skins.Count - 1;
			}
			if (!string.IsNullOrEmpty(this.weaponStoreData.Skins[this.WeaponSkin].Temporary) && !AccountManager.GetWeaponSkin(this.weaponData.ID, this.weaponStoreData.Skins[this.WeaponSkin].ID))
			{
				string[] array = this.weaponStoreData.Skins[this.WeaponSkin].Temporary.Split(new char[]
				{
					"."[0]
				});
				if (array.Length != 3)
				{
					UnityEngine.MonoBehaviour.print("Error Temporary: " + this.weaponStoreData.Skins[this.WeaponSkin].Name + " " + this.weaponStoreData.Skins[this.WeaponSkin].Temporary);
					this.LastSkin();
					return;
				}
				if (new DateTime(2000 + int.Parse(array[2]), int.Parse(array[1]), int.Parse(array[0])) < DateTime.UtcNow)
				{
					this.LastSkin();
					return;
				}
			}
		}
		this.UpdateWeaponSkin();
	}

	public void SelectSkin()
	{
		if (this.selectPanel != mStoreWeapons.SelectPanel.Skins)
		{
			return;
		}
		if (AccountManager.GetGold() < 0 || AccountManager.GetMoney() < 0)
		{
			return;
		}
		AccountManager.SetWeaponSkinSelected(this.weaponData.ID, this.weaponStoreData.Skins[this.WeaponSkin].ID);
		this.UpdateWeaponSkin();
		AccountManager.SetWeaponSkinSelected2(this.weaponData.ID, this.weaponStoreData.Skins[this.WeaponSkin].ID, null, null);
	}

	public void NextStickerPos()
	{
		this.stickerPos++;
		if (this.stickerPos >= this.weaponStickersCount)
		{
			this.stickerPos = 1;
		}
		this.UpdateSelectedSticker();
	}

	public void LastStickerPos()
	{
		this.stickerPos--;
		if (this.stickerPos <= 0)
		{
			this.stickerPos = this.weaponStickersCount;
		}
		this.UpdateSelectedSticker();
	}

	private void UpdateWeaponStickers()
	{
		mWeaponCamera.SetStickers(AccountManager.GetWeaponStickers(this.weaponData.ID, this.weaponStoreData.Skins[this.WeaponSkin].ID));
	}

	private void UpdateSelectedSticker()
	{
		int weaponSticker = AccountManager.GetWeaponSticker(this.weaponData.ID, this.weaponStoreData.Skins[this.WeaponSkin].ID, this.stickerPos);
		if (weaponSticker != -1)
		{
			StickerData stickerData = WeaponManager.GetStickerData(weaponSticker);
			mWeaponCamera.ActivePrevSticker(this.stickerPos, stickerData.ID);
			this.selectStickerButton.cachedGameObject.SetActive(false);
			this.deleteStickerButton.cachedGameObject.SetActive(true);
			this.previewStickerSprite.cachedGameObject.SetActive(true);
			this.previewStickerSprite.spriteName = stickerData.ID.ToString();
			this.stickerName.text = stickerData.Name;
			return;
		}
		if (this.stickers.Length != 0)
		{
			StickerData stickerData2 = WeaponManager.GetStickerData(this.stickers[this.selectSticker]);
			mWeaponCamera.ActivePrevSticker(this.stickerPos, stickerData2.ID);
			this.selectStickerButton.cachedGameObject.SetActive(true);
			this.deleteStickerButton.cachedGameObject.SetActive(false);
			this.previewStickerSprite.cachedGameObject.SetActive(true);
			this.previewStickerSprite.spriteName = stickerData2.ID.ToString();
			this.stickerName.text = stickerData2.Name;
			return;
		}
		mWeaponCamera.DeactivePrevSticker();
		this.selectStickerButton.cachedGameObject.SetActive(false);
		this.deleteStickerButton.cachedGameObject.SetActive(false);
		this.stickerName.text = string.Empty;
		this.previewStickerSprite.cachedGameObject.SetActive(false);
	}

	public void SetSticker()
	{
		if (this.selectPanel != mStoreWeapons.SelectPanel.Stickers)
		{
			return;
		}
		if (AccountManager.GetGold() < 0 || AccountManager.GetMoney() < 0)
		{
			return;
		}
		mPopUp.ShowPopup(Localization.Get("Do you really want to stick a sticker?", true), Localization.Get("Stickers", true), Localization.Get("Yes", true), delegate()
		{
            mPopUp.HideAll();
            AccountManager.DeleteSticker(this.stickers[this.selectSticker], false, null, null);
            AccountManager.UpdateStickersFirebase(null, null);
            AccountManager.SetWeaponSticker(this.weaponData.ID, this.weaponStoreData.Skins[this.WeaponSkin].ID, this.stickerPos, this.stickers[this.selectSticker]);
            AccountManager.AddWeaponStickerFirebase(this.weaponData.ID, this.weaponStoreData.Skins[this.WeaponSkin].ID, this.stickers[this.selectSticker], this.stickerPos);
            this.stickers = AccountManager.GetStickers();
            this.selectSticker = 0;
            this.UpdateSelectedSticker();
            this.UpdateStickerScroll();
        }, Localization.Get("No", true), delegate()
		{
			mPopUp.HideAll();
		});
	}

	public void DeleteSticker()
	{
		if (AccountManager.GetGold() < 0 || AccountManager.GetMoney() < 0)
		{
			return;
		}
        mPopUp.ShowPopup(Localization.Get("Do you really want to remove the sticker? The sticker will be destroyed!"), Localization.Get("Delete"), Localization.Get("Yes"), delegate ()
        {
            mPopUp.HideAll();
            AccountManager.DeleteWeaponSticker(this.weaponData.ID, this.weaponStoreData.Skins[this.WeaponSkin].ID, this.stickerPos);
            AccountManager.DeleteWeaponStickerFirebase(this.weaponData.ID, this.weaponStoreData.Skins[this.WeaponSkin].ID, this.stickerPos);
            this.UpdateSelectedSticker();
        }, Localization.Get("No"), delegate ()
        {
            mPopUp.HideAll();
        });
    }

	public void SelectSticker(GameObject go)
	{
		this.selectSticker = int.Parse(go.name);
		this.UpdateSelectedSticker();
	}

	private void UpdateStickerScroll()
	{
		this.stickerScrollView.ResetPosition();
		this.ClearStickerElements();
		for (int i = 0; i < this.stickers.Length; i++)
		{
			int stickerCount = AccountManager.GetStickerCount(this.stickers[i]);
			if (stickerCount > 0)
			{
				UISprite uisprite = this.GetStickerElement();
				uisprite.spriteName = this.stickers[i].ToString();
				uisprite.cachedTransform.localPosition = new Vector3((float)(50 * i), 0f, 0f);
				uisprite.cachedGameObject.name = i.ToString();
				uisprite.GetComponentInChildren<UILabel>().text = stickerCount.ToString();
				this.stickerElementList.Add(uisprite);
			}
		}
	}

	private UISprite GetStickerElement()
	{
		if (this.stickerElementPool.Count > 0)
		{
			UISprite uisprite = this.stickerElementPool[0];
			this.stickerElementPool.RemoveAt(0);
			uisprite.cachedGameObject.SetActive(true);
			return uisprite;
		}
		GameObject gameObject = this.stickerScrollView.gameObject.AddChild(this.stickerElement.cachedGameObject);
		gameObject.SetActive(true);
		return gameObject.GetComponent<UISprite>();
	}

	private void ClearStickerElements()
	{
		for (int i = 0; i < this.stickerElementList.Count; i++)
		{
			this.stickerElementPool.Add(this.stickerElementList[i]);
		}
		this.stickerElementList.Clear();
	}

	private void GetWeaponList()
	{
		this.weaponsList.Clear();
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			if (GameSettings.instance.Weapons[i].Type == this.weaponType)
			{
				int num = GameSettings.instance.Weapons[i].ID;
				if (num == 4 || num == 3 || num == 12)
				{
					this.weaponsList.Insert(0, GameSettings.instance.Weapons[i].Name);
				}
				else if (!GameSettings.instance.Weapons[i].Lock)
				{
					if (GameSettings.instance.Weapons[i].Secret)
					{
						if (AccountManager.GetWeapon(GameSettings.instance.Weapons[i].ID))
						{
							this.weaponsList.Add(GameSettings.instance.Weapons[i].Name);
						}
					}
					else
					{
						this.weaponsList.Add(GameSettings.instance.Weapons[i].Name);
					}
				}
			}
		}
	}

	private void GetWeaponData()
	{
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			if (GameSettings.instance.Weapons[i].Name == this.weaponsList[this.Weapon])
			{
				this.weaponData = GameSettings.instance.Weapons[i];
				return;
			}
		}
	}

	private void GetWeaponStoreData()
	{
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			if (GameSettings.instance.Weapons[i].Name == this.weaponsList[this.Weapon])
			{
				this.weaponStoreData = GameSettings.instance.WeaponsStore[i];
				return;
			}
		}
	}

	private void SetDamage()
	{
		float num = (float)((this.weaponData.FaceDamage * this.weaponData.FireBullets + this.weaponData.BodyDamage * this.weaponData.FireBullets + this.weaponData.HandDamage * this.weaponData.FireBullets + this.weaponData.LegDamage * this.weaponData.FireBullets) / 4);
		if (this.weaponData.CanFire)
		{
			this.weaponInfo.damageLabel.text = num.ToString();
			this.weaponInfo.damageProgressBar.value = num / this.weaponInfo.maxDamage;
			return;
		}
		this.weaponInfo.damageLabel.text = "-";
		this.weaponInfo.damageProgressBar.value = 0f;
	}

	private void SetFireRate()
	{
		if (this.weaponData.Type == WeaponType.Knife || !this.weaponData.CanFire)
		{
			this.weaponInfo.fireRateLabel.text = "-";
			this.weaponInfo.fireRateProgressBar.value = 0f;
			return;
		}
		int num = Mathf.FloorToInt(100f - this.weaponData.FireRate * 100f / 1.5f);
		this.weaponInfo.fireRateLabel.text = num.ToString();
		this.weaponInfo.fireRateProgressBar.value = (float)num / this.weaponInfo.maxFireRate;
	}

	private void SetAccuracy()
	{
		if (this.weaponData.Type == WeaponType.Knife || !this.weaponData.CanFire)
		{
			this.weaponInfo.accuracyLabel.text = "-";
			this.weaponInfo.accuracyProgressBar.value = 0f;
			return;
		}
		int num = Mathf.FloorToInt(100f - this.weaponData.FireAccuracy - this.weaponData.Accuracy);
		this.weaponInfo.accuracyLabel.text = num.ToString();
		this.weaponInfo.accuracyProgressBar.value = (float)num / this.weaponInfo.maxAccuracy;
	}

	private void SetAmmo()
	{
		if (this.weaponData.Type == WeaponType.Knife || !this.weaponData.CanFire)
		{
			this.weaponInfo.ammoLabel.text = "-";
			this.weaponInfo.ammoProgressBar.value = 0f;
			return;
		}
		int num = this.weaponData.Ammo;
		this.weaponInfo.ammoLabel.text = num.ToString();
		this.weaponInfo.ammoProgressBar.value = (float)num / this.weaponInfo.maxAmmo;
	}

	private void SetMaxAmmo()
	{
		if (this.weaponData.Type == WeaponType.Knife || !this.weaponData.CanFire)
		{
			this.weaponInfo.maxAmmoLabel.text = "-";
			this.weaponInfo.maxAmmoProgressBar.value = 0f;
			return;
		}
		int num = this.weaponData.MaxAmmo;
		this.weaponInfo.maxAmmoLabel.text = num.ToString();
		this.weaponInfo.maxAmmoProgressBar.value = (float)num / this.weaponInfo.maxMaxAmmo;
	}

	private void SetMobility()
	{
		int num = Mathf.FloorToInt(100f - this.weaponData.Mass * 1000f);
		if (!this.weaponData.CanFire)
		{
			this.weaponInfo.mobilityLabel.text = "-";
			return;
		}
		this.weaponInfo.mobilityLabel.text = num.ToString();
		this.weaponInfo.mobilityProgressBar.value = (float)num / this.weaponInfo.maxMobility;
	}

	public void ShowStickersPanel()
	{
		if (this.selectPanel != mStoreWeapons.SelectPanel.Weapon)
		{
			return;
		}
		this.selectPanel = mStoreWeapons.SelectPanel.Stickers;
		this.weaponPanel.SetActive(false);
		this.stickersPanel.SetActive(true);
		this.stickers = AccountManager.GetStickers();
		this.selectSticker = 0;
		this.weaponStickersCount = mWeaponCamera.GetStickersCount();
		this.UpdateSelectedSticker();
		this.UpdateStickerScroll();
	}

	public void ShowSkinPanel()
	{
		if (this.selectPanel != mStoreWeapons.SelectPanel.Weapon)
		{
			return;
		}
		this.selectPanel = mStoreWeapons.SelectPanel.Skins;
		this.weaponPanel.SetActive(false);
		this.skinPanel.SetActive(true);
	}

	public void ShowWeaponPanel()
	{
		mWeaponCamera.ResetRotateX(true);
		if (this.selectPanel == mStoreWeapons.SelectPanel.Skins)
		{
			this.skinPanel.SetActive(false);
		}
		else if (this.selectPanel == mStoreWeapons.SelectPanel.Stickers)
		{
			this.stickersPanel.SetActive(false);
			this.previewStickerSprite.cachedGameObject.SetActive(false);
			mWeaponCamera.DeactivePrevSticker();
		}
		this.weaponPanel.SetActive(true);
		this.selectPanel = mStoreWeapons.SelectPanel.Weapon;
		this.WeaponSkin = AccountManager.GetWeaponSkinSelected(this.weaponData.ID);
		this.UpdateWeaponSkin();
	}

	public void ShareScreenshot()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		this.skinPanel.SetActive(false);
		TimerManager.In(0.5f, delegate()
		{
			AndroidNativeFunctions.TakeScreenshot(string.Concat(new string[]
			{
				this.weaponData.Name,
				" | ",
				this.weaponStoreData.Skins[this.WeaponSkin].Name,
				"_",
				DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")
			}), delegate(string path)
			{
				this.skinPanel.SetActive(true);
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine("#BlockStrike #BS");
				stringBuilder.AppendLine(string.Concat(new string[]
				{
					"My weapon ",
					this.weaponData.Name,
					" | ",
					this.weaponStoreData.Skins[this.WeaponSkin].Name,
					" in game Block Strike"
				}));
				stringBuilder.AppendLine("http://bit.ly/blockstrike");
				AndroidNativeFunctions.ShareScreenshot(stringBuilder.ToString(), "Block Strike", Localization.Get("Share", true), path);
			});
		});
	}

	public void MoneySet(bool isgoldprice)
	{
		if (isgoldprice)
		{
			AccountManager.UpdateGold(AccountManager.GetGold() - this.weaponStoreData.Price, null, null);
			AccountManager.SetGold(AccountManager.GetGold() - this.weaponStoreData.Price);
			return;
		}
		AccountManager.UpdateMoney(AccountManager.GetMoney() - this.weaponStoreData.Price, null, null);
		AccountManager.SetMoney(AccountManager.GetMoney() - this.weaponStoreData.Price);
	}

	[Serializable]
	public class WeaponInfo
	{
		public GameObject panel;

		public UILabel nameLabel;

		public UILabel damageLabel;

		public UIProgressBar damageProgressBar;

		public UILabel fireRateLabel;

		public UIProgressBar fireRateProgressBar;

		public UILabel accuracyLabel;

		public UIProgressBar accuracyProgressBar;

		public UILabel ammoLabel;

		public UIProgressBar ammoProgressBar;

		public UILabel maxAmmoLabel;

		public UIProgressBar maxAmmoProgressBar;

		public UILabel mobilityLabel;

		public UIProgressBar mobilityProgressBar;

		[Space(10f)]
		public float maxDamage = 200f;

		public float maxFireRate = 100f;

		public float maxAccuracy = 100f;

		public float maxAmmo = 100f;

		public float maxMaxAmmo = 500f;

		public float maxMobility = 100f;
	}
}
