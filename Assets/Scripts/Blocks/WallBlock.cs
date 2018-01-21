using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Block))]
public class WallBlock : MonoBehaviour {
	[Header("Parameters")]
	public float baseSpikeProbability = 0.05f;
	public float baseUpSpikeProbability = 0.9f;

	[Header("Tiles")]
	public Sprite tile00;
	public Sprite tile01;
	public Sprite tile02;
	public Sprite tile03;
	public Sprite tile04;
	public Sprite tile10;
	public Sprite tile11;
	public Sprite tile12;
	public Sprite tile13;
	public Sprite tile14;
	public Sprite tile20;
	public Sprite tile21;
	public Sprite tile22;
	public Sprite tile23;
	public Sprite tile24;

	[Header("Spikes")]
	public GameObject spikeUp;
	public GameObject spikeLeft;
	public GameObject spikeRight;
	public GameObject spikeDown;

	new private SpriteRenderer renderer;

	void Start() {
		renderer = GetComponent<SpriteRenderer> ();
		Block block = GetComponent<Block> ();
		block.OnPostProcess += OnPostProcess;
	}

	public void OnPostProcess(int x, int y, List<List<GameObject>> blocks){
		bool up = false, down = false, left = false, right = false;
		int width = blocks [0].Count;
		int height = blocks.Count;
		if (y < 0 || y >= height) {
			return;
		}
		if (x >= 0 && x < width - 1) {
			if (blocks [y] [x + 1] != null && blocks [y] [x + 1].GetComponent<WallBlock>() != null ) {
				right = true;
			}
		}
		if (x <= width - 1 && x > 0) {
			if (blocks [y] [x - 1] != null && blocks [y] [x - 1].GetComponent<WallBlock>() != null ) {
				left = true;
			}
		}
		if (y >= 0 && y < height - 1) {
			if (blocks [y + 1] [x] != null && blocks [y + 1] [x].GetComponent<WallBlock>() != null ) {
				down = true;
			}
		}
		if (y <= height - 1 && y > 0) {
			if (blocks [y - 1] [x] != null && blocks [y - 1] [x].GetComponent<WallBlock>() != null ) {
				up = true;
			}
		}
		if (x == 0) {
			left = true;
		}
		if (x == width - 1) {
			right = true;
		}
		if (y == 0) {
			up = true;
		}
		if (y == height - 1) {
			down = true;
		}
		if (up && down && left && right) {
			renderer.sprite = tile11;
		} else if (up && down && left) {
			renderer.sprite = tile13;
		} else if (up && down && right) {
			renderer.sprite = tile10;
		} else if (up && left && right) {
			if (Random.Range (0, 1) == 1) {
				renderer.sprite = tile21;
			} else {
				renderer.sprite = tile22; 
			}
		} else if (down && left && right) {
			if (Random.Range (0, 1) == 1) {
				renderer.sprite = tile01; 
			} else {
				renderer.sprite = tile02; 
			}
		} else if (up && left) {
			renderer.sprite = tile23;
		} else if (up && right) {
			renderer.sprite = tile20;
		} else if (down && left) {
			renderer.sprite = tile03;
		} else if (down && right) {
			renderer.sprite = tile00;
		} else if (down && up) {
			renderer.sprite = tile14;
		} else if (left && !right) {
			renderer.sprite = tile24;
		} else if (right && !left) {
			renderer.sprite = tile04;
		} else {
			renderer.sprite = tile12;
		}

		// Generate additional features
		if (!up) {
			if (Random.value < baseUpSpikeProbability) {
				Instantiate (spikeUp, transform);
			}
		}
		if (!left) {
			if (Random.value < baseSpikeProbability) {
				Instantiate (spikeLeft, transform);
			}
		}
		if (!right) {
			if (Random.value < baseSpikeProbability) {
				Instantiate (spikeRight, transform);
			}
		}
		if (!down) {
			if (Random.value < baseSpikeProbability) {
				Instantiate (spikeDown, transform);
			}
		}
	}
}
