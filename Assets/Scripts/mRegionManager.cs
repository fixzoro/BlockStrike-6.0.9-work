using System.Collections;
using UnityEngine;

public class mRegionManager : MonoBehaviour
{
    public UILabel[] labels;

    public bool actived;

    public void Check()
	{
		if (this.actived)
		{
			return;
		}
		this.actived = true;
		base.StartCoroutine(this.CheckCoroutine());
	}

	private IEnumerator CheckCoroutine()
	{
		PhotonPingManager pingManager = new PhotonPingManager();
		foreach (Region region in PhotonNetwork.networkingPeer.AvailableRegions)
		{
			base.StartCoroutine(pingManager.PingSocket(region));
		}
		while (!pingManager.Done)
		{
			Debug.Log(pingManager.Done);
			yield return new WaitForSeconds(0.1f);
		}
		foreach (Region region2 in PhotonNetwork.networkingPeer.AvailableRegions)
		{
			switch (region2.Code)
			{
			case CloudRegionCode.eu:
				this.labels[0].text = region2.Ping + "ms";
				break;
			case CloudRegionCode.us:
				this.labels[1].text = region2.Ping + "ms";
				break;
			case CloudRegionCode.au:
				this.labels[5].text = region2.Ping + "ms";
				break;
			case CloudRegionCode.sa:
				this.labels[3].text = region2.Ping + "ms";
				break;
			case CloudRegionCode.kr:
				this.labels[2].text = region2.Ping + "ms";
				break;
			case CloudRegionCode.@in:
				this.labels[4].text = region2.Ping + "ms";
				break;
			}
		}
		this.actived = false;
		yield break;
	}
}
