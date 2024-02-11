using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(AudioListener))]
public class vp_FPCamera : vp_Component
{
    public bool EnableCameraCollision;

    public vp_FPController FPController;

    public Vector2 MouseSensitivity = new Vector2(5f, 5f);

    public int MouseSmoothSteps = 10;

    public float MouseSmoothWeight = 0.5f;

    public bool MouseAcceleration;

    public float MouseAccelerationThreshold = 0.4f;

    protected Vector2 m_MouseMove = Vector2.zero;

    protected List<Vector2> m_MouseSmoothBuffer = new List<Vector2>();

    public CryptoFloat RenderingFieldOfView = 60f;

    public CryptoFloat RenderingZoomDamping = 0.2f;

    protected float m_FinalZoomTime;

    public CryptoVector3 PositionOffset;

    public CryptoFloat PositionGroundLimit = 0.1f;

    public CryptoFloat PositionSpringStiffness = 0.01f;

    public CryptoFloat PositionSpringDamping = 0.25f;

    public CryptoFloat PositionSpring2Stiffness = 0.95f;

    public CryptoFloat PositionSpring2Damping = 0.25f;

    public CryptoFloat PositionKneeling = 0.025f;

    public CryptoInt PositionKneelingSoftness = 1;

    public CryptoFloat PositionEarthQuakeFactor = 1f;

    protected vp_SpringThread m_PositionSpring;

    protected vp_SpringThread m_PositionSpring2;

    protected bool m_DrawCameraCollisionDebugLine;

    public Vector2 RotationPitchLimit = new Vector2(90f, -90f);

    public Vector2 RotationYawLimit = new Vector2(-360f, 360f);

    public CryptoFloat RotationSpringStiffness = 0.01f;

    public CryptoFloat RotationSpringDamping = 0.25f;

    public CryptoFloat RotationKneeling = 0.025f;

    public CryptoInt RotationKneelingSoftness = 1;

    public CryptoFloat RotationStrafeRoll = 0.01f;

    public CryptoFloat RotationEarthQuakeFactor = 0f;

    protected float m_Pitch;

    protected float m_Yaw;

    protected vp_SpringThread m_RotationSpring;

    protected Vector2 m_InitialRotation = Vector2.zero;

    public float ShakeSpeed;

    public Vector3 ShakeAmplitude = new Vector3(10f, 10f, 0f);

    protected Vector3 m_Shake = Vector3.zero;

    public Vector4 BobRate = new Vector4(0f, 1.4f, 0f, 0.7f);

    public Vector4 BobAmplitude = new Vector4(0f, 0.25f, 0f, 0.5f);

    public float BobInputVelocityScale = 1f;

    public float BobMaxInputVelocity = 100f;

    public bool BobRequireGroundContact = true;

    protected float m_LastBobSpeed;

    protected Vector4 m_CurrentBobAmp = Vector4.zero;

    protected Vector4 m_CurrentBobVal = Vector4.zero;

    protected float m_BobSpeed;

    public vp_FPCamera.BobStepDelegate BobStepCallback;

    public float BobStepThreshold = 10f;

    protected float m_LastUpBob;

    protected bool m_BobWasElevating;

    protected Vector3 m_CameraCollisionStartPos = Vector3.zero;

    protected Vector3 m_CameraCollisionEndPos = Vector3.zero;

    protected RaycastHit m_CameraHit;

    private bool controllerGrounded;

    private Vector3 controllerVelocity;

    private float bobTime;

    public delegate void BobStepDelegate();

    public bool DrawCameraCollisionDebugLine
	{
		get
		{
			return this.m_DrawCameraCollisionDebugLine;
		}
		set
		{
			this.m_DrawCameraCollisionDebugLine = value;
		}
	}

	public Vector2 Angle
	{
		get
		{
			return new Vector2(this.m_Pitch, this.m_Yaw);
		}
		set
		{
			this.Pitch = value.x;
			this.Yaw = value.y;
		}
	}

	public Vector3 Forward
	{
		get
		{
			return this.m_Transform.forward;
		}
	}

	public float Pitch
	{
		get
		{
			return this.m_Pitch;
		}
		set
		{
			if (value > (float)nValue.int90)
			{
				value -= (float)nValue.int360;
			}
			this.m_Pitch = value;
		}
	}

