using UnityEngine;
using System.Collections;

public class Bullet_Controller : MonoBehaviour {
	/*public Vector3 current_normal;
	public GameObject Player_Object;
	public playerController PlayerScript;
	public GameObject Camera_Object;
	public Camera_Control CameraScript;*/

	public bool GravityBullet = false;
	public int shooterNumber;
	public float Time2Live = 2;

	private float Time2Live_left;

	/*private Vector3 Camera_Forward_tmp;
	private Vector3 New_Player_Forward_tmp;*/

	void Start()
	{
		Time2Live_left = Time2Live;
		/*//if (networkView.isMine)
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
		}*/
	}

	void FixedUpdate (){
		Time2Live_left -= Time.fixedDeltaTime;
		//Debug.Log (Time2Live_left);
		if(Time2Live_left<0)
			Destroy (gameObject);
	}

	void OnCollisionEnter(Collision collision) {
		/*current_normal = collision.contacts[0].normal*0.7f; //zodat rounden altijd goed gaat (BEUNOPLOSSING !!!! WARNING WARNING WARNING)

		current_normal.x = Mathf.Round (current_normal.x); 
		current_normal.y = Mathf.Round (current_normal.y);
		current_normal.z = Mathf.Round (current_normal.z);

		Camera_Forward_tmp = CameraScript.transform.forward;

		if(GravityBullet){
		//CameraScript.transform.localEulerAngles = new Vector3(85, transform.localEulerAngles.y, 0);
		PlayerScript.Gravity_Direction = -1f*current_normal;
		New_Player_Forward_tmp = BasicFunctions.ProjectVectorOnPlane(-1f*current_normal, Camera_Forward_tmp);
		PlayerScript.transform.rotation = Quaternion.LookRotation(New_Player_Forward_tmp, current_normal);
		CameraScript.transform.rotation = Quaternion.LookRotation(Camera_Forward_tmp, current_normal);
		}*/
		if (collision.gameObject.tag.Equals ("level"))
		{
			Destroy (gameObject);
		}
		/*else if (collision.gameObject.tag.Equals ("Player"))
		{
			Debug.Log("Player hit");
		}*/
	}
}
