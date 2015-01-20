using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour {

	private Vector3 TruePosition;
	private Quaternion TrueRotation;
	private Vector3 start_local_pos;

	void Start ()
	{
		start_local_pos = transform.localPosition;
	}

	void OnSerializeNetworkView (BitStream stream, NetworkMessageInfo info)
	{
		if (stream.isWriting) {
			TruePosition = transform.position;
			TrueRotation = transform.rotation;
			stream.Serialize (ref TruePosition);
			stream.Serialize (ref TrueRotation);
		} else {
			stream.Serialize (ref TruePosition);
			stream.Serialize (ref TrueRotation);
		}
	}

	void FixedUpdate ()
	{
		if (!networkView.isMine)
		{
			transform.position = Vector3.Lerp (transform.position, TruePosition, Time.fixedDeltaTime * 5f);
			transform.rotation = Quaternion.Lerp (transform.rotation, TrueRotation, Time.fixedDeltaTime * 5f);
		}
	}
}
