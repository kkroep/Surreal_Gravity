using UnityEngine;
using System.Collections;

public class BlockDestroy : MonoBehaviour {

	public bool canBeSelected = true;

	private GameObject robot;
	private bool quitting = false;

	
	void Kill(float lifetime){
		canBeSelected = false;
		Network.Destroy (this.gameObject);

	}

	void canSelect(bool can){
		canBeSelected = can;
	}

	void attachedRobot(GameObject rob){
		robot = rob;
	}

	void OnApplicationQuit() {
		quitting = true;
	}

	void OnDestroy(){
		if(!quitting && Network.isServer){
			robot.SendMessage ("setNeedsSelection", true);
			Pathfinder pathfinder = robot.GetComponent<Pathfinder>();
			pathfinder.foundTarget = false;
			RobotMovement robotmovement = robot.GetComponent<RobotMovement>();
			robotmovement.destroyTarget = false;
			GameObject levelcreator = GameObject.FindGameObjectWithTag("levelSettings");
			Copy_LevelCreator levelSettings = levelcreator.GetComponent<Copy_LevelCreator>();
			levelSettings.setGrid (Mathf.RoundToInt(this.transform.position.x),Mathf.RoundToInt(this.transform.position.y),Mathf.RoundToInt(this.transform.position.z),0);
		}
	}
}
