using Assets.Game.Scripts._Services;
using Assets.Game.Scripts.BaseFramework.Architecture;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIAchievementQuestPage : UIContentTab
{
    [SerializeField] private UIAchievementQuestItem itemPrefab;
    [SerializeField] private RectTransform contentAchievement;

    private List<UIAchievementQuestItem> items = new List<UIAchievementQuestItem>();
    private bool isInit = false;
    private AchievementService service;
    private AchievementQuestTable table;
    private List<AchievementQuestSave> saveList = new List<AchievementQuestSave>();
    public override void Show()
    {
        base.Show();
        service = Architecture.Get<AchievementService>();
        table = DataManager.Base.AchievementQuest;
        if (!isInit)
        {
            isInit = true;
            
            foreach (var model in table.Dictionary)
            {
                var item = PoolManager.Instance.Spawn(itemPrefab, contentAchievement);
                items.Add(item);
                saveList.Add(service.GetSave(model.Value.Type));
            }

        }
        UpdateUI();
    }
    public void UpdateUI()
    {
        saveList = saveList.OrderByDescending(save =>
        {
            return service.CanReceive(save.Type);
        }).ThenBy(save =>
        {
            return service.IsLootAll(save.Type);
        }).ToList();

        int index = 0;
        foreach (var item in items)
        {
            item.Setup(saveList[index++], GetComponentInParent<UIQuestPanel>());
        }
    }
}