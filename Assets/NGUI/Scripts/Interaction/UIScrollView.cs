using System;
using UnityEngine;

// Token: 0x02000077 RID: 119
[RequireComponent(typeof(UIPanel))]
[AddComponentMenu("NGUI/Interaction/Scroll View")]
[ExecuteInEditMode]
public class UIScrollView : MonoBehaviour
{
	// Token: 0x1700003B RID: 59
	// (get) Token: 0x06000372 RID: 882 RVA: 0x0000754E File Offset: 0x0000574E
	public UIPanel panel
	{
		get
		{
			return this.mPanel;
		}
	}

	// Token: 0x1700003C RID: 60
	// (get) Token: 0x06000373 RID: 883 RVA: 0x00007556 File Offset: 0x00005756
	public bool isDragging
	{
		get
		{
			return this.mPressed && this.mDragStarted;
		}
	}

	// Token: 0x1700003D RID: 61
	// (get) Token: 0x06000374 RID: 884 RVA: 0x0000756C File Offset: 0x0000576C
	public virtual Bounds bounds
	{
		get
		{
			if (!this.mCalculatedBounds)
			{
				this.mCalculatedBounds = true;
				this.mTrans = base.transform;
				this.mBounds = NGUIMath.CalculateRelativeWidgetBounds(this.mTrans, this.mTrans);
			}
			return this.mBounds;
		}
	}

	// Token: 0x1700003E RID: 62
	// (get) Token: 0x06000375 RID: 885 RVA: 0x000075A9 File Offset: 0x000057A9
	public bool canMoveHorizontally
	{
		get
		{
			return this.movement == UIScrollView.Movement.Horizontal || this.movement == UIScrollView.Movement.Unrestricted || (this.movement == UIScrollView.Movement.Custom && this.customMovement.x != 0f);
		}
	}

	// Token: 0x1700003F RID: 63
	// (get) Token: 0x06000376 RID: 886 RVA: 0x0002D6A8 File Offset: 0x0002B8A8
	public bool canMoveVertically
	{
		get
		{
			return this.movement == UIScrollView.Movement.Vertical || this.movement == UIScrollView.Movement.Unrestricted || (this.movement == UIScrollView.Movement.Custom && this.customMovement.y != 0f);
		}
	}

	// Token: 0x17000040 RID: 64
	// (get) Token: 0x06000377 RID: 887 RVA: 0x0002D6F4 File Offset: 0x0002B8F4
	public virtual bool shouldMoveHorizontally
	{
		get
		{
			float num = this.bounds.size.x;
			if (this.mPanel.clipping == UIDrawCall.Clipping.SoftClip)
			{
				num += this.mPanel.clipSoftness.x * 2f;
			}
			return Mathf.RoundToInt(num - this.mPanel.width) > 0;
		}
	}

	// Token: 0x17000041 RID: 65
	// (get) Token: 0x06000378 RID: 888 RVA: 0x0002D75C File Offset: 0x0002B95C
	public virtual bool shouldMoveVertically
	{
		get
		{
			float num = this.bounds.size.y;
			if (this.mPanel.clipping == UIDrawCall.Clipping.SoftClip)
			{
				num += this.mPanel.clipSoftness.y * 2f;
			}
			return Mathf.RoundToInt(num - this.mPanel.height) > 0;
		}
	}

	// Token: 0x17000042 RID: 66
	// (get) Token: 0x06000379 RID: 889 RVA: 0x0002D7C4 File Offset: 0x0002B9C4
	protected virtual bool shouldMove
	{
		get
		{
			if (!this.disableDragIfFits)
			{
				return true;
			}
			if (this.mPanel == null)
			{
				this.mPanel = base.GetComponent<UIPanel>();
			}
			Vector4 finalClipRegion = this.mPanel.finalClipRegion;
			Bounds bounds = this.bounds;
			float num = (finalClipRegion.z != 0f) ? (finalClipRegion.z * 0.5f) : ((float)Screen.width);
			float num2 = (finalClipRegion.w != 0f) ? (finalClipRegion.w * 0.5f) : ((float)Screen.height);
			if (this.canMoveHorizontally)
			{
				if (bounds.min.x + 0.001f < finalClipRegion.x - num)
				{
					return true;
				}
				if (bounds.max.x - 0.001f > finalClipRegion.x + num)
				{
					return true;
				}
			}
			if (this.canMoveVertically)
			{
				if (bounds.min.y + 0.001f < finalClipRegion.y - num2)
				{
					return true;
				}
				if (bounds.max.y - 0.001f > finalClipRegion.y + num2)
				{
					return true;
				}
			}
			return false;
		}
	}

