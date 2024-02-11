using System;
using UnityEngine;
using UnityEngine.Events;

namespace CodeStage.AntiCheat.Detectors
{
	// Token: 0x020001B4 RID: 436
	[AddComponentMenu("")]
	public abstract class ActDetectorBase : MonoBehaviour
	{
		// Token: 0x06000FDD RID: 4061 RVA: 0x00072214 File Offset: 0x00070414
		private void Start()
		{
			if (ActDetectorBase.detectorsContainer == null && base.gameObject.name == "Anti-Cheat Toolkit Detectors")
			{
				ActDetectorBase.detectorsContainer = base.gameObject;
			}
			if (this.autoStart && !this.started)
			{
				this.StartDetectionAutomatically();
			}
		}

		// Token: 0x06000FDE RID: 4062 RVA: 0x0000F34B File Offset: 0x0000D54B
		private void OnEnable()
		{
			if (!this.started || (!this.detectionEventHasListener && this.detectionAction == null && !this.DetectorHasAdditionalCallbacks()))
			{
				return;
			}
			this.ResumeDetector();
		}

		// Token: 0x06000FDF RID: 4063 RVA: 0x0000F380 File Offset: 0x0000D580
		private void OnDisable()
		{
			if (!this.started)
			{
				return;
			}
			this.PauseDetector();
		}

		// Token: 0x06000FE0 RID: 4064 RVA: 0x0000F394 File Offset: 0x0000D594
		private void OnApplicationQuit()
		{
			this.DisposeInternal();
		}

		// Token: 0x06000FE1 RID: 4065 RVA: 0x00072274 File Offset: 0x00070474
		protected virtual void OnDestroy()
		{
			this.StopDetectionInternal();
			if (base.transform.childCount == 0 && base.GetComponentsInChildren<Component>().Length <= 2)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			else if (base.name == "Anti-Cheat Toolkit Detectors" && base.GetComponentsInChildren<ActDetectorBase>().Length <= 1)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x06000FE2 RID: 4066 RVA: 0x000722E4 File Offset: 0x000704E4
		protected virtual bool Init(ActDetectorBase instance, string detectorName)
		{
			if (instance != null && instance != this && instance.keepAlive)
			{
				Debug.LogWarning("[ACTk] " + base.name + ": self-destroying, other instance already exists & only one instance allowed!", base.gameObject);
				UnityEngine.Object.Destroy(this);
				return false;
			}
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			return true;
		}

		// Token: 0x06000FE3 RID: 4067 RVA: 0x0000F39C File Offset: 0x0000D59C
		protected virtual void DisposeInternal()
		{
			UnityEngine.Object.Destroy(this);
		}

		// Token: 0x06000FE4 RID: 4068 RVA: 0x00008872 File Offset: 0x00006A72
		protected virtual bool DetectorHasAdditionalCallbacks()
		{
			return false;
		}

		// Token: 0x06000FE5 RID: 4069 RVA: 0x00072348 File Offset: 0x00070548
		internal virtual void OnCheatingDetected()
		{
			if (this.detectionAction != null)
			{
				this.detectionAction();
			}
			if (this.detectionEventHasListener)
			{
				this.detectionEvent.Invoke();
			}
			if (this.autoDispose)
			{
				this.DisposeInternal();
			}
			else
			{
				this.StopDetectionInternal();
			}
		}

		// Token: 0x06000FE6 RID: 4070
		protected abstract void StartDetectionAutomatically();

		// Token: 0x06000FE7 RID: 4071
		protected abstract void StopDetectionInternal();

		// Token: 0x06000FE8 RID: 4072
		protected abstract void PauseDetector();

		// Token: 0x06000FE9 RID: 4073
		protected abstract void ResumeDetector();

		// Token: 0x04000AF9 RID: 2809
		protected const string CONTAINER_NAME = "Anti-Cheat Toolkit Detectors";

		// Token: 0x04000AFA RID: 2810
		protected const string MENU_PATH = "Code Stage/Anti-Cheat Toolkit/";

		// Token: 0x04000AFB RID: 2811
		protected const string GAME_OBJECT_MENU_PATH = "GameObject/Create Other/Code Stage/Anti-Cheat Toolkit/";

		// Token: 0x04000AFC RID: 2812
		protected static GameObject detectorsContainer;

		// Token: 0x04000AFD RID: 2813
		[Tooltip("Automatically start detector. Detection Event will be called on detection.")]
		public bool autoStart = true;

		// Token: 0x04000AFE RID: 2814
		[Tooltip("Detector will survive new level (scene) load if checked.")]
		public bool keepAlive = true;

		// Token: 0x04000AFF RID: 2815
		[Tooltip("Automatically dispose Detector after firing callback.")]
		public bool autoDispose = true;

		// Token: 0x04000B00 RID: 2816
		[SerializeField]
		protected UnityEvent detectionEvent;

		// Token: 0x04000B01 RID: 2817
		protected UnityAction detectionAction;

		// Token: 0x04000B02 RID: 2818
		[SerializeField]
		protected bool detectionEventHasListener;

		// Token: 0x04000B03 RID: 2819
		protected bool isRunning;

		// Token: 0x04000B04 RID: 2820
		protected bool started;
	}
}
