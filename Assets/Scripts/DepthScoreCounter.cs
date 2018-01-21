using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class DepthScoreCounter : MonoBehaviour {
    private Text scoreText;

    private void Awake()
    {
        scoreText = GetComponent<Text>();
    }

    void Update () {
        int depthInt = (int)GameSystem.instance.GetDepth();
        scoreText.text = depthInt.ToString();
	}
}
