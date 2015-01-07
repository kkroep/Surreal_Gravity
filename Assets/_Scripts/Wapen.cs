using UnityEngine;
using System.Collections;

public class Wapen : MonoBehaviour {

	public Referee_script referee;

	void Start ()
	{
		referee = GameObject.FindGameObjectWithTag("Referee_Tag").GetComponent<Referee_script>();
	}

	void OnTriggerEnter (Collider hit)
	{
		if(hit.tag=="Player")
		{
			if(!referee){
				referee = (GameObject.FindGameObjectsWithTag("Referee_Tag"))[0].GetComponent<Referee_script>();
			}
			//referee.frag(gameObject.GetComponent<playerController>().playerNumber, hit.gameObject.GetComponent<playerController>().playerNumber);
			Debug.Log("YO");
		}
	}
}
