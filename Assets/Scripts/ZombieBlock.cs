using System;
using Photon;
using UnityEngine;

public class ZombieBlock : Photon.MonoBehaviour
{
    [Range(1f, 50f)]
    public int ID = 1;

    public int CountAttack = 50;

    [Range(1f, 20f)]
    public int Button = 1;

    public GameObject ActiveBlock;

    [HideInInspector]
    public int StartCountAttack = 50;

    private GameObject mGameObject;

    public GameObject cachedGameObject
	{
		get
		{
			if (this.mGameObject == null)
			{
				this.mGameObject = base.gameObject;
			}
			return this.mGameObject;
		}
	}

	public bool actived
	{
		get
		{
			return this.cachedGameObject.activeSelf;
		}
		set
		{
			this.cachedGameObject.SetActive(value);
			if (this.ActiveBlock != null)
			{
				this.ActiveBlock.SetActive(!value);
			}
		}
	}

	private void Start()
	{
		if (PhotonNetwork.room.GetGameMode() != GameMode.ZombieSurvival)
		{
			return;
		}
		this.StartCountAttack = this.CountAttack;
		EventManager.AddListener("Button" + this.Button, new EventManager.Callback(this.ButtonClick));
		EventManager.AddListener("StartRound", new EventManager.Callback(this.StartRound));
		EventManager.AddListener("WaitPlayer", new EventManager.Callback(this.StartRound));
	}

	private void StartRound()
	{
		this.actived = false;
	}

	private void ButtonClick()
	{
		this.actived = true;
		this.CountAttack = this.StartCountAttack;
	}

	public void Damage(DamageInfo info)
	{
		if (info.team != Team.Red)
		{
			return;
		}
		UICrosshair.Hit();
		ZombieMode.AddDamage((byte)this.ID);
	}

	public void Attack()
	{
		this.CountAttack -= nValue.int1;
		this.CountAttack = Mathf.Clamp(this.CountAttack, nValue.int0, this.StartCountAttack);
	}
}
