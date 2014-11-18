using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class NetworkFancy : MonoBehaviour {

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

	private Account activeAccount;
	private string gameName = "SurrGrav_Multiplayer_Test";
	private int amountPlayers;
	private bool refreshing = false;
	private bool register = false;
	private bool login = false;
	private HostData[] hostD;
	private AccountList list_of_accounts;

	void Start ()
	{
		list_of_accounts = new AccountList ();
		activeAccount = new Account ("", "");
		amountPlayers = 0;
		using (StreamReader sread = new StreamReader("Accounts.txt"))
		{
			list_of_accounts = AccountList.readAccounts(sread);
			sread.Close ();
		}
	}

	public void startServer ()
	{
		bool NAT = !Network.HavePublicAddress();
		Network.InitializeServer (4, 25001, NAT);
		MasterServer.RegisterHost (gameName, "Multiplayer_Test", "Trying to implement Multiplayer");
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
				list_of_accounts = AccountList.readAccounts(srread);
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

		else if (!Network.isClient && !Network.isServer)
		{
			ServerBtn.SetActive(true);
			HostsBtn.SetActive(true);
		}

		if (register)
		{
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
		player1.text = this.activeAccount.Name;
		spawnPlayer ();
	}

	public void OnConnectedToServer ()
	{
		Debug.Log ("Connected to Server: " + gameName);
		spawnPlayer ();
	}

	public void OnMasterServerEvent (MasterServerEvent MSEvent)
	{
		if (MSEvent == MasterServerEvent.RegistrationSucceeded)
		{
			Debug.Log("Server: " + gameName + " is registered!");
		}
	}

	[RPC]
	public void registerAccServer (string Uname, string Pword)
	{
		Account reg_acc = new Account (Uname, Pword);
		if (!list_of_accounts.containsUsername(reg_acc))
		{
			list_of_accounts.addAccount(reg_acc);
			using (StreamWriter swrite = new StreamWriter ("Accounts.txt", true))
			{
				Account.writeAccount (reg_acc, swrite);
			}
			Debug.Log("Account: " + reg_acc.Name + " created");
			register = false;
		}
		else
		{
			Debug.Log("Username not available");
		}
	}

	[RPC]
	public void loginAccServer (string Uname, string Pword)
	{
		Account log_acc = new Account (Uname, Pword);
		using (StreamReader slread = new StreamReader("Accounts.txt"))
		{
			list_of_accounts = AccountList.readAccounts(slread);
			slread.Close ();
		}
		int i = 0;
		while (i < list_of_accounts.sizeList)
		{
			Account acc2 = list_of_accounts.indexOf(i);
			if (log_acc.equals(acc2))
			{
				Debug.Log("Account: " + log_acc.Name + "; " + log_acc.Word);
				break;
			}
			i++;
		}

		if (i != list_of_accounts.sizeList)
		{
			login = false;
			this.activeAccount.Name = log_acc.Name;
			this.activeAccount.Word = log_acc.Word;
			
			if (amountPlayers == 0)
			{
				player1.text = this.activeAccount.Name;
				amountPlayers = 1;
			}
			else if (amountPlayers == 1)
			{
				player2.text = this.activeAccount.Name;
				amountPlayers = 2;
			}
			else if (amountPlayers == 2)
			{
				player3.text = this.activeAccount.Name;
				amountPlayers = 3;
			}
			else if (amountPlayers == 3)
			{
				player4.text = this.activeAccount.Name;
				amountPlayers = 4;
			}
			
			Debug.Log("Active Player: " + activeAccount.Name);
		}
		else
		{
			Debug.Log("Login info incorrect");
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
