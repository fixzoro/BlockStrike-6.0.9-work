using System;
using UnityEngine;

public class ChunkCube : MonoBehaviour
{
    public Transform cube;

    public float distance;

    public float minDistance;

    public Transform player;

    private RaycastHit hit;

    private Ray ray = default(Ray);

    private Vector3 pos;

    private bool isHit;

    private bool isActive;

    private void Start()
	{
		TimerManager.In(0.2f, delegate()
		{
			this.player = GameManager.player.FPCamera.Transform;
		});
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
		if (name == "Fire" && this.isHit && GameManager.player.PlayerWeapon.KnifeData.ID == 47)
		{
			ChunkManager.AddCube((int)this.pos.x, (int)this.pos.y, (int)this.pos.z, 1);
		}
	}

	private void Update()
	{
		if (!this.isActive && GameManager.player.PlayerWeapon.SelectedWeapon == WeaponType.Knife && GameManager.player.PlayerWeapon.KnifeData.ID == 47)
		{
			this.isActive = true;
		}
		else if (this.isActive && GameManager.player.PlayerWeapon.SelectedWeapon != WeaponType.Knife)
		{
			this.isActive = false;
			this.cube.position = Vector3.down * 10f;
		}
		if (this.isActive && Time.frameCount % 2 == 1)
		{
			this.ray.origin = this.player.position;
			this.ray.direction = this.player.forward;
			if (Physics.Raycast(this.ray, out this.hit, this.distance))
			{
				if (this.hit.distance >= this.minDistance)
				{
					if (this.hit.transform.CompareTag("ChunkBlock"))
					{
						this.pos = this.ray.GetPoint(this.hit.distance - 0.5f);
						this.pos = new Vector3(Mathf.Round(this.pos.x), Mathf.Round(this.pos.y), Mathf.Round(this.pos.z));
						this.cube.position = this.pos;
						this.isHit = true;
					}
				}
				else
				{
					this.cube.position = Vector3.down * 10f;
					this.isHit = false;
				}
			}
			else
			{
				this.cube.position = Vector3.down * 10f;
				this.isHit = false;
			}
		}
	}
}
