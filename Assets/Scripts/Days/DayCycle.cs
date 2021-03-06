﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Cares for all the information about the time
/// </summary>
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

	public int WorkingHours
	{
		get
		{
			return closingHour - openingHour;
		}
	}

	public bool IsDay
	{
		get
		{
			return isDay;
		}
	}
	#endregion

	#region private_variables
	private bool isDay = false;
	private int lastHour;
	private int lastMinute = 0;
	private int lastSpeed = 1;
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

	#region unity_methods
	protected override void Awake ()
	{
		base.Awake();
		Time.timeScale = daySpeed;

		lastHour = openingHour;
		gameTime = new DateTime(year: 2017, month: 1, day: 1, hour: openingHour, minute: 0, second: 0);

		startTimeText.text = openingHour + ":00";
		closeTimeText.text = closingHour + ":00";
	}

	private void Update ()
	{
		if ( isDay )
		{
			gameTime = gameTime.AddMinutes(Time.deltaTime);
			CheckAndInvokeHour();
			CheckAndInvokeMinute();
			if ( GameTime.Hour >= closingHour )
			{
				ChangeDay();
			}
		}

		if ( Input.GetKeyDown("i") )
		{
			StartDay();
		}
	}
	#endregion

	#region private_methods
	/// <summary>
	/// Checks if a minute has passed and invokes the delegate
	/// </summary>
	private void CheckAndInvokeMinute ()
	{
		if ( GameTime.Minute != lastMinute )
		{
			lastMinute = GameTime.Minute;
			if ( onMinuteChangedCallback != null )
			{
				onMinuteChangedCallback.Invoke();
			}
		}
	}

	/// <summary>
	/// Checks if an hour has passed and invokes the delegate
	/// </summary>
	private void CheckAndInvokeHour ()
	{
		if ( GameTime.Hour != lastHour )
		{
			lastHour = GameTime.Hour;
			if ( onHourChangedCallback != null )
				onHourChangedCallback.Invoke();
		}
	}

	/// <summary>
	/// Changes the day and invokes the delegate
	/// </summary>
	private void ChangeDay ()
	{
		Debug.Log(OrderStack.Instance.cookedOrders.Count);
		if ( OrderStack.Instance.cookedOrders.Count > 0 ) return;

		lastSpeed = ( int ) daySpeed;
		ChangeGameSpeedTo(1);
		daysPassedSinceStart++;
		isDay = false;
		TimeSpan newDayTime = new TimeSpan(openingHour, 0, 0);
		gameTime = GameTime.Date.AddDays(1) + newDayTime;

		if ( onDayChangedCallback != null )
			onDayChangedCallback.Invoke();
	}
	#endregion

	#region public_methods
	/// <summary>
	/// Starts the day
	/// </summary>
	public void StartDay ()
	{
		if ( !RestaurantManager.Instance.HasEveryWorker )
		{
			UI.Instance.OpenErrorScreen("<b>Can not start the next day!</b><br> The restaurant must have employed every type of worker before it can operate!");
			Debug.Log("Cant start, the restaurant must have employed every type of worker before it can operate!");
			return;
		}
		RestaurantManager.Instance.SeperateWaitersByTables();

		isDay = true;

		if ( onDayStartedCallback != null )
		{
			onDayStartedCallback.Invoke();
		}

		ChangeGameSpeedTo(lastSpeed);
	}


	/// <summary>
	/// Changes Time.timeScale to be equal to the argument given
	/// </summary>
	/// <param name="speed"></param>
	public void ChangeGameSpeedTo ( float speed )
	{
		daySpeed = speed;
		Time.timeScale = daySpeed;
	}

	/// <summary>
	/// Calculates the current from the argument, use only for loading!
	/// </summary>
	/// <param name="dayPassed"></param>
	public void LoadDay ( int dayPassed )
	{
		gameTime = gameTime.AddDays(dayPassed);
		daysPassedSinceStart = dayPassed;
	}

	public void SetStartTime ( string hour )
	{
		try
		{
			openingHour = int.Parse(hour);
		}
		catch ( Exception e )
		{
			Debug.Log("String for startTime is not valid." + e.Message);
		}
		startTimeText.text = openingHour + ":00";
	}

	public void SetCloseHour ( string hour )
	{
		try
		{
			closingHour = int.Parse(hour);
		}
		catch ( Exception e )
		{
			Debug.Log("String for closeTime is not valid." + e.Message);
		}
		closeTimeText.text = closingHour + ":00";
	}
	#endregion
}
