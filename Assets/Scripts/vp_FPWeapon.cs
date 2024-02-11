using System;
using System.Collections.Generic;
using UnityEngine;

public class vp_FPWeapon : vp_Component
{
    private static bool loadCustomData = false;

    private static float customRenderingFieldOfView = -20f;

    private static bool customBob = true;

    public static Vector3 customPositionOffset = Vector3.zero;

    public bool Spectator;

    public float SpectatorVelocity;

    public GameObject WeaponPrefab;

    protected GameObject m_WeaponModel;

    public CharacterController Controller;

    public CryptoFloat RenderingZoomDamping = 0.5f;

    protected float m_FinalZoomTime = nValue.float0;

    public CryptoFloat RenderingFieldOfView = 35f;

    public CryptoVector2 RenderingClippingPlanes = new Vector2(0.01f, 10f);

    public CryptoFloat RenderingZScale = 1f;

    public CryptoVector3 PositionOffset = new Vector3(0.15f, -0.15f, -0.15f);

    public CryptoFloat PositionSpringStiffness = 0.01f;

    public CryptoFloat PositionSpringDamping = 0.25f;

    public CryptoFloat PositionFallRetract = 1f;

    public CryptoFloat PositionPivotSpringStiffness = 0.01f;

    public CryptoFloat PositionPivotSpringDamping = 0.25f;

    public CryptoFloat PositionSpring2Stiffness = 0.95f;

    public CryptoFloat PositionSpring2Damping = 0.25f;

    public CryptoFloat PositionKneeling = 0.06f;

    public CryptoInt PositionKneelingSoftness = 1;

    public CryptoVector3 PositionWalkSlide = new Vector3(0.5f, 0.75f, 0.5f);

    public CryptoVector3 PositionPivot = Vector3.zero;

    public CryptoVector3 RotationPivot = Vector3.zero;

    public CryptoFloat PositionInputVelocityScale = 1f;

    public CryptoFloat PositionMaxInputVelocity = 25f;

    protected vp_SpringThread m_PositionSpring;

    protected vp_SpringThread m_PositionSpring2;

    protected vp_SpringThread m_PositionPivotSpring;

    protected vp_SpringThread m_RotationPivotSpring;

    protected GameObject m_WeaponCamera;

    protected GameObject m_WeaponGroup;

    protected GameObject m_Pivot;

    protected Transform m_WeaponGroupTransform;

    public CryptoVector3 RotationOffset = Vector3.zero;

    public CryptoFloat RotationSpringStiffness = 0.01f;

    public CryptoFloat RotationSpringDamping = 0.25f;

    public CryptoFloat RotationPivotSpringStiffness = 0.01f;

    public CryptoFloat RotationPivotSpringDamping = 0.25f;

    public CryptoFloat RotationSpring2Stiffness = 0.95f;

    public CryptoFloat RotationSpring2Damping = 0.25f;

    public CryptoFloat RotationKneeling = 0f;

    public CryptoInt RotationKneelingSoftness = 1;

    public CryptoVector3 RotationLookSway = new Vector3(1f, 0.7f, 0f);

    public CryptoVector3 RotationStrafeSway = new Vector3(0.3f, 1f, 1.5f);

    public CryptoVector3 RotationFallSway = new Vector3(1f, -0.5f, -3f);

    public CryptoFloat RotationSlopeSway = 0.5f;

    public CryptoFloat RotationInputVelocityScale = 1f;

    public CryptoFloat RotationMaxInputVelocity = 15f;

    protected vp_SpringThread m_RotationSpring;

    protected vp_SpringThread m_RotationSpring2;

    protected Vector3 m_SwayVel = Vector3.zero;

    protected Vector3 m_FallSway = Vector3.zero;

    public CryptoFloat RetractionDistance = 0f;

    public CryptoVector2 RetractionOffset = new Vector2(0f, 0f);

    public CryptoFloat RetractionRelaxSpeed = 0.25f;

    protected bool m_DrawRetractionDebugLine;

    public CryptoFloat ShakeSpeed = 0.05f;

    public CryptoVector3 ShakeAmplitude = new Vector3(0.25f, 0f, 2f);

    protected Vector3 m_Shake = Vector3.zero;

    public CryptoVector4 BobRate = new Vector4(0.9f, 0.45f, 0f, 0f);

    public CryptoVector4 BobAmplitude = new Vector4(0.35f, 0.5f, 0f, 0f);

    protected float m_LastBobSpeed;

    protected Vector4 m_CurrentBobAmp = Vector4.zero;

    protected Vector4 m_CurrentBobVal = Vector4.zero;

    protected float m_BobSpeed;

    public CryptoVector3 StepPositionForce = new Vector3(0f, -0.0012f, -0.0012f);

