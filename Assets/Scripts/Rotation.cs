using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour {

	public float speed = 360f;
	public float offset = 0f;
	
	// Update is called once per frame
	void Update () {
		transform.rotation = Quaternion.Euler(new Vector3 (0, 0, (Time.time * speed + offset) % 360));
	}
}
