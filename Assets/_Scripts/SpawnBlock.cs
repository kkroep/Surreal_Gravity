using UnityEngine;
using System.Collections;

public class SpawnBlock : MonoBehaviour {

	public GameObject buildingBlock;
	public float checkRadius;

	private int spawn;

	void startUp(int sp){
		if (sp>0){
			spawn = sp-1;
			createSecondarySpawn();

		}
	}

	void createSecondarySpawn(){
		Vector3 targetPosition;
		int iterations = 0;
		Vector3 add;
		int checkResult;

		do{
			add = new Vector3(Mathf.RoundToInt((Random.value*2) - 1),0,0);
			if(!(add.x > 0)){
				add.y += Mathf.RoundToInt((Random.value*2) - 1);
			}
			if(!(add.x > 0 && add.y>0)){
				add.z += Mathf.RoundToInt((Random.value*2) - 1);

			}

			targetPosition = this.transform.position + add;
			checkResult = Physics.OverlapSphere( targetPosition, checkRadius ).Length;
			iterations++;
			
			
		}while (checkResult>0 && iterations<20);

		GameObject go = (GameObject)Instantiate (buildingBlock, targetPosition, Quaternion.identity);
		go.SendMessage ("startUp", spawn);

	}
}
