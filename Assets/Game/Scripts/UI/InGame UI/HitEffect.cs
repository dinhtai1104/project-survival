using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class HitEffect : UI.Panel
{
    public static HitEffect Instance;
    [SerializeField]
    private MoreMountains.Feedbacks.MMF_Player startFb;
    public override void PostInit()
    {
        Instance = this;
    }
    public override void Deactive()
    {
        gameObject.SetActive(false);
    }
    public override void Active()
    {
        gameObject.SetActive(true);
    }
    public override void Clear()
    {
        base.Clear();
        Instance = null;
    }
    public void SetUp()
    {
        Active();
        startFb?.PlayFeedbacks();
    }
    
}
