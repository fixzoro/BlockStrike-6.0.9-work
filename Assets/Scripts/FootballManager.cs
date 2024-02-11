using System;
using UnityEngine;

public class FootballManager : MonoBehaviour
{
    public GameObject Ball;

    public Vector3 StartBallPosition;

    private static FootballManager instance;

    private void Awake()
	{
		FootballManager.instance = this;
	}

	public static void StartRound()
	{
		FootballManager.instance.Ball.SetActive(true);
		if (PhotonNetwork.isMasterClient)
		{
			FootballManager.instance.Ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
			FootballManager.instance.Ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
		}
		FootballManager.instance.Ball.transform.position = FootballManager.instance.StartBallPosition;
	}

	public static void FinishRound()
	{
		FootballManager.instance.Ball.SetActive(false);
		if (PhotonNetwork.isMasterClient)
		{
			FootballManager.instance.Ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
			FootballManager.instance.Ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
		}
		FootballManager.instance.Ball.transform.position = FootballManager.instance.StartBallPosition;
	}
}
