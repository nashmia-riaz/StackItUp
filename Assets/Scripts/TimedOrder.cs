using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Order script with a timer, to be used in Endless game mode
/// </summary>
public class TimedOrder : MonoBehaviour
{
    //image that represents timer on screen
    public Image TimerAmount;
    public GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        //start timer at the start of the object
        StartCoroutine(OrderTimer(40f));
    }
    
    IEnumerator OrderTimer(float time) {
        float initialTime = time;
        while(time > 0)
        {
            //pause timer if game is yet paused, has not started or has ended
            while (gameManager.isGamePaused || !gameManager.hasGameStarted || gameManager.hasGameEnded)
                yield return null;

            //continue timer and represent on screen
            time--;
            TimerAmount.fillAmount=(time / initialTime);
            yield return new WaitForSeconds(1.0f);
        }
        //when timer runs out, cancel it from queue
        gameManager.CancelOrder(System.Convert.ToInt32(gameObject.name));
    }
}
