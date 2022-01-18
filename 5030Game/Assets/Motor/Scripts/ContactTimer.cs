using System;
using System.Collections;
using UnityEngine;

public class ContactTimer : MonoBehaviour
{
    public static ContactTimer instance;    // referance to self

    private TimeSpan timeTs;                // holds time elapsed in type TimeSpan for formatting
    private float timeSec;                  // time elapsed in seconds
    private bool timerOn;                   // cofuntion bool, turns timer on and off
    public string Id { get; set; }



    #region Timer control funcs
    //runs when loaded
    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    public void Start()
    {
        TimerBegin();
    }

    //starts timer from zero
    public void TimerBegin()
    {
        timerOn = true;
        timeSec = 0f;
        StartCoroutine(UpdateTimer());
    }

    //Pauses the timer
    public void TimerPause()
    {
        timerOn = false;
    }

    //Restarts the timer from its previous point
    public void TimerResume()
    {
        timerOn = true;
        StartCoroutine(UpdateTimer());
    }
    //Resets the timer
    public void TimerReset()
    {
        timerOn = false;
        timeSec = 0f;
    }

    //Ends coroutine
    public void TimerEnd()
    {
        timerOn = false;
        StopCoroutine(UpdateTimer());
    }
    #endregion

    #region get funcs
    //Returns the current time as a type TimeSpan
    public TimeSpan GetCurrentTime_timespan()
    {
        return timeTs;
    }
    //Returns the current recorded time as type float
    public float GetCurrentTime_float()
    {
        return timeSec;
    }
    #endregion

    //updates the timer
    //coroutine will self destruct upon exit
    private IEnumerator UpdateTimer()
    {
        Debug.Log("UpdateTimer");
        while (timerOn)
        {
            timeSec += Time.deltaTime;
            timeTs = TimeSpan.FromSeconds(timeSec);
            yield return null;
        }
    }

    public float GetTimeS()
    {
        return timeSec;
    }

    public TimeSpan GetTimeTs()
    {
        return timeTs;
    }

    public float GetTimeAndStop()
    {
        float temp = timeSec;
        TimerEnd();
        return temp;
    }
}
