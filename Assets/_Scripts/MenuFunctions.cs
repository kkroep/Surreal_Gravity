using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuFunctions : MonoBehaviour {

	public GameObject Main_Menu;
	public GameObject Multiplayer_Menu;
	public GameObject Server_Menu;
	public GameObject Client_Menu;
	public GameObject Account_Menu;

	public NW_Server serverStuff;

	public AudioClip menu_click_sound;

	public void goToMultiplayer ()
	{
		AudioSource.PlayClipAtPoint(menu_click_sound, transform.position, 0.1F);
		if (AccountManagement.loggedIn)
		{
			Main_Menu.SetActive(false);
			Multiplayer_Menu.SetActive(true);
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
	}

	public void Refresh ()
	{
		AudioSource.PlayClipAtPoint(menu_click_sound, transform.position, 0.5F);
		serverStuff.refreshHost();
	}

	public void Disconnect ()
	{
		AudioSource.PlayClipAtPoint(menu_click_sound, transform.position, 0.1F);
		if (Network.isServer)
		{
			serverStuff.closeServer();
			Server_Menu.SetActive(false);
			Multiplayer_Menu.SetActive(true);
		}
		if (Network.isClient)
		{
			serverStuff.closeClient();
			Client_Menu.SetActive(false);
			Multiplayer_Menu.SetActive(true);
		}
	}

	public void Back ()
	{
		Account_Menu.SetActive(false);
		Multiplayer_Menu.SetActive(false);
		Main_Menu.SetActive(true);
	}

	public void QuitGame ()
	{
		AudioSource.PlayClipAtPoint(menu_click_sound, transform.position, 0.5F);
		Application.Quit ();
	}
}
