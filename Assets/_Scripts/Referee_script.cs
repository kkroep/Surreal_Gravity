using UnityEngine;
using System.Collections;

public class Referee_script : MonoBehaviour {

	public int playerCount = 4;
	public int Lives_count = 3;
	public int[] scores;
	public int[] lives;

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
		Debug.Log (shooter.ToString() + " hit " + target.ToString());
		if (lives [target] <= 1) {
			//Respawm player
			lives [target] = Lives_count;
			scores[shooter] +=1;


			//encode scores to send with RPC
			encodedScore=scores[0].ToString();
			for(int i=1; i<playerCount; i++){
				encodedScore += " " + scores[i];
			}

			//call RPC
			networkView.RPC("UpdateScores", RPCMode.All, encodedScore);
		}else{
			//if the player does not die
			lives [target]--;
		}
	}

[RPC]
	public void UpdateScores(string encodedScore_update){
		Debug.Log (encodedScore_update);
	} 

}
