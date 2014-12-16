﻿using UnityEngine;
using System.Collections;

public class Referee_script : MonoBehaviour {

	public int playerCount;
	public int Lives_count = 3;
	public int[] scores = new int[4]{0,0,0,0};
	public int[] lives = new int[4]{3,3,3,3};
	public NW_Spawning spawnScript;

	private string encodedScore;

	///Initialization
	void Start () {
		scores = new int[playerCount];
		lives = new int[playerCount];
		for (int i=0; i<playerCount; i++) {
			scores[i]=0;
			lives[i]=Lives_count;
		}
	}

	public void frag(int shooter, int target){
		//check if a player actually dies
		//Debug.Log (shooter.ToString() + " hit " + target.ToString());
		int newTarget = (target - 1);
		if (lives [newTarget] <= 1) {
			//Respawn player
			lives [newTarget] = Lives_count;
			scores[shooter-1] +=1;

			//encode scores to send with RPC
			encodedScore = scores[0].ToString();
			for(int i=1; i<playerCount; i++){
				encodedScore += " " + scores[i];
			}

			//call RPC
			networkView.RPC("UpdateScores", RPCMode.All, encodedScore);

			/*for (int j = 0; j < playerCount; j++)
			{
				Debug.Log ("Score[" + j + "]: " + scores[j]);
				Debug.Log("Lives[" + j + "]: " + lives[j]);
			}*/
		}else{
			//if the player does not die
			lives [newTarget]--;
		}
	}

	[RPC]
	public void UpdateScores(string encodedScore_update){
		//Debug.Log (encodedScore_update);
		if (!spawnScript)
		{
			Debug.Log("Spawnscript added");
			spawnScript = GameObject.FindGameObjectWithTag("SpawnTag").GetComponent<NW_Spawning>();
		}
		string[] scores_update = encodedScore_update.Split(' ');
		for (int i = 0; i < scores.Length; i++)
		{
			scores[i] = int.Parse(scores_update[i]);
		}
		Debug.Log("TRYING TO GET THE SCORES SHOWN");
		spawnScript.showScores();
	} 
}
