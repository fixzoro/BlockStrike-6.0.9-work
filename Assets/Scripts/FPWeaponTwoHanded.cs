using System;
using UnityEngine;

public class FPWeaponTwoHanded : MonoBehaviour
{
    public float PositionSpringStiffness = 0.01f;

    public float PositionSpringDamping = 0.25f;

    public float PositionPivotSpringStiffness = 0.01f;

    public float PositionPivotSpringDamping = 0.25f;

    public float RotationPivotSpringStiffness = 0.01f;

    public float RotationPivotSpringDamping = 0.25f;

    public float RotationSpringStiffness = 0.01f;

    public float RotationSpringDamping = 0.25f;

    public float PositionSpring2Stiffness = 0.95f;

    public float PositionSpring2Damping = 0.25f;

    public float RotationSpring2Stiffness = 0.95f;

    public float RotationSpring2Damping = 0.25f;

    public Vector3 PositionOffset = new Vector3(0.15f, -0.15f, -0.15f);

    public Vector3 RotationOffset = Vector3.zero;

    public Vector3 PositionPivot = Vector3.zero;

    public Vector3 RotationPivot = Vector3.zero;

    public CryptoInt KnifeDelayForce = 50;

    public CryptoVector3 KnifeDelayForcePosition;

    public CryptoVector3 KnifeDelayForceRotation;

    public CryptoInt KnifeAttackForce = 50;

    public CryptoVector3 KnifeAttackForcePosition;

    public CryptoVector3 KnifeAttackForceRotation;

    private vp_Spring m_PositionSpring;

    private vp_Spring m_PositionSpring2;

    private vp_Spring m_PositionPivotSpring;

    private vp_Spring m_RotationPivotSpring;

    private vp_Spring m_RotationSpring;

    private vp_Spring m_RotationSpring2;

    private Transform m_Transform;

    private void Start()
	{
		this.m_Transform = base.transform;
		this.m_PositionSpring = new vp_Spring(this.m_Transform.parent, vp_Spring.UpdateMode.Position, true);
		this.m_PositionSpring.RestState = this.PositionOffset;
		this.m_PositionPivotSpring = new vp_Spring(this.m_Transform, vp_Spring.UpdateMode.Position, true);
		this.m_PositionPivotSpring.RestState = this.PositionPivot;
		this.m_PositionSpring2 = new vp_Spring(this.m_Transform, vp_Spring.UpdateMode.PositionAdditive, true);
		this.m_PositionSpring2.MinVelocity = 1E-05f;
		this.m_RotationSpring = new vp_Spring(this.m_Transform.parent, vp_Spring.UpdateMode.Rotation, true);
		this.m_RotationSpring.RestState = this.RotationOffset;
		this.m_RotationPivotSpring = new vp_Spring(this.m_Transform, vp_Spring.UpdateMode.Rotation, true);
		this.m_RotationPivotSpring.RestState = this.RotationPivot;
		this.m_RotationSpring2 = new vp_Spring(this.m_Transform.parent, vp_Spring.UpdateMode.RotationAdditive, true);
		this.m_RotationSpring2.MinVelocity = 1E-05f;
		this.SnapSprings();
		this.Refresh();
	}

	private void FixedUpdate()
	{
		this.UpdateSprings();
	}

	private void UpdateSprings()
	{
		this.m_PositionSpring.FixedUpdate();
		this.m_RotationSpring.FixedUpdate();
		this.m_PositionPivotSpring.FixedUpdate();
		this.m_RotationPivotSpring.FixedUpdate();
		this.m_PositionSpring2.FixedUpdate();
		this.m_RotationSpring2.FixedUpdate();
	}

	public virtual void AddForce2(Vector3 positional, Vector3 angular)
	{
		this.m_PositionSpring2.AddForce(positional);
		this.m_RotationSpring2.AddForce(angular);
	}

	public void AddSoftForce(Vector3 positional, Vector3 angular, int frames)
	{
		this.m_PositionSpring.AddSoftForce(positional, (float)frames);
		this.m_RotationSpring.AddSoftForce(angular, (float)frames);
	}

	public void AddSoftForceKnifeDelay()
	{
		this.m_PositionSpring.AddSoftForce(this.KnifeDelayForcePosition, (float)this.KnifeDelayForce);
		this.m_RotationSpring.AddSoftForce(this.KnifeDelayForceRotation, (float)this.KnifeDelayForce);
	}

	public void AddSoftForceKnifeAttack()
	{
		this.m_PositionSpring.AddSoftForce(this.KnifeAttackForcePosition, (float)this.KnifeAttackForce);
		this.m_RotationSpring.AddSoftForce(this.KnifeAttackForceRotation, (float)this.KnifeAttackForce);
	}

