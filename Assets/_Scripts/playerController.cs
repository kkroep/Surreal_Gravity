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
	private float speed_multiplier = 1f;

	private Vector3 TruePosition;

	public float Gravity_Shift_Time = 10f;
	
	private Quaternion before_shift;
	private Quaternion after_shift;
	private float Gravity_Shift_Counter;

	private Animator anim;

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

	public GameObject playerPrefab;
	public GameObject LightningLine;
	public GameObject KillLine;

	public Account activeAccount; //= BasicFunctions.activeAccount;
	public int playerNumber;
	private int hitCounter = 0;

	public AudioClip kill_shot_sound;
	//public AudioClip gravity_shot_sound;
	public AudioClip jump_sound;
	public static bool dontDestroy;
	public bool isAlive = true;
	public bool endGame = false;

	private NW_Spawning spawnScript;
	private bool spawnChosen = false;

	private Referee_script referee;

	private int vertexsize;
	public int VerticesPerUnit;
	public float Gibrange;
	private float multiplier;

	public float time2death = 0f;

	public float Sphere_collider_radius = 0.6f;
	private bool Can_Jump = false;
	private float JumpTime;

	#endregion

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
			anim = GetComponent<Animator> ();
			spawnScript = GameObject.FindGameObjectWithTag("SpawnTag").GetComponent<NW_Spawning>();
			JumpTime = Time.time;
		}
	}

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		if (stream.isWriting) {
			TruePosition = transform.position;
			stream.Serialize(ref TruePosition);
		} else {
			stream.Serialize(ref TruePosition);
		}
	}
	
	public void Switch_Gravity (Vector3 new_Gravity)
	{
		if (networkView.isMine || BasicFunctions.playOffline)
		{
			before_shift = transform.rotation;
			Gravity_Direction = new_Gravity;
			Vector3 New_Player_Forward_tmp = BasicFunctions.ProjectVectorOnPlane (Gravity_Direction, transform.forward);
			after_shift = Quaternion.LookRotation (New_Player_Forward_tmp, Gravity_Direction * -1f);
			Gravity_Shift_Counter = Gravity_Shift_Time;
		}
	}

	public void Fire_Grav_Bullet (Vector3 pos1, Vector3 pos2)
	{
		networkView.RPC("fireGravityLaser", RPCMode.Others, pos1, pos2, activeAccount.Number);
	}

	public void Fire_Kill_Bullet (Vector3 pos1, Vector3 pos2, int shooter)
	{
		networkView.RPC("fireKillLaser", RPCMode.Others, pos1, pos2, shooter);
	}



	[RPC]
	void fireGravityLaser(Vector3 pos1, Vector3 pos2, int number){
		LineRenderer LightningLineCurrent = (LineRenderer)Instantiate(LightningLine.GetComponent<LineRenderer>());

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

	}

	[RPC]
	void hitByBullet (int shooter, int target)
	{
		Debug.Log("Target: " + target + ", Shooter: " + shooter + ", ActiveNumber: " + activeAccount.Number);
	}

	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (!spawnScript)
			{
				spawnScript = GameObject.FindGameObjectWithTag("SpawnTag").GetComponent<NW_Spawning>();
			}

			if (Network.isServer)
			{
				dontDestroy = true;
				spawnScript.closeServerInGame();
			}
			else if (Network.isClient)
			{
				spawnScript.closeClientInGame();
			}
			else if (BasicFunctions.playOffline)
			{
				Application.LoadLevel("Menu_New");
			}
		}
	}
	
	void FixedUpdate ()
	{
		if (networkView.isMine || BasicFunctions.playOffline) {
			if (Gravity_Shift_Counter > 1f) {
					Gravity_Shift_Counter--;
					transform.rotation = Quaternion.Lerp (after_shift, before_shift, Gravity_Shift_Counter / Gravity_Shift_Time);
					rigidbody.velocity = new Vector3(0f,0f,0f);
			} else if (Gravity_Shift_Counter == 1f) {
					Gravity_Shift_Counter = 0f;
					transform.rotation = after_shift;
					rigidbody.velocity = new Vector3(0f,0f,0f);
			} else {

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
			//DIT STUK BEWEEGT DE SPELER ALLEEN ALS IE NIET DOOD IS
			if (isAlive && !endGame) {
				speed_multiplier = Mathf.Lerp (speed_multiplier, 1f, Time.fixedDeltaTime * 2f);
				Collider[] hitColliders = Physics.OverlapSphere (transform.position + Gravity_Direction*0.6f, Sphere_collider_radius);

				for (int i=0; i<hitColliders.Length; i++) {
					if (hitColliders [i].tag == "level")
					{
						Can_Jump = true;
					}
				}
				
				//HIER WORDT DE VELOCITY BEREKEND
				Vector3 New_Velocity = new Vector3(0f,0f,0f); 
				//New_Velocity = Vector3.Scale(rigidbody.velocity, Gravity_Direction);
				New_Velocity = Vector3.Lerp (Vector3.Scale(rigidbody.velocity, Vector3.Scale(Gravity_Direction,Gravity_Direction)), Gravity_Direction * Gravity_Strength, Time.fixedDeltaTime * 3f); 
				New_Velocity += transform.forward * speed * speed_multiplier * Input.GetAxis ("Vertical");
				New_Velocity += Vector3.Cross (transform.up, transform.forward) * speed * speed_multiplier * Input.GetAxis ("Horizontal");

				if ((Input.GetAxis ("Horizontal") != 0 && Can_Jump) || (Input.GetAxis ("Vertical") != 0 && Can_Jump))  {
					//anim.SetBool ("Walk", true);
					networkView.RPC("WalkAnim", RPCMode.All, BasicFunctions.activeAccount.Number, true);
				}
				
				else{
					//anim.SetBool ("Walk", false);
					networkView.RPC("WalkAnim", RPCMode.All, BasicFunctions.activeAccount.Number, false);
				}
				
				
				rigidbody.velocity = New_Velocity;				

				if (Input.GetKeyDown ("space") && isAlive && !endGame && Can_Jump && (JumpTime+0.35f)<Time.time) 
				{
					rigidbody.velocity += (Gravity_Direction * jumpSpeed * -1f);
					AudioSource.PlayClipAtPoint(jump_sound, transform.position);
					Can_Jump = false;
					JumpTime = Time.time;
					//anim.SetBool ("Jump", true);
					networkView.RPC("JumpAnim", RPCMode.All, BasicFunctions.activeAccount.Number, true);
				}
					
				else{
					//anim.SetBool ("Jump", false);
					networkView.RPC("JumpAnim", RPCMode.All, BasicFunctions.activeAccount.Number, false);
				}

			} else {
				//DIT STUK IS DE SPELER ALS IE DOOD IS
				rigidbody.velocity = new Vector3 (0f, 0f, 0f);
				if (!isAlive) {
						time2death -= Time.fixedDeltaTime;
						if (time2death <= 1f) {
							if (!spawnChosen) {
									int index = Random.Range (0, spawnScript.spawnLocations.Count - 1); //Take random integer
									Vector3 randomSpawnPoint = spawnScript.spawnLocations [index];
									transform.position = randomSpawnPoint;//new Vector3(-1f, -1f, -1f);
									spawnChosen = true;
							}
							if (time2death <= 0f) {
									networkView.RPC ("PlayerRespawn", RPCMode.All, transform.position);
									spawnChosen = false;
							}
						}
				} else {
				}
			}
	} else {
			//Control of other player
			transform.position = Vector3.Lerp(transform.position, TruePosition, Time.fixedDeltaTime * 5f);
		}
	}

	[RPC]
	void PlayerRespawn(Vector3 SpawnPOsition){
		isAlive = true;
		transform.position  = SpawnPOsition;
		gameObject.GetComponent<MeshRenderer> ().enabled = true;
		gameObject.GetComponent<SphereCollider> ().enabled = true;
	}

	[RPC]
	void WalkAnim (int player, bool set)
	{
		referee.players[player-1].anim.SetBool("Walk", set);
	}

	[RPC]
	void JumpAnim (int player, bool set)
	{
		referee.players[player-1].anim.SetBool("Jump", set);
	}

	void OnCollisionStay (Collision collisionInfo)
	{
		if (networkView.isMine && !BasicFunctions.playOffline)
		{
			if (collisionInfo.gameObject.tag == "DeathBoundary") {
				if(!referee){
					referee = (GameObject.FindGameObjectsWithTag("Referee_Tag"))[0].GetComponent<Referee_script>();
				}
				referee.fragged (BasicFunctions.activeAccount.Number);
			}
		}
	}
}