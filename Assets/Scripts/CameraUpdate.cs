using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Camera update.
/// 
/// Updates the local information based on the location of the camera
/// </summary>
public class CameraUpdate : MonoBehaviour {

	public LevelGenerator generator;
	public int offset = 0;

	private Camera camera;
	private int previousTop = 0;
	private int previousBottom = 0;
	private int maxLines = 0;
	private LinkedList<GameObject> lines = new LinkedList<GameObject>();

	// Use this for initialization
	void Start () {
		camera = gameObject.GetComponent<Camera>();
		// Initialize the first viewing window
		int top = Mathf.CeilToInt(transform.position.y + camera.orthographicSize) - 1 + offset;
		int bot = Mathf.FloorToInt(transform.position.y - camera.orthographicSize) - 1 - offset;
		for (int i = top; i > bot; i--) {
			lines.AddLast(generator.GenerateLine (i));
		}
		maxLines = lines.Count;
		previousTop = top;
		previousBottom = bot;
	}

	// Update is called once per frame
	void Update () {
		// Determine the location visible ranges
		int top = Mathf.CeilToInt(transform.position.y + camera.orthographicSize) - 1 + offset;
		int bot = Mathf.FloorToInt(transform.position.y - camera.orthographicSize) - 1 - offset;

		// Determine which lines to render and which to not
		if (previousBottom - bot > 0) {
			// Moved down (remove the number of lines that are no longer covered)
			for (int i = 0; i < previousTop - top; i++) {
				print (i);
				Destroy (lines.First.Value);
				lines.RemoveFirst ();
			}
			// Spawn in new lines (Add the new lines available, iterate over the actual ones)
			for (int i = previousBottom; i > bot; i--) {
				lines.AddLast(generator.GenerateLine (i));
			}
			// Remember what lines were rendered
			previousTop = top;
			previousBottom = bot;
		} else if (previousTop - top < 0) {
			// Moved up
			for (int i = 0; i < Mathf.Clamp(bot - previousBottom, 0, maxLines); i++) {
				Destroy (lines.Last.Value);
				lines.RemoveLast ();
			}
			// Spawn in new lines (Add the new lines available, iterate over the actual ones)
			for (int i = previousTop + 1; i <= top; i++) {
				lines.AddFirst(generator.GenerateLine (i));
			}
			// Remember what lines were rendered
			previousTop = top;
			previousBottom = bot;
		}
	}
}
