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
	public float rotSpeed;
	private int target;

	public bool starting;
	public bool arrived = false;

	private float dx;
	private float dy;
	private float dz;



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
				target = path.Count-2;
				starting = false;
				startTime = Time.time;
				length = Vector3.Distance(start, end);
			}

			dx = end.x - start.x;
			dy = end.y - start.y;
			dz = end.z - start.z;

			Quaternion tolerp = Quaternion.LookRotation(new Vector3(dx*90,dy*90,dz*90),Vector3.up);





			float distCovered = (Time.time - startTime) * speed;
			float fracJourney = distCovered / length;
			if(path.Count > 2){
				this.transform.position = Vector3.Lerp(start, end, fracJourney);
				this.transform.rotation = Quaternion.Lerp (this.transform.rotation,tolerp,Time.deltaTime*rotSpeed);
			}

			if (target<=1 && fracJourney>0.99){
				robotscript.target.SendMessage("Kill",2.0f);
				pathfind.tracedBack = false;
				Quaternion oldpos = transform.rotation;
				this.transform.LookAt(robotscript.target.transform);
				Quaternion newpos = transform.rotation;
				this.transform.rotation = Quaternion.Lerp (oldpos,newpos,Time.deltaTime*rotSpeed);

			}
			/*
			if(target == 0){
				Debug.Log (path[target].xPosition + "," + path[target].yPosition + "," + path[target].zPosition);
			}
			*/
			if (fracJourney>0.99 && target>1){
				selectNext ();
			}		
		}
	}

	void selectNext(){
		target--;
		start = new Vector3(path[target+1].xPosition,path[target+1].yPosition,path[target+1].zPosition);
		end = new Vector3(path[target].xPosition,path[target].yPosition,path[target].zPosition);

		startTime = Time.time;
	}


}
