using System;
using System.Collections.Generic;
using Photon;
using UnityEngine;

public class PlayerAI : Photon.MonoBehaviour, IPunObservable
{
    public static List<PlayerAI> list = new List<PlayerAI>();

    public Transform target;

    public int health = 1000;

    public bool offlineMode;

    public bool dead;

    public int attackDamage = 20;

    public float attackDistance = 5f;

    public float attackWalkDistance = 1.5f;

    public float attackIdleDistance = 3f;

    public float attackSpeed = 0.5f;

    public Vector3 position;

    public float checkPlayers = 1.5f;

    public UnityEngine.AI.NavMeshAgent nav;

    public PlayerAnimator playerAnimator;

    public GameObject attackEffect;

    public float photonSpeed = 11f;

    private Vector3 photonPosition;

    private Quaternion photonRotation;

    public Transform head;

    public float rotateAngle = 5f;

    public nTimer Timer;

    private Vector3 customVector3;

    private bool update1;

    private Transform cachedTransform;

    private bool isJump;

    public static event Action<PlayerAI> startEvent;

	public static event Action<PlayerAI> deadEvent;

	private void Start()
	{
		this.cachedTransform = base.transform;
		this.playerAnimator.grounded = true;
		this.playerAnimator.SetWeapon(WeaponType.Knife);
		this.nav.enabled = (base.photonView.isMine || this.offlineMode);
		this.Timer.In(this.attackSpeed + UnityEngine.Random.Range(-0.25f, 0.25f), true, new TimerDelegate(this.CheckDistance));
		this.Timer.In(0.1f, true, new TimerDelegate(this.UpdateNavigator));
		this.Timer.In(this.checkPlayers, true, new TimerDelegate(this.CheckPlayers));
	}

	private void OnEnable()
	{
		PlayerAI.list.Add(this);
		if (PlayerAI.startEvent != null)
		{
			PlayerAI.startEvent(this);
		}
		PhotonNetwork.onMasterClientSwitched = (PhotonNetwork.PhotonPlayerDelegate)Delegate.Combine(PhotonNetwork.onMasterClientSwitched, new PhotonNetwork.PhotonPlayerDelegate(this.OnMasterClientSwitched));
		PhotonNetwork.onOwnershipTransfered = (PhotonNetwork.ObjectsDelegate)Delegate.Combine(PhotonNetwork.onOwnershipTransfered, new PhotonNetwork.ObjectsDelegate(this.OnOwnershipTransfered));
	}

	private void OnDisable()
	{
		PlayerAI.list.Remove(this);
		PhotonNetwork.onMasterClientSwitched = (PhotonNetwork.PhotonPlayerDelegate)Delegate.Remove(PhotonNetwork.onMasterClientSwitched, new PhotonNetwork.PhotonPlayerDelegate(this.OnMasterClientSwitched));
		PhotonNetwork.onOwnershipTransfered = (PhotonNetwork.ObjectsDelegate)Delegate.Remove(PhotonNetwork.onOwnershipTransfered, new PhotonNetwork.ObjectsDelegate(this.OnOwnershipTransfered));
	}

	private void UpdateNavigator()
	{
		if (base.photonView.isMine || this.offlineMode)
		{
			if (this.target == null)
			{
				if (PlayerInput.instance != null)
				{
					this.target = PlayerInput.instance.PlayerTransform;
				}
			}
			else
			{
				if (this.nav.isOnOffMeshLink)
				{
					this.nav.SetDestination(this.target.position);
				}
				this.playerAnimator.move = Mathf.Clamp01(this.nav.velocity.sqrMagnitude);
				this.nav.updateRotation = ((this.target.position - this.cachedTransform.position).sqrMagnitude >= 10f);
			}
		}
	}

	private void Update()
	{
		if (base.photonView.isMine || this.offlineMode)
		{
			if (this.target != null)
			{
				if (!this.nav.updateRotation)
				{
					this.customVector3 = this.target.position - this.cachedTransform.position;
					this.customVector3.y = 0f;
					this.cachedTransform.rotation = Quaternion.LookRotation(this.customVector3);
				}
				this.customVector3 = this.target.position - this.cachedTransform.position;
				this.playerAnimator.rotate = this.customVector3.y / this.customVector3.sqrMagnitude * this.rotateAngle;
				if (this.nav.isOnOffMeshLink && !this.isJump)
				{
					this.playerAnimator.grounded = false;
					this.isJump = true;
				}
				else if (!this.nav.isOnOffMeshLink && this.isJump)
				{
					this.playerAnimator.grounded = true;
					this.isJump = false;
				}
			}
		}
		else
		{
			this.cachedTransform.position = Vector3.Lerp(this.cachedTransform.position, this.photonPosition, Time.deltaTime * this.photonSpeed);
			this.cachedTransform.rotation = Quaternion.Lerp(this.cachedTransform.rotation, this.photonRotation, Time.deltaTime * this.photonSpeed);
		}
	}

