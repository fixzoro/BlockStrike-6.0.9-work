using System;
using System.Collections.Generic;
using UnityEngine;

public static class vp_AudioUtility
{
	public static void PlayRandomSound(AudioSource audioSource, List<AudioClip> sounds, Vector2 pitchRange)
	{
		if (audioSource == null)
		{
			return;
		}
		if (sounds == null || sounds.Count == 0)
		{
			return;
		}
		AudioClip audioClip = sounds[UnityEngine.Random.Range(0, sounds.Count)];
		if (audioClip == null)
		{
			return;
		}
		if (pitchRange == Vector2.one)
		{
			audioSource.pitch = Time.timeScale;
		}
		else
		{
			audioSource.pitch = UnityEngine.Random.Range(pitchRange.x, pitchRange.y) * Time.timeScale;
		}
		audioSource.PlayOneShot(audioClip);
	}

	public static void PlayRandomSound(AudioSource audioSource, List<AudioClip> sounds)
	{
		vp_AudioUtility.PlayRandomSound(audioSource, sounds, Vector2.one);
	}
}
