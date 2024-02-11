using System;
using UnityEngine;

public class UIFollowTarget : MonoBehaviour
{
    public Transform target;

    public Camera uiCamera;

    public UIWidget widget;

    public UISprite sprite;

    public UILabel label;

    private Camera gameCamera;

    private Transform gameCameraTransform;

    public float alphaDistance;

    public float alphaDistance2;

    public Vector3 customPos;

    public Rect minMaxPos = new Rect(0.1f, 0.9f, 0.1f, 0.9f);

    private Vector3 pos;

    private bool isActive;

    private static UIFollowTarget instance;

    private void Start()
	{
		UIFollowTarget.instance = this;
	}

	public static void SetTarget(Transform target, Camera cam, string text, Color color)
	{
		UIFollowTarget.instance.widget.cachedGameObject.SetActive(true);
		UIFollowTarget.instance.target = target;
		UIFollowTarget.instance.gameCamera = cam;
		UIFollowTarget.instance.gameCameraTransform = cam.transform;
		UIFollowTarget.instance.label.text = text;
		UIFollowTarget.instance.sprite.color = color;
		UIFollowTarget.instance.isActive = true;
	}

	public static void Deactive()
	{
		UIFollowTarget.instance.widget.cachedGameObject.SetActive(false);
		UIFollowTarget.instance.isActive = false;
	}

	private void Update()
	{
		if (this.isActive)
		{
			if (this.target == null)
			{
				this.isActive = false;
				return;
			}
			this.pos = this.gameCamera.WorldToViewportPoint(this.target.position);
			if (this.pos.z < 0f)
			{
				this.pos *= -1f;
			}
			this.pos.x = Mathf.Clamp(this.pos.x, this.minMaxPos.x, this.minMaxPos.y);
			this.pos.y = Mathf.Clamp(this.pos.y, this.minMaxPos.width, this.minMaxPos.height);
			this.widget.cachedTransform.position = this.uiCamera.ViewportToWorldPoint(this.pos);
			this.pos = this.widget.cachedTransform.localPosition;
			this.pos.x = (float)Mathf.FloorToInt(this.pos.x);
			this.pos.y = (float)Mathf.FloorToInt(this.pos.y);
			this.pos.z = 0f;
			this.widget.cachedTransform.localPosition = this.pos + this.customPos;
			this.sprite.alpha = Mathf.Clamp(Vector3.Distance(this.target.position, this.gameCameraTransform.position) * this.alphaDistance - this.alphaDistance2, 0f, 1f);
			this.label.alpha = this.sprite.alpha;
			this.widget.UpdateWidget();
			this.sprite.UpdateWidget();
			this.label.UpdateWidget();
		}
	}
}
