using System;
using UnityEngine;

public class ZombieButton : MonoBehaviour
{
    public Team PlayerTeam = Team.Blue;

    [Range(1f, 20f)]
    public int Button = 1;

    public KeyCode Keycode;

    public MeshRenderer ButtonRenderer;

    public Transform ButtonRedBlock;

    private bool isTrigger;

    private bool isClickButton;

    private bool Active = true;

    private void Start()
	{
		EventManager.AddListener("StartRound", new EventManager.Callback(this.StartRound));
		EventManager.AddListener("WaitPlayer", new EventManager.Callback(this.StartRound));
		EventManager.AddListener<byte>("ZombieClickButton", new EventManager.Callback<byte>(this.DeactiveButton));
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
		if (name == "Fire" && this.isTrigger && !this.isClickButton && this.ButtonRenderer.isVisible)
		{
			this.ClickButton();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!this.isClickButton && other.CompareTag("Player"))
		{
			PlayerInput component = other.GetComponent<PlayerInput>();
			if (component != null && component.PlayerTeam == this.PlayerTeam)
			{
				this.isTrigger = true;
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (!this.isClickButton && other.CompareTag("Player"))
		{
			PlayerInput component = other.GetComponent<PlayerInput>();
			if (component != null && component.PlayerTeam == this.PlayerTeam)
			{
				this.isTrigger = false;
			}
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
				this.ButtonRedBlock.localPosition = new Vector3(-nValue.float01, nValue.float02, (float)nValue.int0);
			}
		});
	}

	private void ClickButton()
	{
		if (!this.Active)
		{
			return;
		}
		if (GameManager.roundState != RoundState.PlayRound)
		{
			return;
		}
		ZombieMode.ClickButton((byte)this.Button);
		this.isClickButton = true;
		this.isTrigger = false;
	}

	public void DeactiveButton(byte button)
	{
		if ((int)button != this.Button)
		{
			return;
		}
		this.isClickButton = true;
		this.isTrigger = false;
		EventManager.Dispatch("Button" + this.Button);
		if (this.ButtonRedBlock != null)
		{
			this.ButtonRedBlock.localPosition = new Vector3(-0.1f, 0.198f, -0.02f);
		}
	}

	[ContextMenu("Click Button")]
	private void GetClickButton()
	{
		this.ClickButton();
	}
}
