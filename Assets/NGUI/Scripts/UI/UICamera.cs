using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000EE RID: 238
[AddComponentMenu("NGUI/UI/Event System (UICamera)")]
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class UICamera : MonoBehaviour
{
	// Token: 0x17000124 RID: 292
	// (get) Token: 0x0600080F RID: 2063 RVA: 0x00007F30 File Offset: 0x00006130
	[Obsolete("Use new OnDragStart / OnDragOver / OnDragOut / OnDragEnd events instead")]
	public bool stickyPress
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000125 RID: 293
	// (get) Token: 0x06000810 RID: 2064 RVA: 0x0000A31D File Offset: 0x0000851D
	// (set) Token: 0x06000811 RID: 2065 RVA: 0x0000A334 File Offset: 0x00008534
	public static bool disableController
	{
		get
		{
			return UICamera.mDisableController && !UIPopupList.isOpen;
		}
		set
		{
			UICamera.mDisableController = value;
		}
	}

	// Token: 0x17000126 RID: 294
	// (get) Token: 0x06000812 RID: 2066 RVA: 0x0000A33C File Offset: 0x0000853C
	// (set) Token: 0x06000813 RID: 2067 RVA: 0x0000A343 File Offset: 0x00008543
	[Obsolete("Use lastEventPosition instead. It handles controller input properly.")]
	public static Vector2 lastTouchPosition
	{
		get
		{
			return UICamera.mLastPos;
		}
		set
		{
			UICamera.mLastPos = value;
		}
	}

	// Token: 0x17000127 RID: 295
	// (get) Token: 0x06000814 RID: 2068 RVA: 0x0004BBA4 File Offset: 0x00049DA4
	// (set) Token: 0x06000815 RID: 2069 RVA: 0x0000A343 File Offset: 0x00008543
	public static Vector2 lastEventPosition
	{
		get
		{
			UICamera.ControlScheme currentScheme = UICamera.currentScheme;
			if (currentScheme == UICamera.ControlScheme.Controller)
			{
				GameObject hoveredObject = UICamera.hoveredObject;
				if (hoveredObject != null)
				{
					Bounds bounds = NGUIMath.CalculateAbsoluteWidgetBounds(hoveredObject.transform);
					Camera camera = NGUITools.FindCameraForLayer(hoveredObject.layer);
					return camera.WorldToScreenPoint(bounds.center);
				}
			}
			return UICamera.mLastPos;
		}
		set
		{
			UICamera.mLastPos = value;
		}
	}

	// Token: 0x17000128 RID: 296
	// (get) Token: 0x06000816 RID: 2070 RVA: 0x0000A34B File Offset: 0x0000854B
	public static UICamera first
	{
		get
		{
			if (UICamera.list == null || UICamera.list.size == 0)
			{
				return null;
			}
			return UICamera.list[0];
		}
	}

	// Token: 0x17000129 RID: 297
	// (get) Token: 0x06000817 RID: 2071 RVA: 0x0004BC00 File Offset: 0x00049E00
	// (set) Token: 0x06000818 RID: 2072 RVA: 0x0004BC94 File Offset: 0x00049E94
	public static UICamera.ControlScheme currentScheme
	{
		get
		{
			if (UICamera.mCurrentKey == KeyCode.None)
			{
				return UICamera.ControlScheme.Touch;
			}
			if (UICamera.mCurrentKey >= KeyCode.JoystickButton0)
			{
				return UICamera.ControlScheme.Controller;
			}
			if (!(UICamera.current != null))
			{
				return UICamera.ControlScheme.Mouse;
			}
			if (UICamera.mLastScheme == UICamera.ControlScheme.Controller && (UICamera.mCurrentKey == UICamera.current.submitKey0 || UICamera.mCurrentKey == UICamera.current.submitKey1))
			{
				return UICamera.ControlScheme.Controller;
			}
			if (UICamera.current.useMouse)
			{
				return UICamera.ControlScheme.Mouse;
			}
			if (UICamera.current.useTouch)
			{
				return UICamera.ControlScheme.Touch;
			}
			return UICamera.ControlScheme.Controller;
		}
		set
		{
			if (UICamera.mLastScheme != value)
			{
				if (value == UICamera.ControlScheme.Mouse)
				{
					UICamera.currentKey = KeyCode.Mouse0;
				}
				else if (value == UICamera.ControlScheme.Controller)
				{
					UICamera.currentKey = KeyCode.JoystickButton0;
				}
				else if (value == UICamera.ControlScheme.Touch)
				{
					UICamera.currentKey = KeyCode.None;
				}
				else
				{
					UICamera.currentKey = KeyCode.Alpha0;
				}
				UICamera.mLastScheme = value;
			}
		}
	}

	// Token: 0x1700012A RID: 298
	// (get) Token: 0x06000819 RID: 2073 RVA: 0x0000A373 File Offset: 0x00008573
	// (set) Token: 0x0600081A RID: 2074 RVA: 0x0004BCF8 File Offset: 0x00049EF8
	public static KeyCode currentKey
	{
		get
		{
			return UICamera.mCurrentKey;
		}
		set
		{
			if (UICamera.mCurrentKey != value)
			{
				UICamera.ControlScheme controlScheme = UICamera.mLastScheme;
				UICamera.mCurrentKey = value;
				UICamera.mLastScheme = UICamera.currentScheme;
				if (controlScheme != UICamera.mLastScheme)
				{
					UICamera.HideTooltip();
					if (UICamera.onSchemeChange != null)
					{
						UICamera.onSchemeChange();
					}
				}
			}
		}
	}

	// Token: 0x1700012B RID: 299
	// (get) Token: 0x0600081B RID: 2075 RVA: 0x0004BD4C File Offset: 0x00049F4C
	public static Ray currentRay
	{
		get
		{
			return (!(UICamera.currentCamera != null) || UICamera.currentTouch == null) ? default(Ray) : UICamera.currentCamera.ScreenPointToRay(UICamera.currentTouch.pos);
		}
	}

	// Token: 0x1700012C RID: 300
	// (get) Token: 0x0600081C RID: 2076 RVA: 0x0000A37A File Offset: 0x0000857A
	public static bool inputHasFocus
	{
		get
		{
			return UICamera.mInputFocus && UICamera.mSelected && UICamera.mSelected.activeInHierarchy;
		}
	}

	// Token: 0x1700012D RID: 301
	// (get) Token: 0x0600081D RID: 2077 RVA: 0x0000A3A7 File Offset: 0x000085A7
	// (set) Token: 0x0600081E RID: 2078 RVA: 0x0000A3AE File Offset: 0x000085AE
	[Obsolete("Use delegates instead such as UICamera.onClick, UICamera.onHover, etc.")]
	public static GameObject genericEventHandler
	{
		get
		{
			return UICamera.mGenericHandler;
		}
		set
		{
			UICamera.mGenericHandler = value;
		}
	}

	// Token: 0x1700012E RID: 302
	// (get) Token: 0x0600081F RID: 2079 RVA: 0x0000A3B6 File Offset: 0x000085B6
	public static UICamera.MouseOrTouch mouse0
	{
		get
		{
			return UICamera.mMouse[0];
		}
	}

	// Token: 0x1700012F RID: 303
	// (get) Token: 0x06000820 RID: 2080 RVA: 0x0000A3BF File Offset: 0x000085BF
	public static UICamera.MouseOrTouch mouse1
	{
		get
		{
			return UICamera.mMouse[1];
		}
	}

	// Token: 0x17000130 RID: 304
	// (get) Token: 0x06000821 RID: 2081 RVA: 0x0000A3C8 File Offset: 0x000085C8
	public static UICamera.MouseOrTouch mouse2
	{
		get
		{
			return UICamera.mMouse[2];
		}
	}

	// Token: 0x17000131 RID: 305
	// (get) Token: 0x06000822 RID: 2082 RVA: 0x0000A3D1 File Offset: 0x000085D1
	private bool handlesEvents
	{
		get
		{
			return UICamera.eventHandler == this;
		}
	}

	// Token: 0x17000132 RID: 306
	// (get) Token: 0x06000823 RID: 2083 RVA: 0x0000A3DE File Offset: 0x000085DE
	public Camera cachedCamera
	{
		get
		{
			if (this.mCam == null)
			{
				this.mCam = base.GetComponent<Camera>();
			}
			return this.mCam;
		}
	}

	// Token: 0x17000133 RID: 307
	// (get) Token: 0x06000824 RID: 2084 RVA: 0x0000A403 File Offset: 0x00008603
	// (set) Token: 0x06000825 RID: 2085 RVA: 0x0000A40A File Offset: 0x0000860A
	public static GameObject tooltipObject
	{
		get
		{
			return UICamera.mTooltip;
		}
		set
		{
			UICamera.ShowTooltip(value);
		}
	}

	// Token: 0x06000826 RID: 2086 RVA: 0x0000A413 File Offset: 0x00008613
	public static bool IsPartOfUI(GameObject go)
	{
		return !(go == null) && !(go == UICamera.fallThrough) && NGUITools.FindInParents<UIRoot>(go) != null;
	}

	// Token: 0x17000134 RID: 308
	// (get) Token: 0x06000827 RID: 2087 RVA: 0x0004BD9C File Offset: 0x00049F9C
	public static bool isOverUI
	{
		get
		{
			int frameCount = Time.frameCount;
			if (UICamera.mLastOverCheck != frameCount)
			{
				UICamera.mLastOverCheck = frameCount;
				if (UICamera.currentTouch != null)
				{
					if (UICamera.currentTouch.pressed != null)
					{
						UICamera.mLastOverResult = UICamera.IsPartOfUI(UICamera.currentTouch.pressed);
						return UICamera.mLastOverResult;
					}
					UICamera.mLastOverResult = UICamera.IsPartOfUI(UICamera.currentTouch.current);
					return UICamera.mLastOverResult;
				}
				else
				{
					int i = 0;
					int count = UICamera.activeTouches.Count;
					while (i < count)
					{
						UICamera.MouseOrTouch mouseOrTouch = UICamera.activeTouches[i];
						if (UICamera.IsPartOfUI(mouseOrTouch.pressed))
						{
							UICamera.mLastOverResult = true;
							return UICamera.mLastOverResult;
						}
						i++;
					}
					for (int j = 0; j < 3; j++)
					{
						UICamera.MouseOrTouch mouseOrTouch2 = UICamera.mMouse[j];
						if (UICamera.IsPartOfUI((!(mouseOrTouch2.pressed != null)) ? mouseOrTouch2.current : mouseOrTouch2.pressed))
						{
							UICamera.mLastOverResult = true;
							return UICamera.mLastOverResult;
						}
					}
					UICamera.mLastOverResult = UICamera.IsPartOfUI(UICamera.controller.pressed);
				}
			}
			return UICamera.mLastOverResult;
		}
	}

	// Token: 0x17000135 RID: 309
	// (get) Token: 0x06000828 RID: 2088 RVA: 0x0004BECC File Offset: 0x0004A0CC
	public static bool uiHasFocus
	{
		get
		{
			int frameCount = Time.frameCount;
			if (UICamera.mLastFocusCheck != frameCount)
			{
				UICamera.mLastFocusCheck = frameCount;
				if (UICamera.inputHasFocus)
				{
					UICamera.mLastFocusResult = true;
					return UICamera.mLastFocusResult;
				}
				if (UICamera.currentTouch != null)
				{
					UICamera.mLastFocusResult = UICamera.currentTouch.isOverUI;
					return UICamera.mLastFocusResult;
				}
				int i = 0;
				int count = UICamera.activeTouches.Count;
				while (i < count)
				{
					UICamera.MouseOrTouch mouseOrTouch = UICamera.activeTouches[i];
					if (UICamera.IsPartOfUI(mouseOrTouch.pressed))
					{
						UICamera.mLastFocusResult = true;
						return UICamera.mLastFocusResult;
					}
					i++;
				}
				for (int j = 0; j < 3; j++)
				{
					UICamera.MouseOrTouch mouseOrTouch2 = UICamera.mMouse[j];
					if (UICamera.IsPartOfUI(mouseOrTouch2.pressed) || UICamera.IsPartOfUI(mouseOrTouch2.current))
					{
						UICamera.mLastFocusResult = true;
						return UICamera.mLastFocusResult;
					}
				}
				UICamera.mLastFocusResult = UICamera.IsPartOfUI(UICamera.controller.pressed);
			}
			return UICamera.mLastFocusResult;
		}
	}

	// Token: 0x17000136 RID: 310
	// (get) Token: 0x06000829 RID: 2089 RVA: 0x0004BFD4 File Offset: 0x0004A1D4
	public static bool interactingWithUI
	{
		get
		{
			int frameCount = Time.frameCount;
			if (UICamera.mLastInteractionCheck != frameCount)
			{
				UICamera.mLastInteractionCheck = frameCount;
				if (UICamera.inputHasFocus)
				{
					UICamera.mLastInteractionResult = true;
					return UICamera.mLastInteractionResult;
				}
				int i = 0;
				int count = UICamera.activeTouches.Count;
				while (i < count)
				{
					UICamera.MouseOrTouch mouseOrTouch = UICamera.activeTouches[i];
					if (UICamera.IsPartOfUI(mouseOrTouch.pressed))
					{
						UICamera.mLastInteractionResult = true;
						return UICamera.mLastInteractionResult;
					}
					i++;
				}
				for (int j = 0; j < 3; j++)
				{
					UICamera.MouseOrTouch mouseOrTouch2 = UICamera.mMouse[j];
					if (UICamera.IsPartOfUI(mouseOrTouch2.pressed))
					{
						UICamera.mLastInteractionResult = true;
						return UICamera.mLastInteractionResult;
					}
				}
				UICamera.mLastInteractionResult = UICamera.IsPartOfUI(UICamera.controller.pressed);
			}
			return UICamera.mLastInteractionResult;
		}
	}

	// Token: 0x17000137 RID: 311
	// (get) Token: 0x0600082A RID: 2090 RVA: 0x0004C0AC File Offset: 0x0004A2AC
	// (set) Token: 0x0600082B RID: 2091 RVA: 0x0004C114 File Offset: 0x0004A314
	public static GameObject hoveredObject
	{
		get
		{
			if (UICamera.currentTouch != null && (UICamera.currentScheme != UICamera.ControlScheme.Mouse || UICamera.currentTouch.dragStarted))
			{
				return UICamera.currentTouch.current;
			}
			if (UICamera.mHover && UICamera.mHover.activeInHierarchy)
			{
				return UICamera.mHover;
			}
			UICamera.mHover = null;
			return null;
		}
		set
		{
			if (UICamera.mHover == value)
			{
				return;
			}
			bool flag = false;
			UICamera uicamera = UICamera.current;
			if (UICamera.currentTouch == null)
			{
				flag = true;
				UICamera.currentTouchID = -100;
				UICamera.currentTouch = UICamera.controller;
			}
			UICamera.ShowTooltip(null);
			if (UICamera.mSelected && UICamera.currentScheme == UICamera.ControlScheme.Controller)
			{
				UICamera.Notify(UICamera.mSelected, "OnSelect", false);
				if (UICamera.onSelect != null)
				{
					UICamera.onSelect(UICamera.mSelected, false);
				}
				UICamera.mSelected = null;
			}
			if (UICamera.mHover)
			{
				UICamera.Notify(UICamera.mHover, "OnHover", false);
				if (UICamera.onHover != null)
				{
					UICamera.onHover(UICamera.mHover, false);
				}
			}
			UICamera.mHover = value;
			UICamera.currentTouch.clickNotification = UICamera.ClickNotification.None;
			if (UICamera.mHover)
			{
				if (flag)
				{
					UICamera uicamera2 = (!(UICamera.mHover != null)) ? UICamera.list[0] : UICamera.FindCameraForLayer(UICamera.mHover.layer);
					if (uicamera2 != null)
					{
						UICamera.current = uicamera2;
						UICamera.currentCamera = uicamera2.cachedCamera;
					}
				}
				if (UICamera.onHover != null)
				{
					UICamera.onHover(UICamera.mHover, true);
				}
				UICamera.Notify(UICamera.mHover, "OnHover", true);
			}
			if (flag)
			{
				UICamera.current = uicamera;
				UICamera.currentCamera = ((!(uicamera != null)) ? null : uicamera.cachedCamera);
				UICamera.currentTouch = null;
				UICamera.currentTouchID = -100;
			}
		}
	}

	// Token: 0x17000138 RID: 312
	// (get) Token: 0x0600082C RID: 2092 RVA: 0x0004C2C4 File Offset: 0x0004A4C4
	// (set) Token: 0x0600082D RID: 2093 RVA: 0x0004C42C File Offset: 0x0004A62C
	public static GameObject controllerNavigationObject
	{
		get
		{
			if (UICamera.controller.current && UICamera.controller.current.activeInHierarchy)
			{
				return UICamera.controller.current;
			}
			if (UICamera.currentScheme == UICamera.ControlScheme.Controller && UICamera.current != null && UICamera.current.useController && !UICamera.ignoreControllerInput && UIKeyNavigation.list.size > 0)
			{
				for (int i = 0; i < UIKeyNavigation.list.size; i++)
				{
					UIKeyNavigation uikeyNavigation = UIKeyNavigation.list[i];
					if (uikeyNavigation && uikeyNavigation.constraint != UIKeyNavigation.Constraint.Explicit && uikeyNavigation.startsSelected)
					{
						UICamera.hoveredObject = uikeyNavigation.gameObject;
						UICamera.controller.current = UICamera.mHover;
						return UICamera.mHover;
					}
				}
				if (UICamera.mHover == null)
				{
					for (int j = 0; j < UIKeyNavigation.list.size; j++)
					{
						UIKeyNavigation uikeyNavigation2 = UIKeyNavigation.list[j];
						if (uikeyNavigation2 && uikeyNavigation2.constraint != UIKeyNavigation.Constraint.Explicit)
						{
							UICamera.hoveredObject = uikeyNavigation2.gameObject;
							UICamera.controller.current = UICamera.mHover;
							return UICamera.mHover;
						}
					}
				}
			}
			UICamera.controller.current = null;
			return null;
		}
		set
		{
			if (UICamera.controller.current != value && UICamera.controller.current)
			{
				UICamera.Notify(UICamera.controller.current, "OnHover", false);
				if (UICamera.onHover != null)
				{
					UICamera.onHover(UICamera.controller.current, false);
				}
				UICamera.controller.current = null;
			}
			UICamera.hoveredObject = value;
		}
	}

	// Token: 0x17000139 RID: 313
	// (get) Token: 0x0600082E RID: 2094 RVA: 0x0000A43F File Offset: 0x0000863F
	// (set) Token: 0x0600082F RID: 2095 RVA: 0x0004C4AC File Offset: 0x0004A6AC
	public static GameObject selectedObject
	{
		get
		{
			if (UICamera.mSelected && UICamera.mSelected.activeInHierarchy)
			{
				return UICamera.mSelected;
			}
			UICamera.mSelected = null;
			return null;
		}
		set
		{
			if (UICamera.mSelected == value)
			{
				UICamera.hoveredObject = value;
				UICamera.controller.current = value;
				return;
			}
			UICamera.ShowTooltip(null);
			bool flag = false;
			UICamera uicamera = UICamera.current;
			if (UICamera.currentTouch == null)
			{
				flag = true;
				UICamera.currentTouchID = -100;
				UICamera.currentTouch = UICamera.controller;
			}
			UICamera.mInputFocus = false;
			if (UICamera.mSelected)
			{
				UICamera.Notify(UICamera.mSelected, "OnSelect", false);
				if (UICamera.onSelect != null)
				{
					UICamera.onSelect(UICamera.mSelected, false);
				}
			}
			UICamera.mSelected = value;
			UICamera.currentTouch.clickNotification = UICamera.ClickNotification.None;
			if (UICamera.mSelected && flag)
			{
				UICamera uicamera2 = (!(UICamera.mSelected != null)) ? UICamera.list[0] : UICamera.FindCameraForLayer(UICamera.mSelected.layer);
				if (uicamera2 != null)
				{
					UICamera.current = uicamera2;
					UICamera.currentCamera = uicamera2.cachedCamera;
				}
			}
			if (UICamera.mSelected)
			{
				if (UICamera.clickUIInput)
				{
					UICamera.mInputFocus = (UICamera.mSelected.activeInHierarchy && UICamera.mSelected.GetComponent<UIInput>() != null);
				}
				if (UICamera.onSelect != null)
				{
					UICamera.onSelect(UICamera.mSelected, true);
				}
				UICamera.Notify(UICamera.mSelected, "OnSelect", true);
			}
			if (flag)
			{
				UICamera.current = uicamera;
				UICamera.currentCamera = ((!(uicamera != null)) ? null : uicamera.cachedCamera);
				UICamera.currentTouch = null;
				UICamera.currentTouchID = -100;
			}
		}
	}

	// Token: 0x06000830 RID: 2096 RVA: 0x0004C664 File Offset: 0x0004A864
	public static bool IsPressed(GameObject go)
	{
		for (int i = 0; i < 3; i++)
		{
			if (UICamera.mMouse[i].pressed == go)
			{
				return true;
			}
		}
		int j = 0;
		int count = UICamera.activeTouches.Count;
		while (j < count)
		{
			UICamera.MouseOrTouch mouseOrTouch = UICamera.activeTouches[j];
			if (mouseOrTouch.pressed == go)
			{
				return true;
			}
			j++;
		}
		return UICamera.controller.pressed == go;
	}

	// Token: 0x1700013A RID: 314
	// (get) Token: 0x06000831 RID: 2097 RVA: 0x0000A46C File Offset: 0x0000866C
	[Obsolete("Use either 'CountInputSources()' or 'activeTouches.Count'")]
	public static int touchCount
	{
		get
		{
			return UICamera.CountInputSources();
		}
	}

	// Token: 0x06000832 RID: 2098 RVA: 0x0004C6F0 File Offset: 0x0004A8F0
	public static int CountInputSources()
	{
		int num = 0;
		int i = 0;
		int count = UICamera.activeTouches.Count;
		while (i < count)
		{
			UICamera.MouseOrTouch mouseOrTouch = UICamera.activeTouches[i];
			if (mouseOrTouch.pressed != null)
			{
				num++;
			}
			i++;
		}
		for (int j = 0; j < UICamera.mMouse.Length; j++)
		{
			if (UICamera.mMouse[j].pressed != null)
			{
				num++;
			}
		}
		if (UICamera.controller.pressed != null)
		{
			num++;
		}
		return num;
	}

	// Token: 0x1700013B RID: 315
	// (get) Token: 0x06000833 RID: 2099 RVA: 0x0004C790 File Offset: 0x0004A990
	public static int dragCount
	{
		get
		{
			int num = 0;
			int i = 0;
			int count = UICamera.activeTouches.Count;
			while (i < count)
			{
				UICamera.MouseOrTouch mouseOrTouch = UICamera.activeTouches[i];
				if (mouseOrTouch.dragged != null)
				{
					num++;
				}
				i++;
			}
			for (int j = 0; j < UICamera.mMouse.Length; j++)
			{
				if (UICamera.mMouse[j].dragged != null)
				{
					num++;
				}
			}
			if (UICamera.controller.dragged != null)
			{
				num++;
			}
			return num;
		}
	}

	// Token: 0x1700013C RID: 316
	// (get) Token: 0x06000834 RID: 2100 RVA: 0x0004C830 File Offset: 0x0004AA30
	public static Camera mainCamera
	{
		get
		{
			UICamera eventHandler = UICamera.eventHandler;
			return (!(eventHandler != null)) ? null : eventHandler.cachedCamera;
		}
	}

	// Token: 0x1700013D RID: 317
	// (get) Token: 0x06000835 RID: 2101 RVA: 0x0004C85C File Offset: 0x0004AA5C
	public static UICamera eventHandler
	{
		get
		{
			for (int i = 0; i < UICamera.list.size; i++)
			{
				UICamera uicamera = UICamera.list.buffer[i];
				if (!(uicamera == null) && uicamera.enabled && NGUITools.GetActive(uicamera.gameObject))
				{
					return uicamera;
				}
			}
			return null;
		}
	}

	// Token: 0x06000836 RID: 2102 RVA: 0x0000A473 File Offset: 0x00008673
	private static int CompareFunc(UICamera a, UICamera b)
	{
		if (a.cachedCamera.depth < b.cachedCamera.depth)
		{
			return 1;
		}
		if (a.cachedCamera.depth > b.cachedCamera.depth)
		{
			return -1;
		}
		return 0;
	}

	// Token: 0x06000837 RID: 2103 RVA: 0x0004C8C0 File Offset: 0x0004AAC0
	private static Rigidbody FindRootRigidbody(Transform trans)
	{
		while (trans != null)
		{
			if (trans.GetComponent<UIPanel>() != null)
			{
				break;
			}
			Rigidbody rigidbody = trans.GetComponent<Rigidbody>();
			if (rigidbody != null)
			{
				return rigidbody;
			}
			trans = trans.parent;
		}
		return null;
	}

	// Token: 0x06000838 RID: 2104 RVA: 0x0004C914 File Offset: 0x0004AB14
	private static Rigidbody2D FindRootRigidbody2D(Transform trans)
	{
		while (trans != null)
		{
			if (trans.GetComponent<UIPanel>() != null)
			{
				break;
			}
			Rigidbody2D rigidbody2D = trans.GetComponent<Rigidbody2D>();
			if (rigidbody2D != null)
			{
				return rigidbody2D;
			}
			trans = trans.parent;
		}
		return null;
	}

	// Token: 0x06000839 RID: 2105 RVA: 0x0004C968 File Offset: 0x0004AB68
	public static void Raycast(UICamera.MouseOrTouch touch)
	{
		nProfiler.BeginSample("UICamera.Raycast (MouseOrTouch)");
		if (!UICamera.Raycast(touch.pos))
		{
			UICamera.mRayHitObject = UICamera.fallThrough;
		}
		if (UICamera.mRayHitObject == null)
		{
			UICamera.mRayHitObject = UICamera.mGenericHandler;
		}
		touch.last = touch.current;
		touch.current = UICamera.mRayHitObject;
		UICamera.mLastPos = touch.pos;
		nProfiler.EndSample();
	}

	// Token: 0x0600083A RID: 2106 RVA: 0x0004C9E0 File Offset: 0x0004ABE0
	public static bool Raycast(Vector3 inPos)
	{
		nProfiler.BeginSample("UICamera.Raycast (Vector3)");
		for (int i = 0; i < UICamera.list.size; i++)
		{
			UICamera uicamera = UICamera.list.buffer[i];
			if (uicamera.enabled && NGUITools.GetActive(uicamera.gameObject))
			{
				UICamera.currentCamera = uicamera.cachedCamera;
				Vector3 vector = UICamera.currentCamera.ScreenToViewportPoint(inPos);
				if (!float.IsNaN(vector.x) && !float.IsNaN(vector.y))
				{
					if (vector.x >= 0f && vector.x <= 1f && vector.y >= 0f && vector.y <= 1f)
					{
						Ray ray = UICamera.currentCamera.ScreenPointToRay(inPos);
						int layerMask = UICamera.currentCamera.cullingMask & uicamera.eventReceiverMask;
						float distance = (uicamera.rangeDistance <= 0f) ? (UICamera.currentCamera.farClipPlane - UICamera.currentCamera.nearClipPlane) : uicamera.rangeDistance;
						if (uicamera.eventType == UICamera.EventType.World_3D)
						{
							UICamera.lastWorldRay = ray;
							if (Physics.Raycast(ray, out UICamera.lastHit, distance, layerMask))
							{
								UICamera.lastWorldPosition = UICamera.lastHit.point;
								UICamera.mRayHitObject = UICamera.lastHit.collider.gameObject;
								if (!uicamera.eventsGoToColliders)
								{
									Rigidbody componentInParent = UICamera.mRayHitObject.gameObject.GetComponentInParent<Rigidbody>();
									if (componentInParent != null)
									{
										UICamera.mRayHitObject = componentInParent.gameObject;
									}
								}
								nProfiler.EndSample();
								return true;
							}
						}
						else if (uicamera.eventType == UICamera.EventType.UI_3D)
						{
							RaycastHit[] array = Physics.RaycastAll(ray, distance, layerMask);
							int num = array.Length;
							if (num > 1)
							{
								int j = 0;
								while (j < num)
								{
									GameObject gameObject = array[j].collider.gameObject;
									UIWidget component = gameObject.GetComponent<UIWidget>();
									if (component != null)
									{
										if (component.isVisible)
										{
											if (component.hitCheck == null || component.hitCheck(array[j].point))
											{
												goto IL_26F;
											}
										}
									}
									else
									{
										UIRect uirect = NGUITools.FindInParents<UIRect>(gameObject);
										if (!(uirect != null) || uirect.finalAlpha >= 0.001f)
										{
											goto IL_26F;
										}
									}
									IL_2F0:
									j++;
									continue;
									IL_26F:
									UICamera.mHit.depth = NGUITools.CalculateRaycastDepth(gameObject);
									if (UICamera.mHit.depth != 2147483647)
									{
										UICamera.mHit.hit = array[j];
										UICamera.mHit.point = array[j].point;
										UICamera.mHit.go = array[j].collider.gameObject;
										UICamera.mHits.Add(UICamera.mHit);
										goto IL_2F0;
									}
									goto IL_2F0;
								}
								UICamera.mHits.Sort((UICamera.DepthEntry r1, UICamera.DepthEntry r2) => r2.depth.CompareTo(r1.depth));
								for (int k = 0; k < UICamera.mHits.size; k++)
								{
									if (UICamera.IsVisible(ref UICamera.mHits.buffer[k]))
									{
										UICamera.lastHit = UICamera.mHits[k].hit;
										UICamera.mRayHitObject = UICamera.mHits[k].go;
										UICamera.lastWorldRay = ray;
										UICamera.lastWorldPosition = UICamera.mHits[k].point;
										UICamera.mHits.Clear();
										nProfiler.EndSample();
										return true;
									}
								}
								UICamera.mHits.Clear();
							}
							else if (num == 1)
							{
								GameObject gameObject2 = array[0].collider.gameObject;
								UIWidget component2 = gameObject2.GetComponent<UIWidget>();
								if (component2 != null)
								{
									if (!component2.isVisible)
									{
										goto IL_812;
									}
									if (component2.hitCheck != null && !component2.hitCheck(array[0].point))
									{
										goto IL_812;
									}
								}
								else
								{
									UIRect uirect2 = NGUITools.FindInParents<UIRect>(gameObject2);
									if (uirect2 != null && uirect2.finalAlpha < 0.001f)
									{
										goto IL_812;
									}
								}
								if (UICamera.IsVisible(array[0].point, array[0].collider.gameObject))
								{
									UICamera.lastHit = array[0];
									UICamera.lastWorldRay = ray;
									UICamera.lastWorldPosition = array[0].point;
									UICamera.mRayHitObject = UICamera.lastHit.collider.gameObject;
									nProfiler.EndSample();
									return true;
								}
							}
						}
						else if (uicamera.eventType == UICamera.EventType.World_2D)
						{
							if (UICamera.m2DPlane.Raycast(ray, out distance))
							{
								Vector3 point = ray.GetPoint(distance);
								Collider2D collider2D = Physics2D.OverlapPoint(point, layerMask);
								if (collider2D)
								{
									UICamera.lastWorldPosition = point;
									UICamera.mRayHitObject = collider2D.gameObject;
									if (!uicamera.eventsGoToColliders)
									{
										Rigidbody2D rigidbody2D = UICamera.FindRootRigidbody2D(UICamera.mRayHitObject.transform);
										if (rigidbody2D != null)
										{
											UICamera.mRayHitObject = rigidbody2D.gameObject;
										}
									}
									nProfiler.EndSample();
									return true;
								}
							}
						}
						else if (uicamera.eventType == UICamera.EventType.UI_2D)
						{
							if (UICamera.m2DPlane.Raycast(ray, out distance))
							{
								UICamera.lastWorldPosition = ray.GetPoint(distance);
								Collider2D[] array2 = Physics2D.OverlapPointAll(UICamera.lastWorldPosition, layerMask);
								int num2 = array2.Length;
								if (num2 > 1)
								{
									int l = 0;
									while (l < num2)
									{
										GameObject gameObject3 = array2[l].gameObject;
										UIWidget component3 = gameObject3.GetComponent<UIWidget>();
										if (component3 != null)
										{
											if (component3.isVisible)
											{
												if (component3.hitCheck == null || component3.hitCheck(UICamera.lastWorldPosition))
												{
													goto IL_663;
												}
											}
										}
										else
										{
											UIRect uirect3 = NGUITools.FindInParents<UIRect>(gameObject3);
											if (!(uirect3 != null) || uirect3.finalAlpha >= 0.001f)
											{
												goto IL_663;
											}
										}
										IL_6B2:
										l++;
										continue;
										IL_663:
										UICamera.mHit.depth = NGUITools.CalculateRaycastDepth(gameObject3);
										if (UICamera.mHit.depth != 2147483647)
										{
											UICamera.mHit.go = gameObject3;
											UICamera.mHit.point = UICamera.lastWorldPosition;
											UICamera.mHits.Add(UICamera.mHit);
											goto IL_6B2;
										}
										goto IL_6B2;
									}
									UICamera.mHits.Sort((UICamera.DepthEntry r1, UICamera.DepthEntry r2) => r2.depth.CompareTo(r1.depth));
									for (int m = 0; m < UICamera.mHits.size; m++)
									{
										if (UICamera.IsVisible(ref UICamera.mHits.buffer[m]))
										{
											UICamera.mRayHitObject = UICamera.mHits[m].go;
											UICamera.mHits.Clear();
											nProfiler.EndSample();
											return true;
										}
									}
									UICamera.mHits.Clear();
								}
								else if (num2 == 1)
								{
									GameObject gameObject4 = array2[0].gameObject;
									UIWidget component4 = gameObject4.GetComponent<UIWidget>();
									if (component4 != null)
									{
										if (!component4.isVisible)
										{
											goto IL_812;
										}
										if (component4.hitCheck != null && !component4.hitCheck(UICamera.lastWorldPosition))
										{
											goto IL_812;
										}
									}
									else
									{
										UIRect uirect4 = NGUITools.FindInParents<UIRect>(gameObject4);
										if (uirect4 != null && uirect4.finalAlpha < 0.001f)
										{
											goto IL_812;
										}
									}
									if (UICamera.IsVisible(UICamera.lastWorldPosition, gameObject4))
									{
										UICamera.mRayHitObject = gameObject4;
										nProfiler.EndSample();
										return true;
									}
								}
							}
						}
					}
				}
			}
			IL_812:;
		}
		nProfiler.EndSample();
		return false;
	}

	// Token: 0x0600083B RID: 2107 RVA: 0x0004D21C File Offset: 0x0004B41C
	private static bool IsVisible(Vector3 worldPoint, GameObject go)
	{
		UIPanel uipanel = NGUITools.FindInParents<UIPanel>(go);
		while (uipanel != null)
		{
			if (!uipanel.IsVisible(worldPoint))
			{
				return false;
			}
			uipanel = uipanel.parentPanel;
		}
		return true;
	}

	// Token: 0x0600083C RID: 2108 RVA: 0x0004D258 File Offset: 0x0004B458
	private static bool IsVisible(ref UICamera.DepthEntry de)
	{
		UIPanel uipanel = NGUITools.FindInParents<UIPanel>(de.go);
		while (uipanel != null)
		{
			if (!uipanel.IsVisible(de.point))
			{
				return false;
			}
			uipanel = uipanel.parentPanel;
		}
		return true;
	}

	// Token: 0x0600083D RID: 2109 RVA: 0x0000A4B0 File Offset: 0x000086B0
	public static bool IsHighlighted(GameObject go)
	{
		return UICamera.hoveredObject == go;
	}

	// Token: 0x0600083E RID: 2110 RVA: 0x0004D2A0 File Offset: 0x0004B4A0
	public static UICamera FindCameraForLayer(int layer)
	{
		int num = 1 << layer;
		for (int i = 0; i < UICamera.list.size; i++)
		{
			UICamera uicamera = UICamera.list.buffer[i];
			Camera cachedCamera = uicamera.cachedCamera;
			if (cachedCamera != null && (cachedCamera.cullingMask & num) != 0)
			{
				return uicamera;
			}
		}
		return null;
	}

	// Token: 0x0600083F RID: 2111 RVA: 0x0000A4BD File Offset: 0x000086BD
	private static int GetDirection(KeyCode up, KeyCode down)
	{
		if (UICamera.GetKeyDown(up))
		{
			UICamera.currentKey = up;
			return 1;
		}
		if (UICamera.GetKeyDown(down))
		{
			UICamera.currentKey = down;
			return -1;
		}
		return 0;
	}

	// Token: 0x06000840 RID: 2112 RVA: 0x0004D300 File Offset: 0x0004B500
	private static int GetDirection(KeyCode up0, KeyCode up1, KeyCode down0, KeyCode down1)
	{
		if (UICamera.GetKeyDown(up0))
		{
			UICamera.currentKey = up0;
			return 1;
		}
		if (UICamera.GetKeyDown(up1))
		{
			UICamera.currentKey = up1;
			return 1;
		}
		if (UICamera.GetKeyDown(down0))
		{
			UICamera.currentKey = down0;
			return -1;
		}
		if (UICamera.GetKeyDown(down1))
		{
			UICamera.currentKey = down1;
			return -1;
		}
		return 0;
	}

	// Token: 0x06000841 RID: 2113 RVA: 0x0004D370 File Offset: 0x0004B570
	private static int GetDirection(string axis)
	{
		float time = RealTime.time;
		if (UICamera.mNextEvent < time && !string.IsNullOrEmpty(axis))
		{
			float num = UICamera.GetAxis(axis);
			if (num > 0.75f)
			{
				UICamera.currentKey = KeyCode.JoystickButton0;
				UICamera.mNextEvent = time + 0.25f;
				return 1;
			}
			if (num < -0.75f)
			{
				UICamera.currentKey = KeyCode.JoystickButton0;
				UICamera.mNextEvent = time + 0.25f;
				return -1;
			}
		}
		return 0;
	}

	// Token: 0x06000842 RID: 2114 RVA: 0x0004D3EC File Offset: 0x0004B5EC
	public static void Notify(GameObject go, string funcName, object obj)
	{
		if (!UICamera.sendNotify)
		{
			return;
		}
		if (UICamera.mNotifying > 10)
		{
			return;
		}
		if (UICamera.currentScheme == UICamera.ControlScheme.Controller && UIPopupList.isOpen && UIPopupList.current.source == go && UIPopupList.isOpen)
		{
			go = UIPopupList.current.gameObject;
		}
		if (go && go.activeInHierarchy)
		{
			UICamera.mNotifying++;
			go.SendMessage(funcName, obj, SendMessageOptions.DontRequireReceiver);
			if (UICamera.mGenericHandler != null && UICamera.mGenericHandler != go)
			{
				UICamera.mGenericHandler.SendMessage(funcName, obj, SendMessageOptions.DontRequireReceiver);
			}
			UICamera.mNotifying--;
		}
	}

	// Token: 0x06000843 RID: 2115 RVA: 0x0004D4B8 File Offset: 0x0004B6B8
	private void Awake()
	{
		UICamera.mWidth = Screen.width;
		UICamera.mHeight = Screen.height;
		UICamera.currentScheme = UICamera.ControlScheme.Touch;
		UICamera.mMouse[0].pos = Input.mousePosition;
		for (int i = 1; i < 3; i++)
		{
			UICamera.mMouse[i].pos = UICamera.mMouse[0].pos;
			UICamera.mMouse[i].lastPos = UICamera.mMouse[0].pos;
		}
		UICamera.mLastPos = UICamera.mMouse[0].pos;
	}

	// Token: 0x06000844 RID: 2116 RVA: 0x0000A4F0 File Offset: 0x000086F0
	private void OnEnable()
	{
		UICamera.list.Add(this);
		UICamera.list.Sort(new BetterList<UICamera>.CompareFunc(UICamera.CompareFunc));
	}

	// Token: 0x06000845 RID: 2117 RVA: 0x0000A513 File Offset: 0x00008713
	private void OnDisable()
	{
		UICamera.list.Remove(this);
	}

	// Token: 0x06000846 RID: 2118 RVA: 0x0004D548 File Offset: 0x0004B748
	private void Start()
	{
		UICamera.list.Sort(new BetterList<UICamera>.CompareFunc(UICamera.CompareFunc));
		if (this.eventType != UICamera.EventType.World_3D && this.cachedCamera.transparencySortMode != TransparencySortMode.Orthographic)
		{
			this.cachedCamera.transparencySortMode = TransparencySortMode.Orthographic;
		}
		if (Application.isPlaying)
		{
			if (UICamera.fallThrough == null)
			{
				UIRoot uiroot = NGUITools.FindInParents<UIRoot>(base.gameObject);
				UICamera.fallThrough = ((!(uiroot != null)) ? base.gameObject : uiroot.gameObject);
			}
			this.cachedCamera.eventMask = 0;
		}
	}

	// Token: 0x06000847 RID: 2119 RVA: 0x0000A521 File Offset: 0x00008721
	[ContextMenu("Start ignoring events")]
	private void StartIgnoring()
	{
		UICamera.ignoreAllEvents = true;
	}

	// Token: 0x06000848 RID: 2120 RVA: 0x0000A529 File Offset: 0x00008729
	[ContextMenu("Stop ignoring events")]
	private void StopIgnoring()
	{
		UICamera.ignoreAllEvents = false;
	}

	// Token: 0x06000849 RID: 2121 RVA: 0x0000A531 File Offset: 0x00008731
	private void Update()
	{
		if (UICamera.ignoreAllEvents)
		{
			return;
		}
		if (!this.handlesEvents)
		{
			return;
		}
		if (this.processEventsIn == UICamera.ProcessEventsIn.Update)
		{
			this.ProcessEvents();
		}
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        useMouse = true;
#endif
    }

	// Token: 0x0600084A RID: 2122 RVA: 0x0004D5E8 File Offset: 0x0004B7E8
	private void LateUpdate()
	{
		if (!this.handlesEvents)
		{
			return;
		}
		if (this.processEventsIn == UICamera.ProcessEventsIn.LateUpdate)
		{
			this.ProcessEvents();
		}
		int width = Screen.width;
		int height = Screen.height;
		if (width != UICamera.mWidth || height != UICamera.mHeight)
		{
			UICamera.mWidth = width;
			UICamera.mHeight = height;
			UIRoot.Broadcast("UpdateAnchors");
			if (UICamera.onScreenResize != null)
			{
				UICamera.onScreenResize();
			}
		}
	}

	// Token: 0x0600084B RID: 2123 RVA: 0x0004D660 File Offset: 0x0004B860
	private void ProcessEvents()
	{
		nProfiler.BeginSample("UICamera.ProcessEvents");
		UICamera.current = this;
		NGUIDebug.debugRaycast = this.debug;
		if (this.useTouch)
		{
			this.ProcessTouches();
		}
		else if (this.useMouse)
		{
			this.ProcessMouse();
		}
		if (UICamera.onCustomInput != null)
		{
			UICamera.onCustomInput();
		}
		if ((this.useKeyboard || this.useController) && !UICamera.disableController && !UICamera.ignoreControllerInput)
		{
			this.ProcessOthers();
		}
		if (this.useMouse && UICamera.mHover != null)
		{
			float num = string.IsNullOrEmpty(this.scrollAxisName) ? 0f : UICamera.GetAxis(this.scrollAxisName);
			if (num != 0f)
			{
				if (UICamera.onScroll != null)
				{
					UICamera.onScroll(UICamera.mHover, num);
				}
				UICamera.Notify(UICamera.mHover, "OnScroll", num);
			}
			if (UICamera.currentScheme == UICamera.ControlScheme.Mouse && UICamera.showTooltips && UICamera.mTooltipTime != 0f && !UIPopupList.isOpen && UICamera.mMouse[0].dragged == null && (UICamera.mTooltipTime < Time.unscaledTime || UICamera.GetKey(KeyCode.LeftShift) || UICamera.GetKey(KeyCode.RightShift)))
			{
				UICamera.currentTouch = UICamera.mMouse[0];
				UICamera.currentTouchID = -1;
				UICamera.ShowTooltip(UICamera.mHover);
			}
		}
		if (UICamera.mTooltip != null && !NGUITools.GetActive(UICamera.mTooltip))
		{
			UICamera.ShowTooltip(null);
		}
		UICamera.current = null;
		UICamera.currentTouchID = -100;
		nProfiler.EndSample();
	}

	// Token: 0x0600084C RID: 2124 RVA: 0x0004D844 File Offset: 0x0004BA44
	public void ProcessMouse()
	{
		bool flag = false;
		bool flag2 = false;
		for (int i = 0; i < 3; i++)
		{
			if (Input.GetMouseButtonDown(i))
			{
				UICamera.currentKey = KeyCode.Mouse0 + i;
				flag2 = true;
				flag = true;
			}
			else if (Input.GetMouseButton(i))
			{
				UICamera.currentKey = KeyCode.Mouse0 + i;
				flag = true;
			}
		}
		if (UICamera.currentScheme == UICamera.ControlScheme.Touch && UICamera.activeTouches.Count > 0)
		{
			return;
		}
		UICamera.currentTouch = UICamera.mMouse[0];
		Vector2 vector = Input.mousePosition;
		if (UICamera.currentTouch.ignoreDelta == 0)
		{
			UICamera.currentTouch.delta = vector - UICamera.currentTouch.pos;
		}
		else
		{
			UICamera.currentTouch.ignoreDelta--;
			UICamera.currentTouch.delta.x = 0f;
			UICamera.currentTouch.delta.y = 0f;
		}
		float sqrMagnitude = UICamera.currentTouch.delta.sqrMagnitude;
		UICamera.currentTouch.pos = vector;
		UICamera.mLastPos = vector;
		bool flag3 = false;
		if (UICamera.currentScheme != UICamera.ControlScheme.Mouse)
		{
			if (sqrMagnitude < 0.001f)
			{
				return;
			}
			UICamera.currentKey = KeyCode.Mouse0;
			flag3 = true;
		}
		else if (sqrMagnitude > 0.001f)
		{
			flag3 = true;
		}
		for (int j = 1; j < 3; j++)
		{
			UICamera.mMouse[j].pos = UICamera.currentTouch.pos;
			UICamera.mMouse[j].delta = UICamera.currentTouch.delta;
		}
		if (flag || flag3 || this.mNextRaycast < RealTime.time)
		{
			this.mNextRaycast = RealTime.time + 0.02f;
			UICamera.Raycast(UICamera.currentTouch);
			if (flag)
			{
				flag3 = true;
				for (int k = 0; k < 3; k++)
				{
					UICamera.mMouse[k].current = UICamera.currentTouch.current;
				}
			}
			else if (UICamera.mMouse[0].current != UICamera.currentTouch.current)
			{
				UICamera.currentKey = KeyCode.Mouse0;
				flag3 = true;
				for (int l = 0; l < 3; l++)
				{
					UICamera.mMouse[l].current = UICamera.currentTouch.current;
				}
			}
		}
		bool flag4 = UICamera.currentTouch.last != UICamera.currentTouch.current;
		bool flag5 = UICamera.currentTouch.pressed != null;
		if (!flag5 && flag3)
		{
			UICamera.hoveredObject = UICamera.currentTouch.current;
		}
		UICamera.currentTouchID = -1;
		if (flag4)
		{
			UICamera.currentKey = KeyCode.Mouse0;
		}
		if (!flag && flag3)
		{
			if (UICamera.mTooltipTime != 0f)
			{
				UICamera.mTooltipTime = Time.unscaledTime + this.tooltipDelay;
			}
			else if (UICamera.mTooltip != null && (!this.stickyTooltip || flag4))
			{
				UICamera.ShowTooltip(null);
			}
		}
		if (flag3 && UICamera.onMouseMove != null)
		{
			UICamera.onMouseMove(UICamera.currentTouch.delta);
			UICamera.currentTouch = null;
		}
		if (flag4 && (flag2 || (flag5 && !flag)))
		{
			UICamera.hoveredObject = null;
		}
		for (int m = 0; m < 3; m++)
		{
			bool mouseButtonDown = Input.GetMouseButtonDown(m);
			bool mouseButtonUp = Input.GetMouseButtonUp(m);
			if (mouseButtonDown || mouseButtonUp)
			{
				UICamera.currentKey = KeyCode.Mouse0 + m;
			}
			UICamera.currentTouch = UICamera.mMouse[m];
			UICamera.currentTouchID = -1 - m;
			UICamera.currentKey = KeyCode.Mouse0 + m;
			if (mouseButtonDown)
			{
				UICamera.currentTouch.pressedCam = UICamera.currentCamera;
				UICamera.currentTouch.pressTime = RealTime.time;
			}
			else if (UICamera.currentTouch.pressed != null)
			{
				UICamera.currentCamera = UICamera.currentTouch.pressedCam;
			}
			this.ProcessTouch(mouseButtonDown, mouseButtonUp);
		}
		if (!flag && flag4)
		{
			UICamera.currentTouch = UICamera.mMouse[0];
			UICamera.mTooltipTime = Time.unscaledTime + this.tooltipDelay;
			UICamera.currentTouchID = -1;
			UICamera.currentKey = KeyCode.Mouse0;
			UICamera.hoveredObject = UICamera.currentTouch.current;
		}
		UICamera.currentTouch = null;
		UICamera.mMouse[0].last = UICamera.mMouse[0].current;
		for (int n = 1; n < 3; n++)
		{
			UICamera.mMouse[n].last = UICamera.mMouse[0].last;
		}
	}

	// Token: 0x0600084D RID: 2125 RVA: 0x0004DD04 File Offset: 0x0004BF04
	public void ProcessTouches()
	{
		nProfiler.BeginSample("UICamera.ProcessTouches");
		int touchCount = Input.touchCount;
		for (int i = 0; i < touchCount; i++)
		{
			UnityEngine.Touch touch = Input.GetTouch(i);
			TouchPhase phase = touch.phase;
			int fingerId = touch.fingerId;
			Vector2 position = touch.position;
			int tapCount = touch.tapCount;
			UICamera.currentTouchID = ((!this.allowMultiTouch) ? 1 : fingerId);
			UICamera.currentTouch = UICamera.GetTouch(UICamera.currentTouchID, true);
			bool flag = phase == TouchPhase.Began || UICamera.currentTouch.touchBegan;
			bool flag2 = phase == TouchPhase.Canceled || phase == TouchPhase.Ended;
			UICamera.currentTouch.delta = position - UICamera.currentTouch.pos;
			UICamera.currentTouch.pos = position;
			UICamera.currentKey = KeyCode.None;
			UICamera.Raycast(UICamera.currentTouch);
			if (flag)
			{
				UICamera.currentTouch.pressedCam = UICamera.currentCamera;
			}
			else if (UICamera.currentTouch.pressed != null)
			{
				UICamera.currentCamera = UICamera.currentTouch.pressedCam;
			}
			if (tapCount > 1)
			{
				UICamera.currentTouch.clickTime = RealTime.time;
			}
			this.ProcessTouch(flag, flag2);
			if (flag2)
			{
				UICamera.RemoveTouch(UICamera.currentTouchID);
			}
			UICamera.currentTouch.touchBegan = false;
			UICamera.currentTouch.last = null;
			UICamera.currentTouch = null;
			if (!this.allowMultiTouch)
			{
				break;
			}
		}
		if (touchCount == 0)
		{
			if (UICamera.mUsingTouchEvents)
			{
				UICamera.mUsingTouchEvents = false;
				return;
			}
			if (this.useMouse)
			{
				this.ProcessMouse();
			}
		}
		else
		{
			UICamera.mUsingTouchEvents = true;
		}
		nProfiler.EndSample();
	}

	// Token: 0x0600084E RID: 2126 RVA: 0x0004DEC0 File Offset: 0x0004C0C0
	private void ProcessFakeTouches()
	{
		bool mouseButtonDown = Input.GetMouseButtonDown(0);
		bool mouseButtonUp = Input.GetMouseButtonUp(0);
		bool mouseButton = Input.GetMouseButton(0);
		if (mouseButtonDown || mouseButtonUp || mouseButton)
		{
			UICamera.currentTouchID = 1;
			UICamera.currentTouch = UICamera.mMouse[0];
			UICamera.currentTouch.touchBegan = mouseButtonDown;
			if (mouseButtonDown)
			{
				UICamera.currentTouch.pressTime = RealTime.time;
				UICamera.activeTouches.Add(UICamera.currentTouch);
			}
			Vector2 vector = Input.mousePosition;
			UICamera.currentTouch.delta = vector - UICamera.currentTouch.pos;
			UICamera.currentTouch.pos = vector;
			UICamera.Raycast(UICamera.currentTouch);
			if (mouseButtonDown)
			{
				UICamera.currentTouch.pressedCam = UICamera.currentCamera;
			}
			else if (UICamera.currentTouch.pressed != null)
			{
				UICamera.currentCamera = UICamera.currentTouch.pressedCam;
			}
			UICamera.currentKey = KeyCode.None;
			this.ProcessTouch(mouseButtonDown, mouseButtonUp);
			if (mouseButtonUp)
			{
				UICamera.activeTouches.Remove(UICamera.currentTouch);
			}
			UICamera.currentTouch.last = null;
			UICamera.currentTouch = null;
		}
	}

	// Token: 0x0600084F RID: 2127 RVA: 0x0004DFE4 File Offset: 0x0004C1E4
	public void ProcessOthers()
	{
		UICamera.currentTouchID = -100;
		UICamera.currentTouch = UICamera.controller;
		bool flag = false;
		bool flag2 = false;
		if (this.submitKey0 != KeyCode.None && UICamera.GetKeyDown(this.submitKey0))
		{
			UICamera.currentKey = this.submitKey0;
			flag = true;
		}
		else if (this.submitKey1 != KeyCode.None && UICamera.GetKeyDown(this.submitKey1))
		{
			UICamera.currentKey = this.submitKey1;
			flag = true;
		}
		else if ((this.submitKey0 == KeyCode.Return || this.submitKey1 == KeyCode.Return) && UICamera.GetKeyDown(KeyCode.KeypadEnter))
		{
			UICamera.currentKey = this.submitKey0;
			flag = true;
		}
		if (this.submitKey0 != KeyCode.None && UICamera.GetKeyUp(this.submitKey0))
		{
			UICamera.currentKey = this.submitKey0;
			flag2 = true;
		}
		else if (this.submitKey1 != KeyCode.None && UICamera.GetKeyUp(this.submitKey1))
		{
			UICamera.currentKey = this.submitKey1;
			flag2 = true;
		}
		else if ((this.submitKey0 == KeyCode.Return || this.submitKey1 == KeyCode.Return) && UICamera.GetKeyUp(KeyCode.KeypadEnter))
		{
			UICamera.currentKey = this.submitKey0;
			flag2 = true;
		}
		if (flag)
		{
			UICamera.currentTouch.pressTime = RealTime.time;
		}
		if ((flag || flag2) && UICamera.currentScheme == UICamera.ControlScheme.Controller)
		{
			UICamera.currentTouch.current = UICamera.controllerNavigationObject;
			this.ProcessTouch(flag, flag2);
			UICamera.currentTouch.last = UICamera.currentTouch.current;
		}
		KeyCode keyCode = KeyCode.None;
		if (this.useController && !UICamera.ignoreControllerInput)
		{
			if (!UICamera.disableController && UICamera.currentScheme == UICamera.ControlScheme.Controller && (UICamera.currentTouch.current == null || !UICamera.currentTouch.current.activeInHierarchy))
			{
				UICamera.currentTouch.current = UICamera.controllerNavigationObject;
			}
			if (!string.IsNullOrEmpty(this.verticalAxisName))
			{
				int direction = UICamera.GetDirection(this.verticalAxisName);
				if (direction != 0)
				{
					UICamera.ShowTooltip(null);
					UICamera.currentScheme = UICamera.ControlScheme.Controller;
					UICamera.currentTouch.current = UICamera.controllerNavigationObject;
					if (UICamera.currentTouch.current != null)
					{
						keyCode = ((direction <= 0) ? KeyCode.DownArrow : KeyCode.UpArrow);
						if (UICamera.onNavigate != null)
						{
							UICamera.onNavigate(UICamera.currentTouch.current, keyCode);
						}
						UICamera.Notify(UICamera.currentTouch.current, "OnNavigate", keyCode);
					}
				}
			}
			if (!string.IsNullOrEmpty(this.horizontalAxisName))
			{
				int direction2 = UICamera.GetDirection(this.horizontalAxisName);
				if (direction2 != 0)
				{
					UICamera.ShowTooltip(null);
					UICamera.currentScheme = UICamera.ControlScheme.Controller;
					UICamera.currentTouch.current = UICamera.controllerNavigationObject;
					if (UICamera.currentTouch.current != null)
					{
						keyCode = ((direction2 <= 0) ? KeyCode.LeftArrow : KeyCode.RightArrow);
						if (UICamera.onNavigate != null)
						{
							UICamera.onNavigate(UICamera.currentTouch.current, keyCode);
						}
						UICamera.Notify(UICamera.currentTouch.current, "OnNavigate", keyCode);
					}
				}
			}
			float num = string.IsNullOrEmpty(this.horizontalPanAxisName) ? 0f : UICamera.GetAxis(this.horizontalPanAxisName);
			float num2 = string.IsNullOrEmpty(this.verticalPanAxisName) ? 0f : UICamera.GetAxis(this.verticalPanAxisName);
			if (num != 0f || num2 != 0f)
			{
				UICamera.ShowTooltip(null);
				UICamera.currentScheme = UICamera.ControlScheme.Controller;
				UICamera.currentTouch.current = UICamera.controllerNavigationObject;
				if (UICamera.currentTouch.current != null)
				{
					Vector2 vector = new Vector2(num, num2);
					vector *= Time.unscaledDeltaTime;
					if (UICamera.onPan != null)
					{
						UICamera.onPan(UICamera.currentTouch.current, vector);
					}
					UICamera.Notify(UICamera.currentTouch.current, "OnPan", vector);
				}
			}
		}
		if ((UICamera.GetAnyKeyDown == null) ? Input.anyKeyDown : UICamera.GetAnyKeyDown())
		{
			int i = 0;
			int num3 = NGUITools.keys.Length;
			while (i < num3)
			{
				KeyCode keyCode2 = NGUITools.keys[i];
				if (keyCode != keyCode2)
				{
					if (UICamera.GetKeyDown(keyCode2))
					{
						if (this.useKeyboard || keyCode2 >= KeyCode.Mouse0)
						{
							if ((this.useController && !UICamera.ignoreControllerInput) || keyCode2 < KeyCode.JoystickButton0)
							{
								if (this.useMouse || keyCode2 < KeyCode.Mouse0 || keyCode2 > KeyCode.Mouse6)
								{
									UICamera.currentKey = keyCode2;
									if (UICamera.onKey != null)
									{
										UICamera.onKey(UICamera.currentTouch.current, keyCode2);
									}
									UICamera.Notify(UICamera.currentTouch.current, "OnKey", keyCode2);
								}
							}
						}
					}
				}
				i++;
			}
		}
		UICamera.currentTouch = null;
	}

	// Token: 0x06000850 RID: 2128 RVA: 0x0004E560 File Offset: 0x0004C760
	private void ProcessPress(bool pressed, float click, float drag)
	{
		if (pressed)
		{
			if (UICamera.mTooltip != null)
			{
				UICamera.ShowTooltip(null);
			}
			UICamera.mTooltipTime = Time.unscaledTime + this.tooltipDelay;
			UICamera.currentTouch.pressStarted = true;
			if (UICamera.onPress != null && UICamera.currentTouch.pressed)
			{
				UICamera.onPress(UICamera.currentTouch.pressed, false);
			}
			UICamera.Notify(UICamera.currentTouch.pressed, "OnPress", false);
			if (UICamera.currentScheme == UICamera.ControlScheme.Mouse && UICamera.hoveredObject == null && UICamera.currentTouch.current != null)
			{
				UICamera.hoveredObject = UICamera.currentTouch.current;
			}
			UICamera.currentTouch.pressed = UICamera.currentTouch.current;
			UICamera.currentTouch.dragged = UICamera.currentTouch.current;
			UICamera.currentTouch.clickNotification = UICamera.ClickNotification.BasedOnDelta;
			UICamera.currentTouch.totalDelta = Vector2.zero;
			UICamera.currentTouch.dragStarted = false;
			if (UICamera.onPress != null && UICamera.currentTouch.pressed)
			{
				UICamera.onPress(UICamera.currentTouch.pressed, true);
			}
			UICamera.Notify(UICamera.currentTouch.pressed, "OnPress", true);
			if (UICamera.mSelected != UICamera.currentTouch.pressed)
			{
				UICamera.mInputFocus = false;
				if (UICamera.mSelected)
				{
					UICamera.Notify(UICamera.mSelected, "OnSelect", false);
					if (UICamera.onSelect != null)
					{
						UICamera.onSelect(UICamera.mSelected, false);
					}
				}
				UICamera.mSelected = UICamera.currentTouch.pressed;
				if (UICamera.currentTouch.pressed != null)
				{
				}
				if (UICamera.mSelected)
				{
					if (UICamera.clickUIInput)
					{
						UICamera.mInputFocus = (UICamera.mSelected.activeInHierarchy && UICamera.mSelected.GetComponent<UIInput>() != null);
					}
					if (UICamera.onSelect != null)
					{
						UICamera.onSelect(UICamera.mSelected, true);
					}
					UICamera.Notify(UICamera.mSelected, "OnSelect", true);
				}
			}
		}
		else if (UICamera.currentTouch.pressed != null && (UICamera.currentTouch.delta.sqrMagnitude != 0f || UICamera.currentTouch.current != UICamera.currentTouch.last))
		{
			UICamera.currentTouch.totalDelta += UICamera.currentTouch.delta;
			float sqrMagnitude = UICamera.currentTouch.totalDelta.sqrMagnitude;
			bool flag = false;
			if (!UICamera.currentTouch.dragStarted && UICamera.currentTouch.last != UICamera.currentTouch.current)
			{
				UICamera.currentTouch.dragStarted = true;
				UICamera.currentTouch.delta = UICamera.currentTouch.totalDelta;
				UICamera.isDragging = true;
				if (UICamera.onDragStart != null)
				{
					UICamera.onDragStart(UICamera.currentTouch.dragged);
				}
				UICamera.Notify(UICamera.currentTouch.dragged, "OnDragStart", null);
				if (UICamera.onDragOver != null)
				{
					UICamera.onDragOver(UICamera.currentTouch.last, UICamera.currentTouch.dragged);
				}
				UICamera.Notify(UICamera.currentTouch.last, "OnDragOver", UICamera.currentTouch.dragged);
				UICamera.isDragging = false;
			}
			else if (!UICamera.currentTouch.dragStarted && drag < sqrMagnitude)
			{
				flag = true;
				UICamera.currentTouch.dragStarted = true;
				UICamera.currentTouch.delta = UICamera.currentTouch.totalDelta;
			}
			if (UICamera.currentTouch.dragStarted)
			{
				if (UICamera.mTooltip != null)
				{
					UICamera.ShowTooltip(null);
				}
				UICamera.isDragging = true;
				bool flag2 = UICamera.currentTouch.clickNotification == UICamera.ClickNotification.None;
				if (flag)
				{
					if (UICamera.onDragStart != null)
					{
						UICamera.onDragStart(UICamera.currentTouch.dragged);
					}
					UICamera.Notify(UICamera.currentTouch.dragged, "OnDragStart", null);
					if (UICamera.onDragOver != null)
					{
						UICamera.onDragOver(UICamera.currentTouch.last, UICamera.currentTouch.dragged);
					}
					UICamera.Notify(UICamera.currentTouch.current, "OnDragOver", UICamera.currentTouch.dragged);
				}
				else if (UICamera.currentTouch.last != UICamera.currentTouch.current)
				{
					if (UICamera.onDragOut != null)
					{
						UICamera.onDragOut(UICamera.currentTouch.last, UICamera.currentTouch.dragged);
					}
					UICamera.Notify(UICamera.currentTouch.last, "OnDragOut", UICamera.currentTouch.dragged);
					if (UICamera.onDragOver != null)
					{
						UICamera.onDragOver(UICamera.currentTouch.last, UICamera.currentTouch.dragged);
					}
					UICamera.Notify(UICamera.currentTouch.current, "OnDragOver", UICamera.currentTouch.dragged);
				}
				if (UICamera.onDrag != null)
				{
					UICamera.onDrag(UICamera.currentTouch.dragged, UICamera.currentTouch.delta);
				}
				UICamera.Notify(UICamera.currentTouch.dragged, "OnDrag", UICamera.currentTouch.delta);
				UICamera.currentTouch.last = UICamera.currentTouch.current;
				UICamera.isDragging = false;
				if (flag2)
				{
					UICamera.currentTouch.clickNotification = UICamera.ClickNotification.None;
				}
				else if (UICamera.currentTouch.clickNotification == UICamera.ClickNotification.BasedOnDelta && click < sqrMagnitude)
				{
					UICamera.currentTouch.clickNotification = UICamera.ClickNotification.None;
				}
			}
		}
	}

	// Token: 0x06000851 RID: 2129 RVA: 0x0004EB50 File Offset: 0x0004CD50
	private void ProcessRelease(bool isMouse, float drag)
	{
		if (UICamera.currentTouch == null)
		{
			return;
		}
		UICamera.currentTouch.pressStarted = false;
		if (UICamera.currentTouch.pressed != null)
		{
			if (UICamera.currentTouch.dragStarted)
			{
				if (UICamera.onDragOut != null)
				{
					UICamera.onDragOut(UICamera.currentTouch.last, UICamera.currentTouch.dragged);
				}
				UICamera.Notify(UICamera.currentTouch.last, "OnDragOut", UICamera.currentTouch.dragged);
				if (UICamera.onDragEnd != null)
				{
					UICamera.onDragEnd(UICamera.currentTouch.dragged);
				}
				UICamera.Notify(UICamera.currentTouch.dragged, "OnDragEnd", null);
			}
			if (UICamera.onPress != null)
			{
				UICamera.onPress(UICamera.currentTouch.pressed, false);
			}
			UICamera.Notify(UICamera.currentTouch.pressed, "OnPress", false);
			if (isMouse)
			{
				bool flag = this.HasCollider(UICamera.currentTouch.pressed);
				if (flag)
				{
					if (UICamera.mHover == UICamera.currentTouch.current)
					{
						if (UICamera.onHover != null)
						{
							UICamera.onHover(UICamera.currentTouch.current, true);
						}
						UICamera.Notify(UICamera.currentTouch.current, "OnHover", true);
					}
					else
					{
						UICamera.hoveredObject = UICamera.currentTouch.current;
					}
				}
			}
			if (UICamera.currentTouch.dragged == UICamera.currentTouch.current || (UICamera.currentScheme != UICamera.ControlScheme.Controller && UICamera.currentTouch.clickNotification != UICamera.ClickNotification.None && UICamera.currentTouch.totalDelta.sqrMagnitude < drag))
			{
				if (UICamera.currentTouch.clickNotification != UICamera.ClickNotification.None && UICamera.currentTouch.pressed == UICamera.currentTouch.current)
				{
					UICamera.ShowTooltip(null);
					float time = RealTime.time;
					if (UICamera.onClick != null)
					{
						UICamera.onClick(UICamera.currentTouch.pressed);
					}
					UICamera.Notify(UICamera.currentTouch.pressed, "OnClick", null);
					if (UICamera.currentTouch.clickTime + 0.35f > time)
					{
						if (UICamera.onDoubleClick != null)
						{
							UICamera.onDoubleClick(UICamera.currentTouch.pressed);
						}
						UICamera.Notify(UICamera.currentTouch.pressed, "OnDoubleClick", null);
					}
					UICamera.currentTouch.clickTime = time;
				}
			}
			else if (UICamera.currentTouch.dragStarted)
			{
				if (UICamera.onDrop != null)
				{
					UICamera.onDrop(UICamera.currentTouch.current, UICamera.currentTouch.dragged);
				}
				UICamera.Notify(UICamera.currentTouch.current, "OnDrop", UICamera.currentTouch.dragged);
			}
		}
		UICamera.currentTouch.dragStarted = false;
		UICamera.currentTouch.pressed = null;
		UICamera.currentTouch.dragged = null;
	}

	// Token: 0x06000852 RID: 2130 RVA: 0x0004EE58 File Offset: 0x0004D058
	private bool HasCollider(GameObject go)
	{
		if (go == null)
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

	// Token: 0x06000853 RID: 2131 RVA: 0x0004EEAC File Offset: 0x0004D0AC
	public void ProcessTouch(bool pressed, bool released)
	{
		nProfiler.BeginSample("UICamera.ProcessTouch");
		if (released)
		{
			UICamera.mTooltipTime = 0f;
		}
		bool flag = UICamera.currentScheme == UICamera.ControlScheme.Mouse;
		float num = (!flag) ? this.touchDragThreshold : this.mouseDragThreshold;
		float num2 = (!flag) ? this.touchClickThreshold : this.mouseClickThreshold;
		num *= num;
		num2 *= num2;
		if (UICamera.currentTouch.pressed != null)
		{
			if (released)
			{
				this.ProcessRelease(flag, num);
			}
			this.ProcessPress(pressed, num2, num);
			if (this.tooltipDelay != 0f && UICamera.currentTouch.deltaTime > this.tooltipDelay && UICamera.currentTouch.pressed == UICamera.currentTouch.current && UICamera.mTooltipTime != 0f && !UICamera.currentTouch.dragStarted)
			{
				UICamera.mTooltipTime = 0f;
				UICamera.currentTouch.clickNotification = UICamera.ClickNotification.None;
				if (this.longPressTooltip)
				{
					UICamera.ShowTooltip(UICamera.currentTouch.pressed);
				}
				UICamera.Notify(UICamera.currentTouch.current, "OnLongPress", null);
			}
		}
		else if (flag || pressed || released)
		{
			this.ProcessPress(pressed, num2, num);
			if (released)
			{
				this.ProcessRelease(flag, num);
			}
		}
		nProfiler.EndSample();
	}

	// Token: 0x06000854 RID: 2132 RVA: 0x0000A55B File Offset: 0x0000875B
	public static void CancelNextTooltip()
	{
		UICamera.mTooltipTime = 0f;
	}

	// Token: 0x06000855 RID: 2133 RVA: 0x0004F01C File Offset: 0x0004D21C
	public static bool ShowTooltip(GameObject go)
	{
		if (UICamera.mTooltip != go)
		{
			if (UICamera.mTooltip != null)
			{
				if (UICamera.onTooltip != null)
				{
					UICamera.onTooltip(UICamera.mTooltip, false);
				}
				UICamera.Notify(UICamera.mTooltip, "OnTooltip", false);
			}
			UICamera.mTooltip = go;
			UICamera.mTooltipTime = 0f;
			if (UICamera.mTooltip != null)
			{
				if (UICamera.onTooltip != null)
				{
					UICamera.onTooltip(UICamera.mTooltip, true);
				}
				UICamera.Notify(UICamera.mTooltip, "OnTooltip", true);
			}
			return true;
		}
		return false;
	}

	// Token: 0x06000856 RID: 2134 RVA: 0x0000A567 File Offset: 0x00008767
	public static bool HideTooltip()
	{
		return UICamera.ShowTooltip(null);
	}

	// Token: 0x06000857 RID: 2135 RVA: 0x0000A56F File Offset: 0x0000876F
	public static void ResetTooltip(float delay = 0.5f)
	{
		UICamera.ShowTooltip(null);
		UICamera.mTooltipTime = Time.unscaledTime + delay;
	}

	// Token: 0x0400056A RID: 1386
	public static bool sendNotify = false;

	// Token: 0x0400056B RID: 1387
	public static bool clickUIInput = true;

	// Token: 0x0400056C RID: 1388
	public static BetterList<UICamera> list = new BetterList<UICamera>();

	// Token: 0x0400056D RID: 1389
	public static UICamera.GetKeyStateFunc GetKeyDown = (KeyCode key) => (key < KeyCode.JoystickButton0 || !UICamera.ignoreControllerInput) && Input.GetKeyDown(key);

	// Token: 0x0400056E RID: 1390
	public static UICamera.GetKeyStateFunc GetKeyUp = (KeyCode key) => (key < KeyCode.JoystickButton0 || !UICamera.ignoreControllerInput) && Input.GetKeyUp(key);

	// Token: 0x0400056F RID: 1391
	public static UICamera.GetKeyStateFunc GetKey = (KeyCode key) => (key < KeyCode.JoystickButton0 || !UICamera.ignoreControllerInput) && Input.GetKey(key);

	// Token: 0x04000570 RID: 1392
	public static UICamera.GetAxisFunc GetAxis = delegate(string axis)
	{
		if (UICamera.ignoreControllerInput)
		{
			return 0f;
		}
		return Input.GetAxis(axis);
	};

	// Token: 0x04000571 RID: 1393
	public static UICamera.GetAnyKeyFunc GetAnyKeyDown;

	// Token: 0x04000572 RID: 1394
	public static UICamera.GetMouseDelegate GetMouse = (int button) => UICamera.mMouse[button];

	// Token: 0x04000573 RID: 1395
	public static UICamera.GetTouchDelegate GetTouch = delegate(int id, bool createIfMissing)
	{
		if (id < 0)
		{
			return UICamera.GetMouse(-id - 1);
		}
		int i = 0;
		int size = UICamera.mTouchIDs.size;
		while (i < size)
		{
			if (UICamera.mTouchIDs[i] == id)
			{
				return UICamera.activeTouches[i];
			}
			i++;
		}
		if (createIfMissing)
		{
			UICamera.MouseOrTouch mouseOrTouch = new UICamera.MouseOrTouch();
			mouseOrTouch.pressTime = RealTime.time;
			mouseOrTouch.touchBegan = true;
			UICamera.activeTouches.Add(mouseOrTouch);
			UICamera.mTouchIDs.Add(id);
			return mouseOrTouch;
		}
		return null;
	};

	// Token: 0x04000574 RID: 1396
	public static UICamera.RemoveTouchDelegate RemoveTouch = delegate(int id)
	{
		int i = 0;
		int size = UICamera.mTouchIDs.size;
		while (i < size)
		{
			if (UICamera.mTouchIDs[i] == id)
			{
				UICamera.mTouchIDs.RemoveAt(i);
				UICamera.activeTouches.RemoveAt(i);
				return;
			}
			i++;
		}
	};

	// Token: 0x04000575 RID: 1397
	public static UICamera.OnScreenResize onScreenResize;

	// Token: 0x04000576 RID: 1398
	public UICamera.EventType eventType = UICamera.EventType.UI_3D;

	// Token: 0x04000577 RID: 1399
	public bool eventsGoToColliders;

	// Token: 0x04000578 RID: 1400
	public LayerMask eventReceiverMask = -1;

	// Token: 0x04000579 RID: 1401
	public UICamera.ProcessEventsIn processEventsIn;

	// Token: 0x0400057A RID: 1402
	public bool debug;

	// Token: 0x0400057B RID: 1403
	public bool useMouse = true;

	// Token: 0x0400057C RID: 1404
	public bool useTouch = true;

	// Token: 0x0400057D RID: 1405
	public bool allowMultiTouch = true;

	// Token: 0x0400057E RID: 1406
	public bool useKeyboard = true;

	// Token: 0x0400057F RID: 1407
	public bool useController = true;

	// Token: 0x04000580 RID: 1408
	public bool stickyTooltip = true;

	// Token: 0x04000581 RID: 1409
	public float tooltipDelay = 1f;

	// Token: 0x04000582 RID: 1410
	public bool longPressTooltip;

	// Token: 0x04000583 RID: 1411
	public float mouseDragThreshold = 4f;

	// Token: 0x04000584 RID: 1412
	public float mouseClickThreshold = 10f;

	// Token: 0x04000585 RID: 1413
	public float touchDragThreshold = 40f;

	// Token: 0x04000586 RID: 1414
	public float touchClickThreshold = 40f;

	// Token: 0x04000587 RID: 1415
	public float rangeDistance = -1f;

	// Token: 0x04000588 RID: 1416
	public string horizontalAxisName = "Horizontal";

	// Token: 0x04000589 RID: 1417
	public string verticalAxisName = "Vertical";

	// Token: 0x0400058A RID: 1418
	public string horizontalPanAxisName;

	// Token: 0x0400058B RID: 1419
	public string verticalPanAxisName;

	// Token: 0x0400058C RID: 1420
	public string scrollAxisName = "Mouse ScrollWheel";

	// Token: 0x0400058D RID: 1421
	[Tooltip("If enabled, command-click will result in a right-click event on OSX")]
	public bool commandClick = true;

	// Token: 0x0400058E RID: 1422
	public KeyCode submitKey0 = KeyCode.Return;

	// Token: 0x0400058F RID: 1423
	public KeyCode submitKey1 = KeyCode.JoystickButton0;

	// Token: 0x04000590 RID: 1424
	public KeyCode cancelKey0 = KeyCode.Escape;

	// Token: 0x04000591 RID: 1425
	public KeyCode cancelKey1 = KeyCode.JoystickButton1;

	// Token: 0x04000592 RID: 1426
	public bool autoHideCursor = true;

	// Token: 0x04000593 RID: 1427
	public static UICamera.OnCustomInput onCustomInput;

	// Token: 0x04000594 RID: 1428
	public static bool showTooltips = true;

	// Token: 0x04000595 RID: 1429
	public static bool ignoreAllEvents = false;

	// Token: 0x04000596 RID: 1430
	public static bool ignoreControllerInput = false;

	// Token: 0x04000597 RID: 1431
	private static bool mDisableController = false;

	// Token: 0x04000598 RID: 1432
	private static Vector2 mLastPos = Vector2.zero;

	// Token: 0x04000599 RID: 1433
	public static Vector3 lastWorldPosition = Vector3.zero;

	// Token: 0x0400059A RID: 1434
	public static Ray lastWorldRay = default(Ray);

	// Token: 0x0400059B RID: 1435
	public static RaycastHit lastHit;

	// Token: 0x0400059C RID: 1436
	public static UICamera current = null;

	// Token: 0x0400059D RID: 1437
	public static Camera currentCamera = null;

	// Token: 0x0400059E RID: 1438
	public static UICamera.OnSchemeChange onSchemeChange;

	// Token: 0x0400059F RID: 1439
	private static UICamera.ControlScheme mLastScheme = UICamera.ControlScheme.Mouse;

	// Token: 0x040005A0 RID: 1440
	public static int currentTouchID = -100;

	// Token: 0x040005A1 RID: 1441
	private static KeyCode mCurrentKey = KeyCode.Alpha0;

	// Token: 0x040005A2 RID: 1442
	public static UICamera.MouseOrTouch currentTouch = null;

	// Token: 0x040005A3 RID: 1443
	private static bool mInputFocus = false;

	// Token: 0x040005A4 RID: 1444
	private static GameObject mGenericHandler;

	// Token: 0x040005A5 RID: 1445
	public static GameObject fallThrough;

	// Token: 0x040005A6 RID: 1446
	public static UICamera.VoidDelegate onClick;

	// Token: 0x040005A7 RID: 1447
	public static UICamera.VoidDelegate onDoubleClick;

	// Token: 0x040005A8 RID: 1448
	public static UICamera.BoolDelegate onHover;

	// Token: 0x040005A9 RID: 1449
	public static UICamera.BoolDelegate onPress;

	// Token: 0x040005AA RID: 1450
	public static UICamera.BoolDelegate onSelect;

	// Token: 0x040005AB RID: 1451
	public static UICamera.FloatDelegate onScroll;

	// Token: 0x040005AC RID: 1452
	public static UICamera.VectorDelegate onDrag;

	// Token: 0x040005AD RID: 1453
	public static UICamera.VoidDelegate onDragStart;

	// Token: 0x040005AE RID: 1454
	public static UICamera.ObjectDelegate onDragOver;

	// Token: 0x040005AF RID: 1455
	public static UICamera.ObjectDelegate onDragOut;

	// Token: 0x040005B0 RID: 1456
	public static UICamera.VoidDelegate onDragEnd;

	// Token: 0x040005B1 RID: 1457
	public static UICamera.ObjectDelegate onDrop;

	// Token: 0x040005B2 RID: 1458
	public static UICamera.KeyCodeDelegate onKey;

	// Token: 0x040005B3 RID: 1459
	public static UICamera.KeyCodeDelegate onNavigate;

	// Token: 0x040005B4 RID: 1460
	public static UICamera.VectorDelegate onPan;

	// Token: 0x040005B5 RID: 1461
	public static UICamera.BoolDelegate onTooltip;

	// Token: 0x040005B6 RID: 1462
	public static UICamera.MoveDelegate onMouseMove;

	// Token: 0x040005B7 RID: 1463
	private static UICamera.MouseOrTouch[] mMouse = new UICamera.MouseOrTouch[]
	{
		new UICamera.MouseOrTouch(),
		new UICamera.MouseOrTouch(),
		new UICamera.MouseOrTouch()
	};

	// Token: 0x040005B8 RID: 1464
	public static UICamera.MouseOrTouch controller = new UICamera.MouseOrTouch();

	// Token: 0x040005B9 RID: 1465
	public static List<UICamera.MouseOrTouch> activeTouches = new List<UICamera.MouseOrTouch>();

	// Token: 0x040005BA RID: 1466
	private static BetterList<int> mTouchIDs = new BetterList<int>();

	// Token: 0x040005BB RID: 1467
	private static int mWidth = 0;

	// Token: 0x040005BC RID: 1468
	private static int mHeight = 0;

	// Token: 0x040005BD RID: 1469
	private static GameObject mTooltip = null;

	// Token: 0x040005BE RID: 1470
	private Camera mCam;

	// Token: 0x040005BF RID: 1471
	private static float mTooltipTime = 0f;

	// Token: 0x040005C0 RID: 1472
	private float mNextRaycast;

	// Token: 0x040005C1 RID: 1473
	public static bool isDragging = false;

	// Token: 0x040005C2 RID: 1474
	private static int mLastInteractionCheck = -1;

	// Token: 0x040005C3 RID: 1475
	private static bool mLastInteractionResult = false;

	// Token: 0x040005C4 RID: 1476
	private static int mLastFocusCheck = -1;

	// Token: 0x040005C5 RID: 1477
	private static bool mLastFocusResult = false;

	// Token: 0x040005C6 RID: 1478
	private static int mLastOverCheck = -1;

	// Token: 0x040005C7 RID: 1479
	private static bool mLastOverResult = false;

	// Token: 0x040005C8 RID: 1480
	private static GameObject mRayHitObject;

	// Token: 0x040005C9 RID: 1481
	private static GameObject mHover;

	// Token: 0x040005CA RID: 1482
	private static GameObject mSelected;

	// Token: 0x040005CB RID: 1483
	private static UICamera.DepthEntry mHit = default(UICamera.DepthEntry);

	// Token: 0x040005CC RID: 1484
	private static BetterList<UICamera.DepthEntry> mHits = new BetterList<UICamera.DepthEntry>();

	// Token: 0x040005CD RID: 1485
	private static Plane m2DPlane = new Plane(Vector3.back, 0f);

	// Token: 0x040005CE RID: 1486
	private static float mNextEvent = 0f;

	// Token: 0x040005CF RID: 1487
	private static int mNotifying = 0;

	// Token: 0x040005D0 RID: 1488
	private static bool mUsingTouchEvents = true;

	// Token: 0x020000EF RID: 239
	[DoNotObfuscateNGUI]
	public enum ControlScheme
	{
		// Token: 0x040005DB RID: 1499
		Mouse,
		// Token: 0x040005DC RID: 1500
		Touch,
		// Token: 0x040005DD RID: 1501
		Controller
	}

	// Token: 0x020000F0 RID: 240
	[DoNotObfuscateNGUI]
	public enum ClickNotification
	{
		// Token: 0x040005DF RID: 1503
		None,
		// Token: 0x040005E0 RID: 1504
		Always,
		// Token: 0x040005E1 RID: 1505
		BasedOnDelta
	}

	// Token: 0x020000F1 RID: 241
	public class MouseOrTouch
	{
		// Token: 0x1700013E RID: 318
		// (get) Token: 0x06000862 RID: 2146 RVA: 0x0000A62D File Offset: 0x0000882D
		public float deltaTime
		{
			get
			{
				return RealTime.time - this.pressTime;
			}
		}

		// Token: 0x1700013F RID: 319
		// (get) Token: 0x06000863 RID: 2147 RVA: 0x0000A63B File Offset: 0x0000883B
		public bool isOverUI
		{
			get
			{
				return this.current != null && this.current != UICamera.fallThrough && NGUITools.FindInParents<UIRoot>(this.current) != null;
			}
		}

		// Token: 0x040005E2 RID: 1506
		public KeyCode key;

		// Token: 0x040005E3 RID: 1507
		public Vector2 pos;

		// Token: 0x040005E4 RID: 1508
		public Vector2 lastPos;

		// Token: 0x040005E5 RID: 1509
		public Vector2 delta;

		// Token: 0x040005E6 RID: 1510
		public Vector2 totalDelta;

		// Token: 0x040005E7 RID: 1511
		public Camera pressedCam;

		// Token: 0x040005E8 RID: 1512
		public GameObject last;

		// Token: 0x040005E9 RID: 1513
		public GameObject current;

		// Token: 0x040005EA RID: 1514
		public GameObject pressed;

		// Token: 0x040005EB RID: 1515
		public GameObject dragged;

		// Token: 0x040005EC RID: 1516
		public float pressTime;

		// Token: 0x040005ED RID: 1517
		public float clickTime;

		// Token: 0x040005EE RID: 1518
		public UICamera.ClickNotification clickNotification = UICamera.ClickNotification.Always;

		// Token: 0x040005EF RID: 1519
		public bool touchBegan = true;

		// Token: 0x040005F0 RID: 1520
		public bool pressStarted;

		// Token: 0x040005F1 RID: 1521
		public bool dragStarted;

		// Token: 0x040005F2 RID: 1522
		public int ignoreDelta;
	}

	// Token: 0x020000F2 RID: 242
	[DoNotObfuscateNGUI]
	public enum EventType
	{
		// Token: 0x040005F4 RID: 1524
		World_3D,
		// Token: 0x040005F5 RID: 1525
		UI_3D,
		// Token: 0x040005F6 RID: 1526
		World_2D,
		// Token: 0x040005F7 RID: 1527
		UI_2D
	}

	// Token: 0x020000F3 RID: 243
	[DoNotObfuscateNGUI]
	public enum ProcessEventsIn
	{
		// Token: 0x040005F9 RID: 1529
		Update,
		// Token: 0x040005FA RID: 1530
		LateUpdate
	}

	// Token: 0x020000F4 RID: 244
	private struct DepthEntry
	{
		// Token: 0x040005FB RID: 1531
		public int depth;

		// Token: 0x040005FC RID: 1532
		public RaycastHit hit;

		// Token: 0x040005FD RID: 1533
		public Vector3 point;

		// Token: 0x040005FE RID: 1534
		public GameObject go;
	}

	// Token: 0x020000F5 RID: 245
	public class Touch
	{
		// Token: 0x040005FF RID: 1535
		public int fingerId;

		// Token: 0x04000600 RID: 1536
		public TouchPhase phase;

		// Token: 0x04000601 RID: 1537
		public Vector2 position;

		// Token: 0x04000602 RID: 1538
		public int tapCount;
	}

	// Token: 0x020000F6 RID: 246
	// (Invoke) Token: 0x06000866 RID: 2150
	public delegate bool GetKeyStateFunc(KeyCode key);

	// Token: 0x020000F7 RID: 247
	// (Invoke) Token: 0x0600086A RID: 2154
	public delegate float GetAxisFunc(string name);

	// Token: 0x020000F8 RID: 248
	// (Invoke) Token: 0x0600086E RID: 2158
	public delegate bool GetAnyKeyFunc();

	// Token: 0x020000F9 RID: 249
	// (Invoke) Token: 0x06000872 RID: 2162
	public delegate UICamera.MouseOrTouch GetMouseDelegate(int button);

	// Token: 0x020000FA RID: 250
	// (Invoke) Token: 0x06000876 RID: 2166
	public delegate UICamera.MouseOrTouch GetTouchDelegate(int id, bool createIfMissing);

	// Token: 0x020000FB RID: 251
	// (Invoke) Token: 0x0600087A RID: 2170
	public delegate void RemoveTouchDelegate(int id);

	// Token: 0x020000FC RID: 252
	// (Invoke) Token: 0x0600087E RID: 2174
	public delegate void OnScreenResize();

	// Token: 0x020000FD RID: 253
	// (Invoke) Token: 0x06000882 RID: 2178
	public delegate void OnCustomInput();

	// Token: 0x020000FE RID: 254
	// (Invoke) Token: 0x06000886 RID: 2182
	public delegate void OnSchemeChange();

	// Token: 0x020000FF RID: 255
	// (Invoke) Token: 0x0600088A RID: 2186
	public delegate void MoveDelegate(Vector2 delta);

	// Token: 0x02000100 RID: 256
	// (Invoke) Token: 0x0600088E RID: 2190
	public delegate void VoidDelegate(GameObject go);

	// Token: 0x02000101 RID: 257
	// (Invoke) Token: 0x06000892 RID: 2194
	public delegate void BoolDelegate(GameObject go, bool state);

	// Token: 0x02000102 RID: 258
	// (Invoke) Token: 0x06000896 RID: 2198
	public delegate void FloatDelegate(GameObject go, float delta);

	// Token: 0x02000103 RID: 259
	// (Invoke) Token: 0x0600089A RID: 2202
	public delegate void VectorDelegate(GameObject go, Vector2 delta);

	// Token: 0x02000104 RID: 260
	// (Invoke) Token: 0x0600089E RID: 2206
	public delegate void ObjectDelegate(GameObject go, GameObject obj);

	// Token: 0x02000105 RID: 261
	// (Invoke) Token: 0x060008A2 RID: 2210
	public delegate void KeyCodeDelegate(GameObject go, KeyCode key);
}
