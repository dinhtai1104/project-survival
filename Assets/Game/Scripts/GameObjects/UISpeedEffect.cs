using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISpeedEffect : UI.Panel
{
    public static UISpeedEffect Instance;
    [SerializeField]
    private MoreMountains.Feedbacks.MMF_Player startFb,endFb;
    public override void PostInit()
    {
        Instance = this;
        Messenger.AddListener<bool>(EventKey.SpeedBoost, OnBoosted);

    }
    public override void OnDestroy()
    {
        base.OnDestroy();
    }
    public override void Deactive()
    {
        gameObject.SetActive(false);
    }
    private void OnBoosted(bool isBoosted)
    {
        if (isBoosted)
        {
            Active();
            startFb?.PlayFeedbacks();
        }
        else
        {
            startFb?.StopFeedbacks();
            endFb?.PlayFeedbacks();
        }
    }
    public override void Active()
    {
        gameObject.SetActive(true);
    }
    public override void Clear()
    {
        base.Clear();
        Instance = null;
        Messenger.RemoveListener<bool>(EventKey.SpeedBoost, OnBoosted);

    }
    public void SetUp()
    {
        Active();
        startFb?.PlayFeedbacks();
    }

}
