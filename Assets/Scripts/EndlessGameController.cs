using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script controls the mechanics of the Endless game mode
/// </summary>
public class EndlessGameController : GameManager
{
    //prefab for the order in UI
    public GameObject orderPrefab;
    //Reference to order container in UI
    public Transform ordersContainer;
    
    private void Start()
    {
        spawnTimer = 0;
        timerLimit = 1f;
        livesText.text = lives.ToString();

        //At start generate a random order
        GenerateOrder();

        //Start a looped timer that generates a random order after every X seconds
        StartCoroutine(QueueOrders(30));
    }

    //Generates a random order after every 'time' seconds
    IEnumerator QueueOrders(float time)
    {
        float initialTime = time;
        while(time > 0)
        {
            time--;
            yield return new WaitForSeconds(1);
        }
        GenerateOrder();
        StartCoroutine(QueueOrders(initialTime));
    }

    //Generates an order 
    void GenerateOrder()
    {
        //if 4 orders already exist, then skip this
        if (ordersContainer.childCount >= 4)
            return;

        //Pick any burger randomly from the menu of 4
        int randomOrder = Random.Range(0, 4);

        //Instantiate and set in canvas
        GameObject orderObj = Instantiate(orderPrefab);
        orderObj.transform.SetParent(ordersContainer, false);
        orderObj.transform.Find("Image").GetComponent<Image>().sprite = burgerIcons[randomOrder];
        //set the name as the order type (0-4 correspond to burgers in menu). We use this to later delete the burger
        orderObj.name = randomOrder.ToString();

        //When this icon is tapped, show the burger breakdown and pause game
        orderObj.GetComponent<Button>().onClick.AddListener(()=> {
            this.SetBurgerBreakdown(randomOrder);
            this.ShowBurgerBreakdown();
        });

        //give the order the game manager reference, so it can cancel the order when the timer runs out
        orderObj.GetComponent<TimedOrder>().gameManager = this;
    }

    /// <summary>
    /// Sets burger breakdown image
    /// </summary>
    /// <param name="imageNo">0-4 corresponds to burgers in menu</param>
    void SetBurgerBreakdown(int imageNo)
    {
        currentBurgerImage.sprite = burgerBreakdowns[imageNo];
    }

    /// <summary>
    /// Called when an ingredient is collected on the bun
    /// </summary>
    /// <param name="name">name of the ingredient</param>
    public override void IngredientStacked(string name)
    {
        //If a heart was collected, gain a life
        if (name == "Heart")
        {
            GainLife();
            return;
        }

        //when top bun is collected, mark order as complete and check if it exists in queue
        if (name == "Top Bun")
        {
            CheckIfBurgerInQueue();
        }
    }

    /// <summary>
    /// This burger goes through the order queue one by one to see if it matches
    /// </summary>
    void CheckIfBurgerInQueue()
    {
        bool isAMatch = true;
        int orderMatched;

        //go through the queue and see if ingredients match
        for (int i = 0; i < ordersContainer.childCount; i++)
        {
            isAMatch = true;
            GameObject order = ordersContainer.GetChild(i).gameObject;
            Recipe toCheck = recipes[System.Convert.ToInt32(order.name)];
            
            //as simple check is to see if number of ingredients is same
            if (player.transform.childCount != toCheck.ingredients.Length)
            {
                isAMatch = false;
                continue;
            }

            //check and match each ingredient
            for (int j = player.transform.childCount - 1; j >= 0; j--)
            {
                //if ingredients don't match, the burger does not exist
                if (player.transform.GetChild(j).tag != toCheck.ingredients[player.transform.childCount - 1 - j])
                {
                    isAMatch = false;
                    break;
                }
            }

            //if all ingredients matched, give player score and destroy the order from queue
            if (isAMatch)
            {
                orderMatched = System.Convert.ToInt32(order.name);
                DestroyOrderInQueue(orderMatched);
                Score();
                DiscardBurger();
                return;
            }
        }

        //if the burger didn't exist in queue, lose a life
        if(!isAMatch)
        {
            LoseLife();
            DiscardBurger();
        }
    }

    /// <summary>
    /// Called usually when burger timer runs out, or if a wrong burger is made
    /// </summary>
    /// <param name="orderType"></param>
    public override void CancelOrder(int orderType)
    {
        LoseLife();
        DiscardBurger();
        DestroyOrderInQueue(orderType);
    }

    /// <summary>
    /// Destroys the burger from UI container
    /// </summary>
    /// <param name="orderType"></param>
    void DestroyOrderInQueue(int orderType)
    {
        for (int i = 0; i < ordersContainer.childCount; i++)
        {
            GameObject order = ordersContainer.GetChild(i).gameObject;
            if (order.name == orderType.ToString())
            {
                order.transform.SetParent(null);
                Destroy(order);

                if (ordersContainer.childCount <= 0)
                    GenerateOrder();

                return;
            }
        }

    }
}
