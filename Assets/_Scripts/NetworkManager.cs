using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviour {
	
	//public GameObject serverBtn;
	//public GameObject hostsBtn;
	//public GameObject offlineBtn;
	public GameObject playerPrefab;
	//public Transform spawnLoc;
	public Transform spawn1;
	public Transform spawn2;
	public Transform spawn3;
	public Transform spawn4;
	public Transform spawn5;
	
	//private string gameName = "YOLO_Test_Main_Game";
	private bool refreshing = false;
	//private HostData[] hostD;
	private GameObject player;
	private static List<Transform> spawnLocations;
	private Transform randomSpawnPoint;
	private bool playOffline;

	void Start ()
	{
		playOffline = Networkmanager2.playOffline;
		spawnLocations = new List<Transform> ();
		spawnLocations.Add(spawn1);
		spawnLocations.Add(spawn2);
		spawnLocations.Add(spawn3);
		spawnLocations.Add(spawn4);
		spawnLocations.Add(spawn5);
		spawnPlayer();
	}
	
	/*public void OnServerInitialized () 
	{
		Debug.Log ("Server Initialized: " + gameName);
		spawnPlayer();
	}
	
	public void OnConnectedToServer ()
	{
		Debug.Log ("Connected to Server: " + gameName);
		spawnPlayer();
	}
	
	public void OnMasterServerEvent (MasterServerEvent MSEvent)
	{
		if (MSEvent == MasterServerEvent.RegistrationSucceeded)
		{
			Debug.Log("Server: " + gameName + " is registered!");
		}
	}*/
	
	public void spawnPlayer ()
	{
		int index = Random.Range (0, spawnLocations.Count); //Take random integer
		randomSpawnPoint = spawnLocations[index]; //Pick random spawnpoint (because of random int)
		if (playOffline)
		{
			player = Object.Instantiate (playerPrefab, randomSpawnPoint.position, Quaternion.identity) as GameObject;
		}
		else
		{
			player = Network.Instantiate (playerPrefab, randomSpawnPoint.position, Quaternion.identity, 0) as GameObject; //Instantiate player on the spawn point
			//networkView.RPC("removeSpawnPoint", RPCMode.AllBuffered, index); //Remove spawnpoint out of the list (no duplicate spawnpoints!)
		}
		//player.GetComponent<Copy_playerController>().playOffline = playOffline;
	}
	
	[RPC]
	public void removeSpawnPoint (int index)
	{
		spawnLocations.RemoveAt(index);
	}
	
	void Update () 
	{
		if (Input.GetKeyDown(KeyCode.R))
		{
			Application.LoadLevel(Application.loadedLevel);
		}
	}
}