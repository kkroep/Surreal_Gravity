using UnityEngine;
using System.Collections;

public class CharacterView : MonoBehaviour {

	private GameObject armature;
	private GameObject circle;
	private GameObject armMesh;
	private GameObject armsArmature;

	// Use this for initialization
	void Start () {
		armature = GameObject.Find ("Armature");
		circle = GameObject.Find ("Circle");
		armMesh = GameObject.Find ("ArmMesh");
		armsArmature = GameObject.Find ("ArmsArmature");

	}
	
	// Update is called once per frame
	void Update () {
		if(networkView.isMine)
		{
			Destroy(armature);
			Destroy(circle);
			
			// or u can use childB.renderer.enabled=false; if u just want to hide it
		}
		else
		{
			Destroy(armMesh);
			Destroy(armsArmature);
			
		}
	}
}
