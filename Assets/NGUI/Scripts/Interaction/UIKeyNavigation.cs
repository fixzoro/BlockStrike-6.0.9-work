using System;
using UnityEngine;

// Token: 0x02000064 RID: 100
[AddComponentMenu("NGUI/Interaction/Key Navigation")]
public class UIKeyNavigation : MonoBehaviour
{
	// Token: 0x1700001C RID: 28
	// (get) Token: 0x060002C1 RID: 705 RVA: 0x00029130 File Offset: 0x00027330
	public static UIKeyNavigation current
	{
		get
		{
			GameObject hoveredObject = UICamera.hoveredObject;
			if (hoveredObject == null)
			{
				return null;
			}
			return hoveredObject.GetComponent<UIKeyNavigation>();
		}
	}

	// Token: 0x1700001D RID: 29
	// (get) Token: 0x060002C2 RID: 706 RVA: 0x00029158 File Offset: 0x00027358
	public bool isColliderEnabled
	{
		get
		{
			if (!base.enabled || !base.gameObject.activeInHierarchy)
			{
				return false;
			}
			Collider component = base.GetComponent<Collider>();
			if (component != null)
			{
				return component.enabled;
			}
			Collider2D component2 = base.GetComponent<Collider2D>();
			return component2 != null && component2.enabled;
		}
	}

	// Token: 0x060002C3 RID: 707 RVA: 0x00006D34 File Offset: 0x00004F34
	protected virtual void OnEnable()
	{
		UIKeyNavigation.list.Add(this);
		if (this.mStarted)
		{
			base.Invoke("Start", 0.001f);
		}
	}

	// Token: 0x060002C4 RID: 708 RVA: 0x00006D5C File Offset: 0x00004F5C
	private void Start()
	{
		this.mStarted = true;
		if (this.startsSelected && this.isColliderEnabled)
		{
			UICamera.selectedObject = base.gameObject;
		}
	}

	// Token: 0x060002C5 RID: 709 RVA: 0x00006D86 File Offset: 0x00004F86
	protected virtual void OnDisable()
	{
		UIKeyNavigation.list.Remove(this);
	}

	// Token: 0x060002C6 RID: 710 RVA: 0x000291B8 File Offset: 0x000273B8
	private static bool IsActive(GameObject go)
	{
		if (!go || !go.activeInHierarchy)
		{
			return false;
		}
		Collider component = go.GetComponent<Collider>();
		if (component != null)
		{
			return component.enabled;
		}
		Collider2D component2 = go.GetComponent<Collider2D>();
		return component2 != null && component2.enabled;
	}

	// Token: 0x060002C7 RID: 711 RVA: 0x00029214 File Offset: 0x00027414
	public GameObject GetLeft()
	{
		if (UIKeyNavigation.IsActive(this.onLeft))
		{
			return this.onLeft;
		}
		if (this.constraint == UIKeyNavigation.Constraint.Vertical || this.constraint == UIKeyNavigation.Constraint.Explicit)
		{
			return null;
		}
		return this.Get(Vector3.left, 1f, 2f);
	}

	// Token: 0x060002C8 RID: 712 RVA: 0x00029268 File Offset: 0x00027468
	public GameObject GetRight()
	{
		if (UIKeyNavigation.IsActive(this.onRight))
		{
			return this.onRight;
		}
		if (this.constraint == UIKeyNavigation.Constraint.Vertical || this.constraint == UIKeyNavigation.Constraint.Explicit)
		{
			return null;
		}
		return this.Get(Vector3.right, 1f, 2f);
	}

	// Token: 0x060002C9 RID: 713 RVA: 0x000292BC File Offset: 0x000274BC
	public GameObject GetUp()
	{
		if (UIKeyNavigation.IsActive(this.onUp))
		{
			return this.onUp;
		}
		if (this.constraint == UIKeyNavigation.Constraint.Horizontal || this.constraint == UIKeyNavigation.Constraint.Explicit)
		{
			return null;
		}
		return this.Get(Vector3.up, 2f, 1f);
	}

	// Token: 0x060002CA RID: 714 RVA: 0x00029310 File Offset: 0x00027510
	public GameObject GetDown()
	{
		if (UIKeyNavigation.IsActive(this.onDown))
		{
			return this.onDown;
		}
		if (this.constraint == UIKeyNavigation.Constraint.Horizontal || this.constraint == UIKeyNavigation.Constraint.Explicit)
		{
			return null;
		}
		return this.Get(Vector3.down, 2f, 1f);
	}

	// Token: 0x060002CB RID: 715 RVA: 0x00029364 File Offset: 0x00027564
	public GameObject Get(Vector3 myDir, float x = 1f, float y = 1f)
	{
		Transform transform = base.transform;
		myDir = transform.TransformDirection(myDir);
		Vector3 center = UIKeyNavigation.GetCenter(base.gameObject);
		float num = float.MaxValue;
		GameObject result = null;
		for (int i = 0; i < UIKeyNavigation.list.size; i++)
		{
			UIKeyNavigation uikeyNavigation = UIKeyNavigation.list[i];
			if (!(uikeyNavigation == this) && uikeyNavigation.constraint != UIKeyNavigation.Constraint.Explicit && uikeyNavigation.isColliderEnabled)
			{
				UIWidget component = uikeyNavigation.GetComponent<UIWidget>();
				if (!(component != null) || component.alpha != 0f)
				{
					Vector3 direction = UIKeyNavigation.GetCenter(uikeyNavigation.gameObject) - center;
					float num2 = Vector3.Dot(myDir, direction.normalized);
					if (num2 >= 0.707f)
					{
						direction = transform.InverseTransformDirection(direction);
						direction.x *= x;
						direction.y *= y;
						float sqrMagnitude = direction.sqrMagnitude;
						if (sqrMagnitude <= num)
						{
							result = uikeyNavigation.gameObject;
							num = sqrMagnitude;
						}
					}
				}
			}
		}
		return result;
	}

