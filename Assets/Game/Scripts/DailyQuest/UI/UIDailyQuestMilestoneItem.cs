using Assets.Game.Scripts._Services;
using Assets.Game.Scripts.BaseFramework.Architecture;
using Assets.Game.Scripts.Utilities;
using com.mec;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UIDailyQuestMilestoneItem : MonoBehaviour
{
    [SerializeField] private Image chestIconImg;
    [SerializeField] private TextMeshProUGUI milestoneTxt;
    [SerializeField] private GameObject rewardGO;
    [SerializeField] private GameObject noti;
    [SerializeField] private List<UIIconText> rewardPanel;

    private DailyQuestProgressEntity entity;
    private QuestService service;
    public void Setup(DailyQuestProgressEntity entity)
    {
        service = Architecture.Get<QuestService>();
        this.entity = entity;
    }

    public void UpdateUI()
    {
        DOTween.Kill(gameObject);
        transform.localScale = Vector3.one;
        rewardGO.SetActive(false);
        noti.SetActive(false);
        if (service.IsReceiveMilestone(entity.Id))
        {
            if (entity.IsSpecial)
            {
                chestIconImg.sprite = ResourcesLoader.Instance.GetSprite(AtlasName.DailyQuest, "Chest_Special_Open");
            }
            else
            {
                chestIconImg.sprite = ResourcesLoader.Instance.GetSprite(AtlasName.DailyQuest, "Chest_Mini_Open");
            }
        }
        else
        {
            if (entity.IsSpecial)
            {
                chestIconImg.sprite = ResourcesLoader.Instance.GetSprite(AtlasName.DailyQuest, "Chest_Special_Close");
            }
            else
            {
                chestIconImg.sprite = ResourcesLoader.Instance.GetSprite(AtlasName.DailyQuest, "Chest_Mini_Close");
            }
        }
        for (int i = 0; i < rewardPanel.Count; i++)
        {
            if (i >= entity.Reward.Count)
            {
                rewardPanel[i].gameObject.SetActive(false);
            }
            else
            {
                rewardPanel[i].gameObject.SetActive(true);
                rewardPanel[i].Set(entity.Reward[i].Data);
            }
        }
        chestIconImg.SetNativeSize();

        if (service.CanReceiveMilestone(entity.Id))
        {
            noti.SetActive(true);
            transform.DOScale(Vector3.one * 1.1f, 0.2f).SetLoops(-1, LoopType.Yoyo).SetDelay(0.2f).SetId(gameObject);
        }
        else
        {
            noti.SetActive(false);
        }
        milestoneTxt.text = entity.Milestone.TruncateValue();
    }

    private bool showReward = false;

    private IEnumerator<float> _PanelReward()
    {
        showReward = true;
        rewardGO.SetActive(true);
        rewardGO.transform.DOScale(Vector3.one, 0.25f).From(Vector3.zero).SetEase(Ease.OutBack).SetId(rewardGO);
        yield return Timing.WaitForSeconds(1f);
        rewardGO.transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack).OnComplete(() =>
        {
            rewardGO.SetActive(false);
        }).SetId(rewardGO);
        showReward = false;
    }

    public void ReceiveOnClicked()
    {
        if (!service.CanReceiveMilestone(entity.Id))
        {
            DOTween.Kill(rewardGO);
            if (!showReward)
            {
                Timing.RunCoroutine(_PanelReward());
            }
            else
            {
                rewardGO.transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack).OnComplete(() =>
                {
                    rewardGO.SetActive(false);
                }).SetId(rewardGO);
                showReward = false;
            }
            return;
        }
        showReward = false;
        service.ReceiveMilestone(entity.Id);
        PanelManager.ShowRewards(entity.Reward);
        UpdateUI();

        foreach (var rw in entity.Reward)
        {
            string item = "Exp";
            double remaining = 0;
            if (rw.Data is ResourceData)
            {
                item = (rw.Data as ResourceData).Resource.ToString();
                remaining = DataManager.Save.Resources.GetResource((rw.Data as ResourceData).Resource);
            }
            // Track
            FirebaseAnalysticController.Tracker.NewEvent("earn_resource")
                .AddStringParam("item_category", rw.Type.ToString())
                .AddStringParam("item_id", item)
                .AddStringParam("source", "daily_quest_sum")
                .AddIntParam("source_id", entity.Milestone)
                .AddDoubleParam("value", rw.Data.ValueLoot)
                .AddDoubleParam("remaining_value", remaining)
                .AddDoubleParam("total_earned_value", FirebaseAnalysticController.Instance.GetTrackingResourceEarn(EResource.Gem))
                .Track();
        }
    }
}