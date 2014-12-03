using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RobotMovement : MonoBehaviour {
	public Pathfinder pathfind;

	private List<Node> path;

	// Update is called once per frame
	void Update () {
		pathfind = this.GetComponent<Pathfinder>();
		if(pathfind.tracedBack == true){
			path = pathfind.path;
			for (int i=0;i<path.Count;i++){

			}
		}
	
	}
	/*
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

	public MoveObject(Vector3 startPos, Vector3 endPos, float moveTime){
		float i = 0;
		float rate = 1.0/moveTime;
		while(i<1.0){
			i += Time.deltaTime*rate;
			transform.position = Vector3.Lerp (startPos,endPos, Mathf.SmoothStep (0.0,1.0,i));
			                               

		}
	}
	*/
}
