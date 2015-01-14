using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class Referee_script : MonoBehaviour {

	public int playerCount;
	public int Lives_count = 3;
	//public List<int> kills = new List<int>();
	//public List<int> deaths = new List<int>();
	//public List<int> scores = new List<int>();
	public List<int> lives = new List<int>();
	public NW_Spawning spawnScript;
	public List<playerController> players;
	public float respawnTimer = 1.5f; //seconds 

	public GameObject[] tmp;
	private bool Allplayers_Spawned = false;

	//private int maxPoints = 10;
	public int winner;

	public ScoreScreen scoreScreen;

	/*private string encodedKills;
	private string encodedDeaths;
	private string encodedDeaths2;
	private string encodedScore;
	private string encodedScore2;*/
	private string encodedLives;

	//public AudioClip score_up_sound;
	//public AudioClip less_live_sound;

	///Initialization
	void Start () {
		scoreScreen = GameObject.FindGameObjectWithTag("ScoreScreen").GetComponent<ScoreScreen>();
		playerCount = BasicFunctions.amountPlayers;
		players = new List<playerController>();
		//kills = new List<int>();
		//deaths = new List<int>();
		//scores = new List<int>();
		lives = new List<int>();

		for (int i=0; i<playerCount; i++) {
			//kills.Add (0);
			//deaths.Add (0);
			//scores.Add (0);
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
				//spawnScript.showScores ();
				spawnScript.showLives ();
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
		if (lives [target-1] <= 1) {

			//respawn player
			networkView.RPC("KillPlayer", RPCMode.All, target);
			networkView.RPC("PlayDead", RPCMode.All, target);;

			lives [target-1] = Lives_count;
			//deaths [target-1] += 1;
			//kills [shooter-1] += 1;
			//scores[shooter-1] += 1;
			scoreScreen.UpdateScore(shooter, target);

			//encode scores to send with RPC
			//encodedKills = kills[0].ToString();
			//encodedDeaths = deaths[0].ToString();
			//encodedScore = scores[0].ToString();
			scoreScreen.EncodeStrings ();
			encodedLives = lives[0].ToString();
			for(int i=1; i<playerCount; i++){
				//encodedKills += " " + kills[i];
				//encodedDeaths += " " + deaths[i];
				//encodedScore += " " + scores[i];
				encodedLives += " " + lives[i];
			}

			//call RPC
			//networkView.RPC("UpdateInfo", RPCMode.All, encodedKills, encodedDeaths, encodedScore);
			networkView.RPC("showLives", RPCMode.All, encodedLives);

			/*if (scores[shooter-1] >= maxPoints)
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
				string url = "http://drproject.twi.tudelft.nl:8082/GameRegister?Server=" + BasicFunctions.activeAccounts[0] + "&Finished=1" + "&Gamemode=" + gamemode + "&Winnaar=" + winnerLog;
				WWW www = new WWW (url);
				StartCoroutine (WaitForGameLog (www));
			}*/

		}else{
			//if the player does not die
			networkView.RPC("PlayGetHit", RPCMode.All, target);
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
		if (!scoreScreen)
		{
			scoreScreen = GameObject.FindGameObjectWithTag("ScoreScreen").GetComponent<ScoreScreen>();
		}
		networkView.RPC("KillPlayer", RPCMode.All, target);
		scoreScreen.UpdateScoreDB (target);
		//scoreScreen.EncodeStringsDB ();
		/*deaths[target-1] += 1;
		scores[target-1] -= 1;
		encodedDeaths2 = deaths[0].ToString();
		encodedScore2 = scores[0].ToString();
		for(int i=1; i<playerCount; i++){
			encodedDeaths2 += " " + deaths[i];
			encodedScore2 += " " + scores[i];
		}*/
		//networkView.RPC("UpdateDeathBInfo", RPCMode.All, encodedDeaths2, encodedScore2);
	}

	public void showScoreLiveR ()
	{
		//string enc_kills = kills[0].ToString ();
		//string enc_deaths = deaths[0].ToString ();
		//string enc_score = scores[0].ToString ();
		string enc_lives = lives[0].ToString ();
		for (int i = 1; i < playerCount; i++)
		{
			//enc_kills += " " + kills[i];
			//enc_deaths += " " + deaths[i];
			//enc_score += " " + scores[i];
			enc_lives += " " + lives[i];
		}
		//networkView.RPC("UpdateInfo", RPCMode.All, enc_kills, enc_deaths, enc_score);
		networkView.RPC("showLives", RPCMode.All, enc_lives);
	}

	public void EndGame (int shooter)
	{
		networkView.RPC("finishGame", RPCMode.All, shooter);
		//networkView.RPC("setEndGameText", RPCMode.All);
		if (BasicFunctions.loginServer && Network.isServer)
		{
			string winnerLog = BasicFunctions.activeAccounts[winner-1];
			string url = "http://drproject.twi.tudelft.nl:8082/GameRegister?Server=" + BasicFunctions.activeAccounts[0] + "&Finished=1" + "&Gamemode=" + scoreScreen.gamemode + "&Winnaar=" + winnerLog;
			WWW www = new WWW (url);
			StartCoroutine (WaitForGameLog (www));
		}
	}
	[RPC]
	public void PlayGetHit (int target)
	{
		PlayGetHit(target);
	}

	[RPC]
	public void PlayDead (int target)
	{
		PlayDead (target);
	}
	/* Called when a player is killed
	 */
	[RPC]
	public void KillPlayer(int target){
		players[target-1].isAlive = false;
		players[target-1].time2death = respawnTimer;
		players[target-1].setScreenTimer();
		//players [target - 1].gameObject.GetComponent<MeshRenderer> ().enabled = false;
		players [target - 1].gameObject.GetComponent<SphereCollider> ().enabled = false;
	}
	/* Called when someone scores a point
	 */
	/*[RPC]
	public void UpdateInfo(string encodedKills_update, string encodedDeaths_update, string encodedScore_update){
		if (!spawnScript)
		{
			spawnScript = GameObject.FindGameObjectWithTag("SpawnTag").GetComponent<NW_Spawning>();
		}
		string[] kills_update = encodedKills_update.Split(' ');
		string[] deaths_update = encodedDeaths_update.Split(' ');
		string[] scores_update = encodedScore_update.Split(' ');
		for (int i = 0; i < scores.Count; i++)
		{
			kills[i] = int.Parse(kills_update[i]);
			deaths[i] = int.Parse(deaths_update[i]);
			scores[i] = int.Parse(scores_update[i]);
		}
		//spawnScript.showScores();
	}*/

	/*[RPC]
	public void UpdateDeathBInfo (string encodedDeaths_update, string encodedScore_update)
	{
		if (!spawnScript)
		{
			spawnScript = GameObject.FindGameObjectWithTag("SpawnTag").GetComponent<NW_Spawning>();
		}
		string[] deaths_update = encodedDeaths_update.Split(' ');
		string[] scores_update = encodedScore_update.Split(' ');
		for (int i = 0; i < scores.Count; i++)
		{
			deaths[i] = int.Parse(deaths_update[i]);
			scores[i] = int.Parse(scores_update[i]);
		}
	}*/
	/* Called when someone loses a live
	 */
	[RPC]
	public void showLives (string encodedLives_update){
		if (!spawnScript)
		{
			spawnScript = GameObject.FindGameObjectWithTag("SpawnTag").GetComponent<NW_Spawning>();
		}
		string[] lives_update = encodedLives_update.Split(' ');
		for (int i = 0; i < lives.Count; i++)
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
				//players [j].gameObject.GetComponent<MeshRenderer> ().enabled = false;
				players [j].gameObject.GetComponent<SphereCollider> ().enabled = false;
				players [j].setEndScreenTimer(winner);
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