﻿using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading;

namespace Foundation.Game.Time
{
    public class UnbiasedTime : MonoBehaviour
    {

        private static UnbiasedTime instance;
        public static UnbiasedTime Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<UnbiasedTime>();
                    if (instance == null)
                    {
                        var gameObject = new GameObject("UnbiasedTime Manager");
                        instance = gameObject.AddComponent<UnbiasedTime>();
                    }
                }
                return instance;
            }
        }

        // Estimated difference in seconds between device time and real world time
        // timeOffset = deviceTime - worldTime;
        public long timeOffset = 0;

        private void Awake()
        {
            SessionStart();
            DontDestroyOnLoad(gameObject);
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                SessionEnd();
            }
            else
            {
                SessionStart();
            }
        }

        private void OnApplicationQuit()
        {
            SessionEnd();
        }

        public static DateTime UtcNow
        {
            get
            {
#if UNITY_EDITOR
                // Debug.Log(TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow));
                return DateTime.UtcNow;
#endif
#pragma warning disable 162
#if UNITY_EDITOR || DEVELOPMENT
                return DateTime.UtcNow;
#else
            return Instance.Now().ToUniversalTime();
#endif
#pragma warning restore 162
            }
        }

        // Returns estimated DateTime value taking into account possible device time changes
        public DateTime Now()
        {
            return DateTime.Now.AddSeconds(-1.0f * timeOffset);
        }

        // timeOffset value is cached for performance reasons (calls to native plugins can be expensive). 
        // This method is used to update offset value in cases if you think device time was changed by user. 
        // 
        // However, time offset is updated automatically when app gets backgrounded or foregrounded. 
        // 
        public void UpdateTimeOffset()
        {
#if UNITY_ANDROID
        UpdateTimeOffsetAndroid();
#elif UNITY_IPHONE
		UpdateTimeOffsetIOS();
#endif
        }

        // Returns true if native plugin was unable to calculate unbiased time and had fallen back to device DateTime. 
        // This can happen after device reboot. Player can cheat by closing the game, changing time and rebooting device. 
        // This method can help tracking this situation. 
        public bool IsUsingSystemTime()
        {
#if UNITY_ANDROID
        return UsingSystemTimeAndroid();
#elif UNITY_IPHONE
			return UsingSystemTimeIOS();
#else
            return true;
#endif
        }

        private void SessionStart()
        {
#if UNITY_ANDROID
        StartAndroid();
#elif UNITY_IPHONE
			StartIOS();
#endif
        }

        private void SessionEnd()
        {
#if UNITY_ANDROID
        EndAndroid();
#elif UNITY_IPHONE
			EndIOS();
#endif
        }

        // Platform specific code
        // 

#if UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void _vtcOnSessionStart();

	[DllImport ("__Internal")]
	private static extern void _vtcOnSessionEnd();
	
	[DllImport ("__Internal")]
	private static extern int _vtcTimestampOffset();

	[DllImport ("__Internal")]
	private static extern int _vtcUsingSystemTime();

	private void UpdateTimeOffsetIOS() {
		if (Application.platform != RuntimePlatform.IPhonePlayer) {
			return;
		}

		timeOffset = _vtcTimestampOffset();
	}

	private void StartIOS() {
		if (Application.platform != RuntimePlatform.IPhonePlayer) {
			return;
		}

		_vtcOnSessionStart();
		timeOffset = _vtcTimestampOffset();
	}

	private void EndIOS() {
		if (Application.platform != RuntimePlatform.IPhonePlayer) {
			return;
		}

		_vtcOnSessionEnd();
	}

	private bool UsingSystemTimeIOS() {
		if (Application.platform != RuntimePlatform.IPhonePlayer) {
			return true;
		}
		
		return _vtcUsingSystemTime() != 0;
	}
#endif


#if UNITY_ANDROID
    private void UpdateTimeOffsetAndroid() {
        if (Application.platform != RuntimePlatform.Android) {
            return;
        }

        using (var activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (var unbiasedTimeClass = new AndroidJavaClass("com.vasilij.unbiasedtime.UnbiasedTime")) {
            var playerActivityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
            if (playerActivityContext != null && unbiasedTimeClass != null) {
                timeOffset = unbiasedTimeClass.CallStatic<long>("vtcTimestampOffset", playerActivityContext);
            }
        }
    }

    private void StartAndroid() {
        if (Application.platform != RuntimePlatform.Android) {
            return;
        }

        using (var activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (var unbiasedTimeClass = new AndroidJavaClass("com.vasilij.unbiasedtime.UnbiasedTime")) {
            var playerActivityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
            if (playerActivityContext != null && unbiasedTimeClass != null) {
                unbiasedTimeClass.CallStatic("vtcOnSessionStart", playerActivityContext);
                timeOffset = unbiasedTimeClass.CallStatic<long>("vtcTimestampOffset");
            }
        }
    }

    private void EndAndroid() {
        if (Application.platform != RuntimePlatform.Android) {
            return;
        }

        using (var activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (var unbiasedTimeClass = new AndroidJavaClass("com.vasilij.unbiasedtime.UnbiasedTime")) {
            var playerActivityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
            if (playerActivityContext != null && unbiasedTimeClass != null) {
                unbiasedTimeClass.CallStatic("vtcOnSessionEnd", playerActivityContext);
            }
        }
    }

    private bool UsingSystemTimeAndroid() {
        if (Application.platform != RuntimePlatform.Android) {
            return true;
        }

        using (var activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (var unbiasedTimeClass = new AndroidJavaClass("com.vasilij.unbiasedtime.UnbiasedTime")) {
            var playerActivityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
            if (playerActivityContext != null && unbiasedTimeClass != null) {
                return unbiasedTimeClass.CallStatic<bool>("vtcUsingDeviceTime");
            }
        }

        return true;
    }
#endif
    }
}