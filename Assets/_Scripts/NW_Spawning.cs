﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class NW_Spawning : MonoBehaviour {

	public GameObject playerPrefab;
	public GameObject refereePrefab;
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
	private int amountPlayers = BasicFunctions.amountPlayers;
	
	void Start ()
	{
		spawnLocations = new List<Transform> ();
		spawnLocations.Add(spawn1);
		spawnLocations.Add(spawn2);
		spawnLocations.Add(spawn3);
		spawnLocations.Add(spawn4);
		spawnLocations.Add(spawn5);
		spawnPlayer();
		/*for (int i = 0; i < BasicFunctions.amountPlayers; i++)
		{
			BasicFunctions.connectedPlayers[i].Points = 0;
			BasicFunctions.gamePoints[i] = 0;
		}*/

		if (!BasicFunctions.playOffline)
		{
			//networkView.RPC("showScores", RPCMode.AllBuffered);
			debugScore.text = "ActiveNumber: " + BasicFunctions.activeAccount.Number;
			networkView.RPC("spawnReferee", RPCMode.All);
		}
	}
	
	public void spawnPlayer ()
	{
		int index = Random.Range (0, spawnLocations.Count-1); //Take random integer
		randomSpawnPoint = spawnLocations[index]; //Pick random spawnpoint (because of random int)
		if (BasicFunctions.playOffline)
		{
			Object.Instantiate (playerPrefab, randomSpawnPoint.position, Quaternion.identity);
		}
		else
		{
			GameObject playerN = Network.Instantiate (playerPrefab, randomSpawnPoint.position, Quaternion.identity, 0) as GameObject; //Instantiate player on the spawn point
			//playerN.GetComponent<playerController>().activeAccount = BasicFunctions.activeAccount;
			//playerN.GetComponent<playerController>().playerNumber = BasicFunctions.activeAccount.Number;
			networkView.RPC("setNumbers", RPCMode.All, playerN.networkView.viewID, BasicFunctions.activeAccount.Name, BasicFunctions.activeAccount.Word, BasicFunctions.activeAccount.Number);
			//networkView.RPC("removeSpawnPoint", RPCMode.AllBuffered, index); //Remove spawnpoint out of the list (no duplicate spawnpoints!)
		}
	}

	[RPC]
	public void spawnReferee ()
	{
		GameObject referee = Object.Instantiate (refereePrefab, new Vector3(0,0,0), Quaternion.identity) as GameObject;
		referee.GetComponent<Referee_script>().playerCount = amountPlayers;
	}

	[RPC]
	public void setNumbers (NetworkViewID player, string Uname, string Pword, int Number)
	{
		NetworkView playerN = NetworkView.Find(player);
		playerN.GetComponent<playerController>().activeAccount = new Account(Uname, Pword);
		playerN.GetComponent<playerController>().playerNumber = Number;
	}

	[RPC]
	public void removeSpawnPoint (int index)
	{
		spawnLocations.RemoveAt(index);
	}

	[RPC]
	public void showScores ()
	{
		debugScore.text = "";
		for (int i = 0; i < BasicFunctions.amountPlayers; i++)
		{
			debugScore.text = debugScore.text + BasicFunctions.activeAccounts[i] + "[" + BasicFunctions.accountNumbers[i] + "]: " + BasicFunctions.gamePoints[i] + "\n";
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