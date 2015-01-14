﻿using UnityEngine;
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
	public Text score1;
	public Text score2;
	public Text score3;
	public Text score4;

	public List<int> kills;
	public List<int> deaths;
	public List<int> scores;

	public string gamemode;

	public bool offline;

	private int maxPoints = 10;

	private string encodedKills;
	private string encodedDeaths;
	private string encodedDeaths2;
	private string encodedScore;
	private string encodedScore2;

	private List<Text> playersT;
	private List<Text> scoresT;

	private Referee_script referee;

	// Use this for initialization
	void Start ()
	{
		/*players = new List<Text> ();
		playersT.Add(player1);
		playersT.Add(player2);
		playersT.Add(player3);
		playersT.Add(player4);
		scoresT.Add (score1);
		scoresT.Add (score2);
		scoresT.Add (score3);
		scoresT.Add (score4);

		for (int i = 0; i < BasicFunctions.amountPlayers; i++)
		{
			playersT[i].enabled = true;
			playersT[i].text = "" + BasicFunctions.activeAccounts[i];
			scoresT[i].text = "" + 0;
		}*/

		for (int i=0; i<BasicFunctions.amountPlayers; i++) {
			kills.Add (0);
			deaths.Add (0);
			scores.Add (0);
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Tab)) {
			scoreScreen.SetActive (true);
		}
		if (Input.GetKeyUp (KeyCode.Tab)) {
			scoreScreen.SetActive (false);
		}
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

	public void UpdateScoreDB (int target)
	{
		deaths[target-1] += 1;
		scores[target-1] -= 1;
		Debug.Log ("Target: " + deaths[target-1]);
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
	}
}
