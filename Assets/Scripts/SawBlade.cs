using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rotation))]
public class SawBlade : MonoBehaviour {

	public float minSpeed = 1f;
	public float maxSpeed = 360f;

	void Start (){
		GetComponent<Rotation> ().offset = Random.Range (0f, 360f);
		GetComponent<Rotation> ().speed = Random.Range (minSpeed, maxSpeed);
		print (GetComponent<Rotation> ().offset);
		print (GetComponent<Rotation> ().speed);
	}

}
