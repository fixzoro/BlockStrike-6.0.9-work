using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class UICrosshair : MonoBehaviour
{
    public GameObject Crosshair;

    public UISprite LeftSprite;

    public UISprite RightSprite;

    public UISprite TopSprite;

    public UISprite BottomSprite;

    public UISprite PointSprite;

    public CryptoFloat MaxAccuracy;

    public CryptoFloat Accuracy;

    private Vector2 FireAccuracy;

    public CryptoInt AccuracyWidth = 1600;

    public CryptoInt AccuracyHeight = 960;

    private Tweener Tween;

    [Header("Hit Settings")]
    public UISprite HitLeftSprite;

    public UISprite HitRightSprite;

    public UISprite HitTopSprite;

    public UISprite HitBottomSprite;

    public float HitDuration;

    private Tweener HitTween;

    private float HitAlpha;

    private bool HitMarker = true;

    [Header("Scope")]
    public GameObject RifleScope;

    private bool isDynamic = true;

    public int Gap;

    private bool isActive;

    private static UICrosshair instance;

    private void Awake()
	{
		UICrosshair.instance = this;
	}

	private void Start()
	{
		EventManager.AddListener("OnSettings", new EventManager.Callback(this.UpdateSettings));
		this.UpdateSettings();
		this.Tween = DOTween.To(() => this.Accuracy, delegate(float x)
		{
			this.Accuracy = x;
		}, (float)nValue.int5, nValue.float12).SetAutoKill(false).SetEase(Ease.OutQuart);
		this.Tween.OnUpdate(delegate
		{
			if (this.isDynamic)
			{
				this.LeftSprite.cachedTransform.localPosition = Vector3.left * (this.Accuracy + (float)this.Gap);
				this.RightSprite.cachedTransform.localPosition = Vector3.right * (this.Accuracy + (float)this.Gap);
				this.TopSprite.cachedTransform.localPosition = Vector3.up * (this.Accuracy + (float)this.Gap);
				this.BottomSprite.cachedTransform.localPosition = Vector3.down * (this.Accuracy + (float)this.Gap);
				this.LeftSprite.UpdateWidget();
				this.RightSprite.UpdateWidget();
				this.TopSprite.UpdateWidget();
				this.BottomSprite.UpdateWidget();
			}
            else
            {
                this.LeftSprite.UpdateWidget();
                this.RightSprite.UpdateWidget();
                this.TopSprite.UpdateWidget();
                this.BottomSprite.UpdateWidget();
            }
		});
		this.HitTween = DOTween.To(() => this.HitAlpha, delegate(float x)
		{
			this.HitAlpha = x;
		}, (float)nValue.int0, this.HitDuration).SetAutoKill(false);
		this.HitTween.OnUpdate(delegate
		{
			if (this.HitMarker)
			{
				this.HitLeftSprite.alpha = this.HitAlpha;
				this.HitRightSprite.alpha = this.HitAlpha;
				this.HitTopSprite.alpha = this.HitAlpha;
				this.HitBottomSprite.alpha = this.HitAlpha;
			}
		});
	}

	private void OnEnable()
	{
		this.isActive = true;
	}

	private void OnDisable()
	{
		this.isActive = false;
	}

	public static void SetAccuracy(float accuracy)
	{
		if (UICrosshair.instance == null || !UICrosshair.instance.isActive)
		{
			return;
		}
		float num = accuracy * nValue.float15;
		if (UICrosshair.instance.Tween != null)
		{
			UICrosshair.instance.Tween.ChangeEndValue(num, true);
		}
		UICrosshair.instance.Accuracy = num;
		UICrosshair.instance.UpdateCrosshair();
		if (!UICrosshair.instance.isDynamic)
		{
			UICrosshair.instance.LeftSprite.cachedTransform.localPosition = Vector3.left * (num + (float)UICrosshair.instance.Gap);
			UICrosshair.instance.RightSprite.cachedTransform.localPosition = Vector3.right * (num + (float)UICrosshair.instance.Gap);
			UICrosshair.instance.TopSprite.cachedTransform.localPosition = Vector3.up * (num + (float)UICrosshair.instance.Gap);
			UICrosshair.instance.BottomSprite.cachedTransform.localPosition = Vector3.down * (num + (float)UICrosshair.instance.Gap);
		}
	}

	public static Vector2 Fire(float accuracy)
	{
        nProfiler.BeginSample("UICrosshair.Fire");
        if ((object)instance == null || !instance.isActive)
        {
            return new Vector2(UnityEngine.Random.Range(0f - accuracy, accuracy), UnityEngine.Random.Range(0f - accuracy, accuracy));
        }
        instance.FireAccuracy = (Vector2)Vector3.zero;
        if (accuracy != (float)nValue.int0)
        {
            instance.FireAccuracy = new Vector2((float)instance.Accuracy / (float)(int)instance.AccuracyWidth, (float)instance.Accuracy / (float)(int)instance.AccuracyHeight);
            UICrosshair uICrosshair = instance;
            uICrosshair.Accuracy = (float)uICrosshair.Accuracy + accuracy * nValue.float15;
            instance.Accuracy = Mathf.Min((float)instance.Accuracy, (float)instance.MaxAccuracy);
            instance.UpdateCrosshair();
        }
        nProfiler.EndSample();
        return instance.FireAccuracy;
    }

	public static void SetMove(float move)
	{
		if (move != (float)nValue.int0)
		{
			UICrosshair uicrosshair = UICrosshair.instance;
			uicrosshair.Accuracy += move;
			UICrosshair.instance.Accuracy = Mathf.Min(UICrosshair.instance.Accuracy, UICrosshair.instance.MaxAccuracy);
			UICrosshair.instance.UpdateCrosshair();
		}
	}

	private void UpdateCrosshair()
	{
        nProfiler.BeginSample("UICrosshair.UpdateCrosshair");
        TweenExtensions.Restart((Tween)(object)Tween.ChangeStartValue((object)(float)Accuracy, -1f), true, -1f);
        nProfiler.EndSample();
    }

	public static void Hit()
	{
        if (instance.HitMarker)
        {
            instance.HitAlpha = nValue.int1;
            TweenExtensions.Restart((Tween)(object)instance.HitTween.ChangeStartValue((object)instance.HitAlpha, -1f), true, -1f);
        }
    }

	public static void SetActiveScope(bool active)
	{
		UICrosshair.instance.RifleScope.SetActive(active);
		UICrosshair.SetActiveCrosshair(!active);
	}

	public static void SetActiveCrosshair(bool active)
	{
		try
		{
			UICrosshair.instance.Crosshair.SetActive(active);
		}
		catch
		{
		}
	}

	private void UpdateSettings()
	{
		this.HitMarker = Settings.HitMarker;
		int num = Mathf.FloorToInt(nPlayerPrefs.GetFloat("CrosshairSize", 0.2f) * 40f + 4f);
		this.LeftSprite.width = num;
		this.RightSprite.width = num;
		this.TopSprite.height = num;
		this.BottomSprite.height = num;
		int num2 = Mathf.FloorToInt(nPlayerPrefs.GetFloat("CrosshairThickness", 0.1f) * 20f + 2f);
		this.LeftSprite.height = num2;
		this.RightSprite.height = num2;
		this.TopSprite.width = num2;
		this.BottomSprite.width = num2;
		this.PointSprite.width = num2;
		this.PointSprite.height = num2;
		this.Gap = Mathf.FloorToInt(nPlayerPrefs.GetFloat("CrosshairGap", 0f) * 20f);
		this.LeftSprite.cachedTransform.localPosition = Vector3.left * (this.Accuracy + (float)this.Gap);
		this.RightSprite.cachedTransform.localPosition = Vector3.right * (this.Accuracy + (float)this.Gap);
		this.TopSprite.cachedTransform.localPosition = Vector3.up * (this.Accuracy + (float)this.Gap);
		this.BottomSprite.cachedTransform.localPosition = Vector3.down * (this.Accuracy + (float)this.Gap);
		string[] array = nPlayerPrefs.GetString("CrosshairColor", "1|1|1|1").Split(new char[]
		{
			"|"[0]
		});
		Color color = new Color(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]), float.Parse(array[3]));
		this.LeftSprite.color = color;
		this.RightSprite.color = color;
		this.TopSprite.color = color;
		this.BottomSprite.color = color;
		this.PointSprite.color = color;
		float @float = nPlayerPrefs.GetFloat("CrosshairAlpha", 1f);
		this.LeftSprite.alpha = @float;
		this.RightSprite.alpha = @float;
		this.TopSprite.alpha = @float;
		this.BottomSprite.alpha = @float;
		this.PointSprite.alpha = @float;
		this.PointSprite.cachedGameObject.SetActive(nPlayerPrefs.GetInt("CrosshairPoint", 0) == 1);
		this.LeftSprite.cachedGameObject.SetActive(nPlayerPrefs.GetInt("CrosshairEnable_0", 1) == 1);
		this.RightSprite.cachedGameObject.SetActive(nPlayerPrefs.GetInt("CrosshairEnable_1", 1) == 1);
		this.TopSprite.cachedGameObject.SetActive(nPlayerPrefs.GetInt("CrosshairEnable_2", 1) == 1);
		this.BottomSprite.cachedGameObject.SetActive(nPlayerPrefs.GetInt("CrosshairEnable_3", 1) == 1);
		this.isDynamic = (nPlayerPrefs.GetInt("CrosshairDynamics", 1) == 1);
	}
}
