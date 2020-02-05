using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Base class for game manager
/// </summary>
public class GameManager : MonoBehaviour
{
    public GameObject player;
    public PlayerController playerController;
    int moveStep = 0;
    public Transform[] spawnPoints;
    public GameObject[] ingredients;
    float currentSpeed = 0.08f;

    protected float spawnTimer, timerLimit;

    //all recipes in game
    protected Recipe[] recipes = {
        new Recipe(new string[]{"Patty", "Lettuce","Cheese", "Top Bun"}) ,
        new Recipe(new string[]{"Cheese", "Patty", "Lettuce","Cheese", "Top Bun"}),
        new Recipe(new string[]{"Patty", "Tomatoes", "Lettuce", "Cheese", "Top Bun"}),
        new Recipe(new string[]{"Patty", "Cheese", "Lettuce", "Tomatoes","Patty", "Cheese", "Lettuce", "Top Bun"})
    };

    //images for burger breakdowns, and icons on top in UI
    public Sprite[] burgerBreakdowns, burgerIcons;

    public bool hasGameStarted = false;
    public bool hasGameEnded = false;
    public bool isGamePaused = false;

    protected int lives = 3;
    protected int score = 0;
    public Text livesText, scoreText;

    public GameObject CrossPrefab, TickPrefab, StarParticlesPrefab;

    public bool shouldStackBurger = true;

    public GameObject burgerBreakdownPanel, gameOverPanel;

    public Image currentBurgerImage;

    public AudioClip scoreSound, loseLifeSound, gameOverSound, yaySound;

    //spawn ingredients as long as game is currently running and not paused
    void Update()
    {
        if (!hasGameStarted || isGamePaused || hasGameEnded) return;

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= timerLimit)
        {
            spawnTimer = 0;
            spawnIngredients();
        }
    }

    //move bottom bun to right
    public void moveRight()
    {
        //do nothing if game is paused
        if (isGamePaused) return;
        //if bun is already in the left most lane do nothing
        if (moveStep < 1)
            moveStep++;

        player.transform.position = new Vector3(1.25f * moveStep, player.transform.position.y, player.transform.position.z);
    }

    public void moveLeft()
    {
        if (isGamePaused) return;
        if (moveStep > -1)
            moveStep--;

        player.transform.position = new Vector3(1.25f * moveStep, player.transform.position.y, player.transform.position.z);
    }

    int previousLaneSpawn = 0;

    /// <summary>
    /// Randomly spawn ingredients in lanes
    /// </summary>
    void spawnIngredients() {
        int laneToSpawn = Random.Range(0, 3);
        //if the new lane is the same lane where previous ingredient was spawned, keep getting random ints until this isnt true
        while (laneToSpawn == previousLaneSpawn)
        {
            laneToSpawn = Random.Range(0, 3);
        }
        int ingredientToSpawn;

        //we roll a biased dice, 1/15 chance of spawning a heart
        int randomIngredient = Random.Range(0, 15);
        if (randomIngredient < 1)
            ingredientToSpawn = 0;
        else
            ingredientToSpawn = Random.Range(1, 6);

        //spawn ingredient on screen
        GameObject ingredientSpawned = Instantiate(ingredients[ingredientToSpawn], spawnPoints[laneToSpawn].position, ingredients[ingredientToSpawn].transform.rotation);
        ingredientSpawned.GetComponent<IngredientScript>().speed = currentSpeed;
        ingredientSpawned.GetComponent<SpriteRenderer>().sortingOrder = playerController.numberOfIngredients;
        
        previousLaneSpawn = laneToSpawn;

    }

    public void StartGame()
    {
        hasGameStarted = true;
    }

    //increment lives
    protected void GainLife()
    {
        lives++;
        livesText.text = lives.ToString();
    }

    public virtual void IngredientStacked(string name)
    {    }

    public virtual void CancelOrder(int orderType) { }

    //discard all ingredients on the bun and delete them
    protected void DiscardBurger()
    {
        //while ingredients are being deleted, don't stack more
        shouldStackBurger = false;

        int ingredientsToDelete = player.transform.childCount;
        StartCoroutine(Helper.WaitFor(0.75f, () => {
            for (int i = 0; i < ingredientsToDelete; i++)
                Destroy(player.transform.GetChild(i).gameObject);
            shouldStackBurger = true;
        }));
        
        playerController.ResetBurger();
    }

    //decrement life, vibrate phone, show cross on screen
    protected void LoseLife()
    {
        Vibration.Vibrate(1000);
        GameObject CrossObject = Instantiate(CrossPrefab, new Vector3(0, 0, 0), player.transform.rotation);
        CrossObject.transform.SetParent(player.transform, false);
        CrossObject.GetComponent<SpriteRenderer>().sortingOrder = playerController.numberOfIngredients + 1;
        lives--;
        livesText.text = lives.ToString();

        //if player is out of all lives, end the game
        if (lives <= 0)
        {
            hasGameEnded = true;
            gameOverPanel.SetActive(true);
            GameObject.FindGameObjectWithTag("Sound").GetComponent<AudioSource>().PlayOneShot(gameOverSound);
        }
        else
            GameObject.FindGameObjectWithTag("Sound").GetComponent<AudioSource>().PlayOneShot(loseLifeSound);
    }

    //increment score, show tick on screen and play sound
    protected void Score()
    {
        GameObject tickObject = Instantiate(TickPrefab, new Vector3(0, 0, 0), player.transform.rotation);
        tickObject.transform.SetParent(player.transform, false);
        tickObject.GetComponent<SpriteRenderer>().sortingOrder = playerController.numberOfIngredients + 1;
        Instantiate(StarParticlesPrefab).transform.SetParent(player.transform, false);
        score += 100 * (PlayerPrefs.GetInt("RecipeSelected", 0) + 1);
        scoreText.text = score.ToString();
        GameObject.FindGameObjectWithTag("Sound").GetComponent<AudioSource>().PlayOneShot(scoreSound);
    }

    //show burger breakdown and pause game
    public void ShowBurgerBreakdown()
    {
        isGamePaused = true;
        burgerBreakdownPanel.SetActive(true);
    }

    //close burger breakdown using cross
    public void CloseBurgerBreakdown()
    {
        isGamePaused = false;
        burgerBreakdownPanel.SetActive(false);
    }

    //go to menu
    public void OnClickMenu() {
        SceneManager.LoadScene("Menu Scene");
    }

    //restart scene
    public void OnClickRestart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
