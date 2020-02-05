using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Timer script for the initial timer at the start
/// </summary>
public class TmerScript : MonoBehaviour
{
    public GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Timer(0, 1));
    }

    IEnumerator Timer(float limit, float increment)
    {
        float time = 4;
        while(time > limit)
        {
            time -= increment;
            if (time > 0)
                gameObject.GetComponent<Text>().text = time.ToString();
            else
                gameObject.GetComponent<Text>().text = "GO!";
            yield return new WaitForSeconds(increment);
        }

        //once the timer runs through, start the game
        gameManager.StartGame();
        gameObject.SetActive(false);
    }
    
}
