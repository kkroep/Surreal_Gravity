using UnityEngine;
using System.Collections;

public class DestroyAnimation : MonoBehaviour {

	private GameObject parentObject;
	private RobotMovement robotmovement;
	private RobotScript robotscript;

	// Use this for initialization
	void Start () {
		parentObject = transform.parent.gameObject;
		robotmovement = parentObject.GetComponent<RobotMovement>();
		robotscript = parentObject.GetComponent<RobotScript>();
	}
	
	// Update is called once per frame
	void Update () {
		if(robotmovement.destroyTarget == true){
			robotscript.target.SendMessage("Kill",2.0f);
		}
	
	}
}
