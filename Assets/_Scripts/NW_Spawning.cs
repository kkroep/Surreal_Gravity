using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class NW_Spawning : MonoBehaviour {

	public GameObject playerPrefab;
	public GameObject refereePrefab;
	public Copy_LevelCreator levelCreator;
	public Text debugScore;
	public Text debugLives;
	public Text endGameText;

	public List<Vector3> spawnLocations;
	private GameObject player;
	private Vector3 randomSpawnPoint;
	private bool refreshing = false;
	private bool playOffline;
	private int amountSpawnPoints = 5;
	private GameObject referee;
	private Referee_script refScript;
	private bool canSpawn = true;
	
	void Start ()
	{
		spawnLocations = new List<Vector3> ();
		for (int i = 0; i < amountSpawnPoints; i++)
		{
			int width = Random.Range(0, levelCreator.levelWidth);
			int height = Random.Range(0, levelCreator.levelHeight);
			int depth = Random.Range(0, levelCreator.levelDepth);
			Vector3 spawn = new Vector3 (width, height, depth);
			if (Network.isServer)
			{
				networkView.RPC("addSpawnPoints", RPCMode.All, spawn);
			}
			else if (BasicFunctions.playOffline)
			{
				spawnLocations.Add (spawn);
			}
		}

		if (!BasicFunctions.playOffline)
		{
			if (Network.isServer)
			{
				referee = Network.Instantiate (refereePrefab, new Vector3(0,0,0), Quaternion.identity, 0) as GameObject; //Spawn referee
			}
		}
		else
		{
			spawnPlayer();
		}
	}
	
	public void spawnPlayer ()
	{
		int index = Random.Range (0, spawnLocations.Count-1); //Take random integer
		randomSpawnPoint = spawnLocations[index]; //Pick random spawnpoint (because of random int)
		if (BasicFunctions.playOffline)
		{
			Object.Instantiate (playerPrefab, randomSpawnPoint, Quaternion.identity);
		}
		else
		{
			GameObject playerN = Network.Instantiate (playerPrefab, randomSpawnPoint, Quaternion.identity, 0) as GameObject; //Instantiate player on the spawn point
			player = playerN;
			networkView.RPC("setNumbers", RPCMode.All, playerN.networkView.viewID, BasicFunctions.activeAccount.Name, BasicFunctions.activeAccount.Word, BasicFunctions.activeAccount.Number);
		}
	}

	public void showScores ()
	{
		if (!refScript) //if script not yet assigned, assign it
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
	/* Leave game gracefully as client
	 */ 
	public void closeClientInGame ()
	{
		networkView.RPC ("deleteUNServerInGame", RPCMode.Server, BasicFunctions.activeAccount.Name, BasicFunctions.activeAccount.Number);
		refScript.showScoreLive();
		networkView.RPC("makePlayerInvis", RPCMode.All, player.networkView.viewID);
		BasicFunctions.amountPlayers = 0;
		BasicFunctions.activeAccounts.Clear ();
		BasicFunctions.accountNumbers.Clear ();
		Network.Disconnect();
		playerController.dontDestroy = true;
		Application.LoadLevel("Menu_New");
	}
	/* Close the server gracefully
	 */
	public void closeServerInGame ()
	{
		if (BasicFunctions.amountPlayers > 1)
		{
			networkView.RPC("clearAccountsInGame", RPCMode.All);
			networkView.RPC("quitGame", RPCMode.All);
		}
		else
		{
			BasicFunctions.amountPlayers = 0;
			BasicFunctions.activeAccounts.Clear();
			BasicFunctions.accountNumbers.Clear();
			Application.LoadLevel("Menu_New");
		}
	}
	/* Fill spawnpositionvector
	 */
	[RPC]
	public void addSpawnPoints (Vector3 spawnPos)
	{
		spawnLocations.Add (spawnPos);
	}
	/* Assign number to a player
	 */
	[RPC]
	public void setNumbers (NetworkViewID player, string Uname, string Pword, int Number)
	{
		NetworkView playerN = NetworkView.Find(player);
		playerN.GetComponent<playerController>().activeAccount = new Account(Uname, Pword);
		playerN.GetComponent<playerController>().playerNumber = Number;
		switch (Number)
		{
		case 1 : Debug.Log("Player1");
			break;
		case 2 : Debug.Log("Player2");
			break;
		case 3 : Debug.Log("Player3");
			break;
		case 4 : Debug.Log("Player4");
			break;
		default : Debug.Log("DIKKE TERROR ERROR");
			break;
		}

	[RPC]
	public void quitGame ()
	{
		Application.LoadLevel("Menu_New");
	}

	[RPC]
	public void showScoresRPC ()
	{
		if (!refScript)
		{
			refScript = (GameObject.FindGameObjectsWithTag("Referee_Tag"))[0].GetComponent<Referee_script>();
		}
		debugScore.text = "Scores: \n";
		for (int i = 0; i < BasicFunctions.amountPlayers; i++)
		{
			debugScore.text = debugScore.text + BasicFunctions.activeAccounts[i] + ": " + refScript.scores[i] + "\n";
		}
	}

	[RPC]
	public void showLivesRPC ()
	{
		if (!refScript)
		{
			refScript = (GameObject.FindGameObjectsWithTag("Referee_Tag"))[0].GetComponent<Referee_script>();
		}
		debugLives.text = "Lives \n";
		for (int i = 0; i < BasicFunctions.amountPlayers; i++)
		{
			debugLives.text = debugLives.text + BasicFunctions.activeAccounts[i] + ": " + refScript.lives[i] + "\n";
		}
	}

	[RPC]
	void clearAccountsInGame ()
	{
		BasicFunctions.amountPlayers = 0;
		BasicFunctions.activeAccounts.Clear();
		BasicFunctions.accountNumbers.Clear();
	}

	[RPC]
	void deleteUNServerInGame (string UN, int Number)
	{
		if (Network.isServer)
		{
			BasicFunctions.activeAccounts.Remove(UN);
			BasicFunctions.accountNumbers.Remove(Number);
			if (BasicFunctions.amountPlayers > 2)
				networkView.RPC ("setAmountPlayers", RPCMode.AllBuffered, false);
			else
				BasicFunctions.amountPlayers = 1;
			refScript.scores.RemoveAt((Number-1));
			refScript.lives.RemoveAt((Number-1));
			refScript.players.RemoveAt((Number-1));
			refScript.playerCount -= 1;
			if (BasicFunctions.amountPlayers > 2)
				networkView.RPC("deleteUNClientsInGame", RPCMode.AllBuffered, UN, Number);
		}
	}
	
	[RPC]
	void deleteUNClientsInGame (string UN, int Number)
	{
		if (Network.isClient)
		{
			BasicFunctions.activeAccounts.Remove(UN);
			BasicFunctions.accountNumbers.Remove(Number);
		}
	}
	/* Set the amount of players currently connected
	 */
	[RPC]
	void setAmountPlayers (bool up)
	{
		if (up)
			BasicFunctions.amountPlayers += 1;
		else 
			BasicFunctions.amountPlayers -= 1;
	}

	[RPC]
	void makePlayerInvis (NetworkViewID ID)
	{
		NetworkView quitterView = NetworkView.Find (ID);
		GameObject quitter = quitterView.gameObject;
		Destroy(quitter);
	}
	
	void Update () 
	{
		if (!BasicFunctions.playOffline && spawnLocations.Count == amountSpawnPoints && canSpawn)
		{
			canSpawn = false;
			spawnPlayer();
		}
	}
}