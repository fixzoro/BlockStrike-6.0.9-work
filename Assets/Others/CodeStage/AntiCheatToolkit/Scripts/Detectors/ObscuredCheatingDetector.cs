using System;
using UnityEngine;
using UnityEngine.Events;

namespace CodeStage.AntiCheat.Detectors
{
	// Token: 0x020001B7 RID: 439
	[AddComponentMenu("Code Stage/Anti-Cheat Toolkit/Obscured Cheating Detector")]
	public class ObscuredCheatingDetector : ActDetectorBase
	{
		// Token: 0x06001005 RID: 4101 RVA: 0x0000F561 File Offset: 0x0000D761
		private ObscuredCheatingDetector()
		{
		}

		// Token: 0x06001006 RID: 4102 RVA: 0x0000F595 File Offset: 0x0000D795
		public static void StartDetection()
		{
			if (ObscuredCheatingDetector.Instance != null)
			{
				ObscuredCheatingDetector.Instance.StartDetectionInternal(null);
			}
			else
			{
				Debug.LogError("[ACTk] Obscured Cheating Detector: can't be started since it doesn't exists in scene or not yet initialized!");
			}
		}

		// Token: 0x06001007 RID: 4103 RVA: 0x0000F5C1 File Offset: 0x0000D7C1
		public static void StartDetection(UnityAction callback)
		{
			ObscuredCheatingDetector.GetOrCreateInstance.StartDetectionInternal(callback);
		}

		// Token: 0x06001008 RID: 4104 RVA: 0x0000F5CE File Offset: 0x0000D7CE
		public static void StopDetection()
		{
			if (ObscuredCheatingDetector.Instance != null)
			{
				ObscuredCheatingDetector.Instance.StopDetectionInternal();
			}
		}

		// Token: 0x06001009 RID: 4105 RVA: 0x0000F5EA File Offset: 0x0000D7EA
		public static void Dispose()
		{
			if (ObscuredCheatingDetector.Instance != null)
			{
				ObscuredCheatingDetector.Instance.DisposeInternal();
			}
		}

		// Token: 0x170002FF RID: 767
		// (get) Token: 0x0600100A RID: 4106 RVA: 0x0000F606 File Offset: 0x0000D806
		// (set) Token: 0x0600100B RID: 4107 RVA: 0x0000F60D File Offset: 0x0000D80D
		public static ObscuredCheatingDetector Instance { get; private set; }

		// Token: 0x17000300 RID: 768
		// (get) Token: 0x0600100C RID: 4108 RVA: 0x000728A4 File Offset: 0x00070AA4
		private static ObscuredCheatingDetector GetOrCreateInstance
		{
			get
			{
				if (ObscuredCheatingDetector.Instance != null)
				{
					return ObscuredCheatingDetector.Instance;
				}
				if (ActDetectorBase.detectorsContainer == null)
				{
					ActDetectorBase.detectorsContainer = new GameObject("Anti-Cheat Toolkit Detectors");
				}
				ObscuredCheatingDetector.Instance = ActDetectorBase.detectorsContainer.AddComponent<ObscuredCheatingDetector>();
				return ObscuredCheatingDetector.Instance;
			}
		}

		// Token: 0x17000301 RID: 769
		// (get) Token: 0x0600100D RID: 4109 RVA: 0x0000F615 File Offset: 0x0000D815
		internal static bool IsRunning
		{
			get
			{
				return ObscuredCheatingDetector.Instance != null && ObscuredCheatingDetector.Instance.isRunning;
			}
		}

		// Token: 0x0600100E RID: 4110 RVA: 0x0000F62E File Offset: 0x0000D82E
		private void Awake()
		{
			ObscuredCheatingDetector.instancesInScene++;
			if (this.Init(ObscuredCheatingDetector.Instance, "Obscured Cheating Detector"))
			{
				ObscuredCheatingDetector.Instance = this;
			}
		}

		// Token: 0x0600100F RID: 4111 RVA: 0x0000F657 File Offset: 0x0000D857
		protected override void OnDestroy()
		{
			base.OnDestroy();
			ObscuredCheatingDetector.instancesInScene--;
		}

