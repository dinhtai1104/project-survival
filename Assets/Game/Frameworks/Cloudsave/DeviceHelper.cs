using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DeviceHelper
{
    public static bool GetDeviceId(out string deviceId)
    {
        deviceId = string.Empty;
        if (CheckForSupportedMobilePlatform())
        {
#if UNITY_ANDROID
            AndroidJavaClass clsUnity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject objActivity = clsUnity.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject objResolver = objActivity.Call<AndroidJavaObject>("getContentResolver");
            AndroidJavaClass clsSecure = new AndroidJavaClass("android.provider.Settings$Secure");
            deviceId = clsSecure.CallStatic<string>("getString", objResolver, "android_id");
#endif

#if UNITY_IPHONE
            deviceId = UnityEngine.iOS.Device.vendorIdentifier;
#endif
            Debug.Log($"DeviceID: {deviceId}");
            return true;
        }
        else
        {
            deviceId = SystemInfo.deviceUniqueIdentifier;
#if UNITY_EDITOR
            return true;
#endif
            return false;
        }
    }

    private static bool CheckForSupportedMobilePlatform()
    {
        return Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer;
    }
}