	// Token: 0x17000043 RID: 67
	// (get) Token: 0x0600037A RID: 890 RVA: 0x000075E9 File Offset: 0x000057E9
	// (set) Token: 0x0600037B RID: 891 RVA: 0x000075F1 File Offset: 0x000057F1
	public Vector3 currentMomentum
	{
		get
		{
			return this.mMomentum;
		}
		set
		{
			this.mMomentum = value;
			this.mShouldMove = true;
		}
	}

	// Token: 0x0600037C RID: 892 RVA: 0x0002D914 File Offset: 0x0002BB14
	private void Awake()
	{
		this.mTrans = base.transform;
		this.mPanel = base.GetComponent<UIPanel>();
		if (this.mPanel.clipping == UIDrawCall.Clipping.None)
		{
			this.mPanel.clipping = UIDrawCall.Clipping.ConstrainButDontClip;
		}
		if (this.movement != UIScrollView.Movement.Custom && this.scale.sqrMagnitude > 0.001f)
		{
			if (this.scale.x == 1f && this.scale.y == 0f)
			{
				this.movement = UIScrollView.Movement.Horizontal;
			}
			else if (this.scale.x == 0f && this.scale.y == 1f)
			{
				this.movement = UIScrollView.Movement.Vertical;
			}
			else if (this.scale.x == 1f && this.scale.y == 1f)
			{
				this.movement = UIScrollView.Movement.Unrestricted;
			}
			else
			{
				this.movement = UIScrollView.Movement.Custom;
				this.customMovement.x = this.scale.x;
				this.customMovement.y = this.scale.y;
			}
			this.scale = Vector3.zero;
		}
		if (this.contentPivot == UIWidget.Pivot.TopLeft && this.relativePositionOnReset != Vector2.zero)
		{
			this.contentPivot = NGUIMath.GetPivot(new Vector2(this.relativePositionOnReset.x, 1f - this.relativePositionOnReset.y));
			this.relativePositionOnReset = Vector2.zero;
		}
	}

	// Token: 0x0600037D RID: 893 RVA: 0x00007601 File Offset: 0x00005801
	private void OnEnable()
	{
		UIScrollView.list.Add(this);
		if (this.mStarted && Application.isPlaying)
		{
			this.CheckScrollbars();
		}
	}

	// Token: 0x0600037E RID: 894 RVA: 0x00007629 File Offset: 0x00005829
	private void Start()
	{
		this.mStarted = true;
		if (Application.isPlaying)
		{
			this.CheckScrollbars();
		}
	}

	// Token: 0x0600037F RID: 895 RVA: 0x0002DAB0 File Offset: 0x0002BCB0
	private void CheckScrollbars()
	{
		if (this.horizontalScrollBar != null)
		{
			EventDelegate.Add(this.horizontalScrollBar.onChange, new EventDelegate.Callback(this.OnScrollBar));
			this.horizontalScrollBar.BroadcastMessage("CacheDefaultColor", SendMessageOptions.DontRequireReceiver);
			this.horizontalScrollBar.alpha = ((this.showScrollBars != UIScrollView.ShowCondition.Always && !this.shouldMoveHorizontally) ? 0f : 1f);
		}
		if (this.verticalScrollBar != null)
		{
			EventDelegate.Add(this.verticalScrollBar.onChange, new EventDelegate.Callback(this.OnScrollBar));
			this.verticalScrollBar.BroadcastMessage("CacheDefaultColor", SendMessageOptions.DontRequireReceiver);
			this.verticalScrollBar.alpha = ((this.showScrollBars != UIScrollView.ShowCondition.Always && !this.shouldMoveVertically) ? 0f : 1f);
		}
	}

	// Token: 0x06000380 RID: 896 RVA: 0x00007642 File Offset: 0x00005842
	private void OnDisable()
	{
		UIScrollView.list.Remove(this);
		this.mPressed = false;
	}

	// Token: 0x06000381 RID: 897 RVA: 0x00007657 File Offset: 0x00005857
	public bool RestrictWithinBounds(bool instant)
	{
		return this.RestrictWithinBounds(instant, true, true);
	}

