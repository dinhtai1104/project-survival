using Cysharp.Threading.Tasks;
using Game.GameActor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TrackingTag : MonoBehaviour
{
    public ScreenCharacterTag screenTag;
    private ActorBase actor;
    public string Tag;
    bool isTriggered = false;
    bool active = false;
    private void OnEnable()
    {
        active = false;
        actor = GetComponentInParent<ActorBase>();
        Messenger.AddListener<int, int>(EventKey.GameStart, OnGameStart);
        Messenger.AddListener<bool>(EventKey.GameClear, OnGameClear);
    }

    private void OnGameClear(bool arg1)
    {
        active = false;
        HideTag();

    }

    private void OnGameStart(int arg1, int arg2)
    {
        active = true;
    }

    private void OnDisable()
    {
        HideTag();
    }
    private void OnBecameVisible()
    {
        if (!isTriggered ||!active) return;
        WaitForTag().ContinueWith(HideTag).Forget();
    }
    void HideTag()
    {
        isTriggered = false;
        if (screenTag != null)
        { 
            screenTag.SetActive(false);
            screenTag = null;
        }
    }
    void ShowTag()
    {
        screenTag.SetUp(actor);
    }

    async UniTask WaitForTag()
    {
        await UniTask.WaitUntil(() => screenTag != null);
    }
    private void OnBecameInvisible()
    {
        if (actor.IsDead() || !actor.gameObject.activeSelf || !active) return;
        isTriggered = true;
        Game.Pool.GameObjectSpawner.Instance.GetAsync($"ScreenCharacterTag {Tag}").ContinueWith(obj =>
        {
            screenTag = obj.GetComponent<ScreenCharacterTag>();
            ShowTag();
        }).Forget();

    }
}
