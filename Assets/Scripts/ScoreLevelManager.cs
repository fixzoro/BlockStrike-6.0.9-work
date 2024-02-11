using System;
using UnityEngine;

public class ScoreLevelManager : MonoBehaviour
{
    public TextMesh label;

    private void Start()
	{
		EventManager.AddListener("UpdateScore", new EventManager.Callback(this.UpdateScore));
		this.UpdateScore();
	}

	private void UpdateScore()
	{
		this.label.text = GameManager.redScore + ":" + GameManager.blueScore;
	}
}
