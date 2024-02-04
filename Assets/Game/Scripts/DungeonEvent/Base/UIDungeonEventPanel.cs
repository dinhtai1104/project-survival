using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UIDungeonEventPanel : UI.Panel
{
    [SerializeField] private RectTransform content;
    [SerializeField] private UIDungeonEventItem itemPrefab;
    private List<UIDungeonEventItem> items;
    public override void PostInit()
    {
    }
    public override void Show()
    {
        base.Show();
        items = new List<UIDungeonEventItem>();
        var config = DataManager.Base.DungeonEventConfig;

        foreach (var ev in config.Dictionary)
        {
            var item = PoolManager.Instance.Spawn(itemPrefab, content);
            items.Add(item);
            item.onJoinDungeonEvent += OnJoinDungeon;
            item.Setup(ev.Key);
        }
    }

    private void OnJoinDungeon(EDungeonEvent dungeon)
    {
        var save = DataManager.Save.DungeonEvent.Saves[dungeon];
        DataManager.Save.DungeonEventSession.JoinDungeon(save.CurrentDungeon);
        DataManager.Save.DungeonEventSession.SetDungeonEvent(dungeon);
        DataManager.Save.DungeonEventSession.Save();
        Game.Controller.Instance.StartLevel(GameMode.DungeonEvent, save.CurrentDungeon, dungeon).Forget();
    }

    public override void Close()
    {
        foreach (var it in items)
        {
            it.onJoinDungeonEvent -= OnJoinDungeon;
            PoolManager.Instance.Despawn(it.gameObject);
        }
        base.Close();
    }
}