	public float Yaw
	{
		get
		{
			return this.m_Yaw;
		}
		set
		{
			this.m_InitialRotation = Vector2.zero;
			this.m_Yaw = value;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		if (this.FPController == null)
		{
			this.FPController = base.Root.GetComponent<vp_FPController>();
		}
		this.m_InitialRotation = new Vector2(base.Transform.eulerAngles.y, base.Transform.eulerAngles.x);
		base.Parent.gameObject.layer = 30;
		foreach (object obj in base.Parent)
		{
			Transform transform = (Transform)obj;
			transform.gameObject.layer = 30;
		}
		base.GetComponent<Camera>().cullingMask &= ~(nValue.int1 << 30 | nValue.int1 << 31);
		base.GetComponent<Camera>().depth = (float)nValue.int0;
		foreach (object obj2 in base.Transform)
		{
			Transform transform2 = (Transform)obj2;
			Camera camera = (Camera)transform2.GetComponent(typeof(Camera));
			if (camera != null)
			{
				camera.transform.localPosition = Vector3.zero;
				camera.transform.localEulerAngles = Vector3.zero;
				camera.clearFlags = CameraClearFlags.Depth;
				camera.cullingMask = nValue.int1 << 31;
				camera.depth = (float)nValue.int1;
				camera.farClipPlane = (float)nValue.int100;
				camera.nearClipPlane = nValue.float001;
				camera.fieldOfView = (float)nValue.int60;
				break;
			}
		}
		this.m_PositionSpring = new vp_SpringThread();
		this.m_PositionSpring.MinVelocity = nValue.float000001;
		this.m_PositionSpring.RestState = this.PositionOffset;
		this.m_PositionSpring2 = new vp_SpringThread();
		this.m_PositionSpring2.MinVelocity = nValue.float000001;
		this.m_RotationSpring = new vp_SpringThread();
		this.m_RotationSpring.MinVelocity = nValue.float000001;
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		vp_FPController fpcontroller = this.FPController;
		fpcontroller.FallImpactEvent = (Action<float>)Delegate.Combine(fpcontroller.FallImpactEvent, new Action<float>(this.OnMessage_FallImpact));
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		vp_FPController fpcontroller = this.FPController;
		fpcontroller.FallImpactEvent = (Action<float>)Delegate.Remove(fpcontroller.FallImpactEvent, new Action<float>(this.OnMessage_FallImpact));
	}

	protected override void Start()
	{
		base.Start();
		this.Refresh();
		this.SnapSprings();
		this.SnapZoom();
	}

	protected override void Init()
	{
		base.Init();
	}

	protected override void Update()
	{
		base.Update();
		this.controllerGrounded = this.FPController.Grounded;
		this.controllerVelocity = this.FPController.mCharacterController.velocity;
		this.DetectBobStep(this.m_BobSpeed, this.m_CurrentBobVal.y);
        #if !UNITY_EDITOR
        this.OnUpdateThread();
        #endif
		this.OnFixedUpdateThread();
	}

    #if UNITY_EDITOR
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        this.OnUpdateThread();
    }
    #endif

    public void OnUpdateThread()
	{
		this.UpdateBob();
		this.UpdateShakes();
	}

	private void OnFixedUpdateThread()
	{
		this.m_PositionSpring.FixedUpdate();
		this.m_PositionSpring2.FixedUpdate();
		this.m_RotationSpring.FixedUpdate();
	}

	protected override void LateUpdate()
	{
		base.LateUpdate();
		this.m_Transform.localPosition = this.m_PositionSpring.State + this.m_PositionSpring2.State;
		this.DoCameraCollision();
		Quaternion lhs = Quaternion.AngleAxis(this.m_Yaw + this.m_InitialRotation.x, Vector3.up);
		Quaternion rhs = Quaternion.AngleAxis((float)nValue.int0, Vector3.left);
		base.Parent.rotation = vp_MathUtility.NaNSafeQuaternion(lhs * rhs, base.Parent.rotation);
		SkyboxManager.GetCameraParent().rotation = base.Parent.rotation;
		rhs = Quaternion.AngleAxis(-this.m_Pitch - this.m_InitialRotation.y, Vector3.left);
		base.Transform.rotation = vp_MathUtility.NaNSafeQuaternion(lhs * rhs, base.Transform.rotation);
		base.Transform.localEulerAngles += vp_MathUtility.NaNSafeVector3(Vector3.forward * this.m_RotationSpring.State.z, default(Vector3));
		SkyboxManager.GetCamera().rotation = base.Transform.rotation;
	}

