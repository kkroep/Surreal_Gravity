using UnityEngine;
using System.Collections;

public class Networkmanager2 : MonoBehaviour {

	public static bool playOffline = false;

	private string gameName = "YOLO_Test_Main_Game";
	private bool refreshing = false;
	private HostData[] hostD;
	private float timer = 1.5f;
	private bool goOn = false;

	public void startServer ()
	{
		bool NAT = !Network.HavePublicAddress();
		Network.InitializeServer (4, 25001, NAT); //Initialiseer Server; max connecties  = 4, port = 25001 
		MasterServer.RegisterHost (gameName, "Multiplayer_Test", "Trying to implement Multiplayer"); //Registreer de Server
		Application.LoadLevel(1);
	}

	public void refreshHost ()
	{
		MasterServer.RequestHostList (gameName);
		refreshing = true;
	}
	
	public void playOfflineFunction ()
	{
		playOffline = true;
		Application.LoadLevel(1); //"Copy_Of_Main_Game");
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
		if (goOn)
		{
			timer -= Time.deltaTime;
			if (timer <= 0)
			{
				timer = 0;
				Application.LoadLevel(1);
			}
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
						goOn = true;
					}
				}
			}
		}
	}
}
