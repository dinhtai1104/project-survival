using Assets.Game.Scripts._Services;
using Assets.Game.Scripts.BaseFramework.Architecture;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UIDailyQuestMilestoneBar : MonoBehaviour
{
    [SerializeField] private RectTransform _viewportRect;
    [SerializeField] private UIDailyQuestMilestoneItem _miniChestPrefab;
    [SerializeField] private RectTransform _filledRect;
    [SerializeField] private float sizeXFilledRect;

    private List<UIDailyQuestMilestoneItem> items = new List<UIDailyQuestMilestoneItem>();

    private QuestService service;
    private DailyQuestProgressTable dailyQuestProgress;

    private bool isInit = false;

    private void OnEnable()
    {
        Messenger.AddListener(EventKey.ClaimQuest, OnClaimMission);
    }

    private void OnClaimMission()
    {
        UpdateUI();
    }

    private void OnDisable()
    {
        Messenger.RemoveListener(EventKey.ClaimQuest, OnClaimMission);
    }
    [Button]
    public void Setup()
    {
        dailyQuestProgress = DataManager.Base.DailyQuestProgress;
        service = Architecture.Get<QuestService>();
        _filledRect.sizeDelta = (new Vector2(0, _filledRect.sizeDelta.y));

        if (!isInit)
        {

            int number = dailyQuestProgress.Dictionary.Count;
            var width = _viewportRect.sizeDelta.x;
            var posIncrease = width / (number - 1);

            var listPos = new List<float>();
            var sum = dailyQuestProgress.Get(dailyQuestProgress.Dictionary.Count - 1).Milestone;

            foreach (var model in dailyQuestProgress.Dictionary)
            {
                var percent = model.Value.Milestone * 1.0f / sum;
                listPos.Add(percent * width);
            }
            
            
            for (int i = 0; i < number; i++)
            {
                var item = PoolManager.Instance.Spawn(_miniChestPrefab, _viewportRect);
                item.GetComponent<RectTransform>().anchoredPosition = new Vector2(listPos[i], 0);
                item.Setup(dailyQuestProgress.Dictionary[i]);
                items.Add(item);
            }

            isInit = true;
        }
        
        UpdateUI();
    }

    public void UpdateUI()
    {
        dailyQuestProgress = DataManager.Base.DailyQuestProgress;
        service = Architecture.Get<QuestService>();
        
        var target = sizeXFilledRect * service.Medal / dailyQuestProgress.TotalScore;
        target = Mathf.Clamp(target, 0, sizeXFilledRect);
        if (_filledRect.sizeDelta.x < target)
        {
            _filledRect.DOSizeDelta(new Vector2(target, _filledRect.sizeDelta.y), 1f);
        }
        foreach(var item in items)
        {
            item.UpdateUI();
        }
    }
}