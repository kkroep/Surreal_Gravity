using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RobotMovement : MonoBehaviour {
	private Pathfinder pathfind;

	private List<Node> path;
/*
	// Update is called once per frame
	void Update () {
		pathfind = this.GetComponent<Pathfinder>();
		if(pathfind.tracedBack == true){
			path = pathfind.path;
			MoveRobot (new Vector3(2,2,2),new Vector3(2,5,2),3.0);
		}
	
	}

	function MoveObject (startPos : Vector3, endPos : Vector3, moveTime : float)
	{
		var i = 0.0;
		var rate = 1.0/moveTime;
		while (i < 1.0)
		{
			i += Time.deltaTime * rate;
			transform.position = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0.0, 1.0, i));
			yield; 
		}
	}


	public void MoveRobot(Vector3 startPos, Vector3 endPos, float moveTime){
		transform.position = Vector3.Lerp (startPos,endPos, Time.deltaTime);			                               

	}
*/

}
