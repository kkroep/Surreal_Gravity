using UnityEngine;
using System.Collections;

public class MenuButtons : MonoBehaviour {
	public Color Button_Idle = Color.green;
	public Color Button_Active = Color.red;

	public bool HoverEffects = true;
	public bool MultiPlayer = false;
	public bool Back = false;
	public bool QuitGame = false;
	public bool StartServer = false;
	public bool Refresh = false;
	public bool Disconnect = false;


	public GameObject Main_Menu;
	public GameObject Multiplayer_Menu;
	public GameObject Server_Menu;

	
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
			Main_Menu.SetActive(false);
			Multiplayer_Menu.SetActive(true);
			renderer.material.color = Button_Idle;
		}
		
		if (Back) {
			Main_Menu.SetActive(true);
			Multiplayer_Menu.SetActive(false);
			renderer.material.color = Button_Idle;
		}

		if (QuitGame) {
			Application.Quit ();
		}

		if (StartServer) {
			Server_Menu.SetActive(true);
			Multiplayer_Menu.SetActive(false);
			renderer.material.color = Button_Idle;
		}

		if (Refresh) {
			Debug.Log ("Refreshed");
		}

		if (Disconnect) {
			Server_Menu.SetActive(false);
			Multiplayer_Menu.SetActive(true);
			renderer.material.color = Button_Idle;
		}
	}

}
