using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainRobotSettings : MonoBehaviour {

	public GameObject robot;
	public List<GameObject> robots;

	private int selectBlock;
	private bool hasspawned = false;
	public Copy_LevelCreator level;

	void Start(){
		playerController.dontDestroy = false;
		hasspawned = false;
		level = GameObject.FindGameObjectWithTag("levelSettings").GetComponent<Copy_LevelCreator>();

	}



	void Update(){

		if(Network.isServer && !BasicFunctions.playOffline && !hasspawned && level.gridinitialised){
			for(int i=0;i<BasicFunctions.maxRobots;i++){
				Network.Instantiate (robot, level.getRobotSpawn (), Quaternion.identity,0);
			}
			hasspawned = true;
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
