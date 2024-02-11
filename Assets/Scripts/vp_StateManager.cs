using System;
using System.Collections.Generic;
using UnityEngine;

public class vp_StateManager
{
    private vp_Component m_Component;

    private List<vp_State> m_States;

    private Dictionary<string, int> m_StateIds;

    private static string m_DefaultStateNoDisableMessage = "Warning: The 'Default' state cannot be disabled.";

    private int m_DefaultId;

    private int m_TargetId;

    public vp_StateManager(vp_Component component, List<vp_State> states)
	{
		this.m_States = states;
		this.m_Component = component;
		this.m_Component.RefreshDefaultState();
		this.m_StateIds = new Dictionary<string, int>(StringComparer.CurrentCulture);
		foreach (vp_State vp_State in this.m_States)
		{
			vp_State.StateManager = this;
			if (!this.m_StateIds.ContainsKey(vp_State.Name))
			{
				this.m_StateIds.Add(vp_State.Name, this.m_States.IndexOf(vp_State));
			}
			else
			{
				Debug.LogWarning(string.Concat(new object[]
				{
					"Warning: ",
					this.m_Component.GetType(),
					" on '",
					this.m_Component.name,
					"' has more than one state named: '",
					vp_State.Name,
					"'. Only the topmost one will be used."
				}));
				this.m_States[this.m_DefaultId].StatesToBlock.Add(this.m_States.IndexOf(vp_State));
			}
			if (vp_State.Preset == null)
			{
				vp_State.Preset = new vp_ComponentPreset();
			}
			if (vp_State.TextAsset != null)
			{
				vp_State.Preset.LoadFromTextAsset(vp_State.TextAsset);
			}
		}
		this.m_DefaultId = this.m_States.Count - 1;
	}

	public void ImposeBlockingList(vp_State blocker)
	{
		if (blocker == null)
		{
			return;
		}
		if (blocker.StatesToBlock == null)
		{
			return;
		}
		if (this.m_States == null)
		{
			return;
		}
		foreach (int index in blocker.StatesToBlock)
		{
			this.m_States[index].AddBlocker(blocker);
		}
	}

	public void RelaxBlockingList(vp_State blocker)
	{
		if (blocker == null)
		{
			return;
		}
		if (blocker.StatesToBlock == null)
		{
			return;
		}
		if (this.m_States == null)
		{
			return;
		}
		foreach (int index in blocker.StatesToBlock)
		{
			this.m_States[index].RemoveBlocker(blocker);
		}
	}

	public void SetState(string state, bool setEnabled = true)
	{
		if (!vp_StateManager.AppPlaying())
		{
			return;
		}
		if (!this.m_StateIds.TryGetValue(state, out this.m_TargetId))
		{
			return;
		}
		if (this.m_TargetId == this.m_DefaultId && !setEnabled)
		{
			Debug.LogWarning(vp_StateManager.m_DefaultStateNoDisableMessage);
			return;
		}
		this.m_States[this.m_TargetId].Enabled = setEnabled;
		this.CombineStates();
		this.m_Component.Refresh();
	}

	public void Reset()
	{
		if (!vp_StateManager.AppPlaying())
		{
			return;
		}
		foreach (vp_State vp_State in this.m_States)
		{
			vp_State.Enabled = false;
		}
		this.m_States[this.m_DefaultId].Enabled = true;
		this.m_TargetId = this.m_DefaultId;
		this.CombineStates();
	}

	public void CombineStates()
	{
		int i = this.m_States.Count - 1;
		while (i > -1)
		{
			if (i == this.m_DefaultId)
			{
				goto IL_76;
			}
			if (this.m_States[i].Enabled)
			{
				if (!this.m_States[i].Blocked)
				{
					if (!(this.m_States[i].TextAsset == null))
					{
						goto IL_76;
					}
				}
			}
			IL_CE:
			i--;
			continue;
			IL_76:
			if (this.m_States[i].Preset == null)
			{
				goto IL_CE;
			}
			if (this.m_States[i].Preset.ComponentType == null)
			{
				goto IL_CE;
			}
			vp_ComponentPreset.Apply(this.m_Component, this.m_States[i].Preset);
			goto IL_CE;
		}
	}

	public bool IsEnabled(string state)
	{
		return vp_StateManager.AppPlaying() && this.m_StateIds.TryGetValue(state, out this.m_TargetId) && this.m_States[this.m_TargetId].Enabled;
	}

	private static bool AppPlaying()
	{
		return true;
	}
}
