using Foundation.Game.Time;
using System;

[System.Serializable]
public class IAPSave
{
    public string package;
    public DateTime date;

    public static IAPSave Buy(string product)
    {
        return new IAPSave { date = UnbiasedTime.UtcNow, package = product };
    }
}