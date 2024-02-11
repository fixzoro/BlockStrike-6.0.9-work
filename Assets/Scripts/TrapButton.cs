using System;
using UnityEngine;

public class TrapButton : MonoBehaviour
{
    public Team PlayerTeam = Team.Red;

    [Range(1f, 30f)]
    public int Key;

    public KeyCode Keycode;

    public MeshRenderer ButtonRenderer;

    public Transform ButtonRedBlock;

    [Header("Weapon Settings")]
    public bool Weapon;

    [SelectedWeapon(WeaponType.Rifle)]
    public int SelectWeapon;

    private bool isTrigger;

    private bool isClickButton;

    private bool Active = true;

    private void Start()
	{
		EventManager.AddListener("StartRound", new EventManager.Callback(this.StartRound));
		EventManager.AddListener("WaitPlayer", new EventManager.Callback(this.StartRound));
		EventManager.AddListener<byte>("DeathRunClickButton", new EventManager.Callback<byte>(this.DeactiveButton));
	}

	private void OnEnable()
	{
		InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Combine(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(this.GetButtonDown));
	}

	private void OnDisable()
	{
		InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Remove(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(this.GetButtonDown));
	}

	private void GetButtonDown(string name)
	{
		if (name == "Fire" && GameManager.roundState == RoundState.PlayRound && this.isTrigger && !this.isClickButton && this.ButtonRenderer.isVisible)
		{
			this.ClickButton();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!this.isClickButton && other.CompareTag("Player") && PhotonNetwork.player.GetTeam() == this.PlayerTeam)
		{
			this.isTrigger = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (!this.isClickButton && other.CompareTag("Player") && PhotonNetwork.player.GetTeam() == this.PlayerTeam)
		{
			this.isTrigger = false;
		}
	}

	private void StartRound()
	{
		this.isClickButton = false;
		this.isTrigger = false;
		this.Active = false;
		TimerManager.In(nValue.float02, delegate()
		{
			this.Active = true;
			if (this.ButtonRedBlock != null)
			{
				this.ButtonRedBlock.localPosition = Vector3.zero;
			}
		});
	}

	private void ClickButton()
	{
		if (!this.Active)
		{
			return;
		}
		DeathRun.ClickButton((byte)this.Key);
		this.isClickButton = true;
		this.isTrigger = false;
		if (this.Weapon)
		{
			GameManager.player.PlayerWeapon.CanFire = false;
			TimerManager.In(nValue.float005, delegate()
			{
				WeaponManager.SetSelectWeapon(WeaponType.Rifle, this.SelectWeapon);
				GameManager.player.PlayerWeapon.UpdateWeaponAll(WeaponType.Rifle);
				GameManager.player.PlayerWeapon.CanFire = true;
			});
		}
	}

	public void DeactiveButton(byte button)
	{
		if ((int)button != this.Key)
		{
			return;
		}
		this.DeactiveButton();
		EventManager.Dispatch("Button" + this.Key);
	}

	public void DeactiveButton()
	{
		this.isClickButton = true;
		this.isTrigger = false;
		if (this.ButtonRedBlock != null)
		{
			this.ButtonRedBlock.localPosition = Vector3.down * nValue.float005;
		}
	}

	[ContextMenu("Click Button")]
	private void GetClickButton()
	{
		this.ClickButton();
	}
}
