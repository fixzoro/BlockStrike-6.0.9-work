using System;
using UnityEngine;

[ExecuteInEditMode]
public class SkyboxManager : MonoBehaviour
{
    [Range(0f, 1f)]
    public float TimeDay;

    public bool Moon;

    public bool Stars;

    public bool Sun;

    public bool Clouds;

    public Material SkyboxMaterial;

    public Transform SkyboxCamera;

    public Transform SkyboxCameraParent;

    public GameObject MoonObject;

    public GameObject StarsObject;

    public GameObject SunObject;

    public GameObject CloudsObject;

    private static SkyboxManager instance;

    private void Awake()
	{
		SkyboxManager.instance = this;
	}

	private void Start()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		this.SkyboxMaterial.mainTextureOffset = new Vector2(this.TimeDay, 0f);
		this.MoonObject.SetActive(this.Moon);
		this.StarsObject.SetActive(this.Stars);
		this.SunObject.SetActive(this.Sun);
		this.CloudsObject.SetActive(this.Clouds);
		EventManager.AddListener("OnSettings", new EventManager.Callback(this.OnSettings));
		this.OnSettings();
	}

	private void Reset()
	{
		this.SkyboxMaterial.mainTextureOffset = new Vector2(this.TimeDay, 0f);
	}

	public static Transform GetCamera()
	{
		return SkyboxManager.instance.SkyboxCamera;
	}

	public static Transform GetCameraParent()
	{
		return SkyboxManager.instance.SkyboxCameraParent;
	}

	private void OnSettings()
	{
		if (Settings.Clouds)
		{
			if (this.Clouds)
			{
				this.CloudsObject.SetActive(true);
				return;
			}
		}
		else
		{
			this.CloudsObject.SetActive(false);
		}
	}
}
