using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class UIStatus : MonoBehaviour
{
    public UILabel label;

    public UISprite background;

    private List<int> list = new List<int>();

    private static UIStatus instance;

    private void Start()
    {
        UIStatus.instance = this;
        PhotonRPC.AddMessage("PhotonStatusNewLine", new PhotonRPC.MessageDelegate(this.PhotonStatusNewLine));
    }

    private void UpdateBackground()
    {
        if (string.IsNullOrEmpty(this.label.text))
        {
            this.background.alpha = 0f;
        }
        else
        {
            this.background.height = this.label.height + 6;
            this.background.alpha = 0.2f;
        }
        this.background.UpdateWidget();
    }

    public static void Add(string text)
    {
        UIStatus.Add(text, false, string.Empty);
    }

    public static void Add(string text, bool local)
    {
        UIStatus.Add(text, local, string.Empty);
    }

    public static void Add(string text, bool local, string localize)
    {
        if (local)
        {
            UIStatus.NewLine(text);
        }
        else
        {
            PhotonDataWrite data = PhotonRPC.GetData();
            if (string.IsNullOrEmpty(localize))
            {
                data.Write((byte)3);
                data.Write(text);
                PhotonRPC.RPC("PhotonStatusNewLine", PhotonTargets.All, data);
            }
            else
            {
                data.Write((byte)4);
                data.Write(text);
                data.Write(localize);
                PhotonRPC.RPC("PhotonStatusNewLine", PhotonTargets.All, data);
            }
        }
    }

    public static void Add(DamageInfo damageInfo)
    {
        PhotonDataWrite data = PhotonRPC.GetData();
        if (damageInfo.otherPlayer)
        {
            data.Write((byte)2);
            data.Write(damageInfo.player);
            data.Write(PhotonNetwork.player.ID);
            data.Write((byte)damageInfo.weapon);
            data.Write(damageInfo.headshot);
            PhotonRPC.RPC("PhotonStatusNewLine", PhotonTargets.All, data);
        }
        else
        {
            data.Write((byte)1);
            data.Write(PhotonNetwork.player.ID);
            PhotonRPC.RPC("PhotonStatusNewLine", PhotonTargets.All, data);
        }
    }

    public static void DisconnectPlayer(PhotonPlayer player)
    {
        UIStatus.NewLine(UIStatus.GetTeamHexColor(player) + " " + Localization.Get("Disconnect", true));
    }

    public static void ConnectPlayer(PhotonPlayer player)
    {
        UIStatus.NewLine(player.UserId + " " + Localization.Get("Connected", true));
    }

    [PunRPC]
    private void PhotonStatusNewLine(PhotonMessage message)
    {
        byte b = message.ReadByte();
        string text = string.Empty;
        bool flag = false;
        switch (b)
        {
            case 1:
                text = GetTeamHexColor(PhotonPlayer.Find(message.ReadInt())) + " " + Localization.Get("died");
                break;
            case 2:
                text = KillerStatus(message.ReadInt(), message.ReadInt(), message.ReadByte(), message.ReadBool());
                break;
            case 3:
                text = message.ReadString();
                flag = text[0] == '.';
                if (flag)
                {
                    text = text.Remove(0, 1);
                }
                break;
            case 4:
                {
                    text = message.ReadString();
                    string newValue = Localization.Get(message.ReadString());
                    text = text.Replace("[@]", newValue);
                    if (flag)
                    {
                        text = text.Remove(0, 1);
                    }
                    break;
                }
        }
        if (flag)
        {
            if (PhotonNetwork.player.GetTeam() == message.sender.GetTeam())
            {
                text = text.Remove(0, 1);
                NewLine(text);
            }
        }
        else
        {
            NewLine(text);
        }
    }

    public static void NewLine(string text)
    {
        UIStatus.instance.List.Add(text);
        UIStatus.instance.UpdateLabel(true);
    }

    private void UpdateLabel(bool clear)
    {
        string text = string.Empty;
        for (int i = 0; i < this.List.Count; i++)
        {
            if (i > 0)
            {
                text += "\n";
            }
            text += this.List[this.List.Count - 1 - i];
        }
        this.label.text = text;
        if (string.IsNullOrEmpty(text))
        {
            this.background.alpha = 0f;
        }
        else
        {
            this.background.alpha = 0.1f;
            this.background.UpdateAnchors();
        }
        if (clear)
        {
            TimerManager.In(5f, new TimerManager.Callback(this.RemoveLabel));
        }
    }

    private List<string> List = new List<string>();

    private void RemoveLabel()
    {
        this.List.RemoveAt(0);
        this.UpdateLabel(false);
    }

    private string KillerStatus(int killerID, int death, int weapon, bool headshot)
    {
        string empty = string.Empty;
        PhotonPlayer player = PhotonPlayer.Find(killerID);
        PhotonPlayer player2 = PhotonPlayer.Find(death);
        string teamHexColor = UIStatus.GetTeamHexColor(player, PhotonPlayer.Find(PlayerInput.PlayerHelperID));
        return string.Concat(new string[]
        {
            teamHexColor,
            "   ",
            UIStatus.GetSpecialSymbol(weapon),
            (!headshot) ? string.Empty : ("   " + UIStatus.GetSpecialSymbol(99)),
            "   ",
            UIStatus.GetTeamHexColor(player2)
        });
    }

    public static string GetTeamHexColor(PhotonPlayer player)
    {
        return UIStatus.GetTeamHexColor(player.UserId, player.GetTeam());
    }

    public static string GetTeamHexColor(PhotonPlayer player, PhotonPlayer helper)
    {
        return UIStatus.GetTeamHexColor(player.UserId, player.GetTeam());
    }

    public static string GetTeamHexColor(string text, Team team)
    {
        if (team == Team.Blue)
        {
            text = "[4688E7]" + text + "[-]";
        }
        else if (team == Team.Red)
        {
            text = "[ED2C2D]" + text + "[-]";
        }
        return text;
    }

    public static string GetSpecialSymbol(int weapon)
    {
        switch (weapon)
        {
            case 1:
                return 'ࠀ'.ToString();
            case 2:
                return 'ࠁ'.ToString();
            case 3:
                return 'ࠂ'.ToString();
            case 4:
                return 'ࠃ'.ToString();
            case 5:
                return 'ࠄ'.ToString();
            case 6:
                return 'ࠅ'.ToString();
            case 7:
                return 'ࠆ'.ToString();
            case 8:
                return 'ࠇ'.ToString();
            case 9:
                return 'ࠈ'.ToString();
            case 10:
                return 'ࠉ'.ToString();
            case 11:
                return 'ࠊ'.ToString();
            case 12:
                return 'ࠋ'.ToString();
            case 13:
                return 'ࠌ'.ToString();
            case 14:
                return 'ࠍ'.ToString();
            case 15:
                return 'ࠎ'.ToString();
            case 16:
                return 'ࠏ'.ToString();
            case 17:
                return '࠯'.ToString();
            case 18:
                return 'ࠐ'.ToString();
            case 19:
                return 'ࠑ'.ToString();
            case 21:
                return 'ࠒ'.ToString();
            case 22:
                return 'ࠓ'.ToString();
            case 23:
                return 'ࠔ'.ToString();
            case 24:
                return 'ࠕ'.ToString();
            case 25:
                return 'ࠖ'.ToString();
            case 26:
                return 'ࠗ'.ToString();
            case 27:
                return '࠘'.ToString();
            case 28:
                return '࠙'.ToString();
            case 29:
                return 'ࠚ'.ToString();
            case 30:
                return 'ࠛ'.ToString();
            case 31:
                return 'ࠜ'.ToString();
            case 32:
                return 'ࠝ'.ToString();
            case 33:
                return 'ࠞ'.ToString();
            case 34:
                return 'ࠟ'.ToString();
            case 35:
                return 'ࠠ'.ToString();
            case 36:
                return 'ࠡ'.ToString();
            case 37:
                return 'ࠢ'.ToString();
            case 38:
                return 'ࠣ'.ToString();
            case 39:
                return 'ࠤ'.ToString();
            case 40:
                return 'ࠥ'.ToString();
            case 41:
                return 'ࠦ'.ToString();
            case 42:
                return 'ࠧ'.ToString();
            case 43:
                return 'ࠨ'.ToString();
            case 44:
                return 'ࠩ'.ToString();
            case 45:
                return 'ࠪ'.ToString();
            case 46:
                return 'ࠫ'.ToString();
            case 48:
                return 'ࠬ'.ToString();
            case 49:
                return '࠭'.ToString();
            case 50:
                return '࠮'.ToString();
            case 51:
                return '࠰'.ToString();
            case 98:
                return '࡞'.ToString();
            case 99:
                return '࡟'.ToString();
        }
        return string.Empty;
    }
}

