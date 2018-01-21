using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Block : MonoBehaviour {
	
	public event Action<int, int, List<List<GameObject>>> OnPostProcess;
	[HideInInspector]
	public int x;
	[HideInInspector]
	public int y;
	public bool dirty = true;

	public void PostProcess(List<List<GameObject>> blocks, int height, int seed){
		if (OnPostProcess != null) {
			UnityEngine.Random.InitState (((Mathf.Abs (x) + 1) * (Mathf.Abs (y) + 1) * 1337 * seed) % 1000000);
			OnPostProcess(x, height - y, blocks);
		}
	}
}
