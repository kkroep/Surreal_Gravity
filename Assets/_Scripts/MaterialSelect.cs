using UnityEngine;
using System.Collections;

public class MaterialSelect : MonoBehaviour {
	public Material[] MaterialList;
	
	// Randomly select a material from the list when instantiated
	void Start () {
		renderer.material = MaterialList [(int) (Random.Range (0, MaterialList.Length-0.0001f))];
		if(this.name.Equals("BuildingBlock1(Clone)")){
			renderer.material.color = Color.red;
		}
		else if(this.name.Equals ("BuildingBlock2(Clone)")){
			renderer.material.color = Color.cyan;
		}
		else if(this.name.Equals ("BuildingBlock3(Clone)")){
			renderer.material.color = Color.green;
		}

	}
}