	// Token: 0x06000382 RID: 898 RVA: 0x0002DB9C File Offset: 0x0002BD9C
	public bool RestrictWithinBounds(bool instant, bool horizontal, bool vertical)
	{
		if (this.mPanel == null)
		{
			return false;
		}
		Bounds bounds = this.bounds;
		Vector3 vector = this.mPanel.CalculateConstrainOffset(bounds.min, bounds.max);
		if (!horizontal)
		{
			vector.x = 0f;
		}
		if (!vertical)
		{
			vector.y = 0f;
		}
		if (vector.sqrMagnitude > 0.1f)
		{
			if (!instant && this.dragEffect == UIScrollView.DragEffect.MomentumAndSpring)
			{
				Vector3 pos = this.mTrans.localPosition + vector;
				pos.x = Mathf.Round(pos.x);
				pos.y = Mathf.Round(pos.y);
				SpringPanel.Begin(this.mPanel.gameObject, pos, 8f);
			}
			else
			{
				this.MoveRelative(vector);
				if (Mathf.Abs(vector.x) > 0.01f)
				{
					this.mMomentum.x = 0f;
				}
				if (Mathf.Abs(vector.y) > 0.01f)
				{
					this.mMomentum.y = 0f;
				}
				if (Mathf.Abs(vector.z) > 0.01f)
				{
					this.mMomentum.z = 0f;
				}
				this.mScroll = 0f;
			}
			return true;
		}
		return false;
	}

	// Token: 0x06000383 RID: 899 RVA: 0x0002DD0C File Offset: 0x0002BF0C
	public void DisableSpring()
	{
		SpringPanel component = base.GetComponent<SpringPanel>();
		if (component != null)
		{
			component.enabled = false;
		}
	}

	// Token: 0x06000384 RID: 900 RVA: 0x00007662 File Offset: 0x00005862
	public void UpdateScrollbars()
	{
		this.UpdateScrollbars(true);
	}

	// Token: 0x06000385 RID: 901 RVA: 0x0002DD34 File Offset: 0x0002BF34
	public virtual void UpdateScrollbars(bool recalculateBounds)
	{
		if (this.mPanel == null)
		{
			return;
		}
		if (this.horizontalScrollBar != null || this.verticalScrollBar != null)
		{
			if (recalculateBounds)
			{
				this.mCalculatedBounds = false;
				this.mShouldMove = this.shouldMove;
			}
			Bounds bounds = this.bounds;
			Vector2 vector = bounds.min;
			Vector2 vector2 = bounds.max;
			if (this.horizontalScrollBar != null && vector2.x > vector.x)
			{
				Vector4 finalClipRegion = this.mPanel.finalClipRegion;
				int num = Mathf.RoundToInt(finalClipRegion.z);
				if ((num & 1) != 0)
				{
					num--;
				}
				float num2 = (float)num * 0.5f;
				num2 = Mathf.Round(num2);
				if (this.mPanel.clipping == UIDrawCall.Clipping.SoftClip)
				{
					num2 -= this.mPanel.clipSoftness.x;
				}
				float contentSize = vector2.x - vector.x;
				float viewSize = num2 * 2f;
				float num3 = vector.x;
				float num4 = vector2.x;
				float num5 = finalClipRegion.x - num2;
				float num6 = finalClipRegion.x + num2;
				num3 = num5 - num3;
				num4 -= num6;
				this.UpdateScrollbars(this.horizontalScrollBar, num3, num4, contentSize, viewSize, false);
			}
			if (this.verticalScrollBar != null && vector2.y > vector.y)
			{
				Vector4 finalClipRegion2 = this.mPanel.finalClipRegion;
				int num7 = Mathf.RoundToInt(finalClipRegion2.w);
				if ((num7 & 1) != 0)
				{
					num7--;
				}
				float num8 = (float)num7 * 0.5f;
				num8 = Mathf.Round(num8);
				if (this.mPanel.clipping == UIDrawCall.Clipping.SoftClip)
				{
					num8 -= this.mPanel.clipSoftness.y;
				}
				float contentSize2 = vector2.y - vector.y;
				float viewSize2 = num8 * 2f;
				float num9 = vector.y;
				float num10 = vector2.y;
				float num11 = finalClipRegion2.y - num8;
				float num12 = finalClipRegion2.y + num8;
				num9 = num11 - num9;
				num10 -= num12;
				this.UpdateScrollbars(this.verticalScrollBar, num9, num10, contentSize2, viewSize2, true);
			}
		}
		else if (recalculateBounds)
		{
			this.mCalculatedBounds = false;
		}
	}

