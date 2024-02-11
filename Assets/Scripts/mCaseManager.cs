using System;
using System.Collections.Generic;
using System.Text;
using Photon;
using UnityEngine;

public class mCaseManager : Photon.MonoBehaviour
{
    [Header("Cases")]
    public mCaseManager.Case[] SkinCases;

    public mCaseManager.Case[] StickerCases;

    private bool isSkinCases = true;

    [Header("Case Wheel")]
    public bool caseRotate;

    private string selectCaseName;

    private float startRotate;

    public AnimationCurve caseWheelCurve;

    public float duration = 8f;

    public float caseWheelLerp = 1f;

    public Vector2 finishInterval;

    private float finishPosition;

    public AudioSource soundSource;

    public float soundInterval;

    private float lastsoundInterval;

    public mCaseItem[] caseItems;

    public mCaseItem finishItem;

    public Transform caseItemsRoot;

    [Header("Finish Panel")]
    public UISprite finishBackground;

    public GameObject finishPanel;

    public UILabel finishLabel;

    public UILabel finishQualityLabel;

    public UIGrid finishQualityTable;

    public UISprite finishLineSprite;

    public GameObject finishPanelGold;

    public GameObject finishAlreadyAvailable;

    public UITexture finishAlreadyAvailableTexture;

    public UILabel finishAlreadyAvailableLabel;

    public UITexture finishFireStatTexture;

    public GameObject finishSecretWeapon;

    private bool isFinishWeapon;

    private WeaponData finishWeapon;

    private WeaponSkinData finishWeaponSkin;

    private StickerData finishSticker;

    private bool finishWeaponFireStat;

    private bool finishWeaponAlready;

    private List<KeyValuePair<int, int>> normalSkins = new List<KeyValuePair<int, int>>();

    private List<KeyValuePair<int, int>> baseSkins = new List<KeyValuePair<int, int>>();

    private List<KeyValuePair<int, int>> professionalSkins = new List<KeyValuePair<int, int>>();

    private List<KeyValuePair<int, int>> legendarySkins = new List<KeyValuePair<int, int>>();

    private List<KeyValuePair<int, int>> secretWeaponSkins = new List<KeyValuePair<int, int>>();

    private List<int> baseStickers = new List<int>();

    private List<int> professionalStickers = new List<int>();

    private List<int> legendaryStickers = new List<int>();

    [Header("Others")]
    public GameObject inAppPanel;

    public GameObject shareButton;

    public GameObject backButton;

    private bool isShow;

    private void Start()
	{
		UIEventListener.Get(this.finishBackground.cachedGameObject).onDrag = new UIEventListener.VectorDelegate(this.RotateWeapon);
	}

	private void OnEnable()
	{
		PhotonNetwork.onDisconnectedFromPhoton = (PhotonNetwork.VoidDelegate)Delegate.Combine(PhotonNetwork.onDisconnectedFromPhoton, new PhotonNetwork.VoidDelegate(this.OnDisconnectedFromPhoton));
	}

	private void OnDisable()
	{
		PhotonNetwork.onDisconnectedFromPhoton = (PhotonNetwork.VoidDelegate)Delegate.Remove(PhotonNetwork.onDisconnectedFromPhoton, new PhotonNetwork.VoidDelegate(this.OnDisconnectedFromPhoton));
	}

	private void RotateWeapon(GameObject go, Vector2 drag)
	{
		mWeaponCamera.Rotate(drag, true);
	}

	private void OnDisconnectedFromPhoton()
	{
		if (this.isShow)
		{
			this.Close();
		}
	}

	private void Update()
	{
		this.UpdateCaseWheel();
	}

	public void Show(bool skin)
	{
		this.isShow = true;
		this.isSkinCases = skin;
		if (this.isSkinCases)
		{
			for (int i = 0; i < this.SkinCases.Length; i++)
			{
				this.SkinCases[i].NameLabel.text = Localization.Get(this.SkinCases[i].Name, true);
				this.SkinCases[i].InfoLabel.text = this.GetCaseInfo(this.SkinCases[i]);
			}
			this.UpdateSkinsList();
			return;
		}
		for (int j = 0; j < this.StickerCases.Length; j++)
		{
			this.StickerCases[j].NameLabel.text = Localization.Get(this.StickerCases[j].Name, true);
			this.StickerCases[j].InfoLabel.text = this.GetCaseInfo(this.StickerCases[j]);
		}
		this.UpdateStickerList();
	}

