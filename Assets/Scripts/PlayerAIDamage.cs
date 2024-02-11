using System;
using UnityEngine;

public class PlayerAIDamage : MonoBehaviour
{
    public PlayerSkinMember Member;

    public PlayerAI playerAI;

    private void Damage(DamageInfo damageInfo)
	{
		if (this.Member == PlayerSkinMember.Face)
		{
			damageInfo.headshot = true;
		}
		damageInfo.damage = WeaponManager.GetMemberDamage(this.Member, damageInfo.weapon);
		this.playerAI.Damage(damageInfo);
	}
}
