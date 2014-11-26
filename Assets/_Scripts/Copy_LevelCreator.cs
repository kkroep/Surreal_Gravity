using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Copy_LevelCreator : MonoBehaviour {
	
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
	public bool ceiling;
	public bool floor;
	public bool plusx;
	public bool negx;
	public bool plusz;
	public bool negz;

	private int[,,] grid;
	private int[] targetPosition;

	private bool playOffline;
	
	// Use this for initialization
	void Start () {
		playOffline = Networkmanager2.playOffline;
		if (Network.isServer || Network.isClient || playOffline)
		{
			grid = new int[levelWidth,levelHeight,levelDepth];
		

			//Fills the grid matrix with 1s, representing large spawns
			//createLargeSpawn() is called numberOfLargeSpawns times, each time creating a separate spawn
			for(int i = 0; i<numberOfLargeSpawns;i++){
			
				createMainSpawn(approxBlocksPerLargeStack);
			
			}

			//fills the grid matrix with 1s, representing small spawns
			//createSmallSpawn() is called numberOfSmallSpawns times, each time creating a separate spawn
			for(int i = 0; i<numberOfSmallSpawns;i++){
				createMainSpawn(approxBlocksPerSmallStack);		
			
			}

			//loop through the grid matrix, drawing a building block every time a 1 is encountered
			draw();
		}		
		
	}

	//function responsible for filling the grid matrix with spawns
	void createMainSpawn(int approxBlocks){
		int checkResult;

		//random offset for the blocks per spawn
		int delta = Random.Range (Mathf.FloorToInt (approxBlocks * -0.1f), Mathf.CeilToInt (approxBlocksPerLargeStack * 0.1f+1f));
		int Blocks = approxBlocksPerLargeStack + delta;

		int iterations = 0;
		do{
			//determine a random intial targetposition to start a spawn, the location is checked to see whether it already contains a 1
			targetPosition = new int[3]{Random.Range(0,levelWidth),Random.Range(0,levelHeight),Random.Range(0,levelDepth)};
			checkResult = grid[targetPosition[0],targetPosition[1],targetPosition[2]];
			iterations++;


		}while (checkResult>0 && iterations<50);

		//Make the grid at position targetposition equal to 1
		grid [targetPosition[0],targetPosition[1],targetPosition[2]] = 1;

		//Create the secondary spawns for the main spawn
		for(int i=0; i<Blocks; i++){
			createSpawn();

		}


		
		
	}

	//function responsible for creating secondary spawn
	void createSpawn(){
		

		//lists that contain the forward and backward direction
		List<int> xList = new List<int>();
		xList.Add (-1);xList.Add (1);
		List<int> yList = new List<int>();
		yList.Add (-1);yList.Add (1);
		List<int> zList = new List<int>();
		zList.Add (-1);zList.Add (1);

		//make sure the algorithm can not move out of the defined grid
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

		//variables used in the selection of the next direction of the building block
		float positionchange1 = Random.value;
		int positionchange2 = 0;

		int checkResult = 0;
		int iterations = 0;

		//define the probability intervals for each direction.
		float xInterval = xScaling/(xScaling + yScaling + zScaling);
		float yInterval = yScaling /(xScaling + yScaling + zScaling);
		float zInterval = zScaling /(xScaling + yScaling + zScaling);

		int direction;


	

		do{				
			int temprandom = 0;
			direction = 0;
				
			//determine which direction the next block should go
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

				
				
				

				
			//if a certain direction is chosen, check if that direction is not already filled with a 1	
			if(positionchange2 == 1){
				checkResult = grid[targetPosition[0]+direction,targetPosition[1],targetPosition[2]];
				if(checkResult<1)
					targetPosition[0] += direction;
			}
			else if(positionchange2 == 2){
				checkResult = grid[targetPosition[0],targetPosition[1]+direction,targetPosition[2]];
				if(checkResult<1)
					targetPosition[1] += direction;
			}
			else if(positionchange2 == 3){
				checkResult = grid[targetPosition[0],targetPosition[1],targetPosition[2]+direction];
				if(checkResult<1)
					targetPosition[2] += direction;
			}

			iterations++;	

			
		}while (checkResult>0 && iterations<50);

		//Set the grid at the new targetposition equal to 1		
		grid[targetPosition[0],targetPosition[1],targetPosition[2]]=1;



	}

	//function that loops through the gread and instanciates a building block when it encounters a 1
	void draw(){


		for(int width=0;width<levelWidth;width++){
			for (int height=0;height<levelHeight;height++){
				for (int depth=0;depth<levelDepth;depth++){


					if(grid[width,height,depth]>0){
						GameObject go = (GameObject)Instantiate (buildingBlock, new Vector3(width,height,depth), Quaternion.identity);
					}
					else if(floor && height ==0){
						GameObject go = (GameObject)Instantiate (buildingBlock, new Vector3(width,height,depth), Quaternion.identity);
					}
					else if(ceiling && height == levelHeight-1){
						GameObject go = (GameObject)Instantiate (buildingBlock, new Vector3(width,height,depth), Quaternion.identity);
					}
					else if(plusx && width == levelWidth-1){
						GameObject go = (GameObject)Instantiate (buildingBlock, new Vector3(width,height,depth), Quaternion.identity);
					}
					else if(negx && width == 0){
						GameObject go = (GameObject)Instantiate (buildingBlock, new Vector3(width,height,depth), Quaternion.identity);
					}
					else if(plusz && depth == levelDepth-1){
						GameObject go = (GameObject)Instantiate (buildingBlock, new Vector3(width,height,depth), Quaternion.identity);
					}
					else if(negz && depth == 0){
						GameObject go = (GameObject)Instantiate (buildingBlock, new Vector3(width,height,depth), Quaternion.identity);
					}
				}
			}
		}
	}


}
