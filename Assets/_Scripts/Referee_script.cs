using UnityEngine;
using System.Collections;

public class Referee_script : MonoBehaviour {

	public int playerCount;
	public int Lives_count = 3;
	public int[] scores = new int[4]{0,0,0,0};
	public int[] lives = new int[4]{3,3,3,3};
	public NW_Spawning spawnScript;
	public playerController[] players;

	public GameObject[] tmp;
	private bool Allplayers_Spawned = false;

	private string encodedScore;
	private string encodedLives;

	///Initialization
	void Start () {
		playerCount = BasicFunctions.amountPlayers;
		players = new playerController[playerCount];
		scores = new int[playerCount];
		lives = new int[playerCount];

		for (int i=0; i<playerCount; i++) {
			scores [i] = 0;
			lives [i] = Lives_count;
		}
	}

	void Update(){
		if (!Allplayers_Spawned) {
			//Debug.Log("has not yet found all players");
			tmp = GameObject.FindGameObjectsWithTag("Player");			

			if(tmp.Length==playerCount){
				Debug.Log("has found all players");
				for (int i=0; i<playerCount; i++) {
					for (int j=0; j<playerCount; j++) {
						if(tmp[j].GetComponent<playerController>().playerNumber==i+1){
							players[i]=tmp[j].GetComponent<playerController>();
							break;
						}
					}
				}
				if (!spawnScript)
				{
					spawnScript = GameObject.FindGameObjectWithTag("SpawnTag").GetComponent<NW_Spawning>();
				}
				spawnScript.showScores ();
				spawnScript.showLives ();
				Allplayers_Spawned = true;
			}
		}
	}

	public void frag(int shooter, int target){
		//check if a player actually dies
		//Debug.Log (shooter.ToString() + " hit " + target.ToString());
		if (lives [target-1] <= 1) {

			//respawn player
			players[target-1].isAlive = false;


			lives [target-1] = Lives_count;
			scores[shooter-1] +=1;

			//encode scores to send with RPC
			encodedScore = scores[0].ToString();
			encodedLives = lives[0].ToString();
			for(int i=1; i<playerCount; i++){
				encodedScore += " " + scores[i];
				encodedLives += " " + lives[i];
			}

			//call RPC
			networkView.RPC("UpdateScores", RPCMode.All, encodedScore);
			networkView.RPC("showLives", RPCMode.All, encodedLives);

			/*for (int j = 0; j < playerCount; j++)
			{
				Debug.Log ("Score[" + j + "]: " + scores[j]);
				Debug.Log("Lives[" + j + "]: " + lives[j]);
			}*/
		}else{
			//if the player does not die
			lives [target-1]--;
			encodedLives = lives[0].ToString();
			for (int i = 1; i <playerCount; i++)
			{
				encodedLives += " " + lives[i];
			}
			networkView.RPC("showLives", RPCMode.All, encodedLives);
		}
	}

	[RPC]
	public void UpdateScores(string encodedScore_update){
		//Debug.Log (encodedScore_update);
		if (!spawnScript)
		{
			spawnScript = GameObject.FindGameObjectWithTag("SpawnTag").GetComponent<NW_Spawning>();
		}
		string[] scores_update = encodedScore_update.Split(' ');
		for (int i = 0; i < scores.Length; i++)
		{
			scores[i] = int.Parse(scores_update[i]);
		}
		spawnScript.showScores();
	}

	[RPC]
	public void showLives (string encodedLives_update){
		if (!spawnScript)
		{
			spawnScript = GameObject.FindGameObjectWithTag("SpawnTag").GetComponent<NW_Spawning>();
		}
		string[] lives_update = encodedLives_update.Split(' ');
		for (int i = 0; i < lives.Length; i++)
		{
			lives[i] = int.Parse(lives_update[i]);
		}
		spawnScript.showLives();
	}
}