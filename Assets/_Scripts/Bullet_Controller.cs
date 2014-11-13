using UnityEngine;
using System.Collections;

public class Bullet_Controller : MonoBehaviour {
	public Vector3 current_normal;
	public GameObject Player_Object;
	public playerController PlayerScript;

	void Start()
	{
		//Find player
		if (Player_Object == null)
						Player_Object = GameObject.FindGameObjectWithTag ("Player");
		//find player script
		PlayerScript = Player_Object.GetComponent<playerController>();
	}

	void OnTriggerEnter(Collider other){
		if(other.gameObject.tag == "level"){
			Debug.Log ("hit level");

		}
	}
	void OnCollisionEnter(Collision collision) {
		current_normal = collision.contacts[0].normal;
		Debug.Log (current_normal);
		PlayerScript.Gravity_Direction = -1f*current_normal;
		Destroy (gameObject);
	}
}