	// Token: 0x06000386 RID: 902 RVA: 0x0002DFA8 File Offset: 0x0002C1A8
	protected void UpdateScrollbars(UIProgressBar slider, float contentMin, float contentMax, float contentSize, float viewSize, bool inverted)
	{
		if (slider == null)
		{
			return;
		}
		this.mIgnoreCallbacks = true;
		float num;
		if (viewSize < contentSize)
		{
			contentMin = Mathf.Clamp01(contentMin / contentSize);
			contentMax = Mathf.Clamp01(contentMax / contentSize);
			num = contentMin + contentMax;
			slider.value = ((!inverted) ? ((num <= 0.001f) ? 1f : (contentMin / num)) : ((num <= 0.001f) ? 0f : (1f - contentMin / num)));
		}
		else
		{
			contentMin = Mathf.Clamp01(-contentMin / contentSize);
			contentMax = Mathf.Clamp01(-contentMax / contentSize);
			num = contentMin + contentMax;
			slider.value = ((!inverted) ? ((num <= 0.001f) ? 1f : (contentMin / num)) : ((num <= 0.001f) ? 0f : (1f - contentMin / num)));
			if (contentSize > 0f)
			{
				contentMin = Mathf.Clamp01(contentMin / contentSize);
				contentMax = Mathf.Clamp01(contentMax / contentSize);
				num = contentMin + contentMax;
			}
		}
		UIScrollBar uiscrollBar = slider as UIScrollBar;
		if (uiscrollBar != null)
		{
			uiscrollBar.barSize = 1f - num;
		}
		this.mIgnoreCallbacks = false;
	}

	// Token: 0x06000387 RID: 903 RVA: 0x0002E0EC File Offset: 0x0002C2EC
	public virtual void SetDragAmount(float x, float y, bool updateScrollbars)
	{
		if (this.mPanel == null)
		{
			this.mPanel = base.GetComponent<UIPanel>();
		}
		this.DisableSpring();
		Bounds bounds = this.bounds;
		if (bounds.min.x == bounds.max.x || bounds.min.y == bounds.max.y)
		{
			return;
		}
		Vector4 finalClipRegion = this.mPanel.finalClipRegion;
		float num = finalClipRegion.z * 0.5f;
		float num2 = finalClipRegion.w * 0.5f;
		float num3 = bounds.min.x + num;
		float num4 = bounds.max.x - num;
		float num5 = bounds.min.y + num2;
		float num6 = bounds.max.y - num2;
		if (this.mPanel.clipping == UIDrawCall.Clipping.SoftClip)
		{
			num3 -= this.mPanel.clipSoftness.x;
			num4 += this.mPanel.clipSoftness.x;
			num5 -= this.mPanel.clipSoftness.y;
			num6 += this.mPanel.clipSoftness.y;
		}
		float num7 = Mathf.Lerp(num3, num4, x);
		float num8 = Mathf.Lerp(num6, num5, y);
		if (!updateScrollbars)
		{
			Vector3 localPosition = this.mTrans.localPosition;
			if (this.canMoveHorizontally)
			{
				localPosition.x += finalClipRegion.x - num7;
			}
			if (this.canMoveVertically)
			{
				localPosition.y += finalClipRegion.y - num8;
			}
			this.mTrans.localPosition = localPosition;
		}
		if (this.canMoveHorizontally)
		{
			finalClipRegion.x = num7;
		}
		if (this.canMoveVertically)
		{
			finalClipRegion.y = num8;
		}
		Vector4 baseClipRegion = this.mPanel.baseClipRegion;
		this.mPanel.clipOffset = new Vector2(finalClipRegion.x - baseClipRegion.x, finalClipRegion.y - baseClipRegion.y);
		if (updateScrollbars)
		{
			this.UpdateScrollbars(this.mDragID == -10);
		}
	}

	// Token: 0x06000388 RID: 904 RVA: 0x0000766B File Offset: 0x0000586B
	public void InvalidateBounds()
	{
		this.mCalculatedBounds = false;
	}

	// Token: 0x06000389 RID: 905 RVA: 0x0002E354 File Offset: 0x0002C554
	[ContextMenu("Reset Clipping Position")]
	public void ResetPosition()
	{
		if (NGUITools.GetActive(this))
		{
			this.mCalculatedBounds = false;
			Vector2 pivotOffset = NGUIMath.GetPivotOffset(this.contentPivot);
			this.SetDragAmount(pivotOffset.x, 1f - pivotOffset.y, false);
			this.SetDragAmount(pivotOffset.x, 1f - pivotOffset.y, true);
		}
	}

