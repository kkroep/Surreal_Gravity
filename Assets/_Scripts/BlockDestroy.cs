using UnityEngine;
using System.Collections;

public class BlockDestroy : MonoBehaviour {

	public bool canBeSelected = true;

	private GameObject robot;


	void Kill(float lifetime){
		canBeSelected = false;
		Destroy (this.gameObject, lifetime);

	}

	void attachedRobot(GameObject rob){
		robot = rob;
		this.renderer.material.color = Color.red;
	}

	void OnDestroy(){
		robot.SendMessage ("setNeedsSelection", true);
	}
}
