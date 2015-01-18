using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class ScoreScreen : MonoBehaviour
{
	public GameObject scoreScreen;

	public Text player1;
	public Text player2;
	public Text player3;
	public Text player4;

	public Text kill1;
	public Text kill2;
	public Text kill3;
	public Text kill4;

	public Text death1;
	public Text death2;
	public Text death3;
	public Text death4;

	public Text score1;
	public Text score2;
	public Text score3;
	public Text score4;

	public Text endScreen;
	
	public List<int> kills;
	public List<int> deaths;
	public List<int> scores;

	public float time2show;
	public int winner;
	public string gamemode;

	public bool offline;
	public bool showScreen = false;

	private int maxPoints = 1;

	private string encodedKills;
	private string encodedDeaths;
	private string encodedDeaths2;
	private string encodedScore;
	private string encodedScore2;

	private List<Text> playersT;
	private List<Text> killsT;
	private List<Text> deathsT;
	private List<Text> scoresT;

	private Referee_script referee;

	// Use this for initialization
	void Start ()
	{
		time2show = 0f;

		kills = new List<int> ();
		deaths = new List<int> ();
		scores = new List<int> ();
		playersT = new List<Text> ();
		killsT = new List<Text> ();
		deathsT = new List<Text> ();
		scoresT = new List<Text> ();
		playersT.Add(player1);
		playersT.Add(player2);
		playersT.Add(player3);
		playersT.Add(player4);
		killsT.Add (kill1);
		killsT.Add (kill2);
		killsT.Add (kill3);
		killsT.Add (kill4);
		deathsT.Add (death1);
		deathsT.Add (death2);
		deathsT.Add (death3);
		deathsT.Add (death4);
		scoresT.Add (score1);
		scoresT.Add (score2);
		scoresT.Add (score3);
		scoresT.Add (score4);

		winner = 0;

		for (int i = 0; i < BasicFunctions.amountPlayers; i++)
		{
			playersT[i].gameObject.SetActive(true);
			playersT[i].text = "" + BasicFunctions.activeAccounts[i];
			killsT[i].text = "" + 0;
			deathsT[i].text = "" + 0;
			scoresT[i].text = "" + 0;
		}

		for (int i=0; i<BasicFunctions.amountPlayers; i++) {
			kills.Add (0);
			deaths.Add (0);
			scores.Add (0);
		}
	}

	public void showScoreScreen ()
	{
		showScreen = true;
		scoreScreen.SetActive(true);
		if (winner != 0)
		{
			Debug.Log("W: " + winner);
			endScreen.enabled = true;
			endScreen.gameObject.SetActive(true);
			endScreen.text = "Winner: " + BasicFunctions.activeAccounts[winner-1];
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!showScreen)
		{
			if(time2show > 0)
			{
				time2show-=Time.deltaTime;
				if(time2show<=0){
					time2show=0;
					scoreScreen.SetActive (false);
				}else
				scoreScreen.SetActive (true);
			}
			else
			{
				if (Input.GetKeyDown (KeyCode.Tab)) {
					scoreScreen.SetActive (true);
				}
				if (Input.GetKeyUp (KeyCode.Tab)) {
					scoreScreen.SetActive (false);
				}
			}
		}
		/*if (winner != 0)
		{
			Debug.Log("W: " + winner);
			endScreen.enabled = true;
			endScreen.gameObject.SetActive(true);
			endScreen.text = "Winner: " + BasicFunctions.activeAccounts[winner-1];
		}*/
	}

	public void UpdateScore (int shooter, int target)
	{
		if (!referee)
		{
			referee = (GameObject.FindGameObjectsWithTag("Referee_Tag"))[0].GetComponent<Referee_script>();
		}
		deaths [target-1] += 1;
		kills [shooter-1] += 1;
		scores[shooter-1] += 1;
		Debug.Log("Shooter: " + kills[shooter-1] + ", Target: " + deaths[target-1]);

		if (scores[shooter-1] >= maxPoints)
		{
			if (BasicFunctions.ForkModus) {
				gamemode = "FORK";
			} else {
				gamemode = "RAILGUN";
			}
			referee.EndGame(shooter);
		}
	}
	public void UpdateScoreScreen(){
		for (int i = 0; i < BasicFunctions.amountPlayers; i++) {
			deathsT [i].text = "" + deaths[i];
			killsT [i].text = "" + kills[i];
			scoresT [i].text = "" + scores[i];
		}
	}

	public void UpdateScoreDB (int target)
	{
		deaths[target-1] += 1;
		scores[target-1] -= 1;
		EncodeStringsDB ();
	}

	public void EncodeStrings ()
	{
		encodedKills = kills[0].ToString();
		encodedDeaths = deaths[0].ToString();
		encodedScore = scores[0].ToString();

		for(int i=1; i<BasicFunctions.amountPlayers; i++)
		{
			encodedKills += " " + kills[i];
			encodedDeaths += " " + deaths[i];
			encodedScore += " " + scores[i];
		}

		networkView.RPC("UpdateInfo", RPCMode.All, encodedKills, encodedDeaths, encodedScore);
	}

	public void EncodeStringsDB ()
	{
		encodedDeaths2 = deaths[0].ToString();
		encodedScore2 = scores[0].ToString();
		for(int i = 1; i < BasicFunctions.amountPlayers; i++)
		{
			encodedDeaths2 += " " + deaths[i];
			encodedScore2 += " " + scores[i];
		}

		networkView.RPC("UpdateInfoDB", RPCMode.All, encodedDeaths2, encodedScore2);
	}

	public void showScoreLiveS ()
	{
		string enc_kills = kills[0].ToString ();
		string enc_deaths = deaths[0].ToString ();
		string enc_score = scores[0].ToString ();
		for (int i = 1; i < BasicFunctions.amountPlayers; i++)
		{
			enc_kills += " " + kills[i];
			enc_deaths += " " + deaths[i];
			enc_score += " " + scores[i];
		}
		networkView.RPC("UpdateInfo", RPCMode.All, enc_kills, enc_deaths, enc_score);
	}

	public void deleteEntry ()
	{
		for (int j = 0; j < playersT.Count; j++)
		{
			playersT[j].gameObject.SetActive(false);
		}
		for (int i = 0; i < BasicFunctions.amountPlayers; i++)
		{
			playersT[i].gameObject.SetActive(true);
			playersT[i].text = BasicFunctions.activeAccounts[i];
		}
		EncodeStrings();
	}

	[RPC]
	public void UpdateInfo(string encodedKills_update, string encodedDeaths_update, string encodedScore_update)
	{
		string[] kills_update = encodedKills_update.Split(' ');
		string[] deaths_update = encodedDeaths_update.Split(' ');
		string[] scores_update = encodedScore_update.Split(' ');
		for (int i = 0; i < scores.Count; i++)
		{
			kills[i] = int.Parse(kills_update[i]);
			deaths[i] = int.Parse(deaths_update[i]);
			scores[i] = int.Parse(scores_update[i]);
		}
		UpdateScoreScreen();
	}

	[RPC]
	public void UpdateInfoDB (string encodedDeaths_update, string encodedScore_update)
	{
		string[] deaths_update = encodedDeaths_update.Split(' ');
		string[] scores_update = encodedScore_update.Split(' ');
		for (int i = 0; i < scores.Count; i++)
		{
			deaths[i] = int.Parse(deaths_update[i]);
			scores[i] = int.Parse(scores_update[i]);
		}
		UpdateScoreScreen();
	}
}
