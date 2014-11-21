using UnityEngine;
using System.Collections;

public class MovementScript : MonoBehaviour{
	/* Wordt ingewisseld voor movement script van de echte game
	 */
	public int speed;
	public int gravity;
	public static Color teamColor;

	private CharacterController charcontr;

	void Start () 
	{
		//renderer.material.color = teamColor;
		charcontr = GetComponent<CharacterController>();
	}

	void Update () 
	{
		//renderer.material.color = teamColor;
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
