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

	private GameObject levelcreator;
	private int[,,] grid;
	private Node[,,] nodeGrid;

	void Start(){
		levelcreator = GameObject.FindGameObjectWithTag("levelSettings");
		level = levelcreator.GetComponent<LevelCreator>();
		grid = level.getGrid ();
		nodeGrid = new Node[level.levelWidth,level.levelHeight,level.levelDepth];

		for(int width=0;width<30;width++){
			for (int height=0;height<30;height++){
				for (int depth=0;depth<30;depth++){
					Debug.Log (depth);
					Node temp =  new Node(width,height,depth);
					temp.heuristicValue = CalculateHeuristicValue(temp);
					if(width>0){
						temp.widthneg = nodeGrid[width-1,height,depth];
					}
					if(width<level.levelWidth-1){
						temp.widthpos = nodeGrid[width+1,height,depth];
					}
					if(height>0){
						temp.heightneg = nodeGrid[width,height-1,depth];
					}
					if(height<level.levelHeight-1){
						temp.heightpos = nodeGrid[width,height+1,depth];
					}
					if(depth>0){
						temp.depthneg = nodeGrid[width,height,depth-1];
					}
					if(depth<level.levelDepth-1){
						temp.depthpos = nodeGrid[width,height,depth+1];
					}

					nodeGrid[width,height,depth] = temp;
				}
			}
		}

		checkingNode = nodeGrid[1,1,1];

		//CalculateAllHeuristics();
		startNode = checkingNode;


	}

	void Update(){
		if (foundTarget == false){
			FindPath();
		}

		if (foundTarget == true){
			TraceBackPath ();
		}


	}


	public int CalculateHeuristicValue(Node temp){
		int heuristic = Mathf.Abs (targetNode.xPosition - temp.xPosition) + Mathf.Abs (targetNode.yPosition - temp.yPosition) + Mathf.Abs (targetNode.zPosition - temp.zPosition);
		return heuristic;
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

		}


	}

	private void TraceBackPath(){


	}

	private void DetermineNodeValues(Node checkingNode, Node other){

	}





}
