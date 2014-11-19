	using UnityEngine;
	using System.Collections;

	public class playerController : MonoBehaviour {
	#region [init look around]
	public float initspeed = 3.0F;
	public float speed;
	public float jumpSpeed = 8.0F; 
	private Vector3 moveDirection = Vector3.zero;
	private Vector3 Current_Global_Force;


	public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 };
	public RotationAxes axes = RotationAxes.MouseX;
	public float sensitivityX = 15F;
	public float sensitivityY = 15F;
	
	public float minimumX = -360F;
	public float maximumX = 360F;
	
	public float minimumY = -60F;
	public float maximumY = 60F;
	
	float rotationY = 0F;
	#endregion

	#region [init other]

	public float Gravity_Strength = 10f;
	public Vector3 Initial_Gravity_Direction;
	public Vector3 Gravity_Direction;

	#endregion

	void Start()
	{
		Screen.lockCursor = true;
		rigidbody.freezeRotation = true;
		Gravity_Direction = Initial_Gravity_Direction;
		Current_Global_Force = Gravity_Direction * Gravity_Strength;
	}

	void FixedUpdate ()
	{
		//transform.rotation = Quaternion.LookRotation(transform.forward, -1f*Gravity_Direction);

		#region [look around]
		if (axes == RotationAxes.MouseXAndY)
		{
			float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;
			
			rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
			
			transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
		}
		else if (axes == RotationAxes.MouseX)
		{
			transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
		}
		else
		{
			rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
			
			transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
		}
		#endregion

		transform.TransformDirection (Vector3.forward);

		rigidbody.velocity = transform.forward * speed * Input.GetAxis("Vertical");
		rigidbody.velocity += Vector3.Cross(transform.up, transform.forward)* speed * Input.GetAxis("Horizontal");

		Current_Global_Force = Vector3.Lerp(Current_Global_Force, Gravity_Direction * Gravity_Strength, Time.fixedDeltaTime*4f); 
		rigidbody.AddForce(Current_Global_Force);
	}




	void OnCollisionStay(Collision collisionInfo) {

		if (Input.GetMouseButtonDown(1)){
			Current_Global_Force=(Gravity_Direction * jumpSpeed *-1f);
		}
	}
}
