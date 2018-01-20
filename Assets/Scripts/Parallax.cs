using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour {

	public Camera cam;
	public float xscale = 1;
	public float yscale = 1;
	new private MeshRenderer renderer;

	void Start() {
		// Find and store the shader
		renderer = gameObject.GetComponent<MeshRenderer>();
	}

	// Update is called once per frame
	void Update () {
		renderer.material.mainTextureOffset = new Vector2 (cam.transform.position.x / 7 * xscale, cam.transform.position.y / 7 * yscale);
	}
}
