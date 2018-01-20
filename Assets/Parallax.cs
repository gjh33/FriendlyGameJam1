using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour {

	public Camera cam;
	public float scale = 1;
	private MeshRenderer renderer;

	void Start() {
		// Find and store the shader
		renderer = gameObject.GetComponent<MeshRenderer>();
	}

	// Update is called once per frame
	void Update () {
		renderer.material.mainTextureOffset = new Vector2 (0f, cam.transform.position.y / 7 * scale);
	}
}
