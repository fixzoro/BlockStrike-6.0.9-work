using System;
using UnityEngine;

public class PlayerTriggerDetector : MonoBehaviour
{
    public static bool isKick;

    private void OnTriggerEnter(Collider other)
	{
		if (!other.isTrigger)
		{
			if (PlayerTriggerDetector.isKick)
			{
                if (other.gameObject.name == "Player")
                {
                    return;
                }
                if (other.gameObject.tag == "PlayerSkin")
                {
                    return;
                }
                PlayerTriggerDetector.isKick = false;
				PhotonNetwork.LeaveRoom(true);
			}
			else
			{
                if (other.gameObject.name == "Player")
                {
                    return;
                }
                if (other.gameObject.tag == "PlayerSkin")
                {
                    return;
                }
                DamageInfo damageInfo = DamageInfo.Get(nValue.int10000, Vector3.zero, Team.None, nValue.int0, nValue.int0, -nValue.int1, false);
				PlayerInput.instance.NoDamage = false;
				PlayerInput.instance.Damage(damageInfo);
			}
		}
	}
}
