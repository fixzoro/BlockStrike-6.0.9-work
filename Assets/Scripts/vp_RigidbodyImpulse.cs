using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class vp_RigidbodyImpulse : MonoBehaviour
{
    protected Rigidbody m_Rigidbody;

    public Vector3 RigidbodyForce = new Vector3(0f, 5f, 0f);

    public float RigidbodySpin = 0.2f;

    protected virtual void Awake()
	{
		this.m_Rigidbody = base.GetComponent<Rigidbody>();
	}

	protected virtual void OnEnable()
	{
		if (this.m_Rigidbody == null)
		{
			return;
		}
		if (this.RigidbodyForce != Vector3.zero)
		{
			this.m_Rigidbody.AddForce(this.RigidbodyForce, ForceMode.Impulse);
		}
		if (this.RigidbodySpin != 0f)
		{
			this.m_Rigidbody.AddTorque(UnityEngine.Random.rotation.eulerAngles * this.RigidbodySpin);
		}
	}
}
