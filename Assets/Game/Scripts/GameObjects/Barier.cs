using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barier : MonoBehaviour
{
    [SerializeField]
    private MoreMountains.Feedbacks.MMF_Player triggerFb;
    bool isTriggered = false;

    bool waitForChest = false;

    public SpritePack[] spritePacks;

    [System.Serializable]
    public struct SpritePack
    {
        public Sprite[] sprites;
    }


    [SerializeField]
    private SpriteRenderer sr;
    private void Start()
    {
    }
    private void OnEnable()
    {
        waitForChest = false;
        GetComponent<BoxCollider2D>().enabled = true;
        isTriggered = false;
        Messenger.AddListener<bool>(EventKey.GameClear, OnGameClear);
        Messenger.AddListener(EventKey.BossChestSpawned, OnChestSpawned);
        Messenger.AddListener(EventKey.BossChestHidden, OnChestHidden);
        GameController.onStageReady += OnStageInit;
    }

    private void OnStageInit(int mode,int dungeonId, int stageId, EDungeonEvent eventType)
    {
        //sr.sprite = sprites[dungeonId];
        sr.sprite = spritePacks[mode].sprites[eventType == EDungeonEvent.None ? dungeonId : (int)eventType];
    }

    private void OnChestHidden()
    {
        waitForChest = false;

    }

    private void OnChestSpawned()
    {
        waitForChest = true;
    }

    private void OnDisable()
    {
        Messenger.RemoveListener<bool>(EventKey.GameClear, OnGameClear);
        Messenger.RemoveListener(EventKey.BossChestSpawned, OnChestSpawned);
        Messenger.RemoveListener(EventKey.BossChestHidden, OnChestSpawned);
        GameController.onStageReady -= OnStageInit;

    }
    private void OnGameClear(bool instantClear)
    {
        if (isTriggered) return;
        isTriggered = true;

        Hide();
    }
  

    async UniTask Hide()
    {
        await UniTask.Delay(250);
        await UniTask.WaitUntil(() => !waitForChest);
        triggerFb?.PlayFeedbacks();

    }
}
