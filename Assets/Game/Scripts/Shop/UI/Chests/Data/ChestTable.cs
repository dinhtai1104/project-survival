using BansheeGz.BGDatabase;
using System;
using System.Collections.Generic;

[System.Serializable]
public class ChestTable : DataTable<EChest, ChestEntity>
{
    public override void GetDatabase()
    {
        EChest lastChest = EChest.Silver;
        int inde = 0;
        DB_Chest.ForEachEntity(e =>
        {
            if (!Enum.TryParse(e.Get<string>("Chest"), out EChest ChestType))
            {
                ChestType = lastChest;
            }
            if (inde == 0)
            {
                lastChest = ChestType;
            }
            if (!Dictionary.ContainsKey(ChestType))
            {
                Dictionary.Add(ChestType, new ChestEntity());
                // Create data
                // Detect Chest Type
                var chestData = Dictionary[ChestType];
                chestData.Type = ChestType;
                
                // Cost Gem
                chestData.CurrencyCost = new ResourceData { Resource = EResource.Gem, Value = e.Get<int>("Gem") };

                // Key
                Enum.TryParse(e.Get<string>("Key"), out EResource keyType);
                chestData.KeyCost = new ResourceData { Resource = keyType, Value = 1 };

                // Time Ads if is possible
                chestData.TimeRewardAd = e.Get<int>("RewardAdTime");
                chestData.MaxRewardDay = e.Get<int>("RewardAdMaxDay");
            }
            // Find data in row and push it into ChestData
            var chestRow = new ChestEquipmentRow(e);
            Dictionary[ChestType].AddRow(chestRow);
        });
    }
}
