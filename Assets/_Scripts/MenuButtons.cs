using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuButtons : MonoBehaviour {
	public Color Button_Idle = Color.green;
	public Color Button_Active = Color.red;

	public TextMesh currentUname;

	public bool HoverEffects = true;
	public bool MultiPlayer = false;
	public bool SinglePlayer = false;
	public bool Back = false;
	public bool QuitGame = false;
	public bool StartServer = false;
	public bool Refresh = false;
	public bool Disconnect = false;
	public bool startGame = false;
	public bool Account = false;
	public bool Register = false;
	public bool Login = false;

	public GameObject Main_Menu;
	public GameObject Multiplayer_Menu;
	public GameObject Server_Menu;
	public GameObject Account_Menu;
	public GameObject Client_Menu;

	public GameObject multiBtn;

	public GameObject regU;
	public GameObject regP;
	public GameObject regPC;
	public GameObject logU;
	public GameObject logP;

	public AccountManagement AccManager;
	public NW_Server serverStuff;
	
	// Use this for initialization
	void OnMouseEnter(){
		if(HoverEffects)
			renderer.material.color = Color.white;
	}

	void Start(){
		renderer.material.color = Button_Idle;
	}

	void OnMouseExit(){
		if(HoverEffects)
		renderer.material.color = Button_Idle;
	}

	void OnMouseUp(){

		if (MultiPlayer) {
			if (AccountManagement.loggedIn)
			{
				Main_Menu.SetActive(false);
				Multiplayer_Menu.SetActive(true);
				renderer.material.color = Button_Idle;
			}
		}

		if (SinglePlayer) {
			BasicFunctions.playOffline = true;
			Application.LoadLevel("Main_Game");
		}
		
		if (Back) {
			Main_Menu.SetActive(true);
			Multiplayer_Menu.SetActive(false);
			Server_Menu.SetActive(false);
			Account_Menu.SetActive(false);
			renderer.material.color = Button_Idle;
			regU.SetActive(false);
			regP.SetActive(false);
			regPC.SetActive(false);
			logU.SetActive(false);
			logP.SetActive(false);
			NW_Server.showServers = false;
			if (AccountManagement.loggedIn)
				multiBtn.GetComponent<MenuButtons>().HoverEffects = true;
		}

		if (QuitGame) {
			Application.Quit ();
		}

		if (StartServer) {
			serverStuff.startServer();
			Server_Menu.SetActive(true);
			Multiplayer_Menu.SetActive(false);
			renderer.material.color = Button_Idle;
		}

		if (Refresh) {
			serverStuff.refreshHost();
		}

		if (Disconnect) {
			serverStuff.closeServer();
			Server_Menu.SetActive(false);
			Multiplayer_Menu.SetActive(true);
			renderer.material.color = Button_Idle;
		}

		if (startGame) {
			serverStuff.startGame();
		}

		if (Account) {
			Main_Menu.SetActive(false);
			Account_Menu.SetActive(true);
			regU.SetActive(true);
			regP.SetActive(true);
			regPC.SetActive(true);
			logU.SetActive(true);
			logP.SetActive(true);
			renderer.material.color = Button_Idle;
			if (BasicFunctions.activeAccount != null)
				currentUname.text = BasicFunctions.activeAccount.Name;
		}

		if (Register) {
			AccManager.registerAccount();
		}

		if (Login) {
			AccManager.loginAccount();
		}
	}
}
