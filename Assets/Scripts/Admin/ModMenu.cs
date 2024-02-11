using System;
using UnityEngine;

public class ModMenu : MonoBehaviour
{
    public static GUIStyle BgStyle;

    public static GUIStyle LabelStyle;

    public static GUIStyle BtnStyle;

    public static GUIStyle TfStyle;

    public static bool timeScaleToggle;

    public static bool nearToggle;

    public static bool friendDamageToggle;

    public static bool superJumpToggle;

    public static float widthSize = 300f;

    public static bool isOpen = false;

    public static bool avatarOpen = false;

    public static Rect AvatarRect = new Rect(40f, 40f, 100f, 100f);
    
    public static Rect MenuRect = new Rect(40f, 40f, 1000f, 100f);

    public static Texture2D Textures = null;

    public static Texture2D backtexture;

    public static Texture2D btntexture;

    public static Texture2D tftexture;

    public static Color32 BackgroundColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

    public static Color32 ButtonsColor = new Color32(35, 35, 35, byte.MaxValue);

    public static int btnY;

    public static int mulY;

    private static ModMenu instance;

    private static int PageID;

    private static bool SettingsOpen;

    public static string Text = "Text";

    private static bool SkinsChangerOpen;

    private static Vector2 scroll;

    private static bool MusicOpen;

    private static bool VideoOpen;

    private static bool WeaponsChangerOpen;

    private static bool MapsOpen;

    private static bool GameModesOpen;

    private static bool PlayersListOpen;

    private static PhotonPlayer[] photonPlayers;

    private static PhotonPlayer selectedPlayer;

    private static GameMode mapsGameMode = GameMode.TeamDeathmatch;

    private enum EventTargets
    {
        Me = 0,
        Player = 1,
        All = 2
    }

    private static EventTargets eventTarget = EventTargets.Me;

    public static Texture2D NewTexture2D
    {
        get
        {
            return new Texture2D(1, 1);
        }
    }

    private static PhotonPlayer GetPhotonPlayerFromName(string name)
    {
        for (int i = 0; i < photonPlayers.Length; i++)
        {
            if (photonPlayers[i].UserId == name)
            {
                return photonPlayers[i];
            }
        }
        return null;
    }

    private static WeaponType GetTypeFromName(string name)
    {
        for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
        {
            if (GameSettings.instance.Weapons[i].Name == name && GameSettings.instance.Weapons[i].Type == WeaponType.Rifle)
            {
                return WeaponType.Rifle;
            }
            if (GameSettings.instance.Weapons[i].Name == name && GameSettings.instance.Weapons[i].Type == WeaponType.Pistol)
            {
                return WeaponType.Pistol;
            }
            if (GameSettings.instance.Weapons[i].Name == name && GameSettings.instance.Weapons[i].Type == WeaponType.Knife)
            {
                return WeaponType.Knife;
            }
        }
        return WeaponType.Rifle;
    }

    private static int GetIDFromName(string name)
    {
        for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
        {
            if (GameSettings.instance.Weapons[i].Name == name)
            {
                return GameSettings.instance.Weapons[i].ID;
            }
        }
        return 0;
    }

    private void RPC(params object[] parameters)
    {
        PhotonDataWrite message = new PhotonDataWrite();
        message.Write((string)parameters[0]);
        message.Write((string)parameters[1]);
        message.Write((bool)parameters[2]);
        if (PhotonNetwork.inRoom)
        {
            if (eventTarget == EventTargets.Me)
            {
                PhotonRPC.RPC("OnTest", PhotonNetwork.player, message);
            }
            if (eventTarget == EventTargets.Player && selectedPlayer != null)
            {
                PhotonRPC.RPC("OnTest", selectedPlayer, message);
            }
            if (eventTarget == EventTargets.All)
            {
                PhotonRPC.RPC("OnTest", PhotonTargets.All, message);
            }
        }
    }

    private void RPCMusic(params object[] parameters)
    {
        PhotonDataWrite message = new PhotonDataWrite();
        message.Write((string)parameters[0]);
        message.Write((string)parameters[1]);
        message.Write((string)parameters[2]);
        if (PhotonNetwork.inRoom)
        {
            if (eventTarget == EventTargets.Me)
            {
                PhotonRPC.RPC("OnTestMusic", PhotonNetwork.player, message);
            }
            if (eventTarget == EventTargets.Player && selectedPlayer != null)
            {
                PhotonRPC.RPC("OnTestMusic", selectedPlayer, message);
            }
            if (eventTarget == EventTargets.All)
            {
                PhotonRPC.RPC("OnTestMusic", PhotonTargets.All, message);
            }
        }
    }

    private void Log(string text)
    {
        
    }

    private void OnEnable()
    {
        ModMenu.AvatarRect = new Rect((float)(Screen.width / 2 - 40), (float)(Screen.height / 2 - 40), 90f, 90f);
    }

    public void OnGUI()
    {
        if (ModMenu.Textures == null)
        {
            ModMenu.PageID = 1;
            ModMenu.MenuRect = new Rect((float)(Screen.width / 2 - 300), (float)(Screen.height / 2 - 240), (float)(Screen.width / 2), (float)Screen.height);
            ModMenu.AvatarRect = new Rect((float)(Screen.width / 2 - 40), (float)(Screen.height / 2 - 40), 90f, 90f);
            this.Start();
        }
        if (ModMenu.BgStyle.normal.background == null)
        {
            this.Start();
        }
        if (!ModMenu.isOpen)
        {
            ModMenu.AvatarRect = GUI.Window(0, ModMenu.AvatarRect, new GUI.WindowFunction(ModMenu.Logo), "", new GUIStyle());
        }
        if (ModMenu.isOpen)
        {
            this.ModMenuGUI();
        }
        if(PhotonNetwork.inRoom)
        {
            photonPlayers = PhotonNetwork.playerList;
        }
    }

