using System;
using UnityEngine;

public class ShootingRangeTargetDamage : MonoBehaviour
{
    public PlayerSkinMember Member;

    public ShootingRangeTarget Target;

    private void Damage(DamageInfo damageInfo)
	{
		if (this.Member == PlayerSkinMember.Face)
		{
			damageInfo.headshot = true;
		}
		damageInfo.damage = WeaponManager.GetMemberDamage(this.Member, damageInfo.weapon);
		if (this.Target.GetActive())
		{
			UICrosshair.Hit();
		}
		this.Target.Damage(damageInfo);
		if (ShootingRangeManager.ShowDamage)
		{
			UIToast.Show(Localization.Get("Damage", true) + ": " + damageInfo.damage, 2f);
		}
	}
}
