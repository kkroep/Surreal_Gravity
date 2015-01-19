using UnityEngine;
using System.Collections;

public class MenuMusic : MonoBehaviour {

	void Update ()
	{
		if (!BasicFunctions.MusicOn)
		{
			this.gameObject.GetComponent<AudioSource>().enabled = false;
		}
		else
		{
			this.gameObject.GetComponent<AudioSource>().enabled = true;
		}
	}
}
