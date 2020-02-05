using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Helper class contains generic functions
/// </summary>
/// 
public static class Helper
{
    //delegate of function to be called at the end of timers
    public delegate void delegateFunction();
    
    //generic timer for int
    public static IEnumerator WaitFor(int seconds, delegateFunction functionToExecute)
    {
        while (seconds > 0)
        {
            seconds--;
            yield return new WaitForSeconds(1);
        }
        functionToExecute();
    }

    //generic timer for float
    public static IEnumerator WaitFor(float seconds, delegateFunction functionToExecute)
    {
        while (seconds > 0)
        {
            seconds-=0.2f;
            yield return new WaitForSeconds(0.2f);
        }
        functionToExecute();
    }
}
