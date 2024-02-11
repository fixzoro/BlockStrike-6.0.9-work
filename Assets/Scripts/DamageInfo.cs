using System;
using ExitGames.Client.Photon;
using UnityEngine;

public struct DamageInfo
{
    public int damage;

    public Vector3 position;

    public Team team;

    public int weapon;

    public int weaponSkin;

    public int player;

    public bool headshot;

    public bool otherPlayer
	{
		get
		{
			return this.player != -1 && this.player != PhotonNetwork.player.ID;
		}
	}

	public void Set(int d, Vector3 p, Team t, int w, int ws, int pl, bool h)
	{
		this.damage = d;
		this.position = p;
		this.team = t;
		this.weapon = w;
		this.weaponSkin = ws;
		this.player = pl;
		this.headshot = h;
	}

	public static DamageInfo Get()
	{
		return default(DamageInfo);
	}

	public static DamageInfo Get(int damage, Vector3 position, Team team, int weapon, int weaponSkin, int player, bool headshot)
	{
		return new DamageInfo
		{
			damage = damage,
			position = position,
			team = team,
			weapon = weapon,
			weaponSkin = weaponSkin,
			player = player,
			headshot = headshot
		};
	}

	public byte[] Deserialize()
	{
		byte[] array = new byte[25];
		int num = 0;
		byte b = (byte)UnityEngine.Random.Range(1, 255);
		Protocol.Serialize(this.position.x, array, ref num);
		Protocol.Serialize(this.position.y, array, ref num);
		Protocol.Serialize(this.position.z, array, ref num);
		Protocol.Serialize(this.damage, array, ref num);
		array[16] = (byte)this.team;
		array[17] = (byte)this.weapon;
		array[18] = b;
		array[19] = (byte)this.weaponSkin;
		array[20] = (byte)((!this.headshot) ? 0 : 1);
		num += 5;
		Protocol.Serialize(this.player, array, ref num);
		for (int i = 0; i < array.Length; i++)
		{
			if (i != 18)
			{
				array[i] ^= b;
			}
		}
		return array;
	}

	public static DamageInfo Serialize(byte[] bytes)
	{
		DamageInfo result = DamageInfo.Get();
		int num = 0;
		byte b = bytes[18];
		for (int i = 0; i < bytes.Length; i++)
		{
			if (i != 18)
			{
				bytes[i] ^= b;
			}
		}
		Protocol.Deserialize(out result.position.x, bytes, ref num);
		Protocol.Deserialize(out result.position.y, bytes, ref num);
		Protocol.Deserialize(out result.position.z, bytes, ref num);
		Protocol.Deserialize(out result.damage, bytes, ref num);
		result.team = (Team)bytes[16];
		result.weapon = (int)bytes[17];
		result.weaponSkin = (int)bytes[19];
		result.headshot = (bytes[20] == 1);
		num += 5;
		Protocol.Deserialize(out result.player, bytes, ref num);
		return result;
	}
}
