using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NetworkScript : MonoBehaviour {

	//public GameObject buttonPrefab;
	//public Transform parent;
	public GameObject ServerBtn;
	public GameObject HostsBtn;
	public GameObject playerPrefab;
	public Transform spawnLocation;

	private string gameName = "SurrGrav_Test_Networking";
	private bool refreshing = false;
	private HostData[] hostD;

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
		}

		if (!Network.isClient && !Network.isServer)
		{
			ServerBtn.SetActive(true);
			HostsBtn.SetActive(true);
		}
	}

	public void spawnPlayer ()
	{
		Network.Instantiate (playerPrefab, spawnLocation.position, Quaternion.identity, 0);
	}

	public void OnServerInitialized () 
	{
		Debug.Log ("Server Initialized");
		spawnPlayer ();
	}

	public void OnConnectedToServer ()
	{
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
					if (GUI.Button(new Rect(Screen.width/4, Screen.height/10 + (i * 100), Screen.width*0.1f, Screen.height*0.05f), hostD[i].gameName))
					{
						Network.Connect(hostD[i]);
					}
				}
			}
		}
	}

}
