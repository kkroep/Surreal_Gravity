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
	public GameObject LightningLine;
	public GameObject KillLine;

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
	public Referee_script referee;
	public float reloadTime = 0f;
	public float Bullet_Speed = 5f;
	public float Gravity_Switch_Timer= 0f;
	public Rigidbody Gravity_Bullet;

	public AudioClip kill_shot_sound;
	public AudioClip gravity_shot_sound;

	private Account activeAccount = BasicFunctions.activeAccount;
	private float lastShot;

	//public AudioClip kill_point_sound;
	//public AudioClip gravity_switch_sound;

	//new
	private int vertexsize;
	public int VerticesPerUnit;
	public float Gibrange;
	private float multiplier;
	//\new
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
			//GetComponent<Camera_Control>().enabled = false;
			GetComponent<AudioListener>().enabled = false;
			playercam.SetActive(false);
		}
	}
	
	/*void Fire_Kill_Bullet()
	{
		if (networkView.isMine)
		{
			if (Time.time > reloadTime + lastShot)
				Debug.Log("Fired");
			{
				//Rigidbody instantiatedProjectile = (Rigidbody)Instantiate( Kill_Bullet, transform.position, transform.rotation );
				Rigidbody instantiatedProjectile = (Rigidbody)Network.Instantiate( Kill_Bullet, transform.position, transform.rotation, 2 );
				instantiatedProjectile.velocity = transform.forward*Bullet_Speed;
				Physics.IgnoreCollision( instantiatedProjectile.collider, player.transform.root.collider );
				lastShot = Time.time;
				networkView.RPC("fireKillBulletS", RPCMode.Others, transform.position, transform.forward); //instantiatedProjectile.networkView.viewID
			}
		}
		else if (BasicFunctions.playOffline)
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
	void fireKillBulletS(Vector3 pos, Vector3 dir)
	{
		NetworkView bulletN = NetworkView.Find(id);
		Rigidbody cloneB = bulletN.rigidbody;
		if (cloneB != null)
		{
			cloneB.velocity = dir*Bullet_Speed;
		}
		//networkView.RPC("fireKillBulletC", RPCMode.Others, startPos, dir);
		//Rigidbody instantiatedProjectileN = (Rigidbody)Instantiate( Kill_Bullet, pos, transform.rotation );
		Rigidbody instantiatedProjectileN = (Rigidbody)Network.Instantiate( Kill_Bullet, pos, transform.rotation, 2 );
		instantiatedProjectileN.velocity = dir*Bullet_Speed;
		Physics.IgnoreCollision( instantiatedProjectileN.collider, player.transform.root.collider );
		lastShot = Time.time;
	}
	
	/*[RPC]
	void fireKillBulletC(Vector3 startPos, Vector3 dir)
	{
		GameObject instantiatedProjectile = Object.Instantiate( Kill_Bullet, startPos, transform.rotation ) as GameObject;
		instantiatedProjectile.rigidbody.velocity = dir*Bullet_Speed;
		Physics.IgnoreCollision( instantiatedProjectile.rigidbody.collider, player.transform.root.collider );
		lastShot = Time.time;
	}*/
	
	void Fire_Gravity_Bullet()
	{
		if (networkView.isMine || BasicFunctions.playOffline)
		{
			AudioSource.PlayClipAtPoint(gravity_shot_sound, transform.position);
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width/2f, Screen.height/2f));
			if (Physics.Raycast(ray, out hit)) {
				Vector3 incomingVec = hit.point - transform.position;
				LineRenderer LightningLineCurrent = (LineRenderer)Instantiate(LightningLine.GetComponent<LineRenderer>());
				//LightningLineCurrent.SetPosition(1, transform.position+new Vector3(0.01f,-0.01f,0.01f));
				//LightningLineCurrent.SetPosition(0, hit.point);
				
				//\new
				float Distance = Mathf.Sqrt((transform.position - hit.point).sqrMagnitude);
				float floatvertexsize = VerticesPerUnit * Distance;
				vertexsize = (int) floatvertexsize;
				if ( vertexsize > 30000 ){
					vertexsize = 30000;
				}
				LightningLineCurrent.SetVertexCount(vertexsize);
				LightningLineCurrent.SetPosition(vertexsize-1, transform.position+new Vector3(0.01f,-0.01f,0.01f));
				for(int i=1; i<(vertexsize-1) ;i++)
				{
					float multiplier = ((i*1.0f)/(vertexsize-1));
					LightningLineCurrent.SetPosition(i, (multiplier*(transform.position - hit.point)) + hit.point + new Vector3 (Random.Range(-Gibrange, Gibrange),Random.Range(-Gibrange, Gibrange),Random.Range(-Gibrange, Gibrange)));			
				}
				LightningLineCurrent.SetPosition(0, hit.point);
				//\new
				
				if (!BasicFunctions.playOffline)
					player.Fire_Grav_Bullet(transform.position+new Vector3(0.01f,-0.01f,0.01f),hit.point);
				if(hit.collider.tag=="level"){
				//AudioSource.PlayClipAtPoint(gravity_switch_sound, transform.position);
				player.Switch_Gravity(hit.normal*-1f);
				}
			}
		}
	}

	void Fire_Kill_Laser()
	{
		if (networkView.isMine || BasicFunctions.playOffline)
		{
			AudioSource.PlayClipAtPoint(kill_shot_sound, transform.position);
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width/2f, Screen.height/2f));
			if (Physics.Raycast(ray, out hit)) {
				//Debug.Log(hit.collider.tag);
				Vector3 incomingVec = hit.point - transform.position;
				LineRenderer KillLineCurrent = (LineRenderer)Instantiate(KillLine.GetComponent<LineRenderer>());
				//KillLineCurrent.SetPosition(1, transform.position+new Vector3(0.01f,-0.01f,0.01f));
				//KillLineCurrent.SetPosition(0, hit.point);
				
				//new
				float Distance = Mathf.Sqrt((transform.position - hit.point).sqrMagnitude);
				float floatvertexsize = VerticesPerUnit * Distance;
				vertexsize = (int) floatvertexsize;
				if ( vertexsize > 30000 ){
					vertexsize = 30000;
				}
				KillLineCurrent.SetVertexCount(vertexsize);
				KillLineCurrent.SetPosition(vertexsize-1, transform.position+new Vector3(0.01f,-0.01f,0.01f));
				for(int i=1; i<(vertexsize-1) ;i++)
				{
					float multiplier = ((i*1.0f)/(vertexsize-1));
					KillLineCurrent.SetPosition(i, (multiplier*(transform.position - hit.point)) + hit.point + new Vector3 (Random.Range(-Gibrange, Gibrange),Random.Range(-Gibrange, Gibrange),Random.Range(-Gibrange, Gibrange)));			
				}
				KillLineCurrent.SetPosition(0, hit.point);
				//\new
				
				int shootNumber = activeAccount.Number;
				KillLineCurrent.GetComponent<Gravity_trace_script>().shooterNumber = shootNumber;
				//Debug.Log("Shooter: " + KillLineCurrent.GetComponent<Gravity_trace_script>().shooterNumber);
				if (!BasicFunctions.playOffline)
					player.Fire_Kill_Bullet(transform.position+new Vector3(0.01f,-0.01f,0.01f),hit.point, shootNumber);
				if(hit.collider.tag=="Player")
				{
					//AudioSource.PlayClipAtPoint (kill_point_sound, transform.position);
					if(!referee){
						referee = (GameObject.FindGameObjectsWithTag("Referee_Tag"))[0].GetComponent<Referee_script>();
					}
					referee.frag(KillLineCurrent.GetComponent<Gravity_trace_script>().shooterNumber, hit.collider.gameObject.GetComponent<playerController>().playerNumber);
				}
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
			
			if (Input.GetMouseButtonDown(1) && player.isAlive && !player.endGame) {
				Fire_Gravity_Bullet();
			}

			if (Input.GetMouseButtonDown(0) && player.isAlive && !player.endGame && !BasicFunctions.ForkModus) {
				Fire_Kill_Laser();
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