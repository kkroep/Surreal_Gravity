using UnityEngine;
using System.Collections;

public class webInteractionTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		string url = "http://drproject.twi.tudelft.nl:8082/Authenticate?playerName=Steven&playerPassword=Steven";
		WWW www = new WWW(url);
		StartCoroutine(WaitForRequest(www));

	}
	
	IEnumerator WaitForRequest(WWW www)
	{
		yield return www;
		
		// check for errors
		if (www.error == null)
		{
			Debug.Log("WWW Ok!: " + www.data);
			Debug.Log ("Henk");
		} else {
			Debug.Log("WWW Error: "+ www.error);
		}    
	}
}
