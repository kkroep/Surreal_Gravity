	using UnityEngine;
	using System.Collections;

	public class playerController : MonoBehaviour {
	#region [init look around]
	public float speed = 3.0F;
	public float jumpSpeed = 8.0F; 
	public float gravity = 20.0F;
	private Vector3 moveDirection = Vector3.zero;
	
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
		rigidbody.freezeRotation = true;
		Gravity_Direction = Initial_Gravity_Direction;
	}

	void Update ()
	{
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

		rigidbody.velocity = transform.forward * speed * Input.GetAxis("Vertical");
		rigidbody.velocity += Vector3.Cross(transform.up, transform.forward)* speed * Input.GetAxis("Horizontal");
		rigidbody.AddForce(Gravity_Direction * Gravity_Strength);
	}

}
