using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviour {

	public GameObject serverBtn;
	public GameObject hostsBtn;
	public GameObject offlineBtn;
	public GameObject playerPrefab;
	//public Transform spawnLoc;
	public Transform spawn1;
	public Transform spawn2;
	public Transform spawn3;
	public Transform spawn4;
	public Transform spawn5;
	public bool playOffline = false;

	private string gameName = "YOLO_Test_Main_Game";
	private bool refreshing = false;
	private HostData[] hostD;
	private GameObject player;
	private static List<Transform> spawnLocations;
	private Transform randomSpawnPoint;

	void Start ()
	{
		spawnLocations = new List<Transform> ();
		spawnLocations.Add(spawn1);
		spawnLocations.Add(spawn2);
		spawnLocations.Add(spawn3);
		spawnLocations.Add(spawn4);
		spawnLocations.Add(spawn5);
	}

	public void startServer ()
	{
		bool NAT = !Network.HavePublicAddress();
		Network.InitializeServer (4, 25001, NAT); //Initialiseer Server; max connecties  = 4, port = 25001 
		MasterServer.RegisterHost (gameName, "Multiplayer_Test", "Trying to implement Multiplayer"); //Registreer de Server
	}

	public void refreshHost ()
	{
		MasterServer.RequestHostList (gameName);
		refreshing = true;
	}

	public void playOfflineFunction ()
	{
		playOffline = true;
		spawnPlayer();
	}

	public void OnServerInitialized () 
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
	}

	public void spawnPlayer ()
	{
		int index = Random.Range (0, spawnLocations.Count); //Take random integer
		randomSpawnPoint = spawnLocations[index]; //Pick random spawnpoint (because of random int)
		if (playOffline)
		{
			player = /*(GameObject)*/ Object.Instantiate (playerPrefab, randomSpawnPoint.position, Quaternion.identity) as GameObject;
			Debug.Log(randomSpawnPoint);
		}
		else
		{
			player = /*(GameObject)*/ Network.Instantiate (playerPrefab, randomSpawnPoint.position, Quaternion.identity, 0) as GameObject; //Instantiate player on the spawn point
			networkView.RPC("removeSpawnPoint", RPCMode.AllBuffered, index); //Remove spawnpoint out of the list (no duplicate spawnpoints!)
		}
		player.GetComponent<Copy_playerController>().playOffline = playOffline;
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

		if (refreshing)
		{
			if (MasterServer.PollHostList ().Length > 0)
			{
				refreshing = false;
				Debug.Log (MasterServer.PollHostList ().Length);
				hostD = MasterServer.PollHostList ();
			}
		}

		if (Network.isClient || Network.isServer || playOffline)
		{
			serverBtn.SetActive(false);
			hostsBtn.SetActive(false);
			offlineBtn.SetActive(false);
		}
	}

	void OnGUI ()
	{
		GUI.backgroundColor = Color.cyan;
		GUI.contentColor = Color.black;
		if (!Network.isClient && !Network.isServer)
		{
			if (hostD != null)
			{
				for (int i = 0; i < hostD.Length; i++)
				{
					if (GUI.Button(new Rect(Screen.width/4, Screen.height/10 + (i * 100), Screen.width*0.1f, Screen.height*0.05f), hostD[i].gameName))
					{
						Network.Connect(hostD[i]);
					}
				}
			}
		}
	}
}
