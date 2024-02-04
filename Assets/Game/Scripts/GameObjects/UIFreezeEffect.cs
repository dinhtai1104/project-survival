using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFreezeEffect : UI.Panel
{
    public static UIFreezeEffect Instance;
    [SerializeField]
    private MoreMountains.Feedbacks.MMF_Player startFb,endFb;
    public override void PostInit()
    {
        Instance = this;
        Messenger.AddListener<bool>(EventKey.FreezeTime, OnTimeFreezed);

    }
    public override void OnDestroy()
    {
        base.OnDestroy();
    }
    public override void Deactive()
    {
        gameObject.SetActive(false);
    }
    private void OnTimeFreezed(bool isFreezed)
    {
        if (isFreezed)
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
        Messenger.RemoveListener<bool>(EventKey.FreezeTime, OnTimeFreezed);

    }
    public void SetUp()
    {
        Active();
        startFb?.PlayFeedbacks();
    }

}
