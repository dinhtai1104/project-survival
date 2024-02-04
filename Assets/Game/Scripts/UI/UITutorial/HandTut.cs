using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EHandTut
{
    MovePad,JumpButton,SkillButton
}
public class HandTut : MonoBehaviour
{
    public EHandTut handTut;
    private void Start()
    {

        transform.localScale = Vector3.zero;
    }
    private void OnEnable()
    {
        Messenger.AddListener<EHandTut>(EventKey.ShowHandTut, Show);
        Messenger.AddListener<EHandTut>(EventKey.HideHandTut, Hide);
    }

    private void OnDisable()
    {
        Messenger.RemoveListener<EHandTut>(EventKey.ShowHandTut, Show);
        Messenger.RemoveListener<EHandTut>(EventKey.HideHandTut, Hide);
    }
    private void OnDestroy()
    {
        Messenger.RemoveListener<EHandTut>(EventKey.ShowHandTut, Show);
        Messenger.RemoveListener<EHandTut>(EventKey.HideHandTut, Hide);
    }
    private void Hide(EHandTut type)
    {
        if (handTut == type)
        {
            transform.localScale = Vector3.zero;
            //gameObject.SetActive(false);
        }
    }

  
    private void Show(EHandTut type)
    {
        if (handTut == type)
        {
            transform.localScale = Vector3.one;

            //gameObject.SetActive(true);
        }
    }
}
