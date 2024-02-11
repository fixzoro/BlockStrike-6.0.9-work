using UnityEngine;

public class mWeaponSkinsShow : MonoBehaviour
{
    public GameObject Button;

    private void Start()
	{
		EventManager.AddListener("AccountUpdate", new EventManager.Callback(this.Check));
		this.Check();
	}

	private void Check()
	{

	}

	public void Load()
	{
		if (!AccountManager.isConnect)
		{
			UIToast.Show(Localization.Get("Connection account", true));
			return;
		}
		if (PhotonNetwork.connected)
		{
			PhotonNetwork.Disconnect();
		}
		LevelManager.LoadLevel("WeaponSkinsShow");
	}
}
