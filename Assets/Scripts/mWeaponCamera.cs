using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class mWeaponCamera : MonoBehaviour
{
    public Transform Point;

    public Transform PointAll;

    public float RotateSpeed = 200f;

    public Vector3 defaultPosition;

    public List<mWeaponCamera.WeaponData> Weapons = new List<mWeaponCamera.WeaponData>();

    private int SelectedWeapon = -1;

    private Camera mCamera;

    private Tweener mTween;

    private int SelectedWeaponID;

    private int SelectedWeaponSkin;

    private int PrevSticker = -1;

    private static mWeaponCamera instance;

    private void Awake()
	{
		mWeaponCamera.instance = this;
	}

	private void Start()
	{
		this.mCamera = base.GetComponent<Camera>();
		this.RotateSpeed = Mathf.Sqrt(this.RotateSpeed) / Mathf.Sqrt(Screen.dpi);
	}

	public static void SetCamera(bool down)
	{
		if (!down)
		{
			mWeaponCamera.instance.PointAll.transform.localPosition = Vector3.zero;
		}
		else
		{
			mWeaponCamera.instance.PointAll.transform.localPosition = new Vector3(0f, -0.4f, 0f);
		}
	}

	public static void Show(string weapon)
	{
		mWeaponCamera.SetFieldOfView(60f);
		mWeaponCamera.instance.mCamera.enabled = true;
		mWeaponCamera.ResetRotateX(true);
		if (mWeaponCamera.instance.SelectedWeapon != -1)
		{
			mWeaponCamera.instance.Weapons[mWeaponCamera.instance.SelectedWeapon].Target.SetActive(false);
			if (mWeaponCamera.instance.Weapons[mWeaponCamera.instance.SelectedWeapon].FireStat != null)
			{
				mWeaponCamera.instance.Weapons[mWeaponCamera.instance.SelectedWeapon].FireStat.SetActive(false);
			}
			mWeaponCamera.instance.SelectedWeapon = -1;
		}
		if (weapon == "BS Gold")
		{
			mWeaponCamera.instance.Weapons[44].Target.SetActive(true);
			mWeaponCamera.instance.SelectedWeapon = 44;
		}
		else if (weapon == "Sticker")
		{
			mWeaponCamera.instance.Weapons[45].Target.SetActive(true);
			mWeaponCamera.instance.SelectedWeapon = 45;
		}
		else
		{
			for (int i = 0; i < mWeaponCamera.instance.Weapons.Count; i++)
			{
				if (mWeaponCamera.instance.Weapons[i].Weapon == weapon)
				{
					mWeaponCamera.instance.Weapons[i].Target.SetActive(true);
					mWeaponCamera.instance.SelectedWeapon = i;
					break;
				}
			}
		}
	}

	public static void Close()
	{
		mWeaponCamera.instance.mCamera.enabled = false;
		if (mWeaponCamera.instance.SelectedWeapon != -1)
		{
			mWeaponCamera.instance.Weapons[mWeaponCamera.instance.SelectedWeapon].Target.SetActive(false);
			mWeaponCamera.instance.SelectedWeapon = -1;
		}
	}

	public static void SetViewportRect(Rect rect, float duration)
	{
		if (mWeaponCamera.instance.mTween != null && mWeaponCamera.instance.mTween.IsActive())
		{
			if (duration == 0f)
			{
				mWeaponCamera.instance.mTween.Kill(false);
				mWeaponCamera.instance.mCamera.rect = rect;
			}
			else
			{
				mWeaponCamera.instance.mTween = mWeaponCamera.instance.mTween.ChangeEndValue(rect, duration, false);
			}
		}
		else if (duration == 0f)
		{
			mWeaponCamera.instance.mCamera.rect = rect;
		}
		else
		{
			mWeaponCamera.instance.mTween = mWeaponCamera.instance.mCamera.DORect(rect, duration);
		}
	}

	public static void SetSkin(int weaponID, int skin)
	{
        if (mWeaponCamera.instance.SelectedWeapon == -1)
		{
			return;
		}
		if (mWeaponCamera.instance.SelectedWeapon == 45)
		{
            mWeaponCamera.instance.Weapons[mWeaponCamera.instance.SelectedWeapon].Target.GetComponent<MeshAtlas>().spriteName = skin.ToString();
			return;
		}
		mWeaponCamera.instance.SelectedWeaponID = weaponID;
		mWeaponCamera.instance.SelectedWeaponSkin = skin;
		MeshAtlas[] componentsInChildren = mWeaponCamera.instance.Weapons[mWeaponCamera.instance.SelectedWeapon].Target.GetComponentsInChildren<MeshAtlas>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].name != "FireStat" && componentsInChildren[i].name != "1" && componentsInChildren[i].name != "2" && componentsInChildren[i].name != "3" && componentsInChildren[i].name != "4" && componentsInChildren[i].name != "5" && componentsInChildren[i].name != "6")
			{
				componentsInChildren[i].spriteName = weaponID + "-" + skin;
			}
		}
		if (AccountManager.GetFireStatCounter(weaponID, skin) > -1 && mWeaponCamera.instance.Weapons[mWeaponCamera.instance.SelectedWeapon].FireStat != null)
		{
			mWeaponCamera.instance.Weapons[mWeaponCamera.instance.SelectedWeapon].FireStat.SetActive(true);
			string text = AccountManager.GetFireStatCounter(weaponID, skin).ToString("D6");
			for (int j = 0; j < text.Length; j++)
			{
                mWeaponCamera.instance.Weapons[mWeaponCamera.instance.SelectedWeapon].FireStatCounters[j].spriteName = "f" + text[j];
			}
		}
		else if (mWeaponCamera.instance.Weapons[mWeaponCamera.instance.SelectedWeapon].FireStat != null)
		{
			mWeaponCamera.instance.Weapons[mWeaponCamera.instance.SelectedWeapon].FireStat.SetActive(false);
		}
	}

	public static void Rotate(Vector2 rotate, bool onlyY)
	{
		if (onlyY)
		{
			mWeaponCamera.instance.Point.Rotate(new Vector2(0f, -rotate.x * mWeaponCamera.instance.RotateSpeed), Space.Self);
		}
		else
		{
			mWeaponCamera.instance.PointAll.Rotate(new Vector2(rotate.y * mWeaponCamera.instance.RotateSpeed, -rotate.x * mWeaponCamera.instance.RotateSpeed), Space.World);
		}
	}

	public static void SetRotation(Vector3 rotation)
	{
		mWeaponCamera.instance.Point.localEulerAngles = rotation;
	}

	public static void ResetRotateX(bool isTween)
	{
		if (isTween)
		{
			mWeaponCamera.instance.Point.DOLocalRotate(mWeaponCamera.instance.defaultPosition, 0.5f, RotateMode.Fast);
			mWeaponCamera.instance.PointAll.DOLocalRotate(mWeaponCamera.instance.defaultPosition, 0.5f, RotateMode.Fast);
		}
		else
		{
			mWeaponCamera.instance.Point.localEulerAngles = mWeaponCamera.instance.defaultPosition;
			mWeaponCamera.instance.PointAll.localEulerAngles = mWeaponCamera.instance.defaultPosition;
		}
	}

	public static void SetFieldOfView(float value)
	{
		mWeaponCamera.SetFieldOfView(-1f, value, -1f);
	}

	public static void SetFieldOfView(float from, float to, float duration)
	{
		if (from == -1f)
		{
			mWeaponCamera.instance.mCamera.fieldOfView = to;
		}
		else
		{
			mWeaponCamera.instance.mCamera.fieldOfView = from;
			mWeaponCamera.instance.mCamera.DOFieldOfView(to, duration);
		}
	}

	public static bool HasStickers()
	{
		return mWeaponCamera.instance.Weapons[mWeaponCamera.instance.SelectedWeapon].Stickers.Length != 0;
	}

	public static int GetStickersCount()
	{
		return mWeaponCamera.instance.Weapons[mWeaponCamera.instance.SelectedWeapon].Stickers.Length;
	}

	public static void SetStickers(AccountWeaponStickers stickers)
	{
		for (int i = 0; i < mWeaponCamera.instance.Weapons[mWeaponCamera.instance.SelectedWeapon].Stickers.Length; i++)
		{
			mWeaponCamera.instance.Weapons[mWeaponCamera.instance.SelectedWeapon].Stickers[i].gameObject.SetActive(false);
		}
		if (stickers != null)
		{
			for (int j = 0; j < stickers.StickerData.Count; j++)
			{
				for (int k = 0; k < mWeaponCamera.instance.Weapons[mWeaponCamera.instance.SelectedWeapon].Stickers.Length; k++)
				{
					if (mWeaponCamera.instance.Weapons[mWeaponCamera.instance.SelectedWeapon].Stickers[k].name == stickers.StickerData[j].Index.ToString())
					{
						mWeaponCamera.instance.Weapons[mWeaponCamera.instance.SelectedWeapon].Stickers[k].gameObject.SetActive(true);
						mWeaponCamera.instance.Weapons[mWeaponCamera.instance.SelectedWeapon].Stickers[k].spriteName = stickers.StickerData[j].StickerID.ToString();
						break;
					}
				}
			}
		}
	}

	public static void ActivePrevSticker(int pos, int id)
	{
		pos--;
		MeshAtlas atlas = mWeaponCamera.instance.Weapons[mWeaponCamera.instance.SelectedWeapon].Stickers[pos];
		if (mWeaponCamera.instance.PrevSticker != pos)
		{
			mWeaponCamera.DeactivePrevSticker();
			atlas.gameObject.SetActive(true);
			mWeaponCamera.instance.PrevSticker = pos;
			TimerManager.In("PrevSticker", 0.3f, -1, 0.3f, delegate()
			{
				atlas.cachedGameObject.SetActive(!atlas.cachedGameObject.activeSelf);
			});
		}
		atlas.spriteName = id.ToString();
	}

	public static void DeactivePrevSticker()
	{
		if (mWeaponCamera.instance.PrevSticker != -1)
		{
			TimerManager.Cancel("PrevSticker");
			if (AccountManager.HasWeaponSticker(mWeaponCamera.instance.SelectedWeaponID, mWeaponCamera.instance.SelectedWeaponSkin, mWeaponCamera.instance.PrevSticker + 1))
			{
				mWeaponCamera.instance.Weapons[mWeaponCamera.instance.SelectedWeapon].Stickers[mWeaponCamera.instance.PrevSticker].gameObject.SetActive(true);
				int weaponSticker = AccountManager.GetWeaponSticker(mWeaponCamera.instance.SelectedWeaponID, mWeaponCamera.instance.SelectedWeaponSkin, mWeaponCamera.instance.PrevSticker + 1);
				mWeaponCamera.instance.Weapons[mWeaponCamera.instance.SelectedWeapon].Stickers[mWeaponCamera.instance.PrevSticker].spriteName = weaponSticker.ToString();
			}
			else
			{
				mWeaponCamera.instance.Weapons[mWeaponCamera.instance.SelectedWeapon].Stickers[mWeaponCamera.instance.PrevSticker].gameObject.SetActive(false);
			}
			mWeaponCamera.instance.PrevSticker = -1;
		}
	}

	[Serializable]
	public class WeaponData
	{
		[SelectedWeapon]
		public string Weapon;

		public GameObject Target;

		public GameObject FireStat;

		public MeshAtlas[] FireStatCounters;

		public MeshAtlas[] Stickers;
	}
}
