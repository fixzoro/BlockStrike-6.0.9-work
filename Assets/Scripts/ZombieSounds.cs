using System;
using UnityEngine;

public class ZombieSounds : MonoBehaviour
{
    public AudioClip[] Clips;

    public AudioSource Source;

    public Vector2 size = new Vector2(5f, 10f);

    private int id;

    private void OnEnable()
	{
		this.PlaySound();
	}

	private void OnDisable()
	{
		TimerManager.Cancel(this.id);
		this.Source.Stop();
	}

	private void PlaySound()
	{
		this.id = TimerManager.In(UnityEngine.Random.Range(this.size.x, this.size.y), -nValue.int1, UnityEngine.Random.Range(this.size.x, this.size.y), delegate()
		{
			if (!this.Source.isPlaying && GameManager.roundState != RoundState.EndRound)
			{
				AudioClip clip = this.Clips[UnityEngine.Random.Range(nValue.int0, this.Clips.Length)];
				this.Source.clip = clip;
				this.Source.Play();
			}
		});
	}
}
