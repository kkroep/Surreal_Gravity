using UnityEngine;
using System.Collections;

public class MenuButtons : MonoBehaviour {
	public Color Button_Idle = Color.green;
	public Color Button_Active = Color.red;

	public bool MultiPlayer = false;
	public bool Back = false;
	public bool QuitGame = false;
	public bool StartServer = false;
	public bool Refresh = false;

	public GameObject Main_Menu;
	public GameObject Multiplayer_Menu;

	// Use this for initialization
	void OnMouseEnter(){
		renderer.material.color = Color.white;
	}

	void Start(){
		renderer.material.color = Button_Idle;
	}

	void OnMouseExit(){
		renderer.material.color = Button_Idle;
	}

	void OnMouseUp(){
		renderer.material.color = Button_Idle;

		if (MultiPlayer) {
			Main_Menu.SetActive(false);
			Multiplayer_Menu.SetActive(true);
		}
		
		if (Back) {
			Main_Menu.SetActive(true);
			Multiplayer_Menu.SetActive(false);
		}

		if (QuitGame) {
			Application.Quit ();
		}

		if (StartServer) {
			Debug.Log ("Started The server");
		}

		if (Refresh) {
			Debug.Log ("Refreshed");
		}
	}

}
