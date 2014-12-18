using UnityEngine;
using System.Collections;

public class DestroyAnimation : MonoBehaviour {

	private GameObject parentObject;
	private RobotMovement robotmovement;
	private RobotScript robotscript;
	private float timer;

	// Use this for initialization
	void Start () {
		parentObject = transform.parent.gameObject;
		robotmovement = parentObject.GetComponent<RobotMovement>();
		robotscript = parentObject.GetComponent<RobotScript>();
		timer = 0;
	}
	
	// Update is called once per frame
	void Update () {
		/*
		if(robotmovement.destroyTarget == true){
			Quaternion tolerp = Quaternion.LookRotation(robotmovement.vectorPath[robotmovement.vectorPath.Count-1],Vector3.up);
			this.transform.rotation = Quaternion.Slerp (this.transform.rotation,tolerp,Time.deltaTime*3.0f);
			if(Quaternion.Angle (this.transform.rotation,tolerp)<1 && Network.isServer){
				robotscript.target.SendMessage("Kill",2.0f);				
			}


			timer += Time.deltaTime;
			if(timer>2.0){
				Debug.Log (robotmovement.vectorPath[robotmovement.vectorPath.Count-1].ToString ());
				robotscript.target.SendMessage("Kill",2.0f);
				timer = 0;
			}


		}
		*/
	
	}
}
