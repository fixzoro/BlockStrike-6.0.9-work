using System;
using UnityEngine;

public class DataMap : MonoBehaviour
{
    public DataMap.ObjData[] dataList;

    public GameObject[] objList;

    public Material[] objMaterials;

    public byte[] objWeaponID;

    public Transform objParent;

    public GameObject weaponBox;

    public Rect[] zones;

    public float procent = 0.3f;

    public int x;

    public int y;

    public CullArea area;

    public Transform[] spawns;

    public byte selectWeaponZone = 1;

    public byte randomWeapon = 1;

    [SelectedWeapon]
    public int[] easyWeaponZone;

    [SelectedWeapon]
    public int[] mediumWeaponZone;

    [SelectedWeapon]
    public int[] hardWeaponZone;

    private byte[] playerArea;

    private byte[] lastPlayerArea = new byte[0];

    private int pGroup;

    private PhotonView pView;

    private void Start()
	{
		this.selectWeaponZone = (byte)UnityEngine.Random.Range(1, 4);
		this.randomWeapon = (byte)UnityEngine.Random.Range(0, 7);
		base.InvokeRepeating("Check", 2f, 1f);
	}

	private void Check()
	{
		if (PhotonNetwork.player.GetDead())
		{
			return;
		}
		if (this.pView == null)
		{
			this.pView = GameManager.controller.photonView;
			base.InvokeRepeating("UpdateActiveGroup", 0f, 1f / (float)PhotonNetwork.sendRateOnSerialize);
			GameManager.player.FPCamera.GetComponent<Camera>().farClipPlane = 120f;
			GameObject gameObject = new GameObject("Camera");
			gameObject.transform.SetParent(GameManager.player.FPCamera.Transform);
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localEulerAngles = Vector3.zero;
			Camera camera = gameObject.AddComponent<Camera>();
			camera.cullingMask = 131072;
			camera.depth = -0.2f;
			camera.clearFlags = CameraClearFlags.Depth;
		}
		this.playerArea = this.area.GetActiveCells(PlayerInput.instance.PlayerTransform.position).ToArray();
		if (object.Equals(this.playerArea, this.lastPlayerArea))
		{
			return;
		}
		for (int i = 0; i < this.dataList.Length; i++)
		{
			if (this.Contains(this.dataList[i].activeArea, this.playerArea))
			{
				if (!this.dataList[i].active)
				{
					switch (this.dataList[i].rot)
					{
					case 0:
						this.dataList[i].obj = PoolManager.Spawn(this.dataList[i].id.ToString(), this.objList[(int)this.dataList[i].id], this.dataList[i].pos, Vector3.zero, this.objParent);
						break;
					case 1:
						this.dataList[i].obj = PoolManager.Spawn(this.dataList[i].id.ToString(), this.objList[(int)this.dataList[i].id], this.dataList[i].pos, new Vector3(0f, 90f, 0f), this.objParent);
						break;
					case 2:
						this.dataList[i].obj = PoolManager.Spawn(this.dataList[i].id.ToString(), this.objList[(int)this.dataList[i].id], this.dataList[i].pos, new Vector3(0f, 180f, 0f), this.objParent);
						break;
					case 3:
						this.dataList[i].obj = PoolManager.Spawn(this.dataList[i].id.ToString(), this.objList[(int)this.dataList[i].id], this.dataList[i].pos, new Vector3(0f, 270f, 0f), this.objParent);
						break;
					}
					this.dataList[i].active = true;
					if (!this.dataList[i].weaponDrop && this.dataList[i].weaponZone == this.selectWeaponZone)
					{
						this.dataList[i].weaponBox = PoolManager.Spawn("weaponBox", this.weaponBox, this.dataList[i].pos + new Vector3(0f, 0.5f, 0f), Vector3.zero, this.objParent);
						DropWeapon component = this.dataList[i].weaponBox.GetComponent<DropWeapon>();
						switch (this.dataList[i].weaponZoneType)
						{
						case 1:
						{
							int num = (int)(this.randomWeapon + this.dataList[i].weaponID) - this.hardWeaponZone.Length;
							if (num < 0)
							{
								num = Mathf.Abs(num);
							}
							component.weaponID = this.hardWeaponZone[num];
							break;
						}
						case 2:
						{
							int num = (int)(this.randomWeapon + this.dataList[i].weaponID) - this.mediumWeaponZone.Length;
							if (num < 0)
							{
								num = Mathf.Abs(num);
							}
							component.weaponID = this.mediumWeaponZone[num];
							break;
						}
						case 3:
						{
							int num = (int)(this.randomWeapon + this.dataList[i].weaponID) - this.easyWeaponZone.Length;
							if (num < 0)
							{
								num = Mathf.Abs(num);
							}
							component.weaponID = this.easyWeaponZone[num];
							break;
						}
						}
					}
					Vector3 position = this.dataList[i].obj.transform.position;
					position.y = position.z;
					for (int j = 0; j < this.zones.Length; j++)
					{
						if (this.zones[j].Contains(position))
						{
							this.dataList[i].obj.GetComponent<Renderer>().material = this.objMaterials[j];
							break;
						}
					}
				}
			}
			else if (this.dataList[i].active)
			{
				PoolManager.Despawn(this.dataList[i].id.ToString(), this.dataList[i].obj);
				this.dataList[i].obj = null;
				this.dataList[i].active = false;
				if (this.dataList[i].weaponBox != null)
				{
					PoolManager.Despawn("weaponBox", this.dataList[i].weaponBox);
					this.dataList[i].weaponBox = null;
				}
			}
		}
		this.lastPlayerArea = this.playerArea;
		PhotonNetwork.SetInterestGroups(this.lastPlayerArea, this.playerArea);
	}

	public void DropWeapon(DropWeapon weapon)
	{
	}

	private bool Contains(byte[] a, byte b)
	{
		return this.Contains(a, new byte[]
		{
			b
		});
	}

	private bool Contains(byte[] a, byte[] b)
	{
		for (int i = 0; i < a.Length; i++)
		{
			for (int j = 0; j < b.Length; j++)
			{
				if (a[i] == b[j])
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool Equals(int[] a, int[] b)
	{
		if (a.Length != b.Length)
		{
			return false;
		}
		Array.Sort<int>(a);
		Array.Sort<int>(b);
		for (int i = 0; i < a.Length; i++)
		{
			if (a[i] != b[i])
			{
				return false;
			}
		}
		return true;
	}

	private void UpdateActiveGroup()
	{
		this.pGroup = ++this.pGroup % this.area.SUBDIVISION_SECOND_LEVEL_ORDER.Length;
		this.pView.group = this.playerArea[this.area.SUBDIVISION_SECOND_LEVEL_ORDER[this.pGroup]];
	}

	[Serializable]
	public class ObjData
	{
		public byte id;

		public byte[] activeArea;

		public Vector3 pos;

		public byte rot;

		public bool active;

		public byte weaponZone;

		public byte weaponZoneType;

		public byte weaponID;

		public bool weaponDrop;

		public GameObject obj;

		public GameObject weaponBox;
	}
}
