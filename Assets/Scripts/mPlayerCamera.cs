using DG.Tweening;
using UnityEngine;

public class mPlayerCamera : MonoBehaviour
{
    public GameObject Player;

    public MeshAtlas Head;

    public MeshAtlas[] Body;

    public MeshAtlas[] Legs;

    public Transform Point;

    public float RotateSpeed = 200f;

    private Camera mCamera;

    private static mPlayerCamera instance;

    private void Awake()
	{
		mPlayerCamera.instance = this;
		this.mCamera = base.GetComponent<Camera>();
		this.RotateSpeed = Mathf.Sqrt(this.RotateSpeed) / Mathf.Sqrt(Screen.dpi);
	}

	public static void Show()
	{
		mPlayerCamera.instance.mCamera.enabled = true;
		mPlayerCamera.instance.Player.SetActive(true);
	}

	public static void Close()
	{
		mPlayerCamera.instance.mCamera.enabled = false;
		mPlayerCamera.instance.Player.SetActive(false);
	}

	public static void Rotate(Vector2 rotate)
	{
		mPlayerCamera.instance.Point.Rotate(new Vector2(0f, -rotate.x * mPlayerCamera.instance.RotateSpeed));
	}

	public static void SetSkin(Team team, string head, string body, string leg)
	{
		UIAtlas atlas = (team != Team.Blue) ? GameSettings.instance.PlayerAtlasRed : GameSettings.instance.PlayerAtlasBlue;
		mPlayerCamera.instance.Head.atlas = atlas;
		mPlayerCamera.instance.Head.spriteName = "0-" + head;
		for (int i = 0; i < mPlayerCamera.instance.Body.Length; i++)
		{
			mPlayerCamera.instance.Body[i].atlas = atlas;
			mPlayerCamera.instance.Body[i].spriteName = "1-" + body;
		}
		for (int j = 0; j < mPlayerCamera.instance.Legs.Length; j++)
		{
			mPlayerCamera.instance.Legs[j].atlas = atlas;
			mPlayerCamera.instance.Legs[j].spriteName = "2-" + leg;
		}
	}

	public static void ResetRotateX()
	{
		mPlayerCamera.instance.Point.DOLocalRotate(Vector3.zero, 0.2f, RotateMode.Fast);
	}
}
