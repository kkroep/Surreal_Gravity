using UnityEngine;
using System.Collections;

public class CharacterView : MonoBehaviour {

	public string armature = "Armature";
	public string body = "Circle";
//	private GameObject armMesh;
//	private GameObject armsArmature;

	void Start () {
/*		if(networkView.isMine || BasicFunctions.playOffline)
		{
	
		/*armMesh = GameObject.Find ("ArmMesh");
		armsArmature = GameObject.Find ("ArmsArmature");*/
		//}

		if(networkView.isMine || BasicFunctions.playOffline)
		{

			Destroy(transform.Find(armature).gameObject);
			Destroy (transform.Find (body).gameObject);
		}
	}

//	void Update () {
	/*	if(networkView.isMine || BasicFunctions.playOffline)
		{
			armature = GameObject.Find ("Armature");
			circle = GameObject.Find ("Circle");
			Destroy(armature);
			Destroy(circle);
		}
		/*
		else
		{
			Destroy(armMesh);
			Destroy(armsArmature);
			
		}*/
	//}
}