	protected virtual void DoCameraCollision()
	{
		if (!this.EnableCameraCollision)
		{
			return;
		}
		this.m_CameraCollisionStartPos = this.FPController.Transform.TransformPoint((float)nValue.int0, this.PositionOffset.y, (float)nValue.int0);
		this.m_CameraCollisionEndPos = base.Transform.position + (base.Transform.position - this.m_CameraCollisionStartPos).normalized * this.FPController.mCharacterController.radius;
		if (Physics.Linecast(this.m_CameraCollisionStartPos, this.m_CameraCollisionEndPos, out this.m_CameraHit, -1749041173) && !this.m_CameraHit.collider.isTrigger)
		{
			base.Transform.position = this.m_CameraHit.point - (this.m_CameraHit.point - this.m_CameraCollisionStartPos).normalized * this.FPController.mCharacterController.radius;
		}
		if (base.Transform.localPosition.y < this.PositionGroundLimit)
		{
			base.Transform.localPosition = new Vector3(base.Transform.localPosition.x, this.PositionGroundLimit, base.Transform.localPosition.z);
		}
	}

	public virtual void AddForce(Vector3 force)
	{
		this.m_PositionSpring.AddForce(force);
	}

	public virtual void AddForce(float x, float y, float z)
	{
		this.AddForce(new Vector3(x, y, z));
	}

	public virtual void AddForce2(Vector3 force)
	{
		this.m_PositionSpring2.AddForce(force);
	}

	public void AddForce2(float x, float y, float z)
	{
		this.AddForce2(new Vector3(x, y, z));
	}

	public virtual void AddRollForce(float force)
	{
		this.m_RotationSpring.AddForce(Vector3.forward * force);
	}

	public virtual void AddRotationForce(Vector3 force)
	{
		this.m_RotationSpring.AddForce(force);
	}

	public void AddRotationForce(float x, float y, float z)
	{
		this.AddRotationForce(new Vector3(x, y, z));
	}

	public void UpdateLook(Vector2 look)
	{
		this.UpdateMouseLook(look);
	}

	protected virtual void UpdateMouseLook(Vector2 look)
	{
		this.m_MouseMove.x = look.x;
		this.m_MouseMove.y = look.y;
		this.MouseSmoothSteps = Mathf.Clamp(this.MouseSmoothSteps, nValue.int1, nValue.int20);
		this.MouseSmoothWeight = Mathf.Clamp01(this.MouseSmoothWeight);
		while (this.m_MouseSmoothBuffer.Count > this.MouseSmoothSteps)
		{
			this.m_MouseSmoothBuffer.RemoveAt(0);
		}
		this.m_MouseSmoothBuffer.Add(this.m_MouseMove);
		float num = (float)nValue.int1;
		Vector2 a = Vector2.zero;
		float num2 = (float)nValue.int0;
		for (int i = this.m_MouseSmoothBuffer.Count - 1; i > 0; i--)
		{
			a += this.m_MouseSmoothBuffer[i] * num;
			num2 += (float)nValue.int1 * num;
			num *= this.MouseSmoothWeight / base.Delta;
		}
		num2 = Mathf.Max((float)nValue.int1, num2);
		Vector2 vector = vp_MathUtility.NaNSafeVector2(a / num2, default(Vector2));
		float num3 = (float)nValue.int0;
		float num4 = Mathf.Abs(vector.x);
		float num5 = Mathf.Abs(vector.y);
		if (this.MouseAcceleration)
		{
			num3 = Mathf.Sqrt(num4 * num4 + num5 * num5) / base.Delta;
			num3 = ((num3 > this.MouseAccelerationThreshold) ? num3 : ((float)nValue.int0));
		}
		this.m_Yaw += vector.x * (this.MouseSensitivity.x + num3);
		this.m_Pitch -= vector.y * (this.MouseSensitivity.y + num3);
		this.m_Yaw = ((this.m_Yaw >= (float)(-(float)nValue.int360)) ? this.m_Yaw : (this.m_Yaw += (float)nValue.int360));
		this.m_Yaw = ((this.m_Yaw <= (float)nValue.int360) ? this.m_Yaw : (this.m_Yaw -= (float)nValue.int360));
		this.m_Yaw = Mathf.Clamp(this.m_Yaw, this.RotationYawLimit.x, this.RotationYawLimit.y);
		this.m_Pitch = ((this.m_Pitch >= (float)(-(float)nValue.int360)) ? this.m_Pitch : (this.m_Pitch += (float)nValue.int360));
		this.m_Pitch = ((this.m_Pitch <= (float)nValue.int360) ? this.m_Pitch : (this.m_Pitch -= (float)nValue.int360));
		this.m_Pitch = Mathf.Clamp(this.m_Pitch, -this.RotationPitchLimit.x, -this.RotationPitchLimit.y);
	}

