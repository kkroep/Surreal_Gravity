using UnityEngine;
using System.Collections;

public class LevelCreator : MonoBehaviour {
	
	public GameObject buildingBlock;
	public int numberOfLargeSpawns;
	public int numberOfSmallSpawns;
	public int approxBlocksPerLargeStack;
	public int approxBlocksPerSmallStack;
	public int levelWidth;
	public int levelHeight;
	public int levelDepth;
	public float checkRadius;
	public float xScaling;
	public float yScaling;
	public float zScaling;
	public bool maxOneDirectionOfChange;

	
	// Use this for initialization
	void Start () {
		
		
		
		for(int i = 0; i<numberOfLargeSpawns;i++){
			
			createLargeSpawn();
			
		}
		
		for(int i = 0; i<numberOfSmallSpawns;i++){
			createSmallSpawn();		
			
		}
		
		
	}
	
	void createLargeSpawn(){
		int checkResult;
		Vector3 targetPosition;
		int iterations = 0;
		do{
			targetPosition = new Vector3(Random.value*levelWidth,Random.value*levelHeight,Random.value*levelDepth);
			checkResult = Physics.OverlapSphere( targetPosition, checkRadius ).Length;
			iterations++;


		}while (checkResult>0 && iterations<50);

		GameObject go = (GameObject)Instantiate (buildingBlock, targetPosition, Quaternion.identity);
		go.SendMessage ("startUp", approxBlocksPerLargeStack);
		
		
	}
	
	void createSmallSpawn(){
		int checkResult;
		int randomNumber = 0;
		Vector3 targetPosition;
		int iterations = 0;
		do{
			targetPosition = new Vector3(Random.value*levelWidth,Random.value*levelHeight,Random.value*levelDepth);
			checkResult = Physics.OverlapSphere( targetPosition, checkRadius ).Length;	
			iterations++;
		}while (checkResult>0 && iterations<50);

		GameObject go = (GameObject)Instantiate (buildingBlock, targetPosition, Quaternion.identity);
		go.SendMessage ("startUp", approxBlocksPerSmallStack);
		
	}
	
	
}
