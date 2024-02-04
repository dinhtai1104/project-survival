using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using UnityEngine;

public class UITweenScale : UITweenBase
{
    [SerializeField] private Vector3 fromScale;
    private Vector3 startScale;
    bool isInit;
    public override void Init()
    {
        base.Init();
        if (!isInit)
        {
            startScale = transform.localScale;
            transform.localScale = fromScale;
            isInit = true;
        }
    }
    public async override UniTask Show()
    {
        Init();
        await UniTask.Delay(TimeSpan.FromSeconds(DelayIn), ignoreTimeScale: true, cancellationToken: cancelToken.Token);
        await transform.DOScale(startScale, DurationIn).SetUpdate(true).SetEase(settings.transitionInBlendCurve).ToUniTask(cancellationToken: cancelToken.Token);
    }

    public async override UniTask Hide()
    {
        try
        {
            base.Init();
            await UniTask.Delay(TimeSpan.FromSeconds(DelayOut), ignoreTimeScale: true, cancellationToken: cancelToken.Token);
            await transform.DOScale(fromScale, DurationOut).SetUpdate(true).SetEase(settings.transitionOutBlendCurve).ToUniTask(cancellationToken: cancelToken.Token);
        }
        catch (Exception ex)
        {

        }
    }
}
