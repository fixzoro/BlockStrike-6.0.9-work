using System;
using UnityEngine;

public class CameraStatic : MonoBehaviour
{
    public CameraManager cameraManager;

    private Transform cameraTransform;

    public void Active()
	{
		this.cameraTransform = this.cameraManager.cameraTransform;
		this.cameraTransform.gameObject.SetActive(true);
		this.cameraTransform.position = this.cameraManager.cameraStaticPoint.position;
		this.cameraTransform.rotation = this.cameraManager.cameraStaticPoint.rotation;
		LODObject.Target = this.cameraTransform;
		SkyboxManager.GetCameraParent().localEulerAngles = Vector3.zero;
		SkyboxManager.GetCamera().rotation = this.cameraTransform.rotation;
	}

	public void Deactive()
	{
		if (CameraManager.type != CameraType.Static)
		{
			return;
		}
		this.cameraTransform = this.cameraManager.cameraTransform;
		this.cameraTransform.gameObject.SetActive(false);
	}
}
