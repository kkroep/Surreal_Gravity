using UnityEngine;
using System.Collections;

public class BasicFunctions : MonoBehaviour {

	public static bool playOffline;
	public static Account activeAccount;

	void Awake ()
	{
		DontDestroyOnLoad(this);
		//activeAccount = new Account("", "");
	}

	public static Vector3 ProjectVectorOnPlane(Vector3 Normal, Vector3 Vector){
		//De projectie is de oorspronkelijke vector minus de component loodrecht op de plane en dus aan de normal
		return Vector - (Vector3.Dot(Vector, Normal) * Normal);
	}

}
