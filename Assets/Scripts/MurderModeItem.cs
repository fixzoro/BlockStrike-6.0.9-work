using System;
using UnityEngine;

public class MurderModeItem : MonoBehaviour
{
    public int ID;

    public MurderModeItem.ItemList Item;

    public bool Active;

    public enum ItemList
    {
        Weapon,
        Clue
    }

    private void OnTriggerEnter(Collider other)
	{
		if (GameManager.roundState == RoundState.EndRound)
		{
			return;
		}
		if (other.CompareTag("Player"))
		{
			PlayerInput component = other.GetComponent<PlayerInput>();
			if (component != null)
			{
			}
		}
	}
}
