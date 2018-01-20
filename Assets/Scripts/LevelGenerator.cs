using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour {

	[Header("Input parameters")]
	public int width = 0;
	public int perlinSeed = 0;

	[Header("Block types")]
	public GameObject wall;
	public GameObject spike;

	public GameObject GenerateLine(int y){
		int seed = (int) (Mathf.PerlinNoise (((float)y + perlinSeed) / 10, ((float)y + perlinSeed) / 10) * 100000);
		Random.InitState (seed);
		// Generate Line holder
		GameObject line = new GameObject("Line " + y);
		line.transform.position = ToWorld (new Vector3 (0, y));

		//  Spawn Tiles based on location
		for (int x = 0; x < width; x++) {
			GameObject block = GenerateBlock (x, y);
			if (block != null) {
				GameObject tile = Instantiate (block);
				tile.transform.SetParent (line.transform);
				tile.transform.localPosition = new Vector3(x, 0, 0);
			}
		}
		return line;
	}

	private Vector3 ToWorld(Vector3 coord){
		return new Vector3(coord.x + 0.5f - width / 2, coord.y + 0.5f, 0);
	}

	private GameObject GenerateBlock(float x, float y){
		if (x == 0 || x == width - 1) {
			return wall;
		}
		float noise = Random.value;
		if (noise < 0.05) {
			return spike;
		}
		return null;
	}
}
