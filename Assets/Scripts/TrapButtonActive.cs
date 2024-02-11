using System;
using UnityEngine;

public class TrapButtonActive : MonoBehaviour
{
    [Range(1f, 30f)]
    public int Key;

    public TrapButton[] Buttons;

    private void Start()
	{
		EventManager.AddListener<byte>("DeathRunClickButton", new EventManager.Callback<byte>(this.DeactiveButtons));
	}

	private void DeactiveButtons(byte button)
	{
		if ((int)button != this.Key)
		{
			return;
		}
		for (int i = 0; i < this.Buttons.Length; i++)
		{
			this.Buttons[i].DeactiveButton();
		}
	}
}
