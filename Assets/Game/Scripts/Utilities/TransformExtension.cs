using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public static class TransformExtension
{
    public static async UniTask Shake(this Transform transform, float time, float power = 1, float scale = 0.1f, CancellationToken cancellationToken = default(CancellationToken))
    {
        float t = time;
        Vector2 defaultPos = transform.localPosition;
        Vector2 pos = Vector2.zero;
        Vector3 s = transform.localScale;
        while (t >= 0)
        {
            if (transform == null) return;
            s.x = s.y = 1 + ((t * scale * power / time));
            transform.localScale = s;
            pos = defaultPos + UnityEngine.Random.insideUnitCircle * (t * 0.15f * power / time);
            transform.localPosition = pos;
            t -= Time.fixedUnscaledDeltaTime;
            await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cancellationToken: cancellationToken);
        }
        transform.localPosition = defaultPos;
    }
    public static async UniTask Shake(this RectTransform transform, float time, float power = 1, float scale = 1f, CancellationToken cancellationToken = default(CancellationToken))
    {
        float t = time;
        Vector2 defaultPos = transform.anchoredPosition;
        Vector2 defaultScale = transform.localScale;
        Vector2 pos = Vector2.zero;
        Vector3 s = transform.localScale;
        while (t >= 0)
        {
            if (transform == null) return;
            s.x = s.y = 1 + ((t * 0.1f * scale / time));
            transform.localScale = s;
            pos = defaultPos + UnityEngine.Random.insideUnitCircle * (t * 1f * power / time);
            transform.anchoredPosition = pos;
            t -= Time.fixedUnscaledDeltaTime;
            await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cancellationToken: cancellationToken);
        }
        transform.anchoredPosition = defaultPos;

    }
}
