using System;
using UnityEngine;

public class DropWeapon : MonoBehaviour
{
    private static DropWeapon selectWeapon;

    public int ID;

    [SelectedWeapon]
    public int weaponID;

    public GameObject Weapon;

    public MeshAtlas[] WeaponAtlas;

    public MeshAtlas[] Stickers;

    public GameObject FireStat;

    public MeshAtlas[] FireStatCounters;

    private bool AutoDrop;

    public bool DestroyDrop;

    public float DestroyTime = -1f;

    public bool CustomData;

    public WeaponCustomData Data = new WeaponCustomData();

    private bool isEnterTrigger;

    private int TimerID;

    private int EventID;

    private BoxCollider boxCollider;

    private void OnEnable()
	{
		if (this.boxCollider == null)
		{
			this.boxCollider = base.GetComponent<BoxCollider>();
		}
		this.EventID = EventManager.AddListener<DamageInfo>("DeadPlayer", new EventManager.Callback<DamageInfo>(this.OnDeadPlayer));
		if (this.DestroyTime > 0f)
		{
			this.TimerID = TimerManager.In(this.DestroyTime, new TimerManager.Callback(this.DestroyWeapon));
		}
        this.gameObject.layer = 2;
	}

	private void OnDisable()
	{
		if (PhotonNetwork.leavingRoom)
		{
			return;
		}
		this.Deactive();
		EventManager.ClearEvent(this.EventID);
		TimerManager.Cancel(this.TimerID);
	}

	private void GetButtonDown(string name)
	{
		if (!this.isEnterTrigger)
		{
			return;
		}
		if (name != "Use")
		{
			return;
		}
		if (DropWeapon.selectWeapon != this)
		{
			return;
		}
		if (GameManager.player.PlayerWeapon.Wielded)
		{
			return;
		}
		if (GameManager.player.Dead)
		{
			return;
		}
		DropWeaponManager.PickupWeapon(this.ID);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (DropWeaponManager.lastDropTime + 0.2f > Time.time)
		{
			return;
		}
		if (!other.CompareTag("Player"))
		{
			return;
		}
		PlayerInput component = other.GetComponent<PlayerInput>();
		if (component == null)
		{
			return;
		}
		if (component.Dead)
		{
			return;
		}
		if (!this.boxCollider.bounds.Intersects(component.mCharacterController.bounds))
		{
			return;
		}
		if (this.AutoDrop && !component.PlayerWeapon.GetWeaponData(WeaponManager.GetWeaponData(this.weaponID).Type).Enabled)
		{
			DropWeaponManager.PickupWeapon(this.ID);
		}
		else if (!component.PlayerWeapon.GetWeaponData(WeaponManager.GetWeaponData(this.weaponID).Type).Enabled)
		{
			this.isEnterTrigger = true;
			InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Combine(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(this.GetButtonDown));
			UIControllerList.Use.cachedGameObject.SetActive(true);
			UIControllerList.UseText.text = Localization.Get("Pick up", true) + " " + WeaponManager.GetWeaponName(this.weaponID);
			DropWeapon.selectWeapon = this;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (!other.CompareTag("Player"))
		{
			return;
		}
		PlayerInput component = other.GetComponent<PlayerInput>();
		if (component == null)
		{
			return;
		}
		this.Deactive();
	}

	private void OnDeadPlayer(DamageInfo info)
	{
		if (!PhotonNetwork.leavingRoom)
		{
			this.Deactive();
		}
	}

	private void Deactive()
	{
		InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Remove(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(this.GetButtonDown));
		if (this.isEnterTrigger && DropWeapon.selectWeapon == this)
		{
			UIControllerList.Use.cachedGameObject.SetActive(false);
			UIControllerList.UseText.text = string.Empty;
		}
		this.isEnterTrigger = false;
	}

	public void PickupWeapon()
	{
		WeaponData weaponData = WeaponManager.GetWeaponData(this.weaponID);
		GameManager.player.PlayerWeapon.DropWeapon(weaponData.Type);
		GameManager.player.PlayerWeapon.UpdateWeapon(weaponData.ID, true, (!this.CustomData) ? null : this.Data);
		this.Deactive();
		if (this.DestroyDrop)
		{
			this.DestroyWeapon();
		}
	}

	public void DestroyWeapon()
	{
		PoolManager.Despawn(this.weaponID + "-Drop", this.Weapon);
	}

	public void UpdateWeapon()
	{
		if (this.CustomData)
		{
			string spriteName = this.weaponID + "-" + this.Data.Skin;
			for (int i = 0; i < this.WeaponAtlas.Length; i++)
			{
				this.WeaponAtlas[i].spriteName = spriteName;
			}
			if (this.Data.FireStatCounter >= 0)
			{
				this.FireStat.SetActive(true);
				string text = this.Data.FireStatCounter.ToString("D6");
				for (int j = 0; j < text.Length; j++)
				{
					this.FireStatCounters[j].spriteName = "f" + text[j];
				}
			}
			else
			{
				this.FireStat.SetActive(false);
			}
			for (int k = 0; k < this.Stickers.Length; k++)
			{
				this.Stickers[k].cachedGameObject.SetActive(false);
			}
			for (int l = 0; l < this.Data.Stickers.Length; l++)
			{
				if (this.Data.Stickers[l] != -1)
				{
					this.Stickers[l].cachedGameObject.SetActive(true);
					this.Stickers[l].spriteName = this.Data.Stickers[l].ToString();
				}
			}
		}
		else
		{
			string spriteName2 = this.weaponID + "-0";
			for (int m = 0; m < this.WeaponAtlas.Length; m++)
			{
				this.WeaponAtlas[m].spriteName = spriteName2;
			}
		}
	}
}
