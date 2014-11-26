using UnityEngine;
using System.Collections;

public class MenuButtons : MonoBehaviour {

	public Color Button_Idle = Color.green;
	public Color Button_Active = Color.red;

	// Use this for initialization
	void OnMouseEnter(){
		renderer.material.color = Color.white;
	}

	void Start(){
		Debug.Log("test");
		renderer.material.color = Button_Idle;
	}

	void OnMouseExit(){
		Debug.Log("test");
		renderer.material.color = Button_Idle;
	}

}
