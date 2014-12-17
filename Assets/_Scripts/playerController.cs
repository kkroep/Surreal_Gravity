using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class playerController : MonoBehaviour
{
	#region [init look around]
	public float speed;
	public float jumpSpeed = 8.0F; 
	private Vector3 moveDirection = Vector3.zero;
	private Vector3 Current_Global_Force;
	public Camera_Control Main_Camera;
	public bool isAlive = true;


	public float Gravity_Shift_Time = 10f;
	
	private Quaternion before_shift;
	private Quaternion after_shift;
	private float Gravity_Shift_Counter;

//	private Animator anim;

	public enum RotationAxes
	{
		MouseXAndY = 0,
		MouseX = 1,
		MouseY = 2 }
	;
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
	public Rigidbody Kill_Bullet;

	public float Bullet_Speed = 15f;

	#endregion

	public GameObject playerPrefab;
	public GameObject LightningLine;
	public GameObject KillLine;

	public Account activeAccount; //= BasicFunctions.activeAccount;
	public int playerNumber;
	private int hitCounter = 0;

	public AudioClip kill_shot_sound;
	//public AudioClip gravity_shot_sound;
	public AudioClip jump_sound;

	//new
	private int vertexsize;
	public int VerticesPerUnit;
	public float Gibrange;
	private float multiplier;

	public float time2death = 0f;

	public float Sphere_collider_radius = 0.6f;
	//\new
	
	void Start ()
	{
		if (networkView.isMine || BasicFunctions.playOffline)
		{
			if (!BasicFunctions.playOffline)
			{
				activeAccount.Number = playerNumber;
			}
			//activeAccount.Points = 0;
			Screen.lockCursor = true;
			rigidbody.freezeRotation = true;
			Gravity_Direction = Initial_Gravity_Direction;
			Current_Global_Force = Gravity_Direction * Gravity_Strength;
			//anim = GetComponent<Animator> ();
		}
	}
	
	public void Switch_Gravity (Vector3 new_Gravity)
	{
		if (networkView.isMine || BasicFunctions.playOffline)
		{
			//AudioSource.PlayClipAtPoint(gravity_shot_sound, transform.position);
			before_shift = transform.rotation;
			Gravity_Direction = new_Gravity;
			Vector3 New_Player_Forward_tmp = BasicFunctions.ProjectVectorOnPlane (Gravity_Direction, transform.forward);
			after_shift = Quaternion.LookRotation (New_Player_Forward_tmp, Gravity_Direction * -1f);
			Gravity_Shift_Counter = Gravity_Shift_Time;
		}
	}

	/*void Fire_Kill_Bullet()
	{
		if (BasicFunctions.playOffline)
		{
			//if (Time.time > reloadTime + lastShot)
			//Debug.Log ("Fired");
			//{
				AudioSource.PlayClipAtPoint(kill_shot_sound, transform.position);
				Rigidbody instantiatedProjectile = (Rigidbody)Instantiate( Kill_Bullet, transform.position, transform.rotation );
				instantiatedProjectile.velocity = Main_Camera.transform.forward*Bullet_Speed;
				Physics.IgnoreCollision( instantiatedProjectile.collider, gameObject.transform.root.collider );
				//lastShot = Time.time;
			//}
		}
		else if (networkView.isMine)
		{
			//if (Time.time > reloadTime + lastShot)
				//Debug.Log("Fired");
			//{
				AudioSource.PlayClipAtPoint(kill_shot_sound, transform.position);
				Rigidbody instantiatedProjectile = (Rigidbody)Instantiate( Kill_Bullet, transform.position, transform.rotation );
				int shootNumber = BasicFunctions.activeAccount.Number;
				instantiatedProjectile.GetComponent<Bullet_Controller>().shooterNumber = shootNumber;
				Debug.Log ("Shooter: " + shootNumber);
				instantiatedProjectile.velocity = Main_Camera.transform.forward*Bullet_Speed;
				Physics.IgnoreCollision( instantiatedProjectile.collider, gameObject.transform.root.collider );
				//lastShot = Time.time;
				networkView.RPC("fireKillBulletS", RPCMode.Others, gameObject.networkView.viewID, transform.position, transform.rotation, Main_Camera.transform.forward, shootNumber);
			//}
		}
	}*/

	public void Fire_Grav_Bullet (Vector3 pos1, Vector3 pos2)
	{
		networkView.RPC("fireGravityLaser", RPCMode.Others, pos1, pos2, activeAccount.Number);
	}

	public void Fire_Kill_Bullet (Vector3 pos1, Vector3 pos2, int shooter)
	{
		networkView.RPC("fireKillLaser", RPCMode.Others, pos1, pos2, shooter);
	}

	/*[RPC]
	void fireKillBulletS(NetworkViewID player, Vector3 pos, Quaternion rot, Vector3 dir, int number)
	{
		NetworkView playerN = NetworkView.Find (player);
		Transform cloneP = playerN.transform;
		Rigidbody instantiatedProjectileN = (Rigidbody)Instantiate( Kill_Bullet, pos, rot );
		instantiatedProjectileN.GetComponent<Bullet_Controller>().shooterNumber = number;
		Debug.Log ("Shooter: " + number);
		instantiatedProjectileN.velocity = dir*Bullet_Speed;
		Physics.IgnoreCollision( instantiatedProjectileN.collider, cloneP.root.collider );
	}*/

	[RPC]
	void fireGravityLaser(Vector3 pos1, Vector3 pos2, int number){
		LineRenderer LightningLineCurrent = (LineRenderer)Instantiate(LightningLine.GetComponent<LineRenderer>());
		//LightningLineCurrent.SetPosition(1, pos1);
		//LightningLineCurrent.SetPosition(0, pos2);
		
		//new
		float Distance = Mathf.Sqrt((pos1 - pos2).sqrMagnitude);
		float floatvertexsize = VerticesPerUnit * Distance;
		vertexsize = (int) floatvertexsize;
		if ( vertexsize > 30000 ){
			vertexsize = 30000;
		}
		LightningLineCurrent.SetVertexCount(vertexsize);
		LightningLineCurrent.SetPosition(vertexsize-1, pos1+new Vector3(0.01f,-0.01f,0.01f));
		for(int i=1; i<(vertexsize-1) ;i++)
		{
			float multiplier = ((i*1.0f)/(vertexsize-1));
			LightningLineCurrent.SetPosition(i, (multiplier*(pos1 - pos2)) + pos2 + new Vector3 (Random.Range(-Gibrange, Gibrange),Random.Range(-Gibrange, Gibrange),Random.Range(-Gibrange, Gibrange)));			
		}
		LightningLineCurrent.SetPosition(0, pos2);
		//\new	
		
	}

	[RPC]
	void fireKillLaser(Vector3 pos1, Vector3 pos2, int Pnumber){
		AudioSource.PlayClipAtPoint(kill_shot_sound, transform.position);
		LineRenderer KillLineCurrent = (LineRenderer)Instantiate(KillLine.GetComponent<LineRenderer>());
		KillLineCurrent.GetComponent<Gravity_trace_script>().shooterNumber = Pnumber;
		//KillLineCurrent.SetPosition(1, pos1);
		//KillLineCurrent.SetPosition(0, pos2);
		
		//new
		float Distance = Mathf.Sqrt((pos1 - pos2).sqrMagnitude);
		float floatvertexsize = VerticesPerUnit * Distance;
		vertexsize = (int) floatvertexsize;
		if ( vertexsize > 30000 ){
			vertexsize = 30000;
		}
		KillLineCurrent.SetVertexCount(vertexsize);
		KillLineCurrent.SetPosition(vertexsize-1, pos1);
		for(int i=1; i<(vertexsize-1) ;i++)
		{
			float multiplier = ((i*1.0f)/(vertexsize-1));
			KillLineCurrent.SetPosition(i, (multiplier*(pos1 - pos2)) + pos2 + new Vector3 (Random.Range(-Gibrange, Gibrange),Random.Range(-Gibrange, Gibrange),Random.Range(-Gibrange, Gibrange)));			
		}
		KillLineCurrent.SetPosition(0, pos2);
		//\new
		
		//Debug.Log("Shooter: " + KillLineCurrent.GetComponent<Gravity_trace_script>().shooterNumber);
	}

	[RPC]
	void hitByBullet (int shooter, int target)
	{
		Debug.Log("Target: " + target + ", Shooter: " + shooter + ", ActiveNumber: " + activeAccount.Number);
		//Debug.Log(shooter + " has shot " + target);
	}

	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (!BasicFunctions.playOffline)
			{
				Network.Disconnect();
			}
		    Application.LoadLevel("Menu");
			Screen.lockCursor = false;
		}
		/*if (Input.GetMouseButtonDown(0)) {
			Fire_Kill_Bullet();
		}*/
	}
	
	void FixedUpdate ()
	{
		if (networkView.isMine || BasicFunctions.playOffline) {
				if (Gravity_Shift_Counter > 1f) {
						Gravity_Shift_Counter--;
						transform.rotation = Quaternion.Lerp (after_shift, before_shift, Gravity_Shift_Counter / Gravity_Shift_Time);
				} else if (Gravity_Shift_Counter == 1f) {
						Gravity_Shift_Counter = 0f;
						transform.rotation = after_shift;
				} else {
						//transform.rotation = Quaternion.LookRotation(transform.forward, -1f*Gravity_Direction);
		
						#region [look around]
						if (axes == RotationAxes.MouseXAndY) {
								float rotationX = transform.localEulerAngles.y + Input.GetAxis ("Mouse X") * sensitivityX;
			
								rotationY += Input.GetAxis ("Mouse Y") * sensitivityY;
								rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
			
								transform.localEulerAngles = new Vector3 (-rotationY, rotationX, 0);
						} else if (axes == RotationAxes.MouseX) {
								transform.Rotate (0, Input.GetAxis ("Mouse X") * sensitivityX, 0);
						} else {
								rotationY += Input.GetAxis ("Mouse Y") * sensitivityY;
								rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
			
								transform.localEulerAngles = new Vector3 (-rotationY, transform.localEulerAngles.y, 0);
						}
				}
				#endregion
	
				transform.TransformDirection (Vector3.forward);
				if (isAlive) {
					float speed_multiplier = 1f;
					Collider[] hitColliders = Physics.OverlapSphere(transform.position, Sphere_collider_radius);

				for(int i=0; i<hitColliders.Length; i++){
					if(hitColliders[i].tag=="level")
						speed_multiplier = 3f;
				}

				rigidbody.velocity = transform.forward * speed * speed_multiplier* Input.GetAxis ("Vertical");
				rigidbody.velocity += Vector3.Cross (transform.up, transform.forward) * speed * speed_multiplier * Input.GetAxis ("Horizontal");
					Current_Global_Force = Vector3.Lerp (Current_Global_Force, Gravity_Direction * Gravity_Strength, Time.fixedDeltaTime * 4f); 
					rigidbody.AddForce (Current_Global_Force);
				}else{
				rigidbody.velocity=new Vector3(0f,0f,0f);
				time2death-=Time.fixedDeltaTime;
				if(time2death<=0f){
					networkView.RPC("PlayerRespawn", RPCMode.All);
				}
			}
		}
	}

	[RPC]
	void PlayerRespawn(){
		isAlive = true;
		transform.position  = new Vector3(-1f, -1f, -1f);
		gameObject.GetComponent<MeshRenderer> ().enabled = true;
		gameObject.GetComponent<SphereCollider> ().enabled = true;
	}

	void OnCollisionStay (Collision collisionInfo)
	{
		if (networkView.isMine || BasicFunctions.playOffline)
		{
			if (Input.GetKeyDown ("space") && isAlive) 
			{
				Current_Global_Force = (Gravity_Direction * jumpSpeed * -1f);
				AudioSource.PlayClipAtPoint(jump_sound, transform.position);
			//	anim.SetBool ("Jump",true);
			}
			
			//else{
			//	anim.SetBool ("Jump",false);
			//}
		}
		if (collisionInfo.gameObject.tag == "Kill_Bullet") {
			//Debug.Log("Target: " + BasicFunctions.accountNumbers[(BasicFunctions.activeAccount.Number-1)]);
			int targetNumber = activeAccount.Number;
			Debug.Log("Target: " + targetNumber + ", Shooter: " + collisionInfo.gameObject.GetComponent<Bullet_Controller>().shooterNumber + ", ActiveNumber: " + activeAccount.Number);
			networkView.RPC("hitByBullet", RPCMode.Server, collisionInfo.gameObject.GetComponent<Bullet_Controller>().shooterNumber, targetNumber);//BasicFunctions.accountNumbers[(BasicFunctions.activeAccount.Number-1)]);
			Destroy(collisionInfo.gameObject);
		}
	}
}