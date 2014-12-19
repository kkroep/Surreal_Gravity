using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class Referee_script : MonoBehaviour {

	public int playerCount;
	public int Lives_count = 3;
	//public int[] scores = new int[4]{0,0,0,0};
	//public int[] lives = new int[4]{3,3,3,3};
	public List<int> scores = new List<int>();
	public List<int> lives = new List<int>();
	public NW_Spawning spawnScript;
	//public playerController[] players;
	public List<playerController> players;
	public float respawnTimer = 1f; //seconds 

	public GameObject[] tmp;
	private bool Allplayers_Spawned = false;

	private Text endGameText;

	private int maxPoints = 1;

	private string encodedScore;
	private string encodedScore2;
	private string encodedLives;

	///Initialization
	void Start () {
		playerCount = BasicFunctions.amountPlayers;
		//players = new playerController[playerCount];
		//scores = new int[playerCount];
		//lives = new int[playerCount];
		players = new List<playerController>();
		scores = new List<int>();
		lives = new List<int>();

		for (int i=0; i<playerCount; i++) {
			//scores [i] = 0;
			//lives [i] = Lives_count;
			scores.Add (0);
			lives.Add (Lives_count);
		}
	}

	void Update(){
		if (!Allplayers_Spawned) {
			Debug.Log("has not yet found all players");
			tmp = GameObject.FindGameObjectsWithTag("Player");			

			if(tmp.Length==playerCount){
				Debug.Log("has found all players");
				for (int i=0; i<playerCount; i++) {
					for (int j=0; j<playerCount; j++) {
						if(tmp[j].GetComponent<playerController>().playerNumber==i+1){
							//players[i]=tmp[j].GetComponent<playerController>();
							players.Add (tmp[j].GetComponent<playerController>());
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
		if (Input.GetKeyDown(KeyCode.P))
		{
			networkView.RPC("finishGame", RPCMode.All);
		}
	}

	public void frag(int shooter, int target){
		//check if a player actually dies
		//Debug.Log (shooter.ToString() + " hit " + target.ToString());
		if (lives [target-1] <= 1) {

			//respawn player
			networkView.RPC("KillPlayer", RPCMode.All, target);

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

			if (scores[shooter-1] >= maxPoints)
			{
				networkView.RPC("finishGame", RPCMode.All);
				endGameText = GameObject.FindGameObjectWithTag("GameEndTag").GetComponent<Text>();
				networkView.RPC("setEndGameText", RPCMode.All);

			}

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

	public void fragged(int target)
	{
		scores[target-1] -= 1;
		encodedScore2 = scores[0].ToString();
		for(int i=1; i<playerCount; i++){
			encodedScore2 += " " + scores[i];
		}

		networkView.RPC("UpdateScores", RPCMode.All, encodedScore2);
	}

	public void showScoreLive ()
	{
		string enc_score = scores[0].ToString ();
		string enc_lives = lives[0].ToString ();
		for (int i = 1; i < playerCount; i++)
		{
			enc_score += " " + scores[i];
			enc_lives += " " + lives[i];
		}
		networkView.RPC("UpdateScores", RPCMode.All, enc_score);
		networkView.RPC("showLives", RPCMode.All, enc_lives);
	}

	[RPC]
	public void KillPlayer(int target){
		players[target-1].isAlive = false;
		players[target-1].time2death = respawnTimer;
		players [target - 1].gameObject.GetComponent<MeshRenderer> ().enabled = false;
		players [target - 1].gameObject.GetComponent<SphereCollider> ().enabled = false;
	}


	[RPC]
	public void UpdateScores(string encodedScore_update){
		if (!spawnScript)
		{
			spawnScript = GameObject.FindGameObjectWithTag("SpawnTag").GetComponent<NW_Spawning>();
		}
		string[] scores_update = encodedScore_update.Split(' ');
		for (int i = 0; i < scores.Count/*Length*/; i++)
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
		for (int i = 0; i < lives.Count/*Length*/; i++)
		{
			lives[i] = int.Parse(lives_update[i]);
		}
		spawnScript.showLives();
	}

	[RPC]
	public void finishGame ()
	{
		for (int i = 0; i < playerCount; i++)
		{
			players[i].endGame = true;
			for (int j = 0; j < playerCount; j++)
			{
				players [j].gameObject.GetComponent<MeshRenderer> ().enabled = false;
				players [j].gameObject.GetComponent<SphereCollider> ().enabled = false;
			}
		}
	}

	[RPC]
	public void setEndGameText ()
	{
		endGameText.text = "Game is over. Press ESC to end the game!";
	}
}