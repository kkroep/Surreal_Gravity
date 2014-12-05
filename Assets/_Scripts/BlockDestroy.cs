using UnityEngine;
using System.Collections;

public class BlockDestroy : MonoBehaviour {

	public bool canBeSelected = true;

	private GameObject robot;
	private bool quitting = false;


	void Kill(float lifetime){
		canBeSelected = false;
		Destroy (this.gameObject, lifetime);

	}

	void attachedRobot(GameObject rob){
		robot = rob;
	}

	void OnApplicationQuit() {
		quitting = true;
	}

	void OnDestroy(){
		if(!quitting){
			robot.SendMessage ("setNeedsSelection", true);
		}
	}
}
