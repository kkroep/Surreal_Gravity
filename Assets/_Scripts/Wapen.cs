using UnityEngine;
using System.Collections;

public class Wapen : MonoBehaviour {

	public Referee_script referee;
	public bool KillLaser_On = false;
	void Start ()
	{
		if (!BasicFunctions.playOffline)
		{
			referee = GameObject.FindGameObjectWithTag("Referee_Tag").GetComponent<Referee_script>();
		}
		else
		{
			this.enabled = false;
		}
	}

	void OnTriggerEnter (Collider hit)
	{
		if (networkView.isMine)
		{
			if( Input.GetMouseButtonDown(0) && hit.tag=="Player");// Input.GetKeyDown ("space"))
			{
				if(!referee){
					referee = (GameObject.FindGameObjectsWithTag("Referee_Tag"))[0].GetComponent<Referee_script>();
				}
				if (gameObject.transform.parent.GetComponent<playerController>().isAlive)
				{
					referee.frag(gameObject.transform.parent.gameObject.GetComponent<playerController>().playerNumber, hit.gameObject.GetComponent<playerController>().playerNumber);
				}
			}
		}
	}
}
