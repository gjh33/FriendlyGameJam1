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

	public void PostProcess(List<List<GameObject>> blocks, int height){
		if (OnPostProcess != null) {
			OnPostProcess(x, height - y, blocks);
		}
	}
}
