using GameUtility;
using System;
using System.Collections.Generic;
using UI;
using UnityEngine;

public class BossChestDropListener : MonoBehaviour
{
    private void OnEnable()
    {
        Messenger.AddListener(EventKey.BossChestSpawned, OnBossChestSpawned);
        Messenger.AddListener(EventKey.BossChestHidden, OnBossChestHidden);
        Messenger.AddListener<EChest>(EventKey.BossChestTrigger, OnBossChestTrigger);
    }

    private void OnDisable()
    {
        Messenger.RemoveListener(EventKey.BossChestSpawned, OnBossChestSpawned);
        Messenger.RemoveListener(EventKey.BossChestHidden, OnBossChestHidden);
        Messenger.RemoveListener<EChest>(EventKey.BossChestTrigger, OnBossChestTrigger);
    }

    private async void OnBossChestTrigger(EChest chest)
    {
        try
        {
            var session = GameController.Instance.GetSession();
            var room = session.MemoryMap.GetRoom(session.CurrentStage);

            var rewardRoom = DataManager.Base.DungeonRoomReward.Get(room).ListRewardRandomInRoom.RandomWeighted(out int index);
            var path = rewardRoom.Chest == 1 ? AddressableName.UIChestRewardSilverBossPanel : AddressableName.UIChestRewardGoldenBossPanel;

            var chestPanel = await PanelManager.CreateAsync<UIChestRewardBasePanel>(path);
            chestPanel.SetRewardRaw(rewardRoom.RewardsRaw);

            var merge = new List<LootParams>();
            foreach (var all in rewardRoom.RewardsRaw)
            {
                merge.Add(all[0]);
            }

            merge = merge.Refactor();

            chestPanel.SetReward(chest, merge);
            chestPanel.onClosed += OnClose;
            chestPanel.Show();
            await chestPanel.Stop();
        }
        catch(System.Exception e)
        {
            Logger.LogError(e);
            OnClose();
        }
    }

    private void OnClose()
    {
        Messenger.Broadcast(EventKey.BossChestHidden);

    }

    private void OnBossChestHidden()
    {
        var session = GameController.Instance.GetSession();
        var room = session.MemoryMap.GetRoom(session.CurrentStage);
        var rewardRoom = DataManager.Base.DungeonRoomReward.Get(room).ListRewardRandomInRoom.RandomWeighted(out int index);
        var merge = new List<LootParams>();
        foreach (var all in rewardRoom.RewardsRaw)
        {
            merge.Add(all[0]);
        }
        merge = merge.Refactor();
        foreach (var reward in merge)
        {
            session.AddLoot(reward);
        }
    }

    private void OnBossChestSpawned()
    {
    }

}