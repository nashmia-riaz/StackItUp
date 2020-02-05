using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script controls the arcade game mode
/// </summary>
public class ArcadeGameController : GameManager
{
    protected Recipe currentRecipe;
    public Image currentBurgerIcon;

    private void Start()
    {
        spawnTimer = 0;
        timerLimit = 1f;
        livesText.text = lives.ToString();

        //set current recipe and it's images
        currentRecipe = recipes[PlayerPrefs.GetInt("RecipeSelected", 0)];
        currentBurgerImage.sprite = burgerBreakdowns[PlayerPrefs.GetInt("RecipeSelected", 0)];
        currentBurgerIcon.sprite = burgerIcons[PlayerPrefs.GetInt("RecipeSelected", 0)];
    }
    
    public override void IngredientStacked(string name)
    {
        //when heart is stacked, gain a ife
        if (name == "Heart")
        {
            GainLife();
            return;
        }

        //check if the ingredient stacked is not correct i.e. next ingredient in recipe - lose a life
        if (name != currentRecipe.ingredients[playerController.numberOfIngredients - 2])
        {
            LoseLife();
            DiscardBurger();
        }
        //when top bun is stacked and is the correct ingredient - score
        else if (name == "Top Bun")
        {
            Score();
            DiscardBurger();
        }
    }
}
