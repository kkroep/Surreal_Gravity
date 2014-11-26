using UnityEngine;
using System.Collections;

public class CamControl : MonoBehaviour {
	
	public GameObject Player;
	public GameObject PlayerCam;
	
	public float currentXRot;
	public float currentYRot;
	
	private Vector3 offset;
	
	private float lookSens = 5;
	private float xRot;
	private float yRot;
	private float yRotV;
	private float xRotV;
	private float lookSmooth = 0.1f;
	
	void Start () 
	{
		offset.y = 0.15f;

		if (!networkView.isMine)
		{
			GetComponent<CamControl>().enabled = false;
			GetComponent<AudioListener>().enabled = false;
			PlayerCam.gameObject.SetActive(false);
		}
	}
	
	void LateUpdate () 
	{
		if (networkView.isMine)
		{
			transform.position = Player.transform.position + offset;
			
			yRot += Input.GetAxis("Mouse X") * lookSens;
			xRot -= Input.GetAxis("Mouse Y") * lookSens;
			
			xRot = Mathf.Clamp(xRot, -90, 90);
			
			currentXRot = Mathf.SmoothDamp(currentXRot, xRot, ref xRotV, lookSmooth);
			currentYRot = Mathf.SmoothDamp(currentYRot, yRot, ref yRotV, lookSmooth);
			
			transform.rotation = Quaternion.Euler(currentXRot, currentYRot, 0);
		}
	}
}
