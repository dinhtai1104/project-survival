using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class UITweenRunner : MonoBehaviour
{
    [SerializeField] private UITweenBase[] transitions;

    private CancellationTokenSource cancelToken;
    public async UniTask Show()
    {
        cancelToken = new CancellationTokenSource();
        if (transitions != null && transitions.Length > 0)
        {
            var listTask = new List<UniTask>();
            foreach (var transition in transitions)
            {
                listTask.Add(transition.Show());
            }
            await UniTask.WhenAll(listTask).AttachExternalCancellation(cancelToken.Token);
        }
    }

    public async UniTask Close()
    {
        cancelToken = new CancellationTokenSource();
        if (transitions != null && transitions.Length > 0)
        {
            var listTask = new List<UniTask>();
            foreach (var transition in transitions)
            {
                listTask.Add(transition.Hide());
            }
            await UniTask.WhenAll(listTask).AttachExternalCancellation(cancelToken.Token);
        }
    }
    private void OnDisable()
    {
        cancelToken?.Cancel();
        cancelToken?.Dispose();
        cancelToken = null;
    }

    public virtual async void Stop()
    {
        if (transitions != null && transitions.Length > 0)
        {
            var listTask = new List<UniTask>();
            foreach (var transition in transitions)
            {
                listTask.Add(transition.Hide());
            }
            await UniTask.WhenAll(listTask);
        }
    }
}