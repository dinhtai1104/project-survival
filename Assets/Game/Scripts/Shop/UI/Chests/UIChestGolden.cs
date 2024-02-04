using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UIChestGolden : UIChestBase
{
    [SerializeField] private TextMeshProUGUI m_GetEpicAfterTxt;
    protected override void Setup()
    {
        base.Setup();
        m_GetEpicAfterTxt.text = I2Localize.GetLocalize("Common/Get_Epic_After", (10 - save.OpenCount % 10));
    }
    protected async override void Open()
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
                    equipmentdata.data = new EquipmentData("bone_armor", ERarity.UnCommon, 0);
                }
            }

            if ((save.OpenCount + 1) % 10 == 0)
            {
                foreach (var data in rw)
                {
                    if (data.Type == ELootType.Equipment)
                    {
                        var eq = data.Data as EquipmentData;
                        eq.Rarity = ERarity.Epic;
                    }
                }
            }
            save.Open();

            foreach (var data in rw)
            {
                rewards.Add(data);
            }
        }
        rewards = rewards.Merge();
        var path = chestType == EChest.Golden ? AddressableName.UIChestRewardGolden_1Panel : AddressableName.UIChestRewardGolden_10Panel;

        PanelManager.CreateAsync<UIChestRewardBasePanel>(path).ContinueWith(async (rewPanel) =>
        {
            rewPanel.Show();
            rewPanel.SetReward(base.chestType, rewards);
            rewPanel.onClosed += () =>
            {
                PanelManager.ShowRewards(rewards).Forget();
                CloudSave.Controller.Instance.ValidateAndSave().Forget();
            };
            await rewPanel.Stop();
        }).Forget();
    }
}