    public CryptoVector3 StepRotationForce = new Vector3(0f, 0f, 0f);

    public CryptoInt StepSoftness = 4;

    public CryptoFloat StepMinVelocity = 0f;

    public CryptoFloat StepPositionBalance = 0f;

    public CryptoFloat StepRotationBalance = 0f;

    public CryptoFloat StepForceScale = 1f;

    protected float m_LastUpBob;

    protected bool m_BobWasElevating;

    protected Vector3 m_PosStep = Vector3.zero;

    protected Vector3 m_RotStep = Vector3.zero;

    public AudioClip SoundWield;

    public AudioClip SoundUnWield;

    public AnimationClip AnimationWield;

    public AnimationClip AnimationUnWield;

    public List<UnityEngine.Object> AnimationAmbient = new List<UnityEngine.Object>();

    protected List<bool> m_AmbAnimPlayed = new List<bool>();

    public Vector2 AmbientInterval = new Vector2(2.5f, 7.5f);

    protected int m_CurrentAmbientAnimation;

    protected int m_AnimationAmbientTimer;

    public CryptoVector3 PositionExitOffset = new Vector3(0f, -1f, 0f);

    public CryptoVector3 RotationExitOffset = new Vector3(40f, 0f, 0f);

    protected bool m_Wielded = true;

    protected Vector2 m_MouseMove = Vector2.zero;

    private Vector3 controllerVelocity;

    private bool controllerIsGrounded;

    private Vector3 swayLocalVelocity;

    private float bobTime;

    private Vector2 LookAxis;

    public bool Wielded
	{
		get
		{
			return this.m_Wielded && base.Rendering;
		}
	}
    
	public GameObject WeaponCamera
	{
		get
		{
			return this.m_WeaponCamera;
		}
	}
    
	public GameObject WeaponModel
	{
		get
		{
			return this.m_WeaponModel;
		}
	}
    
	public CryptoVector3 DefaultPosition
	{
		get
		{
			return (CryptoVector3)base.DefaultState.Preset.GetFieldValue("PositionOffset");
		}
	}
    
	public CryptoVector3 DefaultRotation
	{
		get
		{
			return (CryptoVector3)base.DefaultState.Preset.GetFieldValue("RotationOffset");
		}
	}
    
