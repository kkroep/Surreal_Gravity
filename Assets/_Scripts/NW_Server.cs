using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class NW_Server : MonoBehaviour {

	public MenuButtons menuBtns;

	public TextMesh p1;
	public TextMesh p2;
	public TextMesh p3;
	public TextMesh p4;
	public TextMesh p1c;
	public TextMesh p2c;
	public TextMesh p3c;
	public TextMesh p4c;
	public Text yolo; 

	private string gameTypeName = "Surreal_Gravity: The Game";
	private string gameName;

	public bool connected = false;

	public static bool showServers;
	private static int amountPlayers;

	private bool ClientConn = false;
	private bool ClientUpdate = false;
	private bool refreshing = false;
	private float timer = 0.5f;
	private float refreshT = 2f;
	private int serverPort;
	private int index;
	private bool xx = false;

	private HostData[] hostD;
	private List<string> activeAccounts = new List<string>();
	private List<Account> connectedPlayers = new List<Account>(); //Alleen voor de server
	private List<int> accountNumbers = new List<int>();
	private static List<int> serverPorts;

	void Start ()
	{
		showServers = false;
		serverPorts = new List<int> ();
		serverPorts.Add (25001);
		serverPorts.Add (25002);
		serverPorts.Add (25003);
		/*serverPorts.Add (25004);
		serverPorts.Add (250005);
		serverPorts.Add (250006);
		serverPorts.Add (250007);
		serverPorts.Add (250008);
		serverPorts.Add (250009);
		serverPorts.Add (250010);*/
	}

	public void startServer ()
	{
		BasicFunctions.serverAccount = BasicFunctions.activeAccount;
		gameName = BasicFunctions.serverAccount.Name + "'s Server";
		index = Random.Range (0, serverPorts.Count); //Take random integer
		serverPort = serverPorts[index]; //Pick random spawnpoint (because of random int)

		if (!xx)
			yolo.text = "PORT: " + serverPort;
		startServer2 ();
	}

	public void startServer2 ()
	{
		bool NAT = !Network.HavePublicAddress();
		Network.InitializeServer (4, serverPort, NAT); //Initialiseer Server; max connecties  = 4, port = 25001
		MasterServer.RegisterHost (gameTypeName, gameName, "Implementing Multiplayer in the Menu"); //Registreer de Server
	}

	void OnFailedToConnectToMasterServer ()
	{
		xx = true;
		serverPorts.RemoveAt(index);
		index = Random.Range (0, serverPorts.Count); //Take random integer
		serverPort = serverPorts[index]; //Pick random spawnpoint (because of random int)
		yolo.text = "SHIT, NEWPORT: " + serverPort;
		startServer2();
	}

	public void closeServer ()
	{
		networkView.RPC("clearAccounts", RPCMode.AllBuffered);
		showServers = false;
		clearTexts (true);
		Network.Disconnect();
	}

	public void refreshHost ()
	{
		MasterServer.RequestHostList (gameTypeName);
		refreshing = true;
	}

	public void closeClient ()
	{
		networkView.RPC("deleteUNServer", RPCMode.Server, BasicFunctions.activeAccount.Name, BasicFunctions.activeAccount.Number);
		clearTexts(false);
		Network.Disconnect();
	}

	public void startGame ()
	{
		if (Network.isServer)
		{
			networkView.RPC("beginGame", RPCMode.AllBuffered);
		}
	}

	void OnServerInitialized ()
	{
		amountPlayers = 0;
		Debug.Log ("Server: " + gameName + " registered");
		ClientConn = true;
	}

	void OnConnectedToServer ()
	{
		ClientConn = true;
	}

	void OnDisconnectedFromServer ()
	{
		Debug.Log ("Server disconnected");
		hostD = null;
		menuBtns.Client_Menu.SetActive (false);
		menuBtns.Multiplayer_Menu.SetActive (true);
	}

	public void ClientIsConnected ()
	{
		Debug.Log("#players: " + amountPlayers);
		if (amountPlayers == 0)
		{
			Debug.Log(Network.connections);
			BasicFunctions.activeAccount.Number = 1;
			networkView.RPC ("setAmountPlayers", RPCMode.AllBuffered, true); //Verhoog het aantal spelers
			accountNumbers.Add(BasicFunctions.activeAccount.Number); //this.AccManager.activeAccount.Number);
			activeAccounts.Add(BasicFunctions.activeAccount.Name); //this.AccManager.activeAccount.Name); //Zet username in de lijst
			setTexts1();
			Debug.Log("AA: " + activeAccounts[0]);
		}
		else if (amountPlayers == 1)
		{
			Debug.Log(Network.connections.ToString());
			menuBtns.Multiplayer_Menu.SetActive(false);
			menuBtns.Client_Menu.SetActive(true);
			BasicFunctions.activeAccount.Number = 2;
			networkView.RPC ("setAmountPlayers", RPCMode.AllBuffered, true); //Verhoog het aantal spelers
			networkView.RPC("sendUNtoServer", RPCMode.Server, BasicFunctions.activeAccount.Name, BasicFunctions.activeAccount.Number); //Geef je username mee aan de Server
		}
		else if (amountPlayers == 2)
		{
			menuBtns.Multiplayer_Menu.SetActive(false);
			menuBtns.Client_Menu.SetActive(true);
			BasicFunctions.activeAccount.Number = 3;
			networkView.RPC("setAmountPlayers", RPCMode.AllBuffered, true); //Verhoog het aantal spelers
			networkView.RPC("sendUNtoServer", RPCMode.Server, BasicFunctions.activeAccount.Name, BasicFunctions.activeAccount.Number); //Geef je username mee aan de Server
		}
		else if (amountPlayers == 3)
		{
			menuBtns.Multiplayer_Menu.SetActive(false);
			menuBtns.Client_Menu.SetActive(true);
			BasicFunctions.activeAccount.Number = 4;
			networkView.RPC("setAmountPlayers", RPCMode.AllBuffered, true); //Verhoog het aantal spelers
			networkView.RPC("sendUNtoServer", RPCMode.Server, BasicFunctions.activeAccount.Name, BasicFunctions.activeAccount.Number); //Geef je username mee aan de Server
		}
	}

	public void clearTexts (bool server)
	{
		if (server)
		{
			p1.text = "Self";
			p2.text = "Open";
			p3.text = "Open";
			p4.text = "Open";
		}
		p1c.text = "Server";
		p2c.text = "Open";
		p3c.text = "Open";
		p4c.text = "Open";
	}

	[RPC]
	public void beginGame ()
	{
		Application.LoadLevel("Main_Game");
	}

	[RPC]
	public void clearAccounts ()
	{
		this.activeAccounts.Clear();
		this.accountNumbers.Clear();
	}
	/* Stuur UI data naar de Server
	 */
	[RPC]
	public void sendUNtoServer (string UN, int Number)
	{
		if (Network.isServer)
		{
			if (!this.activeAccounts.Contains(UN))
			{
				this.activeAccounts.Add(UN);
				this.accountNumbers.Add (Number);
			}
			
			for(int i = 0; i < activeAccounts.Count; i++)
			{
				Debug.Log("SERVER: Active Accounts["+i+"]: " + activeAccounts[i]);
				Debug.Log("SERVER: Account Numbers["+i+"]: " + accountNumbers[i]);
				Account adding = new Account (activeAccounts[i], "");
				adding.Number = accountNumbers[i];
				connectedPlayers.Add(adding);
				Debug.Log("SERVER: Connected Players["+i+"]: " + connectedPlayers[i].toString());
				networkView.RPC("sendUNtoClients", RPCMode.AllBuffered, this.activeAccounts[i], this.accountNumbers[i]);
			}
		}
	}
	/* Stuur UI data naar de Clients
	 */
	[RPC]
	public void sendUNtoClients (string UN, int Number)
	{
		if (Network.isClient)
		{
			if (!this.activeAccounts.Contains(UN))
			{
				this.activeAccounts.Add(UN);
				this.accountNumbers.Add(Number);
			}
		}
		networkView.RPC("setTexts", RPCMode.AllBuffered);
		for (int i = 0; i < activeAccounts.Count; i++)
		{
			Debug.Log("CLIENT: Active Accounts["+i+"]: " + activeAccounts[i]);
			Debug.Log("CLIENT: Account Numbers["+i+"]: " + accountNumbers[i]);
		}
	}

	[RPC]
	public void deleteUNServer (string UN, int Number)
	{
		if (Network.isServer)
		{
			this.activeAccounts.Remove(UN);
			this.accountNumbers.Remove(Number);
			for (int i = 0; i < this.activeAccounts.Count; i++)
				Debug.Log("["+i+"]: " + this.activeAccounts[i]);
			networkView.RPC ("setAmountPlayers", RPCMode.AllBuffered, false);
			Debug.Log("#Players: " + amountPlayers);
			clearTexts (true);
			networkView.RPC("setTexts", RPCMode.AllBuffered);
			networkView.RPC("deleteUNClients", RPCMode.AllBuffered, UN, Number);
		}
	}

	[RPC]
	public void deleteUNClients (string UN, int Number)
	{
		if (Network.isClient)
		{
			this.activeAccounts.Remove(UN);
			this.accountNumbers.Remove(Number);
			for (int i = 0; i < this.activeAccounts.Count; i++)
				Debug.Log("["+i+"]: " + this.activeAccounts[i]);
			clearTexts(true);
			networkView.RPC("setTexts", RPCMode.AllBuffered);
		}
	}
	/* Zet de text op de Server
	 */
	public void setTexts1 ()
	{
		p1.text = "-> " + activeAccounts[0];
	}
	/* Zet de text op de Clients
	 */
	[RPC]
	public void setTexts ()
	{
		for (int i = 0; i < activeAccounts.Count; i++)
		{
			if (Network.isServer)
			{
				if (activeAccounts.Count == 1)
				{
					p1.text = "-> " + activeAccounts[0];
				}
				else if (activeAccounts.Count == 2)
				{
					p1.text = "-> " + activeAccounts[0];
					p2.text = activeAccounts[1];
				}
				else if (activeAccounts.Count == 3)
				{
					p1.text = "-> " + activeAccounts[0];
					p2.text = activeAccounts[1];
					p3.text = activeAccounts[2];
				}
				else if (activeAccounts.Count == 4)
				{
					p1.text = "-> " + activeAccounts[0];
					p2.text = activeAccounts[1];
					p3.text = activeAccounts[2];
					p4.text = activeAccounts[3];
				}
			}
			if (Network.isClient)
			{
				if (activeAccounts.Count == 2)
				{
					p1c.text = "-> " + activeAccounts[0];
					p2c.text = activeAccounts[1];
				}
				else if (activeAccounts.Count == 3)
				{
					p1c.text = "-> " + activeAccounts[0];
					p2c.text = activeAccounts[1];
					p3c.text = activeAccounts[2];
				}
				else if (activeAccounts.Count == 4)
				{
					p1c.text = "-> " + activeAccounts[0];
					p2c.text = activeAccounts[1];
					p3c.text = activeAccounts[2];
					p4c.text = activeAccounts[3];
				}
			}
		}
	}

	[RPC]
	public void setAmountPlayers (bool up)
	{
		if (up)
			amountPlayers += 1;
		else 
			amountPlayers -= 1;
	}

	void Update ()
	{
		if (refreshing)
		{
			refreshT -= Time.deltaTime;
			//if (MasterServer.PollHostList ().Length > 0)
			if (refreshT <= 0)
			{
				refreshT = 2;
				refreshing = false;
				Debug.Log (MasterServer.PollHostList ().Length);
				hostD = MasterServer.PollHostList ();
				showServers = true;
			}
		}

		if (ClientConn)
		{
			timer -= Time.deltaTime;
			if (timer <= 0)
			{
				timer = 0.5f;
				ClientConn = false;
				ClientUpdate = true;
			}
		}
		
		if (ClientUpdate)
		{
			ClientIsConnected();
			connected = true;
			ClientUpdate = false;
		}
	}

	void OnGUI ()
	{
		GUI.backgroundColor = Color.cyan;
		GUI.contentColor = Color.black;
		if (!Network.isClient && !Network.isServer)
		{
			if (showServers)
			{
				if (hostD != null)
				{
					for (int i = 0; i < hostD.Length; i++)
					{
						if (GUI.Button(new Rect(44*Screen.width/100, Screen.height/2 + (i * 100), Screen.width*0.12f, Screen.height*0.06f), hostD[i].gameName))
						{
							Network.Connect(hostD[i]);
						}
					}
				}
			}
		}
	}
}
