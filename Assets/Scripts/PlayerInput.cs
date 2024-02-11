using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public static int PlayerHelperID = -1;

    public CryptoInt3 Health = 100;

    public Team PlayerTeam;

    public CryptoFloat PlayerSpeed = 0.18f;

    public bool Dead = true;

    public bool NoDamage;

    public bool Move = true;

    public bool Look = true;

    public bool Zombie;

    public bool DamageSpeed;

    public CryptoInt DamageForce = 0;

    public bool MoveIce;

    public CryptoInt3 MaxHealth = 100;

    public bool Shift;

    public bool Climb;

    public bool Water;

    public bool MoveCrosshair;

    [Header("UFPS")]
    public vp_FPController FPController;

    public vp_FPCamera FPCamera;

    [Header("Player")]
    public Transform PlayerTransform;

    public Camera PlayerCamera;

    public Camera PlayerWeaponCamera;

    public PlayerWeapons PlayerWeapon;

    public ControllerManager Controller;

    public AudioClip[] PlayerFoosteps;

    [Disabled]
    public Vector2 MoveAxis;

    [Disabled]
    public Vector2 LookAxis;

    [Disabled]
    public float RotateCamera;

    [Header("Fall Damage")]
    public bool FallDamage;

    public CryptoFloat FallDamageThreshold = 10f;

    private bool FallingDamage;

    private float StartFallDamage;

    [Header("AFK")]
    public bool AfkEnabled;

    public float AfkDuration = 40f;

    private float AfkTimer = -1f;

    [Header("Bunny Hop")]
    public bool BunnyHopEnabled;

    public CryptoFloat BunnyHopSpeed = 0.4f;

    public CryptoFloat BunnyHopLerp = 5f;

    public CryptoFloat BunnyHopDefaultLerp = 0.5f;

    public CryptoFloat BunnyHopDefaultSpeed = 0.18f;

    private bool BunnyHopActive;

    private bool BunnyHopAutoJump;

    [Header("Surf")]
    public bool SurfEnabled;

    public CryptoFloat SurfAcceleration = 2f;

    public CryptoFloat SurfMaxSpeed = 120f;

    private CryptoFloat SurfSpeed;

    private bool Surf;

    private bool isStopSurf;

    [Header("Others")]
    public AudioSource m_AudioSource;

    public nTimer Timer;

    private bool isJump;

    private TimerData StartNoDamageInvokeData;

    private TimerData SettingsInvokeData;

    private byte GroundedDetect;

    public static PlayerInput instance;

    public bool Grounded
	{
		get
		{
			return !this.Water && !this.Surf && this.mCharacterController.isGrounded;
		}
	}

	public CharacterController mCharacterController
	{
		get
		{
			return this.FPController.mCharacterController;
		}
	}

	private void Start()
	{
		PlayerInput.instance = this;
		this.Controller = base.transform.root.GetComponent<ControllerManager>();
		this.SetHealth(this.Health);
		EventManager.AddListener("OnSettings", new EventManager.Callback(this.OnSettings));
		this.OnSettings();
		this.Timer.In((float)nValue.int2, true, new TimerDelegate(this.CheckController));
		this.Timer.In((float)nValue.int1, true, new TimerDelegate(this.CheckCamera));
		this.Timer.In((float)nValue.int2, true, new TimerDelegate(this.UpdateValue));    }

    #if UNITY_EDITOR
    public static void PrintPlayersIDS()
    {
        for (int i = 0; i < PhotonNetwork.otherPlayers.Length; i++)
        {
            Debug.Log(PhotonNetwork.otherPlayers[i].UserId + " - " + PhotonNetwork.otherPlayers[i].GetPlayerID());
        }
    }
    #endif

    [ContextMenu("Update Value")]
	private void UpdateValue()
	{
		this.Health.UpdateValue();
		this.PlayerSpeed.UpdateValue();
		this.DamageForce.UpdateValue();
		this.MaxHealth.UpdateValue();
		this.FallDamageThreshold.UpdateValue();
		this.BunnyHopSpeed.UpdateValue();
		this.BunnyHopLerp.UpdateValue();
		this.BunnyHopDefaultLerp.UpdateValue();
		this.BunnyHopDefaultSpeed.UpdateValue();
		this.SurfAcceleration.UpdateValue();
		this.SurfMaxSpeed.UpdateValue();
		this.SurfSpeed.UpdateValue();
	}

	private void OnEnable()
	{
		PlayerInput.PlayerHelperID = -1;
		LODObject.Target = this.PlayerCamera.transform;
		vp_FPCamera fpcamera = this.FPCamera;
		fpcamera.BobStepCallback = (vp_FPCamera.BobStepDelegate)Delegate.Combine(fpcamera.BobStepCallback, new vp_FPCamera.BobStepDelegate(this.PlayFoosteps));
		InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Combine(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(this.GetButtonDown));
		InputManager.GetButtonUpEvent = (InputManager.ButtonDelegate)Delegate.Combine(InputManager.GetButtonUpEvent, new InputManager.ButtonDelegate(this.GetButtonUp));
		InputManager.GetAxisEvent = (InputManager.AxisDelegate)Delegate.Combine(InputManager.GetAxisEvent, new InputManager.AxisDelegate(this.GetAxis));
		if (GameManager.startDamage)
		{
			this.StartNoDamage();
		}
		if (this.Climb)
		{
			this.SetClimb(false);
		}
		if (this.Water)
		{
			this.SetWater(false);
		}
		if (this.MoveIce)
		{
			this.SetMoveIce(false);
		}
		this.Dead = false;
		UIDeathScreen.ClearTakenDamage();
	}

	private void OnDisable()
	{
		vp_FPCamera fpcamera = this.FPCamera;
		fpcamera.BobStepCallback = (vp_FPCamera.BobStepDelegate)Delegate.Remove(fpcamera.BobStepCallback, new vp_FPCamera.BobStepDelegate(this.PlayFoosteps));
		InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Remove(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(this.GetButtonDown));
		InputManager.GetButtonUpEvent = (InputManager.ButtonDelegate)Delegate.Remove(InputManager.GetButtonUpEvent, new InputManager.ButtonDelegate(this.GetButtonUp));
		InputManager.GetAxisEvent = (InputManager.AxisDelegate)Delegate.Remove(InputManager.GetAxisEvent, new InputManager.AxisDelegate(this.GetAxis));
		this.MoveAxis = Vector2.zero;
		this.LookAxis = Vector2.zero;
		this.BunnyHopAutoJump = false;
		this.SurfSpeed = nValue.float0;
		this.Dead = true;
		this.isJump = false;
	}

	private void GetButtonDown(string name)
	{
        switch (name)
        {
            case "Jump":
                isJump = true;
                break;
        }
    }

	private void GetButtonUp(string name)
	{
        switch (name)
        {
            case "Jump":
                isJump = false;
                break;
        }
    }

	private void GetAxis(string name, float value)
	{
        switch (name)
        {
            case "Horizontal":
                MoveAxis.x = value;
                break;
            case "Vertical":
                MoveAxis.y = value;
                break;
            case "Mouse X":
                LookAxis.x = value;
                break;
            case "Mouse Y":
                LookAxis.y = value;
                break;
        }
    }

	private void Update()
	{
		this.UpdateMove();
		this.UpdateLook();
		this.UpdateJump();
		this.UpdateBunnyHop();
		this.UpdateAFK();
        #if UNITY_EDITOR || UNITY_STANDALONE_WIN
        this.UpdateCursor();
        #endif
    }

    private void FixedUpdate()
    {
        this.UpdateSurf();
        this.UpdateFallDamage();
        this.UpdateVelocity();
    }

    #if UNITY_EDITOR || UNITY_STANDALONE_WIN
    public bool cursor = true;

    private void UpdateCursor()
    {
        if(Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.P))
        {
            cursor = !cursor;
        }
        Cursor.visible = cursor;
        if(cursor)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    #endif

    private void UpdateMove()
	{
		if (this.Move)
		{
			this.MoveAxis = this.MoveAxis.normalized;
			if (this.PlayerWeapon.isScope && this.PlayerWeapon.GetSelectedWeaponData().Scope2)
			{
				this.MoveAxis = this.MoveAxis.normalized * nValue.float05;
			}
			else if (this.Shift && InputJoystick.shift)
			{
				this.MoveAxis = this.MoveAxis.normalized * nValue.float04;
			}
			else
			{
				this.MoveAxis = this.MoveAxis.normalized;
			}
            #if UNITY_EDITOR || UNITY_STANDALONE_WIN
            float g = 0;
            float h = 0;
            if (Input.GetKey(KeyCode.W))
            {
                g = 1;
            }
            if (Input.GetKey(KeyCode.S))
            {
                g = -1;
            }
            if (Input.GetKey(KeyCode.A))
            {
                h = -1;
            }
            if (Input.GetKey(KeyCode.D))
            {
                h = 1;
            }
            this.MoveAxis = new Vector2(h, g).normalized;
            #endif
            this.FPController.OnValue_InputMoveVector = this.MoveAxis;
			if (this.MoveCrosshair)
			{
				UICrosshair.SetMove(this.MoveAxis.sqrMagnitude);
			}
		}
	}

	private void UpdateLook()
	{
		if (this.Look)
		{
            if (this.PlayerWeapon.isScope)
			{
				this.LookAxis *= ((!this.PlayerWeapon.GetSelectedWeaponData().Scope) ? this.PlayerWeapon.GetSelectedWeaponData().Scope2Sensitivity : this.PlayerWeapon.GetSelectedWeaponData().ScopeSensitivity);
			}
			this.FPCamera.UpdateLook(this.LookAxis);
			this.RotateCamera = -this.FPCamera.Pitch / nValue.float60;
		}
	}

	private void UpdateJump()
	{
		if (this.BunnyHopAutoJump)
		{
			if (this.Grounded)
			{
				if (this.FPController.CanStartJump())
				{
					this.FPController.OnStartJump();
				}
			}
			else
			{
				this.FPController.OnStopJump();
			}
			return;
		}
		if (this.MoveCrosshair && !this.FPController.CanStartJump())
		{
			UICrosshair.SetMove(1f);
		}
		if (this.isJump && !this.Climb && !this.Water)
		{
			if (this.FPController.CanStartJump())
			{
				this.FPController.OnStartJump();
				if (this.MoveCrosshair)
				{
					UICrosshair.SetMove(10f);
				}
			}
		}
		else
		{
			this.FPController.OnStopJump();
		}
	}

	private void UpdateBunnyHop()
	{
		if (!this.BunnyHopEnabled)
		{
			return;
		}
		if (!this.Grounded && this.mCharacterController.velocity.sqrMagnitude > (float)nValue.int20)
		{
			if (!this.BunnyHopActive)
			{
				this.BunnyHopActive = true;
				DOTween.Kill("BunnyHop", false);
				DOTween.To(() => this.FPController.MotorAcceleration, delegate(float x)
				{
					this.FPController.MotorAcceleration = x;
				}, this.BunnyHopSpeed, this.BunnyHopLerp).SetId("BunnyHop");
			}
		}
		else if (this.BunnyHopActive)
		{
			this.BunnyHopActive = false;
			DOTween.Kill("BunnyHop", false);
			DOTween.To(() => this.FPController.MotorAcceleration, delegate(float x)
			{
				this.FPController.MotorAcceleration = x;
			}, this.BunnyHopDefaultSpeed, this.BunnyHopDefaultLerp).SetId("BunnyHop");
		}
	}

	private void UpdateSurf()
	{
		if (!this.SurfEnabled)
		{
			return;
		}
		if (this.isStopSurf)
		{
			this.Surf = false;
			this.SurfSpeed = nValue.float0;
			this.FPController.Stop();
			this.isStopSurf = false;
			return;
		}
		if (this.FPController.GroundAngle > (float)nValue.int30 && this.mCharacterController.isGrounded)
		{
			if (!this.Surf)
			{
				this.SurfSpeed += this.SurfAcceleration + this.mCharacterController.velocity.magnitude * this.SurfAcceleration;
			}
			else
			{
				this.SurfSpeed += this.SurfAcceleration;
			}
			this.Surf = true;
		}
		else if (this.FPController.GroundAngle < (float)nValue.int30 && this.mCharacterController.isGrounded)
		{
			this.Surf = false;
			this.SurfSpeed = nValue.float0;
		}
		else if (this.SurfSpeed > nValue.float0)
		{
			this.Surf = false;
			this.SurfSpeed -= this.SurfAcceleration / (float)nValue.int3;
		}
		this.SurfSpeed = Mathf.Clamp(this.SurfSpeed, nValue.float0, this.SurfMaxSpeed);
		if (this.SurfSpeed > nValue.float0)
		{
            this.FPController.AddForce(this.FPCamera.Forward * (this.SurfSpeed * nValue.float00001 + this.MoveAxis.y / (float)nValue.int100));
		}
	}

	private void UpdateFallDamage()
	{
		if (!this.FallDamage)
		{
			return;
		}
		if (this.Grounded)
		{
			if (this.FallingDamage)
			{
				this.FallingDamage = false;
				if (this.PlayerTransform.position.y < this.StartFallDamage - this.FallDamageThreshold)
				{
					int damage = (int)(this.StartFallDamage - this.PlayerTransform.position.y);
					DamageInfo damageInfo = DamageInfo.Get(damage, Vector3.zero, Team.None, nValue.int0, nValue.int0, -nValue.int1, false);
					this.Damage(damageInfo);
				}
			}
		}
		else if (!this.FallingDamage)
		{
			this.FallingDamage = true;
			this.StartFallDamage = this.PlayerTransform.position.y;
		}
	}

	private void UpdateVelocity()
	{
		if (this.mCharacterController.velocity.y < (float)(-(float)nValue.int100))
		{
			DamageInfo damageInfo = DamageInfo.Get(nValue.int1000, Vector3.zero, Team.None, nValue.int0, nValue.int0, -nValue.int1, false);
			this.Damage(damageInfo);
		}
	}

	public void SetBunnyHopAutoJump(bool active)
	{
		this.BunnyHopAutoJump = active;
	}

	public void Damage(DamageInfo damageInfo)
	{
        if (this.Dead || this.NoDamage)
		{
			return;
		}
		this.SetHealth(this.Health - damageInfo.damage);
		if (damageInfo.position != Vector3.zero)
		{
			UIDamage.Damage(damageInfo.position, this.FPCamera.Transform);
			if (this.DamageForce > nValue.int0)
			{
				float d = (float)nValue.int1 - Vector3.Distance(this.PlayerTransform.position, damageInfo.position) / (float)nValue.int100;
				Vector3 force = (damageInfo.position - this.PlayerTransform.position).normalized * d * (float)(-(float)this.DamageForce) / (float)nValue.int100;
				force.y = nValue.float0;
				this.FPController.AddForce(force);
			}
        }
        if (this.Health <= nValue.int0)
		{
            EventManager.Dispatch<DamageInfo>("DeadPlayer", damageInfo);
			this.PlayerWeapon.DeactiveScope();
		}
		else
		{
			PlayerInput.PlayerHelperID = damageInfo.player;
			if (this.DamageSpeed)
			{
				this.FPController.MotorAcceleration = nValue.float013;
				if (DOTween.IsTweening("DamageSpeed", false))
				{
					DOTween.Kill("DamageSpeed", false);
				}
				DOTween.To(() => this.FPController.MotorAcceleration, delegate(float x)
				{
					this.FPController.MotorAcceleration = x;
				}, nValue.float019, nValue.float15).SetId("DamageSpeed");
			}
			this.FPCamera.AddRollForce((float)UnityEngine.Random.Range(-nValue.int2, nValue.int2));
		}
	}

	private void PlayFoosteps()
	{
		if (this.Water || this.Climb || !this.Grounded)
		{
			return;
		}
		this.UpdateFoosteps();
	}

	public void UpdateFoosteps()
	{
		if (!Settings.Sound)
		{
			return;
		}
		AudioClip clip = this.PlayerFoosteps[UnityEngine.Random.Range(nValue.int0, this.PlayerFoosteps.Length)];
		this.m_AudioSource.pitch = UnityEngine.Random.Range(1f, 1.5f);
		this.m_AudioSource.clip = clip;
		this.m_AudioSource.Play();
	}

	public void StartNoDamage()
	{
		this.NoDamage = true;
		try
		{
			if (GameManager.startDamageTime > (float)(-(float)nValue.int0))
			{
				if (!this.Timer.Contains("StartNoDamage"))
				{
					this.Timer.Create("StartNoDamage", GameManager.startDamageTime, delegate()
					{
						this.NoDamage = false;
					});
				}
				this.Timer.In("StartNoDamage", GameManager.startDamageTime);
			}
		}
		catch
		{
			this.NoDamage = false;
		}
	}

	private void OnSettings()
	{
		if (!this.Timer.Contains("OnSettings"))
		{
			this.Timer.Create("OnSettings", nValue.float01, delegate()
			{
				float num = Settings.Sensitivity * 16f;
				this.FPCamera.MouseSensitivity = new Vector2(num, num);
				this.PlayerWeaponCamera.enabled = Settings.ShowWeapon;
				this.Shift = Settings.ShiftButton;
			});
		}
		this.Timer.In("OnSettings");
	}

	public void SetHealth(int health)
	{
		this.Health = health;
		this.Health = Mathf.Clamp(this.Health, nValue.int0, this.MaxHealth);
		UIHealth.SetHealth(this.Health);
		this.Controller.SetHealth((byte)health);
	}

	public void SetMove(bool move)
	{
		this.Move = move;
		if (!move)
		{
			this.MoveAxis = Vector2.zero;
			this.FPController.OnValue_InputMoveVector = this.MoveAxis;
		}
	}

	public void SetMove(bool move, float duration)
	{
		this.Move = move;
		TimerManager.Cancel("MoveTime");
		TimerManager.In("MoveTime", duration, delegate()
		{
			this.Move = !move;
		});
	}

	public void SetLook(bool look)
	{
		this.Look = look;
	}

	public void SetLook(bool look, float duration)
	{
		this.Look = look;
		TimerManager.Cancel("LookTime");
		TimerManager.In("LookTime", duration, delegate()
		{
			this.Look = !look;
		});
	}

	public void SetMoveIce(bool active)
	{
		this.MoveIce = active;
		if (active)
		{
			DOTween.To(() => this.FPController.MotorDamping, delegate(float x)
			{
				this.FPController.MotorDamping = x;
			}, nValue.float002, nValue.float02);
		}
		else
		{
			DOTween.To(() => this.FPController.MotorDamping, delegate(float x)
			{
				this.FPController.MotorDamping = x;
			}, nValue.float017, nValue.float02);
		}
	}

	public void UpdatePlayerSpeed(float speed)
	{
		this.PlayerSpeed = speed;
		this.FPController.MotorAcceleration = (float)this.PlayerSpeed;
	}

	public void SetPlayerSpeed(float mass)
	{
		this.FPController.MotorAcceleration = this.PlayerSpeed - mass;
	}

	public void SetClimb(bool active)
	{
		this.Climb = active;
		if (this.Climb)
		{
			this.FPController.Stop();
			this.FPController.PhysicsGravityModifier = (float)nValue.int0;
			this.FPController.MotorFreeFly = true;
		}
		else
		{
			this.FPController.PhysicsGravityModifier = nValue.float02;
			this.FPController.MotorFreeFly = false;
		}
	}

	public void SetWater(bool active)
	{
		this.SetWater(active, false);
	}

	public void SetWater(bool active, bool freeGravity)
	{
		this.Water = active;
		if (this.Water)
		{
			if (freeGravity)
			{
				this.FPController.Stop();
			}
			this.FPController.PhysicsGravityModifier = nValue.float0;
			this.FPController.MotorFreeFly = true;
		}
		else
		{
			this.FPController.PhysicsGravityModifier = nValue.float02;
			this.FPController.MotorFreeFly = false;
		}
	}

	public void StopSurf()
	{
		if (!this.Surf)
		{
			return;
		}
		this.isStopSurf = true;
	}

	private void CheckController()
	{
		if (this.Dead)
		{
			return;
		}
		if (this.mCharacterController.slopeLimit != nValue.float45)
		{
			CheckManager.Detected("Controller Error 1");
		}
		if (this.mCharacterController.stepOffset != nValue.float03)
		{
			CheckManager.Detected("Controller Error 2");
		}
		if (this.mCharacterController.center.x != (float)nValue.int0 && this.mCharacterController.center.y != nValue.float0745 && this.mCharacterController.center.z != (float)nValue.int0)
		{
			CheckManager.Detected("Controller Error 3");
		}
		if (this.mCharacterController.radius != nValue.float0375)
		{
			CheckManager.Detected("Controller Error 4");
		}
		if (this.mCharacterController.height != nValue.float149)
		{
			CheckManager.Detected("Controller Error 5");
		}
		if (this.mCharacterController.isGrounded)
		{
			if (Physics.CheckSphere(this.PlayerTransform.position, this.mCharacterController.radius, -1749041173))
			{
				if (this.GroundedDetect > 0)
				{
					this.GroundedDetect -= 1;
				}
			}
			else
			{
				this.GroundedDetect += (byte)nValue.int3;
			}
		}
		if ((int)this.GroundedDetect >= nValue.int9)
		{
			CheckManager.Detected("Controller Error 6");
		}
	}

	private void CheckCamera()
	{
		if (Mathf.Abs(this.FPCamera.Transform.localPosition.x) > nValue.float15 || Mathf.Abs(this.FPCamera.Transform.localPosition.y) > nValue.float15 || Mathf.Abs(this.FPCamera.Transform.localPosition.z) > nValue.float15)
		{
			this.GroundedDetect += 1;
			if ((int)this.GroundedDetect >= nValue.int3)
			{
				CheckManager.Detected("Camera Error");
			}
		}
		else if (this.GroundedDetect > 0)
		{
			this.GroundedDetect -= 1;
		}
		if (this.PlayerCamera.nearClipPlane != nValue.float001)
		{
			CheckManager.Detected("Camera Error");
		}
	}

	private void UpdateAFK()
	{
        #if UNITY_EDITOR || UNITY_STANDALONE_WIN
        return;
        #endif
        if (!this.AfkEnabled)
		{
			return;
		}
		if (Input.touchCount != 0)
		{
			if (Input.GetTouch(0).phase == TouchPhase.Began)
			{
				this.StopAFK();
			}
			if (Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled)
			{
				this.StartAFK();
			}
		}
		if (this.AfkTimer != -1f && GameManager.roundState == RoundState.PlayRound)
		{
			this.AfkTimer -= Time.deltaTime;
			if (this.AfkTimer < 0f)
			{
				this.AFKDetection();
			}
		}
	}

	public void StartAFK()
	{
		if (!this.AfkEnabled)
		{
			return;
		}
		this.AfkTimer = this.AfkDuration;
	}

	public void StopAFK()
	{
		this.AfkTimer = -1f;
	}

	private void AFKDetection()
	{
		GameManager.leaveRoomMessage = "AFK";
		PhotonNetwork.LeaveRoom(true);
	}
}
