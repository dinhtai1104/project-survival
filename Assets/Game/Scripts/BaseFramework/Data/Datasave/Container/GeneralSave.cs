using Foundation.Game.Time;
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GeneralSave : BaseDatasave
{
    public bool IsOnSFX = true;
    public bool IsOnMusic = true;
    public bool IsOnVibrate = true;
    public bool IsHDR = true;
    public DateTime LastTimeOut;
    public bool IsSetTimeEndDay;
    public int PlaySession = 0;
    public bool IsGameTutFinished = false;
    public double TimeOnline = 0;
    public DateTime DateFirstTime;
    public Dictionary<string, int> IntDictionary=new Dictionary<string, int>();
    public GeneralSave(string key) : base(key)
    {
        DateFirstTime = LastTimeOut = UnbiasedTime.UtcNow;
        IsHDR = PlayerPrefs.GetInt("HDR", 0) == 1;
    }

    public override void Fix()
    {
    }

    public void SetLastTimeOut()
    {
        LastTimeOut = UnbiasedTime.UtcNow;
        Save();
    }

    public void SetInt(string key,int value)
    {
        if (IntDictionary == null)
        {
            IntDictionary = new Dictionary<string, int>();
        }
        try
        {
            if (IntDictionary.ContainsKey(key))
            {
                IntDictionary[key] = value;
            }
            else
            {
                IntDictionary.Add(key, value);
            }
        }catch(System.Exception e)
        {
            Logger.LogError(e);
        }
        }
    public int GetInt(string key, int defaultValue = 0)
    {
        if (IntDictionary == null)
        {
            IntDictionary = new Dictionary<string, int>();
        }

        if (IntDictionary.ContainsKey(key))
        {
           return IntDictionary[key]  ;
        }
        return defaultValue;
    }
}