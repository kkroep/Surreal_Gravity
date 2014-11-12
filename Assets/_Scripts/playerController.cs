using UnityEngine;
using System.Collections;

public class playerController : MonoBehaviour {
	public Vector3 Start_Gravity = new Vector3(1f, 0f, 0f);

	// Use this for initialization
	void Start () {
	
	}
	
	void FixedUpdate () {
		if(networkView.isMine || offline){
			//This is the freeze option controlled by gameController
			//if(gameController.Freeze_Counter==0f){

			#region [velocity]
			//Movement in direction of the object
			Vector3 AddPos = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"),0f);
			rigidbody.velocity = AddPos * speed*Time.fixedDeltaTime;
			//rigidbody.AddForce(current_normal*-Gravity_Strength);
			#endregion
			//}
		}
	}
}