	[ContextMenu("Refresh")]
	public void Refresh()
	{
		if (this.m_PositionSpring != null)
		{
			this.m_PositionSpring.Stiffness = new Vector3(this.PositionSpringStiffness, this.PositionSpringStiffness, this.PositionSpringStiffness);
			this.m_PositionSpring.Damping = Vector3.one - new Vector3(this.PositionSpringDamping, this.PositionSpringDamping, this.PositionSpringDamping);
			this.m_PositionSpring.RestState = this.PositionOffset - this.PositionPivot;
		}
		if (this.m_PositionPivotSpring != null)
		{
			this.m_PositionPivotSpring.Stiffness = new Vector3(this.PositionPivotSpringStiffness, this.PositionPivotSpringStiffness, this.PositionPivotSpringStiffness);
			this.m_PositionPivotSpring.Damping = Vector3.one - new Vector3(this.PositionPivotSpringDamping, this.PositionPivotSpringDamping, this.PositionPivotSpringDamping);
			this.m_PositionPivotSpring.RestState = this.PositionPivot;
		}
		if (this.m_RotationPivotSpring != null)
		{
			this.m_RotationPivotSpring.Stiffness = new Vector3(this.RotationPivotSpringStiffness, this.RotationPivotSpringStiffness, this.RotationPivotSpringStiffness);
			this.m_RotationPivotSpring.Damping = Vector3.one - new Vector3(this.RotationPivotSpringDamping, this.RotationPivotSpringDamping, this.RotationPivotSpringDamping);
			this.m_RotationPivotSpring.RestState = this.RotationPivot;
		}
		if (this.m_PositionSpring2 != null)
		{
			this.m_PositionSpring2.Stiffness = new Vector3(this.PositionSpring2Stiffness, this.PositionSpring2Stiffness, this.PositionSpring2Stiffness);
			this.m_PositionSpring2.Damping = Vector3.one - new Vector3(this.PositionSpring2Damping, this.PositionSpring2Damping, this.PositionSpring2Damping);
			this.m_PositionSpring2.RestState = Vector3.zero;
		}
		if (this.m_RotationSpring != null)
		{
			this.m_RotationSpring.Stiffness = new Vector3(this.RotationSpringStiffness, this.RotationSpringStiffness, this.RotationSpringStiffness);
			this.m_RotationSpring.Damping = Vector3.one - new Vector3(this.RotationSpringDamping, this.RotationSpringDamping, this.RotationSpringDamping);
			this.m_RotationSpring.RestState = this.RotationOffset;
		}
		if (this.m_RotationSpring2 != null)
		{
			this.m_RotationSpring2.Stiffness = new Vector3(this.RotationSpring2Stiffness, this.RotationSpring2Stiffness, this.RotationSpring2Stiffness);
			this.m_RotationSpring2.Damping = Vector3.one - new Vector3(this.RotationSpring2Damping, this.RotationSpring2Damping, this.RotationSpring2Damping);
			this.m_RotationSpring2.RestState = Vector3.zero;
		}
	}

	public void SnapSprings()
	{
		if (this.m_PositionSpring != null)
		{
			this.m_PositionSpring.RestState = this.PositionOffset - this.PositionPivot;
			this.m_PositionSpring.State = this.PositionOffset - this.PositionPivot;
			this.m_PositionSpring.Stop(true);
		}
		if (this.m_PositionSpring2 != null)
		{
			this.m_PositionSpring2.RestState = Vector3.zero;
			this.m_PositionSpring2.State = Vector3.zero;
			this.m_PositionSpring2.Stop(true);
		}
		if (this.m_RotationSpring != null)
		{
			this.m_RotationSpring.RestState = this.RotationOffset;
			this.m_RotationSpring.State = this.RotationOffset;
			this.m_RotationSpring.Stop(true);
		}
		if (this.m_RotationSpring2 != null)
		{
			this.m_RotationSpring2.RestState = Vector3.zero;
			this.m_RotationSpring2.State = Vector3.zero;
			this.m_RotationSpring2.Stop(true);
		}
		if (this.m_PositionPivotSpring != null)
		{
			this.m_PositionPivotSpring.RestState = this.PositionPivot;
			this.m_PositionPivotSpring.State = this.PositionPivot;
			this.m_PositionPivotSpring.Stop(true);
		}
		this.m_Transform.localPosition = this.PositionPivot;
		if (this.m_RotationPivotSpring != null)
		{
			this.m_RotationPivotSpring.RestState = this.RotationPivot;
			this.m_RotationPivotSpring.State = this.RotationPivot;
			this.m_RotationPivotSpring.Stop(true);
		}
		this.m_Transform.localEulerAngles = this.RotationPivot;
	}

	public void StopSprings()
	{
		if (this.m_PositionSpring != null)
		{
			this.m_PositionSpring.Stop(true);
		}
		if (this.m_PositionPivotSpring != null)
		{
			this.m_PositionPivotSpring.Stop(true);
		}
		if (this.m_RotationSpring != null)
		{
			this.m_RotationSpring.Stop(true);
		}
		if (this.m_RotationPivotSpring != null)
		{
			this.m_RotationPivotSpring.Stop(true);
		}
		if (this.m_PositionSpring2 != null)
		{
			this.m_PositionSpring2.Stop(true);
		}
		if (this.m_RotationSpring2 != null)
		{
			this.m_RotationSpring2.Stop(true);
		}
	}

	public void Wield()
	{
		this.Refresh();
	}
}
