using Mosframe;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using System;

public class FXChestRewardItem : UIBehaviour, IScrollerDynamicScrollView
{
    int index = -1;
    [SerializeField] protected Image icon;
    [SerializeField] protected FXChestScroller scrollerData;
    [SerializeField] private ParticleSystem effect;
    [SerializeField] protected SetColorParticles colorParticles;

    [SerializeField] private float alpha = 1;
    [SerializeField] protected Color heroFragmentColor;
    [SerializeField] protected Color commonColor;
    [SerializeField] protected Color unCommonColor;
    [SerializeField] protected Color rareColor;
    [SerializeField] protected Color epicColor;
    [SerializeField] protected Color legendaryColor;
    [SerializeField] protected Color ultimateColor;

    public LootParams loot;
    public void SetColor(LootParams target)
    {
        loot = target;
        Color c = commonColor;
        if (target.Type == ELootType.HeroFragment)
        {
            c = heroFragmentColor;
        }
        else if (target.Type == ELootType.Resource)
        {
            c = heroFragmentColor;
        }
        else if (target.Type == ELootType.Fragment)
        {
            c = heroFragmentColor;
        }
        else if (target.Type == ELootType.HeroStone)
        {
            c = heroFragmentColor;
        }
        else
        {
            var eq = (target.Data as EquipmentData);
            if (eq != null)
            {
                switch (eq.Rarity)
                {
                    case ERarity.UnCommon:
                        c = unCommonColor;
                        break;
                    case ERarity.Rare:
                        c = rareColor;
                        break;
                    case ERarity.Epic:
                        c = epicColor;
                        break;
                    case ERarity.Legendary:
                        c = legendaryColor;
                        break;
                    case ERarity.Ultimate:
                        c = ultimateColor;
                        break;
                }
            }
            else
            {
                c = legendaryColor;
            }
            c.a = alpha;
        }

        colorParticles.SetColor(c);
    }

    public virtual void Active(bool active)
    {
        gameObject.SetActive(active);
        if (active == true)
        {
            effect.Play();
        }
    }

    public int getIndex()
    {
        return index;
    }

    public void onUpdateItem(int index)
    {
        this.index = index;
        if (scrollerData.Sprites.Count == 0) return;
        var indexSp = index % scrollerData.Sprites.Count;
        if (index > 0 && indexSp % scrollerData.Sprites.Count == 0)
        {
            scrollerData.CurrentCycle++;
        }
        icon.sprite = scrollerData.Sprites[indexSp];
    }

    public void SetIndex(int v)
    {
        var indexSp = v % scrollerData.Sprites.Count;
        icon.sprite = scrollerData.Sprites[indexSp];
    }
}
