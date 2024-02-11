using UnityEngine;

public class SoundClip : MonoBehaviour
{
    private GameObject cachedGameObject;

    private Transform cachedTransform;

    private AudioSource cachedAudioSource;

    private void Awake()
	{
		this.cachedGameObject = base.gameObject;
		this.cachedTransform = base.transform;
		this.cachedAudioSource = base.GetComponent<AudioSource>();
	}

	public void Play(AudioClip clip, Vector3 pos)
	{
		this.cachedGameObject.SetActive(true);
		this.cachedAudioSource.clip = clip;
		this.cachedTransform.position = pos;
		this.cachedAudioSource.Play();
		TimerManager.In(clip.length, new TimerManager.Callback(this.Stop));
	}

	public void Stop()
	{
		if (!this.cachedGameObject.activeSelf)
		{
			return;
		}
		this.cachedGameObject.SetActive(false);
		SoundManager.AddSoundClipPool(this);
	}

	public static SoundClip Create()
	{
		GameObject gameObject = new GameObject();
		gameObject.name = "SoundClip";
		gameObject.AddComponent<AudioSource>();
		return gameObject.AddComponent<SoundClip>();
	}

	public AudioSource GetSource()
	{
		return this.cachedAudioSource;
	}
}
