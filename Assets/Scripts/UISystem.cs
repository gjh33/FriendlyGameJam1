using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISystem : MonoBehaviour {
    public static UISystem instance;

    private GameObject gameOverScreen;
    private GameObject playUI;

    private void Awake()
    {
        instance = this;

        gameOverScreen = transform.Find("GameOverScreen").gameObject;
        playUI = transform.Find("PlayUI").gameObject;
    }

    public void DisplayGameOverScreen()
    {
        ClearUI();
        gameOverScreen.SetActive(true);
    }

    public void DisplayPlayUI()
    {
        ClearUI();
        playUI.SetActive(true);
    }

    public void ClearUI()
    {
        gameOverScreen.SetActive(false);
        playUI.SetActive(false);
    }
}
