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
	public Text score1;
	public Text score2;
	public Text score3;
	public Text score4;
	public bool offline;

	private List<Text> players;
	private List<Text> scores;

	// Use this for initialization
	void Start ()
	{
		/*players = new List<Text> ();
		players.Add(player1);
		players.Add(player2);
		players.Add(player3);
		players.Add(player4);
		scores.Add (score1);
		scores.Add (score2);
		scores.Add (score3);
		scores.Add (score4);

		for (int i = 0; i < BasicFunctions.amountPlayers; i++)
		{
			players[i].enabled = true;
			players[i].text = "" + BasicFunctions.activeAccounts[i];
			scores[i].text = "" + 0;
		}*/
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
}
