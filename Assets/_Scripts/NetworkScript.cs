using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NetworkScript : MonoBehaviour {

	//public GameObject buttonPrefab;
	//public Transform parent;
	public GameObject ServerBtn;
	public GameObject HostsBtn;
	public GameObject ShutDBtn;
	public GameObject DiscBtn;
	public GameObject playerPrefab;
	public Transform spawnLocation1;
	public Transform spawnLocation2;

	private string gameName = "SurrGrav_Test_Networking";
	private bool refreshing = false;
	private HostData[] hostD;
	//private int amountPlayers = 0;

	public void startServer ()
	{
		bool NAT = !Network.HavePublicAddress();
		Network.InitializeServer (4, 25001, NAT);
		MasterServer.RegisterHost (gameName, "Testing_Game", "Game to test Networking");
	}

	public void refreshHost ()
	{
		MasterServer.RequestHostList (gameName);
		refreshing = true;
		Debug.Log("Refreshing Hosts");
	}

	public void shutDownServer ()
	{
		Network.Disconnect ();
		MasterServer.UnregisterHost();
	}

	public void disconnectFromServer ()
	{
		Network.Disconnect ();
	}

	//public void showHosts ()
	//{
	//	for (int i = 0; i < hostD.Length; i++)
	//	{
	//		GameObject button = (GameObject)Instantiate (buttonPrefab, new Vector3(0, 50 * i, 0), Quaternion.identity);
	//		button.GetComponentInChildren<Text> ().text = hostD[i].gameName;
	//		int index = i;
	//		button.GetComponent<Button> ().onClick.AddListener (
	//			() => {Debug.Log("Connected to " + hostD[index].gameName);}
	//		);
	//		button.transform.parent = parent;
	//	}
	//}

	void Update ()
	{
		if (refreshing)
		{
			if (MasterServer.PollHostList ().Length > 0)
			{
				refreshing = false;
				Debug.Log (MasterServer.PollHostList ().Length);
				hostD = MasterServer.PollHostList ();
			}
		}

		if (Network.isClient || Network.isServer)
		{
			ServerBtn.SetActive(false);
			HostsBtn.SetActive(false);
			if (Network.isServer)
				ShutDBtn.SetActive(true);
			else
				DiscBtn.SetActive(true);
		}

		if (!Network.isClient && !Network.isServer)
		{
			ServerBtn.SetActive(true);
			HostsBtn.SetActive(true);
			ShutDBtn.SetActive(false);
			DiscBtn.SetActive(false);
		}

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (Network.isServer)
			{
				shutDownServer ();
			}
			if (Network.isClient)
			{
				disconnectFromServer ();
			}
		}
	}

	public void spawnPlayer ()
	{
		if (Network.isServer)
		{
			Network.Instantiate (playerPrefab, spawnLocation1.position, Quaternion.identity, 0);
		}
		else if (Network.isClient)
		{
			Network.Instantiate (playerPrefab, spawnLocation2.position, Quaternion.identity, 0);
		}
	}

	public void OnServerInitialized () 
	{
		Debug.Log ("Server Initialized");
		//amountPlayers = 1;
		spawnPlayer ();
	}

	public void OnConnectedToServer ()
	{
		//amountPlayers += 1;
		spawnPlayer ();
	}

	public void OnMasterServerEvent (MasterServerEvent MSEvent)
	{
		if (MSEvent == MasterServerEvent.RegistrationSucceeded)
		{
			Debug.Log("Server is registered!");
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
					if (GUI.Button(new Rect(Screen.width/5, Screen.height/10 + (i * 100), Screen.width*0.1f, Screen.height*0.05f), hostD[i].gameName))
					{
						Network.Connect(hostD[i]);
					}
				}
			}
		}
	}

}
