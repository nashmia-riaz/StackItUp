using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script attached to each ingredient, moves them down the screen depending on speed
/// </summary>
public class IngredientScript : MonoBehaviour
{
    public float speed = .5f;

    GameManager gameManager;
    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }
    // Update is called once per frame
    void Update()
    {
        if (gameManager.isGamePaused || gameManager.hasGameEnded) return;
        transform.position = transform.position + new Vector3(0, -1 * speed, 0);
    }
}