	// Token: 0x0600038A RID: 906 RVA: 0x0002E3B8 File Offset: 0x0002C5B8
	public void UpdatePosition()
	{
		if (!this.mIgnoreCallbacks && (this.horizontalScrollBar != null || this.verticalScrollBar != null))
		{
			this.mIgnoreCallbacks = true;
			this.mCalculatedBounds = false;
			Vector2 pivotOffset = NGUIMath.GetPivotOffset(this.contentPivot);
			float x = (!(this.horizontalScrollBar != null)) ? pivotOffset.x : this.horizontalScrollBar.value;
			float y = (!(this.verticalScrollBar != null)) ? (1f - pivotOffset.y) : this.verticalScrollBar.value;
			this.SetDragAmount(x, y, false);
			this.UpdateScrollbars(true);
			this.mIgnoreCallbacks = false;
		}
	}

	// Token: 0x0600038B RID: 907 RVA: 0x0002E47C File Offset: 0x0002C67C
	public void OnScrollBar()
	{
		if (!this.mIgnoreCallbacks)
		{
			this.mIgnoreCallbacks = true;
			float x = (!(this.horizontalScrollBar != null)) ? 0f : this.horizontalScrollBar.value;
			float y = (!(this.verticalScrollBar != null)) ? 0f : this.verticalScrollBar.value;
			this.SetDragAmount(x, y, false);
			this.mIgnoreCallbacks = false;
		}
	}

	// Token: 0x0600038C RID: 908 RVA: 0x0002E4FC File Offset: 0x0002C6FC
	public virtual void MoveRelative(Vector3 relative)
	{
		this.mTrans.localPosition += relative;
		Vector2 clipOffset = this.mPanel.clipOffset;
		clipOffset.x -= relative.x;
		clipOffset.y -= relative.y;
		this.mPanel.clipOffset = clipOffset;
		this.UpdateScrollbars(false);
	}

	// Token: 0x0600038D RID: 909 RVA: 0x0002E56C File Offset: 0x0002C76C
	public void MoveAbsolute(Vector3 absolute)
	{
		Vector3 a = this.mTrans.InverseTransformPoint(absolute);
		Vector3 b = this.mTrans.InverseTransformPoint(Vector3.zero);
		this.MoveRelative(a - b);
	}

	// Token: 0x0600038E RID: 910 RVA: 0x0002E5A4 File Offset: 0x0002C7A4
	public void Press(bool pressed)
	{
		if (this.mPressed == pressed || UICamera.currentScheme == UICamera.ControlScheme.Controller)
		{
			return;
		}
		if (this.smoothDragStart && pressed)
		{
			this.mDragStarted = false;
			this.mDragStartOffset = Vector2.zero;
		}
		if (base.enabled && NGUITools.GetActive(base.gameObject))
		{
			if (!pressed && this.mDragID == UICamera.currentTouchID)
			{
				this.mDragID = -10;
			}
			this.mCalculatedBounds = false;
			this.mShouldMove = this.shouldMove;
			if (!this.mShouldMove)
			{
				return;
			}
			this.mPressed = pressed;
			if (pressed)
			{
				this.mMomentum = Vector3.zero;
				this.mScroll = 0f;
				this.DisableSpring();
				this.mLastPos = UICamera.lastWorldPosition;
				this.mPlane = new Plane(this.mTrans.rotation * Vector3.back, this.mLastPos);
				Vector2 clipOffset = this.mPanel.clipOffset;
				clipOffset.x = Mathf.Round(clipOffset.x);
				clipOffset.y = Mathf.Round(clipOffset.y);
				this.mPanel.clipOffset = clipOffset;
				Vector3 localPosition = this.mTrans.localPosition;
				localPosition.x = Mathf.Round(localPosition.x);
				localPosition.y = Mathf.Round(localPosition.y);
				this.mTrans.localPosition = localPosition;
				if (!this.smoothDragStart)
				{
					this.mDragStarted = true;
					this.mDragStartOffset = Vector2.zero;
					if (this.onDragStarted != null)
					{
						this.onDragStarted();
					}
				}
			}
			else if (this.centerOnChild)
			{
				if (this.mDragStarted && this.onDragFinished != null)
				{
					this.onDragFinished();
				}
				this.centerOnChild.Recenter();
			}
			else
			{
				if (this.mDragStarted && this.restrictWithinPanel && this.mPanel.clipping != UIDrawCall.Clipping.None)
				{
					this.RestrictWithinBounds(this.dragEffect == UIScrollView.DragEffect.None, this.canMoveHorizontally, this.canMoveVertically);
				}
				if (this.mDragStarted && this.onDragFinished != null)
				{
					this.onDragFinished();
				}
				if (!this.mShouldMove && this.onStoppedMoving != null)
				{
					this.onStoppedMoving();
				}
			}
		}
	}