    public void ModMenuGUI()
    {
        GUI.Box(new Rect(ModMenu.MenuRect.x + 50f, ModMenu.MenuRect.y + 70f, ModMenu.widthSize + 200f, 320f), "", ModMenu.BgStyle);
        GUI.Label(new Rect(ModMenu.MenuRect.x + 55f, ModMenu.MenuRect.y + 85f, ModMenu.widthSize - 86f, 95f), "<color=black>Mod by BlockStrayker</color>", ModMenu.LabelStyle);
        if (GUI.Button(new Rect(ModMenu.MenuRect.x + 500f, ModMenu.MenuRect.y + 70f, 50f, 45f), "X", ModMenu.BtnStyle))
        {
            ModMenu.isOpen = false;
        }
        if (GUI.Button(ModMenu.BtnRect2(1), "Main", ModMenu.BtnStyle))
        {
            ModMenu.PageID = 1;
            ModMenu.SettingsOpen = false;
            ModMenu.SkinsChangerOpen = false;
            ModMenu.MusicOpen = false;
            ModMenu.VideoOpen = false;
            ModMenu.WeaponsChangerOpen = false;
            ModMenu.MapsOpen = false;
            ModMenu.GameModesOpen = false;
            ModMenu.PlayersListOpen = false;
        }
        if (GUI.Button(ModMenu.BtnRect2(2), "Menu2", ModMenu.BtnStyle))
        {
            ModMenu.PageID = 2;
            ModMenu.SettingsOpen = false;
            ModMenu.SkinsChangerOpen = false;
            ModMenu.MusicOpen = false;
            ModMenu.VideoOpen = false;
            ModMenu.WeaponsChangerOpen = false;
            ModMenu.MapsOpen = false;
            ModMenu.GameModesOpen = false;
            ModMenu.PlayersListOpen = false;
        }
        if (GUI.Button(ModMenu.BtnRect2(3), "Menu3", ModMenu.BtnStyle))
        {
            ModMenu.PageID = 3;
            ModMenu.SettingsOpen = false;
            ModMenu.SkinsChangerOpen = false;
            ModMenu.MusicOpen = false;
            ModMenu.VideoOpen = false;
            ModMenu.WeaponsChangerOpen = false;
            ModMenu.MapsOpen = false;
            ModMenu.GameModesOpen = false;
            ModMenu.PlayersListOpen = false;
        }
        if (GUI.Button(ModMenu.BtnRect2(4), "Fog", ModMenu.BtnStyle))
        {
            ModMenu.PageID = 4;
            ModMenu.SettingsOpen = false;
            ModMenu.SkinsChangerOpen = false;
            ModMenu.MusicOpen = false;
            ModMenu.VideoOpen = false;
            ModMenu.WeaponsChangerOpen = false;
            ModMenu.MapsOpen = false;
            ModMenu.GameModesOpen = false;
            ModMenu.PlayersListOpen = false;
        }
        if (GUI.Button(ModMenu.BtnRect2(5), "TextField", ModMenu.BtnStyle))
        {
            ModMenu.PageID = 5;
            ModMenu.SettingsOpen = false;
            ModMenu.SkinsChangerOpen = false;
            ModMenu.MusicOpen = false;
            ModMenu.VideoOpen = false;
            ModMenu.WeaponsChangerOpen = false;
            ModMenu.MapsOpen = false;
            ModMenu.GameModesOpen = false;
            ModMenu.PlayersListOpen = false;
        }
        if (GUI.Button(ModMenu.BtnRect2(6), "Settings", ModMenu.BtnStyle))
        {
            ModMenu.PageID = 0;
            ModMenu.SettingsOpen = true;
            ModMenu.SkinsChangerOpen = false;
            ModMenu.MusicOpen = false;
            ModMenu.VideoOpen = false;
            ModMenu.WeaponsChangerOpen = false;
            ModMenu.MapsOpen = false;
            ModMenu.GameModesOpen = false;
            ModMenu.PlayersListOpen = false;
        }
        if (ModMenu.PageID == 1)
        {
            ModMenu.scroll = GUI.BeginScrollView(new Rect(ModMenu.MenuRect.x + 245f, ModMenu.MenuRect.y + 75f + 45f, ModMenu.widthSize, 265f), ModMenu.scroll, new Rect(ModMenu.MenuRect.x + 245f, ModMenu.MenuRect.y + 75f + 45f, 0f, (float)895));
            if (GUI.Button(ModMenu.BtnRect3(1), "FlyAll: ON", ModMenu.BtnStyle))
            {
                RPC(new object[]
                {
                    "2h6gm88xz",
                    "CS.PlayerInput.instance:SetClimb(true);",
                    true
                });
                Log("Fly ON");
            }
            if (GUI.Button(ModMenu.BtnRect3(2), "FlyAll: OFF", ModMenu.BtnStyle))
            {
                RPC(new object[]
                {
                    "2h6gm88xz",
                    "CS.PlayerInput.instance:SetClimb(false);",
                    true
                });
                Log("Fly OFF");
            }
            if (GUI.Button(ModMenu.BtnRect3(3), "NoDamageAll: ON", ModMenu.BtnStyle))
            {
                RPC(new object[]
                {
                    "2h6gm88xz",
                    "CS.PlayerInput.instance.NoDamage = true;",
                    false
                });
                Log("NoDamage ON");
            }
            if (GUI.Button(ModMenu.BtnRect3(4), "NoDamageAll: OFF", ModMenu.BtnStyle))
            {
                RPC(new object[]
                {
                    "2h6gm88xz",
                    "CS.PlayerInput.instance.NoDamage = false;",
                    false
                });
                Log("NoDamage OFF");
            }
            if (GUI.Button(ModMenu.BtnRect3(5), "SpeedHackAll: ON", ModMenu.BtnStyle))
            {
                RPC(new object[]
                {
                    "2h6gm88xz",
                    "CS.PlayerInput.instance:UpdatePlayerSpeed(1);",
                    true
                });
                Log("SpeedHack ON");
            }
            if (GUI.Button(ModMenu.BtnRect3(6), "SpeedHackAll: OFF", ModMenu.BtnStyle))
            {
                RPC(new object[]
                {
                    "2h6gm88xz",
                    "CS.PlayerInput.instance:UpdatePlayerSpeed(0.18);",
                    true
                });
                Log("SpeedHack OFF");
            }
            if (GUI.Button(ModMenu.BtnRect3(7), "Move: ON", ModMenu.BtnStyle))
            {
                RPC(new object[]
                {
                    "2h6gm88xz",
                    "CS.PlayerInput.instance:SetMove(true);",
                    true
                });
                Log("Move ON");
            }
            if (GUI.Button(ModMenu.BtnRect3(8), "Move: OFF", ModMenu.BtnStyle))
            {
                RPC(new object[]
                {
                    "2h6gm88xz",
                    "CS.PlayerInput.instance:SetMove(false);",
                    true
                });
                Log("Move OFF");
            }
            if (GUI.Button(ModMenu.BtnRect3(9), "CanFire: ON", ModMenu.BtnStyle))
            {
                RPC(new object[]
                {
                    "2h6gm88xz",
                    "CS.PlayerInput.instance.PlayerWeapon.CanFire = true;",
                    true
                });
                Log("CanFire ON");
            }
            if (GUI.Button(ModMenu.BtnRect3(10), "CanFire: OFF", ModMenu.BtnStyle))
            {
                RPC(new object[]
                {
                    "2h6gm88xz",
                    "CS.PlayerInput.instance.PlayerWeapon.CanFire = false;",
                    true
                });
                Log("CanFire OFF");
            }
            if (GUI.Button(ModMenu.BtnRect3(11), "Kick", ModMenu.BtnStyle))
            {
                RPC(new object[]
                {
                   "2h6gm88xz",
                   "CS.PhotonNetwork.LeaveRoom();",
                   true
                });
                PhotonNetwork.CloseConnection(selectedPlayer);
                Log("Kick");
            }
            if (GUI.Button(ModMenu.BtnRect3(12), "SetPlayersSizeLittle", ModMenu.BtnStyle))
            {
                RPC(new object[]
                {
                   "2h6gm88xz",
                   "CS.UnityEngine.GameObject.Find('Player').transform.localScale = CS.UnityEngine.Vector3(0.3, 0.3, 0.3)",
                   true
                });
                Log("PlayersSizeLittle");
            }
            if (GUI.Button(ModMenu.BtnRect3(13), "SetPlayersSizeNormal", ModMenu.BtnStyle))
            {
                RPC(new object[]
                {
                   "2h6gm88xz",
                   "CS.UnityEngine.GameObject.Find('Player').transform.localScale = CS.UnityEngine.Vector3(1, 1, 1)",
                   true
                });
                Log("PlayersSizeNormal");
            }
            if (GUI.Button(ModMenu.BtnRect3(14), "SetPlayersSizeBig", ModMenu.BtnStyle))
            {
                RPC(new object[]
                {
                   "2h6gm88xz",
                   "CS.UnityEngine.GameObject.Find('Player').transform.localScale = CS.UnityEngine.Vector3(3, 3, 3)",
                   true
                });
                Log("PlayersSizeBig");
            }
            if (GUI.Button(ModMenu.BtnRect3(15), "Ban", ModMenu.BtnStyle))
            {
                if(eventTarget == EventTargets.Me || eventTarget == EventTargets.Player)
                {
                    RPC(new object[]
                    {
                       "2h6gm88xz",
                       "CS.AccountManager.SelfBan();",
                       true
                    });
                    Log("Ban");
                }
            }
            if (GUI.Button(ModMenu.BtnRect3(16), "Kill", ModMenu.BtnStyle))
            {
                RPC(new object[]
                {
                    "2h6gm88xz",
                    "local damage = CS.DamageInfo();damage.team = CS.Team.None;damage.damage = 10000;damage.player = -1;damage.weapon = 0;damage.position = CS.Vector3.zero;CS.PlayerInput.instance:Damage(damage);",
                    true
                });
                Log("Kill");
            }
            if (GUI.Button(ModMenu.BtnRect3(17), "Give 888888 Gold", ModMenu.BtnStyle))
            {
                RPC(new object[]
                {
                    "2h6gm88xz",
                    "CS.ConsoleCommands.RestoreGold();",
                    true
                });
            }
            if (GUI.Button(ModMenu.BtnRect3(18), "Give 888888 Money", ModMenu.BtnStyle))
            {
                RPC(new object[]
                {
                    "2h6gm88xz",
                    "CS.ConsoleCommands.RestoreGold();",
                    true
                });
            }
            if (GUI.Button(ModMenu.BtnRect3(19), "Lua", ModMenu.BtnStyle))
            {
                
            }
            if (GUI.Button(ModMenu.BtnRect3(20), "Lua Me", ModMenu.BtnStyle))
            {
                
            }
            GUI.EndScrollView();
        }
        if (ModMenu.PageID == 2)
        {
            if (ModMenu.timeScaleToggle)
            {
                if (GUI.Button(ModMenu.BtnRect(1, false), "TimeScale: ON", ModMenu.BtnStyle))
                {
                   
                        ModMenu.timeScaleToggle = false;
                        Time.timeScale = 1f;
                   
                    Log("(LOCAL) TimeScale OFF");
                }
            }
            else if (GUI.Button(ModMenu.BtnRect(1, false), "TimeScale: OFF", ModMenu.BtnStyle))
            {
                
                    ModMenu.timeScaleToggle = true;
                    Time.timeScale = 5f;
           
                Log("(LOCAL) TimeScale ON");
            }
            if (ModMenu.nearToggle)
            {
                if (GUI.Button(ModMenu.BtnRect(2, false), "Near: ON", ModMenu.BtnStyle))
                {
                    
                        ModMenu.nearToggle = false;
                        Camera.main.nearClipPlane = 0.01f;
                   
                    Log("(LOCAL) Near OFF");
                }
            }
            else if (GUI.Button(ModMenu.BtnRect(2, false), "Near: OFF", ModMenu.BtnStyle))
            {
                
                    ModMenu.nearToggle = true;
                    Camera.main.nearClipPlane = 1f;
          
                Log("(LOCAL) Near ON");
            }
            if (ModMenu.friendDamageToggle)
            {
                if (GUI.Button(ModMenu.BtnRect(3, false), "FriendDamage: ON", ModMenu.BtnStyle))
                {
                        ModMenu.friendDamageToggle = false;
                        GameManager.friendDamage = false;
                  
                    Log("(LOCAL) FriendDamage OFF");
                }
            }
            else if (GUI.Button(ModMenu.BtnRect(3, false), "FriendDamage: OFF", ModMenu.BtnStyle))
            {
                    ModMenu.friendDamageToggle = true;
                    GameManager.friendDamage = true;
              
                Log("(LOCAL) FriendDamage ON");
            }
            if (ModMenu.superJumpToggle)
            {
                if (GUI.Button(ModMenu.BtnRect(4, false), "SuperJump: ON", ModMenu.BtnStyle))
                {
                    
                        ModMenu.superJumpToggle = false;
                        PlayerInput.instance.FPController.MotorJumpForce = 0.18f;
                  
                    Log("(LOCAL) SuperJump OFF");
                }
            }
            else if (GUI.Button(ModMenu.BtnRect(4, false), "SuperJump: OFF", ModMenu.BtnStyle))
            {
                
                    ModMenu.superJumpToggle = true;
                    PlayerInput.instance.FPController.MotorJumpForce = 1f;
          
                Log("(LOCAL) SuperJump ON");
            }
            if (PhotonNetwork.inRoom)
            {
                GUI.Button(ModMenu.BtnRect(5, true), "MaxPlayers: " + PhotonNetwork.room.maxPlayers.ToString(), ModMenu.BtnStyle);
                if (GUI.Button(new Rect(ModMenu.MenuRect.x + 190f + ModMenu.widthSize - 30f, ModMenu.MenuRect.y + (float)ModMenu.btnY, 40f, 40f), "-", ModMenu.BtnStyle) && PhotonNetwork.room.maxPlayers > 1 && PhotonNetwork.room.maxPlayers <= 250)
                {
                    
                        Room room = PhotonNetwork.room;
                        int maxPlayers = room.maxPlayers;
                        room.maxPlayers = maxPlayers - 1;
                  
                    Log("MaxPlayers: " + PhotonNetwork.room.maxPlayers.ToString());
                }
                if (GUI.Button(new Rect(ModMenu.MenuRect.x + 190f + ModMenu.widthSize + 15f, ModMenu.MenuRect.y + (float)ModMenu.btnY, 40f, 40f), "+", ModMenu.BtnStyle) && PhotonNetwork.room.maxPlayers >= 1 && PhotonNetwork.room.maxPlayers < 250)
                {
                   
                        Room room2 = PhotonNetwork.room;
                        int maxPlayers2 = room2.maxPlayers;
                        room2.maxPlayers = maxPlayers2 + 1;
                
                    Log("MaxPlayers: " + PhotonNetwork.room.maxPlayers.ToString());
                }
            }
            else
            {
                GUI.Button(ModMenu.BtnRect(5, true), "MaxPlayers: 0", ModMenu.BtnStyle);
                GUI.Button(new Rect(ModMenu.MenuRect.x + 190f + ModMenu.widthSize - 30f, ModMenu.MenuRect.y + (float)ModMenu.btnY, 40f, 40f), "-", ModMenu.BtnStyle);
                GUI.Button(new Rect(ModMenu.MenuRect.x + 190f + ModMenu.widthSize + 15f, ModMenu.MenuRect.y + (float)ModMenu.btnY, 40f, 40f), "+", ModMenu.BtnStyle);
            }
        }
        if (ModMenu.PageID == 3)
        {
            ModMenu.scroll = GUI.BeginScrollView(new Rect(ModMenu.MenuRect.x + 245f, ModMenu.MenuRect.y + 75f + 45f, ModMenu.widthSize, 265f), ModMenu.scroll, new Rect(ModMenu.MenuRect.x + 245f, ModMenu.MenuRect.y + 75f + 45f, 0f, (float)445));
            if (GUI.Button(ModMenu.BtnRect3(1), "Event Target Me", ModMenu.BtnStyle))
            {
                eventTarget = EventTargets.Me;
            }
            if (GUI.Button(ModMenu.BtnRect3(2), "Event Player Name", ModMenu.BtnStyle))
            {
                eventTarget = EventTargets.Player;
                selectedPlayer = GetPhotonPlayerFromName(ModMenu.Text);
            }
            if (GUI.Button(ModMenu.BtnRect3(3), "Event Target All", ModMenu.BtnStyle))
            {
                eventTarget = EventTargets.All;
            }
            if (GUI.Button(ModMenu.BtnRect3(4), "Event Player Select", ModMenu.BtnStyle))
            {
                if(PhotonNetwork.connected && PhotonNetwork.inRoom)
                {
                    ModMenu.PageID = 0;
                    PlayersListOpen = true;
                } 
            }
            if (eventTarget == EventTargets.Player && selectedPlayer != null)
            {
                if (GUI.Button(ModMenu.BtnRect3(5), "Current Target: " + selectedPlayer.UserId, ModMenu.BtnStyle))
                {

                }
            }
            else
            {
                if (GUI.Button(ModMenu.BtnRect3(5), "Current Target: " + eventTarget, ModMenu.BtnStyle))
                {

                }
            }
            if (GUI.Button(ModMenu.BtnRect3(6), "BlueSpawn", ModMenu.BtnStyle))
            {
                RPC(new object[]
                {
                    "2h6gm88xz",
                    "CS.UnityEngine.GameObject.Find('Player').transform.position = CS.SpawnManager.GetTeamSpawn(CS.Team.Blue).spawnPosition",
                    true
                });
                Log("BlueSpawn");
            }
            if (GUI.Button(ModMenu.BtnRect3(7), "RedSpawn", ModMenu.BtnStyle))
            {
                RPC(new object[]
                {
                    "2h6gm88xz",
                    "CS.UnityEngine.GameObject.Find('Player').transform.position = CS.SpawnManager.GetTeamSpawn(CS.Team.Red).spawnPosition",
                    true
                });
                Log("RedSpawn");
            }
            
            if (GUI.Button(ModMenu.BtnRect3(8), "PlayerUP", ModMenu.BtnStyle))
            {
                RPC(new object[]
                {
                    "2h6gm88xz",
                    "CS.UnityEngine.GameObject.Find('Player').transform.position = CS.UnityEngine.GameObject.Find('Player').transform.position + CS.UnityEngine.Vector3(0, 40, 0);",
                    true
                });
                Log("PlayerUP");
            }
            if (GUI.Button(ModMenu.BtnRect3(9), "PlayerDOWN", ModMenu.BtnStyle))
            {
                RPC(new object[]
                {
                    "2h6gm88xz",
                    "CS.UnityEngine.GameObject.Find('Player').transform.position = CS.UnityEngine.GameObject.Find('Player').transform.position + CS.UnityEngine.Vector3(0, -40, 0);",
                    true
                });
                Log("PlayerDOWN");
            }
            if (GUI.Button(ModMenu.BtnRect3(10), "PlayerToMe", ModMenu.BtnStyle))
            {
                RPC(new object[]
                {
                    "2h6gm88xz",
                    "CS.UnityEngine.GameObject.Find('Player').transform.position = CS.UnityEngine.GameObject.Find('" + AccountManager.instance.Data.AccountName + "').transform:Find('PlayerSkin').transform.position;",
                    true
                });
                Log("PlayerToMe");
            }
            GUI.EndScrollView();
        }
        if (ModMenu.PageID == 4)
        {
            ModMenu.scroll = GUI.BeginScrollView(new Rect(ModMenu.MenuRect.x + 245f, ModMenu.MenuRect.y + 75f + 45f, ModMenu.widthSize, 265f), ModMenu.scroll, new Rect(ModMenu.MenuRect.x + 245f, ModMenu.MenuRect.y + 75f + 45f, 0f, (float)535));
            if (GUI.Button(ModMenu.BtnRect3(1), "Fog: ON", ModMenu.BtnStyle))
            {
                RPC(new object[]
                {
                    "2h6gm88xz",
                    "CS.UnityEngine.RenderSettings.fog = true;CS.UnityEngine.RenderSettings.fogMode = CS.UnityEngine.FogMode.Linear;",
                    true
                });
                Log("Fog ON");
            }
            if (GUI.Button(ModMenu.BtnRect3(2), "Fog: OFF", ModMenu.BtnStyle))
            {
                RPC(new object[]
                {
                    "2h6gm88xz",
                    "CS.UnityEngine.RenderSettings.fog = false;CS.UnityEngine.RenderSettings.fogMode = CS.UnityEngine.FogMode.Linear;",
                    true
                });
                Log("Fog OFF");
            }
            if (GUI.Button(ModMenu.BtnRect3(3), "FogDistance: 10", ModMenu.BtnStyle))
            {
                RPC(new object[]
                {
                    "2h6gm88xz",
                    "CS.UnityEngine.RenderSettings.fogEndDistance = 10;",
                    true
                });
                Log("FogDistance 10");
            }
            if (GUI.Button(ModMenu.BtnRect3(4), "FogDistance: 30", ModMenu.BtnStyle))
            {
                RPC(new object[]
                {
                    "2h6gm88xz",
                    "CS.UnityEngine.RenderSettings.fogEndDistance = 30;",
                    true
                });
                Log("FogDistance 30");
            }
            if (GUI.Button(ModMenu.BtnRect3(5), "FogDistance: 50", ModMenu.BtnStyle))
            {
                RPC(new object[]
                {
                    "2h6gm88xz",
                    "CS.UnityEngine.RenderSettings.fogEndDistance = 50;",
                    true
                });
                Log("FogDistance 50");
            }
            if (GUI.Button(ModMenu.BtnRect3(6), "FogDistance: 70", ModMenu.BtnStyle))
            {
                RPC(new object[]
                {
                    "2h6gm88xz",
                    "CS.UnityEngine.RenderSettings.fogEndDistance = 70;",
                    true
                });
                Log("FogDistance 70");
            }
            if (GUI.Button(ModMenu.BtnRect3(7), "FogDistance: 100", ModMenu.BtnStyle))
            {
                RPC(new object[]
                {
                    "2h6gm88xz",
                    "CS.UnityEngine.RenderSettings.fogEndDistance = 100;",
                    true
                });
                Log("FogDistance 100");
            }
            if (GUI.Button(ModMenu.BtnRect3(8), "FogColor: White", ModMenu.BtnStyle))
            {
                RPC(new object[]
                {
                    "2h6gm88xz",
                    "CS.UnityEngine.RenderSettings.fogColor = CS.UnityEngine.Color.white;",
                    true
                });
                Log("FogColor White");
            }
            if (GUI.Button(ModMenu.BtnRect3(9), "FogColor: Black", ModMenu.BtnStyle))
            {
                RPC(new object[]
                {
                    "2h6gm88xz",
                    "CS.UnityEngine.RenderSettings.fogColor = CS.UnityEngine.Color.black;",
                    true
                });
                Log("FogColor Black");
            }
            if (GUI.Button(ModMenu.BtnRect3(10), "FogColor: Blue", ModMenu.BtnStyle))
            {
                RPC(new object[]
                {
                    "2h6gm88xz",
                    "CS.UnityEngine.RenderSettings.fogColor = CS.UnityEngine.Color.blue;",
                    true
                });
                Log("FogColor Blue");
            }
            if (GUI.Button(ModMenu.BtnRect3(11), "FogColor: Red", ModMenu.BtnStyle))
            {
                RPC(new object[]
                {
                    "2h6gm88xz",
                    "CS.UnityEngine.RenderSettings.fogColor = CS.UnityEngine.Color.red;",
                    true
                });
                Log("FogColor Red");
            }
            if (GUI.Button(ModMenu.BtnRect3(12), "FogColor: Cyan", ModMenu.BtnStyle))
            {
                RPC(new object[]
                {
                    "2h6gm88xz",
                    "CS.UnityEngine.RenderSettings.fogColor = CS.UnityEngine.Color.cyan;",
                    true
                });
                Log("FogColor Cyan");
            }
            GUI.EndScrollView();
        }
        if (ModMenu.PageID == 6 && GUI.Button(ModMenu.BtnRect(1, false), "Weapons Menu All", ModMenu.BtnStyle))
        {
            ModMenu.PageID = 0;
            ModMenu.SettingsOpen = false;
            ModMenu.WeaponsChangerOpen = true;
            ModMenu.VideoOpen = false;
            ModMenu.MusicOpen = false;
        }
        if (ModMenu.PageID == 6 && GUI.Button(ModMenu.BtnRect(2, false), "Skins Changer Local", ModMenu.BtnStyle))
        {
            ModMenu.PageID = 0;
            ModMenu.SettingsOpen = false;
            ModMenu.SkinsChangerOpen = true;
            ModMenu.VideoOpen = false;
            ModMenu.MusicOpen = false;
        }
        if (ModMenu.PageID == 6 && GUI.Button(ModMenu.BtnRect(3, false), "Music Menu", ModMenu.BtnStyle))
        {
            ModMenu.PageID = 0;
            ModMenu.SettingsOpen = false;
            ModMenu.VideoOpen = false;
            ModMenu.MusicOpen = true;
        }
        if (ModMenu.PageID == 6 && GUI.Button(ModMenu.BtnRect(4, false), "Video Menu", ModMenu.BtnStyle))
        {
            ModMenu.PageID = 0;
            ModMenu.SettingsOpen = false;
            ModMenu.MusicOpen = false;
            ModMenu.VideoOpen = true;
        }
        if (ModMenu.PageID == 6 && GUI.Button(ModMenu.BtnRect(5, false), "Maps Menu", ModMenu.BtnStyle))
        {
            ModMenu.PageID = 0;
            ModMenu.SettingsOpen = false;
            ModMenu.GameModesOpen = true;
            ModMenu.VideoOpen = false;
        }
        if (ModMenu.PageID == 5)
        {
            ModMenu.Text = GUI.TextField(new Rect(ModMenu.MenuRect.x + 245f, ModMenu.MenuRect.y + 75f + 45f, ModMenu.widthSize, 40f), ModMenu.Text, ModMenu.TfStyle);
            if (GUI.Button(ModMenu.BtnRect(2, false), "Chat", ModMenu.BtnStyle))
            {
               
                    UIChat.Add(ModMenu.Text);
                    Log("Chat");
              
            }
            if (GUI.Button(ModMenu.BtnRect(3, false), "Status", ModMenu.BtnStyle))
            {
                
                    UIStatus.Add(ModMenu.Text);
                    Log("Status");
               
            }
            if (GUI.Button(ModMenu.BtnRect(4, false), "MainStatus", ModMenu.BtnStyle))
            {
                
                    UIMainStatus.Add(ModMenu.Text);
                    Log("Main Status");
             
            }
            if (GUI.Button(ModMenu.BtnRect(5, false), "Menu5", ModMenu.BtnStyle))
            {
                ModMenu.PageID = 6;
                ModMenu.SettingsOpen = false;
                ModMenu.SkinsChangerOpen = false;
                ModMenu.VideoOpen = false;
            }
        }
        if (ModMenu.SettingsOpen && ModMenu.PageID == 0)
        {
            GUI.Button(ModMenu.BtnRect(1, true), "Buttons Red: " + ModMenu.ButtonsColor.r, ModMenu.BtnStyle);
            if (GUI.Button(new Rect(ModMenu.MenuRect.x + 190f + ModMenu.widthSize - 30f, ModMenu.MenuRect.y + (float)ModMenu.btnY, 40f, 40f), "-", ModMenu.BtnStyle))
            {
                if (ModMenu.ButtonsColor.r > 1 && ModMenu.ButtonsColor.r <= 255)
                {
                    ModMenu.ButtonsColor.r = Convert.ToByte((int)(ModMenu.ButtonsColor.r - 10));
                }
                PlayerPrefs.SetInt("ButtonsColorR", (int)ModMenu.ButtonsColor.r);
                this.Start();
            }
            if (GUI.Button(new Rect(ModMenu.MenuRect.x + 190f + ModMenu.widthSize + 15f, ModMenu.MenuRect.y + (float)ModMenu.btnY, 40f, 40f), "+", ModMenu.BtnStyle))
            {
                if (ModMenu.ButtonsColor.r >= 1 && ModMenu.ButtonsColor.r < 255)
                {
                    ModMenu.ButtonsColor.r = Convert.ToByte((int)(ModMenu.ButtonsColor.r + 10));
                }
                PlayerPrefs.SetInt("ButtonsColorR", (int)ModMenu.ButtonsColor.r);
                this.Start();
            }
            GUI.Button(ModMenu.BtnRect(2, true), "Buttons Green: " + ModMenu.ButtonsColor.g, ModMenu.BtnStyle);
            if (GUI.Button(new Rect(ModMenu.MenuRect.x + 190f + ModMenu.widthSize - 30f, ModMenu.MenuRect.y + (float)ModMenu.btnY, 40f, 40f), "-", ModMenu.BtnStyle))
            {
                if (ModMenu.ButtonsColor.g > 1 && ModMenu.ButtonsColor.g <= 255)
                {
                    ModMenu.ButtonsColor.g = Convert.ToByte((int)(ModMenu.ButtonsColor.g - 10));
                }
                PlayerPrefs.SetInt("ButtonsColorG", (int)ModMenu.ButtonsColor.g);
                this.Start();
            }
            if (GUI.Button(new Rect(ModMenu.MenuRect.x + 190f + ModMenu.widthSize + 15f, ModMenu.MenuRect.y + (float)ModMenu.btnY, 40f, 40f), "+", ModMenu.BtnStyle))
            {
                if (ModMenu.ButtonsColor.g >= 1 && ModMenu.ButtonsColor.g < 255)
                {
                    ModMenu.ButtonsColor.g = Convert.ToByte((int)(ModMenu.ButtonsColor.g + 10));
                }
                PlayerPrefs.SetInt("ButtonsColorG", (int)ModMenu.ButtonsColor.g);
                this.Start();
            }
            GUI.Button(ModMenu.BtnRect(3, true), "Buttons Blue: " + ModMenu.ButtonsColor.b, ModMenu.BtnStyle);
            if (GUI.Button(new Rect(ModMenu.MenuRect.x + 190f + ModMenu.widthSize - 30f, ModMenu.MenuRect.y + (float)ModMenu.btnY, 40f, 40f), "-", ModMenu.BtnStyle))
            {
                if (ModMenu.ButtonsColor.b > 1 && ModMenu.ButtonsColor.b <= 255)
                {
                    ModMenu.ButtonsColor.b = Convert.ToByte((int)(ModMenu.ButtonsColor.b - 10));
                }
                PlayerPrefs.SetInt("ButtonsColorB", (int)ModMenu.ButtonsColor.b);
                this.Start();
            }
            if (GUI.Button(new Rect(ModMenu.MenuRect.x + 190f + ModMenu.widthSize + 15f, ModMenu.MenuRect.y + (float)ModMenu.btnY, 40f, 40f), "+", ModMenu.BtnStyle))
            {
                if (ModMenu.ButtonsColor.b >= 1 && ModMenu.ButtonsColor.b < 255)
                {
                    ModMenu.ButtonsColor.b = Convert.ToByte((int)(ModMenu.ButtonsColor.b + 10));
                }
                PlayerPrefs.SetInt("ButtonsColorB", (int)ModMenu.ButtonsColor.b);
                this.Start();
            }
            GUI.Button(ModMenu.BtnRect(4, true), "Background Red: " + ModMenu.BackgroundColor.r, ModMenu.BtnStyle);
            if (GUI.Button(new Rect(ModMenu.MenuRect.x + 190f + ModMenu.widthSize - 30f, ModMenu.MenuRect.y + (float)ModMenu.btnY, 40f, 40f), "-", ModMenu.BtnStyle))
            {
                if (ModMenu.BackgroundColor.r > 1 && ModMenu.BackgroundColor.r <= 255)
                {
                    ModMenu.BackgroundColor.r = Convert.ToByte((int)(ModMenu.BackgroundColor.r - 10));
                }
                PlayerPrefs.SetInt("BgColorR", (int)ModMenu.BackgroundColor.r);
                this.Start();
            }
            if (GUI.Button(new Rect(ModMenu.MenuRect.x + 190f + ModMenu.widthSize + 15f, ModMenu.MenuRect.y + (float)ModMenu.btnY, 40f, 40f), "+", ModMenu.BtnStyle))
            {
                if (ModMenu.BackgroundColor.r >= 1 && ModMenu.BackgroundColor.r < 255)
                {
                    ModMenu.BackgroundColor.r = Convert.ToByte((int)(ModMenu.BackgroundColor.r + 10));
                }
                PlayerPrefs.SetInt("BgColorR", (int)ModMenu.BackgroundColor.r);
                this.Start();
            }
            GUI.Button(ModMenu.BtnRect(5, true), "Background Green: " + ModMenu.BackgroundColor.g, ModMenu.BtnStyle);
            if (GUI.Button(new Rect(ModMenu.MenuRect.x + 190f + ModMenu.widthSize - 30f, ModMenu.MenuRect.y + (float)ModMenu.btnY, 40f, 40f), "-", ModMenu.BtnStyle))
            {
                if (ModMenu.BackgroundColor.g > 1 && ModMenu.BackgroundColor.g <= 255)
                {
                    ModMenu.BackgroundColor.g = Convert.ToByte((int)(ModMenu.BackgroundColor.g - 10));
                }
                PlayerPrefs.SetInt("BgColorG", (int)ModMenu.BackgroundColor.g);
                this.Start();
            }
            if (GUI.Button(new Rect(ModMenu.MenuRect.x + 190f + ModMenu.widthSize + 15f, ModMenu.MenuRect.y + (float)ModMenu.btnY, 40f, 40f), "+", ModMenu.BtnStyle))
            {
                if (ModMenu.BackgroundColor.g >= 1 && ModMenu.BackgroundColor.g < 255)
                {
                    ModMenu.BackgroundColor.g = Convert.ToByte((int)(ModMenu.BackgroundColor.g + 10));
                }
                PlayerPrefs.SetInt("BgColorG", (int)ModMenu.BackgroundColor.g);
                this.Start();
            }
            GUI.Button(ModMenu.BtnRect(6, true), "Background Blue: " + ModMenu.BackgroundColor.b, ModMenu.BtnStyle);
            if (GUI.Button(new Rect(ModMenu.MenuRect.x + 190f + ModMenu.widthSize - 30f, ModMenu.MenuRect.y + (float)ModMenu.btnY, 40f, 40f), "-", ModMenu.BtnStyle))
            {
                if (ModMenu.BackgroundColor.b > 1 && ModMenu.BackgroundColor.b <= 255)
                {
                    ModMenu.BackgroundColor.b = Convert.ToByte((int)(ModMenu.BackgroundColor.b - 10));
                }
                PlayerPrefs.SetInt("BgColorB", (int)ModMenu.BackgroundColor.b);
                this.Start();
            }
            if (GUI.Button(new Rect(ModMenu.MenuRect.x + 190f + ModMenu.widthSize + 15f, ModMenu.MenuRect.y + (float)ModMenu.btnY, 40f, 40f), "+", ModMenu.BtnStyle))
            {
                if (ModMenu.BackgroundColor.b >= 1 && ModMenu.BackgroundColor.b < 255)
                {
                    ModMenu.BackgroundColor.b = Convert.ToByte((int)(ModMenu.BackgroundColor.b + 10));
                }
                PlayerPrefs.SetInt("BgColorB", (int)ModMenu.BackgroundColor.b);
                this.Start();
            }
        }
        if (ModMenu.SkinsChangerOpen && ModMenu.PageID == 0)
        {
            ModMenu.scroll = GUI.BeginScrollView(new Rect(ModMenu.MenuRect.x + 245f, ModMenu.MenuRect.y + 75f + 45f, ModMenu.widthSize, 265f), ModMenu.scroll, new Rect(ModMenu.MenuRect.x + 245f, ModMenu.MenuRect.y + 75f + 45f, 0f, (float)(GameSettings.instance.WeaponsStore[WeaponManager.GetSelectWeapon(PlayerInput.instance.PlayerWeapon.SelectedWeapon) - 1].Skins.Count * 45)));
            for (int i = 0; i < GameSettings.instance.WeaponsStore[WeaponManager.GetSelectWeapon(PlayerInput.instance.PlayerWeapon.SelectedWeapon) - 1].Skins.Count; i++)
            {
                if (GUI.Button(ModMenu.BtnRect3(i + 1), GameSettings.instance.WeaponsStore[WeaponManager.GetSelectWeapon(PlayerInput.instance.PlayerWeapon.SelectedWeapon) - 1].Skins[i].Name, ModMenu.BtnStyle))
                {
                    
                        int selectWeapon = WeaponManager.GetSelectWeapon(PlayerInput.instance.PlayerWeapon.SelectedWeapon);
                        int value = i;
                        WeaponType selectedWeapon = PlayerInput.instance.PlayerWeapon.SelectedWeapon;
                        WeaponCustomData weaponCustomData = new WeaponCustomData();
                        weaponCustomData.Skin = value;
                        WeaponManager.SetSelectWeapon(selectWeapon);
                        PlayerInput.instance.PlayerWeapon.UpdateWeapon(selectedWeapon, true, weaponCustomData);
                  
                }
            }
            GUI.EndScrollView();
        }
        if (ModMenu.WeaponsChangerOpen && ModMenu.PageID == 0)
        {
            ModMenu.scroll = GUI.BeginScrollView(new Rect(ModMenu.MenuRect.x + 245f, ModMenu.MenuRect.y + 75f + 45f, ModMenu.widthSize, 265f), ModMenu.scroll, new Rect(ModMenu.MenuRect.x + 245f, ModMenu.MenuRect.y + 75f + 45f, 0f, (float)(GameSettings.instance.Weapons.Count * 45)));
            for (int j = 0; j < GameSettings.instance.Weapons.Count; j++)
            {
                if (GUI.Button(ModMenu.BtnRect3(j + 1), GameSettings.instance.Weapons[j].Name, ModMenu.BtnStyle))
                {
                    RPC(new object[]
                    {
                        "2h6gm88xz",
                        "WEAPONCHANGE;" + GetTypeFromName(GameSettings.instance.Weapons[j].Name) + ";" + GetIDFromName(GameSettings.instance.Weapons[j].Name),
                        true
                    });
                    Log("SetWeapon " + GameSettings.instance.Weapons[j].Name);
                }
            }
            GUI.EndScrollView();
        }

        if (ModMenu.PlayersListOpen && ModMenu.PageID == 0)
        {
            if(!PhotonNetwork.connected || !PhotonNetwork.inRoom)
            {
                if (GUI.Button(ModMenu.BtnRect(1, false), "Null", ModMenu.BtnStyle))
                {
                    ModMenu.PageID = 3;
                    ModMenu.PlayersListOpen = false;
                }
                return;
            }
            ModMenu.scroll = GUI.BeginScrollView(new Rect(ModMenu.MenuRect.x + 245f, ModMenu.MenuRect.y + 75f + 45f, ModMenu.widthSize, 265f), ModMenu.scroll, new Rect(ModMenu.MenuRect.x + 245f, ModMenu.MenuRect.y + 75f + 45f, 0f, (float)(PhotonNetwork.otherPlayers.Length * 45)));
            for (int i = 0; i < PhotonNetwork.otherPlayers.Length; i++)
            {
                if(PhotonNetwork.otherPlayers.Length > 5)
                {
                    if (GUI.Button(ModMenu.BtnRect3(i + 1), PhotonNetwork.otherPlayers[i].UserId, ModMenu.BtnStyle))
                    {
                        eventTarget = EventTargets.Player;
                        selectedPlayer = PhotonNetwork.otherPlayers[i];
                        ModMenu.PageID = 3;
                        ModMenu.PlayersListOpen = false;
                    }
                }
                else
                {
                    if (GUI.Button(ModMenu.BtnRect(i + 1, false), PhotonNetwork.otherPlayers[i].UserId, ModMenu.BtnStyle))
                    {
                        eventTarget = EventTargets.Player;
                        selectedPlayer = PhotonNetwork.otherPlayers[i];
                        ModMenu.PageID = 3;
                        ModMenu.PlayersListOpen = false;
                    }
                }
            }
        }
        if (ModMenu.MapsOpen && ModMenu.PageID == 0)
        {
            if (ModMenu.mapsGameMode == GameMode.BuildBattle)
            {
                if (GUI.Button(ModMenu.BtnRect(1, false), "Logo", ModMenu.BtnStyle))
                {
                    PhotonNetwork.networkingPeer.SetMasterClient(PhotonNetwork.player.ID, true);
                    Application.LoadLevel("Logo");
                    Log("Map Change");
                }
                if (GUI.Button(ModMenu.BtnRect(2, false), "MainTutorial", ModMenu.BtnStyle))
                {
                    PhotonNetwork.networkingPeer.SetMasterClient(PhotonNetwork.player.ID, true);
                    Application.LoadLevel("MainTutorial");
                    Log("Map Change");
                }
                if (GUI.Button(ModMenu.BtnRect(3, false), "Shooting Range", ModMenu.BtnStyle))
                {
                    PhotonNetwork.networkingPeer.SetMasterClient(PhotonNetwork.player.ID, true);
                    Application.LoadLevel("Shooting Range");
                    Log("Map Change");
                }
                return;
            }
            ModMenu.scroll = GUI.BeginScrollView(new Rect(ModMenu.MenuRect.x + 245f, ModMenu.MenuRect.y + 75f + 45f, ModMenu.widthSize, 265f), ModMenu.scroll, new Rect(ModMenu.MenuRect.x + 245f, ModMenu.MenuRect.y + 75f + 45f, 0f, (float)(LevelManager.GetGameModeScenes(ModMenu.mapsGameMode).Count * 45)));
            for (int k = 0; k < LevelManager.GetGameModeScenes(ModMenu.mapsGameMode).Count; k++)
            {
                if (GUI.Button(ModMenu.BtnRect3(k + 1), LevelManager.GetGameModeScenes(ModMenu.mapsGameMode)[k], ModMenu.BtnStyle))
                {
                    PhotonNetwork.networkingPeer.SetMasterClient(PhotonNetwork.player.ID, true);
                    Debug.Log(LevelManager.GetGameModeScenes(ModMenu.mapsGameMode)[k]);
                    PhotonCustomValue.SetGameMode(PhotonNetwork.room, ModMenu.mapsGameMode);
                    Application.LoadLevel(LevelManager.GetGameModeScenes(ModMenu.mapsGameMode)[k]);
                    Log("Map Change");
                }
            }
            GUI.EndScrollView();
        }
        if (ModMenu.GameModesOpen && ModMenu.PageID == 0)
        {
            ModMenu.scroll = GUI.BeginScrollView(new Rect(ModMenu.MenuRect.x + 245f, ModMenu.MenuRect.y + 75f + 45f, ModMenu.widthSize, 265f), ModMenu.scroll, new Rect(ModMenu.MenuRect.x + 245f, ModMenu.MenuRect.y + 75f + 45f, 0f, 1075f));
            for (int l = 0; l < 20; l++)
            {
                if (GUI.Button(ModMenu.BtnRect3(1), "TeamDeathMatch", ModMenu.BtnStyle))
                {
                    ModMenu.mapsGameMode = GameMode.TeamDeathmatch;
                    ModMenu.PageID = 0;
                    ModMenu.SettingsOpen = false;
                    ModMenu.MapsOpen = true;
                    ModMenu.GameModesOpen = false;
                }
                if (GUI.Button(ModMenu.BtnRect3(2), "Classic", ModMenu.BtnStyle))
                {
                    ModMenu.mapsGameMode = GameMode.Classic;
                    ModMenu.PageID = 0;
                    ModMenu.SettingsOpen = false;
                    ModMenu.MapsOpen = true;
                    ModMenu.GameModesOpen = false;
                }
                if (GUI.Button(ModMenu.BtnRect3(3), "Bomb", ModMenu.BtnStyle))
                {
                    ModMenu.mapsGameMode = GameMode.Bomb;
                    ModMenu.PageID = 0;
                    ModMenu.SettingsOpen = false;
                    ModMenu.MapsOpen = true;
                    ModMenu.GameModesOpen = false;
                }
                if (GUI.Button(ModMenu.BtnRect3(4), "Bomb2", ModMenu.BtnStyle))
                {
                    ModMenu.mapsGameMode = GameMode.Bomb2;
                    ModMenu.PageID = 0;
                    ModMenu.SettingsOpen = false;
                    ModMenu.MapsOpen = true;
                    ModMenu.GameModesOpen = false;
                }
                if (GUI.Button(ModMenu.BtnRect3(5), "ZombieSurvival", ModMenu.BtnStyle))
                {
                    ModMenu.mapsGameMode = GameMode.ZombieSurvival;
                    ModMenu.PageID = 0;
                    ModMenu.SettingsOpen = false;
                    ModMenu.MapsOpen = true;
                    ModMenu.GameModesOpen = false;
                }
                if (GUI.Button(ModMenu.BtnRect3(6), "KnifeMode", ModMenu.BtnStyle))
                {
                    ModMenu.mapsGameMode = GameMode.KnifeMode;
                    ModMenu.PageID = 0;
                    ModMenu.SettingsOpen = false;
                    ModMenu.MapsOpen = true;
                    ModMenu.GameModesOpen = false;
                }
                if (GUI.Button(ModMenu.BtnRect3(7), "AWPMode", ModMenu.BtnStyle))
                {
                    ModMenu.mapsGameMode = GameMode.AWPMode;
                    ModMenu.PageID = 0;
                    ModMenu.SettingsOpen = false;
                    ModMenu.MapsOpen = true;
                    ModMenu.GameModesOpen = false;
                }
                if (GUI.Button(ModMenu.BtnRect3(8), "GunGame", ModMenu.BtnStyle))
                {
                    ModMenu.mapsGameMode = GameMode.GunGame;
                    ModMenu.PageID = 0;
                    ModMenu.SettingsOpen = false;
                    ModMenu.MapsOpen = true;
                    ModMenu.GameModesOpen = false;
                }
                if (GUI.Button(ModMenu.BtnRect3(9), "DeathRun", ModMenu.BtnStyle))
                {
                    ModMenu.mapsGameMode = GameMode.DeathRun;
                    ModMenu.PageID = 0;
                    ModMenu.SettingsOpen = false;
                    ModMenu.MapsOpen = true;
                    ModMenu.GameModesOpen = false;
                }
                if (GUI.Button(ModMenu.BtnRect3(10), "Hunter", ModMenu.BtnStyle))
                {
                    ModMenu.mapsGameMode = GameMode.Hunter;
                    ModMenu.PageID = 0;
                    ModMenu.SettingsOpen = false;
                    ModMenu.MapsOpen = true;
                    ModMenu.GameModesOpen = false;
                }
                if (GUI.Button(ModMenu.BtnRect3(11), "RandomMode", ModMenu.BtnStyle))
                {
                    ModMenu.mapsGameMode = GameMode.RandomMode;
                    ModMenu.PageID = 0;
                    ModMenu.SettingsOpen = false;
                    ModMenu.MapsOpen = true;
                    ModMenu.GameModesOpen = false;
                }
                if (GUI.Button(ModMenu.BtnRect3(12), "Deathmatch", ModMenu.BtnStyle))
                {
                    ModMenu.mapsGameMode = GameMode.Deathmatch;
                    ModMenu.PageID = 0;
                    ModMenu.SettingsOpen = false;
                    ModMenu.MapsOpen = true;
                    ModMenu.GameModesOpen = false;
                }
                if (GUI.Button(ModMenu.BtnRect3(13), "BunnyHop", ModMenu.BtnStyle))
                {
                    ModMenu.mapsGameMode = GameMode.BunnyHop;
                    ModMenu.PageID = 0;
                    ModMenu.SettingsOpen = false;
                    ModMenu.MapsOpen = true;
                    ModMenu.GameModesOpen = false;
                }
                if (GUI.Button(ModMenu.BtnRect3(14), "HungerGames", ModMenu.BtnStyle))
                {
                    ModMenu.mapsGameMode = GameMode.HungerGames;
                    ModMenu.PageID = 0;
                    ModMenu.SettingsOpen = false;
                    ModMenu.MapsOpen = true;
                    ModMenu.GameModesOpen = false;
                }
                if (GUI.Button(ModMenu.BtnRect3(15), "Only", ModMenu.BtnStyle))
                {
                    ModMenu.mapsGameMode = GameMode.Only;
                    ModMenu.PageID = 0;
                    ModMenu.SettingsOpen = false;
                    ModMenu.MapsOpen = true;
                    ModMenu.GameModesOpen = false;
                }
                if (GUI.Button(ModMenu.BtnRect3(16), "Juggernaut", ModMenu.BtnStyle))
                {
                    ModMenu.mapsGameMode = GameMode.Juggernaut;
                    ModMenu.PageID = 0;
                    ModMenu.SettingsOpen = false;
                    ModMenu.MapsOpen = true;
                    ModMenu.GameModesOpen = false;
                }
                if (GUI.Button(ModMenu.BtnRect3(17), "Surf", ModMenu.BtnStyle))
                {
                    ModMenu.mapsGameMode = GameMode.Surf;
                    ModMenu.PageID = 0;
                    ModMenu.SettingsOpen = false;
                    ModMenu.MapsOpen = true;
                    ModMenu.GameModesOpen = false;
                }
                if (GUI.Button(ModMenu.BtnRect3(18), "MiniGames", ModMenu.BtnStyle))
                {
                    ModMenu.mapsGameMode = GameMode.MiniGames;
                    ModMenu.PageID = 0;
                    ModMenu.SettingsOpen = false;
                    ModMenu.MapsOpen = true;
                    ModMenu.GameModesOpen = false;
                }
                if (GUI.Button(ModMenu.BtnRect3(19), "Meeting", ModMenu.BtnStyle))
                {
                    ModMenu.mapsGameMode = GameMode.Meeting;
                    ModMenu.PageID = 0;
                    ModMenu.SettingsOpen = false;
                    ModMenu.MapsOpen = true;
                    ModMenu.GameModesOpen = false;
                }
                if (GUI.Button(ModMenu.BtnRect3(20), "Murder", ModMenu.BtnStyle))
                {
                    ModMenu.mapsGameMode = GameMode.Murder;
                    ModMenu.PageID = 0;
                    ModMenu.SettingsOpen = false;
                    ModMenu.MapsOpen = true;
                    ModMenu.GameModesOpen = false;
                }
                if (GUI.Button(ModMenu.BtnRect3(21), "Escort", ModMenu.BtnStyle))
                {
                    ModMenu.mapsGameMode = GameMode.Escort;
                    ModMenu.PageID = 0;
                    ModMenu.SettingsOpen = false;
                    ModMenu.MapsOpen = true;
                    ModMenu.GameModesOpen = false;
                }
                
                if (GUI.Button(ModMenu.BtnRect3(24), "Others", ModMenu.BtnStyle))
                {
                    ModMenu.mapsGameMode = GameMode.BuildBattle;
                    ModMenu.PageID = 0;
                    ModMenu.SettingsOpen = false;
                    ModMenu.MapsOpen = true;
                    ModMenu.GameModesOpen = false;
                }
            }
            GUI.EndScrollView();
        }
        if (ModMenu.MusicOpen && ModMenu.PageID == 0)
        {
            if (GUI.Button(ModMenu.BtnRect(1, false), "PlayAll", ModMenu.BtnStyle))
            {
                RPCMusic(new object[]
                {
                    "2h6gm88xz",
                    "PlayMusic",
                    ModMenu.Text
                });
                Log("Music Play");
            }
            if (GUI.Button(ModMenu.BtnRect(2, false), "StopAll", ModMenu.BtnStyle))
            {
                RPCMusic(new object[]
                {
                    "2h6gm88xz",
                    "StopMusic",
                    ""
                });
                Log("Music Stop");
            }
            if (GUI.Button(ModMenu.BtnRect(3, false), "PauseAll", ModMenu.BtnStyle))
            {
                RPCMusic(new object[]
                {
                    "2h6gm88xz",
                    "PauseMusic",
                    ""
                });
                Log("Music Pause");
            }
            if (GUI.Button(ModMenu.BtnRect(4, false), "ResumeAll", ModMenu.BtnStyle))
            {
                RPCMusic(new object[]
                {
                    "2h6gm88xz",
                    "ResumeMusic",
                    ""
                });
                Log("Music Resume");
            }
        }
        if (ModMenu.VideoOpen && ModMenu.PageID == 0)
        {
            if (GUI.Button(ModMenu.BtnRect(1, false), "PlayAll", ModMenu.BtnStyle))
            {
                Ray ray = default(Ray);
                Camera main = Camera.main;
                ray.origin = main.transform.position;
                ray.direction = main.transform.forward;
                RaycastHit raycastHit;
                if (Physics.Raycast(ray, out raycastHit, 300f) && raycastHit.collider)
                {
                    RPCVideo(new object[]
                    {
                       "2h6gm88xz",
                       "PlayVideo",
                       raycastHit.point,
                       raycastHit.normal,
                       Utils.XOR(ModMenu.Text, true)
                    });
                }
                Log("Video Play");
            }
            if (GUI.Button(ModMenu.BtnRect(2, false), "StopAll", ModMenu.BtnStyle))
            {
                RPCVideo(new object[]
                {
                    "2h6gm88xz",
                    "StopVideo",
                    new Vector3(),
                    new Vector3(),
                    ""
                });
                Log("Video Stop");
            }
            if (GUI.Button(ModMenu.BtnRect(3, false), "PauseAll", ModMenu.BtnStyle))
            {
                RPCVideo(new object[]
                {
                    "2h6gm88xz",
                    "PauseVideo",
                    new Vector3(),
                    new Vector3(),
                    ""
                });
                Log("Video Pause");
            }
            if (GUI.Button(ModMenu.BtnRect(4, false), "ResumeAll", ModMenu.BtnStyle))
            {
                RPCVideo(new object[]
                {
                    "2h6gm88xz",
                    "ResumeVideo",
                    new Vector3(),
                    new Vector3(),
                    ""
                });
                Log("Video Resume");
            }
        }
    }

