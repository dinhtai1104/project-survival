using com.mec;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UIChestSilver : UIChestBase
{
    protected override async void Open()
    {
        base.Open();
        var rewards = new List<LootParams>();
        for (int i = 0; i < NumberOpen; i++)
        {
            var rw = GetReward();
            
            if (save.OpenCount == 0)
            {
                // Add force equipment
                var equipmentdata = rw.Find(t => t.Type == ELootType.Equipment);
                if (equipmentdata != null)
                {
                    equipmentdata.data = new EquipmentData("Kunai", ERarity.UnCommon, 0);
                }
            }

            save.Open();
            foreach (var data in rw)
            {
                rewards.Add(data);
            }
        }
        rewards = rewards.Merge();

        PanelManager.CreateAsync<UIChestRewardBasePanel>(AddressableName.UIChestRewardSilverPanel).ContinueWith(async rewPanel =>
        {
            rewPanel.onClosed += () =>
            {
                PanelManager.ShowRewards(rewards);
                CloudSave.Controller.Instance.ValidateAndSave().Forget();
            };
            rewPanel.Show();
            rewPanel.SetReward(base.chestType, rewards);
            await rewPanel.Stop();
        }).Forget();
    }
}