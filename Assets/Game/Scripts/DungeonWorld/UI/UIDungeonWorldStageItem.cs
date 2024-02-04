using Assets.Game.Scripts.DungeonWorld.Data;
using Assets.Game.Scripts.DungeonWorld.Save;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UIDungeonWorldStageItem : MonoBehaviour
{
    [SerializeField] private List<UIInventorySlot> rewardSlotProvide;
    [SerializeField] private TextMeshProUGUI nameTxt;
    [SerializeField] private GameObject lockPanel;
    [SerializeField] private GameObject claimableButton;
    [SerializeField] private GameObject claimedButton;
    [SerializeField] private GameObject lockButton;

    private DungeonWorldStageEntity entity;
    private DungeonWorldStageSave save;
    private CancellationTokenSource tokenSource;
    public void Set(DungeonWorldStageEntity entity)
    {
        for (int i = 0; i < rewardSlotProvide.Count; i++)
        {
            rewardSlotProvide[i].Clear();
        }

        tokenSource = new CancellationTokenSource();
        this.entity = entity;
        this.save = DataManager.Save.DungeonWorld.Get(entity.Dungeon, entity.Stage);
        Setup();
    }
    private void OnEnable()
    {
        claimableButton.GetComponent<Button>().onClick.AddListener(ClaimButton);
    }

    private void ClaimButton()
    {
        DataManager.Save.DungeonWorld.ClaimReward(save.Dungeon, save.Stage);
        var entity = DataManager.Base.DungeonWorld.Get(save.Dungeon);
        PanelManager.ShowRewards(entity.Get(save.Stage).Reward).Forget();
        Setup();
        Messenger.Broadcast(EventKey.ClaimDungeonWorld);

        // Track
        foreach (var rw in entity.Get(save.Stage).Reward) 
        {
            string item = "Exp";
            double remaining = 0;
            if (rw.Data is ResourceData)
            {
                item = (rw.Data as ResourceData).Resource.ToString();
                remaining = DataManager.Save.Resources.GetResource((rw.Data as ResourceData).Resource);
            }

            FirebaseAnalysticController.Tracker.NewEvent("earn_resource")
                .AddStringParam("item_category", rw.Type.ToString())
                .AddStringParam("item_id", (rw.Data as ResourceData).Resource.ToString())
                .AddStringParam("source", "world")
                .AddStringParam("source_id", $"{entity.Dungeon + 1}_{save.Stage + 1}")
                .AddDoubleParam("value", rw.Data.ValueLoot)
                .AddDoubleParam("remaining_value", remaining)
                .AddDoubleParam("total_earned_value", FirebaseAnalysticController.Instance.GetTrackingResourceEarn((rw.Data as ResourceData).Resource))
                .Track();
        }
    }


    private void OnDisable()
    {
        claimableButton.GetComponent<Button>().onClick.RemoveListener(ClaimButton);
        tokenSource.Cancel();
        tokenSource.Dispose();
    }
    private void Setup()
    {
        nameTxt.text = I2Localize.GetLocalize("Common/Title_World_ClearRoom", entity.Stage);
        var reward = entity.Reward;
        var uniTask = new List<UniTask>();
        for (int i = 0; i < rewardSlotProvide.Count; i++)
        {
            if (i < reward.Count)
            {
                var rewardSlotProvideItem = rewardSlotProvide[i];
                rewardSlotProvide[i].gameObject.SetActive(true);
                var path = AddressableName.UILootItemPath;
                path = string.Format(path, reward[i].Type);
                var dataReward = reward[i].Data;
                var task = UIHelper.GetUILootIcon(path, dataReward, rewardSlotProvideItem.transform, 1f).ContinueWith(t =>
                {
                    rewardSlotProvideItem.SetItem(t);
                }).AttachExternalCancellation(tokenSource.Token);
                uniTask.Add(task);
            }
            else
            {
                rewardSlotProvide[i].gameObject.SetActive(false);
            }
        }
        claimedButton.SetActive(false);
        claimableButton.SetActive(false);
        lockButton.SetActive(false);

        bool canReward = false;
        var dungeonSave = DataManager.Save.Dungeon;
        if (dungeonSave.IsDungeonCleared(save.Dungeon))
        {
            if (!save.IsClaimed)
            {
                canReward = true;
            }
        }
        else
        {
            if (dungeonSave.CurrentDungeon >= save.Dungeon)
            {
                if (dungeonSave.BestStage >= save.Stage)
                {
                    if (!save.IsClaimed)
                    {
                        canReward = true;
                    }
                }
            }
        }
        if (canReward)
        {
            if (save.IsClaimed)
            {
                claimedButton.SetActive(true);
            }
            else
            {
                claimableButton.SetActive(true);
            }
            return;
        }

        if (save.IsClaimed)
        {
            claimedButton.SetActive(true);
        }
        else
        {
            lockButton.SetActive(true);
        }
    }
}