using System;
using UnityEngine;
using UnityEngine.Events;

namespace CodeStage.AntiCheat.Detectors
{
	// Token: 0x020001B8 RID: 440
	[AddComponentMenu("Code Stage/Anti-Cheat Toolkit/Speed Hack Detector")]
	public class SpeedHackDetector : ActDetectorBase
	{
		// Token: 0x06001018 RID: 4120 RVA: 0x0000F6E6 File Offset: 0x0000D8E6
		private SpeedHackDetector()
		{
		}

		// Token: 0x06001019 RID: 4121 RVA: 0x000729DC File Offset: 0x00070BDC
		public static void StartDetection()
		{
			if (SpeedHackDetector.Instance != null)
			{
				SpeedHackDetector.Instance.StartDetectionInternal(null, SpeedHackDetector.Instance.interval, SpeedHackDetector.Instance.maxFalsePositives, SpeedHackDetector.Instance.coolDown);
			}
			else
			{
				Debug.LogError("[ACTk] Speed Hack Detector: can't be started since it doesn't exists in scene or not yet initialized!");
			}
		}

		// Token: 0x0600101A RID: 4122 RVA: 0x0000F708 File Offset: 0x0000D908
		public static void StartDetection(UnityAction callback)
		{
			SpeedHackDetector.StartDetection(callback, SpeedHackDetector.GetOrCreateInstance.interval);
		}

		// Token: 0x0600101B RID: 4123 RVA: 0x0000F71A File Offset: 0x0000D91A
		public static void StartDetection(UnityAction callback, float interval)
		{
			SpeedHackDetector.StartDetection(callback, interval, SpeedHackDetector.GetOrCreateInstance.maxFalsePositives);
		}

		// Token: 0x0600101C RID: 4124 RVA: 0x0000F72D File Offset: 0x0000D92D
		public static void StartDetection(UnityAction callback, float interval, byte maxFalsePositives)
		{
			SpeedHackDetector.StartDetection(callback, interval, maxFalsePositives, SpeedHackDetector.GetOrCreateInstance.coolDown);
		}

		// Token: 0x0600101D RID: 4125 RVA: 0x0000F741 File Offset: 0x0000D941
		public static void StartDetection(UnityAction callback, float interval, byte maxFalsePositives, int coolDown)
		{
			SpeedHackDetector.GetOrCreateInstance.StartDetectionInternal(callback, interval, maxFalsePositives, coolDown);
		}

		// Token: 0x0600101E RID: 4126 RVA: 0x0000F751 File Offset: 0x0000D951
		public static void StopDetection()
		{
			if (SpeedHackDetector.Instance != null)
			{
				SpeedHackDetector.Instance.StopDetectionInternal();
			}
		}

		// Token: 0x0600101F RID: 4127 RVA: 0x0000F76D File Offset: 0x0000D96D
		public static void Dispose()
		{
			if (SpeedHackDetector.Instance != null)
			{
				SpeedHackDetector.Instance.DisposeInternal();
			}
		}

		// Token: 0x17000302 RID: 770
		// (get) Token: 0x06001020 RID: 4128 RVA: 0x0000F789 File Offset: 0x0000D989
		// (set) Token: 0x06001021 RID: 4129 RVA: 0x0000F790 File Offset: 0x0000D990
		public static SpeedHackDetector Instance { get; private set; }

		// Token: 0x17000303 RID: 771
		// (get) Token: 0x06001022 RID: 4130 RVA: 0x00072A34 File Offset: 0x00070C34
		private static SpeedHackDetector GetOrCreateInstance
		{
			get
			{
				if (SpeedHackDetector.Instance != null)
				{
					return SpeedHackDetector.Instance;
				}
				if (ActDetectorBase.detectorsContainer == null)
				{
					ActDetectorBase.detectorsContainer = new GameObject("Anti-Cheat Toolkit Detectors");
				}
				SpeedHackDetector.Instance = ActDetectorBase.detectorsContainer.AddComponent<SpeedHackDetector>();
				return SpeedHackDetector.Instance;
			}
		}