	public bool DrawRetractionDebugLine
	{
		get
		{
			return this.m_DrawRetractionDebugLine;
		}
		set
		{
			this.m_DrawRetractionDebugLine = value;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		if (base.transform.parent == null)
		{
			Debug.LogError("Error (" + this + ") Must not be placed in scene root. Disabling self.");
			vp_Utility.Activate(base.gameObject, false);
			return;
		}
		if (PlayerInput.instance != null)
		{
			this.Controller = PlayerInput.instance.mCharacterController;
		}
		else
		{
			this.Controller = base.Transform.root.GetComponent<CharacterController>();
		}
		if (this.Controller == null)
		{
			Debug.LogError("Error (" + this + ") Could not find CharacterController. Disabling self.");
			vp_Utility.Activate(base.gameObject, false);
			return;
		}
		base.Transform.eulerAngles = Vector3.zero;
		foreach (object obj in base.Transform.parent)
		{
			Transform transform = (Transform)obj;
			Camera camera = (Camera)transform.GetComponent(typeof(Camera));
			if (camera != null)
			{
				this.m_WeaponCamera = camera.gameObject;
				break;
			}
		}
		if (base.GetComponent<Collider>() != null)
		{
			base.GetComponent<Collider>().enabled = false;
		}
		if (!vp_FPWeapon.loadCustomData)
		{
			vp_FPWeapon.customRenderingFieldOfView = GameConsole.Load<float>("weapon_fov", 0f);
			vp_FPWeapon.customBob = GameConsole.Load<bool>("weapon_bobup", true);
			vp_FPWeapon.loadCustomData = true;
		}
	}

	protected override void Start()
	{
		base.Start();
		this.InstantiateWeaponModel();
		this.m_WeaponGroup = new GameObject(base.name + "Transform");
		this.m_WeaponGroupTransform = this.m_WeaponGroup.transform;
		this.m_WeaponGroupTransform.parent = base.Transform.parent;
		this.m_WeaponGroupTransform.localPosition = this.PositionOffset + vp_FPWeapon.customPositionOffset;
		vp_Layer.Set(this.m_WeaponGroup, 31, false);
		base.Transform.parent = this.m_WeaponGroupTransform;
		base.Transform.localPosition = Vector3.zero;
		this.m_WeaponGroupTransform.localEulerAngles = this.RotationOffset;
		if (this.m_WeaponCamera != null && vp_Utility.IsActive(this.m_WeaponCamera.gameObject))
		{
			vp_Layer.Set(base.gameObject, 31, true);
		}
		this.m_Pivot = new GameObject("Pivot");
		this.m_Pivot.gameObject.transform.localScale = new Vector3(nValue.float01, nValue.float01, nValue.float01);
		this.m_Pivot.transform.parent = this.m_WeaponGroupTransform;
		this.m_Pivot.transform.localPosition = Vector3.zero;
		this.m_Pivot.layer = 31;
		vp_Utility.Activate(this.m_Pivot.gameObject, false);
		this.m_PositionSpring = new vp_SpringThread();
		this.m_PositionSpring.RestState = this.PositionOffset + vp_FPWeapon.customPositionOffset;
		this.m_PositionPivotSpring = new vp_SpringThread();
		this.m_PositionPivotSpring.RestState = this.PositionPivot;
		this.m_PositionSpring2 = new vp_SpringThread();
		this.m_PositionSpring2.MinVelocity = nValue.float000001;
		this.m_RotationSpring = new vp_SpringThread();
		this.m_RotationSpring.RestState = this.RotationOffset;
		this.m_RotationPivotSpring = new vp_SpringThread();
		this.m_RotationPivotSpring.RestState = this.RotationPivot;
		this.m_RotationSpring2 = new vp_SpringThread();
		this.m_RotationSpring2.MinVelocity = nValue.float000001;
		this.SnapSprings();
		this.Refresh();
	}

	public virtual void InstantiateWeaponModel()
	{
		if (this.WeaponPrefab != null)
		{
			if (this.m_WeaponModel != null && this.m_WeaponModel != base.gameObject)
			{
				UnityEngine.Object.Destroy(this.m_WeaponModel);
			}
			this.m_WeaponModel = (GameObject)UnityEngine.Object.Instantiate(this.WeaponPrefab);
			this.m_WeaponModel.transform.parent = base.transform;
			this.m_WeaponModel.transform.localPosition = Vector3.zero;
			this.m_WeaponModel.transform.localScale = new Vector3(1f, 1f, this.RenderingZScale);
			this.m_WeaponModel.transform.localEulerAngles = Vector3.zero;
			if (this.m_WeaponCamera != null && vp_Utility.IsActive(this.m_WeaponCamera.gameObject))
			{
				vp_Layer.Set(this.m_WeaponModel, 31, true);
			}
		}
		else
		{
			this.m_WeaponModel = base.gameObject;
		}
		base.CacheRenderers();
	}

	protected override void Init()
	{
		base.Init();
		this.ScheduleAmbientAnimation();
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		InputManager.GetAxisEvent = (InputManager.AxisDelegate)Delegate.Combine(InputManager.GetAxisEvent, new InputManager.AxisDelegate(this.GetAxis));
		vp_FPController fpcontroller = PlayerInput.instance.FPController;
		fpcontroller.FallImpactEvent = (Action<float>)Delegate.Combine(fpcontroller.FallImpactEvent, new Action<float>(this.OnMessage_FallImpact));
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		InputManager.GetAxisEvent = (InputManager.AxisDelegate)Delegate.Remove(InputManager.GetAxisEvent, new InputManager.AxisDelegate(this.GetAxis));
		vp_FPController fpcontroller = PlayerInput.instance.FPController;
		fpcontroller.FallImpactEvent = (Action<float>)Delegate.Remove(fpcontroller.FallImpactEvent, new Action<float>(this.OnMessage_FallImpact));
		this.LookAxis = Vector2.zero;
	}

	private void GetAxis(string name, float value)
	{
        if (!Spectator)
        {
            switch (name)
            {
                case "Mouse X":
                    LookAxis.x = value;
                    break;
                case "Mouse Y":
                    LookAxis.y = value;
                    break;
            }
        }
    }

	protected override void Update()
	{
		base.Update();
		this.OnUpdateThread();
		if (this.m_WeaponGroupTransform.localPosition != this.m_PositionSpring.State)
		{
			this.m_WeaponGroupTransform.localPosition = this.m_PositionSpring.State;
		}
		this.m_WeaponGroupTransform.localEulerAngles = this.m_RotationSpring.State + this.m_RotationSpring2.State;
		base.Transform.localPosition = this.m_PositionPivotSpring.State + this.m_PositionSpring2.State;
		if (base.Transform.localEulerAngles != this.m_RotationPivotSpring.State)
		{
			base.Transform.localEulerAngles = this.m_RotationPivotSpring.State;
		}
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		this.controllerVelocity = this.Controller.velocity;
		this.controllerIsGrounded = this.Controller.isGrounded;
		this.swayLocalVelocity = base.Transform.InverseTransformDirection(this.m_SwayVel / 60f);
		this.OnFixedUpdateThread();
	}

	public void OnUpdateThread()
	{
		this.UpdateMouseLook();
	}

	public void OnFixedUpdateThread()
	{
		this.UpdateSwaying();
		this.UpdateBob();
		this.m_PositionSpring.FixedUpdate();
		this.m_PositionPivotSpring.FixedUpdate();
		this.m_RotationPivotSpring.FixedUpdate();
		this.m_PositionSpring2.FixedUpdate();
		this.m_RotationSpring.FixedUpdate();
		this.m_RotationSpring2.FixedUpdate();
	}

	public virtual void AddForce2(Vector3 positional, Vector3 angular)
	{
		this.m_PositionSpring2.AddForce(positional);
		this.m_RotationSpring2.AddForce(angular);
	}

	public virtual void AddForce2(float xPos, float yPos, float zPos, float xRot, float yRot, float zRot)
	{
		this.AddForce2(new Vector3(xPos, yPos, zPos), new Vector3(xRot, yRot, zRot));
	}

	public virtual void AddForce(Vector3 force)
	{
		this.m_PositionSpring.AddForce(force);
	}

	public virtual void AddForce(float x, float y, float z)
	{
		this.AddForce(new Vector3(x, y, z));
	}

	public virtual void AddForce(Vector3 positional, Vector3 angular)
	{
		this.m_PositionSpring.AddForce(positional);
		this.m_RotationSpring.AddForce(angular);
	}

	public virtual void AddForce(float xPos, float yPos, float zPos, float xRot, float yRot, float zRot)
	{
		this.AddForce(new Vector3(xPos, yPos, zPos), new Vector3(xRot, yRot, zRot));
	}

	public virtual void AddSoftForce(Vector3 force, int frames)
	{
		this.m_PositionSpring.AddSoftForce(force, (float)frames);
	}

	public virtual void AddSoftForce(float x, float y, float z, int frames)
	{
		this.AddSoftForce(new Vector3(x, y, z), frames);
	}

	public virtual void AddSoftForce(Vector3 positional, Vector3 angular, int frames)
	{
		this.m_PositionSpring.AddSoftForce(positional, (float)frames);
		this.m_RotationSpring.AddSoftForce(angular, (float)frames);
	}

	public virtual void AddSoftForce(float xPos, float yPos, float zPos, float xRot, float yRot, float zRot, int frames)
	{
		this.AddSoftForce(new Vector3(xPos, yPos, zPos), new Vector3(xRot, yRot, zRot), frames);
	}

	protected virtual void UpdateMouseLook()
	{
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        this.m_MouseMove.x = Input.GetAxis("Mouse X") / (Time.deltaTime * 60f);
        this.m_MouseMove.y = Input.GetAxis("Mouse Y") / (Time.deltaTime * 60f);
#else
        this.m_MouseMove.x = this.LookAxis.x / (Time.deltaTime * 60f);
		this.m_MouseMove.y = this.LookAxis.y / (Time.deltaTime * 60f);
#endif
        this.m_MouseMove *= this.RotationInputVelocityScale;
		this.m_MouseMove = Vector3.Min(this.m_MouseMove, Vector3.one * this.RotationMaxInputVelocity);
		this.m_MouseMove = Vector3.Max(this.m_MouseMove, Vector3.one * -this.RotationMaxInputVelocity);
	}

	protected virtual void UpdateZoom()
	{
		if (this.m_FinalZoomTime <= Time.time)
		{
			return;
		}
		if (!this.m_Wielded)
		{
			return;
		}
		this.RenderingZoomDamping = Mathf.Max(this.RenderingZoomDamping, nValue.float001);
		float t = nValue.float1 - (this.m_FinalZoomTime - Time.time) / this.RenderingZoomDamping;
		if (this.m_WeaponCamera != null && vp_Utility.IsActive(this.m_WeaponCamera.gameObject))
		{
			this.m_WeaponCamera.GetComponent<Camera>().fieldOfView = Mathf.SmoothStep(this.m_WeaponCamera.gameObject.GetComponent<Camera>().fieldOfView, this.RenderingFieldOfView + vp_FPWeapon.customRenderingFieldOfView, t);
		}
	}

	public virtual void Zoom()
	{
		this.m_FinalZoomTime = Time.time + this.RenderingZoomDamping;
	}

	public virtual void SnapZoom()
	{
		if (this.m_WeaponCamera != null && vp_Utility.IsActive(this.m_WeaponCamera.gameObject))
		{
			this.m_WeaponCamera.GetComponent<Camera>().fieldOfView = this.RenderingFieldOfView + vp_FPWeapon.customRenderingFieldOfView;
		}
	}

	protected virtual void UpdateShakes()
	{
		if (this.ShakeSpeed != 0f)
		{
			this.m_Shake = Vector3.Scale(vp_SmoothRandom.GetVector3Centered(this.ShakeSpeed), this.ShakeAmplitude);
			this.m_RotationSpring.AddForce(this.m_Shake);
		}
	}

	protected virtual void UpdateRetraction(bool firstIteration = true)
	{
		if (this.RetractionDistance == 0f)
		{
			return;
		}
		Vector3 vector = this.WeaponModel.transform.TransformPoint(new Vector3(this.RetractionOffset.x, this.RetractionOffset.y, 0f));
		Vector3 end = vector + this.WeaponModel.transform.forward * this.RetractionDistance;
		RaycastHit raycastHit;
		if (Physics.Linecast(vector, end, out raycastHit, -1749041173) && !raycastHit.collider.isTrigger)
		{
			this.WeaponModel.transform.position = raycastHit.point - (raycastHit.point - vector).normalized * (this.RetractionDistance * 0.99f);
			this.WeaponModel.transform.localPosition = Vector3.forward * Mathf.Min(this.WeaponModel.transform.localPosition.z, 0f);
		}
		else if (firstIteration && this.WeaponModel.transform.localPosition != Vector3.zero && this.WeaponModel != base.gameObject)
		{
			this.WeaponModel.transform.localPosition = Vector3.forward * Mathf.SmoothStep(this.WeaponModel.transform.localPosition.z, 0f, this.RetractionRelaxSpeed);
			this.UpdateRetraction(false);
		}
	}

	protected virtual void UpdateBob()
	{
		if (!vp_FPWeapon.customBob)
		{
			return;
		}
		if (this.BobAmplitude == Vector4.zero || this.BobRate == Vector4.zero)
		{
			return;
		}
		if (this.Spectator)
		{
			this.m_BobSpeed = this.SpectatorVelocity;
		}
		else
		{
			this.m_BobSpeed = (this.controllerIsGrounded ? this.controllerVelocity.sqrMagnitude : 0f);
		}
		if (this.m_BobSpeed > 100f)
		{
			this.m_BobSpeed = 100f;
		}
		this.m_BobSpeed = Mathf.Round(this.m_BobSpeed * 1000f) / 1000f;
		if (this.m_BobSpeed == 0f)
		{
			this.m_BobSpeed = this.m_LastBobSpeed * 0.93f;
		}
		this.bobTime += 0.02f;
		this.m_CurrentBobAmp.x = this.m_BobSpeed * (this.BobAmplitude.x * -0.0001f);
		this.m_CurrentBobVal.x = Mathf.Cos(this.bobTime * (this.BobRate.x * 10f)) * this.m_CurrentBobAmp.x;
		this.m_CurrentBobAmp.y = this.m_BobSpeed * (this.BobAmplitude.y * 0.0001f);
		this.m_CurrentBobVal.y = Mathf.Cos(this.bobTime * (this.BobRate.y * 10f)) * this.m_CurrentBobAmp.y;
		this.m_PositionSpring.AddForce(this.m_CurrentBobVal);
		this.m_LastBobSpeed = this.m_BobSpeed;
	}

	protected virtual void UpdateSprings()
	{
		this.m_PositionSpring.FixedUpdate();
		this.m_PositionPivotSpring.FixedUpdate();
		this.m_RotationPivotSpring.FixedUpdate();
		this.m_PositionSpring2.FixedUpdate();
		this.m_RotationSpring.FixedUpdate();
		this.m_RotationSpring2.FixedUpdate();
	}

	protected virtual void UpdateStep()
	{
		if (this.StepMinVelocity <= 0f || !this.Controller.isGrounded || this.Controller.velocity.sqrMagnitude < this.StepMinVelocity)
		{
			return;
		}
		bool flag = this.m_LastUpBob < this.m_CurrentBobVal.x;
		this.m_LastUpBob = this.m_CurrentBobVal.x;
		if (flag && !this.m_BobWasElevating)
		{
			if (Mathf.Cos(Time.time * (this.BobRate.x * 5f)) > 0f)
			{
				this.m_PosStep = this.StepPositionForce - this.StepPositionForce * this.StepPositionBalance;
				this.m_RotStep = this.StepRotationForce - this.StepPositionForce * this.StepRotationBalance;
			}
			else
			{
				this.m_PosStep = this.StepPositionForce + this.StepPositionForce * this.StepPositionBalance;
				this.m_RotStep = Vector3.Scale(this.StepRotationForce - this.StepPositionForce * this.StepRotationBalance, -Vector3.one + Vector3.right * 2f);
			}
			this.AddSoftForce(this.m_PosStep * this.StepForceScale, this.m_RotStep * this.StepForceScale, this.StepSoftness);
		}
		this.m_BobWasElevating = flag;
	}

	protected virtual void UpdateSwaying()
	{
		this.m_SwayVel = this.controllerVelocity * this.PositionInputVelocityScale;
		this.m_SwayVel = Vector3.Min(this.m_SwayVel, Vector3.one * this.PositionMaxInputVelocity);
		this.m_SwayVel = Vector3.Max(this.m_SwayVel, Vector3.one * -this.PositionMaxInputVelocity);
		this.m_RotationSpring.AddForce(new Vector3(this.m_MouseMove.y * (this.RotationLookSway.x * nValue.float0025), this.m_MouseMove.x * (this.RotationLookSway.y * -nValue.float0025), this.m_MouseMove.x * (this.RotationLookSway.z * -nValue.float0025)));
		this.m_FallSway = this.RotationFallSway * (this.m_SwayVel.y * 0.005f);
		if (this.controllerIsGrounded)
		{
			this.m_FallSway *= this.RotationSlopeSway;
		}
		this.m_FallSway.z = Mathf.Max(0f, this.m_FallSway.z);
		this.m_RotationSpring.AddForce(this.m_FallSway);
		this.m_PositionSpring.AddForce(Vector3.forward * -Mathf.Abs(this.m_SwayVel.y * (this.PositionFallRetract * 2.5E-05f)));
		this.m_PositionSpring.AddForce(new Vector3(this.swayLocalVelocity.x * (this.PositionWalkSlide.x * nValue.float00016), -Mathf.Abs(this.swayLocalVelocity.x * (this.PositionWalkSlide.y * nValue.float00016)), -this.swayLocalVelocity.z * (this.PositionWalkSlide.z * nValue.float00016)));
		this.m_RotationSpring.AddForce(new Vector3(-Mathf.Abs(this.swayLocalVelocity.x * (this.RotationStrafeSway.x * nValue.float016)), -(this.swayLocalVelocity.x * (this.RotationStrafeSway.y * nValue.float016)), this.swayLocalVelocity.x * (this.RotationStrafeSway.z * nValue.float016)));
	}

	public virtual void ResetSprings(float positionReset, float rotationReset, float positionPauseTime = 0f, float rotationPauseTime = 0f)
	{
		this.m_PositionSpring.State = Vector3.Lerp(this.m_PositionSpring.State, this.m_PositionSpring.RestState, positionReset);
		this.m_RotationSpring.State = Vector3.Lerp(this.m_RotationSpring.State, this.m_RotationSpring.RestState, rotationReset);
		this.m_PositionPivotSpring.State = Vector3.Lerp(this.m_PositionPivotSpring.State, this.m_PositionPivotSpring.RestState, positionReset);
		this.m_RotationPivotSpring.State = Vector3.Lerp(this.m_RotationPivotSpring.State, this.m_RotationPivotSpring.RestState, rotationReset);
		if (positionPauseTime != 0f)
		{
			this.m_PositionSpring.ForceVelocityFadeIn(positionPauseTime);
		}
		if (rotationPauseTime != 0f)
		{
			this.m_RotationSpring.ForceVelocityFadeIn(rotationPauseTime);
		}
		if (positionPauseTime != 0f)
		{
			this.m_PositionPivotSpring.ForceVelocityFadeIn(positionPauseTime);
		}
		if (rotationPauseTime != 0f)
		{
			this.m_RotationPivotSpring.ForceVelocityFadeIn(rotationPauseTime);
		}
	}

	public override void Refresh()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (this.m_PositionSpring != null)
		{
			this.m_PositionSpring.Stiffness = new Vector3(this.PositionSpringStiffness, this.PositionSpringStiffness, this.PositionSpringStiffness);
			this.m_PositionSpring.Damping = Vector3.one - new Vector3(this.PositionSpringDamping, this.PositionSpringDamping, this.PositionSpringDamping);
			this.m_PositionSpring.RestState = this.PositionOffset + vp_FPWeapon.customPositionOffset - this.PositionPivot;
		}
		if (this.m_PositionPivotSpring != null)
		{
			this.m_PositionPivotSpring.Stiffness = new Vector3(this.PositionPivotSpringStiffness, this.PositionPivotSpringStiffness, this.PositionPivotSpringStiffness);
			this.m_PositionPivotSpring.Damping = Vector3.one - new Vector3(this.PositionPivotSpringDamping, this.PositionPivotSpringDamping, this.PositionPivotSpringDamping);
			this.m_PositionPivotSpring.RestState = this.PositionPivot;
		}
		if (this.m_RotationPivotSpring != null)
		{
			this.m_RotationPivotSpring.Stiffness = new Vector3(this.RotationPivotSpringStiffness, this.RotationPivotSpringStiffness, this.RotationPivotSpringStiffness);
			this.m_RotationPivotSpring.Damping = Vector3.one - new Vector3(this.RotationPivotSpringDamping, this.RotationPivotSpringDamping, this.RotationPivotSpringDamping);
			this.m_RotationPivotSpring.RestState = this.RotationPivot;
		}
		if (this.m_PositionSpring2 != null)
		{
			this.m_PositionSpring2.Stiffness = new Vector3(this.PositionSpring2Stiffness, this.PositionSpring2Stiffness, this.PositionSpring2Stiffness);
			this.m_PositionSpring2.Damping = Vector3.one - new Vector3(this.PositionSpring2Damping, this.PositionSpring2Damping, this.PositionSpring2Damping);
			this.m_PositionSpring2.RestState = Vector3.zero;
		}
		if (this.m_RotationSpring != null)
		{
			this.m_RotationSpring.Stiffness = new Vector3(this.RotationSpringStiffness, this.RotationSpringStiffness, this.RotationSpringStiffness);
			this.m_RotationSpring.Damping = Vector3.one - new Vector3(this.RotationSpringDamping, this.RotationSpringDamping, this.RotationSpringDamping);
			this.m_RotationSpring.RestState = this.RotationOffset;
		}
		if (this.m_RotationSpring2 != null)
		{
			this.m_RotationSpring2.Stiffness = new Vector3(this.RotationSpring2Stiffness, this.RotationSpring2Stiffness, this.RotationSpring2Stiffness);
			this.m_RotationSpring2.Damping = Vector3.one - new Vector3(this.RotationSpring2Damping, this.RotationSpring2Damping, this.RotationSpring2Damping);
			this.m_RotationSpring2.RestState = Vector3.zero;
		}
		if (base.Rendering)
		{
			if (this.m_WeaponCamera != null && vp_Utility.IsActive(this.m_WeaponCamera.gameObject))
			{
				this.m_WeaponCamera.GetComponent<Camera>().nearClipPlane = this.RenderingClippingPlanes.x;
				this.m_WeaponCamera.GetComponent<Camera>().farClipPlane = this.RenderingClippingPlanes.y;
			}
			this.Zoom();
		}
	}

