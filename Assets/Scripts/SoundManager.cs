using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public SoundManager.SoundGameMode[] soundsMode;

    private SoundManager.SoundGameMode selectSoundsMode;

    private List<SoundClip> activeList = new List<SoundClip>();

    private List<SoundClip> poolList = new List<SoundClip>();

    private AudioSource cachedAudioSource;

    private static SoundManager instance;

    private void Awake()
	{
		SoundManager.instance = this;
		this.cachedAudioSource = base.GetComponent<AudioSource>();
	}

	private void Start()
	{
		GameMode gameMode = PhotonNetwork.room.GetGameMode();
		for (int i = 0; i < this.soundsMode.Length; i++)
		{
			if (this.soundsMode[i].mode != gameMode)
			{
				for (int j = 0; j < this.soundsMode[i].sounds.Length; j++)
				{
					for (int k = 0; k < this.soundsMode[i].sounds[j].clips.Length; k++)
					{
						Resources.UnloadAsset(this.soundsMode[i].sounds[j].clips[k]);
						this.soundsMode[i].sounds[j].clips[k] = null;
					}
				}
			}
			else
			{
				this.selectSoundsMode = this.soundsMode[i];
			}
		}
	}

	public static void Play2D(string name)
	{
		if (SoundManager.instance.selectSoundsMode == null)
		{
			Debug.LogWarning("Select Sounds == null");
			return;
		}
		for (int i = 0; i < SoundManager.instance.selectSoundsMode.sounds.Length; i++)
		{
			if (SoundManager.instance.selectSoundsMode.sounds[i].name == name)
			{
				SoundManager.instance.cachedAudioSource.clip = SoundManager.instance.selectSoundsMode.sounds[i].clip;
				SoundManager.instance.cachedAudioSource.Play();
				break;
			}
		}
	}

	public static SoundClip Play3D(string name, Vector3 pos)
	{
		if (SoundManager.instance.selectSoundsMode == null)
		{
			Debug.LogWarning("Select Sounds == null");
			return null;
		}
		for (int i = 0; i < SoundManager.instance.selectSoundsMode.sounds.Length; i++)
		{
			if (SoundManager.instance.selectSoundsMode.sounds[i].name == name)
			{
				SoundClip soundClip = SoundManager.GetSoundClip();
				SoundManager.instance.activeList.Add(soundClip);
				soundClip.Play(SoundManager.instance.selectSoundsMode.sounds[i].clip, pos);
				return soundClip;
			}
		}
		return null;
	}

	private static SoundClip GetSoundClip()
	{
		if (SoundManager.instance.poolList.Count != 0)
		{
			SoundClip result = SoundManager.instance.poolList[0];
			SoundManager.instance.poolList.RemoveAt(0);
			return result;
		}
		return SoundClip.Create();
	}

	public static void AddSoundClipPool(SoundClip clip)
	{
		SoundManager.instance.activeList.Remove(clip);
		SoundManager.instance.poolList.Add(clip);
	}

	[Serializable]
	public class SoundGameMode
	{
		public GameMode mode;

		public SoundManager.SoundData[] sounds;
	}

	[Serializable]
	public class SoundData
	{
		public AudioClip clip
		{
			get
			{
				return this.clips[UnityEngine.Random.Range(0, this.clips.Length)];
			}
		}

		public string name;

		public AudioClip[] clips;
	}
}
