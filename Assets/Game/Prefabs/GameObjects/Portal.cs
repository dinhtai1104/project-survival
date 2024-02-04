using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField]
    private bool buffPortal;

    [SerializeField]
    private MoreMountains.Feedbacks.MMF_Player idleFb, activeFb, triggerFb,teleportFb,teleportQuickFb;
    bool isActivated;
    public bool quickTeleport = false;


    public SpritePack[] spritePacks;

    [System.Serializable]
    public struct SpritePack
    {
        public Sprite[] sprites;
    }

    [SerializeField]
    private SpriteRenderer portalSR;
    private void OnEnable()
    {
        Messenger.AddListener<bool>(EventKey.GameClear, OnStageCleared);
        idleFb.PlayFeedbacks();
        GameController.onStageReady += OnStageInit;

        
    }

    private void OnStageInit(int mode,int dungeonId, int stageId, EDungeonEvent eventType)
    {
        //portalSR.sprite = portalSprites[dungeonId];
        portalSR.sprite = spritePacks[mode].sprites[eventType == EDungeonEvent.None ? dungeonId : (int)eventType];
    }



    private void OnDisable()
    {
        Messenger.RemoveListener<bool>(EventKey.GameClear, OnStageCleared);
        GameController.onStageReady -= OnStageInit;
    }
    public void OnPortalTeleport()
    {
        if (quickTeleport)
        {
            teleportQuickFb.PlayFeedbacks();
        }
        else
        {
            teleportFb.PlayFeedbacks();
        }
    }
    private void OnStageCleared(bool instantClear)
    {
        isActivated = true;
        Invoke(nameof(DelayActive), 1.5f);
    }
    void DelayActive()
    {
        idleFb.StopFeedbacks();
        activeFb.PlayFeedbacks();
    }
    public void OnPlayerEnter()
    {
        triggerFb.PlayFeedbacks();
        Messenger.Broadcast(EventKey.TriggerPortal);
    }
    public void OnTeleportFinish()
    {
        Messenger.Broadcast(EventKey.StageFinish, buffPortal);
    }

   
    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (!isActivated) return;
    //    if (collision.CompareTag("Player"))
    //    {
    //        isActivated = false;
    //        OnPlayerEnter();
    //    }
    //}


}
