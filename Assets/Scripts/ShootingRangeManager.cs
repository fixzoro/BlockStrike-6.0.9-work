using System;
using System.IO;
using UnityEngine;

public class ShootingRangeManager : MonoBehaviour
{
    public Transform Spawn;

    public GameObject PlayerModel;

    public MeshRenderer HeadModel;

    public MeshRenderer[] BodyModel;

    public MeshRenderer[] LegsModel;

    public static bool ShowDamage;

    private void Start()
	{
		CameraManager.SetType(CameraType.Static, new object[0]);
		TimerManager.In(0.2f, delegate()
		{
			ShootingRangeManager.ShowDamage = Settings.ShowDamage;
			UIPanelManager.ShowPanel("Display");
			InputManager.Init();
			WeaponManager.Init();
			this.CreatePlayer();
			UISelectWeapon.AllWeapons = true;
			UISelectWeapon.SelectedUpdateWeaponManager = true;
			UIScore.SetActiveScore(false, 0);
			UIControllerList.Chat.cachedGameObject.SetActive(false);
			UIControllerList.Stats.cachedGameObject.SetActive(false);
		});
	}

	private void CreatePlayer()
	{
		PlayerInput player = GameManager.player;
		player.SetHealth(0);
		CameraManager.SetType(CameraType.None, new object[0]);
		GameManager.controller.ActivePlayer(this.Spawn.position, this.Spawn.eulerAngles);
		player.PlayerWeapon.UpdateWeaponAll(WeaponType.Rifle);
	}

	private void ShowPlayerModel(object value)
	{
		if (!PhotonNetwork.offlineMode)
		{
			return;
		}
		this.PlayerModel.SetActive((bool)value);
	}

	private void SetCustomPlayerSkins(object value)
	{
		if (!this.PlayerModel.activeSelf)
		{
			return;
		}
		string str = string.Empty;
		if (Application.isEditor)
		{
			str = Directory.GetParent(Application.dataPath).FullName + "/Others/Custom Player Skins";
		}
		if (Application.platform == RuntimePlatform.Android)
		{
			string text = new AndroidJavaClass("android.os.Environment").CallStatic<AndroidJavaObject>("getExternalStorageDirectory", new object[0]).Call<string>("getAbsolutePath", new object[0]);
			text += "/Android/data/";
			text += new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity").Call<string>("getPackageName", new object[0]);
			if (Directory.Exists(text))
			{
				if (Directory.Exists(text + "/files/Custom Player Skins"))
				{
					Directory.CreateDirectory(text + "/files/Custom Player Skins");
				}
				str = text + "/files/Custom Player Skins";
			}
			else
			{
				str = Application.dataPath;
			}
		}
		string path = str + "/" + (string)value;
		if (!File.Exists(path))
		{
			MonoBehaviour.print("No found file");
			return;
		}
		byte[] data = File.ReadAllBytes(path);
		Texture2D texture2D = new Texture2D(2, 2);
		texture2D.LoadImage(data);
		texture2D.filterMode = FilterMode.Point;
		if (texture2D.width > 64 || texture2D.height > 64)
		{
			MonoBehaviour.print("Maximum texture size 64x64");
			return;
		}
		Material material = new Material(Shader.Find("Mobile/VertexLit"));
		material.mainTexture = texture2D;
		string lastInvokeCommand = GameConsole.lastInvokeCommand;
		string text2 = lastInvokeCommand;
		switch (text2)
		{
		case "custom_player_head_skin":
			this.HeadModel.sharedMaterial = material;
			break;
		case "custom_player_body_skin":
			for (int i = 0; i < this.BodyModel.Length; i++)
			{
				this.BodyModel[i].sharedMaterial = material;
			}
			break;
		case "custom_player_legs_skin":
			for (int j = 0; j < this.LegsModel.Length; j++)
			{
				this.LegsModel[j].sharedMaterial = material;
			}
			break;
		}
	}
}
