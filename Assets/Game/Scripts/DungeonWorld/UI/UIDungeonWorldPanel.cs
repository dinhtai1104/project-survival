using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDungeonWorldPanel : UI.Panel
{
    [SerializeField] private UIDungeonWorldItem itemPrefab;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private UIDungeonWorldReward rewardPanel;
    private List<UIDungeonWorldItem> items = new List<UIDungeonWorldItem>();

    public override void PostInit()
    {
    }
    public override void Show()
    {
        base.Show();
        var table = DataManager.Base.Dungeon;
        for (int i = 0; i < table.Dictionary.Count; i++)
        {
            var entity = table.Get(i);
            if (entity == null) continue;
            var item = PoolManager.Instance.Spawn(itemPrefab, scrollRect.content);
            items.Add(item);
            item.Set(table.Get(i));
            item.onClicked += Item_onClicked;
        }
        onClosed += ClosedCallback;
    }

    public void Show(int dungeon)
    {
        Show();
        var item = items.Find(t => t.Dungeon == dungeon);
        if (item == null) return;
        var contentSize = scrollRect.content.sizeDelta.x;
        var posX = (item.transform as RectTransform).anchoredPosition.x;
        var normal = posX / contentSize;
        scrollRect.normalizedPosition = Vector2.right * normal;
        rewardPanel.item = item;

        item.Register(rewardPanel);
    }

    private void Item_onClicked(UIDungeonWorldItem item)
    {
        rewardPanel.DeRegister();
        rewardPanel.transform.SetParent(transform);
        rewardPanel.gameObject.SetActive(false);
        rewardPanel.item = item;
        item.Register(rewardPanel);
    }

    private void ClosedCallback()
    {
        PoolManager.Instance.Clear(itemPrefab.gameObject);
        items.Clear();
    }
}
