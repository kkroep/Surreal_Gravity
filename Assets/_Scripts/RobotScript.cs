using UnityEngine;
using System.Collections;

public class RobotScript : MonoBehaviour {
	public GameObject levelcreator;


	private LevelCreator levelSettings;
	private bool needsSelection;
	private GameObject target;
	
	void Start () {
		levelSettings = levelcreator.GetComponent<LevelCreator>();
		needsSelection = true;
	
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
		bool selectbool;
		int selector;
		int iterations = 500;

		if(cubes.Length>0){
			do{
			
				selector = Random.Range (0, cubes.Length);

				target = cubes[selector];
				selectboolget = target.GetComponent<BlockDestroy>();
				selectbool = selectboolget.canBeSelected;

				iterations--;

			}while((target.transform.position.x<xInterval[0] || target.transform.position.x>xInterval[1] ||
			       target.transform.position.y<yInterval[0] || target.transform.position.y>yInterval[1] ||
			       target.transform.position.z<zInterval[0] || target.transform.position.z>zInterval[1]) && iterations>0);

			if(selectbool){
				needsSelection = false;
				target.SendMessage ("Kill", 4.0f);
				target.SendMessage ("attachedRobot", this.gameObject);
			}
		}

	}

	void setNeedsSelection(bool set){
		needsSelection = set;
	}

	void OnDestroy(){
		BlockDestroy selectboolget = target.GetComponent<BlockDestroy>();
		selectboolget.canBeSelected = true;
	}
}