	private void CheckDistance()
	{
		if (!base.photonView.isMine)
		{
			return;
		}
		if (this.target == null)
		{
			return;
		}
		if (this.nav.velocity.sqrMagnitude <= 0.1f)
		{
			if (Vector3.Distance(this.target.position, this.cachedTransform.position) <= this.attackIdleDistance)
			{
				base.photonView.RPC("PhotonFire", PhotonTargets.All);
			}
		}
		else if (Vector3.Distance(this.target.position, this.cachedTransform.position) <= this.attackWalkDistance)
		{
			base.photonView.RPC("PhotonFire", PhotonTargets.All);
		}
	}

	[PunRPC]
	private void PhotonFire()
	{
		this.attackEffect.SetActive(true);
		TimerManager.In(0.05f, delegate()
		{
			this.attackEffect.SetActive(false);
		});
		RaycastHit raycastHit;
		if ((base.photonView.isMine || this.offlineMode) && Physics.Raycast(this.head.position + this.position, this.head.forward, out raycastHit, this.attackDistance) && raycastHit.transform == PlayerInput.instance.PlayerTransform)
		{
			DamageInfo damageInfo = DamageInfo.Get(this.attackDamage + UnityEngine.Random.Range(-5, 5), this.cachedTransform.position, Team.None, 0, 0, -1, false);
			PlayerInput.instance.Damage(damageInfo);
		}
	}

	public void Damage(DamageInfo damageInfo)
	{
		if (this.dead)
		{
			return;
		}
		UICrosshair.Hit();
	}

	[PunRPC]
	private void PhotonDamage(DamageInfo damageInfo)
	{
		if (base.photonView.isMine || this.offlineMode)
		{
			this.health -= damageInfo.damage;
			if (this.health <= 0)
			{
				this.dead = true;
			}
		}
	}

	[PunRPC]
	private void PhotonDead()
	{
		this.health = 0;
		this.dead = true;
		PoolManager.Despawn("Zombie", base.gameObject);
		if (PlayerAI.deadEvent != null)
		{
			PlayerAI.deadEvent(this);
		}
	}

	private void CheckPlayers()
	{
		if (this.offlineMode)
		{
			return;
		}
		if (!base.photonView.isMine)
		{
			return;
		}
		ControllerManager controllerManager = null;
		float num = Vector3.Distance(this.target.position, this.cachedTransform.position) + 1f;
		for (int i = 0; i < ControllerManager.ControllerList.Count; i++)
		{
			if (ControllerManager.ControllerList[i] != null && ControllerManager.ControllerList[i].playerSkin != null && !ControllerManager.ControllerList[i].playerSkin.Dead)
			{
				float num2 = Vector3.Distance(ControllerManager.ControllerList[i].playerSkin.PlayerTransform.position, this.cachedTransform.position);
				if (num2 + 4f < num)
				{
					num = num2;
					controllerManager = ControllerManager.ControllerList[i];
				}
			}
		}
		if (controllerManager == null)
		{
			return;
		}
		base.photonView.TransferOwnership(controllerManager.photonView.owner);
	}

	public void OnPhotonSerializeView(PhotonStream stream)
	{
        if (stream.isWriting)
        {
            this.photonPosition = this.cachedTransform.position;
            this.photonRotation = this.cachedTransform.rotation;
            stream.Write(this.cachedTransform.position);
            stream.Write(this.cachedTransform.rotation);
            stream.Write(this.nav.velocity.sqrMagnitude);
            stream.Write(this.health);
        }
        else
        {
            this.photonPosition = stream.ReadVector3();
            this.photonRotation = stream.ReadQuaternion();
            this.playerAnimator.move = stream.ReadFloat();
            this.health = stream.ReadInt();
        }
    }

	private void OnMasterClientSwitched(PhotonPlayer newMasterClient)
	{
		this.nav.enabled = (newMasterClient.ID == PhotonNetwork.player.ID);
	}

	private void OnOwnershipTransfered(object[] viewAndPlayers)
	{
		this.nav.enabled = (((PhotonPlayer)viewAndPlayers[1]).ID == PhotonNetwork.player.ID);
	}
}
