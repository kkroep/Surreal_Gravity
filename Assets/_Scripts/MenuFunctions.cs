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
	public GameObject Multiplayer_Button;
	public GameObject CantMulti_Text;

	public Text currentUname;
	public Toggle chooseModus;
	public Toggle Music_button;
	public Toggle chooseLoginModus;
	public Text Music_text;
	public Text chooseModusLabelServer;
	public Text chooseModusLabelClient;
	public Text chooseLoginLabel;
	public Text pointsText;
	public Text SensitivityText;
	public Text clientPointsText;
	public InputField login;
	public InputField reg1;
	public InputField reg2;
	public Slider pointsSlider;

	public bool isLoggedIn = false;

	public AccountManagement AccManager;
	public NW_Server serverStuff;

	public AudioClip menu_click_sound;

	private bool setClientMenu = true;
	private bool canTabL = false;
	private bool canTabR1 = false;
	private bool canTabR2 = false;
	private bool canLogin = false;
	private bool canReg = false;

	void Start ()
	{
		BasicFunctions.ForkModus = true;
	}

	public void goToMultiplayer ()
	{
		if (BasicFunctions.MusicOn)
		{
			AudioSource.PlayClipAtPoint(menu_click_sound, transform.position, 0.1F);
		}
		if (AccountManagement.loggedIn)
		{
			Main_Menu.SetActive(false);
			Multiplayer_Menu.SetActive(true);
			NW_Server.showServers = true;
		}
	}

	public void goToSingleplayer ()
	{
		if (BasicFunctions.MusicOn)
		{
			AudioSource.PlayClipAtPoint(menu_click_sound, transform.position, 0.1F);
		}
		BasicFunctions.playOffline = true;
		BasicFunctions.ForkModus = false;
		BasicFunctions.firstStart = false;
		Application.LoadLevel("Main_Game");
	}

	public void goToServer ()
	{
		if (BasicFunctions.MusicOn)
		{
			AudioSource.PlayClipAtPoint(menu_click_sound, transform.position, 0.1F);
		}
		serverStuff.startServer();
		Multiplayer_Menu.SetActive(false);
		Server_Menu.SetActive(true);
	}

	public void goToClient ()
	{
		if (BasicFunctions.MusicOn)
		{
			AudioSource.PlayClipAtPoint(menu_click_sound, transform.position, 0.1F);
		}
		Multiplayer_Menu.SetActive(false);
		Client_Menu.SetActive(true);
	}

	public void goToAccount ()
	{
		if (BasicFunctions.MusicOn)
		{
			AudioSource.PlayClipAtPoint(menu_click_sound, transform.position, 0.1F);
		}
		Main_Menu.SetActive(false);
		Account_Menu.SetActive(true);
		if (BasicFunctions.activeAccount != null)
			currentUname.text = BasicFunctions.activeAccount.Name;
	}

	public void goToSettings ()
	{
		if (BasicFunctions.MusicOn)
		{
			AudioSource.PlayClipAtPoint(menu_click_sound, transform.position, 0.1F);
		}
		Main_Menu.SetActive(false);
		Settings_Menu.SetActive(true);
	}

	public void goToControls ()
	{
		if (BasicFunctions.MusicOn)
		{
			AudioSource.PlayClipAtPoint(menu_click_sound, transform.position, 0.1F);
		}
		Settings_Menu.SetActive(false);
		Control_Menu.SetActive(true);
	}

	public void goToCredits ()
	{
		if (BasicFunctions.MusicOn)
		{
			AudioSource.PlayClipAtPoint(menu_click_sound, transform.position, 0.1F);
		}
		Settings_Menu.SetActive(false);
		Credits_Menu.SetActive(true);
	}

	public void Refresh ()
	{
		if (BasicFunctions.MusicOn)
		{
			AudioSource.PlayClipAtPoint(menu_click_sound, transform.position, 0.1F);
		}
		serverStuff.refreshHost();
	}

	public void ChooseModus ()
	{
		if (BasicFunctions.MusicOn)
		{
			AudioSource.PlayClipAtPoint(menu_click_sound, transform.position, 0.1F);
		}
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

	public void Music_On_OFF ()
	{
		if (BasicFunctions.MusicOn)
		{
			AudioSource.PlayClipAtPoint(menu_click_sound, transform.position, 0.1F);
		}
		if (Music_button.isOn)
		{
			Music_text.text = "ON";
			BasicFunctions.MusicOn = true;
		}
		else
		{
			Music_text.text = "OFF";
			BasicFunctions.MusicOn = false;
		}
	}

	public void ChooseLoginMode ()
	{
		if (BasicFunctions.MusicOn)
		{
			AudioSource.PlayClipAtPoint(menu_click_sound, transform.position, 0.1F);
		}
		if (chooseLoginModus.isOn)
		{
			chooseLoginLabel.text = "Webserver";
			BasicFunctions.loginServer = true;
		}
		else if (!chooseLoginModus.isOn)
		{
			chooseLoginLabel.text = "Local";
			BasicFunctions.loginServer = false;
		}
	}

	public void StartGame ()
	{
		if (BasicFunctions.MusicOn)
		{
			AudioSource.PlayClipAtPoint(menu_click_sound, transform.position, 0.1F);
		}
		serverStuff.startGame();
	}

	public void Disconnect ()
	{
		if (BasicFunctions.MusicOn)
		{
			AudioSource.PlayClipAtPoint(menu_click_sound, transform.position, 0.1F);
		}
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
		if (BasicFunctions.MusicOn)
		{
			AudioSource.PlayClipAtPoint(menu_click_sound, transform.position, 0.1F);
		}
		Account_Menu.SetActive(false);
		Multiplayer_Menu.SetActive(false);
		Settings_Menu.SetActive(false);
		NW_Server.showServers = false;
		Main_Menu.SetActive(true);
	}

	public void BackSettings ()
	{
		if (BasicFunctions.MusicOn)
		{
			AudioSource.PlayClipAtPoint(menu_click_sound, transform.position, 0.1F);
		}
		Control_Menu.SetActive(false);
		Credits_Menu.SetActive(false);
		Settings_Menu.SetActive(true);
	}

	public void QuitGame ()
	{
		if (BasicFunctions.MusicOn)
		{
			AudioSource.PlayClipAtPoint(menu_click_sound, transform.position, 0.1F);
		}
		Application.Quit ();
	}

	public void Register ()
	{
		if (BasicFunctions.MusicOn)
		{
			AudioSource.PlayClipAtPoint(menu_click_sound, transform.position, 0.1F);
		}
		AccManager.registerAccount();
	}
	
	public void Login ()
	{
		if (BasicFunctions.MusicOn)
		{
			AudioSource.PlayClipAtPoint(menu_click_sound, transform.position, 0.1F);
		}
		AccManager.loginAccount();
	}

	public void setPoints (float points)
	{
		pointsText.text = "" + points;
		networkView.RPC("SetMaxPoints", RPCMode.AllBuffered, (int) points);
		networkView.RPC("SetPointsText", RPCMode.OthersBuffered, (int) points);
	}

	public void setMouseSensitivity (float points)
	{
		SensitivityText.text = "" + points;
	}

	public void setTabL ()
	{
		canTabL = true;
		canTabR1 = false;
		canTabR2 = false;
		canLogin = true;
		canReg = false;
	}

	public void setTabR1 ()
	{
		canTabR1 = true;
		canTabL = false;
		canTabR2 = false;
		canReg = true;
		canLogin = false;
	}

	public void setTabR2 ()
	{
		canTabR2 = true;
		canTabL = false;
		canTabR1 = false;
		canReg = true;
		canLogin = false;
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

		if (Input.GetKeyDown(KeyCode.Tab))
		{
			if (canTabL)
			{
				login.Select ();
			}
			else if (canTabR1)
			{
				reg1.Select ();
			}
			else if (canTabR2)
			{
				reg2.Select ();
			}
		}
		else if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
		{
			if (canLogin)
			{
				Login();
			}
			else if (canReg)
			{
				Register();
			}
		}
		if (isLoggedIn)
		{
			isLoggedIn = false;
			Account_Menu.SetActive(false);
			Main_Menu.SetActive(true);
		}
		if (AccountManagement.loggedIn)
		{
			CantMulti_Text.SetActive(false);
			Multiplayer_Button.SetActive(true);
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

	[RPC]
	public void SetMaxPoints (int points)
	{
		BasicFunctions.maxPoints = points;
	}

	[RPC]
	public void SetPointsText (int points)
	{
		clientPointsText.text = "" + points;
	}
}
