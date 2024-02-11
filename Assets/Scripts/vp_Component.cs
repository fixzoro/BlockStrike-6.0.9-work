using System;
using System.Collections.Generic;
using UnityEngine;

public class vp_Component : MonoBehaviour
{
    public bool Persist;

    protected vp_StateManager m_StateManager;

    [NonSerialized]
    protected vp_State m_DefaultState;

    protected bool m_Initialized;

    protected Transform m_Transform;

    protected Transform m_Parent;

    protected Transform m_Root;

    protected AudioSource m_Audio;

    protected Collider m_Collider;

    public List<vp_State> States = new List<vp_State>();

    public List<vp_Component> Children = new List<vp_Component>();

    public List<vp_Component> Siblings = new List<vp_Component>();

    public List<vp_Component> Family = new List<vp_Component>();

    public List<Renderer> Renderers = new List<Renderer>();

    public List<AudioSource> AudioSources = new List<AudioSource>();

    protected int m_DeactivationTimer;

    public vp_StateManager StateManager
	{
		get
		{
			return this.m_StateManager;
		}
	}
    
	public vp_State DefaultState
	{
		get
		{
			return this.m_DefaultState;
		}
	}
    
	public float Delta
	{
		get
		{
			return Time.deltaTime * nValue.float60;
		}
	}
    
	public float SDelta
	{
		get
		{
			return Time.smoothDeltaTime * nValue.float60;
		}
	}
    
	public Transform Transform
	{
		get
		{
			if (this.m_Transform == null)
			{
				this.m_Transform = base.transform;
			}
			return this.m_Transform;
		}
	}
    
	public Transform Parent
	{
		get
		{
			if (this.m_Parent == null)
			{
				this.m_Parent = base.transform.parent;
			}
			return this.m_Parent;
		}
	}
    
	public Transform Root
	{
		get
		{
			if (this.m_Root == null)
			{
				this.m_Root = base.transform.root;
			}
			return this.m_Root;
		}
	}
    
	public AudioSource Audio
	{
		get
		{
			if (this.m_Audio == null)
			{
				this.m_Audio = base.GetComponent<AudioSource>();
			}
			return this.m_Audio;
		}
	}

	public Collider Collider
	{
		get
		{
			if (this.m_Collider == null)
			{
				this.m_Collider = base.GetComponent<Collider>();
			}
			return this.m_Collider;
		}
	}

	public bool Rendering
	{
		get
		{
			return this.Renderers.Count > 0 && this.Renderers[0].enabled;
		}
		set
		{
			foreach (Renderer renderer in this.Renderers)
			{
				if (!(renderer == null))
				{
					renderer.enabled = value;
				}
			}
		}
	}

	protected virtual void Awake()
	{
		this.CacheChildren();
		this.CacheSiblings();
		this.CacheFamily();
		this.CacheRenderers();
		this.CacheAudioSources();
		this.m_StateManager = new vp_StateManager(this, this.States);
		this.StateManager.SetState("Default", base.enabled);
	}

	protected virtual void Start()
	{
		this.ResetState();
	}

	protected virtual void Init()
	{

	}

	protected virtual void OnEnable()
	{

	}

	protected virtual void OnDisable()
	{

	}

	protected virtual void Update()
	{
		if (!this.m_Initialized)
		{
			this.Init();
			this.m_Initialized = true;
		}
	}

	protected virtual void FixedUpdate()
	{

	}

	protected virtual void LateUpdate()
	{

	}

	public void SetState(string state, bool enabled = true, bool recursive = false, bool includeDisabled = false)
	{
		this.m_StateManager.SetState(state, enabled);
		if (recursive)
		{
			foreach (vp_Component vp_Component in this.Children)
			{
				if (includeDisabled || (vp_Utility.IsActive(vp_Component.gameObject) && vp_Component.enabled))
				{
					vp_Component.SetState(state, enabled, true, includeDisabled);
				}
			}
		}
	}

	public void ActivateGameObject(bool setActive = true)
	{
		if (setActive)
		{
			this.Activate();
			foreach (vp_Component vp_Component in this.Siblings)
			{
				vp_Component.Activate();
			}
		}
		else
		{
			this.DeactivateWhenSilent();
			foreach (vp_Component vp_Component2 in this.Siblings)
			{
				vp_Component2.DeactivateWhenSilent();
			}
		}
	}

