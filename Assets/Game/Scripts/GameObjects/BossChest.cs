using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BossChest : MonoBehaviour
{
    public EChest type;
    public MMF_Player openFb;
    bool isOpenned = false;
    private void OnEnable()
    {
        isOpenned = false;
        Messenger.AddListener(EventKey.BossChestHidden, Hide);


    }
    private void OnDisable()
    {
        Messenger.RemoveListener(EventKey.BossChestHidden, Hide);
    }
    public void Trigger()
    {
        if (isOpenned) return;
        isOpenned = true;
        openFb.PlayFeedbacks();
    }

    public void ShowUI()
    {
        GameUIPanel.Instance.inputController.SetActive(false);
        Messenger.Broadcast(EventKey.BossChestTrigger, type);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
        GameUIPanel.Instance.inputController.SetActive(true);
    }
}