		// Token: 0x06001023 RID: 4131 RVA: 0x0000F798 File Offset: 0x0000D998
		private void Awake()
		{
			SpeedHackDetector.instancesInScene++;
			if (this.Init(SpeedHackDetector.Instance, "Speed Hack Detector"))
			{
				SpeedHackDetector.Instance = this;
			}
		}

		// Token: 0x06001024 RID: 4132 RVA: 0x0000F7C1 File Offset: 0x0000D9C1
		protected override void OnDestroy()
		{
			base.OnDestroy();
			SpeedHackDetector.instancesInScene--;
		}

		// Token: 0x06001025 RID: 4133 RVA: 0x0000F7D5 File Offset: 0x0000D9D5
		private void OnLevelWasLoaded()
		{
			this.OnLevelLoadedCallback();
		}

		// Token: 0x06001026 RID: 4134 RVA: 0x00072A8C File Offset: 0x00070C8C
		private void OnLevelLoadedCallback()
		{
			if (SpeedHackDetector.instancesInScene < 2)
			{
				if (!this.keepAlive)
				{
					this.DisposeInternal();
				}
			}
			else if (!this.keepAlive && SpeedHackDetector.Instance != this)
			{
				this.DisposeInternal();
			}
		}

		// Token: 0x06001027 RID: 4135 RVA: 0x0000F7DD File Offset: 0x0000D9DD
		private void OnApplicationPause(bool pause)
		{
			if (!pause)
			{
				this.ResetStartTicks();
			}
		}

		// Token: 0x06001028 RID: 4136 RVA: 0x00072ADC File Offset: 0x00070CDC
		private void Update()
		{
			if (!this.isRunning)
			{
				return;
			}
			long ticks = DateTime.UtcNow.Ticks;
			long num = ticks - this.prevTicks;
			if (num < 0L || num > 10000000L)
			{
				this.ResetStartTicks();
				return;
			}
			this.prevTicks = ticks;
			long num2 = (long)(this.interval * 1E+07f);
			if (ticks - this.prevIntervalTicks >= num2)
			{
				long num3 = (long)Environment.TickCount * 10000L;
				if (Mathf.Abs((float)(num3 - this.vulnerableTicksOnStart - (ticks - this.ticksOnStart))) > 5000000f)
				{
					this.currentFalsePositives += 1;
					if (this.currentFalsePositives > this.maxFalsePositives)
					{
						this.OnCheatingDetected();
					}
					else
					{
						this.currentCooldownShots = 0;
						this.ResetStartTicks();
					}
				}
				else if (this.currentFalsePositives > 0 && this.coolDown > 0)
				{
					this.currentCooldownShots++;
					if (this.currentCooldownShots >= this.coolDown)
					{
						this.currentFalsePositives = 0;
					}
				}
				this.prevIntervalTicks = ticks;
			}
		}

		// Token: 0x06001029 RID: 4137 RVA: 0x00072BFC File Offset: 0x00070DFC
		private void StartDetectionInternal(UnityAction callback, float checkInterval, byte falsePositives, int shotsTillCooldown)
		{
			if (this.isRunning)
			{
				Debug.LogWarning("[ACTk] Speed Hack Detector: already running!", this);
				return;
			}
			if (!base.enabled)
			{
				Debug.LogWarning("[ACTk] Speed Hack Detector: disabled but StartDetection still called from somewhere (see stack trace for this message)!", this);
				return;
			}
			if (callback != null && this.detectionEventHasListener)
			{
				Debug.LogWarning("[ACTk] Speed Hack Detector: has properly configured Detection Event in the inspector, but still get started with Action callback. Both Action and Detection Event will be called on detection. Are you sure you wish to do this?", this);
			}
			if (callback == null && !this.detectionEventHasListener)
			{
				Debug.LogWarning("[ACTk] Speed Hack Detector: was started without any callbacks. Please configure Detection Event in the inspector, or pass the callback Action to the StartDetection method.", this);
				base.enabled = false;
				return;
			}
			this.detectionAction = callback;
			this.interval = checkInterval;
			this.maxFalsePositives = falsePositives;
			this.coolDown = shotsTillCooldown;
			this.ResetStartTicks();
			this.currentFalsePositives = 0;
			this.currentCooldownShots = 0;
			this.started = true;
			this.isRunning = true;
		}

