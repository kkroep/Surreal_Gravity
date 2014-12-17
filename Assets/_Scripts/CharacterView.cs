using UnityEngine;
using System.Collections;

public class CharacterView : MonoBehaviour {

	private GameObject armature;
	private GameObject circle;
	private GameObject armMesh;
	private GameObject armsArmature;

	void Start () {
		armature = GameObject.Find ("Armature");
		circle = GameObject.Find ("Circle");
		/*armMesh = GameObject.Find ("ArmMesh");
		armsArmature = GameObject.Find ("ArmsArmature");*/

	}

	void Update () {
		if(networkView.isMine)
		{
			Destroy(armature);
			Destroy(circle);
		}
		/*
		else
		{
			Destroy(armMesh);
			Destroy(armsArmature);
			
		}*/
	}
}
