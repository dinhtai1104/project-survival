using Assets.Game.Scripts.Utilities;

public class UIChestRewardGoldenPanel : UIChestRewardBasePanel
{
    protected override void SetupData()
    {
        otherSideData = new ResourceData { Value = GetAmountResource(rewards, EResource.MainWpFragment, EResource.BootFragment).Value, Resource = EResource.EquipmentRdFragment };

        var equipmentAll = DataManager.Base.Equipment.Dictionary.Values;
        foreach (var equipment in equipmentAll)
        {
            if (equipment.IdNum >= 0)
            {
                sprites.Add(ResourcesLoader.Instance.GetSprite(AtlasName.Equipment, $"{equipment.Id}"));
            }
        }

        foreach (var rw in rewards)
        {
            if (rw.Data is EquipmentData)
            {
                foreach (var equipment in equipmentAll)
                {
                    if (equipment.IdNum == (((EquipmentData)rw.Data).Id))
                    {
                        TargetIndex.Add(equipment.IdNum);
                    }
                }
            }
        }
        otherSideData = new ResourceData { Value = GetAmountResource(rewards, EResource.MainWpFragment, EResource.BootFragment).Value, Resource = EResource.EquipmentRdFragment };

        var target = GetTargets(rewards, chest >= EChest.Hero ? ELootType.HeroFragment : ELootType.Equipment);
        int index = 0;
        for (int i = 0; i < NumberOfLine; i++)
        {
            int vitri = i % 2 == 0 ? 3 : 2;
            if (chest == EChest.Golden10)
            {
                vitri = i % 2 == 0 ? 0 : 1;
            }
            m_Scrollers[i].SetPositionInLine(vitri);
            for (int j = 0; j < NumberItemOfOneLine; j++)
            {
                var scroll = m_Scrollers[_scrollStt[i]];
                scroll.SetData(chest, target[index]);
                scroll.SetListSprite(sprites);
                scroll.SetTargetIndex(TargetIndex[index]);
                scroll.SetTimeout(TimeChestFX);
                index++;
            }
        }
    }
}
