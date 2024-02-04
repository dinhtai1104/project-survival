using Game.GameActor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetPackEffect : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem ps;
    Character character;
    private void OnEnable()
    {
        this.character = GetComponentInParent<Character>();
        Messenger.AddListener<Character>(EventKey.JetPackTriggered, OnTriggered);
        Messenger.AddListener(EventKey.JetPackReleased, OnReleased);
    }
    private void OnDisable()
    {
        Messenger.RemoveListener<Character>(EventKey.JetPackTriggered, OnTriggered);
        Messenger.RemoveListener(EventKey.JetPackReleased, OnReleased);
    }
    private void OnDestroy()
    {
        Messenger.RemoveListener<Character>(EventKey.JetPackTriggered, OnTriggered);
        Messenger.RemoveListener(EventKey.JetPackReleased, OnReleased);
    }
    private void OnReleased()
    {
        ps.Stop();
    }

    private void OnTriggered(Character character)
    {
        if (this.character != character) return;
        ps.Play();
    }
}