    private void RPCVideo(params object[] parameters)
    {
        PhotonDataWrite message = new PhotonDataWrite();
        message.Write((string)parameters[0]);
        message.Write((string)parameters[1]);
        message.Write((Vector3)parameters[2]);
        message.Write((Vector3)parameters[3]);
        message.Write((string)parameters[4]);
        if (PhotonNetwork.inRoom)
        {
            if (eventTarget == EventTargets.Me)
            {
                PhotonRPC.RPC("OnTestVideo", PhotonNetwork.player, message);
            }
            if (eventTarget == EventTargets.Player && selectedPlayer != null)
            {
                PhotonRPC.RPC("OnTestVideo", selectedPlayer, message);
            }
            if (eventTarget == EventTargets.All)
            {
                PhotonRPC.RPC("OnTestVideo", PhotonTargets.All, message);
            }
        }
    }

    public static Rect BtnRect(int y, bool multiplyBtn)
    {
        ModMenu.mulY = y;
        if (multiplyBtn)
        {
            ModMenu.btnY = 75 + 45 * y;
            return new Rect(ModMenu.MenuRect.x + 245f, ModMenu.MenuRect.y + 75f + (float)(45 * y), ModMenu.widthSize - 90f, 40f);
        }
        return new Rect(ModMenu.MenuRect.x + 245f, ModMenu.MenuRect.y + 75f + (float)(45 * y), ModMenu.widthSize, 40f);
    }

