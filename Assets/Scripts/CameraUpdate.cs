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

	new private Camera camera;
	private int previousTop = 0;
	private int previousBottom = 0;
	private int maxLines = 0;
	private List<List<GameObject>> tiles = new List<List<GameObject>> ();
	private GameObject tileGameObject;
	private bool dirty = false;

	// Use this for initialization
	void Start () {
		camera = gameObject.GetComponent<Camera>();
		tileGameObject = new GameObject ("Tiles");
		tileGameObject.transform.position = generator.ToWorld (new Vector3 ());
		Reset ();
	}

	public void Reset () {
		// Clear all children
		for (int i = 0; i < tiles.Count; i++) {
			foreach (GameObject block in tiles[i]) {
				if (block) {
					Destroy (block);
				}
			}
		}
		tiles = new List<List<GameObject>> ();

		// Initialize the first viewing window
		int top = Mathf.CeilToInt(transform.position.y + camera.orthographicSize) - 1 + offset;
		int bot = Mathf.FloorToInt(transform.position.y - camera.orthographicSize) - 1 - offset;
		for (int i = top; i > bot; i--) {
			tiles.Add(generator.GenerateLine (i, tileGameObject));
		}
		maxLines = tiles.Count;
		previousTop = top;
		previousBottom = bot;
		dirty = true;
	}

	// Update is called once per frame
	void Update () {
		// Determine the location visible ranges
		int top = Mathf.CeilToInt(transform.position.y + camera.orthographicSize) - 1 + offset;
		int bot = Mathf.FloorToInt(transform.position.y - camera.orthographicSize) - 1 - offset;

		// Determine if there is absolutely no overlap
		if (bot <= previousTop && top >= previousBottom) {
			// Determine which lines to render and which to not
			if (previousBottom - bot > 0) {
				// Moved down (remove the number of lines that are no longer covered)
				for (int i = 0; i < Mathf.Clamp (previousTop - top, 0, maxLines); i++) {
					foreach (GameObject block in tiles[0]) {
						if (block) {
							Destroy (block);
						}
					}
					tiles.RemoveAt (0);
				}
				// Spawn in new lines (Add the new lines available, iterate over the actual ones)
				for (int i = previousBottom; i > bot; i--) {
					tiles.Add (generator.GenerateLine (i, tileGameObject));
				}
				// Remember what lines were rendered
				previousTop = top;
				previousBottom = bot;
				dirty = true;
			} else if (previousTop - top < 0) {
				// Moved up
				for (int i = 0; i < Mathf.Clamp (bot - previousBottom, 0, maxLines); i++) {
					foreach (GameObject block in tiles[tiles.Count - 1]) {
						if (block) {
							Destroy (block);
						}
					}
					tiles.RemoveAt (tiles.Count - 1);
				}
				// Spawn in new lines (Add the new lines available, iterate over the actual ones)
				for (int i = previousTop + 1; i <= top; i++) {
					tiles.Insert (0, generator.GenerateLine (i, tileGameObject));
				}
				// Remember what lines were rendered
				previousTop = top;
				previousBottom = bot;
				dirty = true;
			}
		} else {
			Reset ();
		}
	}

	void LateUpdate() {
		if (dirty) {
			generator.PostProcess (tiles, Mathf.CeilToInt(transform.position.y + camera.orthographicSize) - 1 + offset);
			dirty = false;
		}
	}
}
