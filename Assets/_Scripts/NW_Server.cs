﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NW_Server : MonoBehaviour {

	//public static bool playOffline = false;

	public AccountManagement AccManager;

	public TextMesh p1;
	public TextMesh p2;
	public TextMesh p3;
	public TextMesh p4;
	public TextMesh p1c;
	public TextMesh p2c;
	public TextMesh p3c;
	public TextMesh p4c;

	public bool connected = false;

	private static int amountPlayers;

	private string gameName = "Surreal Gravity NOTYETFINISHED";
	private bool ClientConn = false;
	private bool ClientUpdate = false;
	private bool refreshing = false;
	private float timer = 0.5f;

	private HostData[] hostD;
	private List<string> activeAccounts = new List<string>();

	public void startServer ()
	{
		bool NAT = !Network.HavePublicAddress();
		Network.InitializeServer (4, 25001, NAT); //Initialiseer Server; max connecties  = 4, port = 25001 
		MasterServer.RegisterHost (gameName, "Testing the Menu", "Implementing Multiplayer in the Menu"); //Registreer de Server
	}

	public void closeServer ()
	{
		Network.Disconnect();
	}

	public void refreshHost ()
	{
		MasterServer.RequestHostList (gameName);
		refreshing = true;
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
	}

	public void ClientIsConnected ()
	{
		Debug.Log("#players: " + amountPlayers);
		if (amountPlayers == 0)
		{
			this.AccManager.activeAccount.Number = 1;
			Debug.Log("Active Account #" + this.AccManager.activeAccount.Number + ": " + this.AccManager.activeAccount.Name);
			networkView.RPC ("setAmountPlayers", RPCMode.AllBuffered); //Verhoog het aantal spelers
			activeAccounts.Add(this.AccManager.activeAccount.Name); //Zet username in de lijst
			setTexts1();
			Debug.Log("AA: " + activeAccounts[0]);
		}
		else if (amountPlayers == 1)
		{
			this.AccManager.activeAccount.Number = 2;
			Debug.Log("Active Account #" + this.AccManager.activeAccount.Number + ": " + this.AccManager.activeAccount.Name);
			networkView.RPC ("setAmountPlayers", RPCMode.AllBuffered); //Verhoog het aantal spelers
			networkView.RPC("sendUNtoServer", RPCMode.Server, this.AccManager.activeAccount.Name); //Geef je username mee aan de Server
		}
		else if (amountPlayers == 2)
		{
			this.AccManager.activeAccount.Number = 3;
			Debug.Log("Active Account #" + this.AccManager.activeAccount.Number + ": " + this.AccManager.activeAccount.Name);
			networkView.RPC("setAmountPlayers", RPCMode.AllBuffered); //Verhoog het aantal spelers
			networkView.RPC("sendUNtoServer", RPCMode.Server, this.AccManager.activeAccount.Name); //Geef je username mee aan de Server
		}
		else if (amountPlayers == 3)
		{
			this.AccManager.activeAccount.Number = 4;
			Debug.Log("Active Account #" + this.AccManager.activeAccount.Number + ": " + this.AccManager.activeAccount.Name);
			networkView.RPC("setAmountPlayers", RPCMode.AllBuffered); //Verhoog het aantal spelers
			networkView.RPC("sendUNtoServer", RPCMode.Server, this.AccManager.activeAccount.Name); //Geef je username mee aan de Server
		}
	}

	[RPC]
	public void beginGame ()
	{
		Application.LoadLevel("Main_Game");
	}
	/* Stuur UI data naar de Server
	 */
	[RPC]
	public void sendUNtoServer (string UN)
	{
		if (Network.isServer)
		{
			if (!this.activeAccounts.Contains(UN))
			{
				this.activeAccounts.Add(UN);
			}
			
			for(int i = 0; i < activeAccounts.Count; i++)
			{
				Debug.Log("SERVER: Active Accounts[i]: " + activeAccounts[i]);
				networkView.RPC("sendUNtoClients", RPCMode.AllBuffered, this.activeAccounts[i]);
			}
		}
	}
	/* Stuur UI data naar de Clients
	 */
	[RPC]
	public void sendUNtoClients (string UN)
	{
		if (Network.isClient)
		{
			if (!this.activeAccounts.Contains(UN))
			{
				this.activeAccounts.Add(UN);
			}
		}
		networkView.RPC("setTexts", RPCMode.AllBuffered);
		for (int i = 0; i < activeAccounts.Count; i++)
		{
			Debug.Log("CLIENT: Active Accounts[i]: " + activeAccounts[i]);
		}
	}
	/* Zet de text op de Server
	 */
	public void setTexts1 ()
	{
		p1.text = "-> " + activeAccounts[0];
		//player1.color = setColors(teamAccounts[0]);
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
				if (activeAccounts.Count == 2)
				{
					p1.text = "-> " + activeAccounts[0];
					//player1.color = setColors(teamAccounts[0]);
					p2.text = activeAccounts[1];
					//player2.color = setColors(teamAccounts[1]);
				}
				else if (activeAccounts.Count == 3)
				{
					p1.text = "-> " + activeAccounts[0];
					//player1.color = setColors(teamAccounts[0]);
					p2.text = activeAccounts[1];
					//player2.color = setColors(teamAccounts[1]);
					p3.text = activeAccounts[2];
					//player3.color = setColors(teamAccounts[2]);
				}
				else if (activeAccounts.Count == 4)
				{
					p1.text = "-> " + activeAccounts[0];
					//player1.color = setColors(teamAccounts[0]);
					p2.text = activeAccounts[1];
					//player2.color = setColors(teamAccounts[1]);
					p3.text = activeAccounts[2];
					//player3.color = setColors(teamAccounts[2]);
					p4.text = activeAccounts[3];
					//player4.color = setColors(teamAccounts[3]);
				}
			}
			if (Network.isClient)
			{
				if (activeAccounts.Count == 2)
				{
					p1c.text = "-> " + activeAccounts[0];
					//player1.color = setColors(teamAccounts[0]);
					p2c.text = activeAccounts[1];
					//player2.color = setColors(teamAccounts[1]);
				}
				else if (activeAccounts.Count == 3)
				{
					p1c.text = "-> " + activeAccounts[0];
					//player1.color = setColors(teamAccounts[0]);
					p2c.text = activeAccounts[1];
					//player2.color = setColors(teamAccounts[1]);
					p3c.text = activeAccounts[2];
					//player3.color = setColors(teamAccounts[2]);
				}
				else if (activeAccounts.Count == 4)
				{
					p1c.text = "-> " + activeAccounts[0];
					//player1.color = setColors(teamAccounts[0]);
					p2c.text = activeAccounts[1];
					//player2.color = setColors(teamAccounts[1]);
					p3c.text = activeAccounts[2];
					//player3.color = setColors(teamAccounts[2]);
					p4c.text = activeAccounts[3];
					//player4.color = setColors(teamAccounts[3]);
				}
			}
		}
	}

	[RPC]
	public void setAmountPlayers ()
	{
		amountPlayers += 1;
	}

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
