using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RobotMovement : MonoBehaviour {
	private Pathfinder pathfind;
	private RobotScript robotscript;

	private List<Node> path;

	private Vector3 start;
	private Vector3 end = new Vector3(10,10,10);
	private float length;
	private float startTime;
	private float speed = 8.0f;
	private int target;
	private bool starting;

	public bool arrived = false;



	void Start(){
		starting = true;
		pathfind = this.GetComponent<Pathfinder>();
		robotscript = this.GetComponent<RobotScript>();


	}

	void Update() {

		if(pathfind.tracedBack == true){

			path = pathfind.path;



			if(starting){
				start = new Vector3(path[path.Count-1].xPosition,path[path.Count-1].yPosition,path[path.Count-1].zPosition);
				end = new Vector3(path[path.Count-2].xPosition,path[path.Count-2].yPosition,path[path.Count-2].zPosition);
				target = path.Count-3;
				starting = false;
				startTime = Time.time;
				length = Vector3.Distance(start, end);
			}



			float distCovered = (Time.time - startTime) * speed;
			float fracJourney = distCovered / length;
			transform.position = Vector3.Lerp(start, end, fracJourney);

			if (fracJourney>0.99 && target>0){
				selectNext ();
			}

			if (target<=0 && arrived == false){
				robotscript.target.SendMessage("Kill",1.0f);
				arrived = true;
			}





		
		}
	}

	void selectNext(){
		start = new Vector3(path[target+1].xPosition,path[target+1].yPosition,path[target+1].zPosition);
		end = new Vector3(path[target].xPosition,path[target].yPosition,path[target].zPosition);
		target--;
		startTime = Time.time;
	}


}
