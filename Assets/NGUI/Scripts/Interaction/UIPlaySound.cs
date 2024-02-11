using System;
using UnityEngine;

// Token: 0x02000067 RID: 103
[AddComponentMenu("NGUI/Interaction/Play Sound")]
public class UIPlaySound : MonoBehaviour
{
	// Token: 0x1700001F RID: 31
	// (get) Token: 0x060002E6 RID: 742 RVA: 0x00029CFC File Offset: 0x00027EFC
	private bool canPlay
	{
		get
		{
			if (!base.enabled)
			{
				return false;
			}
			UIButton component = base.GetComponent<UIButton>();
			return component == null || component.isEnabled;
		}
	}

	// Token: 0x060002E7 RID: 743 RVA: 0x00006F03 File Offset: 0x00005103
	private void OnEnable()
	{
		if (this.trigger == UIPlaySound.Trigger.OnEnable)
		{
			NGUITools.PlaySound(this.audioClip, this.volume, this.pitch);
		}
	}

	// Token: 0x060002E8 RID: 744 RVA: 0x00006F29 File Offset: 0x00005129
	private void OnDisable()
	{
		if (this.trigger == UIPlaySound.Trigger.OnDisable)
		{
			NGUITools.PlaySound(this.audioClip, this.volume, this.pitch);
		}
	}

	// Token: 0x060002E9 RID: 745 RVA: 0x00029D34 File Offset: 0x00027F34
	private void OnHover(bool isOver)
	{
		if (this.trigger == UIPlaySound.Trigger.OnMouseOver)
		{
			if (this.mIsOver == isOver)
			{
				return;
			}
			this.mIsOver = isOver;
		}
		if (this.canPlay && ((isOver && this.trigger == UIPlaySound.Trigger.OnMouseOver) || (!isOver && this.trigger == UIPlaySound.Trigger.OnMouseOut)))
		{
			NGUITools.PlaySound(this.audioClip, this.volume, this.pitch);
		}
	}

	// Token: 0x060002EA RID: 746 RVA: 0x00029DA8 File Offset: 0x00027FA8
	private void OnPress(bool isPressed)
	{
		if (this.trigger == UIPlaySound.Trigger.OnPress)
		{
			if (this.mIsOver == isPressed)
			{
				return;
			}
			this.mIsOver = isPressed;
		}
		if (this.canPlay && ((isPressed && this.trigger == UIPlaySound.Trigger.OnPress) || (!isPressed && this.trigger == UIPlaySound.Trigger.OnRelease)))
		{
			NGUITools.PlaySound(this.audioClip, this.volume, this.pitch);
		}
	}

	// Token: 0x060002EB RID: 747 RVA: 0x00006F4F File Offset: 0x0000514F
	private void OnClick()
	{
		if (this.canPlay && this.trigger == UIPlaySound.Trigger.OnClick)
		{
			NGUITools.PlaySound(this.audioClip, this.volume, this.pitch);
		}
	}

	// Token: 0x060002EC RID: 748 RVA: 0x00006F7F File Offset: 0x0000517F
	private void OnSelect(bool isSelected)
	{
		if (this.canPlay && (!isSelected || UICamera.currentScheme == UICamera.ControlScheme.Controller))
		{
			this.OnHover(isSelected);
		}
	}

	// Token: 0x060002ED RID: 749 RVA: 0x00006FA4 File Offset: 0x000051A4
	public void Play()
	{
		NGUITools.PlaySound(this.audioClip, this.volume, this.pitch);
	}

	// Token: 0x040001F7 RID: 503
	public AudioClip audioClip;

	// Token: 0x040001F8 RID: 504
	public UIPlaySound.Trigger trigger;

	// Token: 0x040001F9 RID: 505
	[Range(0f, 1f)]
	public float volume = 1f;

	// Token: 0x040001FA RID: 506
	[Range(0f, 2f)]
	public float pitch = 1f;

	// Token: 0x040001FB RID: 507
	private bool mIsOver;

	// Token: 0x02000068 RID: 104
	[DoNotObfuscateNGUI]
	public enum Trigger
	{
		// Token: 0x040001FD RID: 509
		OnClick,
		// Token: 0x040001FE RID: 510
		OnMouseOver,
		// Token: 0x040001FF RID: 511
		OnMouseOut,
		// Token: 0x04000200 RID: 512
		OnPress,
		// Token: 0x04000201 RID: 513
		OnRelease,
		// Token: 0x04000202 RID: 514
		Custom,
		// Token: 0x04000203 RID: 515
		OnEnable,
		// Token: 0x04000204 RID: 516
		OnDisable
	}
}
