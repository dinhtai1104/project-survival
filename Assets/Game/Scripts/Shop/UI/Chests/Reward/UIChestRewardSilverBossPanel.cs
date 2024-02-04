using Assets.Game.Scripts.Utilities;
using com.mec;
using System.Collections.Generic;
using UnityEngine;

public class UIChestRewardSilverBossPanel : UIChestRewardBasePanel
{
    protected override void SetupData()
    {   
        var target = new List<LootParams>();
        int index = 0;
        foreach (var model in rewards)
        {
            if (model.Data is ResourceData)
            {
                var rs = model.Data as ResourceData;
                if (rs.Resource > EResource.Energy)
                {
                    target.Add(model.Clone());
                    TargetIndex.Add(index++);
                }
            }
        }

        var reward = target[0].Data as ResourceData;
        if (reward.Resource >= EResource.MainWpFragment && reward.Resource <= EResource.BootFragment)
        {
            LootEquipmentFragment();
            otherSideData = new ResourceData { Value = GetAmountResource(rewards, EResource.MainWpFragment, EResource.BootFragment).Value, Resource = EResource.EquipmentRdFragment };
        }
        else
        {
            LootHeroStoneFragment();
            otherSideData = new ResourceData { Value = GetAmountResource(rewards, EResource.BaseStone, EResource.LightStone).Value, Resource = EResource.HeroStoneRdFragment };
        }

        index = 0;
        for (int i = 0; i < NumberOfLine; i++)
        {
            m_Scrollers[i].SetPositionInLine((i % 2 == 0 ? 3 : 2));
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

    protected virtual void LootEquipmentFragment()
    {
        sprites = GetSpritesResourceFrom(EResource.MainWpFragment, EResource.BootFragment);
    }
    protected virtual void LootHeroStoneFragment()
    {
        sprites = GetSpritesResourceFrom(EResource.BaseStone, EResource.LightStone);
    }

    protected virtual List<Sprite> GetSpritesResourceFrom(EResource from, EResource to)
    {
        var sprites = new List<Sprite>();
        for (var i = from; i<= to; i++)
        {
            sprites.Add(ResourcesLoader.Instance.GetSprite(AtlasName.Resources, i.ToString()));
        }
        return sprites;
    }
    protected override IEnumerator<float> _WaitForSkipButton()
    {
        yield return Timing.WaitForSeconds(3);
        if (PlayerPrefs.GetInt("BossChestSilver", 0) >= 1)
        {
            buttonSkip.SetActive(true);
        }
        PlayerPrefs.SetInt("BossChestSilver", PlayerPrefs.GetInt("BossChestSilver", 0) + 1);
    }
}