	// Token: 0x060002CC RID: 716 RVA: 0x00029498 File Offset: 0x00027698
	protected static Vector3 GetCenter(GameObject go)
	{
		UIWidget component = go.GetComponent<UIWidget>();
		UICamera uicamera = UICamera.FindCameraForLayer(go.layer);
		if (uicamera != null)
		{
			Vector3 vector = go.transform.position;
			if (component != null)
			{
				Vector3[] worldCorners = component.worldCorners;
				vector = (worldCorners[0] + worldCorners[2]) * 0.5f;
			}
			vector = uicamera.cachedCamera.WorldToScreenPoint(vector);
			vector.z = 0f;
			return vector;
		}
		if (component != null)
		{
			Vector3[] worldCorners2 = component.worldCorners;
			return (worldCorners2[0] + worldCorners2[2]) * 0.5f;
		}
		return go.transform.position;
	}

	// Token: 0x060002CD RID: 717 RVA: 0x00029570 File Offset: 0x00027770
	public virtual void OnNavigate(KeyCode key)
	{
		if (UIPopupList.isOpen)
		{
			return;
		}
		if (UIKeyNavigation.mLastFrame == Time.frameCount)
		{
			return;
		}
		UIKeyNavigation.mLastFrame = Time.frameCount;
		GameObject gameObject = null;
		switch (key)
		{
		case KeyCode.UpArrow:
			gameObject = this.GetUp();
			break;
		case KeyCode.DownArrow:
			gameObject = this.GetDown();
			break;
		case KeyCode.RightArrow:
			gameObject = this.GetRight();
			break;
		case KeyCode.LeftArrow:
			gameObject = this.GetLeft();
			break;
		}
		if (gameObject != null)
		{
			UICamera.hoveredObject = gameObject;
		}
	}

	// Token: 0x060002CE RID: 718 RVA: 0x0002960C File Offset: 0x0002780C
	public virtual void OnKey(KeyCode key)
	{
		if (UIPopupList.isOpen)
		{
			return;
		}
		if (UIKeyNavigation.mLastFrame == Time.frameCount)
		{
			return;
		}
		UIKeyNavigation.mLastFrame = Time.frameCount;
		if (key == KeyCode.Tab)
		{
			GameObject gameObject = this.onTab;
			if (gameObject == null)
			{
				if (UICamera.GetKey(KeyCode.LeftShift) || UICamera.GetKey(KeyCode.RightShift))
				{
					gameObject = this.GetLeft();
					if (gameObject == null)
					{
						gameObject = this.GetUp();
					}
					if (gameObject == null)
					{
						gameObject = this.GetDown();
					}
					if (gameObject == null)
					{
						gameObject = this.GetRight();
					}
				}
				else
				{
					gameObject = this.GetRight();
					if (gameObject == null)
					{
						gameObject = this.GetDown();
					}
					if (gameObject == null)
					{
						gameObject = this.GetUp();
					}
					if (gameObject == null)
					{
						gameObject = this.GetLeft();
					}
				}
			}
			if (gameObject != null)
			{
				UICamera.currentScheme = UICamera.ControlScheme.Controller;
				UICamera.hoveredObject = gameObject;
				UIInput component = gameObject.GetComponent<UIInput>();
				if (component != null)
				{
					component.isSelected = true;
				}
			}
		}
	}

	// Token: 0x060002CF RID: 719 RVA: 0x00006D94 File Offset: 0x00004F94
	protected virtual void OnClick()
	{
		if (NGUITools.GetActive(this.onClick))
		{
			UICamera.hoveredObject = this.onClick;
		}
	}

	// Token: 0x040001D7 RID: 471
	public static BetterList<UIKeyNavigation> list = new BetterList<UIKeyNavigation>();

	// Token: 0x040001D8 RID: 472
	public UIKeyNavigation.Constraint constraint;

	// Token: 0x040001D9 RID: 473
	public GameObject onUp;

	// Token: 0x040001DA RID: 474
	public GameObject onDown;

	// Token: 0x040001DB RID: 475
	public GameObject onLeft;

	// Token: 0x040001DC RID: 476
	public GameObject onRight;

	// Token: 0x040001DD RID: 477
	public GameObject onClick;

	// Token: 0x040001DE RID: 478
	public GameObject onTab;

	// Token: 0x040001DF RID: 479
	public bool startsSelected;

	// Token: 0x040001E0 RID: 480
	[NonSerialized]
	private bool mStarted;

	// Token: 0x040001E1 RID: 481
	public static int mLastFrame = 0;

	// Token: 0x02000065 RID: 101
	[DoNotObfuscateNGUI]
	public enum Constraint
	{
		// Token: 0x040001E3 RID: 483
		None,
		// Token: 0x040001E4 RID: 484
		Vertical,
		// Token: 0x040001E5 RID: 485
		Horizontal,
		// Token: 0x040001E6 RID: 486
		Explicit
	}
}
