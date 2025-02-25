using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using ExtensionKit;
using Mosframe;

public static class Logger
{
    public static void Log(object message)
    {
        Debug.Log(message.ToString());
    }

    public static void Log(object message, Color color)
    {
        Debug.Log($"<color={color.toHtmlColor()}>{message.ToString()}</color>");
    }

    public static void LogError(object message)
    {
        Debug.LogError( message.ToString());
    }
    public static void LogWarning(object message)
    {
        //Debug.LogWarning( message.ToString());
    }
}