	protected virtual void UpdateZoom()
	{
		if (this.m_FinalZoomTime <= Time.time)
		{
			return;
		}
		this.RenderingZoomDamping = Mathf.Max(this.RenderingZoomDamping, nValue.float001);
		float t = (float)nValue.int1 - (this.m_FinalZoomTime - Time.time) / this.RenderingZoomDamping;
		base.gameObject.GetComponent<Camera>().fieldOfView = Mathf.SmoothStep(base.gameObject.GetComponent<Camera>().fieldOfView, this.RenderingFieldOfView, t);
	}

	public virtual void Zoom()
	{
		this.m_FinalZoomTime = Time.time + this.RenderingZoomDamping;
	}

	public virtual void SnapZoom()
	{
		base.gameObject.GetComponent<Camera>().fieldOfView = this.RenderingFieldOfView;
	}

	protected virtual void UpdateShakes()
	{
		if (this.ShakeSpeed != nValue.float0)
		{
			this.m_Yaw -= this.m_Shake.y;
			this.m_Pitch -= this.m_Shake.x;
			this.m_Shake = Vector3.Scale(vp_SmoothRandom.GetVector3Centered(this.ShakeSpeed), this.ShakeAmplitude);
			this.m_Yaw += this.m_Shake.y;
			this.m_Pitch += this.m_Shake.x;
			this.m_RotationSpring.AddForce(Vector3.forward * this.m_Shake.z);
		}
	}

	protected virtual void UpdateBob()
	{
		if (this.BobAmplitude == Vector4.zero || this.BobRate == Vector4.zero)
		{
			return;
		}
		this.m_BobSpeed = ((!this.BobRequireGroundContact || this.controllerGrounded) ? this.controllerVelocity.sqrMagnitude : nValue.float0);
		this.m_BobSpeed = Mathf.Min(this.m_BobSpeed * this.BobInputVelocityScale, this.BobMaxInputVelocity);
		this.m_BobSpeed = Mathf.Round(this.m_BobSpeed * (float)nValue.int1000) / (float)nValue.int1000;
		if (this.m_BobSpeed == (float)nValue.int0)
		{
			this.m_BobSpeed = Mathf.Min(this.m_LastBobSpeed * 0.93f, this.BobMaxInputVelocity);
		}
		this.bobTime += Time.deltaTime;
		this.m_CurrentBobAmp.y = this.m_BobSpeed * (this.BobAmplitude.y * -nValue.float00001);
		this.m_CurrentBobVal.y = Mathf.Cos(this.bobTime * (this.BobRate.y * (float)nValue.int10)) * this.m_CurrentBobAmp.y;
		this.m_CurrentBobAmp.w = this.m_BobSpeed * (this.BobAmplitude.w * nValue.float00001);
		this.m_CurrentBobVal.w = Mathf.Cos(this.bobTime * (this.BobRate.w * (float)nValue.int10)) * this.m_CurrentBobAmp.w;
		this.m_PositionSpring.AddForce(this.m_CurrentBobVal);
		this.AddRollForce(this.m_CurrentBobVal.w);
		this.m_LastBobSpeed = this.m_BobSpeed;
	}

	protected virtual void DetectBobStep(float speed, float upBob)
	{
		if (this.BobStepCallback == null)
		{
			return;
		}
		if (speed < this.BobStepThreshold)
		{
			return;
		}
		bool flag = this.m_LastUpBob < upBob;
		this.m_LastUpBob = upBob;
		if (flag && !this.m_BobWasElevating)
		{
			this.BobStepCallback();
		}
		this.m_BobWasElevating = flag;
	}

	protected virtual void UpdateSwaying()
	{
		this.AddRollForce(base.Transform.InverseTransformDirection(this.FPController.mCharacterController.velocity * 0.016f).x * this.RotationStrafeRoll);
	}

