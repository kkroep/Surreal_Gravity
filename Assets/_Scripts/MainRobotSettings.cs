using UnityEngine;
using System.Collections;

public class MainRobotSettings : MonoBehaviour {
	
	public int numberRobots;
	public GameObject robot;

	private int selectBlock;


	void Start(){
		for(int i=0;i<numberRobots;i++){
			Instantiate (robot, new Vector3(15,15,15), Quaternion.identity);
		}

	
	
	}

}
