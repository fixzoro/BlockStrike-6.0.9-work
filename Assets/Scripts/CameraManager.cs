using System;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public CameraType cameraType;

    public Transform cameraTransform;

    public Transform cameraStaticPoint;

    public CameraDead dead;

    public CameraStatic statiс;

    public CameraSpectate spectate;

    public CameraFirstPerson firstPerson;

    private int playerID = -1;

    private static CameraManager instance;

    public static bool Team;

    public static bool ChangeType;

    public static event Action<int> selectPlayerEvent;

	public static int selectPlayer
	{
		get
		{
			return CameraManager.instance.playerID;
		}
		set
		{
			CameraType type = CameraManager.type;
			if (type != CameraType.Spectate)
			{
				if (type == CameraType.FirstPerson)
				{
					CameraManager.instance.firstPerson.UpdateSelectPlayer(value);
				}
			}
			else
			{
				CameraManager.instance.spectate.UpdateSelectPlayer(value);
			}
		}
	}

	public static CameraType type
	{
		get
		{
			return CameraManager.instance.cameraType;
		}
	}

	public static Transform ActiveCamera
	{
		get
		{
			if (CameraManager.type != CameraType.None)
			{
				return CameraManager.instance.cameraTransform;
			}
			if (PlayerInput.instance == null)
			{
				return null;
			}
			return PlayerInput.instance.FPCamera.Transform;
		}
	}

	public static CameraManager main
	{
		get
		{
			return CameraManager.instance;
		}
	}

	private void Awake()
	{
		CameraManager.instance = this;
	}

	private void OnDisable()
	{
		CameraManager.Team = false;
		CameraManager.ChangeType = false;
	}

	private void LateUpdate()
	{
		switch (this.cameraType)
		{
		case CameraType.Dead:
			this.dead.OnUpdate();
			break;
		case CameraType.Spectate:
			this.spectate.OnUpdate();
			break;
		case CameraType.FirstPerson:
			this.firstPerson.OnUpdate();
			break;
		}
	}

	public void OnSelectPlayer(int id)
	{
		if (CameraManager.selectPlayerEvent != null)
		{
			this.playerID = id;
			CameraManager.selectPlayerEvent(this.playerID);
		}
	}

	public static void SetType(CameraType type, params object[] parameters)
	{
		CameraManager.instance.DeactiveAll();
		CameraManager.instance.cameraType = type;
		switch (type)
		{
		case CameraType.Dead:
			CameraManager.instance.dead.Active(parameters);
			break;
		case CameraType.Static:
			CameraManager.instance.statiс.Active();
			break;
		case CameraType.Spectate:
			CameraManager.instance.spectate.Active(parameters);
			break;
		case CameraType.FirstPerson:
			CameraManager.instance.firstPerson.Active(parameters);
			break;
		}
	}

	private void DeactiveAll()
	{
		this.dead.Deactive();
		this.statiс.Deactive();
		this.spectate.Deactive();
		this.firstPerson.Deactive();
		this.cameraType = CameraType.None;
	}
}