	protected virtual void UpdateSprings()
	{
		this.m_PositionSpring.FixedUpdate();
		this.m_PositionSpring2.FixedUpdate();
		this.m_RotationSpring.FixedUpdate();
	}

	public virtual void DoBomb(Vector3 positionForce, float minRollForce, float maxRollForce)
	{
		this.AddForce2(positionForce);
		float num = UnityEngine.Random.Range(minRollForce, maxRollForce);
		if (UnityEngine.Random.value > nValue.float05)
		{
			num = -num;
		}
		this.AddRollForce(num);
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
			this.m_PositionSpring.MinState.y = this.PositionGroundLimit;
			this.m_PositionSpring.RestState = this.PositionOffset;
		}
		if (this.m_PositionSpring2 != null)
		{
			this.m_PositionSpring2.Stiffness = new Vector3(this.PositionSpring2Stiffness, this.PositionSpring2Stiffness, this.PositionSpring2Stiffness);
			this.m_PositionSpring2.Damping = Vector3.one - new Vector3(this.PositionSpring2Damping, this.PositionSpring2Damping, this.PositionSpring2Damping);
			this.m_PositionSpring2.MinState.y = -this.PositionOffset.y + this.PositionGroundLimit;
		}
		if (this.m_RotationSpring != null)
		{
			this.m_RotationSpring.Stiffness = new Vector3(this.RotationSpringStiffness, this.RotationSpringStiffness, this.RotationSpringStiffness);
			this.m_RotationSpring.Damping = Vector3.one - new Vector3(this.RotationSpringDamping, this.RotationSpringDamping, this.RotationSpringDamping);
		}
		this.Zoom();
	}

	public virtual void SnapSprings()
	{
		if (this.m_PositionSpring != null)
		{
			this.m_PositionSpring.RestState = this.PositionOffset;
			this.m_PositionSpring.State = this.PositionOffset;
			this.m_PositionSpring.Stop(true);
		}
		if (this.m_PositionSpring2 != null)
		{
			this.m_PositionSpring2.RestState = Vector3.zero;
			this.m_PositionSpring2.State = Vector3.zero;
			this.m_PositionSpring2.Stop(true);
		}
		if (this.m_RotationSpring != null)
		{
			this.m_RotationSpring.RestState = Vector3.zero;
			this.m_RotationSpring.State = Vector3.zero;
			this.m_RotationSpring.Stop(true);
		}
	}

	public virtual void StopSprings()
	{
		if (this.m_PositionSpring != null)
		{
			this.m_PositionSpring.Stop(true);
		}
		if (this.m_PositionSpring2 != null)
		{
			this.m_PositionSpring2.Stop(true);
		}
		if (this.m_RotationSpring != null)
		{
			this.m_RotationSpring.Stop(true);
		}
		this.m_BobSpeed = nValue.float0;
		this.m_LastBobSpeed = nValue.float0;
	}

	public virtual void Stop()
	{
		this.SnapSprings();
		this.SnapZoom();
		this.Refresh();
	}

	public virtual void SetRotation(Vector2 eulerAngles, bool stop = true, bool resetInitialRotation = true)
	{
		this.Angle = eulerAngles;
		if (stop)
		{
			this.Stop();
		}
		if (resetInitialRotation)
		{
			this.m_InitialRotation = Vector2.zero;
		}
	}

	public void OnMessage_FallImpact(float impact)
	{
		impact = Mathf.Abs(impact * 55f);
		float num = impact * this.PositionKneeling;
		float num2 = impact * this.RotationKneeling;
		num = Mathf.SmoothStep((float)nValue.int0, (float)nValue.int1, num);
		num2 = Mathf.SmoothStep((float)nValue.int0, (float)nValue.int1, num2);
		num2 = Mathf.SmoothStep((float)nValue.int0, (float)nValue.int1, num2);
		if (this.m_PositionSpring != null)
		{
			this.m_PositionSpring.AddSoftForce(Vector3.down * num, (float)this.PositionKneelingSoftness);
		}
		if (this.m_RotationSpring != null)
		{
			float d = (UnityEngine.Random.value <= nValue.float05) ? (-(num2 * (float)nValue.int2)) : (num2 * (float)nValue.int2);
			this.m_RotationSpring.AddSoftForce(Vector3.forward * d, (float)this.RotationKneelingSoftness);
		}
	}
}
