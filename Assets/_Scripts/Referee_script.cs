using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class Referee_script : MonoBehaviour {

	public int playerCount;
	public int Lives_count = 3;
	public List<int> scores = new List<int>();
	public List<int> lives = new List<int>();
	public NW_Spawning spawnScript;
	public List<playerController> players;
	public float respawnTimer = 1.5f; //seconds 

	public GameObject[] tmp;
	private bool Allplayers_Spawned = false;

	private int maxPoints = 1;
	public int winner;

	private string encodedScore;
	private string encodedScore2;
	private string encodedLives;

	//public AudioClip score_up_sound;
	//public AudioClip less_live_sound;

	///Initialization
	void Start () {
		playerCount = BasicFunctions.amountPlayers;
		players = new List<playerController>();
		scores = new List<int>();
		lives = new List<int>();

		for (int i=0; i<playerCount; i++) {
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
	}
	/* Hit by player
	 */
	public void frag(int shooter, int target){
		//check if a player actually dies
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
				string gamemode;
				if (BasicFunctions.ForkModus) {
					gamemode = "FORK";
				} else {
					gamemode = "RAILGUN";
				}
				networkView.RPC("finishGame", RPCMode.All, shooter);
				networkView.RPC("setEndGameText", RPCMode.All);
				string winnerLog = BasicFunctions.activeAccounts[winner-1];
				string url = "http://drproject.twi.tudelft.nl:8082/GameRegister?Server=" + BasicFunctions.activeAccount.Name + "&Finished=0" + "&Gamemode=" + gamemode + "&Winnaar=" + winnerLog;
				WWW www = new WWW (url);
				StartCoroutine (WaitForGameLog (www));

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
	/* Killed by Boundary of Death
	 */
	public void fragged(int target)
	{
		networkView.RPC("KillPlayer", RPCMode.All, target);
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
	/* Called when a player is killed
	 */
	[RPC]
	public void KillPlayer(int target){
		players[target-1].isAlive = false;
		players[target-1].time2death = respawnTimer;
		players [target - 1].gameObject.GetComponent<MeshRenderer> ().enabled = false;
		players [target - 1].gameObject.GetComponent<SphereCollider> ().enabled = false;
	}
	/* Called when someone scores a point
	 */
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
	/* Called when someone loses a live
	 */
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
				players [j].gameObject.GetComponent<MeshRenderer> ().enabled = false;
				players [j].gameObject.GetComponent<SphereCollider> ().enabled = false;
			}
		}
	}
	/* Show text when the game is over
	 */
	[RPC]
	public void setEndGameText ()
	{
		spawnScript.endGameText.text = "Game is over. Press ESC to end the game! \nWinner: " + BasicFunctions.activeAccounts[winner-1];
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
		
	}
}