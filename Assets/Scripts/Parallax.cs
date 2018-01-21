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
		float height = Camera.main.orthographicSize * 2 * 2;
		float width = Camera.main.aspect * height;
		print (width);
		if (height < width) {
			transform.localScale = new Vector3 (width, width, 1);
		} else {
			transform.localScale = new Vector3 (height, height, 1);
		}
	}

	// Update is called once per frame
	void Update () {
		renderer.material.mainTextureOffset = new Vector2 (cam.transform.position.x / 7 * xscale, cam.transform.position.y / 7 * yscale);
	}
}
