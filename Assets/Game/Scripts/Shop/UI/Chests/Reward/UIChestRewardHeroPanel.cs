using Assets.Game.Scripts.Utilities;
using MoreMountains.Tools;

public class UIChestRewardHeroPanel : UIChestRewardBasePanel
{
    protected override void SetupData()
    {
        for (var i = EResource.NormalHero; i <= EResource.EvilHero; i++)
        {
            sprites.Add(ResourcesLoader.Instance.GetSprite(AtlasName.Resources, $"{i}"));
        }

        var indexHero = rewards.FindIndex(t => t.Type == ELootType.Hero);
        if (indexHero >= 0)
        {
            var hero = rewards.Find(t => t.Type == ELootType.Hero);
            var heroData = hero.Data as HeroData;

            sprites.Add(ResourcesLoader.Instance.GetSprite(AtlasName.Hero, heroData.HeroType.ToString()));
            rewards.MMSwap(indexHero, rewards.Count / 2);
        }

        foreach (var rw in rewards)
        {
            if (rw.Data is ResourceData)
            {
                var rs = rw.Data as ResourceData;
                int id = 0;
                for (var i = EResource.NormalHero; i <= EResource.EvilHero; i++)
                {
                    if (rs.Resource == i)
                    {
                        TargetIndex.Add(id);
                        break;
                    }
                    id++;
                }
            }
            if (rw.Data is HeroData)
            {
                var heroData = rw.Data as HeroData;
                TargetIndex.Add(sprites.Count - 1);
            }
        }

        otherSideData = new ResourceData { Value = GetAmountResource(rewards, EResource.BaseStone, EResource.LightStone).Value, Resource = EResource.HeroStoneRdFragment };

        var target = GetTargets(rewards, ELootType.HeroFragment, ELootType.Hero);
        int index = 0;
        for (int i = 0; i < NumberOfLine; i++)
        {
            int vitri = i % 2 == 0 ? 3 : 2;
            if (chest == EChest.Hero10)
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
