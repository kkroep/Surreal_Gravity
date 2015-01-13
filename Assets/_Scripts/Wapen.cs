using UnityEngine;
using System.Collections;

public class Wapen : MonoBehaviour {

	public Referee_script referee;
	public bool KillLaser_On = false;
	private playerController playerScript;
	private bool Can_Hit = false;

	public AudioClip stab_sound; 
	public AudioClip stab_someone_sound;

	void Start ()
	{
		if (!BasicFunctions.playOffline && BasicFunctions.ForkModus)
		{
			Debug.Log("Forkmodus = on");
			referee = GameObject.FindGameObjectWithTag("Referee_Tag").GetComponent<Referee_script>();
		}
		else
		{
			this.enabled = false;
		}

		playerScript = gameObject.transform.parent.GetComponent<playerController>();
		//Debug.Log("S: " + playerScript.playerNumber);
	}


	void Update(){
		if (Input.GetMouseButtonDown (0) && !transform.parent.GetComponent<playerController>().endGame) {
			AudioSource.PlayClipAtPoint (stab_sound, transform.position);
			collider.enabled = true;
			Can_Hit = true;
			transform.localPosition += new Vector3(0f,0f,0.25f);
				}
		if (Input.GetMouseButtonUp(0) && !transform.parent.GetComponent<playerController>().endGame) {
			collider.enabled = false;
			Can_Hit = false;
			transform.localPosition -= new Vector3(0f,0f,0.25f);
		}

	}

	void OnTriggerStay (Collider hit)
	{
		if (networkView.isMine)
		{
			if(Can_Hit && hit.tag=="Player")// Input.GetKeyDown ("space"))
			{
				AudioSource.PlayClipAtPoint (stab_someone_sound, transform.position);

				if(!referee)
				{
					referee = (GameObject.FindGameObjectsWithTag("Referee_Tag"))[0].GetComponent<Referee_script>();
				}
				if (hit.gameObject.GetComponent<playerController>().isAlive && gameObject.transform.parent.GetComponent<playerController>().isAlive)
				{
					referee.frag(playerScript.playerNumber, hit.gameObject.GetComponent<playerController>().playerNumber);
					Can_Hit = false;
				}
			}
		}
	}
}
