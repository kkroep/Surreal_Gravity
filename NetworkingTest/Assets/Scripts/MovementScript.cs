using UnityEngine;
using System.Collections;

public class MovementScript : MonoBehaviour{

	public int speed;
	public int gravity;

	private CharacterController charcontr;

	// Use this for initialization
	void Start () {

		charcontr = GetComponent<CharacterController>();
	
	}
	
	// Update is called once per frame
	void Update () {

		if (networkView.isMine)
		{
			Vector3 input = new Vector3(Input.GetAxis("Horizontal") * speed * Time.deltaTime, -gravity * Time.deltaTime, Input.GetAxis("Vertical") * speed * Time.deltaTime);
			charcontr.Move(input);
		}
		else
		{
			enabled = false;
		}
	
	}
}