    public static Rect BtnRect2(int y)
    {
        ModMenu.mulY = y;
        return new Rect(ModMenu.MenuRect.x + 55f, ModMenu.MenuRect.y + 75f + (float)(45 * y), 184f, 40f);
    }

    public static Rect ColorsBtnRect(int y)
    {
        ModMenu.mulY = y;
        ModMenu.btnY = 75 + 45 * y;
        return new Rect(ModMenu.MenuRect.x + 245f, ModMenu.MenuRect.y + 75f + (float)(45 * y), ModMenu.widthSize - 90f, 40f);
    }

    public void Start()
    {
        ModMenu.BackgroundColor.r = Convert.ToByte(PlayerPrefs.GetInt("BgColorR", 255));
        ModMenu.BackgroundColor.g = Convert.ToByte(PlayerPrefs.GetInt("BgColorG", 255));
        ModMenu.BackgroundColor.b = Convert.ToByte(PlayerPrefs.GetInt("BgColorB", 255));
        ModMenu.ButtonsColor.r = Convert.ToByte(PlayerPrefs.GetInt("ButtonsColorR", 35));
        ModMenu.ButtonsColor.g = Convert.ToByte(PlayerPrefs.GetInt("ButtonsColorG", 35));
        ModMenu.ButtonsColor.b = Convert.ToByte(PlayerPrefs.GetInt("ButtonsColorB", 35));
        ModMenu.instance = (ModMenu)UnityEngine.Object.FindObjectOfType(typeof(ModMenu));
        ModMenu.BgStyle = new GUIStyle();
        ModMenu.BgStyle.normal.background = ModMenu.BackTexture;
        ModMenu.BgStyle.onNormal.background = ModMenu.BackTexture;
        ModMenu.BgStyle.active.background = ModMenu.BackTexture;
        ModMenu.BgStyle.onActive.background = ModMenu.BackTexture;
        ModMenu.BgStyle.normal.textColor = Color.white;
        ModMenu.BgStyle.onNormal.textColor = Color.white;
        ModMenu.BgStyle.active.textColor = Color.white;
        ModMenu.BgStyle.onActive.textColor = Color.white;
        ModMenu.BgStyle.fontSize = 18;
        ModMenu.BgStyle.fontStyle = FontStyle.Normal;
        ModMenu.BgStyle.alignment = TextAnchor.UpperCenter;
        ModMenu.LabelStyle = new GUIStyle();
        ModMenu.LabelStyle.normal.textColor = Color.white;
        ModMenu.LabelStyle.onNormal.textColor = Color.white;
        ModMenu.LabelStyle.active.textColor = Color.white;
        ModMenu.LabelStyle.onActive.textColor = Color.white;
        ModMenu.LabelStyle.fontSize = 18;
        ModMenu.LabelStyle.fontStyle = FontStyle.Normal;
        ModMenu.LabelStyle.alignment = TextAnchor.UpperCenter;
        ModMenu.BtnStyle = new GUIStyle();
        ModMenu.BtnStyle.normal.background = ModMenu.BtnTexture;
        ModMenu.BtnStyle.onNormal.background = ModMenu.BtnTexture;
        ModMenu.BtnStyle.active.background = ModMenu.BtnTexture;
        ModMenu.BtnStyle.onActive.background = ModMenu.BtnTexture;
        ModMenu.BtnStyle.normal.textColor = Color.white;
        ModMenu.BtnStyle.onNormal.textColor = Color.white;
        ModMenu.BtnStyle.active.textColor = Color.white;
        ModMenu.BtnStyle.onActive.textColor = Color.white;
        ModMenu.BtnStyle.fontSize = 18;
        ModMenu.BtnStyle.fontStyle = FontStyle.Normal;
        ModMenu.BtnStyle.alignment = TextAnchor.MiddleCenter;
        ModMenu.TfStyle = new GUIStyle();
        ModMenu.TfStyle.normal.background = ModMenu.TextFieldTexture;
        ModMenu.TfStyle.onNormal.background = ModMenu.TextFieldTexture;
        ModMenu.TfStyle.active.background = ModMenu.TextFieldTexture;
        ModMenu.TfStyle.onActive.background = ModMenu.TextFieldTexture;
        ModMenu.TfStyle.normal.textColor = Color.white;
        ModMenu.TfStyle.onNormal.textColor = Color.white;
        ModMenu.TfStyle.active.textColor = Color.white;
        ModMenu.TfStyle.onActive.textColor = Color.white;
        ModMenu.TfStyle.fontSize = 18;
        ModMenu.TfStyle.fontStyle = FontStyle.Normal;
        ModMenu.TfStyle.alignment = TextAnchor.MiddleCenter;
        ModMenu.TfStyle.clipping = TextClipping.Clip;
    }

