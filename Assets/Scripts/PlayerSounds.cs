using System;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    public AudioSource Source;

    public AudioClip FireKnife;

    public AudioClip FirePistol;

    public AudioClip FirePistol2;

    public AudioClip FirePistol3;

    public AudioClip FirePistol4;

    public AudioClip FirePistol5;

    public AudioClip FireRifle;

    public AudioClip FireRifle2;

    public AudioClip FireShotgun;

    public AudioClip FireSniper;

    public AudioClip FireSubMachineGun;

    public AudioClip ReloadPistol;

    public AudioClip ReloadRifle;

    public AudioClip AmmoEmpty;

    private WeaponSound LastSound = WeaponSound.None;

    private bool Sound = true;

    private void Start()
	{
		this.UpdateSettings();
		EventManager.AddListener("OnSettings", new EventManager.Callback(this.UpdateSettings));
	}

	public void OnDefault()
	{
		this.LastSound = WeaponSound.None;
		this.Source.Stop();
	}

	private void UpdateSettings()
	{
		this.Sound = Settings.Sound;
	}

	private void OnDisable()
	{
		this.Stop();
	}

	public void Play(WeaponSound sound)
	{
		this.Play(sound, 1f);
	}

	public void Play(WeaponSound sound, float volume)
	{
		nProfiler.BeginSample("PlayerSounds.Play");
		if (!this.Sound)
		{
			return;
		}
		if (this.LastSound != sound)
		{
			switch (sound)
			{
			case WeaponSound.Knife:
				this.Source.clip = this.FireKnife;
				break;
			case WeaponSound.Pistol:
				this.Source.clip = this.FirePistol;
				break;
			case WeaponSound.Pistol2:
				this.Source.clip = this.FirePistol2;
				break;
			case WeaponSound.Pistol3:
				this.Source.clip = this.FirePistol3;
				break;
			case WeaponSound.Pistol4:
				this.Source.clip = this.FirePistol4;
				break;
			case WeaponSound.Pistol5:
				this.Source.clip = this.FirePistol5;
				break;
			case WeaponSound.Rifle:
				this.Source.clip = this.FireRifle;
				break;
			case WeaponSound.Rifle2:
				this.Source.clip = this.FireRifle2;
				break;
			case WeaponSound.Shotgun:
				this.Source.clip = this.FireShotgun;
				break;
			case WeaponSound.Sniper:
				this.Source.clip = this.FireSniper;
				break;
			case WeaponSound.SubMachineGun:
				this.Source.clip = this.FireSubMachineGun;
				break;
			case WeaponSound.ReloadPistol:
				this.Source.PlayOneShot(this.ReloadPistol, volume);
				break;
			case WeaponSound.ReloadRifle:
				this.Source.PlayOneShot(this.ReloadRifle, volume);
				break;
			case WeaponSound.AmmoEmpty:
				this.Source.clip = this.AmmoEmpty;
				break;
			}
		}
		this.LastSound = sound;
		this.Source.volume = volume;
		if (sound != WeaponSound.ReloadPistol && sound != WeaponSound.ReloadRifle && sound != WeaponSound.None)
		{
			this.Source.Play();
		}
		nProfiler.EndSample();
	}

	public void Stop()
	{
		try
		{
			this.LastSound = WeaponSound.None;
			this.Source.Stop();
		}
		catch
		{
		}
	}
}
