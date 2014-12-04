using UnityEngine;
using System.Collections;

public class RobotScript : MonoBehaviour {

	private GameObject levelcreator;
	private LevelCreator levelSettings;
	private bool needsSelection;
	public GameObject target;
	private bool quitting = false;
	private Pathfinder pathfinder;
	
	void Start () {
		levelcreator = GameObject.FindGameObjectWithTag("levelSettings");
		levelSettings = levelcreator.GetComponent<LevelCreator>();
		needsSelection = true;
		pathfinder = this.GetComponent<Pathfinder>();
	
	}

	void Update () {
		if (needsSelection) {
			float[] xInterval = new float[]{0,levelSettings.levelWidth};
			float[] yInterval = new float[]{0,levelSettings.levelHeight};
			float[] zInterval = new float[]{0,levelSettings.levelDepth};
			selectBlock(xInterval,yInterval,zInterval);

			
		}	
	}

	void selectBlock(float[] xInterval, float[] yInterval, float[] zInterval){
		GameObject[] cubes = GameObject.FindGameObjectsWithTag ("level");
		BlockDestroy selectboolget;
		bool selectbool = false;
		int iterations = 500;

		bool isedge = false;

		if(cubes.Length>0){
			do{
			
				int selector = Random.Range (0, cubes.Length);

				target = cubes[selector];

				if(target!=null){
					selectboolget = target.GetComponent<BlockDestroy>();
					selectbool = selectboolget.canBeSelected;
				}
				else{
					break;
				}

				iterations--;

				isedge = levelSettings.isEdge (target.transform.position);

			}while(iterations>0 && !isedge);
			if(iterations<=0){
				Debug.Log ("Couldnt find appropriate edge");
			}



			if(selectbool){
				levelSettings.setGrid (Mathf.RoundToInt(target.transform.position.x),Mathf.RoundToInt(target.transform.position.y),Mathf.RoundToInt(target.transform.position.z),0);
				needsSelection = false;
				target.SendMessage ("canSelect", false);
				target.SendMessage ("attachedRobot", this.gameObject);
				pathfinder.setTargetNode (target.transform.position);
				//Debug.Log (target.transform.position.x + "," + target.transform.position.y + "," + target.transform.position.z);
				pathfinder.findPath = true;
			}
		}

	}

	void setNeedsSelection(bool set){
		needsSelection = set;
	}
	
	void OnApplicationQuit() {
		quitting = true;
	}

	void OnDestroy(){
		if(!quitting){
			BlockDestroy selectboolget = target.GetComponent<BlockDestroy>();
			selectboolget.canBeSelected = true;
		}
	}
}
/*
(target.transform.position.x<xInterval[0] || target.transform.position.x>xInterval[1] ||
 target.transform.position.y<yInterval[0] || target.transform.position.y>yInterval[1] ||
 target.transform.position.z<zInterval[0] || target.transform.position.z>zInterval[1]) 
 */
