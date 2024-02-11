using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameSettings : ScriptableObject
{
    public CryptoString[] Keys;

    public string PhotonID;

    public string InAppPublicKey;

    public GameObject PlayerController;

    public GameObject PlayerSkin;

    public GameObject PlayerRagdoll;

    public GameObject RoundManager;

    public GameObject UIRoot;

    public UIAtlas PlayerAtlasBlue;

    public UIAtlas PlayerAtlasRed;

    public UIAtlas WeaponAtlas;

    public UIAtlas WeaponIconAtlas;

    public UIAtlas StickersAtlas;

    public List<WeaponData> Weapons = new List<WeaponData>();

    public List<WeaponStoreData> WeaponsStore = new List<WeaponStoreData>();

    public List<StickerData> Stickers = new List<StickerData>();

    public List<Vector2> WeaponsCaseSize = new List<Vector2>();

    public List<PlayerStoreSkinData> PlayerStoreHead = new List<PlayerStoreSkinData>();

    public List<PlayerStoreSkinData> PlayerStoreBody = new List<PlayerStoreSkinData>();

    public List<PlayerStoreSkinData> PlayerStoreLegs = new List<PlayerStoreSkinData>();

    public Texture2D NoAvatarTexture;

    public List<Material> CustomMaterials = new List<Material>();

    public Material CustomMaterialError;

    private static GameSettings Instance;

    public static GameSettings instance
	{
		get
		{
			if (GameSettings.Instance == null)
			{
				GameSettings.Instance = (Resources.Load("Others/GameSettings") as GameSettings);
			}
			return GameSettings.Instance;
		}
	}
}