	// Token: 0x0600038F RID: 911 RVA: 0x0002E818 File Offset: 0x0002CA18
	public void Drag()
	{
		if (!this.mPressed || UICamera.currentScheme == UICamera.ControlScheme.Controller)
		{
			return;
		}
		if (base.enabled && NGUITools.GetActive(base.gameObject) && this.mShouldMove)
		{
			if (this.mDragID == -10)
			{
				this.mDragID = UICamera.currentTouchID;
			}
			UICamera.currentTouch.clickNotification = UICamera.ClickNotification.BasedOnDelta;
			if (this.smoothDragStart && !this.mDragStarted)
			{
				this.mDragStarted = true;
				this.mDragStartOffset = UICamera.currentTouch.totalDelta;
				if (this.onDragStarted != null)
				{
					this.onDragStarted();
				}
			}
			Ray ray = (!this.smoothDragStart) ? UICamera.currentCamera.ScreenPointToRay(UICamera.currentTouch.pos) : UICamera.currentCamera.ScreenPointToRay(UICamera.currentTouch.pos - this.mDragStartOffset);
			float distance = 0f;
			if (this.mPlane.Raycast(ray, out distance))
			{
				Vector3 point = ray.GetPoint(distance);
				Vector3 vector = point - this.mLastPos;
				this.mLastPos = point;
				if (vector.x != 0f || vector.y != 0f || vector.z != 0f)
				{
					vector = this.mTrans.InverseTransformDirection(vector);
					if (this.movement == UIScrollView.Movement.Horizontal)
					{
						vector.y = 0f;
						vector.z = 0f;
					}
					else if (this.movement == UIScrollView.Movement.Vertical)
					{
						vector.x = 0f;
						vector.z = 0f;
					}
					else if (this.movement == UIScrollView.Movement.Unrestricted)
					{
						vector.z = 0f;
					}
					else
					{
						vector.Scale(this.customMovement);
					}
					vector = this.mTrans.TransformDirection(vector);
				}
				if (this.dragEffect == UIScrollView.DragEffect.None)
				{
					this.mMomentum = Vector3.zero;
				}
				else
				{
					this.mMomentum = Vector3.Lerp(this.mMomentum, this.mMomentum + vector * (0.01f * this.momentumAmount), 0.67f);
				}
				if (!this.iOSDragEmulation || this.dragEffect != UIScrollView.DragEffect.MomentumAndSpring)
				{
					this.MoveAbsolute(vector);
				}
				else
				{
					Vector3 vector2 = this.mPanel.CalculateConstrainOffset(this.bounds.min, this.bounds.max);
					if (this.movement == UIScrollView.Movement.Horizontal)
					{
						vector2.y = 0f;
					}
					else if (this.movement == UIScrollView.Movement.Vertical)
					{
						vector2.x = 0f;
					}
					else if (this.movement == UIScrollView.Movement.Custom)
					{
						vector2.x *= this.customMovement.x;
						vector2.y *= this.customMovement.y;
					}
					if (vector2.magnitude > 1f)
					{
						this.MoveAbsolute(vector * 0.5f);
						this.mMomentum *= 0.5f;
					}
					else
					{
						this.MoveAbsolute(vector);
					}
				}
				if (this.constrainOnDrag && this.restrictWithinPanel && this.mPanel.clipping != UIDrawCall.Clipping.None && this.dragEffect != UIScrollView.DragEffect.MomentumAndSpring)
				{
					this.RestrictWithinBounds(true, this.canMoveHorizontally, this.canMoveVertically);
				}
			}
		}
	}

	// Token: 0x06000390 RID: 912 RVA: 0x0002EBC0 File Offset: 0x0002CDC0
	public void Scroll(float delta)
	{
		if (base.enabled && NGUITools.GetActive(base.gameObject) && this.scrollWheelFactor != 0f)
		{
			this.DisableSpring();
			this.mShouldMove |= this.shouldMove;
			if (Mathf.Sign(this.mScroll) != Mathf.Sign(delta))
			{
				this.mScroll = 0f;
			}
			this.mScroll += delta * this.scrollWheelFactor;
		}
	}

