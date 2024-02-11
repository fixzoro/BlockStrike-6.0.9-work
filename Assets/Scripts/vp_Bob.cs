using System;
using UnityEngine;

public class vp_Bob : MonoBehaviour
{
    public Vector3 BobAmp = new Vector3(0f, 0.1f, 0f);

    public Vector3 BobRate = new Vector3(0f, 4f, 0f);

    public float BobOffset;

    public float GroundOffset;

    public bool RandomizeBobOffset;

    public bool LocalMotion;

    protected Transform m_Transform;

    protected Vector3 m_InitialPosition;

    protected Vector3 m_Offset;

    protected virtual void Awake()
	{
		this.m_Transform = base.transform;
		this.m_InitialPosition = this.m_Transform.position;
	}

	protected virtual void OnEnable()
	{
		this.m_Transform.position = this.m_InitialPosition;
		if (this.RandomizeBobOffset)
		{
			this.BobOffset = UnityEngine.Random.value;
		}
	}

	protected virtual void Update()
	{
		if (this.BobRate.x != 0f && this.BobAmp.x != 0f)
		{
			this.m_Offset.x = vp_MathUtility.Sinus(this.BobRate.x, this.BobAmp.x, this.BobOffset);
		}
		if (this.BobRate.y != 0f && this.BobAmp.y != 0f)
		{
			this.m_Offset.y = vp_MathUtility.Sinus(this.BobRate.y, this.BobAmp.y, this.BobOffset);
		}
		if (this.BobRate.z != 0f && this.BobAmp.z != 0f)
		{
			this.m_Offset.z = vp_MathUtility.Sinus(this.BobRate.z, this.BobAmp.z, this.BobOffset);
		}
		if (!this.LocalMotion)
		{
			this.m_Transform.position = this.m_InitialPosition + this.m_Offset + Vector3.up * this.GroundOffset;
		}
		else
		{
			this.m_Transform.position = this.m_InitialPosition + Vector3.up * this.GroundOffset;
			this.m_Transform.localPosition += this.m_Transform.TransformDirection(this.m_Offset);
		}
	}
}
