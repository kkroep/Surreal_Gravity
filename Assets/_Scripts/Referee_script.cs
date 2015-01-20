﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class Referee_script : MonoBehaviour {

	public int playerCount;
	public int Lives_count = 3;
	public List<int> lives = new List<int>();
	public NW_Spawning spawnScript;
	public List<playerController> players;
	public float respawnTimer = 1.5f; //seconds 
	public GameObject[] tmp;

	public int winner;

	public ScoreScreen scoreScreen;

	private bool Allplayers_Spawned = false;
	
	private string encodedLives;

	//public AudioClip score_up_sound;
	//public AudioClip less_live_sound;

	///Initialization
	void Start ()
	{
		scoreScreen = GameObject.FindGameObjectWithTag("ScoreScreen").GetComponent<ScoreScreen>();
		playerCount = BasicFunctions.amountPlayers;
		players = new List<playerController>();
		lives = new List<int>();

		for (int i=0; i<playerCount; i++)
		{
			lives.Add (Lives_count);
		}
	}

	void Update()
	{
		if (!Allplayers_Spawned)
		{
			Debug.Log("has not yet found all players");
			tmp = GameObject.FindGameObjectsWithTag("Player");
			if (tmp.Length==playerCount)
			{
				if (!spawnScript)
				{
					spawnScript = GameObject.FindGameObjectWithTag("SpawnTag").GetComponent<NW_Spawning>();
				}
				Debug.Log("has found all players");
				for (int i=0; i<playerCount; i++)
				{
					for (int j=0; j<playerCount; j++)
					{
						if(tmp[j].GetComponent<playerController>().playerNumber==i+1)
						{
							players.Add (tmp[j].GetComponent<playerController>());
							break;
						}
					}
				}
				if (players.Count == playerCount)
				{
					Allplayers_Spawned = true;
				}
			}
		}
	}
	/* Hit by player
	 */
	public void frag(int shooter, int target){
		//check if a player actually dies
		if (lives [target-1] <= 1)
		{
			networkView.RPC("updateLives", RPCMode.Others, 1, (target-1));
			networkView.RPC("showWPanel", RPCMode.All, target);
			//respawn player
			networkView.RPC("KillPlayer", RPCMode.All, target);
			networkView.RPC("PlayDead", RPCMode.All, target);;

			lives [target-1] = Lives_count;
			scoreScreen.UpdateScore(shooter, target);

			//encode scores to send with RPC
			scoreScreen.EncodeStrings ();
			encodedLives = lives[0].ToString();
			for(int i=1; i<playerCount; i++)
			{
				encodedLives += " " + lives[i];
			}
		}
		else
		{
			//if the player does not die
			if (lives [target-1] == 3)
			{
				networkView.RPC("updateLives", RPCMode.Others, 3, (target-1));
			}
			else if (lives [target-1] == 2)
			{
				networkView.RPC("updateLives", RPCMode.Others, 2, (target-1));
			}
			networkView.RPC("PlayGetHit", RPCMode.All, target-1);
			//networkView.RPC("showRedPlayer", RPCMode.All, target);
			players[target-1].hitColorRed();
			lives [target-1]--;
			encodedLives = lives[0].ToString();
			for (int i = 1; i <playerCount; i++)
			{
				encodedLives += " " + lives[i];
			}
			networkView.RPC("showWPanel", RPCMode.All, target);
		}
	}
	/* Killed by Boundary of Death
	 */
	public void fragged(int target)
	{
		if (!scoreScreen)
		{
			scoreScreen = GameObject.FindGameObjectWithTag("ScoreScreen").GetComponent<ScoreScreen>();
		}
		networkView.RPC("KillPlayer", RPCMode.All, target);
		scoreScreen.UpdateScoreDB (target);
	}

	public void EndGame (int shooter)
	{
		networkView.RPC("finishGame", RPCMode.All, shooter);
		if (BasicFunctions.loginServer && Network.isServer)
		{
			string winnerLog = BasicFunctions.activeAccounts[winner-1];
			string url = "http://drproject.twi.tudelft.nl:8082/GameRegister?Server=" + BasicFunctions.activeAccounts[0] + "&Finished=1" + "&Gamemode=" + scoreScreen.gamemode + "&Winnaar=" + winnerLog;
			WWW www = new WWW (url);
			StartCoroutine (WaitForGameLog (www));
		}
	}

	public void EndGameQuit ()
	{
		for (int i = 0; i < playerCount; i++)
		{
			players[i].endGame = true;
			for (int j = 0; j < playerCount; j++)
			{
				//players [j].gameObject.GetComponent<MeshRenderer> ().enabled = false;
				players [j].gameObject.GetComponent<CapsuleCollider> ().enabled = false;
				players [j].setEndScreenTimer(0);
			}
		}
	}

	[RPC]
	public void PlayGetHit (int target)
	{
		for (int i = 0; i < playerCount; i++)
		{
			players[i].PlayGetHit(target);
		}
	}

	[RPC]
	public void PlayDead (int target)
	{
		for (int i = 0; i < playerCount; i++)
		{
			players[i].PlayDead(target);
		}
	}

	[RPC]
	public void showRedPlayer (int target)
	{
		for (int i = 0; i < playerCount; i++)
		{
			if (players[i].activeAccount.Number == target)
			{
				players[i].hitColorRed();
			}
		}
		//players[target-1].hitColorRed();
	}

	[RPC]
	public void showWPanel (int target)
	{
		for (int i = 0; i < playerCount; i++)
		{
			if (players[i].activeAccount.Number == target)
			{
				players[i].showWP = true;
			}
		}
	}

	[RPC]
	public void updateLives (int L, int Number)
	{
		for (int i = 0; i < playerCount; i++)//if (networkView.isMine)
		{
			if (players[i].activeAccount.Number == Number+1)
			{
			if (L == 1)
			{
				if (!players[Number].Leven1)
				{
					players[Number].Leven1 = GameObject.FindGameObjectWithTag("Leven1");
				}
				players[Number].Leven1.SetActive(false);
				players[Number].Leven2.SetActive(false);
				players[Number].Leven3.SetActive(false);
			}
			else if (L == 2)
			{
				if (!players[Number].Leven2)
				{
					players[Number].Leven2 = GameObject.FindGameObjectWithTag("Leven2");
				}
				players[Number].Leven2.SetActive(false);
				players[Number].Leven3.SetActive(false);
			}
			else
			{
				if (!players[Number].Leven3)
				{
					players[Number].Leven3 = GameObject.FindGameObjectWithTag("Leven3");
				}
				players[Number].Leven3.SetActive(false);
			}
			}
		}
	}
	/* Called when a player is killed
	 */
	[RPC]
	public void KillPlayer(int target)
	{
		players[target-1].isAlive = false;
		players[target-1].time2death = respawnTimer;
		players[target-1].setScreenTimer();
		players[target-1].gameObject.GetComponent<CapsuleCollider> ().enabled = false;
		if (!players[target-1].networkView.isMine)
		{
			players[target-1].gameObject.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
		}
		players[target-1].gameObject.GetComponentInChildren<MeshRenderer>().enabled = false;
	}
	/* When game is over, make all the players invisible and prevent them from moving
	 */
	[RPC]
	public void finishGame (int Winner)
	{
		winner = Winner;
		for (int i = 0; i < playerCount; i++)
		{
			players[i].endGame = true;
			for (int j = 0; j < playerCount; j++)
			{
				//players [j].gameObject.GetComponent<MeshRenderer> ().enabled = false;
				players [j].gameObject.GetComponent<CapsuleCollider> ().enabled = false;
				players [j].setEndScreenTimer(winner);
			}
		}
	}

	IEnumerator WaitForGameLog (WWW www)
	{
		yield return www;
		
		if (www.error == null) {
			if (www.text.Equals ("Succesfully Registered Game")) {
				Debug.Log ("Succesfully logged");
				
			} else {
				Debug.Log ("Failed to log");
			}
		}

		for(int i=0;i<BasicFunctions.startingAccounts.Count-1;i++){
			string urlParticipant = "http://drproject.twi.tudelft.nl:8082/ParticipantsRegister?SERVER="+BasicFunctions.activeAccount.Name + "&PLAYER="+BasicFunctions.startingAccounts[i];
			WWW www2 = new WWW(urlParticipant);
			StartCoroutine (WaitForParticipantRegister(www2));
		}
		
		string finalurlparticipant = "http://drproject.twi.tudelft.nl:8082/ParticipantsRegister?SERVER="+BasicFunctions.activeAccount.Name + "&PLAYER="+BasicFunctions.startingAccounts[BasicFunctions.startingAccounts.Count-1];
		WWW www3 = new WWW(finalurlparticipant);
		yield return StartCoroutine (WaitForParticipantRegister(www3));
	}
	
	IEnumerator WaitForParticipantRegister (WWW www)
	{
		yield return www;
		
		if (www.error == null) {
			if (www.text.Equals ("succesfully logged participant")) {
				Debug.Log ("Succesfully logged participant");
			} else {
				Debug.Log ("Failed to log participant");
			}
		}
	}
}
