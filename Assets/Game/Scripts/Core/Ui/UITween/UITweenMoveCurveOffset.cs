using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class UITweenMoveCurveOffset : UITweenBase
{
    public RectTransform target;
    public AnimationCurve curveOffset;
    private Vector3 startPos;
    private Vector3 endPos;
    public async override void Init()
    {
        base.Init();
        await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: cancelToken.Token);

        endPos = target.position;
        startPos = (transform as RectTransform).position;
    }
    public async override UniTask Hide()
    {
    }

    public async override UniTask Show()
    {
        (transform as RectTransform).position = startPos;
        await UniTask.Delay(TimeSpan.FromSeconds(DelayIn), cancellationToken: cancelToken.Token);
        (transform as RectTransform).position = startPos;

        var dir = endPos - startPos;
        float time = 0;
        while (time < DurationIn)
        {
            var target = Vector3.Lerp(startPos, endPos, settings.transitionInBlendCurve.Evaluate(time / DurationIn));
            var offsetY = curveOffset.Evaluate((time / DurationIn) * 100f);
            (transform as RectTransform).position = target + new Vector3(0, offsetY) / 100f;
            time += Time.deltaTime;
            await UniTask.Delay(TimeSpan.FromSeconds(Time.deltaTime));
        }
    }

    private void OnDrawGizmos()
    {
        
    }
}