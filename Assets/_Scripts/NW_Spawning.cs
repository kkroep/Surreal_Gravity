using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NW_Spawning : MonoBehaviour {

	public GameObject playerPrefab;
	public Transform spawn1;
	public Transform spawn2;
	public Transform spawn3;
	public Transform spawn4;
	public Transform spawn5;
	
	private static List<Transform> spawnLocations;
	private GameObject player;
	private Transform randomSpawnPoint;
	private bool refreshing = false;
	private bool playOffline;
	
	void Start ()
	{
		playOffline = NW_Server.playOffline;
		spawnLocations = new List<Transform> ();
		spawnLocations.Add(spawn1);
		spawnLocations.Add(spawn2);
		spawnLocations.Add(spawn3);
		spawnLocations.Add(spawn4);
		spawnLocations.Add(spawn5);
		spawnPlayer();
	}
	
	public void spawnPlayer ()
	{
		int index = Random.Range (0, spawnLocations.Count-1); //Take random integer
		randomSpawnPoint = spawnLocations[index]; //Pick random spawnpoint (because of random int)
		Debug.Log(randomSpawnPoint);
		if (playOffline)
		{
			Object.Instantiate (playerPrefab, randomSpawnPoint.position, Quaternion.identity);
		}
		else
		{
			Network.Instantiate (playerPrefab, randomSpawnPoint.position, Quaternion.identity, 0); //Instantiate player on the spawn point
			networkView.RPC("removeSpawnPoint", RPCMode.AllBuffered, index); //Remove spawnpoint out of the list (no duplicate spawnpoints!)
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