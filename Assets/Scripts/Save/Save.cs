﻿using System.IO;
using UnityEngine;

static public class Save
{

    /// <summary>
    /// AutoSave Function
    /// Subscribed to onDayChangedCallback
    /// </summary>
    public static void OnDayChangeAutoSave ()
    {
        if ( !Customers() )
        {
            Debug.LogError("Could not create save for customers on auto save!");
        }
        if ( !Day() )
        {
            Debug.LogError("Could not create save for DayCycle on auto save!");
        }
        if ( !PersonID() )
        {
            Debug.LogError("Could not create save for personID.");
        }
    }

    public static bool Customers ()
    {
        try
        {
            DoSave(fileName: FilesInfo.customers, objToSave: CustomerManager.Instance.allCustomers);
            return true;
        }
        catch ( System.Exception )
        {
            return false;
        }
    }

    public static bool Day ()
    {
        try
        {
            PlayerPrefs.SetInt(FilesInfo.day, DayCycle.daysPassedSinceStart);
            return true;
        }
        catch ( System.Exception )
        {
            return false;
        }
    }

    public static bool PersonID ()
    {
        try
        {
            PlayerPrefs.SetInt(FilesInfo.personID, Person.LastIdUsed);
            return true;
        }
        catch ( System.Exception )
        {
            return false;
        }
    }

    private static void DoSave (string fileName, object objToSave)
    {
        string str = JsonUtility.ToJson(objToSave);
        using ( FileStream fs = new FileStream(fileName + ".json", FileMode.OpenOrCreate) )
        {
            using ( StreamWriter writer = new StreamWriter(fs) )
            {
                writer.Write(str);
            }
        }
    }
}
