using System;
using UnityEngine;

public class GravitySystem : MonoBehaviour
{
    public CryptoFloat jumpForce = 0.15f;

    public CryptoFloat jumpForceDamping = 0.1f;

    public CryptoFloat gravity = 0.2f;

    public CryptoFloat gravityRagdoll = 9.8f;

    private void Start()
	{
		TimerManager.In(0.5f, delegate()
		{
			vp_FPController fpcontroller = GameManager.player.FPController;
			fpcontroller.PhysicsGravityModifier = (float)this.gravity;
			fpcontroller.MotorJumpForce = (float)this.jumpForce;
			fpcontroller.MotorJumpForceDamping = (float)this.jumpForceDamping;
		});
	}
}
