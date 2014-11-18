using UnityEngine;
using System.Collections;

public class Bullet_Controller : MonoBehaviour {
	public Vector3 current_normal;
	public GameObject Player_Object;
	public playerController PlayerScript;
	public GameObject Camera_Object;
	public Camera_Control CameraScript;

	private Vector3 Camera_Forward_tmp;
	private Vector3 New_Player_Forward_tmp;

	void Start()
	{
		//if (networkView.isMine)
		{
			//Find player
			if (Player_Object == null)
						Player_Object = GameObject.FindGameObjectWithTag ("Player");
			//find player script
			PlayerScript = Player_Object.GetComponent<playerController>();

			if (Camera_Object == null)
				Camera_Object = GameObject.FindGameObjectWithTag ("MainCamera");
			//find player script
			CameraScript = Camera_Object.GetComponent<Camera_Control>();
		}
	}

	void OnCollisionEnter(Collision collision) {
		//if (networkView.isMine)
		{
			current_normal = collision.contacts[0].normal*0.7f; //zodat rounden altijd goed gaat (BEUNOPLOSSING !!!! WARNING WARNING WARNING)

			current_normal.x = Mathf.Round (current_normal.x); 
			current_normal.y = Mathf.Round (current_normal.y);
			current_normal.z = Mathf.Round (current_normal.z);

			Debug.Log (current_normal);
			Camera_Forward_tmp = CameraScript.transform.forward;

			//CameraScript.transform.localEulerAngles = new Vector3(85, transform.localEulerAngles.y, 0);
			PlayerScript.Gravity_Direction = -1f*current_normal;
			New_Player_Forward_tmp = BasicFunctions.ProjectVectorOnPlane(-1f*current_normal, Camera_Forward_tmp);
			PlayerScript.transform.rotation = Quaternion.LookRotation(New_Player_Forward_tmp, -1f*current_normal);
			Destroy (gameObject);
		}
	}
}
