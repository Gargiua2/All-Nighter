using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using Yarn.Unity;
using Yarn;


//This class controls the current time of the game. 
//Other code uses this class when they wish to increment the time. 
//In turn, this class informs other objects when the time ahs changed.
public class TimeManager : MonoBehaviour
{
    //Creates singleton on Awake. Ensures there is no more than one TimeManager in the scene,
    //and gives other scripts an easy means to reference the TimeManager.
    #region Singleton
    public static TimeManager instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    #endregion


    public GameTime time = new GameTime(new Vector3Int(0, 14, 0)); //GameTime representing our global time. 
    public int minuteIncrememntSize = 5; //The smallest increment we might change time by.
    public Action OnTimeChanged; //Action invoked when time changes.


    //Returns our current time, converted from Days/Hours/Minutes to a 12 hour clock time.
    public string GetTimeAsString()
    {
        return time.GetDisplayTime();
    }


    //////The following methods take an input time and add them to our current time.
    
    //Our main method for incrementing the game time. Takes an int value representing minutes,
    //invokes appropriate actions, and triggers certain events.
    public void IncrementTime(int minuteTime)
    {
        //If for some reason we're trying to subtract time, simply return.
        if (minuteTime <= 0)
            return;

        //Increment our time, let the NPCManager know time has changed, and invoke our time changed action to inform other objects.
        time.AddTime(minuteTime);
        NPCManager.instance.OnTimeIncrement(); 
        OnTimeChanged?.Invoke();

        //Everything in the rest of the method is event code (triggering specific timed events, sfx, etc.)
        //In a non-jam project, this would likely exist in its own class, possibly with a system for managing
        //timed events, but due to the scope of this project, it was placed here for simplicity.
        if (time.GetMinuteTime() == 21 * 60)
        {
            Time.timeScale = 0;
            FindObjectOfType<Curtain>().CloseCurtain(lightFlash);
        } 

        if(time.GetMinuteTime() >= 24 * 60)
        {
            //Using the DOTween plugin to add a short delay before triggering this event.
            Sequence s = DOTween.Sequence();
            s.AppendInterval(.25f);
            s.AppendCallback(GameManager.instance.TriggerBadEnding);
        }
        else if(time.GetMinuteTime() % 60 == 0)
        {
            SoundManager.instance.HourChime();
        } else
        {
            SoundManager.instance.MinuteTick();
        }
    }

    //Overload method.
    //Takes a time represented as a Vector3Int. Converts to a GameTime, then to minute time, and calls the base IncrementTime method.
    public void IncrementTime(Vector3Int clockTime)
    {
        IncrementTime((new GameTime(clockTime)).GetMinuteTime());
    }

    //Overlaod method.
    //Takes time represented as a GameTime struct. Converts to minutes, then calls the base IncrementTime method.
    public void IncrementTime(GameTime _time)
    {
        IncrementTime(_time.GetMinuteTime());
    }

    //More event code that I put in this class for simplicity. 
    public void lightFlash()
    {
        Sequence s = DOTween.Sequence();
        s.AppendInterval(4f).SetUpdate(true);
        s.AppendCallback(() => { Time.timeScale = 1; FindObjectOfType<Curtain>().OpenCurtain(); }).SetUpdate(true);
    }
}

//Below is the definition of the 'GameTime' struct I use to represent times in my game. 
//The struct contains a Vector3Int representing the time in Days/Hours/Minutes. 
//The struct contains a number of methods for adding to a given time, and converting the time to different formats.
[System.Serializable]
public struct GameTime
{
    [Tooltip("A Vector3Int representing time. X stores days, Y stores hours, and Z stores minutes")]
    public Vector3Int clockTime;

    //Constructor
    public GameTime(Vector3Int clockTime)
    {
        this.clockTime = clockTime;
    }

    //Adds a given time to a this time.
    public void AddTime(GameTime time)
    {
        clockTime = MinuteTimeToClockTime(GetMinuteTime() + time.GetMinuteTime());
    }

    //Overload that adds time based on an int representing minutes.
    public void AddTime(int minuteTime)
    {
        clockTime = MinuteTimeToClockTime(GetMinuteTime() + minuteTime);
    }

    //Converts from time in Days/Hours/Minutes to time in just minutes.
    public int GetMinuteTime()
    {
        return (clockTime.x * 24 * 60) + (clockTime.y * 60) + clockTime.z;
    }

    //Converts from a time in minutes to a time in Days/Hours/Minutes
    public Vector3Int MinuteTimeToClockTime(int minutes)
    {
        Vector3Int clockTime = new Vector3Int();

        clockTime.x = minutes / (24 * 60);
        minutes = minutes % (24 * 60);

        clockTime.y = minutes / 60;
        minutes = minutes % 60;

        clockTime.z = minutes;

        return clockTime;
    }

    //Returns a string representing this GameTime as a time on a 12hr clock.
    public string GetDisplayTime()
    {
        string suffix;
        int hours;
        suffix = (clockTime.y / 12 == 0) ? " AM" : " PM";
        hours = (clockTime.y / 12 == 0) ? clockTime.y : clockTime.y - 12;

        if (hours == 0)
            hours = 12;

        return hours + ":" + clockTime.z.ToString("00") + suffix;
    }
}
