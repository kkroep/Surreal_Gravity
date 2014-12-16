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

	public bool reset = false;
	public bool arrived = false;
	public bool moving = false;
	public bool rotating = false;
	public bool rotatingcompleted = true;
	public bool movinginitiate = false;
	public bool rotatinginit = false;
	public bool destroyTarget = false;
	public List<Vector3> vectorPath;

	private float dx;
	private float dy;
	private float dz;




	void Start(){
		reset = false;
		vectorPath = new List<Vector3>();
		pathfind = this.GetComponent<Pathfinder>();
		robotscript = this.GetComponent<RobotScript>();
		rotatingcompleted = true;
		movinginitiate = false;
		rotatinginit = false;
		destroyTarget = false;
	}

	void Update() {

		if(pathfind.tracedBack == true){
			moving = true;
			pathfind.tracedBack = false;
			reset = true;
			rotatingcompleted = false;
		}

		if(reset){
			resetFunction();
			reset = false;
		}

		if(!rotatingcompleted){
			dx = end.x - start.x;
			dy = end.y - start.y;
			dz = end.z - start.z;
			
			if(Mathf.Abs(dx)>0 || Mathf.Abs(dy)>0 || Mathf.Abs (dz)>0){
				rotating = true;
				rotatinginit = true;
				moving = false;	
			}		
		}

		if(movinginitiate){
			startTime = Time.time;
			movinginitiate = false;
		}

		if(moving){




			float distCovered = (Time.time - startTime) * speed;
			float fracJourney = distCovered / length;

			if(path.Count>2){
				this.transform.position = Vector3.Lerp(start, end, fracJourney);
			}

			if (target>=(vectorPath.Count-2) && fracJourney>0.9){
				destroyTarget = true;
				moving = false;
				rotating = false;
				moving = false;				
			}

			if (fracJourney>0.85 && target<(vectorPath.Count-2)){
				selectNext ();
				rotatingcompleted = false;
			}
		}



		if(rotating == true && moving == false){
			if(rotatinginit == true){

			}
			Quaternion tolerp = Quaternion.LookRotation(new Vector3(dx*90,dy*90,dz*90),Vector3.up);
			this.transform.rotation = Quaternion.Slerp (this.transform.rotation,tolerp,Time.deltaTime*rotSpeed);
			if(Quaternion.Angle (this.transform.rotation,tolerp)<35){
				moving = true;
				rotating = true;
				rotatingcompleted = true;
				movinginitiate = true;

			}
		}

		if(rotating == true && moving == true){
			if(rotatinginit == true){
				
			}
			Quaternion tolerp = Quaternion.LookRotation(new Vector3(dx*90,dy*90,dz*90),Vector3.up);
			this.transform.rotation = Quaternion.Slerp (this.transform.rotation,tolerp,Time.deltaTime*rotSpeed);
			if(Quaternion.Angle (this.transform.rotation,tolerp)<1){
				moving = true;
				rotating = false;
				rotatingcompleted = true;
				//movinginitiate = true;
				
			}
		}

		if(destroyTarget){
			DestroyTarget();
		}					
	}


	void selectNext(){
		target++;
		start = vectorPath[target-1];
		end = vectorPath[target];
		movinginitiate = true;
	}

	void resetFunction(){
		path = pathfind.path;
		string todebug = routeToString (path);
		vectorPath = routeParser (todebug);
		start = vectorPath[0];
		end = vectorPath[1];
		//target = path.Count-2;
		target = 1;
		reset = false;
		length = Vector3.Distance(start, end);
	}

	void DestroyTarget(){
		Quaternion oldpos = transform.rotation;
		this.transform.LookAt(robotscript.target.transform);
		Quaternion newpos = transform.rotation;
		this.transform.rotation = Quaternion.Slerp (oldpos,newpos,Time.time*rotSpeed);
	}

	string routeToString(List<Node> inpath){
		string temp = System.String.Empty;
		for(int i=path.Count-1;i>=0;i--){
			temp = temp + inpath[i].xPosition + "," + inpath[i].yPosition + "," + inpath[i].zPosition + ";";
		}
		return temp;

	}

	List<Vector3> routeParser(string instring){
		vectorPath.Clear ();
		string[] positions = instring.Split(';');
		string[] coords;
		Vector3 tempCoord;
		for(int i=0;i<positions.Length-1;i++){
			coords = positions[i].Split(',');
			tempCoord = new Vector3(float.Parse(coords[0]),float.Parse(coords[1]),float.Parse (coords[2]));
			vectorPath.Add(tempCoord);
		}
		return vectorPath;
	}


	



}
