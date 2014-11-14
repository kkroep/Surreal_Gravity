using UnityEngine;
using System.Collections;

public class SpawnBlock : MonoBehaviour {

	public GameObject buildingBlock;
	public GameObject Settings;
	public float checkRadius;

	private float xScaling;
	private float yScaling;
	private float zScaling;
	private bool maxOneDirectionOfChange;
	private int spawn;
	/*
	void startUp1(bool maxOneDirection){
		maxOneDirectionOfChange = maxOneDirection;
	}

	void startUp2(float xScale){
		xScaling = xScale;
	}

	void startUp3(float yScale){
		yScaling = yScale;
	}

	void startUp4(float zScale){
		zScaling = zScale;
	}
	*/

	void startUp(int sp){
		if (sp>0){
			spawn = sp-1;
			createSecondarySpawn();

		}
	}



	void createSecondarySpawn(){

		LevelCreator settings = Settings.GetComponent<LevelCreator>();


		Vector3 targetPosition;
		int iterations = 0;
		Vector3 add;
		int checkResult;

		do{
			if(settings.maxOneDirectionOfChange){
				float positionchange1 = Mathf.CeilToInt (Random.value*100);
				int positionchange2;
				float xInterval = (100/3);
				float yInterval = (100/3);
				float zInterval = (100/3);


				if(settings.xScaling > 1){
					xInterval = ((100/3)*settings.xScaling);
					yInterval = (100-xInterval)/2;
					zInterval = yInterval;
				}
				else if(settings.yScaling > 1){
					yInterval = ((100/3)*settings.yScaling);
					xInterval = (100-yInterval)/2;
					zInterval = xInterval;
				}
				else{
					zInterval = ((100/3)*settings.zScaling);
					xInterval = (100-zInterval)/2;
					yInterval = xInterval;
				}



				if(positionchange1<xInterval){
					positionchange2 = 1;
				}
				else if(positionchange1<(xInterval + yInterval)){
					positionchange2 = 2;
				}
				else{
					positionchange2 = 3;
				}

				int direction;

				float temprandom = Random.value;
				if(Random.value<0.5){
					direction = -1;
				}
				else{
					direction = 1;
				}

				add = new Vector3(0,0,0);


				if(positionchange2 == 1)
					add.x = direction;
				else if(positionchange2 == 2)
					add.y = direction;
				else if(positionchange2 == 3)
					add.z = direction;

			}
			
			else{
				add = new Vector3(Mathf.RoundToInt((Random.value*2) - 1),Mathf.RoundToInt((Random.value*2) - 1),Mathf.RoundToInt((Random.value*2) - 1));
				/*
				int i = 20;
				while((Mathf.Abs (add.x) + Mathf.Abs (add.y) + Mathf.Abs (add.z))>1 && i >0){
					int zerosetter = Mathf.CeilToInt (Random.value*3);

					Debug.Log (zerosetter);

					if(zerosetter == 1)
						add.x = 0;
					else if(zerosetter == 2)
						add.y = 0;
					else if(zerosetter == 3)
						add.z = 0;

					i--;

				}

				if(!(add.x > 0)){
					add.y += Mathf.RoundToInt((Random.value*2) - 1);
				}
				if(!(add.x > 0 && add.y>0)){
					add.z += Mathf.RoundToInt((Random.value*2) - 1);

				}
				*/
			}

			targetPosition = this.transform.position + add;
			checkResult = Physics.OverlapSphere( targetPosition, checkRadius ).Length;
			iterations++;

			
			
		}while (checkResult>0 && iterations<20);

		GameObject go = (GameObject)Instantiate (buildingBlock, targetPosition, Quaternion.identity);
		go.SendMessage ("startUp", spawn);
	}
}
