using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public abstract class UITweenBase : MonoBehaviour, ITransitionElement
{
    public ETweenRun RunType = ETweenRun.Auto;
    [SerializeField] protected UITweenFadeMoveSettings settings;
    protected CancellationTokenSource cancelToken;

    [SerializeField] protected float delayIn;
    [SerializeField] protected float delayOut;
    [Range(0f, 1f)]
    [SerializeField] protected float scaleFactor = 1f;

    public float DelayIn => delayIn * scaleFactor;
    public float DelayOut => delayOut * scaleFactor;
    public float DurationIn => settings.transitionInDuration * scaleFactor;
    public float DurationOut => settings.transitionOutDuration * scaleFactor;
    public virtual bool IsCompleted { get; }

    public abstract UniTask Hide();
    public async UniTask AutoShow()
    {
        if (RunType == ETweenRun.Auto)
        {
            await Show();
        }
    }
    public async UniTask AutoHide()
    {
        if (RunType == ETweenRun.Auto)
        {
            await Hide();
        }
    }

    public virtual void Init()
    {
        if (cancelToken != null) { cancelToken.Cancel(); }
        cancelToken = new CancellationTokenSource();
    }

    public abstract UniTask Show();
}