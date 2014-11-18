﻿	using UnityEngine;
	using System.Collections;

	/// MouseLook rotates the transform based on the mouse delta.
	/// Minimum and Maximum values can be used to constrain the possible rotation

	/// To make an FPS style character:
	/// - Create a capsule.
	/// - Add the MouseLook script to the capsule.
	///   -> Set the mouse look to use LookX. (You want to only turn character but not tilt it)
	/// - Add FPSInputController script to the capsule
	///   -> A CharacterMotor and a CharacterController component will be automatically added.

	/// - Create a camera. Make the camera a child of the capsule. Reset it's transform.
	/// - Add a MouseLook script to the camera.
	///   -> Set the mouse look to use LookY. (You want the camera to tilt up and down like a head. The character already turns.)

	[AddComponentMenu("Camera-Control/Mouse Look")]
	public class Camera_ControlTest : MonoBehaviour {

	public GameObject mCamera;

	#region [init for look around]
	public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
	public RotationAxes axes = RotationAxes.MouseY;
	public float sensitivityX = 15F;
	public float sensitivityY = 15F;
		
	public float minimumX = -360F;
	public float maximumX = 360F;
		
	public float minimumY = -60F;
	public float maximumY = 60F;
		
	float rotationY = 0F;
	#endregion
	#region [init other]
	public GameObject player;
	private float lastShot;
	public float reloadTime = 0f;
	public Rigidbody bullet;
	float Bullet_Speed = 5f;


	#endregion

	void Start()
	{
		lastShot = Time.time;

		if (!networkView.isMine)
		{
			GetComponent<Camera_ControlTest>().enabled = false;
			GetComponent<AudioListener>().enabled = false;
			mCamera.gameObject.SetActive(false);
		}
	}

	void Fire_Bullet()
	{
		if (networkView.isMine)
		{
			if (Time.time > reloadTime + lastShot)
						Debug.Log ("Fired");
			{
				Rigidbody instantiatedProjectile = (Rigidbody)Instantiate( bullet, transform.position, transform.rotation );
				instantiatedProjectile.velocity = transform.forward*Bullet_Speed;
				Physics.IgnoreCollision( instantiatedProjectile.collider, player.transform.root.collider );
				lastShot = Time.time;
			}
		}
	}

	void Update ()
	{
		if (networkView.isMine)
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

			transform.position = player.transform.position;

			if (Input.GetMouseButtonDown(0)) {
					Fire_Bullet();
			}
		}
	}
}






























