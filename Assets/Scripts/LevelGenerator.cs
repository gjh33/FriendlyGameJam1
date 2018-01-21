using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour {

	[Header("Input parameters")]
	public int width = 0;
	public int perlinSeed = 0;
	public int layerDepth = 100;
	public int innerWidth = 10;
	public int ledgePeriod = 100; // Inversely related
	public int ledgeWidth = 1;

	[Header("Block types")]
	public GameObject wall;
	public GameObject spike;

	// Used to offset the ledge
	private int ledgeSeed = 1000;

	public List<GameObject> GenerateLine(int y, GameObject holder){
		int seed = (int) (Mathf.PerlinNoise (((float)y + perlinSeed) / 10, ((float)y + perlinSeed) / 10) * 100000);
		List<GameObject> blocks = new List<GameObject> ();
		Random.InitState (seed);

		//  Spawn Tiles based on location
		for (int x = 0; x < width; x++) {
			GameObject block = GenerateBlock (x, y);
			GameObject tile = null;
			if (block != null) {
				tile = Instantiate (block);
				tile.transform.SetParent (holder.transform);
				Debug.Assert (tile.GetComponent<Block>()); 
				tile.GetComponent<Block>().x = x; 
				tile.GetComponent<Block>().y = y; 
				tile.transform.localPosition = new Vector3(x, y, 1);
			}
			blocks.Add (tile);
		}
		return blocks;
	}

	public void PostProcess(List<List<GameObject>> tiles, int height){
		for (int i = 0; i < tiles.Count; i++) {
			for (int j = 0; j < tiles[i].Count; j++) {
				Random.InitState((new Vector3 (i, (j + height) * 1337 % 81512345, perlinSeed)).GetHashCode ());
				if (tiles[i][j] != null) {
					Block block = tiles [i] [j].GetComponent<Block> ();
					// Call post-process once the full world of blocks is created ontop of the outside
					if (i > 0 && i < tiles.Count - 1 && block.dirty) {
						block.PostProcess (tiles, height);
						block.dirty = false;
					}
				}

			}
		}
	}

	public Vector3 ToWorld(Vector3 coord){
		return new Vector3(coord.x + 0.5f - width / 2, coord.y + 0.5f, 0);
	}

	private GameObject GenerateBlock(float x, float y){
		GameObject block = null;
		if (y < 0) {
			int leftRandOffset = Mathf.RoundToInt(ledgeWidth * Mathf.PerlinNoise(((float)y + perlinSeed) / ledgePeriod, 0));
			int rightRandOffset = Mathf.RoundToInt(ledgeWidth * Mathf.PerlinNoise(((float)y + perlinSeed + ledgeSeed) / ledgePeriod, 0));
			if (x < width / 2 - innerWidth / 2 - leftRandOffset || x >= width / 2 + innerWidth / 2 + rightRandOffset) {
				block = wall;	
			}
			float noise = Random.value;
			if (noise < 0.05f * (-y / layerDepth)) {
				block = wall;
			}
		} else {
			// above zone generation
			if ((x < width / 2 - innerWidth / 2 - 1 || x >= width / 2 + innerWidth / 2 + 1) && y == 0) {
				block = wall;
			} else if ((x < width / 2 - innerWidth / 2 - 3 || x >= width / 2 + innerWidth / 2 + 3) && y == 1) {
				block = wall;
			} else if ((x < width / 2 - innerWidth / 2 - 4 || x >= width / 2 + innerWidth / 2 + 4) && (y > 1 && y <= 10)) {
				block = wall;
			} else if (y > 10) {
				block = wall;
			}
		}
		return block;
	}
}
