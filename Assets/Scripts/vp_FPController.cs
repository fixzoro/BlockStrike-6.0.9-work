using System;
using UnityEngine;

public class vp_FPController : vp_Component
{
    public vp_FPCamera FPCamera;

    public Action<float> FallImpactEvent;

    public CharacterController m_CharacterController;

    protected Vector3 m_FixedPosition = Vector3.zero;

    protected Vector3 m_SmoothPosition = Vector3.zero;

    protected bool m_Grounded;

    public RaycastHit m_GroundHit;

    protected RaycastHit m_LastGroundHit;

    protected RaycastHit m_WallHit;

    protected float m_FallImpact;

    public CryptoFloatFast MotorAcceleration = 0.18f;

    public CryptoFloatFast MotorDamping = 0.17f;

    public CryptoFloatFast MotorBackwardsSpeed = 0.65f;

    public CryptoFloatFast MotorAirSpeed = 0.35f;

    public CryptoFloatFast MotorSlopeSpeedUp = 1f;

    public CryptoFloatFast MotorSlopeSpeedDown = 1f;

    public bool MotorFreeFly;

    protected Vector3 m_MoveDirection;

    protected float m_SlopeFactor = nValue.float1;

    protected Vector3 m_MotorThrottle;

    protected float m_MotorAirSpeedModifier = nValue.float1;

    protected float m_CurrentAntiBumpOffset;

    protected Vector2 m_MoveVector;

    public CryptoFloatFast MotorJumpForce = 0.18f;

    public CryptoFloatFast MotorJumpForceDamping = 1.08f;

    protected bool m_MotorJumpDone = true;

    public bool MotorDoubleJump;

    private bool MotorDoubleJumpFirst;

    private bool MotorDoubleJumpFirstDown;

    private bool MotorDoubleJumpSecond;

    protected float m_FallSpeed;

    protected float m_LastFallSpeed;

    protected float m_HighestFallSpeed;

    public CryptoFloatFast PhysicsForceDamping = 1.05f;

    public CryptoFloatFast PhysicsGravityModifier;

    public CryptoFloatFast PhysicsGravityModifierConst = 0.2f;

    public CryptoFloatFast PhysicsSlopeSlideLimit = 30f;

    public CryptoFloatFast PhysicsSlopeSlidiness = 0.15f;

    public CryptoFloatFast PhysicsWallBounce = 0f;

    public CryptoFloatFast PhysicsWallFriction = 0f;

    public bool PhysicsHasCollisionTrigger = true;

    protected GameObject m_Trigger;

    protected Vector3 m_ExternalForce = Vector3.zero;

    protected Vector3[] m_SmoothForceFrame = new Vector3[120];

    protected bool m_Slide;

    protected bool m_SlideFast;

    protected float m_SlideFallSpeed;

    protected float m_OnSteepGroundSince;

    protected float m_SlopeSlideSpeed;

    protected Vector3 m_PredictedPos = Vector3.zero;

    protected Vector3 m_PrevPos = Vector3.zero;

    protected Vector3 m_PrevDir = Vector3.zero;

    protected Vector3 m_NewDir = Vector3.zero;

    protected CryptoFloatFast m_ForceImpact = 0f;

    protected float m_ForceMultiplier;

    protected Vector3 CapsuleBottom = Vector3.zero;

    protected Vector3 CapsuleTop = Vector3.zero;

    protected Transform m_Platform;

    protected Vector3 m_PositionOnPlatform = Vector3.zero;

    protected float m_LastPlatformAngle;

    protected Vector3 m_LastPlatformPos = Vector3.zero;

    private Ray sphereCastRay = new Ray(Vector3.zero, Vector3.down);

    public CharacterController mCharacterController
	{
		get
		{
			if (this.m_CharacterController == null)
			{
				this.m_CharacterController = base.gameObject.GetComponent<CharacterController>();
			}
			return this.m_CharacterController;
		}
	}

	public Vector3 SmoothPosition
	{
		get
		{
			return this.m_SmoothPosition;
		}
	}

	public bool Grounded
	{
		get
		{
			return this.m_Grounded;
		}
	}

	public Vector3 GroundNormal
	{
		get
		{
			return this.m_GroundHit.normal;
		}
	}
    
	public float GroundAngle
	{
		get
		{
			return Vector3.Angle(this.m_GroundHit.normal, Vector3.up);
		}
	}
    
