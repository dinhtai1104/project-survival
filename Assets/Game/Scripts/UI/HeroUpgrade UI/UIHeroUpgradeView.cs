using System;
using UnityEngine;

public class UIHeroUpgradeView : MonoBehaviour
{
    [SerializeField] private UIHeroUpgradeBase LevelUpgrade;
    [SerializeField] private UIHeroUpgradeBase StarUpgrade;

    public delegate void OnHeroClicked(EHero hero);
    public OnHeroClicked onHeroClicked;

  
    public void SetHero(UIHeroUpgradePanel panel, EHero hero)
    {
        LevelUpgrade.Init(panel, hero);
        StarUpgrade.Init(panel, hero);
    }
    private void OnDisable()
    {
        LevelUpgrade.Clear();
        StarUpgrade.Clear();
    }
}