	// Token: 0x06000391 RID: 913 RVA: 0x0002EC48 File Offset: 0x0002CE48
	private void LateUpdate()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		float deltaTime = RealTime.deltaTime;
		if (this.showScrollBars != UIScrollView.ShowCondition.Always && (this.verticalScrollBar || this.horizontalScrollBar))
		{
			bool flag = false;
			bool flag2 = false;
			if (this.showScrollBars != UIScrollView.ShowCondition.WhenDragging || this.mDragID != -10 || this.mMomentum.magnitude > 0.01f)
			{
				flag = this.shouldMoveVertically;
				flag2 = this.shouldMoveHorizontally;
			}
			if (this.verticalScrollBar)
			{
				float num = this.verticalScrollBar.alpha;
				num += ((!flag) ? (-deltaTime * 3f) : (deltaTime * 6f));
				num = Mathf.Clamp01(num);
				if (this.verticalScrollBar.alpha != num)
				{
					this.verticalScrollBar.alpha = num;
				}
			}
			if (this.horizontalScrollBar)
			{
				float num2 = this.horizontalScrollBar.alpha;
				num2 += ((!flag2) ? (-deltaTime * 3f) : (deltaTime * 6f));
				num2 = Mathf.Clamp01(num2);
				if (this.horizontalScrollBar.alpha != num2)
				{
					this.horizontalScrollBar.alpha = num2;
				}
			}
		}
		if (!this.mShouldMove)
		{
			return;
		}
		if (!this.mPressed)
		{
			if (this.mMomentum.magnitude > 0.0001f || Mathf.Abs(this.mScroll) > 0.0001f)
			{
				if (this.movement == UIScrollView.Movement.Horizontal)
				{
					this.mMomentum -= this.mTrans.TransformDirection(new Vector3(this.mScroll * 0.05f, 0f, 0f));
				}
				else if (this.movement == UIScrollView.Movement.Vertical)
				{
					this.mMomentum -= this.mTrans.TransformDirection(new Vector3(0f, this.mScroll * 0.05f, 0f));
				}
				else if (this.movement == UIScrollView.Movement.Unrestricted)
				{
					this.mMomentum -= this.mTrans.TransformDirection(new Vector3(this.mScroll * 0.05f, this.mScroll * 0.05f, 0f));
				}
				else
				{
					this.mMomentum -= this.mTrans.TransformDirection(new Vector3(this.mScroll * this.customMovement.x * 0.05f, this.mScroll * this.customMovement.y * 0.05f, 0f));
				}
				this.mScroll = NGUIMath.SpringLerp(this.mScroll, 0f, 20f, deltaTime);
				Vector3 absolute = NGUIMath.SpringDampen(ref this.mMomentum, this.dampenStrength, deltaTime);
				this.MoveAbsolute(absolute);
				if (this.restrictWithinPanel && this.mPanel.clipping != UIDrawCall.Clipping.None)
				{
					if (NGUITools.GetActive(this.centerOnChild))
					{
						if (this.centerOnChild.nextPageThreshold != 0f)
						{
							this.mMomentum = Vector3.zero;
							this.mScroll = 0f;
						}
						else
						{
							this.centerOnChild.Recenter();
						}
					}
					else
					{
						this.RestrictWithinBounds(false, this.canMoveHorizontally, this.canMoveVertically);
					}
				}
				if (this.onMomentumMove != null)
				{
					this.onMomentumMove();
				}
			}
			else
			{
				this.mScroll = 0f;
				this.mMomentum = Vector3.zero;
				SpringPanel component = base.GetComponent<SpringPanel>();
				if (component != null && component.enabled)
				{
					return;
				}
				this.mShouldMove = false;
				if (this.onStoppedMoving != null)
				{
					this.onStoppedMoving();
				}
			}
		}
		else
		{
			this.mScroll = 0f;
			NGUIMath.SpringDampen(ref this.mMomentum, 9f, deltaTime);
		}
	}

	// Token: 0x06000392 RID: 914 RVA: 0x0002F050 File Offset: 0x0002D250
	public void OnPan(Vector2 delta)
	{
		if (this.horizontalScrollBar != null)
		{
			this.horizontalScrollBar.OnPan(delta);
		}
		if (this.verticalScrollBar != null)
		{
			this.verticalScrollBar.OnPan(delta);
		}
		if (this.horizontalScrollBar == null && this.verticalScrollBar == null)
		{
			if (this.canMoveHorizontally)
			{
				this.Scroll(delta.x);
			}
			else if (this.canMoveVertically)
			{
				this.Scroll(delta.y);
			}
		}
	}

	// Token: 0x0400027F RID: 639
	public static BetterList<UIScrollView> list = new BetterList<UIScrollView>();

	// Token: 0x04000280 RID: 640
	public UIScrollView.Movement movement;

	// Token: 0x04000281 RID: 641
	public UIScrollView.DragEffect dragEffect = UIScrollView.DragEffect.MomentumAndSpring;

	// Token: 0x04000282 RID: 642
	public bool restrictWithinPanel = true;

	// Token: 0x04000283 RID: 643
	[Tooltip("Whether the scroll view will execute its constrain within bounds logic on every drag operation")]
	public bool constrainOnDrag;

	// Token: 0x04000284 RID: 644
	public bool disableDragIfFits;

	// Token: 0x04000285 RID: 645
	public bool smoothDragStart = true;

	// Token: 0x04000286 RID: 646
	public bool iOSDragEmulation = true;

	// Token: 0x04000287 RID: 647
	public float scrollWheelFactor = 0.25f;

	// Token: 0x04000288 RID: 648
	public float momentumAmount = 35f;

	// Token: 0x04000289 RID: 649
	public float dampenStrength = 9f;

	// Token: 0x0400028A RID: 650
	public UIProgressBar horizontalScrollBar;

	// Token: 0x0400028B RID: 651
	public UIProgressBar verticalScrollBar;

	// Token: 0x0400028C RID: 652
	public UIScrollView.ShowCondition showScrollBars = UIScrollView.ShowCondition.OnlyIfNeeded;

	// Token: 0x0400028D RID: 653
	public Vector2 customMovement = new Vector2(1f, 0f);

	// Token: 0x0400028E RID: 654
	public UIWidget.Pivot contentPivot;

	// Token: 0x0400028F RID: 655
	public UIScrollView.OnDragNotification onDragStarted;

	// Token: 0x04000290 RID: 656
	public UIScrollView.OnDragNotification onDragFinished;

	// Token: 0x04000291 RID: 657
	public UIScrollView.OnDragNotification onMomentumMove;

	// Token: 0x04000292 RID: 658
	public UIScrollView.OnDragNotification onStoppedMoving;

	// Token: 0x04000293 RID: 659
	[SerializeField]
	[HideInInspector]
	private Vector3 scale = new Vector3(1f, 0f, 0f);

	// Token: 0x04000294 RID: 660
	[HideInInspector]
	[SerializeField]
	private Vector2 relativePositionOnReset = Vector2.zero;

	// Token: 0x04000295 RID: 661
	protected Transform mTrans;

	// Token: 0x04000296 RID: 662
	protected UIPanel mPanel;

	// Token: 0x04000297 RID: 663
	protected Plane mPlane;

	// Token: 0x04000298 RID: 664
	protected Vector3 mLastPos;

	// Token: 0x04000299 RID: 665
	protected bool mPressed;

	// Token: 0x0400029A RID: 666
	protected Vector3 mMomentum = Vector3.zero;

	// Token: 0x0400029B RID: 667
	protected float mScroll;

	// Token: 0x0400029C RID: 668
	protected Bounds mBounds;

	// Token: 0x0400029D RID: 669
	protected bool mCalculatedBounds;

	// Token: 0x0400029E RID: 670
	protected bool mShouldMove;

	// Token: 0x0400029F RID: 671
	protected bool mIgnoreCallbacks;

	// Token: 0x040002A0 RID: 672
	protected int mDragID = -10;

	// Token: 0x040002A1 RID: 673
	protected Vector2 mDragStartOffset = Vector2.zero;

	// Token: 0x040002A2 RID: 674
	protected bool mDragStarted;

	// Token: 0x040002A3 RID: 675
	[NonSerialized]
	private bool mStarted;

	// Token: 0x040002A4 RID: 676
	[HideInInspector]
	public UICenterOnChild centerOnChild;

	// Token: 0x02000078 RID: 120
	[DoNotObfuscateNGUI]
	public enum Movement
	{
		// Token: 0x040002A6 RID: 678
		Horizontal,
		// Token: 0x040002A7 RID: 679
		Vertical,
		// Token: 0x040002A8 RID: 680
		Unrestricted,
		// Token: 0x040002A9 RID: 681
		Custom
	}

	// Token: 0x02000079 RID: 121
	[DoNotObfuscateNGUI]
	public enum DragEffect
	{
		// Token: 0x040002AB RID: 683
		None,
		// Token: 0x040002AC RID: 684
		Momentum,
		// Token: 0x040002AD RID: 685
		MomentumAndSpring
	}

	// Token: 0x0200007A RID: 122
	[DoNotObfuscateNGUI]
	public enum ShowCondition
	{
		// Token: 0x040002AF RID: 687
		Always,
		// Token: 0x040002B0 RID: 688
		OnlyIfNeeded,
		// Token: 0x040002B1 RID: 689
		WhenDragging
	}

	// Token: 0x0200007B RID: 123
	// (Invoke) Token: 0x06000394 RID: 916
	public delegate void OnDragNotification();
}