	public Transform GroundTransform
	{
		get
		{
			return this.m_GroundHit.transform;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		this.mCharacterController.center = new Vector3(nValue.float0, this.mCharacterController.height * nValue.float05, nValue.float0);
		this.mCharacterController.radius = nValue.float0375;
		if (Physics.gravity.y != nValue.gravity)
		{
			CheckManager.Detected("Error Gravity");
		}
		TimerManager.In((float)nValue.int5, -1, (float)nValue.int5, new TimerManager.Callback(this.CheckValue));
	}

	private void CheckValue()
	{
		this.MotorAcceleration.CheckValue();
		this.MotorDamping.CheckValue();
		this.MotorBackwardsSpeed.CheckValue();
		this.MotorAirSpeed.CheckValue();
		this.MotorSlopeSpeedUp.CheckValue();
		this.MotorSlopeSpeedDown.CheckValue();
		this.PhysicsForceDamping.CheckValue();
		this.PhysicsGravityModifier.CheckValue();
		this.PhysicsSlopeSlideLimit.CheckValue();
		this.PhysicsSlopeSlidiness.CheckValue();
		this.PhysicsWallBounce.CheckValue();
		this.PhysicsWallFriction.CheckValue();
		this.m_ForceImpact.CheckValue();
	}

	protected override void OnEnable()
	{
		base.OnEnable();
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		this.m_Platform = null;
	}

	protected override void Start()
	{
		base.Start();
		this.SetPosition(base.Transform.position);
		if (this.PhysicsHasCollisionTrigger)
		{
			this.m_Trigger = new GameObject("Trigger");
			this.m_Trigger.transform.parent = this.m_Transform;
			CapsuleCollider capsuleCollider = this.m_Trigger.AddComponent<CapsuleCollider>();
			capsuleCollider.isTrigger = true;
			capsuleCollider.radius = this.mCharacterController.radius + nValue.float008;
			capsuleCollider.height = this.mCharacterController.height + nValue.float008 * 2f;
			capsuleCollider.center = this.mCharacterController.center;
			this.m_Trigger.layer = 30;
			this.m_Trigger.transform.localPosition = Vector3.zero;
		}
	}

	protected override void Update()
	{
		base.Update();
	}

	protected override void LateUpdate()
	{
		base.LateUpdate();
	}

	protected override void FixedUpdate()
	{
		this.UpdateMotor();
		this.UpdateJump();
		this.UpdateForces();
		this.UpdateSliding();
		this.FixedMove();
		this.UpdateCollisions();
		this.UpdatePlatformMove();
		this.m_PrevPos = base.Transform.position;
	}

	public void OnFixedUpdateThread()
	{
		this.UpdateJump();
		this.UpdateForces();
		this.UpdateSliding();
	}

	protected virtual void UpdateMotor()
	{
		if (!this.MotorFreeFly)
		{
			this.UpdateThrottleWalk();
		}
		else
		{
			this.UpdateThrottleFree();
		}
		this.m_MotorThrottle = vp_MathUtility.SnapToZero(this.m_MotorThrottle, 0.0001f);
	}

	protected virtual void UpdateThrottleWalk()
	{
		this.UpdateSlopeFactor();
		if (this.m_Grounded)
		{
			this.m_MotorAirSpeedModifier = nValue.float1;
		}
		else
		{
			this.m_MotorAirSpeedModifier = this.MotorAirSpeed.GetValue();
		}
		this.m_MotorThrottle += this.m_MoveVector.y * (base.Transform.TransformDirection(Vector3.forward * (this.MotorAcceleration.GetValue() * nValue.float01) * this.m_MotorAirSpeedModifier) * this.m_SlopeFactor);
		this.m_MotorThrottle += this.m_MoveVector.x * (base.Transform.TransformDirection(Vector3.right * (this.MotorAcceleration.GetValue() * nValue.float01) * this.m_MotorAirSpeedModifier) * this.m_SlopeFactor);
		this.m_MotorThrottle.x = this.m_MotorThrottle.x / (nValue.float1 + this.MotorDamping.GetValue() * this.m_MotorAirSpeedModifier);
		this.m_MotorThrottle.z = this.m_MotorThrottle.z / (nValue.float1 + this.MotorDamping.GetValue() * this.m_MotorAirSpeedModifier);
	}

	protected virtual void UpdateThrottleFree()
	{
		this.m_MotorThrottle += this.m_MoveVector.y * base.Transform.TransformDirection(base.Transform.InverseTransformDirection(this.FPCamera.Forward) * (this.MotorAcceleration.GetValue() * nValue.float01));
		this.m_MotorThrottle += this.m_MoveVector.x * base.Transform.TransformDirection(Vector3.right * (this.MotorAcceleration.GetValue() * nValue.float01));
		this.m_MotorThrottle.x = this.m_MotorThrottle.x / (nValue.float1 + this.MotorDamping.GetValue());
		this.m_MotorThrottle.z = this.m_MotorThrottle.z / (nValue.float1 + this.MotorDamping.GetValue());
	}

	protected virtual void UpdateJump()
	{
		this.m_MotorThrottle.y = this.m_MotorThrottle.y / this.MotorJumpForceDamping.GetValue();
	}

	protected virtual void UpdateForces()
	{
		if (this.m_Grounded && this.m_FallSpeed <= nValue.float0)
		{
			this.m_FallSpeed = nValue.gravity * (this.PhysicsGravityModifier.GetValue() * nValue.float0002);
		}
		else
		{
			this.m_FallSpeed += nValue.gravity * (this.PhysicsGravityModifier.GetValue() * nValue.float0002);
		}
		if (this.m_FallSpeed < this.m_LastFallSpeed)
		{
			this.m_HighestFallSpeed = this.m_FallSpeed;
		}
		this.m_LastFallSpeed = this.m_FallSpeed;
		if (this.m_SmoothForceFrame[0] != Vector3.zero)
		{
			this.AddForceInternal(this.m_SmoothForceFrame[0]);
			for (int i = 0; i < 120; i++)
			{
				this.m_SmoothForceFrame[i] = ((i >= 119) ? Vector3.zero : this.m_SmoothForceFrame[i + 1]);
				if (this.m_SmoothForceFrame[i] == Vector3.zero)
				{
					break;
				}
			}
		}
		this.m_ExternalForce /= this.PhysicsForceDamping.GetValue();
	}

	protected virtual void UpdateSliding()
	{
		bool slideFast = this.m_SlideFast;
		this.m_Slide = false;
		if (!this.m_Grounded)
		{
			this.m_OnSteepGroundSince = nValue.float0;
			this.m_SlideFast = false;
		}
		else if (this.GroundAngle > this.PhysicsSlopeSlideLimit.GetValue())
		{
			this.m_Slide = true;
			if (this.GroundAngle <= 45f)
			{
				this.m_SlopeSlideSpeed = Mathf.Max(this.m_SlopeSlideSpeed, this.PhysicsSlopeSlidiness.GetValue() * nValue.float001);
				this.m_OnSteepGroundSince = nValue.float0;
				this.m_SlideFast = false;
				this.m_SlopeSlideSpeed = ((Mathf.Abs(this.m_SlopeSlideSpeed) >= nValue.float00001) ? (this.m_SlopeSlideSpeed / 1.05f) : nValue.float0);
			}
			else
			{
				if (this.m_SlopeSlideSpeed > nValue.float001)
				{
					this.m_SlideFast = true;
				}
				if (this.m_OnSteepGroundSince == nValue.float0)
				{
					this.m_OnSteepGroundSince = Time.time;
				}
				this.m_SlopeSlideSpeed += this.PhysicsSlopeSlidiness.GetValue() * nValue.float001 * ((Time.time - this.m_OnSteepGroundSince) * nValue.float0125);
				this.m_SlopeSlideSpeed = Mathf.Max(this.PhysicsSlopeSlidiness.GetValue() * nValue.float001, this.m_SlopeSlideSpeed);
			}
			this.AddForce(Vector3.Cross(Vector3.Cross(this.GroundNormal, Vector3.down), this.GroundNormal) * this.m_SlopeSlideSpeed);
		}
		else
		{
			this.m_OnSteepGroundSince = nValue.float0;
			this.m_SlideFast = false;
			this.m_SlopeSlideSpeed = nValue.float0;
		}
		if (this.m_MotorThrottle != Vector3.zero)
		{
			this.m_Slide = false;
		}
		if (this.m_SlideFast)
		{
			this.m_SlideFallSpeed = base.Transform.position.y;
		}
		else if (slideFast && !this.Grounded)
		{
			this.m_FallSpeed = base.Transform.position.y - this.m_SlideFallSpeed;
			this.m_FallSpeed = Mathf.Clamp(this.m_FallSpeed, nValue.float0, nValue.float001);
		}
	}

	protected virtual void FixedMove()
	{
		this.m_MoveDirection = Vector3.zero;
		this.m_MoveDirection += this.m_ExternalForce;
		this.m_MoveDirection += this.m_MotorThrottle;
		this.m_MoveDirection.y = this.m_MoveDirection.y + this.m_FallSpeed;
		if (this.MotorDoubleJump && this.MotorDoubleJumpFirst && this.Grounded)
		{
			this.MotorDoubleJumpFirst = false;
			this.MotorDoubleJumpFirstDown = false;
			this.MotorDoubleJumpSecond = false;
		}
		this.m_CurrentAntiBumpOffset = nValue.float0;
		if (this.m_Grounded && this.m_MotorThrottle.y <= nValue.float0001)
		{
			this.m_CurrentAntiBumpOffset = Mathf.Max(this.mCharacterController.stepOffset, Vector3.Scale(this.m_MoveDirection, Vector3.one - Vector3.up).magnitude);
			this.m_MoveDirection += this.m_CurrentAntiBumpOffset * Vector3.down;
		}
		this.m_PredictedPos = base.Transform.position + this.m_MoveDirection * base.Delta;
		if (this.m_Platform != null && this.m_PositionOnPlatform != Vector3.zero)
		{
			this.mCharacterController.Move(this.m_Platform.TransformPoint(this.m_PositionOnPlatform) - this.m_Transform.position);
		}
		this.mCharacterController.Move(this.m_MoveDirection * base.Delta);
		this.sphereCastRay.origin = base.Transform.position + Vector3.up * this.mCharacterController.radius;
		Physics.SphereCast(this.sphereCastRay, this.mCharacterController.radius, out this.m_GroundHit, nValue.float008 + nValue.float0001, -1749041173);
		this.m_Grounded = (this.m_GroundHit.collider != null);
		if (this.m_GroundHit.transform == null && this.m_LastGroundHit.transform != null)
		{
			if (this.m_Platform != null && this.m_PositionOnPlatform != Vector3.zero)
			{
				this.AddForce(this.m_Platform.position - this.m_LastPlatformPos);
				this.m_Platform = null;
			}
			if (this.m_CurrentAntiBumpOffset != nValue.float0)
			{
				this.mCharacterController.Move(this.m_CurrentAntiBumpOffset * Vector3.up * base.Delta);
				this.m_PredictedPos += this.m_CurrentAntiBumpOffset * Vector3.up * base.Delta;
				this.m_MoveDirection += this.m_CurrentAntiBumpOffset * Vector3.up;
			}
		}
	}

	protected virtual void UpdateCollisions()
	{
		if (this.m_GroundHit.transform != null && this.m_GroundHit.transform != this.m_LastGroundHit.transform)
		{
			if (!this.MotorFreeFly)
			{
				this.m_FallImpact = -this.m_HighestFallSpeed;
			}
			else
			{
				this.m_FallImpact = -(this.mCharacterController.velocity.y * nValue.float001);
			}
			this.m_SmoothPosition.y = base.Transform.position.y;
			this.DeflectDownForce();
			this.m_HighestFallSpeed = nValue.float0;
			if (this.FallImpactEvent != null)
			{
				this.FallImpactEvent(this.m_FallImpact);
			}
			this.m_MotorThrottle.y = nValue.float0;
			if (this.m_GroundHit.collider.gameObject.layer == 28)
			{
				this.m_Platform = this.m_GroundHit.transform;
				this.m_LastPlatformAngle = this.m_Platform.eulerAngles.y;
			}
			else
			{
				this.m_Platform = null;
			}
		}
		else
		{
			this.m_FallImpact = nValue.float0;
		}
		this.m_LastGroundHit = this.m_GroundHit;
		if (this.m_PredictedPos.x != base.Transform.position.x || (this.m_PredictedPos.z != base.Transform.position.z && this.m_ExternalForce != Vector3.zero))
		{
			this.DeflectHorizontalForce();
		}
	}

	protected virtual void UpdateSlopeFactor()
	{
		if (!this.m_Grounded)
		{
			this.m_SlopeFactor = nValue.float1;
			return;
		}
		this.m_SlopeFactor = nValue.float1 + (nValue.float1 - Vector3.Angle(this.m_GroundHit.normal, this.m_MotorThrottle) / (float)nValue.int90);
		if (Mathf.Abs(nValue.float1 - this.m_SlopeFactor) < nValue.float001)
		{
			this.m_SlopeFactor = nValue.float1;
		}
		else if (this.m_SlopeFactor > nValue.float1)
		{
			if (this.MotorSlopeSpeedDown.GetValue() == nValue.float1)
			{
				this.m_SlopeFactor = nValue.float1 / this.m_SlopeFactor;
				this.m_SlopeFactor *= nValue.float12;
			}
			else
			{
				this.m_SlopeFactor *= this.MotorSlopeSpeedDown.GetValue();
			}
		}
		else
		{
			if (this.MotorSlopeSpeedUp.GetValue() == nValue.float1)
			{
				this.m_SlopeFactor *= nValue.float12;
			}
			else
			{
				this.m_SlopeFactor *= this.MotorSlopeSpeedUp.GetValue();
			}
			if (this.GroundAngle > this.mCharacterController.slopeLimit)
			{
				this.m_SlopeFactor = nValue.float0;
			}
		}
	}

	protected virtual void UpdatePlatformMove()
	{
		if (this.m_Platform == null)
		{
			return;
		}
		this.m_PositionOnPlatform = this.m_Platform.InverseTransformPoint(this.m_Transform.position);
		this.FPCamera.Angle = new Vector2(this.FPCamera.Angle.x, this.FPCamera.Angle.y - Mathf.DeltaAngle(this.m_Platform.eulerAngles.y, this.m_LastPlatformAngle));
		this.m_LastPlatformAngle = this.m_Platform.eulerAngles.y;
		this.m_LastPlatformPos = this.m_Platform.position;
		this.m_SmoothPosition = base.Transform.position;
	}

	public virtual void SetPosition(Vector3 position)
	{
		base.Transform.position = position;
		this.m_PrevPos = position;
		this.m_SmoothPosition = position;
		this.m_Platform = null;
	}

	protected virtual void AddForceInternal(Vector3 force)
	{
		this.m_ExternalForce += force;
	}

	public virtual void AddForce(float x, float y, float z)
	{
		this.AddForce(new Vector3(x, y, z));
	}

	public virtual void AddForce(Vector3 force)
	{
		this.AddForceInternal(force);
	}

	public virtual void AddSoftForce(Vector3 force, float frames)
	{
		frames = Mathf.Clamp(frames, nValue.float1, 120f);
		this.AddForceInternal(force / frames);
		for (int i = 0; i < Mathf.RoundToInt(frames) - nValue.int1; i++)
		{
			this.m_SmoothForceFrame[i] += force / frames;
		}
	}

	public virtual void StopSoftForce()
	{
		for (int i = 0; i < 120; i++)
		{
			if (this.m_SmoothForceFrame[i] == Vector3.zero)
			{
				break;
			}
			this.m_SmoothForceFrame[i] = Vector3.zero;
		}
	}

	public virtual void Stop()
	{
		this.mCharacterController.Move(Vector2.zero);
		this.m_MotorThrottle = Vector3.zero;
		this.m_ExternalForce = Vector2.zero;
		this.StopSoftForce();
		this.m_MoveVector = Vector2.zero;
		this.m_FallSpeed = nValue.float0;
		this.m_LastFallSpeed = nValue.float0;
		this.m_HighestFallSpeed = nValue.float0;
		this.m_SmoothPosition = base.Transform.position;
	}

	public virtual void DeflectDownForce()
	{
		if (this.GroundAngle > this.PhysicsSlopeSlideLimit.GetValue())
		{
			this.m_SlopeSlideSpeed = this.m_FallImpact * nValue.float025;
		}
		if (this.GroundAngle > 85f)
		{
			this.m_MotorThrottle += vp_3DUtility.HorizontalVector(this.GroundNormal * this.m_FallImpact);
			this.m_Grounded = false;
		}
	}

	protected virtual void DeflectHorizontalForce()
	{
		this.m_PredictedPos.y = base.Transform.position.y;
		this.m_PrevPos.y = base.Transform.position.y;
		this.m_PrevDir = (this.m_PredictedPos - this.m_PrevPos).normalized;
		this.CapsuleBottom = this.m_PrevPos + Vector3.up * this.mCharacterController.radius;
		this.CapsuleTop = this.CapsuleBottom + Vector3.up * (this.mCharacterController.height - this.mCharacterController.radius * 2f);
		if (!Physics.CapsuleCast(this.CapsuleBottom, this.CapsuleTop, this.mCharacterController.radius, this.m_PrevDir, out this.m_WallHit, Vector3.Distance(this.m_PrevPos, this.m_PredictedPos), -1749041173))
		{
			return;
		}
		this.m_NewDir = Vector3.Cross(this.m_WallHit.normal, Vector3.up).normalized;
		if (Vector3.Dot(Vector3.Cross(this.m_WallHit.point - base.Transform.position, this.m_PrevPos - base.Transform.position), Vector3.up) > nValue.float0)
		{
			this.m_NewDir = -this.m_NewDir;
		}
		this.m_ForceMultiplier = Mathf.Abs(Vector3.Dot(this.m_PrevDir, this.m_NewDir)) * (nValue.float1 - this.PhysicsWallFriction.GetValue());
		if (this.PhysicsWallBounce.GetValue() > nValue.float0)
		{
			this.m_NewDir = Vector3.Lerp(this.m_NewDir, Vector3.Reflect(this.m_PrevDir, this.m_WallHit.normal), this.PhysicsWallBounce.GetValue());
			this.m_ForceMultiplier = Mathf.Lerp(this.m_ForceMultiplier, nValue.float1, this.PhysicsWallBounce.GetValue() * (nValue.float1 - this.PhysicsWallFriction.GetValue()));
		}
		this.m_ForceImpact = nValue.float0;
		float y = this.m_ExternalForce.y;
		this.m_ExternalForce.y = nValue.float0;
		this.m_ForceImpact = this.m_ExternalForce.magnitude;
		this.m_ExternalForce = this.m_NewDir * this.m_ExternalForce.magnitude * this.m_ForceMultiplier;
		this.m_ForceImpact -= this.m_ExternalForce.magnitude;
		for (int i = 0; i < 120; i++)
		{
			if (this.m_SmoothForceFrame[i] == Vector3.zero)
			{
				break;
			}
			this.m_SmoothForceFrame[i] = this.m_SmoothForceFrame[i].magnitude * this.m_NewDir * this.m_ForceMultiplier;
		}
		this.m_ExternalForce.y = y;
	}

	public bool CanStartJump()
	{
		if (this.MotorDoubleJump)
		{
			if (!this.MotorDoubleJumpFirst)
			{
				return true;
			}
			if (this.MotorDoubleJumpFirst && !this.MotorDoubleJumpFirstDown)
			{
				return false;
			}
			if (this.MotorDoubleJumpFirst && this.MotorDoubleJumpFirstDown && !this.MotorDoubleJumpSecond)
			{
				return true;
			}
			if (this.MotorDoubleJumpFirst && this.MotorDoubleJumpSecond)
			{
				return false;
			}
		}
		return this.MotorFreeFly || (this.m_Grounded && this.m_MotorJumpDone && this.GroundAngle <= this.mCharacterController.slopeLimit);
	}

	public void OnStartJump()
	{
		this.m_MotorJumpDone = false;
		if (this.MotorDoubleJump)
		{
			if (!this.MotorDoubleJumpFirst)
			{
				this.MotorDoubleJumpFirst = true;
			}
			else
			{
				this.m_FallSpeed = nValue.float0;
				this.m_LastFallSpeed = nValue.float0;
				this.m_HighestFallSpeed = nValue.float0;
				this.MotorDoubleJumpSecond = true;
			}
		}
		if (this.MotorFreeFly && !this.Grounded)
		{
			return;
		}
		this.m_MotorThrottle.y = this.MotorJumpForce.GetValue();
		this.m_SmoothPosition.y = base.Transform.position.y;
	}

	public void OnStopJump()
	{
		this.m_MotorJumpDone = true;
		if (this.MotorDoubleJump && this.MotorDoubleJumpFirst)
		{
			this.MotorDoubleJumpFirstDown = true;
		}
	}

	public Vector2 OnValue_InputMoveVector
	{
		get
		{
			return this.m_MoveVector;
		}
		set
		{
			this.m_MoveVector = ((value.y >= 0f) ? value : (value * this.MotorBackwardsSpeed.GetValue()));
		}
	}

	public class xRaycastHit
	{
		public Vector3 normal;

		public Vector3 point;

		public Transform transform;

		public GameObject gameObject;
	}
}
