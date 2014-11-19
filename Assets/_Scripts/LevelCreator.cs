using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

	private int[,,] grid;
	private int[] targetPosition;

	
	// Use this for initialization
	void Start () {
		grid = new int[levelWidth,levelHeight,levelDepth];
		

		for(int i = 0; i<numberOfLargeSpawns;i++){
			
			createLargeSpawn();
			
		}


		for(int i = 0; i<numberOfSmallSpawns;i++){
			createSmallSpawn();		
			
		}

		draw();
		
		
	}
	
	void createLargeSpawn(){
		int checkResult;
		int Blocks = approxBlocksPerLargeStack;

		int iterations = 0;
		do{
			targetPosition = new int[3]{Random.Range(0,levelWidth),Random.Range(0,levelHeight),Random.Range(0,levelDepth)};
			checkResult = grid[targetPosition[0],targetPosition[1],targetPosition[2]];
			iterations++;


		}while (checkResult>0 && iterations<50);

		grid [targetPosition[0],targetPosition[1],targetPosition[2]] = 1;

		for(int i=0; i<Blocks; i++){
			createSpawn();

		}


		
		
	}


	void createSmallSpawn(){
		int checkResult;
		int Blocks = approxBlocksPerSmallStack;
		
		int iterations = 0;
		do{
			targetPosition = new int[3]{Random.Range(0,levelWidth),Random.Range(0,levelHeight),Random.Range(0,levelDepth)};
			checkResult = grid[targetPosition[0],targetPosition[1],targetPosition[2]];
			iterations++;
			
			
		}while (checkResult>0 && iterations<50);
		
		grid [targetPosition[0],targetPosition[1],targetPosition[2]] = 1;
		
		for(int i=0; i<Blocks; i++){
			createSpawn();
			
		}
		
	}


	void createSpawn(){
		


		List<int> xList = new List<int>();
		xList.Add (-1);xList.Add (1);
		List<int> yList = new List<int>();
		yList.Add (-1);yList.Add (1);
		List<int> zList = new List<int>();
		zList.Add (-1);zList.Add (1);

		if(targetPosition[0] == 0){
			xList.RemoveAt (0);
		}
		if(targetPosition[1] == 0){
			yList.RemoveAt (0);
		}
		if(targetPosition[2] == 0){
			zList.RemoveAt (0);
		}
		if(targetPosition[0] == levelWidth-1){
			xList.RemoveAt (1);
		}
		if(targetPosition[1] == levelHeight-1){
			yList.RemoveAt (1);
		}
		if(targetPosition[2] == levelDepth-1){
			zList.RemoveAt (1);
		}

		float positionchange1 = Random.value;
		int positionchange2 = 0;
		int[] temp = targetPosition;
		int checkResult = 1;
		int iterations = 0;

		float xInterval = xScaling/(xScaling+yScaling+zScaling);
		float yInterval = yScaling /(xScaling + yScaling + zScaling);
		float zInterval = zScaling /(xScaling + yScaling + zScaling);


	

		do{
				
			int temprandom = 0;
			int direction = 0;
				
				
			if(positionchange1<xInterval && xList.Count>0){
				temprandom = Random.Range(0,xList.Count);
				positionchange2 = 1;
				direction = xList[temprandom];
				xList.RemoveAt(temprandom);
			}
			else if(positionchange1<(xInterval + yInterval) && yList.Count>0){
				temprandom = Random.Range(0,yList.Count);
				positionchange2 = 2;
				direction = yList[temprandom];
				yList.RemoveAt(temprandom);
			}
			else if(positionchange1<(xInterval + yInterval + zInterval) && zList.Count>0){
				temprandom = Random.Range(0,zList.Count);
				positionchange2 = 3;
				direction = zList[temprandom];
				zList.RemoveAt(temprandom);
			}
				
				
				

				
				
			if(positionchange2 == 1)
				temp[0] += direction;
			else if(positionchange2 == 2)
				temp[1] += direction;
			else if(positionchange2 == 3)
				temp[2] += direction;
				
				
				
				
		

			checkResult = grid[temp[0],temp[1],temp[2]];
			iterations++;			
			
		}while (checkResult>0 && iterations<50);

		targetPosition = temp;
		grid[targetPosition[0],targetPosition[1],targetPosition[2]]=1;



	}

	void draw(){


		for(int width=0;width<levelWidth;width++){
			for (int height=0;height<levelHeight;height++){
				for (int depth=0;depth<levelDepth;depth++){


					if(grid[width,height,depth]>0){
						GameObject go = (GameObject)Instantiate (buildingBlock, new Vector3(width,height,depth), Quaternion.identity);
					}


				}


			}
		


		}



	}
	
	
}