	public override void Activate()
	{
		this.m_Wielded = true;
		base.Rendering = true;
		TimerManager.Cancel(this.m_DeactivationTimer);
		this.SnapZoom();
		if (this.m_WeaponGroup != null && !vp_Utility.IsActive(this.m_WeaponGroup))
		{
			vp_Utility.Activate(this.m_WeaponGroup, true);
		}
		this.SetPivotVisible(false);
	}

	public override void Deactivate()
	{
		this.m_Wielded = false;
		if (this.m_WeaponGroup != null && vp_Utility.IsActive(this.m_WeaponGroup))
		{
			vp_Utility.Activate(this.m_WeaponGroup, false);
		}
	}

	public virtual void SnapPivot()
	{
		if (this.m_PositionSpring != null)
		{
			this.m_PositionSpring.RestState = this.PositionOffset + vp_FPWeapon.customPositionOffset - this.PositionPivot;
			this.m_PositionSpring.State = this.PositionOffset + vp_FPWeapon.customPositionOffset - this.PositionPivot;
		}
		if (this.m_WeaponGroup != null)
		{
			this.m_WeaponGroupTransform.localPosition = this.PositionOffset + vp_FPWeapon.customPositionOffset - this.PositionPivot;
		}
		if (this.m_PositionPivotSpring != null)
		{
			this.m_PositionPivotSpring.RestState = this.PositionPivot;
			this.m_PositionPivotSpring.State = this.PositionPivot;
		}
		if (this.m_RotationPivotSpring != null)
		{
			this.m_RotationPivotSpring.RestState = this.RotationPivot;
			this.m_RotationPivotSpring.State = this.RotationPivot;
		}
		base.Transform.localPosition = this.PositionPivot;
		base.Transform.localEulerAngles = this.RotationPivot;
	}

