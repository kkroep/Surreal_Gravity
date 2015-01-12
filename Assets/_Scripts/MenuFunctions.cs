using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuFunctions : MonoBehaviour {

	public GameObject Main_Menu;
	public GameObject Multiplayer_Menu;
	public GameObject Server_Menu;
	public GameObject Client_Menu;
	public GameObject Account_Menu;
	public GameObject Control_Menu;
	public GameObject Settings_Menu;
	public GameObject Credits_Menu;

	public Text currentUname;
	public Toggle chooseModus;
	public Text chooseModusLabelServer;
	public Text chooseModusLabelClient;

	public AccountManagement AccManager;
	public NW_Server serverStuff;

	public AudioClip menu_click_sound;

	private bool setClientMenu = true;

	public void goToMultiplayer ()
	{
		AudioSource.PlayClipAtPoint(menu_click_sound, transform.position, 0.1F);
		if (AccountManagement.loggedIn)
		{
			Main_Menu.SetActive(false);
			Multiplayer_Menu.SetActive(true);
			NW_Server.showServers = true;
		}
	}

	public void goToSingleplayer ()
	{
		AudioSource.PlayClipAtPoint(menu_click_sound, transform.position, 0.1F);
		BasicFunctions.playOffline = true;
		BasicFunctions.firstStart = false;
		Application.LoadLevel("Main_Game");
	}

	public void goToServer ()
	{
		AudioSource.PlayClipAtPoint(menu_click_sound, transform.position, 0.1F);
		serverStuff.startServer();
		Multiplayer_Menu.SetActive(false);
		Server_Menu.SetActive(true);
	}

	public void goToClient ()
	{
		AudioSource.PlayClipAtPoint(menu_click_sound, transform.position, 0.1F);
		Multiplayer_Menu.SetActive(false);
		Client_Menu.SetActive(true);
	}

	public void goToAccount ()
	{
		AudioSource.PlayClipAtPoint(menu_click_sound, transform.position, 0.1F);
		Main_Menu.SetActive(false);
		Account_Menu.SetActive(true);
		if (BasicFunctions.activeAccount != null)
			currentUname.text = BasicFunctions.activeAccount.Name;
	}

	public void goToSettings ()
	{
		AudioSource.PlayClipAtPoint(menu_click_sound, transform.position, 0.1F);
		Main_Menu.SetActive(false);
		Settings_Menu.SetActive(true);
	}

	public void goToControls ()
	{
		AudioSource.PlayClipAtPoint(menu_click_sound, transform.position, 0.1F);
		Settings_Menu.SetActive(false);
		Control_Menu.SetActive(true);
	}

	public void goToCredits ()
	{
		AudioSource.PlayClipAtPoint(menu_click_sound, transform.position, 0.1F);
		Settings_Menu.SetActive(false);
		Credits_Menu.SetActive(true);
	}

	public void Refresh ()
	{
		AudioSource.PlayClipAtPoint(menu_click_sound, transform.position, 0.5F);
		serverStuff.refreshHost();
	}

	public void ChooseModus ()
	{
		if (Network.isServer)
		{
			if (chooseModus.isOn)
			{
				chooseModusLabelServer.text = "Fork";
				networkView.RPC("SetGameMode", RPCMode.AllBuffered, true);
				networkView.RPC("SetGameModeText", RPCMode.OthersBuffered, true);
			}
			else
			{
				chooseModusLabelServer.text = "RailGun";
				networkView.RPC("SetGameMode", RPCMode.AllBuffered, false);
				networkView.RPC("SetGameModeText", RPCMode.OthersBuffered, false);
			}
		}
	}

	public void StartGame ()
	{
		AudioSource.PlayClipAtPoint(menu_click_sound, transform.position, 0.1F);
		serverStuff.startGame();
	}

	public void Disconnect ()
	{
		AudioSource.PlayClipAtPoint(menu_click_sound, transform.position, 0.1F);
		if (Network.isServer)
		{
			serverStuff.closeServer();
			NW_Server.showServers = true;
			Server_Menu.SetActive(false);
			Multiplayer_Menu.SetActive(true);
		}
		if (Network.isClient)
		{
			serverStuff.closeClient();
			NW_Server.showServers = true;
			Client_Menu.SetActive(false);
			Multiplayer_Menu.SetActive(true);
		}
	}

	public void Back ()
	{
		AudioSource.PlayClipAtPoint(menu_click_sound, transform.position, 0.1F);
		Account_Menu.SetActive(false);
		Multiplayer_Menu.SetActive(false);
		Settings_Menu.SetActive(false);
		NW_Server.showServers = false;
		Main_Menu.SetActive(true);
	}

	public void BackSettings ()
	{
		AudioSource.PlayClipAtPoint(menu_click_sound, transform.position, 0.1F);
		Control_Menu.SetActive(false);
		Credits_Menu.SetActive(false);
		Settings_Menu.SetActive(true);
	}

	public void QuitGame ()
	{
		AudioSource.PlayClipAtPoint(menu_click_sound, transform.position, 0.5F);
		Application.Quit ();
	}

	public void Register ()
	{
		AudioSource.PlayClipAtPoint(menu_click_sound, transform.position, 0.5F);
		AccManager.registerAccount();
	}
	
	public void Login ()
	{
		AudioSource.PlayClipAtPoint(menu_click_sound, transform.position, 0.5F);
		AccManager.loginAccount();
	}

	void Update ()
	{
		if (!BasicFunctions.playOffline && serverStuff.connected)
		{
			if (Network.isClient && setClientMenu)
			{
				Multiplayer_Menu.SetActive(false);
				Client_Menu.SetActive(true);
				setClientMenu = false;
			}
		}
	}

	[RPC]
	public void SetGameMode (bool fork)
	{
		if (fork)
		{
			BasicFunctions.ForkModus = true;
		}
		else
		{
			BasicFunctions.ForkModus = false;
		}
	}

	[RPC]
	public void SetGameModeText (bool fork)
	{
		if (fork)
		{
			chooseModusLabelClient.text = "Fork";
		}
		else
		{
			chooseModusLabelClient.text = "RailGun";
		}
	}
}