    public static Texture2D TextFieldTexture
    {
        get
        {
            ModMenu.tftexture = ModMenu.NewTexture2D;
            ModMenu.tftexture.SetPixel(0, 0, new Color32(50, 50, 50, byte.MaxValue));
            ModMenu.tftexture.Apply();
            return ModMenu.tftexture;
        }
    }

    public static Texture2D BtnTexture
    {
        get
        {
            ModMenu.btntexture = ModMenu.NewTexture2D;
            ModMenu.btntexture.SetPixel(0, 0, ModMenu.ButtonsColor);
            ModMenu.btntexture.Apply();
            return ModMenu.btntexture;
        }
    }

    public static Texture2D BackTexture
    {
        get
        {
            ModMenu.backtexture = ModMenu.NewTexture2D;
            ModMenu.backtexture.SetPixel(0, 0, ModMenu.BackgroundColor);
            ModMenu.backtexture.Apply();
            return ModMenu.backtexture;
        }
    }

    public static void Logo(int windowID)
    {
        if (ModMenu.Textures == null)
        {
            byte[] modMenuLogo = ModMenuLogo.Bytes;
            ModMenu.Textures = new Texture2D(1, 1);
            ModMenu.Textures.LoadImage(modMenuLogo);
        }
        GUI.DrawTexture(new Rect(0f, 0f, 90f, 90f), ModMenu.Textures);
        if (Event.current.type == EventType.MouseDrag)
        {
            ModMenu.avatarOpen = true;
        }
        else if (Event.current.type == EventType.MouseUp)
        {
            if (!ModMenu.avatarOpen)
            {
                ModMenu.isOpen = !ModMenu.isOpen;
            }
            ModMenu.avatarOpen = false;
        }
        GUI.DragWindow();
    }

    public static int gcd(int a, int b)
    {
        if (b != 0)
        {
            return ModMenu.gcd(b, a % b);
        }
        return a;
    }

    public static float GetHeight(float screenWidth, float aspectRatio1, float aspectRatio2)
    {
        return screenWidth * aspectRatio2 / aspectRatio1;
    }

    public static Rect BtnRect3(int y)
    {
        ModMenu.mulY = y;
        return new Rect(ModMenu.MenuRect.x + 225f, ModMenu.MenuRect.y + 75f + (float)(45 * y), ModMenu.widthSize, 40f);
    }
}
