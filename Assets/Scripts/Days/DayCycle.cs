﻿using System;
using TMPro;
using UnityEngine;

public class DayCycle : GenericSingletonClass<DayCycle>
{
    #region public_variables
    public static int daysPassedSinceStart = 0;

    public float daySpeed = 5;
    public int openingHour = 14;
    public int closingHour = 22;

	public TextMeshProUGUI startTimeText;
	public TextMeshProUGUI closeTimeText;


	private DateTime gameTime;

    public DateTime GameTime
    {
        get
        {
            return gameTime;
        }
    }
    #endregion

    #region private_variables
    private bool isDay = false;
    private int lastHour;
    private int lastMinute = 0;
    #endregion

    #region delegates
    public delegate void OnDayChanged ();
    public OnDayChanged onDayChangedCallback;

    public delegate void OnHourChanged ();
    public OnHourChanged onHourChangedCallback;

    public delegate void OnMinuteChanged ();
    public OnMinuteChanged onMinuteChangedCallback;

    public delegate void OnDayStarted ();
    public OnDayStarted onDayStartedCallback;
	#endregion

	private void Start ()
    {
        // AutoSave function subscribe -> every time day changes
        onDayChangedCallback += Save.OnDayChangeAutoSave;

        Time.timeScale = daySpeed;

        lastHour = openingHour;
        gameTime = new DateTime(year: 2017, month: 1, day: 1, hour: openingHour, minute: 0, second: 0);

        if ( Load.Day() )
        {
            gameTime = gameTime.AddDays(daysPassedSinceStart);
        }
        else
        {
            daysPassedSinceStart = 0;
        }

        Load.PersonID();

		startTimeText.text = openingHour + ":00";
		closeTimeText.text = closingHour + ":00";
	}

	private void Update ()
    {
        if ( isDay )
        {
            gameTime = gameTime.AddMinutes(Time.deltaTime);
            if ( GameTime.Hour != lastHour )
            {
                lastHour = GameTime.Hour;
                if ( onHourChangedCallback != null )
                    onHourChangedCallback.Invoke();
            }

            if ( GameTime.Minute != lastMinute )
            {
                lastMinute = GameTime.Minute;
                if ( onMinuteChangedCallback != null )
                {
                    onMinuteChangedCallback.Invoke();
                }
            }

            if ( GameTime.Hour == closingHour )
            {
                ChangeDay();
                Debug.Log("Day ended.");
            }
        }
        if ( Input.GetKeyDown("i") )
        {
            StartDay();
        }
    }

    /// <summary>
    /// When the hour equals the closing hour
    /// 
    /// Stops the timer
    /// Resets the hour
    /// 
    /// Invokes onDayChanged
    /// 
    /// </summary>
    private void ChangeDay ()
    {
        daysPassedSinceStart++;
        isDay = false;
        TimeSpan newDayTime = new TimeSpan(openingHour, 0, 0);
        gameTime = GameTime.Date.AddDays(1) + newDayTime;

        if ( onDayChangedCallback != null )
            onDayChangedCallback.Invoke();
    }

    public void StartDay ()
    {
        isDay = true;

        if ( onDayStartedCallback != null )
        {
            onDayStartedCallback.Invoke();
        }
    }

	public void ChangeGameSpeedTo(float speed)
	{
		daySpeed = speed;
		Time.timeScale = daySpeed;
	}
}
