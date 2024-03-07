using CodeStage.AntiCheat.ObscuredTypes;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class CSharpExtension
{

    private static string[] ScoreNames = new string[] { "", "k", "M", "B", "T",
            "aa", "ab", "ac", "ad", "ae", "af", "ag", "ah", "ai", "aj", "ak", "al", "am", "an", "ao", "ap", "aq", "ar", "as", "at", "au", "av", "aw", "ax", "ay", "az",
            "ba", "bb", "bc", "bd", "be", "bf", "bg", "bh", "bi", "bj", "bk", "bl", "bm", "bn", "bo", "bp", "bq", "br", "bs", "bt", "bu", "bv", "bw", "bx", "by", "bz",
            "ca", "cb", "cc", "cd", "ce", "cf", "cg", "ch", "ci", "cj", "ck", "cl", "cm", "cn", "co", "cp", "cq", "cr", "cs", "ct", "cu", "cv", "cw", "cx", "cy", "cz",
            "da", "db", "dc", "dd", "de", "df", "dg", "dh", "di", "dj", "dk", "dl", "dm", "dn", "do", "dp", "dq", "dr", "ds", "dt", "du", "dv", "dw", "dx", "dy", "dz",
        };

    public static string ScoreShow(double Score, int numberScore = 3)
    {
        string result;
        string[] ScoreNames = new string[] { "", "M", "B", "T", "aa", "ab", "ac", "ad", "ae", "af", "ag", "ah", "ai", "aj", "ak", "al", "am", "an", "ao", "ap", "aq", "ar", "as", "at", "au", "av", "aw", "ax", "ay", "az", "ba", "bb", "bc", "bd", "be", "bf", "bg", "bh", "bi", "bj", "bk", "bl", "bm", "bn", "bo", "bp", "bq", "br", "bs", "bt", "bu", "bv", "bw", "bx", "by", "bz", };
        int i;

        for (i = 0; i < ScoreNames.Length; i++)
            if (Score < Mathf.Pow(10, 6))
                break;
            else Score = Mathf.Floor((float)(Score / 100f)) / 10f;

        if (Score == Mathf.Floor((float)Score))
            result = string.Format("{0:N0}", Score) + ScoreNames[i];
        else result = Score.ToString("F1") + ScoreNames[i];
        return result;
    }

    public static string PriceShow(this decimal price)
    {
        string result = string.Format(price == Decimal.ToInt32(price) ? "{0:n0}" : "{0:n}", price);
        return result;
    }

    public static string TruncateValue(this ObscuredDouble value, int numberScore = 3)
    {
        return ScoreShow(value, numberScore);
    }
    public static string TruncateValue(this ObscuredLong value, int numberScore = 3)
    {
        return ScoreShow(value, numberScore);
    }
    public static string TruncateValue(this ObscuredInt value, int numberScore = 3)
    {
        return ScoreShow(value, numberScore);
    }

    public static string TruncateValue(this double value, int numberScore = 3)
    {
        return ScoreShow(value, numberScore);
    }
    public static string TruncateValue(this long value)
    {
        return ((double)value).TruncateValue();
    }
    public static string TruncateValue(this int value)
    {
        return ((double)value).TruncateValue();
    }
    
    public static string TruncateValue(this float value, int numberScore = 3)
    {
        return ScoreShow(value, numberScore);
    }
    public static bool Contains<T>(this T[] array, T item)
    {
        foreach(T t in array)
        {
            if (t.Equals(item))
            {
                return true;
            }
        }
        return false;
    }
    public static void CleanUp<T>(this IList<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            list[i] = default(T);
        }
    }
    public static int ToInt(this float value)
    {
        return (int)value;
    }
    public static int ToInt(this double value)
    {
        return (int)value;
    }

    public static T Random<T>(this List<T> list)
    {
        if (list.Count == 0)
        {
            return (T)default;
        }
        return list[UnityEngine.Random.Range(0, list.Count)];
    } 
    public static T Random<T>(this List<T> list, List<T> except = null)
    {
        var newList = new List<T>(list);
        if (except != null)
        {
            for (int i = newList.Count - 1; i >= 0; i--)
            {
                if (except.Contains(newList[i]))
                {
                    newList.RemoveAt(i);
                }
            }
        }
        if (newList.Count == 0)
        {
            return (T)default;
        }
        return newList[UnityEngine.Random.Range(0, newList.Count)];
    }
     public static T Random<T>(this T[] list)
    {
        if (list.Length == 0) return default(T);
        return list[UnityEngine.Random.Range(0, list.Length)];
    }

    public static string ConvertTimeToString(this TimeSpan value)
    {
        if (value.Days == 0)
        {
            return $"{value.Hours:00}H:{value.Minutes:00}M:{value.Seconds:00}S";
        }
        if (value.Hours == 0)
        {
            return $"{value.Minutes:00}M:{value.Seconds:00}S";
        }
        return $"{value.Days}D:{value.Hours:00}H";
    }
    public static string ConvertMinuteSecond(this TimeSpan value)
    {
        return $"{value.Minutes:00}:{value.Seconds:00}";
    }
    public static string AddParams(this string value, params object[] args)
    {
        return string.Format(value, args);
    }

    public static string GreenColor(this string value)
    {
        return $"<color=#66DD1B>{value}</color>";
    }
    public static string RedColor(this string value)
    {
        return $"<color=#D63823>{value}</color>";
    }
    public static string GreenColor(this int value)
    {
        return $"<color=#66DD1B>{value.TruncateValue()}</color>";
    }
    public static string RedColor(this int value)
    {
        return $"<color=#D63823>{value.TruncateValue()}</color>";
    }
    public static string GreenColor(this long value)
    {
        return $"<color=#66DD1B>{value.TruncateValue()}</color>";
    }
    public static string RedColor(this long value)
    {
        return $"<color=#D63823>{value.TruncateValue()}</color>";
    }
    public static string GreenColor(this double value)
    {
        return $"<color=#66DD1B>{value.TruncateValue()}</color>";
    }
    public static string RedColor(this double value)
    {
        return $"<color=#D63823>{value.TruncateValue()}</color>";
    }

    public static string FormatRequire(int owner, int require)
    {
        if (owner >= require)
        {
            return $"{owner.GreenColor()}/{require}";
        }
        return $"{owner.RedColor()}/{require}";
    }
    public static string FormatRequire(long owner, long require)
    {
        if (owner >= require)
        {
            return $"{owner.GreenColor()}/{require}";
        }
        return $"{owner.RedColor()}/{require}";
    }
    public static string FormatRequire(double owner, double require)
    {
        if (owner >= require)
        {
            return $"{owner.GreenColor()}/{require}";
        }
        return $"{owner.RedColor()}/{require}";
    }
}