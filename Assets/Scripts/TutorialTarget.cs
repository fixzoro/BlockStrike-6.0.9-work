using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class TutorialTarget : MonoBehaviour
{
    public Vector3 DefaultPosition;

    public Vector3 ActivePosition;

    public float Duration = 1f;

    public UnityEvent DamageCallback;

    private bool Activated;

    public void SetActive(bool active)
	{
		this.Activated = active;
		Vector3 endValue = (!this.Activated) ? this.DefaultPosition : this.ActivePosition;
		base.transform.DOLocalRotate(endValue, this.Duration, RotateMode.Fast);
	}

	public bool GetActive()
	{
		return this.Activated;
	}

	private void Damage(DamageInfo damageInfo)
	{
		if (this.Activated)
		{
			this.DamageCallback.Invoke();
		}
	}
}
