using com.mec;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScrollBackground : MonoBehaviour
{
    [SerializeField] private RawImage bg;
    [SerializeField] private Vector2 offsetScroll;
    private Rect rect;
    private void OnEnable()
    {
        Timing.RunCoroutine(_Update(), gameObject);
    }

    private void OnDisable()
    {
        Timing.KillCoroutines(gameObject);
    }
    private IEnumerator<float> _Update()
    {
        rect = bg.uvRect;
        while (true)
        {
            rect.x += offsetScroll.x * Time.deltaTime;
            rect.y += offsetScroll.y * Time.deltaTime;
            bg.uvRect = rect;
            yield return Time.deltaTime;
        }
    }
}