		// Token: 0x06001010 RID: 4112 RVA: 0x0000F66B File Offset: 0x0000D86B
		private void OnLevelWasLoaded()
		{
			this.OnLevelLoadedCallback();
		}

		// Token: 0x06001011 RID: 4113 RVA: 0x000728FC File Offset: 0x00070AFC
		private void OnLevelLoadedCallback()
		{
			if (ObscuredCheatingDetector.instancesInScene < 2)
			{
				if (!this.keepAlive)
				{
					this.DisposeInternal();
				}
			}
			else if (!this.keepAlive && ObscuredCheatingDetector.Instance != this)
			{
				this.DisposeInternal();
			}
		}

		// Token: 0x06001012 RID: 4114 RVA: 0x0007294C File Offset: 0x00070B4C
		private void StartDetectionInternal(UnityAction callback)
		{
			if (this.isRunning)
			{
				Debug.LogWarning("[ACTk] Obscured Cheating Detector: already running!", this);
				return;
			}
			if (!base.enabled)
			{
				Debug.LogWarning("[ACTk] Obscured Cheating Detector: disabled but StartDetection still called from somewhere (see stack trace for this message)!", this);
				return;
			}
			if (callback != null && this.detectionEventHasListener)
			{
				Debug.LogWarning("[ACTk] Obscured Cheating Detector: has properly configured Detection Event in the inspector, but still get started with Action callback. Both Action and Detection Event will be called on detection. Are you sure you wish to do this?", this);
			}
			if (callback == null && !this.detectionEventHasListener)
			{
				Debug.LogWarning("[ACTk] Obscured Cheating Detector: was started without any callbacks. Please configure Detection Event in the inspector, or pass the callback Action to the StartDetection method.", this);
				base.enabled = false;
				return;
			}
			this.detectionAction = callback;
			this.started = true;
			this.isRunning = true;
		}

		// Token: 0x06001013 RID: 4115 RVA: 0x0000F673 File Offset: 0x0000D873
		protected override void StartDetectionAutomatically()
		{
			this.StartDetectionInternal(null);
		}

		// Token: 0x06001014 RID: 4116 RVA: 0x0000F67C File Offset: 0x0000D87C
		protected override void PauseDetector()
		{
			this.isRunning = false;
		}

		// Token: 0x06001015 RID: 4117 RVA: 0x0000F685 File Offset: 0x0000D885
		protected override void ResumeDetector()
		{
			if (this.detectionAction == null && !this.detectionEventHasListener)
			{
				return;
			}
			this.isRunning = true;
		}

		// Token: 0x06001016 RID: 4118 RVA: 0x0000F6A5 File Offset: 0x0000D8A5
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

		// Token: 0x06001017 RID: 4119 RVA: 0x0000F6C8 File Offset: 0x0000D8C8
		protected override void DisposeInternal()
		{
			base.DisposeInternal();
			if (ObscuredCheatingDetector.Instance == this)
			{
				ObscuredCheatingDetector.Instance = null;
			}
		}

		// Token: 0x04000B0F RID: 2831
		internal const string COMPONENT_NAME = "Obscured Cheating Detector";

		// Token: 0x04000B10 RID: 2832
		internal const string FINAL_LOG_PREFIX = "[ACTk] Obscured Cheating Detector: ";

		// Token: 0x04000B11 RID: 2833
		private static int instancesInScene;

		// Token: 0x04000B12 RID: 2834
		[Tooltip("Max allowed difference between encrypted and fake values in ObscuredFloat. Increase in case of false positives.")]
		public float floatEpsilon = 0.0001f;

		// Token: 0x04000B13 RID: 2835
		[Tooltip("Max allowed difference between encrypted and fake values in ObscuredVector2. Increase in case of false positives.")]
		public float vector2Epsilon = 0.1f;

		// Token: 0x04000B14 RID: 2836
		[Tooltip("Max allowed difference between encrypted and fake values in ObscuredVector3. Increase in case of false positives.")]
		public float vector3Epsilon = 0.1f;

		// Token: 0x04000B15 RID: 2837
		[Tooltip("Max allowed difference between encrypted and fake values in ObscuredQuaternion. Increase in case of false positives.")]
		public float quaternionEpsilon = 0.1f;
	}
}
