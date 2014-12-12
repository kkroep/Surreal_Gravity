using UnityEngine;
using System.Collections;

public class Referee_script : MonoBehaviour {

	public int playerCount = 4;
	public int Lives_count = 3;
	public int[] scores;
	public int[] lives;

	private string encodedScore;

	// Use this for initialization
	void Start () {
		scores = new int[playerCount];
		lives = new int[playerCount];
		for (int i=0; i<playerCount; i++) {
			scores[i]=0;
			lives[i]=Lives_count;
		}
	}

	public void frag(int shooter, int target){
		if (lives [target] <= 1) {
			//Respawm player
			lives [target] = Lives_count;
			scores[shooter] +=1;

			encodedScore="";
			for(int i=0; i<playerCount; i++){
				encodedScore += scores[i];
			}
			networkView.RPC("UpdateScores", RPCMode.Others, encodedScore);
		}else{
			lives [target]--;
		}
	}

[RPC]
	public void UpdateScores(string encodedScore_update){
		Debug.Log (encodedScore_update);
	} 

}
