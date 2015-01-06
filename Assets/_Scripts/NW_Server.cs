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

	private bool ClientConn = false;
	private bool ClientUpdate = false;
	private bool refreshing = false;
	private float timer = 0.5f;
	private float refreshT = 2f;
	private int serverPort;
	private int index;
	private int maxPlayers = 3;//(for now only 1) 3 //maxPlayers = # of clients

	private HostData[] hostD;

	void Start ()
	{
		showServers = false;
		Screen.lockCursor = false;
	}

	public void startServer ()
	{
		gameName = BasicFunctions.activeAccount.Name + "'s Server";
		serverPort = Random.Range (0, 30000); //Take random integer
		bool NAT = !Network.HavePublicAddress();
		Network.InitializeServer (4, serverPort, NAT); //Initialiseer Server; max connecties  = 4, port = 25001
		Network.maxConnections = maxPlayers;
		MasterServer.RegisterHost (gameTypeName, gameName, "Testing the ALPHA-Game"); //Registreer de Server
	}

	void OnFailedToConnect ()
	{
	}

	public void closeServer ()
	{
		networkView.RPC("clearAccounts", RPCMode.AllBuffered);
		showServers = false;
		clearTexts (true);
		MasterServer.UnregisterHost();
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
		BasicFunctions.amountPlayers = 0;
		BasicFunctions.activeAccounts.Clear ();
		BasicFunctions.accountNumbers.Clear ();
		Network.Disconnect();
	}

	public void startGame ()
	{
		if (Network.isServer)
		{
			Network.maxConnections = BasicFunctions.amountPlayers - 1; //Players can't connect midgame
			networkView.RPC("beginGame", RPCMode.AllBuffered);
		}
	}

	void OnServerInitialized ()
	{
		BasicFunctions.amountPlayers = 0;
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
		if (BasicFunctions.amountPlayers == 0)
		{
			BasicFunctions.activeAccount.Number = 1;
			networkView.RPC ("setAmountPlayers", RPCMode.AllBuffered, true); //Verhoog het aantal spelers
			BasicFunctions.accountNumbers.Add(BasicFunctions.activeAccount.Number); //this.AccManager.activeAccount.Number);
			BasicFunctions.activeAccounts.Add(BasicFunctions.activeAccount.Name); //this.AccManager.activeAccount.Name); //Zet username in de lijst
			setTexts1();
		}
		else
		{
			menuBtns.Multiplayer_Menu.SetActive(false);
			menuBtns.Client_Menu.SetActive(true);
			BasicFunctions.activeAccount.Number = Network.connections.Length + 1;
			Debug.Log("NW NUMMER: " + BasicFunctions.activeAccount.Number);
			networkView.RPC ("setAmountPlayers", RPCMode.AllBuffered, true); //Verhoog het aantal spelers
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
		else
		{
			p1c.text = "Server";
			p2c.text = "Open";
			p3c.text = "Open";
			p4c.text = "Open";
		}
	}

	[RPC]
	public void beginGame ()
	{
		BasicFunctions.firstStart = false;
		Application.LoadLevel("Main_Game");
	}

	[RPC]
	public void clearAccounts ()
	{
		BasicFunctions.activeAccounts.Clear();
		BasicFunctions.accountNumbers.Clear();
	}
	/* Stuur UI data naar de Server
	 */
	[RPC]
	public void sendUNtoServer (string UN, int Number)
	{
		if (Network.isServer)
		{
			if (!BasicFunctions.activeAccounts.Contains(UN))
			{
				BasicFunctions.activeAccounts.Add(UN);
				BasicFunctions.accountNumbers.Add (Number);
			}
			
			for(int i = 0; i < BasicFunctions.activeAccounts.Count; i++)
			{
				Account adding = new Account (BasicFunctions.activeAccounts[i], "");
				adding.Number = BasicFunctions.accountNumbers[i];
				if (!BasicFunctions.connectedPlayers.Contains(adding))
				{
					BasicFunctions.connectedPlayers.Add(adding);
				}
				networkView.RPC("sendUNtoClients", RPCMode.AllBuffered, BasicFunctions.activeAccounts[i], BasicFunctions.accountNumbers[i]);
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
			if (!BasicFunctions.activeAccounts.Contains(UN))
			{
				BasicFunctions.activeAccounts.Add(UN);
				BasicFunctions.accountNumbers.Add(Number);
			}
		}
		networkView.RPC("setTexts", RPCMode.AllBuffered);
	}

	[RPC]
	public void deleteUNServer (string UN, int Number)
	{
		if (Network.isServer)
		{
			BasicFunctions.activeAccounts.Remove(UN);
			BasicFunctions.accountNumbers.Remove(Number);
			networkView.RPC ("setAmountPlayers", RPCMode.AllBuffered, false);
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
			BasicFunctions.activeAccounts.Remove(UN);
			BasicFunctions.accountNumbers.Remove(Number);
			clearTexts(false);
			networkView.RPC("setTexts", RPCMode.AllBuffered);
		}
	}
	/* Zet de text op de Server
	 */
	public void setTexts1 ()
	{
		p1.text = "-> " + BasicFunctions.activeAccounts[0];
	}
	/* Zet de text op de Clients
	 */
	[RPC]
	public void setTexts ()
	{
		for (int i = 0; i < BasicFunctions.activeAccounts.Count; i++)
		{
			if (Network.isServer)
			{
				// Modulair maken   ************TO DO**************
				if (BasicFunctions.activeAccounts.Count == 1)
				{
					p1.text = "-> " + BasicFunctions.activeAccounts[0];
				}
				else if (BasicFunctions.activeAccounts.Count == 2)
				{
					p1.text = "-> " + BasicFunctions.activeAccounts[0];
					p2.text = BasicFunctions.activeAccounts[1];
				}
				else if (BasicFunctions.activeAccounts.Count == 3)
				{
					p1.text = "-> " + BasicFunctions.activeAccounts[0];
					p2.text = BasicFunctions.activeAccounts[1];
					p3.text = BasicFunctions.activeAccounts[2];
				}
				else if (BasicFunctions.activeAccounts.Count == 4)
				{
					p1.text = "-> " + BasicFunctions.activeAccounts[0];
					p2.text = BasicFunctions.activeAccounts[1];
					p3.text = BasicFunctions.activeAccounts[2];
					p4.text = BasicFunctions.activeAccounts[3];
				}
			}
			if (Network.isClient)
			{
				if (BasicFunctions.activeAccounts.Count == 2)
				{
					p1c.text = "-> " + BasicFunctions.activeAccounts[0];
					p2c.text = BasicFunctions.activeAccounts[1];
				}
				else if (BasicFunctions.activeAccounts.Count == 3)
				{
					p1c.text = "-> " + BasicFunctions.activeAccounts[0];
					p2c.text = BasicFunctions.activeAccounts[1];
					p3c.text = BasicFunctions.activeAccounts[2];
				}
				else if (BasicFunctions.activeAccounts.Count == 4)
				{
					p1c.text = "-> " + BasicFunctions.activeAccounts[0];
					p2c.text = BasicFunctions.activeAccounts[1];
					p3c.text = BasicFunctions.activeAccounts[2];
					p4c.text = BasicFunctions.activeAccounts[3];
				}
			}
		}
	}

	[RPC]
	public void setAmountPlayers (bool up)
	{
		if (up)
			BasicFunctions.amountPlayers += 1;
		else 
			BasicFunctions.amountPlayers -= 1;
	}

	void Update ()
	{
		if (refreshing)
		{
			refreshT -= Time.deltaTime;
			if (refreshT <= 0)
			{
				refreshT = 2;
				refreshing = false;
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
						if (GUI.Button(new Rect(44*Screen.width/100, Screen.height/2 + (i * 50), Screen.width*0.12f, Screen.height*0.06f), hostD[i].gameName))
						{
							Network.Connect(hostD[i]);
						}
					}
				}
			}
		}
	}
}
