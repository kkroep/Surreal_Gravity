using UnityEngine;
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
public class Camera_Control : MonoBehaviour {
	
	#region [init for look around]
	public LineRenderer LigntingLine;
	
	public Texture2D crosshairImage;
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
	public playerController player;
	public GameObject playercam;
	private float lastShot;
	public float reloadTime = 0f;
	public Rigidbody Gravity_Bullet;
	public Rigidbody Kill_Bullet;
	public float Bullet_Speed = 5f;
	public float Gravity_Switch_Timer= 0f;
	
	#endregion
	
	void Start()
	{
		if (networkView.isMine || BasicFunctions.playOffline)
		{
			lastShot = Time.time;
			transform.rotation = player.transform.rotation;
		}
		else
		{
			GetComponent<Camera_Control>().enabled = false;
			GetComponent<AudioListener>().enabled = false;
			playercam.SetActive(false);
		}
	}
	
	void Fire_Kill_Bullet()
	{
		/*if (networkView.isMine)
		{
			if (Time.time > reloadTime + lastShot)
			{
				//Rigidbody instantiatedProjectile = Network.Instantiate( Kill_Bullet, transform.position, transform.rotation, 0 ) as Rigidbody;
				//Physics.IgnoreCollision( instantiatedProjectile.collider, player.transform.root.collider );
				//lastShot = Time.time;
				if (!Network.isServer)
					networkView.RPC("fireKillBulletS", RPCMode.Server, transform.position); //instantiatedProjectile.networkView.viewID, transform.forward);
				else
				{
					Rigidbody instantiatedProjectile = Object.Instantiate( Kill_Bullet, transform.position, transform.rotation ) as Rigidbody;
					instantiatedProjectile.velocity = transform.forward*Bullet_Speed;
					Physics.IgnoreCollision( instantiatedProjectile.collider, player.transform.root.collider );
					lastShot = Time.time;
					networkView.RPC("fireKillBulletC", RPCMode.Others, transform.position, transform.forward);
				}
			}
		}
		else*/ if (BasicFunctions.playOffline)
		{
			if (Time.time > reloadTime + lastShot)
				Debug.Log ("Fired");
			{
				Rigidbody instantiatedProjectile = (Rigidbody)Instantiate( Kill_Bullet, transform.position, transform.rotation );
				instantiatedProjectile.velocity = transform.forward*Bullet_Speed;
				Physics.IgnoreCollision( instantiatedProjectile.collider, player.transform.root.collider );
				lastShot = Time.time;
			}
		}
	}

	[RPC]
	void fireKillBulletS(Vector3 startPos)
	{
		//NetworkView bulletN = NetworkView.Find(id);
		//Rigidbody cloneB = bulletN.rigidbody;
		//if (cloneB != null)
		//{
		//	cloneB.velocity = dir*Bullet_Speed;
		//}
		Rigidbody instantiatedProjectile = Object.Instantiate( Kill_Bullet, startPos, transform.rotation ) as Rigidbody;
		instantiatedProjectile.velocity = transform.forward*Bullet_Speed;
		Physics.IgnoreCollision( instantiatedProjectile.collider, player.transform.root.collider );
		lastShot = Time.time;
		networkView.RPC("fireKillBulletC", RPCMode.Others, startPos, transform.forward);
	}

	[RPC]
	void fireKillBulletC(Vector3 startPos, Vector3 dir)
	{
		Rigidbody instantiatedProjectile = Object.Instantiate( Kill_Bullet, startPos, transform.rotation ) as Rigidbody;
		instantiatedProjectile.velocity = dir*Bullet_Speed;
		Physics.IgnoreCollision( instantiatedProjectile.collider, player.transform.root.collider );
		lastShot = Time.time;
	}
	
	void Fire_Gravity_Bullet()
	{
		if (networkView.isMine || BasicFunctions.playOffline)
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width/2f, Screen.height/2f));
			if (Physics.Raycast(ray, out hit)) {
				Vector3 incomingVec = hit.point - transform.position;
				LigntingLine.SetPosition(1, transform.position+new Vector3(0.01f,-0.01f,0.01f));
				LigntingLine.SetPosition(0, hit.point);
				player.Switch_Gravity(hit.normal*-1f);
			}
		}
	}
	
	void Update ()
	{
		if (networkView.isMine || BasicFunctions.playOffline)
		{
			if(Gravity_Switch_Timer>0f)
			{
				
			}else
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
			}
			#endregion
			
			transform.position = player.transform.position;
			
			if (Input.GetMouseButtonDown(1)) {
				Fire_Gravity_Bullet();
			}
			if (Input.GetMouseButtonDown(0)) {
				Fire_Kill_Bullet();
			}
		}
	}
	
	void OnGUI()
	{
		if (networkView.isMine || BasicFunctions.playOffline)
		{
			float xMin = (Screen.width / 2) - (crosshairImage.width / 2);
			float yMin = (Screen.height / 2) - (crosshairImage.height / 2);
			GUI.DrawTexture(new Rect(xMin, yMin, crosshairImage.width, crosshairImage.height), crosshairImage);
		}
	}
	
	void Gravity_Switch(){
		
	}
}