		// Token: 0x0600102A RID: 4138 RVA: 0x0000F7EB File Offset: 0x0000D9EB
		protected override void StartDetectionAutomatically()
		{
			this.StartDetectionInternal(null, this.interval, this.maxFalsePositives, this.coolDown);
		}

		// Token: 0x0600102B RID: 4139 RVA: 0x0000F67C File Offset: 0x0000D87C
		protected override void PauseDetector()
		{
			this.isRunning = false;
		}

		// Token: 0x0600102C RID: 4140 RVA: 0x0000F685 File Offset: 0x0000D885
		protected override void ResumeDetector()
		{
			if (this.detectionAction == null && !this.detectionEventHasListener)
			{
				return;
			}
			this.isRunning = true;
		}

		// Token: 0x0600102D RID: 4141 RVA: 0x0000F6A5 File Offset: 0x0000D8A5
		protected override void StopDetectionInternal()
		{
			if (!this.started)
			{
				return;
			}
			this.detectionAction = null;
			this.started = false;
			this.isRunning = false;
		}

		// Token: 0x0600102E RID: 4142 RVA: 0x0000F806 File Offset: 0x0000DA06
		protected override void DisposeInternal()
		{
			base.DisposeInternal();
			if (SpeedHackDetector.Instance == this)
			{
				SpeedHackDetector.Instance = null;
			}
		}

		// Token: 0x0600102F RID: 4143 RVA: 0x00072CB8 File Offset: 0x00070EB8
		private void ResetStartTicks()
		{
			this.ticksOnStart = DateTime.UtcNow.Ticks;
			this.vulnerableTicksOnStart = (long)Environment.TickCount * 10000L;
			this.prevTicks = this.ticksOnStart;
			this.prevIntervalTicks = this.ticksOnStart;
		}

		// Token: 0x04000B17 RID: 2839
		internal const string COMPONENT_NAME = "Speed Hack Detector";

		// Token: 0x04000B18 RID: 2840
		internal const string FINAL_LOG_PREFIX = "[ACTk] Speed Hack Detector: ";

		// Token: 0x04000B19 RID: 2841
		private const long TICKS_PER_SECOND = 10000000L;

		// Token: 0x04000B1A RID: 2842
		private const int THRESHOLD = 5000000;

		// Token: 0x04000B1B RID: 2843
		private static int instancesInScene;

		// Token: 0x04000B1C RID: 2844
		[Tooltip("Time (in seconds) between detector checks.")]
		public float interval = 1f;

		// Token: 0x04000B1D RID: 2845
		[Tooltip("Maximum false positives count allowed before registering speed hack.")]
		public byte maxFalsePositives = 3;

		// Token: 0x04000B1E RID: 2846
		[Tooltip("Amount of sequential successful checks before clearing internal false positives counter.\nSet 0 to disable Cool Down feature.")]
		public int coolDown = 30;

		// Token: 0x04000B1F RID: 2847
		private byte currentFalsePositives;

		// Token: 0x04000B20 RID: 2848
		private int currentCooldownShots;

		// Token: 0x04000B21 RID: 2849
		private long ticksOnStart;

		// Token: 0x04000B22 RID: 2850
		private long vulnerableTicksOnStart;

		// Token: 0x04000B23 RID: 2851
		private long prevTicks;

		// Token: 0x04000B24 RID: 2852
		private long prevIntervalTicks;
	}
}
