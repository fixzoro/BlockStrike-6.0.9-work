using System;
using UnityEngine;

public class mFriendsElement : MonoBehaviour
{
    public UIWidget Widget;

    public UILabel Name;

    public UISprite StatusSprite;

    public UILabel InRoomLabel;

    private int playerID;

    public FriendInfo info;

    public int ID
	{
		get
		{
			return this.playerID;
		}
	}

	public void SetData(int id)
	{
		if (!this.Widget.cachedGameObject.activeSelf)
		{
			return;
		}
		this.playerID = id;
		string @string = CryptoPrefs.GetString("Friend_#" + id.ToString(), "#" + id.ToString());
		this.Name.text = @string;
		if (PhotonNetwork.Friends != null)
		{
			for (int i = 0; i < PhotonNetwork.Friends.Count; i++)
			{
				if (PhotonNetwork.Friends[i].UserId == @string)
				{
					this.info = PhotonNetwork.Friends[i];
					this.StatusSprite.cachedGameObject.SetActive(true);
					this.StatusSprite.color = ((!this.info.IsOnline) ? new Color32(122, 122, 122, byte.MaxValue) : new Color32(79, 181, 82, byte.MaxValue));
					if (this.info.IsInRoom)
					{
						RoomInfo room = this.GetRoom(this.info.Room);
						if (room.isOfficialServer())
						{
							this.InRoomLabel.text = ((room != null) ? string.Concat(new object[]
							{
								room.Name.Replace("off", Localization.Get("Official Servers", true)),
								" - ",
								Localization.Get(room.GetGameMode().ToString(), true),
								" - ",
								room.GetSceneName(),
								" - ",
								room.PlayerCount,
								"/",
								room.MaxPlayers
							}) : string.Empty);
						}
						else
						{
							this.InRoomLabel.text = ((room != null) ? string.Concat(new object[]
							{
								room.Name,
								" - ",
								Localization.Get(room.GetGameMode().ToString(), true),
								" - ",
								room.GetSceneName(),
								" - ",
								room.PlayerCount,
								"/",
								room.MaxPlayers
							}) : string.Empty);
						}
					}
					else
					{
						this.InRoomLabel.text = string.Empty;
					}
					base.name = ((!this.info.IsOnline) ? ("1-" + this.info.UserId) : "0");
				}
			}
		}
		else
		{
			this.StatusSprite.cachedGameObject.SetActive(false);
		}
	}

	public void UpdateStatus(FriendInfo info)
	{
		if (info.UserId != this.Name.text)
		{
			return;
		}
		if (!this.Widget.cachedGameObject.activeSelf)
		{
			return;
		}
		this.StatusSprite.cachedGameObject.SetActive(true);
		this.StatusSprite.color = ((!info.IsOnline) ? new Color32(122, 122, 122, byte.MaxValue) : new Color32(79, 181, 82, byte.MaxValue));
		if (info.IsInRoom)
		{
			RoomInfo room = this.GetRoom(info.Room);
			if (room.isOfficialServer())
			{
				this.InRoomLabel.text = ((room != null) ? string.Concat(new object[]
				{
					room.Name.Replace("off", Localization.Get("Official Servers", true)),
					" - ",
					Localization.Get(room.GetGameMode().ToString(), true),
					" - ",
					room.GetSceneName(),
					" - ",
					room.PlayerCount,
					"/",
					room.MaxPlayers
				}) : string.Empty);
			}
			else
			{
				this.InRoomLabel.text = ((room != null) ? string.Concat(new object[]
				{
					room.Name,
					" - ",
					Localization.Get(room.GetGameMode().ToString(), true),
					" - ",
					room.GetSceneName(),
					" - ",
					room.PlayerCount,
					"/",
					room.MaxPlayers
				}) : string.Empty);
			}
		}
		else
		{
			this.InRoomLabel.text = string.Empty;
		}
		base.name = ((!info.IsOnline) ? ("1-" + info.UserId) : "0");
	}

	private RoomInfo GetRoom(string name)
	{
		RoomInfo[] roomList = PhotonNetwork.GetRoomList();
		for (int i = 0; i < roomList.Length; i++)
		{
			if (roomList[i].Name == name)
			{
				return roomList[i];
			}
		}
		return null;
	}
}
