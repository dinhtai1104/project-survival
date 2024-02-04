using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HeroEntity
{
    public int Id;
    public EHero TypeHero;
    public float BaseHp;
    public float BaseDmg;
    public float BaseSpeed;
    public float BaseDmgCrit;
    public float BaseRateCrit;
    public float BaseSpeedMove; 
    public float BaseJumpForce; 
    public float BaseJumpCount; 
    public float BaseClimbDrag;
    public int NumberFragmentUnlock;
    // Grown
    public float HpGrown;
    public float DmgGrown;

    public Stat HpGrownStat;
    public Stat DmgGrownStat;

    /// <summary>
    /// Use for upgrade level
    /// </summary>
    public EResource StoneResource;

    /// <summary>
    /// Use for upgrade star
    /// </summary>
    public EResource HeroResource;


    public List<HeroLevelUpgradeEntity> LevelUpgrades = new List<HeroLevelUpgradeEntity>();
    public List<HeroStarUpgradeEntity> StarUpgrades = new List<HeroStarUpgradeEntity>();

    public HeroEntity() { }
    public HeroEntity(DB_Hero e)
    {
        Id = e.Get<int>("Id");
        TypeHero = (EHero)Id;
        BaseHp = e.Get<float>("BaseHp");
        BaseDmg = e.Get<float>("BaseDmg");
        BaseSpeed = e.Get<float>("BaseSpeed");
        BaseDmgCrit = e.Get<float>("BaseDmgCrit");
        BaseRateCrit = e.Get<float>("BaseRateCrit");
        BaseSpeedMove = e.Get<float>("BaseSpeedMove");
        BaseJumpForce = e.Get<float>("BaseJumpForce");
        BaseJumpCount = e.Get<int>("BaseJumpCount");
        NumberFragmentUnlock = e.Get<int>("FragmentUnlock");

        HpGrown = e.Get<float>("HpGrown");
        DmgGrown = e.Get<float>("DmgGrown");

        HpGrownStat = new Stat(HpGrown, 0);
        DmgGrownStat = new Stat(HpGrown, 0);

        Enum.TryParse(e.Get<string>("StoneResource"), out StoneResource);
        Enum.TryParse(e.Get<string>("HeroResource"), out HeroResource);
    }
    public void SetBaseStat(IStatGroup stats, object sourceAdd)
    {
        if (stats == null)
        {
            Debug.Log("Stat Group not exist");
            return;
        }
        stats.SetBaseValue(StatKey.Hp, BaseHp);
        stats.SetBaseValue(StatKey.Dmg, BaseDmg);
        stats.SetBaseValue(StatKey.CritDmg, BaseDmgCrit);
        stats.SetBaseValue(StatKey.CritRate, BaseRateCrit);
        stats.SetBaseValue(StatKey.SpeedMove, BaseSpeed);
        stats.SetBaseValue(StatKey.JumpForce, BaseJumpForce);
        stats.SetBaseValue(StatKey.JumpCount, BaseJumpCount);

    }

    public void SetLevelUpgrades(List<HeroLevelUpgradeEntity> upgrades)
    {
        LevelUpgrades = upgrades;
    }
    public void SetStarUpgrades(List<HeroStarUpgradeEntity> upgrades)
    {
        StarUpgrades = upgrades;
    }
}