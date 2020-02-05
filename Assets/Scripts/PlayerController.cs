using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controller for player i.e. bottom bun
/// </summary>
public class PlayerController : MonoBehaviour
{
    //number of ingredients on player, set to 1 by default as bottom bun counts as one
    //this variable is also used to set the sorting order of the ingredient sprite later
    public int numberOfIngredients = 1;
    public GameManager gameManager;
    public Vector3 initialColliderSize;

    public AudioClip smack, yay;

    // Start is called before the first frame update
    void Start()
    {
        initialColliderSize = gameObject.GetComponent<BoxCollider2D>().size;
    }

    //reset the burger collider and no. of ingredients
    public void ResetBurger()
    {
        gameObject.GetComponent<BoxCollider2D>().size = initialColliderSize;
        numberOfIngredients = 1;
    }

    /// <summary>
    /// Called when anything collides with the bottom bun, usually an ingredient
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if the ingredient had passed by the bun, or if game is currently not running
        if (collision.transform.position.y < transform.position.y || !gameManager.shouldStackBurger) return;
        
        collision.gameObject.GetComponent<IngredientScript>().enabled = false;
        collision.GetComponent<SpriteRenderer>().sortingOrder = numberOfIngredients;

        //if an ingredient is collected
        if (collision.gameObject.tag != "Heart")
        {
            //add it to the burger i.e. set as parent to the bottom bun
            collision.transform.SetParent(transform, true);
            collision.transform.SetAsFirstSibling();

            //increase collider size so next ingredient stacks on top of the bun and collected ingredients
            Vector3 playerColliderSize = gameObject.GetComponent<BoxCollider2D>().size;
            Vector3 ingredientsColliderSize = collision.GetComponent<BoxCollider2D>().size;
            gameObject.GetComponent<BoxCollider2D>().size = new Vector2(playerColliderSize.x, playerColliderSize.y + ingredientsColliderSize.y);
            
            GameObject.FindGameObjectWithTag("Sound").GetComponent<AudioSource>().PlayOneShot(smack);
            ++numberOfIngredients;


        }

        //Tell game manager which ingredient was collected
        gameManager.IngredientStacked(collision.gameObject.tag);

        //if heart is collected, play sound and destroy the heart
        if(collision.gameObject.tag == "Heart")
        {
            GameObject.FindGameObjectWithTag("Sound").GetComponent<AudioSource>().PlayOneShot(yay);
            Destroy(collision.gameObject);
        }
    }
}
