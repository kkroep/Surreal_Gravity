using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pathfinder : MonoBehaviour{

	private List<Node> openList = new List<Node>();
	private List<Node> closedList = new List<Node>();
	private Node checkingNode = null;
	public Node startNode = null;
	public Node targetNode = null;
	public bool foundTarget = false;
	public int baseMovementCost = 10;
	public LevelCreator level;

	public List<Node> path = new List<Node>();

	public bool tracedBack = false;

	public RobotMovement robotmovement;
	private GameObject levelcreator;
	private int[,,] grid;
	private Node[,,] nodeGrid;
<<<<<<< HEAD
	public bool findPath = false;
	public bool reset = true;
=======
>>>>>>> origin/master

	void Start(){
		robotmovement = this.GetComponent<RobotMovement>();

		nodeGrid = new Node[level.levelWidth,level.levelHeight,level.levelDepth];


		for(int width=0;width<30;width++){
			for (int height=0;height<30;height++){
				for (int depth=0;depth<30;depth++){

					Node temp =  new Node(width,height,depth);
					nodeGrid[width,height,depth] = temp;


					if(width>0){
						nodeGrid[width,height,depth].widthneg = nodeGrid[width-1,height,depth];
						nodeGrid[width-1,height,depth].widthpos = nodeGrid[width,height,depth];
					}

					if(height>0){
						nodeGrid[width,height,depth].heightneg = nodeGrid[width,height-1,depth];
						nodeGrid[width,height-1,depth].heightpos = nodeGrid[width,height,depth];
					}
					if(depth>0){
						nodeGrid[width,height,depth].depthneg = nodeGrid[width,height,depth-1];
						nodeGrid[width,height,depth-1].depthpos = nodeGrid[width,height,depth];
					}





				}
			}
		}
<<<<<<< HEAD



		
=======
>>>>>>> origin/master

		targetNode = nodeGrid[20,10,5];

		CalculateAllHeuristics();


		checkingNode = nodeGrid[2,2,2];
		startNode = checkingNode;



	}

	void Update(){

<<<<<<< HEAD


		if(findPath){
			if(reset){
			if(openList != null && closedList != null){
				for (int i=0; i<openList.Count;i++){
					openList[i].parentNode = null;
				}
				for (int i=0; i<closedList.Count;i++){
					closedList[i].parentNode = null;
				}
			}

			path = new List<Node>();
			openList = new List<Node>();
			closedList = new List<Node>();
			startNode = nodeGrid[Mathf.RoundToInt(this.transform.position.x),Mathf.RoundToInt(this.transform.position.y),Mathf.RoundToInt(this.transform.position.z)];
			checkingNode = startNode;
			CalculateAllHeuristics();

			levelcreator = GameObject.FindGameObjectWithTag("levelSettings");
			level = levelcreator.GetComponent<LevelCreator>();
			grid = level.getGrid ();
			reset = false;
			}

			int j = 0;

			while(j<50){

=======
		while(foundTarget == false){
			levelcreator = GameObject.FindGameObjectWithTag("levelSettings");
			level = levelcreator.GetComponent<LevelCreator>();
			grid = level.getGrid ();

			if (foundTarget == false){
>>>>>>> origin/master

				FindPath();
				//Debug.Log (checkingNode.xPosition + "," + checkingNode.yPosition + "," + checkingNode.zPosition);

			}

<<<<<<< HEAD
				}

				if (foundTarget == true){

					if(tracedBack == false){
						TraceBackPath ();
						reset = true;
					}
=======
			if (foundTarget == true){
				if(tracedBack == false){
					TraceBackPath ();
>>>>>>> origin/master
				}
				j++;
			}
		}




	}



	public int CalculateHeuristicValue(Node temp){
		int heuristic = Mathf.Abs (targetNode.xPosition - temp.xPosition) + Mathf.Abs (targetNode.yPosition - temp.yPosition) + Mathf.Abs (targetNode.zPosition - temp.zPosition);
		return heuristic;
	}

	public void CalculateAllHeuristics(){		
		for(int width=0;width<30;width++){
			for (int height=0;height<30;height++){
				for (int depth=0;depth<30;depth++){
					nodeGrid[width,height,depth].hValue = CalculateHeuristicValue (nodeGrid[width,height,depth]);
				}
			}
		}

	}

	private void FindPath(){
		if (foundTarget == false){
			if (checkingNode.widthneg != null)
				DetermineNodeValues(checkingNode,checkingNode.widthneg);
			if (checkingNode.widthpos != null)
				DetermineNodeValues(checkingNode,checkingNode.widthpos);
			if (checkingNode.heightneg != null)
				DetermineNodeValues(checkingNode,checkingNode.heightneg);
			if (checkingNode.heightpos != null)
				DetermineNodeValues(checkingNode,checkingNode.heightpos);
			if (checkingNode.depthneg != null)
				DetermineNodeValues(checkingNode,checkingNode.depthneg);
			if (checkingNode.depthpos != null)
				DetermineNodeValues(checkingNode,checkingNode.depthpos);

			deleteFromOpenList(checkingNode);
			addToClosedList(checkingNode);
			checkingNode = smallestFValueNode();


		}



	}

	private void TraceBackPath(){
		Node node = targetNode.parentNode;
		path.Add (startNode);
		do{
			path.Add(node);
			node = node.parentNode;

		}while(node != null);

		robotmovement.starting = true;
		tracedBack = true;
<<<<<<< HEAD

=======
		for(int i=0;i<path.Count;i++){
			Debug.Log (path[i].xPosition + "," + path[i].yPosition + "," + path[i].zPosition);
		}
>>>>>>> origin/master
	}

	private void DetermineNodeValues(Node currentNode, Node testing){

		if (testing == null)
			return;
		if (testing == targetNode){
			targetNode.parentNode = currentNode;
			foundTarget = true;
			return;
		}
		if(grid[testing.xPosition,testing.yPosition,testing.zPosition] == 1){
			return;
		}

		if(closedList.Contains (testing) == false){
			if(openList.Contains (testing)== true){
				int newGValue = currentNode.gValue + baseMovementCost;

				if(newGValue < testing.gValue){
					testing.parentNode = currentNode;
					testing.gValue = newGValue;
					testing.calculatetotalValue();
				}
			}
			else{
				testing.parentNode = currentNode;
				testing.gValue = currentNode.gValue + baseMovementCost;
				testing.calculatetotalValue();
				addToOpenList(testing);

			}

			
		}
	}

	private void addToOpenList(Node toadd){
		openList.Add(toadd);
	}

	private void addToClosedList(Node toadd){
		closedList.Add(toadd);
	}

	private void deleteFromOpenList(Node todel){
		openList.Remove(todel);
	}

	private void deleteFromClosedList(Node todel){
		closedList.Remove (todel);
	}

	private Node smallestFValueNode(){
		Node smallest = openList[0];
		for(int i =1;i<openList.Count;i++){
			if (openList[i].totalValue < smallest.totalValue){
				smallest = openList[i];
			}
		}
		return smallest;
	}





}
