using UnityEngine;

public class mFireSounds : MonoBehaviour
{
    public PlayerSounds sounds;

    public float volume = 1f;

    private void Start()
	{
		TimerManager.In(1f, new TimerManager.Callback(this.Fire));
	}

	private void Fire()
	{
		int num = UnityEngine.Random.Range(0, 8);
		float num2 = 0f;
		switch (num)
		{
		case 0:
			num2 = UnityEngine.Random.Range(0.095f, 0.1f);
			TimerManager.In(num2, UnityEngine.Random.Range(3, 8), num2, delegate()
			{
				this.sounds.Play(WeaponSound.Rifle, this.volume);
			});
			break;
		case 1:
			num2 = UnityEngine.Random.Range(0.85f, 0.095f);
			TimerManager.In(num2, UnityEngine.Random.Range(3, 8), num2, delegate()
			{
				this.sounds.Play(WeaponSound.Rifle2, this.volume);
			});
			break;
		case 2:
			num2 = UnityEngine.Random.Range(0.1f, 0.15f);
			TimerManager.In(num2, UnityEngine.Random.Range(1, 5), num2, delegate()
			{
				this.sounds.Play(WeaponSound.Pistol, this.volume);
			});
			break;
		case 3:
			num2 = UnityEngine.Random.Range(0.1f, 0.15f);
			TimerManager.In(num2, UnityEngine.Random.Range(1, 5), num2, delegate()
			{
				this.sounds.Play(WeaponSound.Pistol2, this.volume);
			});
			break;
		case 4:
			num2 = UnityEngine.Random.Range(0.1f, 0.15f);
			TimerManager.In(num2, UnityEngine.Random.Range(1, 5), num2, delegate()
			{
				this.sounds.Play(WeaponSound.Pistol3, this.volume);
			});
			break;
		case 5:
			num2 = UnityEngine.Random.Range(0.1f, 0.15f);
			TimerManager.In(num2, UnityEngine.Random.Range(1, 5), num2, delegate()
			{
				this.sounds.Play(WeaponSound.Pistol4, this.volume);
			});
			break;
		case 6:
			num2 = UnityEngine.Random.Range(0.1f, 0.15f);
			TimerManager.In(num2, UnityEngine.Random.Range(1, 5), num2, delegate()
			{
				this.sounds.Play(WeaponSound.Pistol5, this.volume);
			});
			break;
		case 7:
			num2 = UnityEngine.Random.Range(0.2f, 0.35f);
			TimerManager.In(num2, UnityEngine.Random.Range(1, 3), num2, delegate()
			{
				this.sounds.Play(WeaponSound.Shotgun, this.volume);
			});
			break;
		case 8:
			TimerManager.In(num2, delegate()
			{
				this.sounds.Play(WeaponSound.Sniper, this.volume);
			});
			break;
		}
		if (UnityEngine.Random.value > 0.75f)
		{
			TimerManager.In(UnityEngine.Random.Range(0.7f, 1f), new TimerManager.Callback(this.Fire));
		}
		else
		{
			TimerManager.In(UnityEngine.Random.Range(1f, 2.5f), new TimerManager.Callback(this.Fire));
		}
	}
}
