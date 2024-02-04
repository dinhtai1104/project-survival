using Assets.Game.Scripts.Utilities;
using com.mec;
using MoreMountains.Tools;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIChestRewardGoldenBossPanel : UIChestRewardBasePanel
{
    protected override void SetupData()
    {
        var target = new List<LootParams>();

        int index = 0;
        foreach (var model in rewardsRaw)
        {
            if (model[0].Data is ResourceData)
            {
                var rs = model[0].Data as ResourceData;
                if (rs.Resource > EResource.Energy)
                {
                    target.Add(model[0].Clone());
                }
            }
        }
        var spriteEquipmentFrag = GetSpritesResourceFrom(EResource.MainWpFragment, EResource.BootFragment);
        var spritestoneFrag = GetSpritesResourceFrom(EResource.BaseStone, EResource.LightStone);

        var sprites = new List<List<Sprite>>();
        sprites.Add(spriteEquipmentFrag);
        sprites.Add(spriteEquipmentFrag);
        sprites.Add(spritestoneFrag);

        target = target.OrderByDescending(t => (t.Data as ResourceData).Resource).ToList();
        //target.MMSwap(0, 1);
         otherSideData = new ResourceData { Value = GetAmountResource(rewards, EResource.MainWpFragment, EResource.BootFragment).Value, Resource = EResource.EquipmentRdFragment };


        index = 0;

        foreach (var model in rewardsRaw)
        {
            if (model[0].Data is ResourceData)
            {
                var rs = model[0].Data as ResourceData;
                if (rs.Resource > EResource.Energy)
                {
                    TargetIndex.Add(index++);
                }
            }
        }
        index = 0;
        for (int i = 0; i < NumberOfLine; i++)
        {
            int vitri = i % 2 == 0 ? 3 : 2;
           
            m_Scrollers[i].SetPositionInLine(vitri);
            for (int j = 0; j < NumberItemOfOneLine; j++)
            {
                var scroll = m_Scrollers[_scrollStt[i]];
                scroll.SetData(chest, target[index]);
                scroll.SetListSprite(sprites[index]);
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
        for (var i = from; i <= to; i++)
        {
            sprites.Add(ResourcesLoader.Instance.GetSprite(AtlasName.Resources, i.ToString()));
        }
        return sprites;
    }
    protected override IEnumerator<float> _WaitForSkipButton()
    {
        yield return Timing.WaitForSeconds(3);
        if (PlayerPrefs.GetInt("BossChestGold", 0) >= 1)
        {
            buttonSkip.SetActive(true);
        }
        PlayerPrefs.SetInt("BossChestGold", PlayerPrefs.GetInt("BossChestGold", 0) + 1);
    }
}
