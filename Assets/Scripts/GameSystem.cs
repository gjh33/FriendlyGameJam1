using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSystem : MonoBehaviour {

    public static GameSystem instance;

    public GameObject playerPrefab;
    public GameObject spawnLocation;
    public float surfaceY;

    [HideInInspector]
    public GameObject playerInstance;

    private float currentPlayerDepth = 0;

    // Init
    private void Awake()
    {
        instance = this;
        if (playerPrefab == null) throw new System.Exception("Player not declared to game system. Please provide a reference to the player Prefab");
        if (spawnLocation == null) throw new System.Exception("Spawn location not declared to game system. Please provide a reference to a game object who's transform is the spawn location for the player");
    }

    // Start the game
    private void Start()
    {
        StartGame();
    }

    // Update player health
    private void Update()
    {
        if (playerInstance != null)
        {
            currentPlayerDepth = surfaceY - playerInstance.transform.position.y;
        }
        else
        {
            currentPlayerDepth = 0;
        }
    }

    // Starts the game
    public void StartGame()
    {
        UISystem.instance.DisplayPlayUI();
        Respawn();
    }

    public void Respawn()
    {
        if (playerInstance == null)
        {
            playerInstance = Instantiate(playerPrefab, spawnLocation.transform.position, Quaternion.identity);
        }
        else
        {
            playerInstance.transform.position = spawnLocation.transform.position;
            playerInstance.SetActive(true);
        }
        // Set camera target
        Camera.main.GetComponent<CameraController>().target = playerInstance.transform;

        // Reset z value
        playerInstance.transform.position = new Vector3(playerInstance.transform.position.x, playerInstance.transform.position.y, 3);
    }

    public void GameOver()
    {
        UISystem.instance.DisplayGameOverScreen();
    }

    public float GetDepth()
    {
        return currentPlayerDepth;
    }
}
