using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class NW_Spawning : MonoBehaviour {

	public GameObject playerPrefab;
	public Transform spawn1;
	public Transform spawn2;
	public Transform spawn3;
	public Transform spawn4;
	public Transform spawn5;
	public Text debugScore;
	
	private static List<Transform> spawnLocations;
	private GameObject player;
	private Transform randomSpawnPoint;
	private bool refreshing = false;
	private bool playOffline;
	
	void Start ()
	{
		spawnLocations = new List<Transform> ();
		spawnLocations.Add(spawn1);
		spawnLocations.Add(spawn2);
		spawnLocations.Add(spawn3);
		spawnLocations.Add(spawn4);
		spawnLocations.Add(spawn5);
		spawnPlayer();
		if (!playOffline)
			networkView.RPC("showScores", RPCMode.AllBuffered, BasicFunctions.amountPlayers);
	}
	
	public void spawnPlayer ()
	{
		int index = Random.Range (0, spawnLocations.Count-1); //Take random integer
		randomSpawnPoint = spawnLocations[index]; //Pick random spawnpoint (because of random int)
		//Debug.Log(randomSpawnPoint);
		if (BasicFunctions.playOffline)
		{
			Object.Instantiate (playerPrefab, randomSpawnPoint.position, Quaternion.identity);
		}
		else
		{
			Network.Instantiate (playerPrefab, randomSpawnPoint.position, Quaternion.identity, 0); //Instantiate player on the spawn point
			//networkView.RPC("removeSpawnPoint", RPCMode.AllBuffered, index); //Remove spawnpoint out of the list (no duplicate spawnpoints!)
		}
		//player.GetComponent<Copy_playerController>().playOffline = playOffline;
	}
	
	[RPC]
	public void removeSpawnPoint (int index)
	{
		spawnLocations.RemoveAt(index);
	}

	[RPC]
	public void showScores (int players)
	{
		if (players == 2)
		{
			debugScore.text = "Player 1: " + BasicFunctions.activeAccounts[0] + "\n Player 2: " + BasicFunctions.activeAccounts[1];
		}
		else if (players == 3)
		{
			debugScore.text = "Player 1: " + BasicFunctions.activeAccounts[0] + "\n Player 2: " + BasicFunctions.activeAccounts[1] + "\n Player 3: " + BasicFunctions.activeAccounts[2];
		}
		else if (players == 4)
		{
			debugScore.text = "Player 1: " + BasicFunctions.activeAccounts[0] + "\n Player 2: " + BasicFunctions.activeAccounts[1] + "\n Player 3: " + BasicFunctions.activeAccounts[2]
			+ "\n Player 4: " + BasicFunctions.activeAccounts[3];
		}
	}
	
	void Update () 
	{
		if (Input.GetKeyDown(KeyCode.R))
		{
			Application.LoadLevel(Application.loadedLevel);
		}
	}
}