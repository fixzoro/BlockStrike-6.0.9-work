using UnityEngine;

public class mServerSettings : MonoBehaviour
{
    public GameObject settingsButton;

    [Header("Only")]
    public GameObject onlyPanel;

    public UIPopupList onlyWeaponPopupList;

    private static mServerSettings instance;

    private void Start()
	{
		mServerSettings.instance = this;
	}

	public static void Check(GameMode mode, string map)
	{
		if (mode != GameMode.Only)
		{
			if (mode != GameMode.MiniGames)
			{
				mServerSettings.instance.gameObject.SendMessage("SetDefaultMaxPlayers");
				mServerSettings.instance.settingsButton.SetActive(false);
			}
			else if (map == "50Traps")
			{
				mServerSettings.instance.gameObject.SendMessage("SetMaxPlayers", new int[]
				{
					4,
					8,
					16,
					24,
					32
				});
			}
			else
			{
				mServerSettings.instance.gameObject.SendMessage("SetDefaultMaxPlayers");
			}
		}
		else
		{
			mServerSettings.instance.settingsButton.SetActive(true);
			mServerSettings.instance.gameObject.SendMessage("SetDefaultMaxPlayers");
		}
	}

	public void Open()
	{
		GameMode mode = mCreateServer.mode;
		if (mode == GameMode.Only)
		{
			this.StartOnlyMode();
		}
	}

	public void Close()
	{
		GameMode mode = mCreateServer.mode;
		if (mode == GameMode.Only)
		{
			this.onlyPanel.gameObject.SetActive(false);
		}
	}

	private void StartOnlyMode()
	{
		this.onlyPanel.gameObject.SetActive(true);
		this.onlyWeaponPopupList.Clear();
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			if (!GameSettings.instance.Weapons[i].Lock && !GameSettings.instance.Weapons[i].Secret)
			{
				this.onlyWeaponPopupList.AddItem(GameSettings.instance.Weapons[i].Name);
			}
		}
		this.onlyWeaponPopupList.value = this.onlyWeaponPopupList.items[0];
	}

	public static int GetOnlyWeapon()
	{
		int num = WeaponManager.GetWeaponID(mServerSettings.instance.onlyWeaponPopupList.value);
		if (num <= 0)
		{
			num = 1;
		}
		return num;
	}
}
