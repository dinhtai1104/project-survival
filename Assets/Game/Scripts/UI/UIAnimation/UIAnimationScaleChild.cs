using com.mec;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimationScaleChild : MonoBehaviour
{
    public float delay = 0;
    [SerializeField] private List<RectTransform> childs = new List<RectTransform>();

    private void OnEnable()
    {
        Timing.RunCoroutine(_Scale(), Segment.RealtimeUpdate, gameObject);
    }

    private void OnDisable()
    {
        Timing.KillCoroutines(gameObject);
        DOTween.Kill(gameObject);
    }

    private IEnumerator<float> _Scale()
    {
        foreach (var child in childs)
        {
            child.localScale = Vector3.zero;
        }
        yield return Timing.WaitForSeconds(delay);
        foreach (var child in childs)
        {
            child.DOScale(Vector3.one * 1.1f, 0.2f).OnComplete(() =>
            {
                child.DOScale(Vector3.one, 0.1f).SetUpdate(true).SetId(gameObject);
            }).SetUpdate(true).SetId(gameObject);
            yield return Timing.WaitForSeconds(0.05f);
        }
    }
}