using System;
using UnityEngine;

public class WeaponCameraFlip : MonoBehaviour
{
    private bool flip;

    private Camera cam;

    private float fov;

    private static WeaponCameraFlip instance;

    private void Start()
	{
		WeaponCameraFlip.instance = this;
		this.cam = base.GetComponent<Camera>();
		this.fov = 60f;
		this.flip = GameConsole.Load<bool>("weapon_left_hand", false);
		if (this.flip)
		{
			this.cam.projectionMatrix = this.cam.projectionMatrix * Matrix4x4.Scale(new Vector3(-1f, 1f, 1f));
		}
	}

	private void Update()
	{
		if (this.cam.fieldOfView != this.fov && this.flip)
		{
			this.fov = this.cam.fieldOfView;
			this.cam.ResetProjectionMatrix();
			GL.invertCulling = false;
			this.cam.projectionMatrix = this.cam.projectionMatrix * Matrix4x4.Scale(new Vector3(-1f, 1f, 1f));
		}
	}

	[ContextMenu("Flip")]
	public static void OnFlip(bool value)
	{
		if (WeaponCameraFlip.instance == null)
		{
			return;
		}
		if (!WeaponCameraFlip.instance.gameObject.activeSelf)
		{
			return;
		}
		if (WeaponCameraFlip.instance.flip == value)
		{
			return;
		}
		WeaponCameraFlip.instance.flip = value;
		if (!WeaponCameraFlip.instance.flip)
		{
			WeaponCameraFlip.instance.cam.ResetProjectionMatrix();
			GL.invertCulling = false;
		}
		else
		{
			WeaponCameraFlip.instance.cam.projectionMatrix = WeaponCameraFlip.instance.cam.projectionMatrix * Matrix4x4.Scale(new Vector3(-1f, 1f, 1f));
		}
	}

	private void OnPreRender()
	{
		if (this.flip)
		{
			GL.invertCulling = true;
		}
	}

	private void OnPostRender()
	{
		if (this.flip)
		{
			GL.invertCulling = false;
		}
	}
}
