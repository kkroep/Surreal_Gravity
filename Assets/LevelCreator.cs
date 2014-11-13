using UnityEngine;
using System.Collections;

public class LevelCreator : MonoBehaviour {
	
	public GameObject buildingBlock;
	public int numberOfLargeSpawns;
	public int numberOfSmallSpawns;
	public int approxBlocks;
	public int levelWidth;
	public int levelHeight;
	public int levelDepth;
	public float checkRadius;
	
	private int randomNumber;
	
	// Use this for initialization
	void Start () {
		
		
		
		for(int i = 0; i<numberOfLargeSpawns;i++){
			
			createLargeSpawn();
			
		}
		
		for(int i = 0; i<numberOfLargeSpawns;i++){
			createSmallSpawn();		
			
		}
		
		
	}
	
	void createLargeSpawn(){
		int checkResult;
		randomNumber = (int)(Random.value)*approxBlocks/numberOfLargeSpawns;
		checkRadius = randomNumber;
		Vector3 targetPosition;
		do{
			targetPosition = new Vector3(Random.value*levelWidth,Random.value*levelHeight,Random.value*levelDepth);
			checkResult = Physics.OverlapSphere( targetPosition, checkRadius ).Length;		
			
		}while (checkResult>0);
		
		
	}
	
	void createSmallSpawn(){
		int checkResult;
		Vector3 targetPosition;
		do{
			targetPosition = new Vector3(Random.value*levelWidth,Random.value*levelHeight,Random.value*levelDepth);
			checkResult = Physics.OverlapSphere( targetPosition, checkRadius ).Length;				
		}while (checkResult>0);
		
		
	}
	
	
}
