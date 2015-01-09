using UnityEngine;
using System.Collections;

public class webInteractionTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		string url = "http://drproject.twi.tudelft.nl:8082/Authenticate?playerName=Steven&playerPassword=Steven2";
		WWW www = new WWW(url);
		StartCoroutine(WaitForRequest(www));

	}
	
	IEnumerator WaitForRequest(WWW www)
	{
		yield return www;
		
		// check for errors
		if (www.error == null)
		{
			if(www.data.Equals("Succesfully Authorized")){
				Debug.Log ("Succes");
			}
			else{
				Debug.Log ("Unauthorized");
			}
			//Debug.Log("WWW Ok!: " + www.data);
		} else {
			Debug.Log("WWW Error: "+ www.error);
		}    
	}
}
