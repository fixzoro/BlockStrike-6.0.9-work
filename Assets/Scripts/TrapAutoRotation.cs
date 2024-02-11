using UnityEngine;

public class TrapAutoRotation : MonoBehaviour
{
    public Vector3 rotate;

    private Transform mTransform;

    private void Start()
	{
		this.mTransform = base.transform;
	}

	private void Update()
	{
		this.mTransform.Rotate(this.rotate);
	}
}
