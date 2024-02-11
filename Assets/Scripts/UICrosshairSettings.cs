using System;
using UnityEngine;

public class UICrosshairSettings : MonoBehaviour
{
    public UISprite[] Crosshair;

    public UIToggle[] CrosshairToogle;

    public UISprite Point;

    public UIToggle DynamicsToogle;

    public UIToggle PointToogle;

    public UILabel SizeLabel;

    public UILabel ThicknessLabel;

    public UILabel GapLabel;

    public UILabel AlphaLabel;

    public UISlider SizeSlider;

    public UISlider ThicknessSlider;

    public UISlider GapSlider;

    public UISlider AlphaSlider;

    public UIColorPicker ColorPicker;

    private void Awake()
	{
		this.DynamicsToogle.value = (nPlayerPrefs.GetInt("CrosshairDynamics", 1) == 1);
		this.PointToogle.value = (nPlayerPrefs.GetInt("CrosshairPoint", 0) == 1);
		for (int i = 0; i < this.CrosshairToogle.Length; i++)
		{
			this.CrosshairToogle[i].value = (nPlayerPrefs.GetInt("CrosshairEnable_" + i, 1) == 1);
		}
		this.SizeSlider.value = nPlayerPrefs.GetFloat("CrosshairSize", 0.2f);
		this.ThicknessSlider.value = nPlayerPrefs.GetFloat("CrosshairThickness", 0.1f);
		this.GapSlider.value = nPlayerPrefs.GetFloat("CrosshairGap", 0f);
		this.AlphaSlider.value = nPlayerPrefs.GetFloat("CrosshairAlpha", 1f);
		string[] array = nPlayerPrefs.GetString("CrosshairColor", "1|1|1|1").Split(new char[]
		{
			"|"[0]
		});
		this.ColorPicker.value = new Color(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]), float.Parse(array[3]));
		this.UpdateAll();
	}

	public void SetSize()
	{
		int num = Mathf.FloorToInt(this.SizeSlider.value * 40f + 4f);
		this.SizeLabel.text = Localization.Get("Size", true) + ": " + num;
		this.Crosshair[0].width = num;
		this.Crosshair[1].width = num;
		this.Crosshair[2].height = num;
		this.Crosshair[3].height = num;
		nPlayerPrefs.SetFloat("CrosshairSize", this.SizeSlider.value);
	}

	public void SetThickness()
	{
		int num = Mathf.FloorToInt(this.ThicknessSlider.value * 20f + 2f);
		this.ThicknessLabel.text = Localization.Get("Thickness", true) + ": " + num;
		this.Crosshair[0].height = num;
		this.Crosshair[1].height = num;
		this.Crosshair[2].width = num;
		this.Crosshair[3].width = num;
		this.Point.width = num;
		this.Point.height = num;
		nPlayerPrefs.SetFloat("CrosshairThickness", this.ThicknessSlider.value);
	}

	public void SetGap()
	{
		int num = Mathf.FloorToInt(this.GapSlider.value * 20f) + 10;
		this.GapLabel.text = Localization.Get("Gap", true) + ": " + (num - 10);
		this.Crosshair[0].cachedTransform.localPosition = Vector3.left * (float)num;
		this.Crosshair[1].cachedTransform.localPosition = Vector3.right * (float)num;
		this.Crosshair[2].cachedTransform.localPosition = Vector3.up * (float)num;
		this.Crosshair[3].cachedTransform.localPosition = Vector3.down * (float)num;
		nPlayerPrefs.SetFloat("CrosshairGap", this.GapSlider.value);
	}

	public void SetAlpha()
	{
		float alpha = Mathf.Clamp(this.AlphaSlider.value, 0.01f, 1f);
		this.AlphaLabel.text = Localization.Get("Alpha", true) + ": " + alpha.ToString("f2");
		this.Crosshair[0].alpha = alpha;
		this.Crosshair[1].alpha = alpha;
		this.Crosshair[2].alpha = alpha;
		this.Crosshair[3].alpha = alpha;
		this.Point.alpha = alpha;
		nPlayerPrefs.SetFloat("CrosshairAlpha", this.AlphaSlider.value);
	}

	public void SetColor()
	{
		Color value = this.ColorPicker.value;
		this.Crosshair[0].color = value;
		this.Crosshair[1].color = value;
		this.Crosshair[2].color = value;
		this.Crosshair[3].color = value;
		this.Point.color = value;
		nPlayerPrefs.SetString("CrosshairColor", string.Concat(new object[]
		{
			value.r,
			"|",
			value.g,
			"|",
			value.b,
			"|",
			value.a
		}));
	}

	public void SetPoint()
	{
		this.Point.cachedGameObject.SetActive(this.PointToogle.value);
		nPlayerPrefs.SetInt("CrosshairPoint", (!this.PointToogle.value) ? 0 : 1);
	}

	public void SetDynamics()
	{
		nPlayerPrefs.SetInt("CrosshairDynamics", (!this.DynamicsToogle.value) ? 0 : 1);
	}

	public void SetCrosshair()
	{
		this.Crosshair[0].cachedGameObject.SetActive(this.CrosshairToogle[0].value);
		this.Crosshair[1].cachedGameObject.SetActive(this.CrosshairToogle[1].value);
		this.Crosshair[2].cachedGameObject.SetActive(this.CrosshairToogle[2].value);
		this.Crosshair[3].cachedGameObject.SetActive(this.CrosshairToogle[3].value);
		for (int i = 0; i < this.CrosshairToogle.Length; i++)
		{
			nPlayerPrefs.SetInt("CrosshairEnable_" + i, (!this.CrosshairToogle[i].value) ? 0 : 1);
		}
	}

	public void SetDefault()
	{
		this.DynamicsToogle.value = true;
		this.PointToogle.value = false;
		for (int i = 0; i < this.CrosshairToogle.Length; i++)
		{
			this.CrosshairToogle[i].value = true;
		}
		this.SizeSlider.value = 0.2f;
		this.ThicknessSlider.value = 0.1f;
		this.GapSlider.value = 0f;
		this.AlphaSlider.value = 1f;
		this.ColorPicker.Select(Color.white);
		nPlayerPrefs.SetInt("CrosshairDynamics", 1);
		nPlayerPrefs.SetInt("CrosshairPoint", 0);
		for (int j = 0; j < this.CrosshairToogle.Length; j++)
		{
			nPlayerPrefs.SetInt("CrosshairEnable_" + j, 1);
		}
		nPlayerPrefs.SetFloat("CrosshairSize", 0.2f);
		nPlayerPrefs.SetFloat("CrosshairThickness", 0.1f);
		nPlayerPrefs.SetFloat("CrosshairGap", 0f);
		nPlayerPrefs.SetFloat("CrosshairAlpha", 1f);
		nPlayerPrefs.SetString("CrosshairColor", "1|1|1|1");
		this.UpdateAll();
	}

	private void UpdateAll()
	{
		this.SetSize();
		this.SetThickness();
		this.SetGap();
		this.SetAlpha();
		this.SetColor();
		this.SetPoint();
		this.SetDynamics();
		this.SetCrosshair();
	}
}