	public void Close()
	{
		this.isShow = false;
		this.caseRotate = false;
		mWeaponCamera.SetCamera(false);
		mWeaponCamera.Close();
		this.finishBackground.cachedGameObject.SetActive(false);
		mPanelManager.Show("Cases", true);
	}

	private string GetCaseInfo(mCaseManager.Case selectCase)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(Localization.Get("ChanceOfDrop", true) + ":");
		stringBuilder.AppendLine(string.Empty);
		if (selectCase.Money)
		{
			stringBuilder.AppendLine("BS Coins -  50%");
		}
		if (selectCase.Normal != 0)
		{
			stringBuilder.AppendLine(string.Concat(new object[]
			{
				Localization.Get("Normal quality", true),
				" - ",
				selectCase.Normal,
				"%"
			}));
		}
		if (selectCase.Base != 0)
		{
			stringBuilder.AppendLine(string.Concat(new object[]
			{
				"[2098ff]",
				Localization.Get("Basic quality", true),
				"[-] - ",
				selectCase.Base,
				"%"
			}));
		}
		if (selectCase.Professional != 0)
		{
			stringBuilder.AppendLine(string.Concat(new object[]
			{
				"[ff3838]",
				Localization.Get("Professional quality", true),
				"[-] - ",
				selectCase.Professional,
				"%"
			}));
		}
		if (selectCase.Legendary != 0)
		{
			stringBuilder.AppendLine(string.Concat(new object[]
			{
				"[ff2093]",
				Localization.Get("Legendary quality", true),
				"[-] - ",
				selectCase.Legendary,
				"%"
			}));
		}
		if (selectCase.SecretWeapon != 0)
		{
			stringBuilder.AppendLine(string.Concat(new object[]
			{
				"[757575]",
				Localization.Get("Secret Weapon", true),
				"[-] - ",
				selectCase.SecretWeapon,
				"%"
			}));
		}
		return stringBuilder.ToString();
	}

	private mCaseManager.Case GetCase(string caseName)
	{
		if (this.isSkinCases)
		{
			for (int i = 0; i < this.SkinCases.Length; i++)
			{
				if (this.SkinCases[i].Name == caseName)
				{
					return this.SkinCases[i];
				}
			}
		}
		else
		{
			for (int j = 0; j < this.StickerCases.Length; j++)
			{
				if (this.StickerCases[j].Name == caseName)
				{
					return this.StickerCases[j];
				}
			}
		}
		return null;
	}

	private int GetCaseIndex(string caseName)
	{
		if (this.isSkinCases)
		{
			for (int i = 0; i < this.SkinCases.Length; i++)
			{
				if (this.SkinCases[i].Name == caseName)
				{
					return i;
				}
			}
		}
		else
		{
			for (int j = 0; j < this.StickerCases.Length; j++)
			{
				if (this.StickerCases[j].Name == caseName)
				{
					return j;
				}
			}
		}
		return -1;
	}

	private void UpdateSkinsList()
	{
		if (this.normalSkins.Count != 0)
		{
			return;
		}
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			if (!GameSettings.instance.Weapons[i].Lock && !GameSettings.instance.Weapons[i].Secret)
			{
				for (int j = 0; j < GameSettings.instance.WeaponsStore[i].Skins.Count; j++)
				{
					if (GameSettings.instance.WeaponsStore[i].Skins[j].Price == 0 && string.IsNullOrEmpty(GameSettings.instance.WeaponsStore[i].Skins[j].Temporary))
					{
						switch (GameSettings.instance.WeaponsStore[i].Skins[j].Quality)
						{
						case WeaponSkinQuality.Normal:
							this.normalSkins.Add(new KeyValuePair<int, int>(GameSettings.instance.Weapons[i].ID, GameSettings.instance.WeaponsStore[i].Skins[j].ID));
							break;
						case WeaponSkinQuality.Basic:
							this.baseSkins.Add(new KeyValuePair<int, int>(GameSettings.instance.Weapons[i].ID, GameSettings.instance.WeaponsStore[i].Skins[j].ID));
							break;
						case WeaponSkinQuality.Professional:
							this.professionalSkins.Add(new KeyValuePair<int, int>(GameSettings.instance.Weapons[i].ID, GameSettings.instance.WeaponsStore[i].Skins[j].ID));
							break;
						case WeaponSkinQuality.Legendary:
                            if(GameSettings.instance.WeaponsStore[i].Skins[j].Name == "Snowman")
                            {
                                break;
                            }
                            if (GameSettings.instance.WeaponsStore[i].Skins[j].Name == "Halloween")
                            {
                                break;
                            }
                            this.legendarySkins.Add(new KeyValuePair<int, int>(GameSettings.instance.Weapons[i].ID, GameSettings.instance.WeaponsStore[i].Skins[j].ID));
							break;
						}
					}
				}
			}
		}
		for (int k = 0; k < GameSettings.instance.Weapons.Count; k++)
		{
			if (!GameSettings.instance.Weapons[k].Lock && GameSettings.instance.Weapons[k].Secret)
			{
				for (int l = 0; l < GameSettings.instance.WeaponsStore[k].Skins.Count; l++)
				{
					if (GameSettings.instance.WeaponsStore[k].Skins[l].Quality != WeaponSkinQuality.Default && GameSettings.instance.WeaponsStore[k].Skins[l].Price == 0)
					{
						this.secretWeaponSkins.Add(new KeyValuePair<int, int>(GameSettings.instance.Weapons[k].ID, GameSettings.instance.WeaponsStore[k].Skins[l].ID));
					}
				}
			}
		}
	}

	private void UpdateStickerList()
	{
		if (this.baseStickers.Count != 0)
		{
			return;
		}
		for (int i = 0; i < GameSettings.instance.Stickers.Count; i++)
		{
			switch (GameSettings.instance.Stickers[i].Quality)
			{
			case StickerQuality.Basic:
				this.baseStickers.Add(GameSettings.instance.Stickers[i].ID);
				break;
			case StickerQuality.Professional:
				this.professionalStickers.Add(GameSettings.instance.Stickers[i].ID);
				break;
			case StickerQuality.Legendary:
				this.legendaryStickers.Add(GameSettings.instance.Stickers[i].ID);
				break;
			}
		}
	}

	public void OnSpeed()
	{
		this.duration = 1f;
		this.caseWheelLerp = 10f;
	}

    public void StartCaseWheel(string caseName)
    {
        if (!AccountManager.isConnect)
        {
            UIToast.Show(Localization.Get("Connection account", true));
            return;
        }
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            UIToast.Show("Not Internet");
            return;
        }
        mCaseManager.Case @case = this.GetCase(caseName);
        this.duration = 8f;
        this.caseWheelLerp = 1f;
        this.selectCaseName = caseName;
        mCaseManager.Case case2 = this.GetCase(caseName);
        if (case2.Price != 0)
        {
            int num = (case2.Currency != GameCurrency.Gold) ? AccountManager.GetMoney() : AccountManager.GetGold();
            if (case2.Price > num)
            {
                this.inAppPanel.SetActive(true);
                UIToast.Show(Localization.Get("Not enough money", true));
                return;
            }
        }
        this.finishWeaponFireStat = false;
        this.finishWeaponAlready = false;
        mPopUp.SetActiveWaitPanel(true, Localization.Get("Please wait", true));
        this.UpdateOtherscaseItems();
        if (@case.Money)
        {
            this.SetFinishMoneyData(delegate()
            {
                mPopUp.SetActiveWaitPanel(false);
                if (case2.Currency == GameCurrency.Gold)
                {
                    AccountData data = AccountManager.instance.Data;
                    data.Gold -= case2.Price;
                    AccountManager.UpdateGold(data.Gold, null, null);
                }
                else
                {
                    AccountData data2 = AccountManager.instance.Data;
                    data2.Money -= case2.Price;
                    AccountManager.UpdateMoney(data2.Money, null, null);
                }
                mPanelManager.Show("CaseWheel", false);
                this.caseItemsRoot.localPosition = new Vector3(0f, this.caseItemsRoot.localPosition.y, 0f);
                this.startRotate = Time.time;
                this.caseRotate = true;
                this.lastsoundInterval = this.soundInterval / 2f;
                this.finishPosition = (float)((int)UnityEngine.Random.Range(this.finishInterval.x, this.finishInterval.y));
            }, delegate (string error)
            {
                mPopUp.SetActiveWaitPanel(false);
                UIToast.Show(error);
            });
            return;
        }
        this.SetFinishWeaponData(@case, delegate(string complete) 
        {
            mPopUp.SetActiveWaitPanel(false);
            if (case2.Currency == GameCurrency.Gold)
            {
                AccountData data = AccountManager.instance.Data;
                data.Gold -= case2.Price;
                AccountManager.UpdateGold(data.Gold, null, null);
            }
            else
            {
                AccountData data2 = AccountManager.instance.Data;
                data2.Money -= case2.Price;
                AccountManager.UpdateMoney(data2.Money, null, null);
            }
            mPanelManager.Show("CaseWheel", false);
            this.caseItemsRoot.localPosition = new Vector3(0f, this.caseItemsRoot.localPosition.y, 0f);
            this.startRotate = Time.time;
            this.caseRotate = true;
            this.lastsoundInterval = this.soundInterval / 2f;
            this.finishPosition = (float)((int)UnityEngine.Random.Range(this.finishInterval.x, this.finishInterval.y));
        }, delegate(string error) 
        {
            mPopUp.SetActiveWaitPanel(false);
            UIToast.Show(error);
        });
	}

    private void SetFinishWeaponData(mCaseManager.Case selectCase, Action<string> complete, Action<string> failed)
    {
        int randomQualty = this.GetRandomQualty(selectCase);
        if (this.isSkinCases)
        {
            this.isFinishWeapon = true;
            bool flag = selectCase.SecretWeapon >= UnityEngine.Random.Range(nValue.int1, nValue.int100);
            KeyValuePair<int, int> keyValuePair = default(KeyValuePair<int, int>);
            if (flag)
            {
                keyValuePair = this.secretWeaponSkins[UnityEngine.Random.Range(nValue.int0, this.secretWeaponSkins.Count)];
            }
            else
            {
                switch (randomQualty)
                {
                    case 1:
                        keyValuePair = this.normalSkins[UnityEngine.Random.Range(nValue.int0, this.normalSkins.Count)];
                        break;
                    case 2:
                        keyValuePair = this.baseSkins[UnityEngine.Random.Range(nValue.int0, this.baseSkins.Count)];
                        break;
                    case 3:
                        keyValuePair = this.professionalSkins[UnityEngine.Random.Range(nValue.int0, this.professionalSkins.Count)];
                        break;
                    case 4:
                        keyValuePair = this.legendarySkins[UnityEngine.Random.Range(nValue.int0, this.legendarySkins.Count)];
                        break;
                }
            }
            this.finishWeapon = WeaponManager.GetWeaponData(keyValuePair.Key);
            this.finishWeaponSkin = WeaponManager.GetWeaponSkin(keyValuePair.Key, keyValuePair.Value);
            if (this.finishWeaponSkin.Quality != WeaponSkinQuality.Default && this.finishWeaponSkin.Quality != WeaponSkinQuality.Normal && this.finishWeapon.Type != WeaponType.Knife && this.GetCase(this.selectCaseName).FireStat >= UnityEngine.Random.Range(1, 100))
            {
                this.finishWeaponFireStat = true;
            }
            else
            {
                this.finishWeaponFireStat = false;
            }
            if (this.finishWeaponFireStat)
            {
                this.finishWeaponFireStat = true;
            }
            else
            {
                this.finishWeaponAlready = AccountManager.GetWeaponSkin(this.finishWeapon.ID, this.finishWeaponSkin.ID);
            }
            AccountManager.SetWeaponSkin(this.finishWeapon.ID, this.finishWeaponSkin.ID);
            AccountManager.UpdateWeaponSkins(this.finishWeapon.Secret, this.finishWeapon.ID, complete, failed);
            this.SetCaseWeaponItem(this.finishItem, keyValuePair.Key, keyValuePair.Value);
            return;
        }
        int index = -1;
        switch (randomQualty)
        {
            case 1:
                index = this.baseStickers[UnityEngine.Random.Range(nValue.int0, this.baseStickers.Count)];
                break;
            case 2:
                index = this.professionalStickers[UnityEngine.Random.Range(nValue.int0, this.professionalStickers.Count)];
                break;
            case 3:
                index = this.legendaryStickers[UnityEngine.Random.Range(nValue.int0, this.legendaryStickers.Count)];
                break;
        }
        this.finishSticker = GameSettings.instance.Stickers[index];
        AccountManager.SetStickers(this.finishSticker.ID);
        AccountManager.UpdateStickersFirebase(complete, failed);
        this.SetCaseStickerItem(this.finishItem, this.finishSticker);
    }

    private void SetFinishMoneyData(Action complete, Action<string> failed)
    {
        this.isFinishWeapon = false;
        AccountManager.SetGold1(2);
        AccountManager.UpdateGold(AccountManager.GetGold(), complete, failed);
        this.SetCaseMoneyItem(this.finishItem);
    }

    private void UpdateOtherscaseItems()
	{
		mCaseManager.Case @case = this.GetCase(this.selectCaseName);
		bool flag = @case.Money;
		int max = GameSettings.instance.Stickers.Count - 1;
		for (int i = 0; i < this.caseItems.Length; i++)
		{
			flag = @case.Money;
			if (flag && UnityEngine.Random.value < 0.4f)
			{
				flag = false;
			}
			if (flag)
			{
				this.SetCaseMoneyItem(this.caseItems[i]);
			}
			else if (this.isSkinCases)
			{
				int num = UnityEngine.Random.Range(0, 100);
				KeyValuePair<int, int> keyValuePair = default(KeyValuePair<int, int>);
				if (num < 25)
				{
					if (@case.Normal != 0)
					{
						keyValuePair = this.normalSkins[UnityEngine.Random.Range(0, this.normalSkins.Count)];
					}
					else
					{
						keyValuePair = this.baseSkins[UnityEngine.Random.Range(0, this.baseSkins.Count)];
					}
				}
				else if (num < 50)
				{
					keyValuePair = this.baseSkins[UnityEngine.Random.Range(0, this.baseSkins.Count)];
				}
				else if (num < 75)
				{
					keyValuePair = this.professionalSkins[UnityEngine.Random.Range(0, this.professionalSkins.Count)];
				}
				else if (i > 10 && @case.SecretWeapon != 0 && num > 98)
				{
					keyValuePair = this.secretWeaponSkins[UnityEngine.Random.Range(0, this.secretWeaponSkins.Count)];
				}
				else
				{
					keyValuePair = this.legendarySkins[UnityEngine.Random.Range(0, this.legendarySkins.Count)];
				}
				this.SetCaseWeaponItem(this.caseItems[i], keyValuePair.Key, keyValuePair.Value);
			}
			else
			{
				int index = UnityEngine.Random.Range(0, max);
				this.SetCaseStickerItem(this.caseItems[i], GameSettings.instance.Stickers[index]);
			}
		}
	}

	private void UpdateCaseWheel()
	{
		if (!this.caseRotate)
		{
			return;
		}
		float time = (Time.time - this.startRotate) / this.duration;
		float num = this.caseWheelCurve.Evaluate(time);
		float x = num * this.finishPosition;
		float num2 = 1f - num + this.caseWheelLerp;
		this.caseItemsRoot.localPosition = Vector3.Lerp(this.caseItemsRoot.localPosition, new Vector3(x, this.caseItemsRoot.localPosition.y, 0f), Time.deltaTime * num2);
		if (-this.caseItemsRoot.localPosition.x > this.lastsoundInterval && this.duration != 1f)
		{
			this.soundSource.PlayOneShot(this.soundSource.clip);
			this.lastsoundInterval += this.soundInterval;
		}
		if (this.caseItemsRoot.localPosition.x <= this.finishPosition + 2f)
		{
			this.caseRotate = false;
			this.StartFinishPanel();
		}
	}

	private void SetCaseWeaponItem(mCaseItem caseItem, int weaponID, int ID)
	{
		WeaponSkinData weaponSkin = WeaponManager.GetWeaponSkin(weaponID, ID);
		caseItem.weaponTexture.atlas = GameSettings.instance.WeaponIconAtlas;
		caseItem.weaponTexture.cachedGameObject.SetActive(true);
		caseItem.weaponShadow.atlas = GameSettings.instance.WeaponIconAtlas;
		caseItem.weaponShadow.cachedGameObject.SetActive(true);
		caseItem.goldTexture.cachedGameObject.SetActive(false);
		caseItem.weaponTexture.spriteName = weaponID + "-" + ID;
		caseItem.weaponShadow.spriteName = weaponID + "-" + ID;
		caseItem.weaponTexture.width = (int)GameSettings.instance.WeaponsCaseSize[weaponID - 1].x;
		caseItem.weaponTexture.height = (int)GameSettings.instance.WeaponsCaseSize[weaponID - 1].y;
		caseItem.weaponShadow.width = (int)GameSettings.instance.WeaponsCaseSize[weaponID - 1].x;
		caseItem.weaponShadow.height = (int)GameSettings.instance.WeaponsCaseSize[weaponID - 1].y;
		caseItem.nameLabel.text = weaponSkin.Name;
		caseItem.qualityLabel.text = Localization.Get(weaponSkin.Quality.ToString() + " quality", true);
		caseItem.qualityBackground.color = this.GetSkinQualityColor(weaponSkin.Quality);
	}

	private void SetCaseMoneyItem(mCaseItem caseItem)
	{
		caseItem.goldTexture.cachedGameObject.SetActive(true);
		caseItem.weaponTexture.cachedGameObject.SetActive(false);
		caseItem.weaponShadow.cachedGameObject.SetActive(false);
		caseItem.goldTexture.width = 80;
		caseItem.goldTexture.height = 80;
		caseItem.goldTexture.uvRect = new Rect(0f, 0f, 1f, 1f);
		caseItem.nameLabel.text = "BS GOLD";
		caseItem.qualityLabel.text = string.Empty;
		caseItem.qualityBackground.color = new Color(0.83f, 0.584f, 0f);
	}

	private void SetCaseStickerItem(mCaseItem caseItem, StickerData sticker)
	{
		caseItem.weaponTexture.atlas = GameSettings.instance.StickersAtlas;
		caseItem.weaponTexture.cachedGameObject.SetActive(true);
		caseItem.weaponShadow.cachedGameObject.SetActive(true);
		caseItem.goldTexture.cachedGameObject.SetActive(false);
		caseItem.weaponTexture.spriteName = sticker.ID.ToString();
		caseItem.weaponShadow.spriteName = sticker.ID.ToString();
		caseItem.weaponTexture.width = 64;
		caseItem.weaponTexture.height = 64;
		caseItem.weaponShadow.width = 64;
		caseItem.weaponShadow.height = 64;
		caseItem.nameLabel.text = sticker.Name;
		caseItem.qualityLabel.text = Localization.Get(sticker.Quality.ToString() + " quality", true);
		caseItem.qualityBackground.color = this.GetStickerQualityColor(sticker.Quality);
	}

	private Color GetSkinQualityColor(WeaponSkinQuality quality)
	{
		switch (quality)
		{
		case WeaponSkinQuality.Default:
		case WeaponSkinQuality.Normal:
			return new Color(0.63f, 0.63f, 0.63f, 1f);
		case WeaponSkinQuality.Basic:
			return new Color(0.07f, 0.65f, 0.87f, 1f);
		case WeaponSkinQuality.Professional:
			return new Color(0.9f, 0f, 0f, 1f);
		case WeaponSkinQuality.Legendary:
			return new Color(0.87f, 0f, 0.38f, 1f);
		default:
			return new Color(0.63f, 0.63f, 0.63f, 1f);
		}
	}

	private Color GetStickerQualityColor(StickerQuality quality)
	{
		switch (quality)
		{
		case StickerQuality.Basic:
			return new Color(0.07f, 0.65f, 0.87f, 1f);
		case StickerQuality.Professional:
			return new Color(0.9f, 0f, 0f, 1f);
		case StickerQuality.Legendary:
			return new Color(0.87f, 0f, 0.38f, 1f);
		default:
			return new Color(0.63f, 0.63f, 0.63f, 1f);
		}
	}

	public void StartFinishPanel()
	{
		UIFade.Fade(0f, 1f, 0.3f);
		TimerManager.In(0.5f, delegate()
		{
			mPanelManager.Show("CaseFinish", false);
			UIFade.Fade(1f, 0f, 0.3f);
			if (this.isSkinCases)
			{
				if (this.isFinishWeapon)
				{
					Color skinQualityColor = this.GetSkinQualityColor(this.finishWeaponSkin.Quality);
					if (this.finishWeaponSkin.Quality != WeaponSkinQuality.Default && this.finishWeaponSkin.Quality != WeaponSkinQuality.Normal && this.finishWeapon.Type != WeaponType.Knife)
					{
                        if (this.finishWeaponFireStat)
                        {
                            AccountManager.SetFireStat(this.finishWeapon.ID, this.finishWeaponSkin.ID);
                            AccountManager.SetFireStatFireBase(true, this.finishWeapon.ID, this.finishWeaponSkin.ID, 0);
                        }
                        this.finishFireStatTexture.cachedGameObject.SetActive(this.finishWeaponFireStat);
                    }
					else
					{
						this.finishFireStatTexture.cachedGameObject.SetActive(false);
					}
					this.finishSecretWeapon.gameObject.SetActive(this.finishWeapon.Secret);
					this.finishQualityTable.Reposition();
					this.finishBackground.color = skinQualityColor;
					this.finishBackground.alpha = 0.6f;
					this.finishBackground.cachedGameObject.SetActive(true);
					this.finishLabel.text = this.finishWeapon.Name + " | " + this.finishWeaponSkin.Name;
					this.finishQualityLabel.text = Localization.Get(this.finishWeaponSkin.Quality.ToString() + " quality", true);
					this.finishLineSprite.color = skinQualityColor;
					this.finishPanel.SetActive(true);
					this.finishPanelGold.SetActive(false);
					mWeaponCamera.SetCamera(true);
					mWeaponCamera.SetViewportRect(new Rect(0f, 0f, 1f, 1f), 0.5f);
					mWeaponCamera.Show(this.finishWeapon.Name);
					mWeaponCamera.SetSkin(this.finishWeapon.ID, this.finishWeaponSkin.ID);
					this.finishAlreadyAvailable.SetActive(this.finishWeaponAlready);
                    if(finishWeaponAlready)
                    {
                        AccountData data = AccountManager.instance.Data;
                        data.Gold += 2;
                        AccountManager.UpdateGold(data.Gold, null, null);
                    }
				}
				else
				{
					Color skinQualityColor2 = this.GetSkinQualityColor(WeaponSkinQuality.Normal);
					this.finishBackground.color = skinQualityColor2;
					this.finishBackground.alpha = 0.6f;
					this.finishBackground.cachedGameObject.SetActive(true);
					this.finishPanel.SetActive(false);
					this.finishPanelGold.SetActive(true);
					this.finishLineSprite.color = skinQualityColor2;
					mWeaponCamera.SetCamera(true);
					mWeaponCamera.SetViewportRect(new Rect(0f, 0f, 1f, 1f), 0.5f);
					mWeaponCamera.Show("BS Gold");
					this.finishAlreadyAvailable.SetActive(false);
					this.finishFireStatTexture.cachedGameObject.SetActive(false);
					this.finishSecretWeapon.gameObject.SetActive(false);
				}
			}
			else
			{
				Color stickerQualityColor = this.GetStickerQualityColor(this.finishSticker.Quality);
				this.finishFireStatTexture.cachedGameObject.SetActive(false);
				this.finishQualityTable.Reposition();
				this.finishSecretWeapon.gameObject.SetActive(false);
				this.finishBackground.color = stickerQualityColor;
				this.finishBackground.alpha = 0.6f;
				this.finishBackground.cachedGameObject.SetActive(true);
				this.finishLabel.text = this.finishSticker.Name;
				this.finishQualityLabel.text = Localization.Get(this.finishSticker.Quality.ToString() + " quality", true);
				this.finishLineSprite.color = stickerQualityColor;
				this.finishPanel.SetActive(true);
				this.finishPanelGold.SetActive(false);
				mWeaponCamera.SetCamera(true);
				mWeaponCamera.SetViewportRect(new Rect(0f, 0f, 1f, 1f), 0.5f);
				mWeaponCamera.Show("Sticker");
				mWeaponCamera.SetSkin(0, this.finishSticker.ID);
				this.finishAlreadyAvailable.SetActive(false);
			}
			EventManager.Dispatch("AccountUpdate");
		});
	}

	public void StartRewardedCase()
	{
		if (!AccountManager.isConnect)
		{
			UIToast.Show(Localization.Get("Connection account", true));
			return;
		}
		mPopUp.SetActiveWaitPanel(true, Localization.Get("Loading", true) + "...");
		TimerManager.In(0.5f, delegate()
		{
			this.RewardedVideoComplete(1, "Free");
		});
	}

	private void RewardedVideoComplete(int amount, string name)
	{
		TimerManager.In(0.3f, delegate()
		{
			mPopUp.SetActiveWaitPanel(false);
			this.StartCaseWheel("Free");
		});
	}

	private void RewardedVideoAborted()
	{
		TimerManager.In(0.3f, delegate()
		{
			UIToast.Show(Localization.Get("Cancel", true));
			mPopUp.SetActiveWaitPanel(false);
		});
	}

	private void RewardedVideoFailed()
	{
		TimerManager.In(0.3f, delegate()
		{
			UIToast.Show(Localization.Get("Video not available", true));
			mPopUp.SetActiveWaitPanel(false);
		});
	}

	public void ShareScreenshot()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		this.backButton.SetActive(false);
		this.shareButton.SetActive(false);
		TimerManager.In(0.5f, delegate()
		{
			AndroidNativeFunctions.TakeScreenshot(string.Concat(new string[]
			{
				this.finishWeapon.Name,
				" | ",
				this.finishWeaponSkin.Name,
				"_",
				DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")
			}), delegate(string path)
			{
				this.backButton.SetActive(true);
				this.shareButton.SetActive(true);
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine("#BlockStrike #BS");
				stringBuilder.AppendLine(string.Concat(new string[]
				{
					"I just got ",
					this.finishWeapon.Name,
					" | ",
					this.finishWeaponSkin.Name,
					" in game Block Strike"
				}));
				stringBuilder.AppendLine("http://bit.ly/blockstrike");
				AndroidNativeFunctions.ShareScreenshot(stringBuilder.ToString(), "Block Strike", Localization.Get("Share", true), path);
			});
		});
	}

    private int GetRandomQualty(mCaseManager.Case selectCase)
    {
        int num = UnityEngine.Random.Range(0, 100);
        if (this.isSkinCases)
        {
            int num2 = (!selectCase.Money) ? 1 : 2;
            if ((selectCase.Normal + selectCase.Base + selectCase.Professional) * num2 < num)
            {
                return 4;
            }
            if ((selectCase.Normal + selectCase.Base) * num2 < num)
            {
                return 3;
            }
            if (selectCase.Normal * num2 < num)
            {
                return 2;
            }
            return 1;
        }
        else
        {
            if (selectCase.Base + selectCase.Professional < num)
            {
                return 3;
            }
            if (selectCase.Base < num)
            {
                return 2;
            }
            return 1;
        }
    }

	[Serializable]
	public class Case
	{
		public string Name;

		public GameCurrency Currency;

		public CryptoInt Price;

		public UILabel NameLabel;

		public UILabel InfoLabel;

		public CryptoBool Money;

		public CryptoInt Normal;

		public CryptoInt Base;

		public CryptoInt Professional;

		public CryptoInt Legendary;

		public CryptoInt FireStat;

        public CryptoInt SecretWeapon;
	}
}
