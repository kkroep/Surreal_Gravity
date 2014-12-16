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

	private string encodedScore;
	private string encodedLives;

	///Initialization
	void Start () {
		players = new playerController[playerCount];
		scores = new int[playerCount];
		lives = new int[playerCount];

		tmp = GameObject.FindGameObjectsWithTag("Player");
		Debug.Log(tmp.Length.ToString()+" and " + playerCount.ToString());
		for (int i=0; i<playerCount; i++) {
			scores[i]=0;
			lives[i]=Lives_count;


			for (int j=0; j<playerCount; j++) {
				if(tmp[j].GetComponent<playerController>().playerNumber==i+1){
					players[i]=tmp[j].GetComponent<playerController>();
						break;
				}
			}
		}
	}

	public void frag(int shooter, int target){
		//check if a player actually dies
		//Debug.Log (shooter.ToString() + " hit " + target.ToString());
		int newTarget = (target - 1);
		if (lives [newTarget] <= 1) {

			//respawn player
			players[target].isAlive = false;


			lives [newTarget] = Lives_count;
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
			lives [newTarget]--;
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