	public virtual void SetPivotVisible(bool visible)
	{
		if (this.m_Pivot == null)
		{
			return;
		}
		vp_Utility.Activate(this.m_Pivot.gameObject, visible);
	}

	public virtual void SnapToExit()
	{
		this.RotationOffset = this.RotationExitOffset;
		this.PositionOffset = this.PositionExitOffset;
		this.SnapSprings();
		this.SnapPivot();
	}

	public virtual void SnapSprings()
	{
		if (this.m_PositionSpring != null)
		{
			this.m_PositionSpring.RestState = this.PositionOffset + vp_FPWeapon.customPositionOffset - this.PositionPivot;
			this.m_PositionSpring.State = this.PositionOffset + vp_FPWeapon.customPositionOffset - this.PositionPivot;
			this.m_PositionSpring.Stop(true);
		}
		if (this.m_WeaponGroup != null)
		{
			this.m_WeaponGroupTransform.localPosition = this.PositionOffset + vp_FPWeapon.customPositionOffset - this.PositionPivot;
		}
		if (this.m_PositionPivotSpring != null)
		{
			this.m_PositionPivotSpring.RestState = this.PositionPivot;
			this.m_PositionPivotSpring.State = this.PositionPivot;
			this.m_PositionPivotSpring.Stop(true);
		}
		base.Transform.localPosition = this.PositionPivot;
		if (this.m_PositionSpring2 != null)
		{
			this.m_PositionSpring2.RestState = Vector3.zero;
			this.m_PositionSpring2.State = Vector3.zero;
			this.m_PositionSpring2.Stop(true);
		}
		if (this.m_RotationPivotSpring != null)
		{
			this.m_RotationPivotSpring.RestState = this.RotationPivot;
			this.m_RotationPivotSpring.State = this.RotationPivot;
			this.m_RotationPivotSpring.Stop(true);
		}
		base.Transform.localEulerAngles = this.RotationPivot;
		if (this.m_RotationSpring != null)
		{
			this.m_RotationSpring.RestState = this.RotationOffset;
			this.m_RotationSpring.State = this.RotationOffset;
			this.m_RotationSpring.Stop(true);
		}
		if (this.m_RotationSpring2 != null)
		{
			this.m_RotationSpring2.RestState = Vector3.zero;
			this.m_RotationSpring2.State = Vector3.zero;
			this.m_RotationSpring2.Stop(true);
		}
	}

