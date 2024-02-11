using System;
using DG.Tweening;
using UnityEngine;

public class BombAudio : MonoBehaviour
{
    public AudioSource BombAudioSource;

    public AudioClip BombAudioClip;

    public Light BombLight;

    private int BombAudioID;

    private float BombTime;

    private int BombCount;

    private void OnEnable()
	{
		if (BombManager.BombPlaced)
		{
			this.BombLight.intensity = (float)nValue.int0;
			this.BombLight.DOIntensity((float)nValue.int8, nValue.float07).SetLoops(-nValue.int1, LoopType.Yoyo);
		}
	}

	private void OnDisable()
	{
		this.BombLight.DOKill(false);
	}

	public void Play(float time)
	{
		this.BombTime = time;
	}

	public void Boom()
	{
		this.Stop();
	}

	public void Stop()
	{
		this.BombCount = nValue.int0;
		this.BombTime = (float)(-(float)nValue.int1);
		TimerManager.Cancel(this.BombAudioID);
		this.BombAudioID = nValue.int0;
	}

	private void Update()
	{
		if (this.BombTime > nValue.float08)
		{
			if (this.BombTime > (float)nValue.int20 && this.BombTime < (float)nValue.int35 && this.BombCount != nValue.int1)
			{
				this.BombCount = nValue.int1;
				TimerManager.Cancel(this.BombAudioID);
				this.BombAudioID = TimerManager.In((float)nValue.int0, -nValue.int1, (float)nValue.int1, delegate()
				{
					this.BombAudioSource.PlayOneShot(this.BombAudioClip);
				});
			}
			else if (this.BombTime > (float)nValue.int10 && this.BombTime < (float)nValue.int20 && this.BombCount != nValue.int2)
			{
				this.BombCount = nValue.int2;
				TimerManager.Cancel(this.BombAudioID);
				this.BombAudioID = TimerManager.In((float)nValue.int0, -nValue.int1, nValue.float05, delegate()
				{
					this.BombAudioSource.PlayOneShot(this.BombAudioClip);
				});
			}
			else if (this.BombTime > (float)nValue.int0 && this.BombTime < (float)nValue.int10 && this.BombCount != nValue.int3)
			{
				this.BombCount = nValue.int3;
				TimerManager.Cancel(this.BombAudioID);
				this.BombAudioID = TimerManager.In((float)nValue.int0, -nValue.int1, nValue.float025, delegate()
				{
					this.BombAudioSource.PlayOneShot(this.BombAudioClip);
				});
			}
			this.BombTime -= Time.deltaTime;
		}
		else if (this.BombCount != nValue.int0)
		{
			this.BombCount = nValue.int0;
			this.BombTime = (float)(-(float)nValue.int1);
			TimerManager.Cancel(this.BombAudioID);
			this.BombAudioID = nValue.int0;
		}
	}
}
