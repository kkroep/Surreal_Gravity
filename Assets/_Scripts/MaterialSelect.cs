using UnityEngine;
using System.Collections;

public class MaterialSelect : MonoBehaviour {
	public Material[] MaterialList;
	
	// Randomly select a material from the list when instantiated
	void Start () {
		renderer.material = MaterialList [(int) (Random.Range (0, MaterialList.Length-0.0001f))];
		Debug.Log (this.name);
		if(this.name.Equals("BuildingBlock1(Clone)")){
			renderer.material.color = Color.red;
			Debug.Log ("BuildingBlock1");
		}
		else if(this.name.Equals ("BuildingBlock2(Clone)")){
			renderer.material.color = Color.cyan;
			Debug.Log ("BuildingBlock2");
		}
		else if(this.name.Equals ("BuildingBlock3(Clone)")){
			renderer.material.color = Color.green;
			Debug.Log ("BuildingBlock3");
		}

	}
}
