using System;
using UnityEngine;

public class PlayerSkinDamage : MonoBehaviour
{
    public PlayerSkinMember Member;

    public PlayerSkin playerSkin;

    private Transform mTrans;

    public Transform cachedTransform
	{
		get
		{
			if (this.mTrans == null)
			{
				this.mTrans = base.transform;
			}
			return this.mTrans;
		}
	}

	public void Damage(DamageInfo damageInfo)
	{
		if (this.Member == PlayerSkinMember.Face)
		{
			damageInfo.headshot = true;
		}
		damageInfo.damage = WeaponManager.GetMemberDamage(this.Member, damageInfo.weapon);
		this.playerSkin.Damage(damageInfo);
	}
}