	public virtual void StopSprings()
	{
		if (this.m_PositionSpring != null)
		{
			this.m_PositionSpring.Stop(true);
		}
		if (this.m_PositionPivotSpring != null)
		{
			this.m_PositionPivotSpring.Stop(true);
		}
		if (this.m_PositionSpring2 != null)
		{
			this.m_PositionSpring2.Stop(true);
		}
		if (this.m_RotationSpring != null)
		{
			this.m_RotationSpring.Stop(true);
		}
		if (this.m_RotationPivotSpring != null)
		{
			this.m_RotationPivotSpring.Stop(true);
		}
		if (this.m_RotationSpring2 != null)
		{
			this.m_RotationSpring2.Stop(true);
		}
	}

	public virtual void Wield(bool showWeapon = true)
	{
		if (showWeapon)
		{
			this.SnapToExit();
		}
		if (showWeapon)
		{
			this.PositionOffset = this.DefaultPosition;
			this.RotationOffset = this.DefaultRotation;
		}
		else
		{
			this.PositionOffset = this.PositionExitOffset;
			this.RotationOffset = this.RotationExitOffset;
		}
		this.m_Wielded = showWeapon;
		this.Refresh();
		base.StateManager.CombineStates();
		if (base.Audio != null)
		{
		}
		if (((!showWeapon) ? this.AnimationUnWield : this.AnimationWield) != null && vp_Utility.IsActive(base.gameObject))
		{
			this.m_WeaponModel.GetComponent<Animation>().CrossFade(((!showWeapon) ? this.AnimationUnWield : this.AnimationWield).name);
		}
	}

