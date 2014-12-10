using UnityEngine;
using System.Collections;

public class Gravity_trace_script : MonoBehaviour {
	public float Time2Live = 0.5f;
	private float Time2Live_left;

	void Start(){
		Time2Live_left = Time2Live;
	}

	void FixedUpdate (){
		Time2Live_left -= Time.fixedDeltaTime;
		//Debug.Log (Time2Live_left);
		if(Time2Live_left<0)
			Destroy (gameObject);
	}
}
