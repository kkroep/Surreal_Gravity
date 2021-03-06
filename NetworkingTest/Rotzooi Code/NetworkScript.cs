﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class NetworkScript : MonoBehaviour {

	public GameObject ServerBtn;
	public GameObject HostsBtn;
	public GameObject RegBtn;
	public GameObject RegABtn;
	public GameObject loginBtn;
	public GameObject loginABtn;
	public GameObject playerPrefab;
	public GameObject textField;
	public GameObject textPWField;
	public GameObject loginField;
	public GameObject loginPWField;
	public GameObject cancelBtn;
	public Transform spawnLocation;
	public Text registerField;
	public Text registerPWField;
	public Text loginFieldT;
	public Text loginPWFieldT;
	public Text cancelText;
	public Text player1;
	public Text player2;
	public Text player3;
	public Text player4;

	private string gameName = "SurrGrav_Test_Networking";
	private string activeName;
	private string activePW;
	private int amountPlayers;
	private bool refreshing = false;
	private bool register = false;
	private bool login = false;
	private HostData[] hostD;
	private ArrayList list_name = new ArrayList();
	private ArrayList list_pw = new ArrayList();

	void Start ()
	{
		amountPlayers = 0;
		using (StreamReader sread = new StreamReader("Accounts.txt"))
		{
			int i = 0;
			while (sread.Peek() != -1)
			{
				string name = sread.ReadLine();
				string word = sread.ReadLine();
				list_name.Add(name);
				list_pw.Add(word);
				Debug.Log(list_name[i]);
				Debug.Log(list_pw[i]);
				i++;
			}
			sread.Close ();
		}
	}

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

	public void goToRegister ()
	{
		if (Network.isServer || Network.isClient)
			register = true;
	}

	public void registerAccount ()
	{
		string username = registerField.text.ToString();
		string password = registerPWField.text.ToString();
		networkView.RPC("registerAccServer", RPCMode.AllBuffered, username, password);
	}

	public void goToLogin ()
	{
		if (Network.isServer || Network.isClient)
		{
			using (StreamReader srread = new StreamReader("Accounts.txt"))
			{
				int i = 0;
				while (srread.Peek() != -1)
				{
					string name = srread.ReadLine();
					string word = srread.ReadLine();
					list_name.Add(name);
					list_pw.Add(word);
					Debug.Log(list_name[i]);
					Debug.Log(list_pw[i]);
					i++;
				}
				srread.Close ();
			}
			login = true;
		}
	}

	public void loginAccount ()
	{
		string username = loginFieldT.text.ToString();
		string password = loginPWFieldT.text.ToString();
		networkView.RPC("loginAccServer", RPCMode.AllBuffered, username, password);
	}

	public void cancelLogin ()
	{
		if (register)
		{
			cancelText.text = "Cancel Register";
			register = false;
		}
		if (login)
		{
			cancelText.text = "Cancel Login";
			login = false;
		}
	}

	void Update ()
	{
		//Debug.Log(activeName);
		//Debug.Log(activePW);
		if (!register && !login)
		{
			if (Input.GetKeyDown(KeyCode.KeypadEnter))
			{
		    	PlayerPrefs.DeleteAll ();
				Debug.Log("Accounts Deleted");
			}
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

		if (register)
		{
			ServerBtn.SetActive(false);
			HostsBtn.SetActive(false);
			RegBtn.SetActive(false);
			RegABtn.SetActive(true);
			loginBtn.SetActive(false);
			textField.SetActive(true);
			textPWField.SetActive(true);
			loginField.SetActive(false);
			loginPWField.SetActive(false);
			cancelBtn.SetActive(true);
		}
		else if (login)
		{
			ServerBtn.SetActive(false);
			HostsBtn.SetActive(false);
			RegBtn.SetActive(false);
			loginBtn.SetActive(false);
			loginABtn.SetActive(true);
			textField.SetActive(false);
			textPWField.SetActive(false);
			loginField.SetActive(true);
			loginPWField.SetActive(true);
			cancelBtn.SetActive(true);
		}
		else
		{
			ServerBtn.SetActive(true);
			HostsBtn.SetActive(true);
			RegBtn.SetActive(true);
			RegABtn.SetActive(false);
			loginBtn.SetActive(true);
			loginABtn.SetActive(false);
			textField.SetActive(false);
			textPWField.SetActive(false);
			loginField.SetActive(false);
			loginPWField.SetActive(false);
			cancelBtn.SetActive(false);
		}
	}

	public void spawnPlayer ()
	{
		Network.Instantiate (playerPrefab, spawnLocation.position, Quaternion.identity, 0);
	}

	public void OnServerInitialized () 
	{
		Debug.Log ("Server Initialized: " + gameName);
		amountPlayers = 0;
		player1.text = this.activeName;
		spawnPlayer ();
	}

	public void OnConnectedToServer ()
	{
		Debug.Log ("Connected to Server");
		spawnPlayer ();
	}

	public void OnMasterServerEvent (MasterServerEvent MSEvent)
	{
		if (MSEvent == MasterServerEvent.RegistrationSucceeded)
		{
			Debug.Log("Server is registered!");
		}
	}

	[RPC]
	public void registerAccServer (string Username, string PassWord)
	{
		if (!list_name.Contains(Username))
		{
			using (StreamWriter swrite = new StreamWriter ("Accounts.txt", true))
			{
				swrite.WriteLine(Username);
				swrite.WriteLine(PassWord);
			}
			list_name.Add(Username);
			list_pw.Add(PassWord);
			Debug.Log("Account : " + Username + " created");
			register = false;
		}
		else
		{
			Debug.Log("Username not available");
		}
	}

	[RPC]
	public void loginAccServer (string Username, string Password)
	{
		if (list_name.Contains(Username))
		{
			int index = 0;
			for (int i = 0; i < list_name.Count; i++)
			{
				if (list_name[i].Equals(Username))
				{
					Debug.Log(list_name[i]);
					index = i;
					break;
				}
			}
		
			string pw = (string) list_pw[index];
			if (pw.Equals(Password))
			{
				Debug.Log("Password Correct");
				login = false;
				activeName = Username;
				activePW = Password;
				if (amountPlayers == 0)
				{
					player1.text = this.activeName;
					amountPlayers = 1;
				}
				else if (amountPlayers == 1)
				{
					player2.text = this.activeName;
					amountPlayers = 2;
				}
				else if (amountPlayers == 2)
				{
					player3.text = this.activeName;
					amountPlayers = 3;
				}
				else if (amountPlayers == 3)
				{
					player4.text = this.activeName;
					amountPlayers = 4;
				}
				Debug.Log(activeName);
				Debug.Log(activePW);
			}
			else
			{
				Debug.Log("Password Incorrect");
			}

		}
		else
		{
			Debug.Log("Username not found");
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