	public virtual void ScheduleAmbientAnimation()
	{
		if (this.AnimationAmbient.Count == 0 || !vp_Utility.IsActive(base.gameObject))
		{
			return;
		}
		this.m_AnimationAmbientTimer = TimerManager.In(UnityEngine.Random.Range(this.AmbientInterval.x, this.AmbientInterval.y), delegate()
		{
			if (vp_Utility.IsActive(base.gameObject))
			{
				this.m_CurrentAmbientAnimation = UnityEngine.Random.Range(0, this.AnimationAmbient.Count);
				if (this.AnimationAmbient[this.m_CurrentAmbientAnimation] != null)
				{
					this.m_WeaponModel.GetComponent<Animation>().CrossFadeQueued(this.AnimationAmbient[this.m_CurrentAmbientAnimation].name);
					this.ScheduleAmbientAnimation();
				}
			}
		});
	}

	private void OnMessage_FallImpact(float impact)
	{
		if (this.Spectator)
		{
			return;
		}
		if (this.m_PositionSpring != null)
		{
			this.m_PositionSpring.AddSoftForce(Vector3.down * impact * this.PositionKneeling, (float)this.PositionKneelingSoftness);
		}
		if (this.m_RotationSpring != null)
		{
			this.m_RotationSpring.AddSoftForce(Vector3.right * impact * this.RotationKneeling, (float)this.RotationKneelingSoftness);
		}
	}
}
