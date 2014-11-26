using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
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
	public GameObject chooseTeam;
	public GameObject showUName;
	public GameObject showPW;
	public GameObject showTeam;
	public GameObject switchTeam;
	public GameObject switchTeamF;
	public GameObject switchTeamField;
	public Transform spawnLocation;
	public Transform spawn1;
	public Transform spawn2;
	public Transform spawn3;
	public Transform spawn4;
	public Text registerField;
	public Text registerPWField;
	public Text loginFieldT;
	public Text loginPWFieldT;
	public Text cancelText;
	public Text player1;
	public Text player2;
	public Text player3;
	public Text player4;
	public Text chooseTeamText;
	public Text switchTeamText;
	public Text testZooi;
	public Text testZooi2;
	
	private string gameName = "SurrGrav_Multiplayer_Test";
	private static int amountPlayers;
	private bool refreshing;
	private bool register;
	private bool login;
	private bool switching;
	private Transform randomSpawnPoint;
	private HostData[] hostD;
	private AccountList list_of_accounts;
	private List<string> activeAccounts;
	private List<string> teamAccounts;
	private static List<Transform> spawnLocations;
	private Account log_acc;
	private Account activeAccount;

	/*********************
	 * Starting stuff
	 *********************/
	void Start ()
	{
		refreshing = false;
		register = false;
		login = false;
		switching = false;
		this.list_of_accounts = new AccountList (); //List containing all Accounts
		this.activeAccount = new Account ("", ""); //Account which is chosen
		this.teamAccounts = new List<string> (); //List containing the team of each account/player
		this.log_acc = new Account ("",""); //Used for logging in
		activeAccounts = new List<string>(); //List containing all active accounts/players
		using (StreamReader sread = new StreamReader("Accounts.txt"))
		{
			this.list_of_accounts = AccountList.readAccounts(sread); //Reading in all the Accounts
			sread.Close ();
		}
		spawnLocations = new List<Transform> ();
		spawnLocations.Add(spawn1);
		spawnLocations.Add(spawn2);
		spawnLocations.Add(spawn3);
		spawnLocations.Add(spawn4);
		for (int i = 0; i < spawnLocations.Count; i++)
			testZooi2.text = testZooi2.text + "\n" +  "Spawn[" + i + "]: " + spawnLocations[i];
	}

	/*********************
	 * Network Stuff
	 *********************/
	/* Start een Server
	 */
	public void startServer ()
	{
		//bool NAT = !Network.HavePublicAddress();
		Network.InitializeServer (4, 25001, true); //Initialiseer Server; max connecties  = 4, port = 25001 
		MasterServer.RegisterHost (gameName, "Multiplayer_Test", "Trying to implement Multiplayer"); //Registreer de Server
	}
	/* Zoek (een) Server(s)
	 */
	public void refreshHost ()
	{
		MasterServer.RequestHostList (gameName);
		refreshing = true;
	}
	/* Server is gemaakt
	 */
	public void OnServerInitialized () 
	{
		Debug.Log ("Server Initialized: " + gameName);
		amountPlayers = 0;
	}
	/* Iemand verbindt met de Server
	 */
	public void OnConnectedToServer ()
	{
		Debug.Log ("Connected to Server: " + gameName);
		this.activeAccount.Name = "";
		this.activeAccount.Word = "";
		this.activeAccount.Number = 0;
	}
	/* Er vindt een event plaats op de MasterServer van Unity (bijvoorbeeld onze Server verbindt met de MasterServer)
	 */
	public void OnMasterServerEvent (MasterServerEvent MSEvent)
	{
		if (MSEvent == MasterServerEvent.RegistrationSucceeded)
		{
			Debug.Log("Server: " + gameName + " is registered!");
		}
	}

	/*********************
	 * Registering Account
	 *********************/
	/* Zet registering to true
	 */
	public void goToRegister ()
	{
		if (Network.isServer || Network.isClient)
			register = true;
	}
	/* Lees de username en password in
	 */
	public void registerAccount ()
	{
		string username = registerField.text.ToString();
		string password = registerPWField.text.ToString();
		if (Network.isClient)
			networkView.RPC("registerAccServer", RPCMode.Server, username, password);
		else
			registerAccAsServer (username, password); //Als server, dan lokaal registeren
	}
	/* Lokaal registreren van account; dit doet alleen de Server
	 */
	public void registerAccAsServer (string Uname, string Pword)
	{
		Account reg_acc = new Account (Uname, Pword);
		if (!list_of_accounts.containsUsername(reg_acc))
		{
			list_of_accounts.addAccount(reg_acc);
			using (StreamWriter swrite = new StreamWriter ("Accounts.txt", true))
			{
				Account.writeAccount (reg_acc, swrite);
				swrite.Close ();
			}
			Debug.Log("Account: " + reg_acc.Name + " created");
			register = false;
		}
		else
		{
			Debug.Log("Username not available");
		}
	}
	/* Registreren van account op de Server
	 */
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
				swrite.Close();
			}
			Debug.Log("Account: " + reg_acc.Name + " created");
			register = false;
		}
		else
		{
			Debug.Log("Username not available");
		}
	}

	/*********************
	 * Inloggen account
	 *********************/
	/* Zet login to true
	 */
	public void goToLogin ()
	{
		if (Network.isServer || Network.isClient)
		{
			using (StreamReader srread = new StreamReader("Accounts.txt"))
			{
				this.list_of_accounts = AccountList.readAccounts(srread); //Haal meest recente accountslist op
				srread.Close ();
			}
			login = true;
		}
	}
	/* Lees de username, het password en het team in
	 */
	public void loginAccount ()
	{
		string username = loginFieldT.text.ToString();
		string password = loginPWFieldT.text.ToString();
		string team = chooseTeamText.text.ToString();
		networkView.RPC("loginAccServer", RPCMode.AllBuffered, username, password, team);
	}
	/* Login op een account en geef de username en het gekozen team mee aan de Server
	 */
	[RPC]
	public void loginAccServer (string Uname, string Pword, string Team)
	{	if (login)
		{
			this.log_acc.Name = Uname;
			this.log_acc.Word = Pword;
			using (StreamReader slread = new StreamReader("Accounts.txt"))
			{
				this.list_of_accounts = AccountList.readAccounts(slread);
				slread.Close ();
			}
			int i = 0;
			while (i < this.list_of_accounts.sizeList)
			{
				Account acc2 = this.list_of_accounts.indexOf(i);
				if (this.log_acc.equals(acc2))
				{
					Debug.Log("Account: " + this.log_acc.Name + "; " + this.log_acc.Word);
					break;
				}
				i++;
			}
			
			if (i != this.list_of_accounts.sizeList) //Dus account bestaat en password is correct
			{
				login = false;
				this.activeAccount.Name = this.log_acc.Name;
				this.activeAccount.Word = this.log_acc.Word;
				
				if (amountPlayers == 0) //Dus je bent de Server
				{
					this.activeAccount.Number = 1;
					Debug.Log("Active Account #1: " + this.activeAccount.Name);
					Debug.Log("Active Team #1: " + Team);
					networkView.RPC ("setAmountPlayers", RPCMode.AllBuffered); //Verhoog het aantal spelers
					activeAccounts.Add(this.activeAccount.Name); //Zet username in de lijst
					teamAccounts.Add(Team); //Zet team in de lijst
					setTexts1(); //Maak je username visible
					Debug.Log("AA: " + activeAccounts[0] + ", TA: " + teamAccounts[0]);
					spawnPlayer (); //Spawn je player
				}
				else if (amountPlayers == 1)
				{
					this.activeAccount.Number = 2;
					Debug.Log("Active Account #2: " + this.activeAccount.Name);
					networkView.RPC ("setAmountPlayers", RPCMode.AllBuffered); //Verhoog het aantal spelers
					networkView.RPC("sendUNtoServer", RPCMode.Server, this.activeAccount.Name, Team); //Geef je username mee aan de Server
					spawnPlayer (); //Spawn je player
				}
				else if (amountPlayers == 2)
				{
					this.activeAccount.Number = 3;
					Debug.Log("Active Account #3: " + this.activeAccount.Name);
					networkView.RPC("setAmountPlayers", RPCMode.AllBuffered); //Verhoog het aantal spelers
					networkView.RPC("sendUNtoServer", RPCMode.Server, this.activeAccount.Name, Team); //Geef je username mee aan de Server
					spawnPlayer (); //Spawn je player
				}
				else if (amountPlayers == 3)
				{
					this.activeAccount.Number = 4;
					Debug.Log("Active Account #4: " + this.activeAccount.Name);
					networkView.RPC("setAmountPlayers", RPCMode.AllBuffered); //Verhoog het aantal spelers
					networkView.RPC("sendUNtoServer", RPCMode.Server, this.activeAccount.Name, Team); //Geef je username mee aan de Server
					spawnPlayer (); //Spawn je player
				}
				
				Debug.Log("Active Player: " + this.activeAccount.Name);
			}
			else
			{
				Debug.Log("Login info incorrect");
			}
			Debug.Log("Numb: " + this.activeAccount.Number);
			Debug.Log("#: " + amountPlayers);
		}
	}

	/*********************
	 * Switchen van Team
	 *********************/
	/* Zet switching to true
	 */ 
	public void goToSwitch ()
	{
		switching = true;
	}
	/* Lees het nieuwe team in
	 */
	public void switchTeams ()
	{
		string newTeam = switchTeamText.text.ToString();
		networkView.RPC("switchTeamsR", RPCMode.AllBuffered, activeAccount.Number, newTeam);
		switching = false;
	}
	/* Geef aan de Server mee welke speler van team wil wisselen
	 */
	[RPC]
	public void switchTeamsR (int accNumber, string team)
	{
		networkView.RPC("switchTeamsRS", RPCMode.Server, accNumber, team);
	}
	/* Op basis van het nieuwe team, verander de kleur van de speler op de Server en stuur de info door naar de Clients
	 */
	[RPC]
	public void switchTeamsRS (int accNumber, string team)
	{
		testZooi2.text = "";
		if (accNumber == 1)
		{
			teamAccounts[0] = team;
		}
		else if (accNumber == 2)
		{
			teamAccounts[1] = team;
		}
		else if (accNumber == 3)
		{
			teamAccounts[2] = team;
		}
		else if (accNumber == 4)
		{
			teamAccounts[3] = team;
		}
		for (int ka = 0; ka < teamAccounts.Count; ka++)
		{
			testZooi2.text = testZooi2.text + "teamAccounts[" + ka + "]: " + teamAccounts[ka] + "\n";
		}
		networkView.RPC("switchTeamsRC", RPCMode.OthersBuffered, accNumber, team);
	}
	/* Op basis van het nieuwe team, verander de kleur van de speler op de Clients
	 */
	[RPC]
	public void switchTeamsRC (int accNumber, string team)
	{
		testZooi2.text = "";
		if (accNumber == 1)
		{
			teamAccounts[0] = team;
		}
		else if (accNumber == 2)
		{
			teamAccounts[1] = team;
		}
		else if (accNumber == 3)
		{
			teamAccounts[2] = team;
		}
		else if (accNumber == 4)
		{
			teamAccounts[3] = team;
		}
		for (int ka = 0; ka < teamAccounts.Count; ka++)
		{
			testZooi2.text = testZooi2.text + "teamAccounts[" + ka + "]: " + teamAccounts[ka] + "\n";
		}
		setColors(team);
		networkView.RPC("setTexts", RPCMode.AllBuffered);
	}

	/*********************
	 * Cancel Login/Register
	 *********************/
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
		if (switching)
		{
			cancelText.text = "Stay on team";
			switching = false;
		}
	}
	/*********************
	 * UI Functionalities
	 *********************/
	/* Stuur UI data naar de Server
	 */
	[RPC]
	public void sendUNtoServer (string UN, string TEAM)
	{
		if (Network.isServer)
		{
			if (!this.activeAccounts.Contains(UN))
			{
				this.activeAccounts.Add(UN);
				this.teamAccounts.Add(TEAM);
			}
			
			for(int i = 0; i < activeAccounts.Count; i++)
			{
				Debug.Log("Active Accounts[i]: " + activeAccounts[i]);
				Debug.Log("Active Teams[i]: " + teamAccounts[i]);
				networkView.RPC("sendUNtoClients", RPCMode.AllBuffered, this.activeAccounts[i], this.teamAccounts[i]);
			}
		}
	}
	/* Stuur UI data naar de Clients
	 */
	[RPC]
	public void sendUNtoClients (string UN, string TEAM)
	{
		if (Network.isClient)
		{
			if (!activeAccounts.Contains(UN))
			{
				activeAccounts.Add(UN);
				teamAccounts.Add(TEAM);
				setColors (TEAM);
			}
		}
		networkView.RPC("setTexts", RPCMode.AllBuffered);
		for (int i = 0; i < activeAccounts.Count; i++)
		{
			Debug.Log("Active Accounts[i]: " + activeAccounts[i]);
		}
	}
	/* Koppel een kleur aan een teamnummer
	 */
	public Color setColors (string Team)
	{
		if (Team.Equals("1"))
		{
			return Color.blue;
		}
		else if (Team.Equals("2"))
		{
			return Color.red;
		}
		else if (Team.Equals("3"))
		{
			return Color.yellow;
		}
		else if (Team.Equals("4"))
		{
			return Color.green;
		}
		else
		{
			return Color.black;
		}
	}
	/* Zet de text op de Server
	 */
	public void setTexts1 ()
	{
		player1.text = "P1: " + activeAccounts[0];
		player1.color = setColors(teamAccounts[0]);
	}
	/* Zet de text op de Clients
	 */
	[RPC]
	public void setTexts ()
	{
		for (int i = 0; i < activeAccounts.Count; i++)
		{
			if (activeAccounts.Count == 2)
			{
				player1.text = "P1: " + activeAccounts[0];
				player1.color = setColors(teamAccounts[0]);
				player2.text = "P2: " + activeAccounts[1];
				player2.color = setColors(teamAccounts[1]);
			}
			else if (activeAccounts.Count == 3)
			{
				player1.text = "P1: " + activeAccounts[0];
				player1.color = setColors(teamAccounts[0]);
				player2.text = "P2: " + activeAccounts[1];
				player2.color = setColors(teamAccounts[1]);
				player3.text = "P3: " + activeAccounts[2];
				player3.color = setColors(teamAccounts[2]);
			}
			else if (activeAccounts.Count == 4)
			{
				player1.text = "P1: " + activeAccounts[0];
				player1.color = setColors(teamAccounts[0]);
				player2.text = "P2: " + activeAccounts[1];
				player2.color = setColors(teamAccounts[1]);
				player3.text = "P3: " + activeAccounts[2];
				player3.color = setColors(teamAccounts[2]);
				player4.text = "P4: " + activeAccounts[3];
				player4.color = setColors(teamAccounts[3]);
			}
		}
	}
	/*********************
	 * Update function
	 *********************/
	void Update ()
	{
		testZooi.text = "Username: " + this.activeAccount.Name + "\n" + "Player#: " + this.activeAccount.Number + "\n" + "#ofPlayers: " + amountPlayers;
	
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
		if (activeAccount.Name.Equals(""))
		{
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
				chooseTeam.SetActive(false);
				showUName.SetActive(true);
				showPW.SetActive(true);
				showTeam.SetActive(false);
				switchTeam.SetActive(false);
				switchTeamF.SetActive(false);
				switchTeamField.SetActive(false);
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
				chooseTeam.SetActive(true);
				showUName.SetActive(true);
				showPW.SetActive(true);
				showTeam.SetActive(true);
				switchTeam.SetActive(false);
				switchTeamF.SetActive(false);
				switchTeamField.SetActive(false);
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
				chooseTeam.SetActive(false);
				showUName.SetActive(false);
				showPW.SetActive(false);
				showTeam.SetActive(false);
				switchTeam.SetActive(false);
				switchTeamF.SetActive(false);
				switchTeamField.SetActive(false);
			}
		}
		else
		{
			if (!switching)
			{
				RegBtn.SetActive(false);
				RegABtn.SetActive(false);
				loginBtn.SetActive(false);
				loginABtn.SetActive(false);
				textField.SetActive(false);
				textPWField.SetActive(false);
				loginField.SetActive(false);
				loginPWField.SetActive(false);
				cancelBtn.SetActive(false);
				chooseTeam.SetActive(false);
				showUName.SetActive(false);
				showPW.SetActive(false);
				showTeam.SetActive(false);
				switchTeam.SetActive(true);
				switchTeamF.SetActive(false);
				switchTeamField.SetActive(false);
			}
			else
			{
				RegBtn.SetActive(false);
				RegABtn.SetActive(false);
				loginBtn.SetActive(false);
				loginABtn.SetActive(false);
				textField.SetActive(false);
				textPWField.SetActive(false);
				loginField.SetActive(false);
				loginPWField.SetActive(false);
				cancelBtn.SetActive(true);
				chooseTeam.SetActive(false);
				showUName.SetActive(false);
				showPW.SetActive(false);
				showTeam.SetActive(false);
				switchTeam.SetActive(false);
				switchTeamF.SetActive(true);
				switchTeamField.SetActive(true);
			}
		}
	}
	/*********************
	 * Spawning Functions
	 *********************/
	/* Spawn een player
	 */
	public void spawnPlayer ()
	{
		/*if (activeAccount.Number == 1)
			Network.Instantiate (playerPrefab, spawn1.position, Quaternion.identity, 0);
		if (activeAccount.Number == 2)
			Network.Instantiate (playerPrefab, spawn2.position, Quaternion.identity, 0);
		if (activeAccount.Number == 3)
			Network.Instantiate (playerPrefab, spawn3.position, Quaternion.identity, 0);
		if (activeAccount.Number == 4)
			Network.Instantiate (playerPrefab, spawn4.position, Quaternion.identity, 0);
		*/
		int index = Random.Range (0, spawnLocations.Count); //Take random integer
		randomSpawnPoint = spawnLocations[index]; //Pick random spawnpoint (because of random int)
		Network.Instantiate (playerPrefab, randomSpawnPoint.position, Quaternion.identity, 0); //Instantiate player on the spawn point
		networkView.RPC("removeSpawnPoint", RPCMode.AllBuffered, index); //Remove spawnpoint out of the list (no duplicate spawnpoints!)
	}
	/* Verwijder het spawnpoint waar net een player gespawned is
	 */
	[RPC]
	public void removeSpawnPoint (int index)
	{
		spawnLocations.RemoveAt(index);
		testZooi2.text = "";
		for (int i = 0; i < spawnLocations.Count; i++)
			testZooi2.text = testZooi2.text + "\n" + "newSpawn[" + i + "]: " + spawnLocations[i];
	}

	/* Verhoog het aantal spelers met 1
	 */
	[RPC]
	public void setAmountPlayers ()
	{
		amountPlayers += 1;
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
						activeAccount.Name = "";
					}
				}
			}
		}
	}

}
