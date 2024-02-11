using System;
using System.Text;
using UnityEngine;
using UnityEngine.Profiling;

public class UIFramesPerSecond : MonoBehaviour
{
    public const int MEMORY_DIVIDER = 1048576;

    public UILabel label;

    private bool activated;

    private float accum;

    private float frames;

    private StringBuilder builder;

    private static bool allStats;

    private void Start()
	{
		EventManager.AddListener("OnSettings", new EventManager.Callback(this.OnSettings));
		this.OnSettings();
		UIFramesPerSecond.allStats = GameConsole.Load<bool>("show_all_stats", false);
	}

	private void OnDisable()
	{
		TimerManager.Cancel("FPSMeter");
	}

	private void Update()
	{
		if (this.activated)
		{
			this.accum += Time.timeScale / Time.deltaTime;
			this.frames += 1f;
		}
	}

	private void OnSettings()
	{
		this.activated = Settings.FPSMeter;
		if (this.activated)
		{
			if (TimerManager.IsActive("FPSMeter"))
			{
				TimerManager.Cancel("FPSMeter");
			}
			TimerManager.In("FPSMeter", 1f, -1, 1f, new TimerManager.Callback(this.UpdateLabel));
		}
		else
		{
			TimerManager.Cancel("FPSMeter");
		}
		this.label.gameObject.SetActive(this.activated);
	}

	private void UpdateLabel()
	{
        int number = Mathf.RoundToInt(this.accum / this.frames);
        this.accum = 0f;
        this.frames = 0f;
        if (UIFramesPerSecond.allStats)
        {
            if (this.builder == null)
            {
                this.builder = new StringBuilder();
            }
            this.builder.Length = 0;
            this.builder.Capacity = 0;
            this.builder.Append("FPS: ").Append(StringCache.Get(number)).Append(" | ");
            this.builder.Append("PING: ").Append(StringCache.Get(PhotonNetwork.GetPing())).Append(" | ");
            this.builder.Append("MEM TOTAL: ").Append(StringCache.Get(Profiler.GetTotalReservedMemory() / 1048576U)).Append(" | ");
            this.builder.Append("MEM ALLOC: ").Append(StringCache.Get(Profiler.GetTotalAllocatedMemory() / 1048576U)).Append(" | ");
            this.builder.Append("MEM MONO: ").Append(StringCache.Get((int)(GC.GetTotalMemory(false) / 1048576L))).Append(" | ");
            this.builder.Append("MEM UNSED: ").Append(StringCache.Get(Profiler.GetTotalUnusedReservedMemory() / 1048576U));
            this.label.text = this.builder.ToString();
        }
        else
        {
            this.label.text = "FPS: " + StringCache.Get(number);
        }
    }
}