	public void ResetState()
	{
		this.m_StateManager.Reset();
		this.Refresh();
	}

	public bool StateEnabled(string stateName)
	{
		return this.m_StateManager.IsEnabled(stateName);
	}

	public void RefreshDefaultState()
	{
		vp_State vp_State = null;
		if (this.States.Count == 0)
		{
			vp_State = new vp_State(base.GetType().Name, "Default", null, null);
			this.States.Add(vp_State);
		}
		else
		{
			for (int i = this.States.Count - 1; i > -1; i--)
			{
				if (this.States[i].Name == "Default")
				{
					vp_State = this.States[i];
					this.States.Remove(vp_State);
					this.States.Add(vp_State);
				}
			}
			if (vp_State == null)
			{
				vp_State = new vp_State(base.GetType().Name, "Default", null, null);
				this.States.Add(vp_State);
			}
		}
		if (vp_State.Preset == null || vp_State.Preset.ComponentType == null)
		{
			vp_State.Preset = new vp_ComponentPreset();
		}
		if (vp_State.TextAsset == null)
		{
			vp_State.Preset.InitFromComponent(this);
		}
		vp_State.Enabled = true;
		this.m_DefaultState = vp_State;
	}

	public void ApplyPreset(vp_ComponentPreset preset)
	{
		vp_ComponentPreset.Apply(this, preset);
		this.RefreshDefaultState();
		this.Refresh();
	}

	public vp_ComponentPreset Load(string path)
	{
		vp_ComponentPreset result = vp_ComponentPreset.LoadFromResources(this, path);
		this.RefreshDefaultState();
		this.Refresh();
		return result;
	}

	public vp_ComponentPreset Load(TextAsset asset)
	{
		vp_ComponentPreset result = vp_ComponentPreset.LoadFromTextAsset(this, asset);
		this.RefreshDefaultState();
		this.Refresh();
		return result;
	}

	public void CacheChildren()
	{
		this.Children.Clear();
		foreach (vp_Component vp_Component in base.GetComponentsInChildren<vp_Component>(true))
		{
			if (vp_Component.transform.parent == base.transform)
			{
				this.Children.Add(vp_Component);
			}
		}
	}

	public void CacheSiblings()
	{
		this.Siblings.Clear();
		foreach (vp_Component vp_Component in base.GetComponents<vp_Component>())
		{
			if (vp_Component != this)
			{
				this.Siblings.Add(vp_Component);
			}
		}
	}

	public void CacheFamily()
	{
		this.Family.Clear();
		foreach (vp_Component vp_Component in base.transform.root.GetComponentsInChildren<vp_Component>(true))
		{
			if (vp_Component != this)
			{
				this.Family.Add(vp_Component);
			}
		}
	}

	public void CacheRenderers()
	{
		this.Renderers.Clear();
		foreach (Renderer item in base.GetComponentsInChildren<Renderer>(true))
		{
			this.Renderers.Add(item);
		}
	}

	public void CacheAudioSources()
	{
		this.AudioSources.Clear();
		foreach (AudioSource item in base.GetComponentsInChildren<AudioSource>(true))
		{
			this.AudioSources.Add(item);
		}
	}

	public virtual void Activate()
	{
		TimerManager.Cancel(this.m_DeactivationTimer);
		vp_Utility.Activate(base.gameObject, true);
	}

	public virtual void Deactivate()
	{
		vp_Utility.Activate(base.gameObject, false);
	}

	public void DeactivateWhenSilent()
	{
		if (this == null)
		{
			return;
		}
		if (vp_Utility.IsActive(base.gameObject))
		{
			foreach (AudioSource audioSource in this.AudioSources)
			{
				if (audioSource.isPlaying && !audioSource.loop)
				{
					this.Rendering = false;
					this.m_DeactivationTimer = TimerManager.In(0.1f, delegate()
					{
						this.DeactivateWhenSilent();
					});
					return;
				}
			}
		}
		this.Deactivate();
	}

	public virtual void Refresh()
	{

	}
}
