using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class UIDamage : MonoBehaviour
{
    public Transform Arrow;

    public UISprite ArrowSprite;

    public float Duration = 1f;

    public float ArrowAngleOffset;

    [Disabled]
    public Vector3 AttackPosition;

    [Disabled]
    public Transform Player;

    [Disabled]
    public float LookAtAngle;

    private Tweener cachedTweener;

    private static UIDamage instance;

    private void Start()
	{
		UIDamage.instance = this;
	}

	public static void Damage(Vector3 position, Transform playerCamera)
	{
		UIDamage.instance.AttackPosition = position;
		UIDamage.instance.Player = playerCamera;
		UIDamage.instance.UpdateDamage();
		UIDamage.instance.ArrowSprite.alpha = 1f;
		if (UIDamage.instance.cachedTweener == null)
		{
			UIDamage.instance.cachedTweener = DOTween.To(() => UIDamage.instance.ArrowSprite.alpha, delegate(float x)
			{
				UIDamage.instance.ArrowSprite.alpha = x;
			}, 0f, UIDamage.instance.Duration).OnUpdate(new TweenCallback(UIDamage.instance.UpdateDamage)).SetAutoKill(false);
		}
		else
		{
			UIDamage.instance.cachedTweener.ChangeStartValue(UIDamage.instance.ArrowSprite.alpha, -1f).Restart(true, -1f);
		}
	}

	private void UpdateDamage()
	{
		Vector3 rhs = this.AttackPosition - this.Player.position;
		rhs.y = 0f;
		rhs.Normalize();
		Vector3 forward = this.Player.forward;
		float num = Vector3.Dot(forward, rhs);
		if (Vector3.Cross(forward, rhs).y > 0f)
		{
			this.LookAtAngle = (1f - num) * -90f;
		}
		else
		{
			this.LookAtAngle = (1f - num) * 90f;
		}
		this.LookAtAngle += this.ArrowAngleOffset;
		Vector3 localEulerAngles = this.Arrow.localEulerAngles;
		localEulerAngles.z = this.LookAtAngle;
		this.Arrow.localEulerAngles = localEulerAngles;
	}
}
