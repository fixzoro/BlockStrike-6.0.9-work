using System;
using UnityEngine;

public class PlayerSkinRagdoll : MonoBehaviour
{
    public static float force;

    public Rigidbody[] playerLimbs;

    public PlayerSkinAtlas playerAtlas;

    public Transform playerRightWeaponRoot;

    public Transform playerLeftWeaponRoot;

    [HideInInspector]
    public Vector3 defaultPosition;

    private Transform mTransform;

    private GameObject mGameObject;

    public Transform cachedTransform
	{
		get
		{
			if (this.mTransform == null)
			{
				this.mTransform = base.transform;
			}
			return this.mTransform;
		}
	}

	public GameObject cachedGameObject
	{
		get
		{
			if (this.mGameObject == null)
			{
				this.mGameObject = base.gameObject;
			}
			return this.mGameObject;
		}
	}

	public void SetSkin(UIAtlas atlas, string head, string body, string legs)
	{
		if (this.playerAtlas != null)
		{
			this.playerAtlas.atlas = atlas;
			this.playerAtlas.SetSprite("0-" + head, "1-" + body, "2-" + legs);
			return;
		}
	}

	public void Active(Vector3 f, Transform[] tr)
	{
		this.playerAtlas.skinnedMeshRenderer.enabled = true;
		for (int i = 0; i < tr.Length; i++)
		{
			this.playerLimbs[i].position = tr[i].position;
			this.playerLimbs[i].rotation = tr[i].rotation;
			this.playerLimbs[i].isKinematic = false;
			this.playerLimbs[i].velocity = Vector3.zero;
			this.playerLimbs[i].AddForce(f * PlayerSkinRagdoll.force);
            try
            {
                BoxCollider collider = GameObject.FindObjectOfType<PlayerTriggerDetector>().GetComponent<BoxCollider>();
                if (this.playerLimbs[i].gameObject.GetComponent<BoxCollider>() != null)
                {
                    Physics.IgnoreCollision(this.playerLimbs[i].gameObject.GetComponent<BoxCollider>(), PlayerInput.instance.mCharacterController);
                    Physics.IgnoreCollision(this.playerLimbs[i].gameObject.GetComponent<BoxCollider>(), collider);
                }
                if (this.playerLimbs[i].gameObject.GetComponent<CapsuleCollider>() != null)
                {
                    Physics.IgnoreCollision(this.playerLimbs[i].gameObject.GetComponent<CapsuleCollider>(), PlayerInput.instance.mCharacterController);
                    Physics.IgnoreCollision(this.playerLimbs[i].gameObject.GetComponent<CapsuleCollider>(), collider);
                }
                if (this.playerLimbs[i].gameObject.GetComponent<SphereCollider>() != null)
                {
                    Physics.IgnoreCollision(this.playerLimbs[i].gameObject.GetComponent<SphereCollider>(), PlayerInput.instance.mCharacterController);
                    Physics.IgnoreCollision(this.playerLimbs[i].gameObject.GetComponent<SphereCollider>(), collider);
                }
            }
            catch
            {

            }
            
        }
	}

	public void Deactive()
	{
		this.playerAtlas.skinnedMeshRenderer.enabled = false;
		for (int i = 0; i < this.playerLimbs.Length; i++)
		{
			this.playerLimbs[i].isKinematic = true;
		}
		this.playerLimbs[0].position = this.defaultPosition;
	}
}
