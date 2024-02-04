using Assets.Game.Scripts.Utilities;
using UnityEngine;

public class FXChestRewardItemHasBg : FXChestRewardItem
{
    public GameObject bgScroll;

    public override void Active(bool active)
    {
        base.Active(active);
        bgScroll.SetActive(false);
        loot = scrollerData.GetTarget(0);
        var rs = loot.Data as ResourceData;
        if (rs != null)
        {
            icon.sprite = ResourcesLoader.Instance.GetSprite(AtlasName.Resources, rs.Resource.ToString());
        }
        var type = loot.Data.GetAllData()[0].Type;
        if (loot != null)
        {
            switch (type)
            {
                case ELootType.Resource:
                    
                    break;
                case ELootType.BuffCard:
                    break;
                case ELootType.Fragment:
                    bgScroll.SetActive(true);
                    break;
                case ELootType.HeroFragment:
                    break;
                case ELootType.HeroStone:
                   
                    break;
                case ELootType.Hero:
                    break;
                case ELootType.Exp:
                    break;
            }
        }
        if (rs.Resource >= EResource.MainWpFragment && rs.Resource <= EResource.BootFragment)
        {
            bgScroll.SetActive(true);
        }
    }
}