using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RobotMovement : MonoBehaviour {
	private Pathfinder pathfind;

	private List<Node> path;
<<<<<<< HEAD

	private Vector3 start;
	private Vector3 end = new Vector3(10,10,10);
	private float length;
	private float startTime;
	public float speed;
	private int target;

	public bool starting;
	public bool arrived = false;



	void Start(){
		starting = true;
=======
/*
	// Update is called once per frame
	void Update () {
>>>>>>> origin/master
		pathfind = this.GetComponent<Pathfinder>();
		if(pathfind.tracedBack == true){
<<<<<<< HEAD






			if(starting){
				path = pathfind.path;
				start = new Vector3(path[path.Count-1].xPosition,path[path.Count-1].yPosition,path[path.Count-1].zPosition);
				end = new Vector3(path[path.Count-2].xPosition,path[path.Count-2].yPosition,path[path.Count-2].zPosition);
				target = path.Count-3;
				starting = false;
				startTime = Time.time;
				length = Vector3.Distance(start, end);
			}



				float distCovered = (Time.time - startTime) * speed;
				float fracJourney = distCovered / length;
			if(path.Count > 2){
				transform.position = Vector3.Lerp(start, end, fracJourney);
			}

				if (target<=0 && fracJourney>0.99){
					robotscript.target.SendMessage("Kill",1.0f);
					pathfind.tracedBack = false;
				}

				if (fracJourney>0.99 && target>=0){
					selectNext ();
				}
			






		
=======
			path = pathfind.path;
			MoveRobot (new Vector3(2,2,2),new Vector3(2,5,2),3.0);
>>>>>>> origin/master
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
