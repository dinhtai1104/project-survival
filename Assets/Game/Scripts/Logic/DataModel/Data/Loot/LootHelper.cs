using Assets.Game.Scripts.BaseFramework.Architecture;
using System;
using System.Collections.Generic;

public static class LootHelper
{
    public static List<LootParams> Refactor(this List<LootParams> lootParams, int multiply = 1)
    {
        var iDatas = new List<LootParams>();
        for (int i = 0; i < multiply; i++)
        {
            foreach (var loot in lootParams)
            {
                var list = loot.Data.GetAllData();
                var list2 = new List<LootParams>();
                foreach (var data in list)
                {
                    list2.Add(new LootParams { Type = data.Type, Data = data.Data });
                }
                iDatas.AddRange(list2);
            }
        }
        iDatas = LootHelper.Merge(iDatas);
        iDatas = ApplyServicePremiumBattlePass(iDatas);
        return iDatas;
    }

    private static List<LootParams> ApplyServicePremiumBattlePass(List<LootParams> iDatas)
    {
        var battlePass = Architecture.Get<BattlePassService>();

        foreach (var d in iDatas)
        {
            if (d.Type == ELootType.Resource)
            {
                var rs = d.Data as ResourceData;
                if (rs != null)
                {
                    if (rs.Resource >= EResource.MainWpFragment && rs.Resource <= EResource.BootFragment)
                    {
                        rs.Multiply(1 + battlePass.GetExtraFragmentDrop());
                    }
                    if (rs.Resource >= EResource.BaseStone && rs.Resource <= EResource.LightStone)
                    {
                        rs.Multiply(1 + battlePass.GetExtraStoneDrop());
                    }
                }
            }
        }

        return iDatas;
    }

    public static List<LootParams> Merge(this List<LootParams> lootList)
    {
        var rs = new List<LootParams>();

        for (int i = 0; i < lootList.Count; i++)
        {
            var lootA = lootList[i].Clone();
            var dataA = lootA.Data;
            var lootTypeA = lootA.Type;

            bool canMerge = false;

            for (int j = 0; j < rs.Count; j++)
            {
                var lootB = rs[j];
                var dataB = lootB.Data;
                var lootTypeB = lootB.Type;
                if (lootTypeA == lootTypeB)
                {
                    if (dataB.CanMergeData)
                    {
                        if (dataB.Add(dataA))
                        {
                            canMerge = true;
                            break;
                        }
                    }
                }
            }
            if (!canMerge)
            {
                rs.Add(lootA);
            }
        }

        return rs;
    }
    public static List<LootParams> MergeWithEquipment(this List<LootParams> lootList)
    {
        var rs = new List<LootParams>();

        for (int i = 0; i < lootList.Count; i++)
        {
            var lootA = lootList[i].Clone();
            var dataA = lootA.Data;
            var lootTypeA = lootA.Type;

            bool canMerge = false;

            for (int j = 0; j < rs.Count; j++)
            {
                var lootB = rs[j];
                var dataB = lootB.Data;
                var lootTypeB = lootB.Type;
                if (lootTypeA == lootTypeB)
                {
                    if (dataB.CanMergeData || dataB is EquipmentData)
                    {
                        if (dataB.Add(dataA))
                        {
                            canMerge = true;
                            break;
                        }
                    }
                }
            }
            if (!canMerge)
            {
                rs.Add(lootA);
            }
        }

        return rs;
    }
}