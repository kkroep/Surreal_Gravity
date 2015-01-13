using UnityEngine;
using System.Collections;

public class ScoreScreen : MonoBehaviour
{

		public GameObject scoreScreen;

		// Use this for initialization
		void Start ()
		{
	
		}
	
		// Update is called once per frame
		void Update ()
		{
				if (Input.GetKeyDown (KeyCode.Tab)) {
						scoreScreen.SetActive (true);
				}
				if (Input.GetKeyUp (KeyCode.Tab)) {
						scoreScreen.SetActive (false);
				}
		}
}
