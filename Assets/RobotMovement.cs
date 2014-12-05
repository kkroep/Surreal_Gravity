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
	public float speed;
	private int target;

	public bool starting;
	public bool arrived = false;



	void Start(){
		starting = true;
		pathfind = this.GetComponent<Pathfinder>();
		robotscript = this.GetComponent<RobotScript>();


	}

	void Update() {

		if(pathfind.tracedBack == true){






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
			






		
		}
	}

	void selectNext(){
		start = new Vector3(path[target+1].xPosition,path[target+1].yPosition,path[target+1].zPosition);
		end = new Vector3(path[target].xPosition,path[target].yPosition,path[target].zPosition);
		target--;
		startTime = Time.time;
	}


}
