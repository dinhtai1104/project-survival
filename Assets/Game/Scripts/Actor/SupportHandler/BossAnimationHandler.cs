using com.mec;
using Spine.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BossAnimationHandler : EnemyAnimationHandler
{
    private bool isGetHit = false;
    private CoroutineHandle getHitEff;
    public override void SetGetHit()
    {
        if (!isGetHit)
        {
            isGetHit = true;
            getHitEff = Timing.RunCoroutine(_GetHit());
        }
    }

    private IEnumerator<float> _GetHit()
    {
        ColorUtility.TryParseHtmlString("#ff8080", out Color color);
        anim.Skeleton.SetColor(color);
        yield return Timing.WaitForSeconds(0.05f);
        anim.Skeleton.SetColor(Color.white);
        isGetHit = false;
    }
}