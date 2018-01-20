using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Block : MonoBehaviour {
	
	public event Action<int, int, List<List<GameObject>>> OnPostProcess;

	public void PostProcess(int x, int y, List<List<GameObject>> blocks){
		if (OnPostProcess != null) {
			OnPostProcess(x, y, blocks);
		}
	}
}
