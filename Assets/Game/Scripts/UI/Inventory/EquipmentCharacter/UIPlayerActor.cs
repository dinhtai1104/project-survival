using Coffee.UIExtensions;
using com.mec;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UIPlayerActor : UIActor
{
    [SerializeField] protected UIParticle _vfxAppear;

    public void PlayVFXAppear()
    {
        _vfxAppear = GetComponentInChildren<UIParticle>();
        if (_vfxAppear != null)
        {
            _vfxAppear.Stop();
            _vfxAppear.Play();
        }
    }
    public void PlayVFXAppearDelay(float delay)
    {
        Timing.RunCoroutine(_Delay(delay), gameObject);
    }

    private IEnumerator<float> _Delay(float delay)
    {
        yield return Timing.WaitForSeconds(delay);
        PlayVFXAppear();
    }
    private void OnDisable()
    {
        Timing.KillCoroutines(gameObject);
    }
}