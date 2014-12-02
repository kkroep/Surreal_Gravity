using UnityEngine;
using System.Collections;

public class Node{



	public int heuristicValue = 0;
	public int movementCost = 0;
	public int totalCost = 0;
	public Node parentNode = null;
	public Node widthneg = null;
	public Node widthpos = null;
	public Node heightneg = null;
	public Node heightpos = null;
	public Node depthneg = null;
	public Node depthpos = null;


	public int xPosition;
	public int yPosition;
	public int zPosition;

	public bool canPass;

	public Node(int x, int y, int z){
		xPosition = x;
		yPosition = y;
		zPosition = z;

	}


}
