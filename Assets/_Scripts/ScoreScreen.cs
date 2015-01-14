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

	private List<Text> playersT;
	private List<Text> scoresT;
	private List<int> kills;
	private List<int> deaths;
	private List<int> scores;

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
