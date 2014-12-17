using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainRobotSettings : MonoBehaviour {
	
	public int numberRobots;
	public GameObject robot;
	public List<GameObject> robots;

	private int selectBlock;


	void Start(){
		if(Network.isServer){
			for(int i=0;i<numberRobots;i++){
				Network.Instantiate (robot, new Vector3(10,10,10), Quaternion.identity,0);
			}
		}

		/*
		for(int i=0;i<robots.Count;i++){
			networkView.RPC ("instantiateRobot",RPCMode.Others,robots[i].networkView.viewID);
		}
		*/
	}
	/*
	[RPC]
	void instantiateRobot(NetworkViewID ID){
		NetworkView robotN = NetworkView.Find (ID);
		Instantiate (robot, new Vector3(15,15,15), Quaternion.identity);
		robot.networkView = robotN;
	}
	*/
}
