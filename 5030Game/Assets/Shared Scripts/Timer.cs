using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public static Timer instance;   // referance to self
    public Text timerText;          // referance to UI text

    private TimeSpan timeTs;        // holds time elapsed in type TimeSpan for formatting
    private float timeSec;          // time elapsed in seconds
    private bool timerOn;           // cofuntion bool, turns timer on and off

    #region Timer control funcs
    //runs when loaded
    private void Awake() {
        instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
        timerText.text = "00:00.00";
        timerOn = false;
        timeSec = 0f;
        timeTs = new TimeSpan(0, 0, 0, 0, 0);
    }

    //starts timer from zero
    public void TimerBegin()
    {
        timerOn = true;
        timeSec = 0f;
        timeTs = new TimeSpan(0, 0, 0, 0, 0);
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
        timerText.text = "00:00.00";
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

    //Dummy Function to prevent error in Puzzle.cs line 92 where this function is called.
    //Could not time Timer class reference in either original Logic or Sensory games to recreate function.
    //Mariaye please, remake this function sorry for losing the original 
    public bool GetTimerStatus() { return timerOn; }

    #endregion

    //Updates the time and updates the text field.
    //coroutine will self destruct upon exit
    private IEnumerator UpdateTimer()
    {
        while (timerOn)
        {
            timeSec += Time.deltaTime;
            timeTs = TimeSpan.FromSeconds(timeSec);
            string str = timeTs.ToString("mm':'ss'.'fff");
            timerText.text = str;

            yield return null;
        }
    }

}
