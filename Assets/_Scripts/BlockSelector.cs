using UnityEngine;
using System.Collections;

public class BlockSelector : MonoBehaviour {
	public GameObject robotSettings;

	private MainRobotSettings settings;
	private bool isSelector = false;



	void Start() {
		settings = robotSettings.GetComponent<MainRobotSettings>();
	}

	void Update(){
		float selector = Random.value;
		float timer = 0;
		if(selector>0.99 && settings.numberRobots>0){
			isSelector = true;
		}

		if(isSelector){
			isSelectorFunction();
			timer += Time.deltaTime;
			if (timer>0.1){
				settings.numberRobots += 1;
			}

		}

	}

	public void isSelectorFunction(){
		settings.numberRobots -= 1;
		Destroy (this.gameObject, 0.1f);


	}
	/*
	public GameObject getSelection(float[] xInterval, float[] yInterval, float[] zInterval){
		

	}
	*/

}
