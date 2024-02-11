using System;
using UnityEngine;

public class CameraDead : MonoBehaviour
{
    public CameraManager cameraManager;

    public Rigidbody cameraRigidbody;

    public Transform cameraTransform;

    public BoxCollider cameraBoxCollider;

    private void Start()
	{
		this.cameraRigidbody.detectCollisions = false;
	}

	public void Active(object[] parameters)
	{
		this.Active((Vector3)parameters[0], (Vector3)parameters[1], (Vector3)parameters[2]);
	}

	public void Active(Vector3 position, Vector3 rotation, Vector3 force)
	{
		this.cameraTransform.gameObject.SetActive(true);
		this.cameraBoxCollider.isTrigger = false;
		this.cameraRigidbody.isKinematic = false;
		this.cameraRigidbody.detectCollisions = true;
		this.cameraTransform.position = position;
		this.cameraTransform.eulerAngles = rotation;
		this.cameraRigidbody.velocity = Vector3.zero;
		this.cameraRigidbody.AddForce(force);
		this.cameraRigidbody.AddRelativeForce(force);
		LODObject.Target = this.cameraTransform;
		SkyboxManager.GetCameraParent().localEulerAngles = Vector3.zero;
	}

	public void Deactive()
	{
		if (CameraManager.type != CameraType.Dead)
		{
			return;
		}
		this.cameraRigidbody.isKinematic = true;
		this.cameraRigidbody.detectCollisions = false;
		this.cameraBoxCollider.isTrigger = true;
		this.cameraTransform.gameObject.SetActive(false);
	}

	public void OnUpdate()
	{
		if (CameraManager.type != CameraType.Dead)
		{
			return;
		}
		SkyboxManager.GetCamera().rotation = this.cameraTransform.rotation;
	}
}
