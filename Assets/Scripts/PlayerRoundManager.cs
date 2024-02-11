using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRoundManager
{
    private static GameMode Mode;

    private static CryptoInt2 XP = 0;

    private static CryptoInt2 Money = 0;

    private static CryptoInt2 Kills = 0;

    private static CryptoInt2 Headshot = 0;

    private static CryptoInt2 Deaths = 0;

    private static List<PlayerRoundManager.cFireStat> FireStat = new List<PlayerRoundManager.cFireStat>();

    private static float StartTime;

    private static float FinishTime;

    public static GameMode GetMode()
	{
		return PlayerRoundManager.Mode;
	}

	public static int GetXP()
	{
		return PlayerRoundManager.XP;
	}

	public static int GetMoney()
	{
		return PlayerRoundManager.Money;
	}

	public static int GetKills()
	{
		return PlayerRoundManager.Kills;
	}

	public static int GetHeadshot()
	{
		return PlayerRoundManager.Headshot;
	}

	public static int GetDeaths()
	{
		return PlayerRoundManager.Deaths;
	}

	public static float GetTime()
	{
		return PlayerRoundManager.FinishTime - PlayerRoundManager.StartTime;
	}

	public static void SetMode(GameMode mode)
	{
		PlayerRoundManager.Mode = mode;
		if (PlayerRoundManager.StartTime == 0f)
		{
			PlayerRoundManager.StartTime = Time.time;
		}
	}

	public static void SetXP(int xp)
	{
		PlayerRoundManager.SetXP(xp, false);
	}

	public static void SetXP(int xp, bool simple)
	{
		if (LevelManager.customScene)
		{
			return;
		}
		if (simple)
		{
			PlayerRoundManager.XP += xp;
			return;
		}
		PlayerRoundManager.XP += Mathf.Clamp(Mathf.RoundToInt((float)PhotonNetwork.room.PlayerCount / (float)nValue.int12 * (float)xp), nValue.int0, xp);
	}

	public static void SetMoney(int money)
	{
		PlayerRoundManager.SetMoney(money, false);
	}

	public static void SetMoney(int money, bool simple)
	{
		if (LevelManager.customScene)
		{
			return;
		}
		if (simple)
		{
			PlayerRoundManager.Money += money;
			return;
		}
		PlayerRoundManager.Money += Mathf.Clamp(Mathf.RoundToInt((float)PhotonNetwork.room.PlayerCount / (float)nValue.int12 * (float)money), nValue.int0, money);
	}

	public static void SetKills1()
	{
		if (LevelManager.customScene)
		{
			return;
		}
		PlayerRoundManager.Kills += nValue.int1;
	}

	public static void SetHeadshot1()
	{
		if (LevelManager.customScene)
		{
			return;
		}
		PlayerRoundManager.Headshot += nValue.int1;
	}

	public static void SetDeaths1()
	{
		if (LevelManager.customScene)
		{
			return;
		}
		PlayerRoundManager.Deaths += nValue.int1;
	}

	public static void SetFireStat1(int weapon, int skin)
	{
		if (LevelManager.customScene)
		{
			return;
		}
		bool flag = false;
		for (int i = 0; i < PlayerRoundManager.FireStat.Count; i++)
		{
			if (PlayerRoundManager.FireStat[i].weapon == weapon)
			{
				if (PlayerRoundManager.FireStat[i].skins.Contains(skin))
				{
					for (int j = 0; j < PlayerRoundManager.FireStat[i].skins.Count; j++)
					{
						if (PlayerRoundManager.FireStat[i].skins[j] == skin)
						{
							List<CryptoInt> counts;
							int index;
							CryptoInt value = (counts = PlayerRoundManager.FireStat[i].counts)[index = j];
							counts[index] = ++value;
						}
					}
				}
				else
				{
					PlayerRoundManager.FireStat[i].skins.Add(skin);
					PlayerRoundManager.FireStat[i].counts.Add(nValue.int1);
				}
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			PlayerRoundManager.cFireStat cFireStat = new PlayerRoundManager.cFireStat();
			cFireStat.weapon = weapon;
			cFireStat.skins.Add(skin);
			cFireStat.counts.Add(nValue.int1);
			PlayerRoundManager.FireStat.Add(cFireStat);
		}
	}

	public static bool hasValue
	{
		get
		{
			return PlayerRoundManager.GetXP() != 0 || PlayerRoundManager.GetMoney() != 0 || PlayerRoundManager.GetKills() != 0 || PlayerRoundManager.GetHeadshot() != 0 || PlayerRoundManager.GetDeaths() != 0;
		}
	}

	public static void Show()
	{
		if (!PhotonNetwork.offlineMode)
		{
			PlayerRoundManager.FinishTime = Time.time;
			TimerManager.In(0.5f, false, delegate()
			{
				PlayerRoundManager.ShowPopup();
			});
		}
	}

	private static void ShowPopup()
	{
		if (PlayerRoundManager.hasValue)
		{
			mPlayerRoundManager.Show();
			PlayerRoundManager.SetData();
			return;
		}
		PlayerRoundManager.Clear();
	}

	private static void SetData()
	{
		AccountManager.SetXP1(PlayerRoundManager.GetXP());
		AccountManager.SetKills1(PlayerRoundManager.GetKills());
		AccountManager.SetDeaths1(PlayerRoundManager.GetDeaths());
		AccountManager.SetHeadshot1(PlayerRoundManager.GetHeadshot());
		AccountManager.SetMoney1(PlayerRoundManager.GetMoney());
        AccountManager.SetTime1((long)PlayerRoundManager.GetTime());
        TimerManager.In(0.03f, delegate()
		{
			AccountManager.UpdatePlayerRoundData(AccountManager.GetXP(), AccountManager.GetKills(), AccountManager.GetMoney(), AccountManager.GetDeaths(), AccountManager.GetHeadshot(), AccountManager.GetTime(), AccountManager.GetLevel(), null, new Action<string>(PlayerRoundManager.Failed));
		});
	}

	private static void Failed(string error)
	{
		TimerManager.In(0.1f, delegate()
		{
			mPopUp.ShowPopup(Localization.Get("Error", true) + ": " + error, Localization.Get("Error", true), Localization.Get("Retry", true), delegate()
			{
				mPopUp.HideAll();
				PlayerRoundManager.SetData();
			});
		});
	}

	public static void Clear()
	{
		PlayerRoundManager.XP = nValue.int0;
		PlayerRoundManager.Money = nValue.int0;
		PlayerRoundManager.Kills = nValue.int0;
		PlayerRoundManager.Headshot = nValue.int0;
		PlayerRoundManager.Deaths = nValue.int0;
		PlayerRoundManager.StartTime = (float)nValue.int0;
		PlayerRoundManager.FinishTime = (float)nValue.int0;
		PlayerRoundManager.FireStat.Clear();
	}

	public class cFireStat
	{
		public CryptoInt weapon;

		public List<CryptoInt> skins = new List<CryptoInt>();

		public List<CryptoInt> counts = new List<CryptoInt>();
	}
}
