using System;
using UnityEngine;

public class mClanPlayerElement : MonoBehaviour
{
    public UIWidget Widget;

    public UILabel Name;

    private int playerID;

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
	}
}
