using UnityEngine;
using System.Collections;

public class RobotScript : MonoBehaviour {

	private GameObject levelcreator;
	private Copy_LevelCreator levelSettings;
	private bool needsSelection;
	public GameObject target;
	private bool quitting = false;
	private Pathfinder pathfinder;
	
	void Start () {
		levelcreator = GameObject.FindGameObjectWithTag("levelSettings");
		levelSettings = levelcreator.GetComponent<Copy_LevelCreator>();
		needsSelection = true;
		pathfinder = this.GetComponent<Pathfinder>();
	
	}

	void Update () {
		if (needsSelection && Network.isServer) {
			/*
			float[] xInterval = new float[]{0,levelSettings.levelWidth};
			float[] yInterval = new float[]{0,levelSettings.levelHeight};
			float[] zInterval = new float[]{0,levelSettings.levelDepth};
			*/
			selectBlock();

			
		}	
	}

	void selectBlock(){

		GameObject[] cubes = GameObject.FindGameObjectsWithTag ("level");

		BlockDestroy selectboolget;
		bool selectbool = false;
		int iterations = 50;

		bool isedge = false;

		if(cubes.Length>0){

			do{


				int selector = Random.Range (0, cubes.Length);


				target = cubes[selector];



				if(target != null){

					selectboolget = target.GetComponent<BlockDestroy>();

					selectbool = selectboolget.canBeSelected;
					isedge = levelSettings.isEdge (target.transform.position);
					if(isedge){
						break;
					}
				}
				else{
					continue;
				}
				iterations--;





			}while(iterations>0);
			if(iterations<=0){
				iterations = 50;
				do{
					iterations--;
					int selector = Random.Range (0, cubes.Length);
					
					target = cubes[selector];
					
					if(target!=null){
						selectboolget = target.GetComponent<BlockDestroy>();
						selectbool = selectboolget.canBeSelected;
						break;
					}					
				}while(iterations>0);

			}




			if(selectbool){
				target.SendMessage ("canSelect", false);
				target.SendMessage ("attachedRobot", this.gameObject);
				pathfinder.setTargetNode (target.transform.position);
				//Debug.Log (target.transform.position.x + "," + target.transform.position.y + "," + target.transform.position.z);
				pathfinder.findPath = true;
				if(pathfinder.targetNode != null){
					needsSelection = false;
				}
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
		if(!quitting && !playerController.dontDestroy){
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
