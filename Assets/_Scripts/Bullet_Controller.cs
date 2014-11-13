using UnityEngine;
using System.Collections;

public class Bullet_Controller : MonoBehaviour {
	public 

	void OnTriggerEnter(Collider other){
		if(other.gameObject.tag == "level"){
			Debug.Log ("Hit a block");

		}
	}
}
