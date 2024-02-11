using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace CodeStage.AntiCheat.Detectors
{
	// Token: 0x020001B9 RID: 441
	[AddComponentMenu("Code Stage/Anti-Cheat Toolkit/WallHack Detector")]
	public class WallHackDetector : ActDetectorBase
	{
		// Token: 0x06001030 RID: 4144 RVA: 0x00072D04 File Offset: 0x00070F04
		private WallHackDetector()
		{
		}

		// Token: 0x17000304 RID: 772
		// (get) Token: 0x06001031 RID: 4145 RVA: 0x0000F824 File Offset: 0x0000DA24
		// (set) Token: 0x06001032 RID: 4146 RVA: 0x00072D94 File Offset: 0x00070F94
		public bool CheckRigidbody
		{
			get
			{
				return this.checkRigidbody;
			}
			set
			{
				if (this.checkRigidbody == value || !Application.isPlaying || !base.enabled || !base.gameObject.activeSelf)
				{
					return;
				}
				this.checkRigidbody = value;
				if (!this.started)
				{
					return;
				}
				this.UpdateServiceContainer();
				if (this.checkRigidbody)
				{
					this.StartRigidModule();
				}
				else
				{
					this.StopRigidModule();
				}
			}
		}

		// Token: 0x17000305 RID: 773
		// (get) Token: 0x06001033 RID: 4147 RVA: 0x0000F82C File Offset: 0x0000DA2C
		// (set) Token: 0x06001034 RID: 4148 RVA: 0x00072E08 File Offset: 0x00071008
		public bool CheckController
		{
			get
			{
				return this.checkController;
			}
			set
			{
				if (this.checkController == value || !Application.isPlaying || !base.enabled || !base.gameObject.activeSelf)
				{
					return;
				}
				this.checkController = value;
				if (!this.started)
				{
					return;
				}
				this.UpdateServiceContainer();
				if (this.checkController)
				{
					this.StartControllerModule();
				}
				else
				{
					this.StopControllerModule();
				}
			}
		}

		// Token: 0x17000306 RID: 774
		// (get) Token: 0x06001035 RID: 4149 RVA: 0x0000F834 File Offset: 0x0000DA34
		// (set) Token: 0x06001036 RID: 4150 RVA: 0x00072E7C File Offset: 0x0007107C
		public bool CheckWireframe
		{
			get
			{
				return this.checkWireframe;
			}
			set
			{
				if (this.checkWireframe == value || !Application.isPlaying || !base.enabled || !base.gameObject.activeSelf)
				{
					return;
				}
				this.checkWireframe = value;
				if (!this.started)
				{
					return;
				}
				this.UpdateServiceContainer();
				if (this.checkWireframe)
				{
					this.StartWireframeModule();
				}
				else
				{
					this.StopWireframeModule();
				}
			}
		}

		// Token: 0x17000307 RID: 775
		// (get) Token: 0x06001037 RID: 4151 RVA: 0x0000F83C File Offset: 0x0000DA3C
		// (set) Token: 0x06001038 RID: 4152 RVA: 0x00072EF0 File Offset: 0x000710F0
		public bool CheckRaycast
		{
			get
			{
				return this.checkRaycast;
			}
			set
			{
				if (this.checkRaycast == value || !Application.isPlaying || !base.enabled || !base.gameObject.activeSelf)
				{
					return;
				}
				this.checkRaycast = value;
				if (!this.started)
				{
					return;
				}
				this.UpdateServiceContainer();
				if (this.checkRaycast)
				{
					this.StartRaycastModule();
				}
				else
				{
					this.StopRaycastModule();
				}
			}
		}

		// Token: 0x06001039 RID: 4153 RVA: 0x0000F844 File Offset: 0x0000DA44
		public static void StartDetection()
		{
			if (WallHackDetector.Instance != null)
			{
				WallHackDetector.Instance.StartDetectionInternal(null, WallHackDetector.Instance.spawnPosition, WallHackDetector.Instance.maxFalsePositives);
			}
			else
			{
				Debug.LogError("[ACTk] WallHack Detector: can't be started since it doesn't exists in scene or not yet initialized!");
			}
		}

		// Token: 0x0600103A RID: 4154 RVA: 0x0000F884 File Offset: 0x0000DA84
		public static void StartDetection(UnityAction callback)
		{
			WallHackDetector.StartDetection(callback, WallHackDetector.GetOrCreateInstance.spawnPosition);
		}

		// Token: 0x0600103B RID: 4155 RVA: 0x0000F896 File Offset: 0x0000DA96
		public static void StartDetection(UnityAction callback, Vector3 spawnPosition)
		{
			WallHackDetector.StartDetection(callback, spawnPosition, WallHackDetector.GetOrCreateInstance.maxFalsePositives);
		}

		// Token: 0x0600103C RID: 4156 RVA: 0x0000F8A9 File Offset: 0x0000DAA9
		public static void StartDetection(UnityAction callback, Vector3 spawnPosition, byte maxFalsePositives)
		{
			WallHackDetector.GetOrCreateInstance.StartDetectionInternal(callback, spawnPosition, maxFalsePositives);
		}

		// Token: 0x0600103D RID: 4157 RVA: 0x0000F8B8 File Offset: 0x0000DAB8
		public static void StopDetection()
		{
			if (WallHackDetector.Instance != null)
			{
				WallHackDetector.Instance.StopDetectionInternal();
			}
		}

		// Token: 0x0600103E RID: 4158 RVA: 0x0000F8D4 File Offset: 0x0000DAD4
		public static void Dispose()
		{
			if (WallHackDetector.Instance != null)
			{
				WallHackDetector.Instance.DisposeInternal();
			}
		}

		// Token: 0x17000308 RID: 776
		// (get) Token: 0x0600103F RID: 4159 RVA: 0x0000F8F0 File Offset: 0x0000DAF0
		// (set) Token: 0x06001040 RID: 4160 RVA: 0x0000F8F7 File Offset: 0x0000DAF7
		public static WallHackDetector Instance { get; private set; }

		// Token: 0x17000309 RID: 777
		// (get) Token: 0x06001041 RID: 4161 RVA: 0x00072F64 File Offset: 0x00071164
		private static WallHackDetector GetOrCreateInstance
		{
			get
			{
				if (WallHackDetector.Instance != null)
				{
					return WallHackDetector.Instance;
				}
				if (ActDetectorBase.detectorsContainer == null)
				{
					ActDetectorBase.detectorsContainer = new GameObject("Anti-Cheat Toolkit Detectors");
				}
				WallHackDetector.Instance = ActDetectorBase.detectorsContainer.AddComponent<WallHackDetector>();
				return WallHackDetector.Instance;
			}
		}

		// Token: 0x06001042 RID: 4162 RVA: 0x0000F8FF File Offset: 0x0000DAFF
		private void Awake()
		{
			WallHackDetector.instancesInScene++;
			if (this.Init(WallHackDetector.Instance, "WallHack Detector"))
			{
				WallHackDetector.Instance = this;
			}
		}

		// Token: 0x06001043 RID: 4163 RVA: 0x00072FBC File Offset: 0x000711BC
		protected override void OnDestroy()
		{
			base.OnDestroy();
			base.StopAllCoroutines();
			if (this.serviceContainer != null)
			{
				UnityEngine.Object.Destroy(this.serviceContainer);
			}
			if (this.wfMaterial != null)
			{
				this.wfMaterial.mainTexture = null;
				this.wfMaterial.shader = null;
				this.wfMaterial = null;
				this.wfShader = null;
				this.shaderTexture = null;
				this.targetTexture = null;
				this.renderTexture.DiscardContents();
				this.renderTexture.Release();
				this.renderTexture = null;
			}
			WallHackDetector.instancesInScene--;
		}

		// Token: 0x06001044 RID: 4164 RVA: 0x0000F928 File Offset: 0x0000DB28
		private void OnLevelWasLoaded()
		{
			this.OnLevelLoadedCallback();
		}

		// Token: 0x06001045 RID: 4165 RVA: 0x00073060 File Offset: 0x00071260
		private void OnLevelLoadedCallback()
		{
			if (WallHackDetector.instancesInScene < 2)
			{
				if (!this.keepAlive)
				{
					this.DisposeInternal();
				}
			}
			else if (!this.keepAlive && WallHackDetector.Instance != this)
			{
				this.DisposeInternal();
			}
		}

		// Token: 0x06001046 RID: 4166 RVA: 0x000730B0 File Offset: 0x000712B0
		private void FixedUpdate()
		{
			if (!this.isRunning || !this.checkRigidbody || this.rigidPlayer == null)
			{
				return;
			}
			if (this.rigidPlayer.transform.localPosition.z > 1f)
			{
				this.rigidbodyDetections += 1;
				if (!this.Detect())
				{
					this.StopRigidModule();
					this.StartRigidModule();
				}
			}
		}

		// Token: 0x06001047 RID: 4167 RVA: 0x00073130 File Offset: 0x00071330
		private void Update()
		{
			if (!this.isRunning || !this.checkController || this.charControllerPlayer == null)
			{
				return;
			}
			if (this.charControllerVelocity > 0f)
			{
				this.charControllerPlayer.Move(new Vector3(UnityEngine.Random.Range(-0.002f, 0.002f), 0f, this.charControllerVelocity));
				if (this.charControllerPlayer.transform.localPosition.z > 1f)
				{
					this.controllerDetections += 1;
					if (!this.Detect())
					{
						this.StopControllerModule();
						this.StartControllerModule();
					}
				}
			}
		}

		// Token: 0x06001048 RID: 4168 RVA: 0x000731E8 File Offset: 0x000713E8
		private void StartDetectionInternal(UnityAction callback, Vector3 servicePosition, byte falsePositivesInRow)
		{
			if (this.isRunning)
			{
				Debug.LogWarning("[ACTk] WallHack Detector: already running!", this);
				return;
			}
			if (!base.enabled)
			{
				Debug.LogWarning("[ACTk] WallHack Detector: disabled but StartDetection still called from somewhere (see stack trace for this message)!", this);
				return;
			}
			if (callback != null && this.detectionEventHasListener)
			{
				Debug.LogWarning("[ACTk] WallHack Detector: has properly configured Detection Event in the inspector, but still get started with Action callback. Both Action and Detection Event will be called on detection. Are you sure you wish to do this?", this);
			}
			if (callback == null && !this.detectionEventHasListener)
			{
				Debug.LogWarning("[ACTk] WallHack Detector: was started without any callbacks. Please configure Detection Event in the inspector, or pass the callback Action to the StartDetection method.", this);
				base.enabled = false;
				return;
			}
			this.detectionAction = callback;
			this.spawnPosition = servicePosition;
			this.maxFalsePositives = falsePositivesInRow;
			this.rigidbodyDetections = 0;
			this.controllerDetections = 0;
			this.wireframeDetections = 0;
			this.raycastDetections = 0;
			base.StartCoroutine(this.InitDetector());
			this.started = true;
			this.isRunning = true;
		}

		// Token: 0x06001049 RID: 4169 RVA: 0x0000F930 File Offset: 0x0000DB30
		protected override void StartDetectionAutomatically()
		{
			this.StartDetectionInternal(null, this.spawnPosition, this.maxFalsePositives);
		}

		// Token: 0x0600104A RID: 4170 RVA: 0x0000F945 File Offset: 0x0000DB45
		protected override void PauseDetector()
		{
			if (!this.isRunning)
			{
				return;
			}
			this.isRunning = false;
			this.StopRigidModule();
			this.StopControllerModule();
			this.StopWireframeModule();
			this.StopRaycastModule();
		}

		// Token: 0x0600104B RID: 4171 RVA: 0x000732B0 File Offset: 0x000714B0
		protected override void ResumeDetector()
		{
			if (this.detectionAction == null && !this.detectionEventHasListener)
			{
				return;
			}
			this.isRunning = true;
			if (this.checkRigidbody)
			{
				this.StartRigidModule();
			}
			if (this.checkController)
			{
				this.StartControllerModule();
			}
			if (this.checkWireframe)
			{
				this.StartWireframeModule();
			}
			if (this.checkRaycast)
			{
				this.StartRaycastModule();
			}
		}

		// Token: 0x0600104C RID: 4172 RVA: 0x0000F972 File Offset: 0x0000DB72
		protected override void StopDetectionInternal()
		{
			if (!this.started)
			{
				return;
			}
			this.PauseDetector();
			this.detectionAction = null;
			this.isRunning = false;
		}

		// Token: 0x0600104D RID: 4173 RVA: 0x0000F994 File Offset: 0x0000DB94
		protected override void DisposeInternal()
		{
			base.DisposeInternal();
			if (WallHackDetector.Instance == this)
			{
				WallHackDetector.Instance = null;
			}
		}

		// Token: 0x0600104E RID: 4174 RVA: 0x00073320 File Offset: 0x00071520
		private void UpdateServiceContainer()
		{
			if (base.enabled && base.gameObject.activeSelf)
			{
				if (this.whLayer == -1)
				{
					this.whLayer = LayerMask.NameToLayer("Ignore Raycast");
				}
				if (this.raycastMask == -1)
				{
					this.raycastMask = LayerMask.GetMask(new string[]
					{
						"Ignore Raycast"
					});
				}
				if (this.serviceContainer == null)
				{
					this.serviceContainer = new GameObject("[WH Detector Service]");
					this.serviceContainer.layer = this.whLayer;
					this.serviceContainer.transform.position = this.spawnPosition;
					UnityEngine.Object.DontDestroyOnLoad(this.serviceContainer);
				}
				if ((this.checkRigidbody || this.checkController) && this.solidWall == null)
				{
					this.solidWall = new GameObject("SolidWall");
					this.solidWall.AddComponent<BoxCollider>();
					this.solidWall.layer = this.whLayer;
					this.solidWall.transform.parent = this.serviceContainer.transform;
					this.solidWall.transform.localScale = new Vector3(3f, 3f, 0.5f);
					this.solidWall.transform.localPosition = Vector3.zero;
				}
				else if (!this.checkRigidbody && !this.checkController && this.solidWall != null)
				{
					UnityEngine.Object.Destroy(this.solidWall);
				}
				if (this.checkWireframe && this.wfCamera == null)
				{
					if (this.wfShader == null)
					{
						this.wfShader = Shader.Find("Hidden/ACTk/WallHackTexture");
					}
					if (this.wfShader == null)
					{
						Debug.LogError("[ACTk] WallHack Detector: can't find 'Hidden/ACTk/WallHackTexture' shader!\nPlease make sure you have it included at the Editor > Project Settings > Graphics.", this);
						this.checkWireframe = false;
					}
					else if (!this.wfShader.isSupported)
					{
						Debug.LogError("[ACTk] WallHack Detector: can't detect wireframe cheats on this platform!", this);
						this.checkWireframe = false;
					}
					else
					{
						if (this.wfColor1 == Color.black)
						{
							this.wfColor1 = WallHackDetector.GenerateColor();
							do
							{
								this.wfColor2 = WallHackDetector.GenerateColor();
							}
							while (WallHackDetector.ColorsSimilar(this.wfColor1, this.wfColor2, 10));
						}
						if (this.shaderTexture == null)
						{
							this.shaderTexture = new Texture2D(4, 4, TextureFormat.RGB24, false);
							this.shaderTexture.filterMode = FilterMode.Point;
							Color[] array = new Color[16];
							for (int i = 0; i < 16; i++)
							{
								if (i < 8)
								{
									array[i] = this.wfColor1;
								}
								else
								{
									array[i] = this.wfColor2;
								}
							}
							this.shaderTexture.SetPixels(array, 0);
							this.shaderTexture.Apply();
						}
						if (this.renderTexture == null)
						{
							this.renderTexture = new RenderTexture(4, 4, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
							this.renderTexture.autoGenerateMips = false;
							this.renderTexture.filterMode = FilterMode.Point;
							this.renderTexture.Create();
						}
						if (this.targetTexture == null)
						{
							this.targetTexture = new Texture2D(4, 4, TextureFormat.RGB24, false);
							this.targetTexture.filterMode = FilterMode.Point;
						}
						if (this.wfMaterial == null)
						{
							this.wfMaterial = new Material(this.wfShader);
							this.wfMaterial.mainTexture = this.shaderTexture;
						}
						if (this.foregroundRenderer == null)
						{
							GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
							UnityEngine.Object.Destroy(gameObject.GetComponent<BoxCollider>());
							gameObject.name = "WireframeFore";
							gameObject.layer = this.whLayer;
							gameObject.transform.parent = this.serviceContainer.transform;
							gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
							this.foregroundRenderer = gameObject.GetComponent<MeshRenderer>();
							this.foregroundRenderer.sharedMaterial = this.wfMaterial;
							this.foregroundRenderer.castShadows = false;
							this.foregroundRenderer.receiveShadows = false;
							this.foregroundRenderer.enabled = false;
						}
						if (this.backgroundRenderer == null)
						{
							GameObject gameObject2 = GameObject.CreatePrimitive(PrimitiveType.Quad);
							UnityEngine.Object.Destroy(gameObject2.GetComponent<MeshCollider>());
							gameObject2.name = "WireframeBack";
							gameObject2.layer = this.whLayer;
							gameObject2.transform.parent = this.serviceContainer.transform;
							gameObject2.transform.localPosition = new Vector3(0f, 0f, 1f);
							gameObject2.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
							this.backgroundRenderer = gameObject2.GetComponent<MeshRenderer>();
							this.backgroundRenderer.sharedMaterial = this.wfMaterial;
							this.backgroundRenderer.castShadows = false;
							this.backgroundRenderer.receiveShadows = false;
							this.backgroundRenderer.enabled = false;
						}
						if (this.wfCamera == null)
						{
							this.wfCamera = new GameObject("WireframeCamera").AddComponent<Camera>();
							this.wfCamera.gameObject.layer = this.whLayer;
							this.wfCamera.transform.parent = this.serviceContainer.transform;
							this.wfCamera.transform.localPosition = new Vector3(0f, 0f, -1f);
							this.wfCamera.clearFlags = CameraClearFlags.Color;
							this.wfCamera.backgroundColor = Color.black;
							this.wfCamera.orthographic = true;
							this.wfCamera.orthographicSize = 0.5f;
							this.wfCamera.nearClipPlane = 0.01f;
							this.wfCamera.farClipPlane = 2.1f;
							this.wfCamera.depth = 0f;
							this.wfCamera.renderingPath = RenderingPath.Forward;
							this.wfCamera.useOcclusionCulling = false;
							this.wfCamera.allowHDR = false;
							this.wfCamera.targetTexture = this.renderTexture;
							this.wfCamera.enabled = false;
						}
					}
				}
				else if (!this.checkWireframe && this.wfCamera != null)
				{
					UnityEngine.Object.Destroy(this.foregroundRenderer.gameObject);
					UnityEngine.Object.Destroy(this.backgroundRenderer.gameObject);
					this.wfCamera.targetTexture = null;
					UnityEngine.Object.Destroy(this.wfCamera.gameObject);
				}
				if (this.checkRaycast && this.thinWall == null)
				{
					this.thinWall = GameObject.CreatePrimitive(PrimitiveType.Plane);
					this.thinWall.name = "ThinWall";
					this.thinWall.layer = this.whLayer;
					this.thinWall.transform.parent = this.serviceContainer.transform;
					this.thinWall.transform.localScale = new Vector3(0.2f, 1f, 0.2f);
					this.thinWall.transform.localRotation = Quaternion.Euler(270f, 0f, 0f);
					this.thinWall.transform.localPosition = new Vector3(0f, 0f, 1.4f);
					UnityEngine.Object.Destroy(this.thinWall.GetComponent<Renderer>());
					UnityEngine.Object.Destroy(this.thinWall.GetComponent<MeshFilter>());
				}
				else if (!this.checkRaycast && this.thinWall != null)
				{
					UnityEngine.Object.Destroy(this.thinWall);
				}
			}
			else if (this.serviceContainer != null)
			{
				UnityEngine.Object.Destroy(this.serviceContainer);
			}
		}

		// Token: 0x0600104F RID: 4175 RVA: 0x00073B20 File Offset: 0x00071D20
		private IEnumerator InitDetector()
		{
			yield return this.waitForEndOfFrame;
			this.UpdateServiceContainer();
			if (this.checkRigidbody)
			{
				this.StartRigidModule();
			}
			if (this.checkController)
			{
				this.StartControllerModule();
			}
			if (this.checkWireframe)
			{
				this.StartWireframeModule();
			}
			if (this.checkRaycast)
			{
				this.StartRaycastModule();
			}
			yield break;
		}

		// Token: 0x06001050 RID: 4176 RVA: 0x00073B3C File Offset: 0x00071D3C
		private void StartRigidModule()
		{
			if (!this.checkRigidbody)
			{
				this.StopRigidModule();
				this.UninitRigidModule();
				this.UpdateServiceContainer();
				return;
			}
			if (!this.rigidPlayer)
			{
				this.InitRigidModule();
			}
			if (this.rigidPlayer.transform.localPosition.z <= 1f && this.rigidbodyDetections > 0)
			{
				this.rigidbodyDetections = 0;
			}
			this.rigidPlayer.rotation = Quaternion.identity;
			this.rigidPlayer.angularVelocity = Vector3.zero;
			this.rigidPlayer.transform.localPosition = new Vector3(0.75f, 0f, -1f);
			this.rigidPlayer.velocity = this.rigidPlayerVelocity;
			base.Invoke("StartRigidModule", 4f);
		}

		// Token: 0x06001051 RID: 4177 RVA: 0x00073C18 File Offset: 0x00071E18
		private void StartControllerModule()
		{
			if (!this.checkController)
			{
				this.StopControllerModule();
				this.UninitControllerModule();
				this.UpdateServiceContainer();
				return;
			}
			if (!this.charControllerPlayer)
			{
				this.InitControllerModule();
			}
			if (this.charControllerPlayer.transform.localPosition.z <= 1f && this.controllerDetections > 0)
			{
				this.controllerDetections = 0;
			}
			this.charControllerPlayer.transform.localPosition = new Vector3(-0.75f, 0f, -1f);
			this.charControllerVelocity = 0.01f;
			base.Invoke("StartControllerModule", 4f);
		}

		// Token: 0x06001052 RID: 4178 RVA: 0x0000F9B2 File Offset: 0x0000DBB2
		private void StartWireframeModule()
		{
			if (!this.checkWireframe)
			{
				this.StopWireframeModule();
				this.UpdateServiceContainer();
				return;
			}
			if (!this.wireframeDetected)
			{
				base.Invoke("ShootWireframeModule", (float)this.wireframeDelay);
			}
		}

		// Token: 0x06001053 RID: 4179 RVA: 0x0000F9E9 File Offset: 0x0000DBE9
		private void ShootWireframeModule()
		{
			base.StartCoroutine(this.CaptureFrame());
			base.Invoke("ShootWireframeModule", (float)this.wireframeDelay);
		}

		// Token: 0x06001054 RID: 4180 RVA: 0x00073CD0 File Offset: 0x00071ED0
		private IEnumerator CaptureFrame()
		{
			this.wfCamera.enabled = true;
			yield return this.waitForEndOfFrame;
			this.foregroundRenderer.enabled = true;
			this.backgroundRenderer.enabled = true;
			RenderTexture previousActive = RenderTexture.active;
			RenderTexture.active = this.renderTexture;
			this.wfCamera.Render();
			this.foregroundRenderer.enabled = false;
			this.backgroundRenderer.enabled = false;
			while (!this.renderTexture.IsCreated())
			{
				yield return this.waitForEndOfFrame;
			}
			this.targetTexture.ReadPixels(new Rect(0f, 0f, 4f, 4f), 0, 0, false);
			this.targetTexture.Apply();
			RenderTexture.active = previousActive;
			if (this.wfCamera == null)
			{
				yield return null;
			}
			this.wfCamera.enabled = false;
			if (!(this.targetTexture.GetPixel(0, 3) != this.wfColor1) && !(this.targetTexture.GetPixel(0, 1) != this.wfColor2) && !(this.targetTexture.GetPixel(3, 3) != this.wfColor1) && !(this.targetTexture.GetPixel(3, 1) != this.wfColor2) && !(this.targetTexture.GetPixel(1, 3) != this.wfColor1) && !(this.targetTexture.GetPixel(2, 3) != this.wfColor1) && !(this.targetTexture.GetPixel(1, 1) != this.wfColor2) && !(this.targetTexture.GetPixel(2, 1) != this.wfColor2))
			{
				if (this.wireframeDetections > 0)
				{
					this.wireframeDetections = 0;
				}
			}
			else
			{
				this.wireframeDetections += 1;
				this.wireframeDetected = this.Detect();
			}
			yield return null;
			yield break;
		}

		// Token: 0x06001055 RID: 4181 RVA: 0x0000FA0A File Offset: 0x0000DC0A
		private void StartRaycastModule()
		{
			if (!this.checkRaycast)
			{
				this.StopRaycastModule();
				this.UpdateServiceContainer();
				return;
			}
			base.Invoke("ShootRaycastModule", (float)this.raycastDelay);
		}

		// Token: 0x06001056 RID: 4182 RVA: 0x00073CEC File Offset: 0x00071EEC
		private void ShootRaycastModule()
		{
			if (Physics.Raycast(this.serviceContainer.transform.position, this.serviceContainer.transform.TransformDirection(Vector3.forward), 1.5f, this.raycastMask))
			{
				if (this.raycastDetections > 0)
				{
					this.raycastDetections = 0;
				}
			}
			else
			{
				this.raycastDetections += 1;
				if (this.Detect())
				{
					return;
				}
			}
			base.Invoke("ShootRaycastModule", (float)this.raycastDelay);
		}

		// Token: 0x06001057 RID: 4183 RVA: 0x0000FA36 File Offset: 0x0000DC36
		private void StopRigidModule()
		{
			if (this.rigidPlayer)
			{
				this.rigidPlayer.velocity = Vector3.zero;
			}
			base.CancelInvoke("StartRigidModule");
		}

		// Token: 0x06001058 RID: 4184 RVA: 0x0000FA63 File Offset: 0x0000DC63
		private void StopControllerModule()
		{
			if (this.charControllerPlayer)
			{
				this.charControllerVelocity = 0f;
			}
			base.CancelInvoke("StartControllerModule");
		}

		// Token: 0x06001059 RID: 4185 RVA: 0x0000FA8B File Offset: 0x0000DC8B
		private void StopWireframeModule()
		{
			base.CancelInvoke("ShootWireframeModule");
		}

		// Token: 0x0600105A RID: 4186 RVA: 0x0000FA98 File Offset: 0x0000DC98
		private void StopRaycastModule()
		{
			base.CancelInvoke("ShootRaycastModule");
		}

		// Token: 0x0600105B RID: 4187 RVA: 0x00073D78 File Offset: 0x00071F78
		private void InitRigidModule()
		{
			GameObject gameObject = new GameObject("RigidPlayer");
			gameObject.AddComponent<CapsuleCollider>().height = 2f;
			gameObject.layer = this.whLayer;
			gameObject.transform.parent = this.serviceContainer.transform;
			gameObject.transform.localPosition = new Vector3(0.75f, 0f, -1f);
			this.rigidPlayer = gameObject.AddComponent<Rigidbody>();
			this.rigidPlayer.useGravity = false;
		}

		// Token: 0x0600105C RID: 4188 RVA: 0x00073DFC File Offset: 0x00071FFC
		private void InitControllerModule()
		{
			GameObject gameObject = new GameObject("ControlledPlayer");
			gameObject.AddComponent<CapsuleCollider>().height = 2f;
			gameObject.layer = this.whLayer;
			gameObject.transform.parent = this.serviceContainer.transform;
			gameObject.transform.localPosition = new Vector3(-0.75f, 0f, -1f);
			this.charControllerPlayer = gameObject.AddComponent<CharacterController>();
		}

		// Token: 0x0600105D RID: 4189 RVA: 0x0000FAA5 File Offset: 0x0000DCA5
		private void UninitRigidModule()
		{
			if (!this.rigidPlayer)
			{
				return;
			}
			UnityEngine.Object.Destroy(this.rigidPlayer.gameObject);
			this.rigidPlayer = null;
		}

		// Token: 0x0600105E RID: 4190 RVA: 0x0000FACF File Offset: 0x0000DCCF
		private void UninitControllerModule()
		{
			if (!this.charControllerPlayer)
			{
				return;
			}
			UnityEngine.Object.Destroy(this.charControllerPlayer.gameObject);
			this.charControllerPlayer = null;
		}

		// Token: 0x0600105F RID: 4191 RVA: 0x00073E74 File Offset: 0x00072074
		private bool Detect()
		{
			bool result = false;
			if (this.controllerDetections > this.maxFalsePositives || this.rigidbodyDetections > this.maxFalsePositives || this.wireframeDetections > this.maxFalsePositives || this.raycastDetections > this.maxFalsePositives)
			{
				this.OnCheatingDetected();
				result = true;
			}
			return result;
		}

		// Token: 0x06001060 RID: 4192 RVA: 0x0000FAF9 File Offset: 0x0000DCF9
		private static Color32 GenerateColor()
		{
			return new Color32((byte)UnityEngine.Random.Range(0, 256), (byte)UnityEngine.Random.Range(0, 256), (byte)UnityEngine.Random.Range(0, 256), byte.MaxValue);
		}

		// Token: 0x06001061 RID: 4193 RVA: 0x00073ED0 File Offset: 0x000720D0
		private static bool ColorsSimilar(Color32 c1, Color32 c2, int tolerance)
		{
			return Math.Abs((int)(c1.r - c2.r)) < tolerance && Math.Abs((int)(c1.g - c2.g)) < tolerance && Math.Abs((int)(c1.b - c2.b)) < tolerance;
		}

		// Token: 0x04000B26 RID: 2854
		internal const string COMPONENT_NAME = "WallHack Detector";

		// Token: 0x04000B27 RID: 2855
		internal const string FINAL_LOG_PREFIX = "[ACTk] WallHack Detector: ";

		// Token: 0x04000B28 RID: 2856
		private const string SERVICE_CONTAINER_NAME = "[WH Detector Service]";

		// Token: 0x04000B29 RID: 2857
		private const string WIREFRAME_SHADER_NAME = "Hidden/ACTk/WallHackTexture";

		// Token: 0x04000B2A RID: 2858
		private const int SHADER_TEXTURE_SIZE = 4;

		// Token: 0x04000B2B RID: 2859
		private const int RENDER_TEXTURE_SIZE = 4;

		// Token: 0x04000B2C RID: 2860
		private readonly Vector3 rigidPlayerVelocity = new Vector3(0f, 0f, 1f);

		// Token: 0x04000B2D RID: 2861
		private static int instancesInScene;

		// Token: 0x04000B2E RID: 2862
		private readonly WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

		// Token: 0x04000B2F RID: 2863
		[SerializeField]
		[Tooltip("Check for the \"walk through the walls\" kind of cheats made via Rigidbody hacks?")]
		private bool checkRigidbody = true;

		// Token: 0x04000B30 RID: 2864
		[SerializeField]
		[Tooltip("Check for the \"walk through the walls\" kind of cheats made via Character Controller hacks?")]
		private bool checkController = true;

		// Token: 0x04000B31 RID: 2865
		[Tooltip("Check for the \"see through the walls\" kind of cheats made via shader or driver hacks (wireframe, color alpha, etc.)?")]
		[SerializeField]
		private bool checkWireframe = true;

		// Token: 0x04000B32 RID: 2866
		[SerializeField]
		[Tooltip("Check for the \"shoot through the walls\" kind of cheats made via Raycast hacks?")]
		private bool checkRaycast = true;

		// Token: 0x04000B33 RID: 2867
		[Range(1f, 60f)]
		[Tooltip("Delay between Wireframe module checks, from 1 up to 60 secs.")]
		public int wireframeDelay = 10;

		// Token: 0x04000B34 RID: 2868
		[Tooltip("Delay between Raycast module checks, from 1 up to 60 secs.")]
		[Range(1f, 60f)]
		public int raycastDelay = 10;

		// Token: 0x04000B35 RID: 2869
		[Tooltip("World position of the container for service objects within 3x3x3 cube (drawn as red wire cube in scene).")]
		public Vector3 spawnPosition;

		// Token: 0x04000B36 RID: 2870
		[Tooltip("Maximum false positives in a row for each detection module before registering a wall hack.")]
		public byte maxFalsePositives = 3;

		// Token: 0x04000B37 RID: 2871
		private GameObject serviceContainer;

		// Token: 0x04000B38 RID: 2872
		private GameObject solidWall;

		// Token: 0x04000B39 RID: 2873
		private GameObject thinWall;

		// Token: 0x04000B3A RID: 2874
		private Camera wfCamera;

		// Token: 0x04000B3B RID: 2875
		private MeshRenderer foregroundRenderer;

		// Token: 0x04000B3C RID: 2876
		private MeshRenderer backgroundRenderer;

		// Token: 0x04000B3D RID: 2877
		private Color wfColor1 = Color.black;

		// Token: 0x04000B3E RID: 2878
		private Color wfColor2 = Color.black;

		// Token: 0x04000B3F RID: 2879
		private Shader wfShader;

		// Token: 0x04000B40 RID: 2880
		private Material wfMaterial;

		// Token: 0x04000B41 RID: 2881
		private Texture2D shaderTexture;

		// Token: 0x04000B42 RID: 2882
		private Texture2D targetTexture;

		// Token: 0x04000B43 RID: 2883
		private RenderTexture renderTexture;

		// Token: 0x04000B44 RID: 2884
		private int whLayer = -1;

		// Token: 0x04000B45 RID: 2885
		private int raycastMask = -1;

		// Token: 0x04000B46 RID: 2886
		private Rigidbody rigidPlayer;

		// Token: 0x04000B47 RID: 2887
		private CharacterController charControllerPlayer;

		// Token: 0x04000B48 RID: 2888
		private float charControllerVelocity;

		// Token: 0x04000B49 RID: 2889
		private byte rigidbodyDetections;

		// Token: 0x04000B4A RID: 2890
		private byte controllerDetections;

		// Token: 0x04000B4B RID: 2891
		private byte wireframeDetections;

		// Token: 0x04000B4C RID: 2892
		private byte raycastDetections;

		// Token: 0x04000B4D RID: 2893
		private bool wireframeDetected;
	}
}
