using com.mec;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class UITweenFadeMove : UITweenBase
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Vector2 offset;

    private CanvasGroup[] canvases;
    private Vector2 startPos;
    private Vector2 targetPos;

    private bool completed;

    private bool isInit = false;

    private void Awake()
    {
       
    }
    public override void Init()
    {
        base.Init();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 0;
        if (!isInit)
        {
            startPos = (transform as RectTransform).localPosition;
            targetPos = startPos + offset;
            (transform as RectTransform).localPosition = targetPos;
            isInit = true;
            return;
        }
       
        (transform as RectTransform).localPosition = targetPos;
        canvasGroup.alpha = 0;
    }

    public override async UniTask Hide()
    {
        completed = false;
        await UniTask.Delay(TimeSpan.FromSeconds(DelayOut), ignoreTimeScale: true, cancellationToken: cancelToken.Token);
        try
        {
            var list = new List<UniTask>();
            list.Add((transform as RectTransform).DOLocalMove(targetPos, DurationOut).SetEase(settings.transitionOutBlendCurve).SetUpdate(true).ToUniTask());

            list.Add(canvasGroup.DOFade(0, DurationOut).SetEase(settings.transitionOutBlendCurve).SetUpdate(true).ToUniTask());
            await UniTask.WhenAll(list).AttachExternalCancellation(cancelToken.Token);
        }
        catch (Exception ex)
        {

        }
        completed = true;
    }

    public override async UniTask Show()
    {
        completed = false;
        await UniTask.Delay(TimeSpan.FromSeconds(DelayIn), ignoreTimeScale: true, cancellationToken: cancelToken.Token);
        try
        {
            var list = new List<UniTask>();
            list.Add((transform as RectTransform).DOLocalMove(startPos, DurationIn).SetEase(settings.transitionInBlendCurve).SetUpdate(true).ToUniTask());
            list.Add(canvasGroup.DOFade(1, DurationIn).SetEase(settings.transitionInBlendCurve).SetUpdate(true).ToUniTask());
            await UniTask.WhenAll(list).AttachExternalCancellation(cancelToken.Token);
        }
        catch (Exception ex)
        {

        }
        completed = true;
    }
}