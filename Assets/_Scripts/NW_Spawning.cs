using UnityEngine;
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
	public Text debugLives;
	
	private static List<Transform> spawnLocations;
	private GameObject player;
	private Transform randomSpawnPoint;
	private bool refreshing = false;
	private bool playOffline;
	private int amountPlayers = BasicFunctions.amountPlayers;
	private GameObject referee;
	private Referee_script refScript;
	//private bool spawnRef = true;
	
	void Start ()
	{
		spawnLocations = new List<Transform> ();
		spawnLocations.Add(spawn1);
		spawnLocations.Add(spawn2);
		spawnLocations.Add(spawn3);
		spawnLocations.Add(spawn4);
		spawnLocations.Add(spawn5);
		spawnPlayer();

		if (!BasicFunctions.playOffline)
		{
			if (Network.isServer)
			{
				referee = Network.Instantiate (refereePrefab, new Vector3(0,0,0), Quaternion.identity, 0) as GameObject;
				referee.GetComponent<Referee_script>().playerCount = amountPlayers;
			}
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
			networkView.RPC("setNumbers", RPCMode.All, playerN.networkView.viewID, BasicFunctions.activeAccount.Name, BasicFunctions.activeAccount.Word, BasicFunctions.activeAccount.Number);
			//networkView.RPC("removeSpawnPoint", RPCMode.AllBuffered, index); //Remove spawnpoint out of the list (no duplicate spawnpoints!)
		}
	}

	public void respawnPlayer ()
	{

	}

	public void showScores ()
	{
		if (!refScript)
		{
			refScript = (GameObject.FindGameObjectsWithTag("Referee_Tag"))[0].GetComponent<Referee_script>();
		}
		networkView.RPC("showScoresRPC", RPCMode.All);
	}

	public void showLives ()
	{
		if (!refScript)
		{
			refScript = (GameObject.FindGameObjectsWithTag("Referee_Tag"))[0].GetComponent<Referee_script>();
		}
		networkView.RPC("showLivesRPC", RPCMode.All);
	}

	/*[RPC]
	public void spawnReferee ()
	{
		referee = Network.Instantiate (refereePrefab, new Vector3(0,0,0), Quaternion.identity, 0) as GameObject;
		referee.GetComponent<Referee_script>().playerCount = amountPlayers;
	}*/

	[RPC]
	public void respawnPlayerRPC ()
	{

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
	public void showScoresRPC ()
	{
		debugScore.text = "Scores: \n";
		for (int i = 0; i < amountPlayers; i++)
		{
			debugScore.text = debugScore.text + BasicFunctions.activeAccounts[i] + ": " + refScript.scores[i] + "\n";
		}
	}

	[RPC]
	public void showLivesRPC ()
	{
		debugLives.text = "Lives \n";
		for (int i = 0; i < amountPlayers; i++)
		{
			debugLives.text = debugLives.text + BasicFunctions.activeAccounts[i] + ": " + refScript.lives[i] + "\n";
		}
	}
	
	void Update () 
	{
		if (Input.GetKeyDown(KeyCode.R))
		{
			Application.LoadLevel(Application.loadedLevel);
		}

		/*if (spawnRef && Network.isServer && GameObject.FindGameObjectsWithTag("Player").Length == amountPlayers)
		{
			networkView.RPC ("spawnReferee", RPCMode.All);
			spawnRef = false;
		}*/

	}
}