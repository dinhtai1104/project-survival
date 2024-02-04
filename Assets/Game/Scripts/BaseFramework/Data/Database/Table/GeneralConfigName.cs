using System;
using UnityEngine;

public static class GeneralConfigName
{
    public readonly static string FORMULA_EXP_MONSTER = "n% * (level - 1) * stat_base";
    public readonly static string START_COIN = "START_COIN";
    public readonly static string START_GEM = "START_GEM";
    public readonly static string START_DIAMOND = "START_DIAMOND";
}

/// <summary>
/// Generic Try Get Value From GeneralConfig
/// </summary>
public static class GeneralClassTryGetValue
{
    public static Vector2 TryGetVector2(this string s)
    {
        try 
        {
            var split = s.Split(';');
            return new Vector2(float.Parse(split[0]), float.Parse(split[1]));
        }
        catch (System.Exception e)
        {
            return Vector2.zero;
        }
    }
    public static Vector3 TryGetVector3(this string s)
    {
        try
        {
            var split = s.Split(';');
            if (split.Length == 3)
            {
                return new Vector3(float.Parse(split[0]), float.Parse(split[1]), float.Parse(split[2]));
            }
            return s.TryGetVector2();
        }
        catch (System.Exception e)
        {
            return Vector3.zero;
        }
    }
    public static float TryGetFloat(this string s)
    {
        if (float.TryParse(s, out float x))
            return x;
        return 0;
    }
    public static int TryGetInt(this string s)
    {
        if (int.TryParse(s, out int x))
            return x;
        return 0;
    }
    public static long TryGetLong(this string s)
    {
        long.TryParse(s, out long x);
        return x;
    }
    public static bool TryGetBool(this string s)
    {
        return bool.Parse(s);
    }
}