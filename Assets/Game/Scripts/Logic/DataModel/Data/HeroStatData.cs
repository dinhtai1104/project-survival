using System;
using System.Collections.Generic;

[System.Serializable]
public class HeroStatData
{
    public string source => EStatSource.Hero;
    public string sourceLevel => EStatSource.HeroLevelModifier;
    public string sourceAllHero => EStatSource.HeroModifierAll;

    private PlayerData playerData;

    public EHero heroType;
    public IStatGroup heroStat;

    private HeroEntity heroEntity;
    private HeroSave heroSave;

    private StatModifier HpStatBase;
    private StatModifier DmgStatBase;

    private StatModifier HpStatGrownBase;
    private StatModifier DmgStatGrownBase;

    private StatModifier HpStatBaseFinal;
    private StatModifier DmgStatBaseFinal;

    private StatModifier HpStatGrownFinal;
    private StatModifier DmgStatGrownFinal;

    private StatModifier HpModifierStatFinal;
    private StatModifier DmgModifierStatFinal;

    [UnityEngine.SerializeField]
    public List<AttributeStatModifier> _modifierForAllHeroes = new List<AttributeStatModifier>();

    /// <summary>
    /// First apply base stat like : Base + Base Grown
    /// Then add all special stat (stat for all hero) to PlayerData to store them
    /// </summary>
    public void _Apply()
    {
        int Level = heroSave.Level;
        int Star = heroSave.Star;


        HpStatGrownFinal.Value = HpStatGrownBase.Value;
        DmgStatGrownFinal.Value = DmgStatGrownBase.Value;

        HpStatBaseFinal.Value = HpStatBase.Value;
        DmgStatBaseFinal.Value = DmgStatBase.Value;
        if (Star > 0)
        {
            var starUpgradeE = heroEntity.StarUpgrades[Star - 1];

            // Change base stat depend star
            HpStatBaseFinal.Value += starUpgradeE.HpBaseAdd;
            DmgStatBaseFinal.Value += starUpgradeE.DmgBaseAdd;

            // Change grown stat depend star
            HpStatGrownFinal.Value += starUpgradeE.HpGrownAdd;
            DmgStatGrownFinal.Value += starUpgradeE.DmgGrownAdd;

            // change by star
            heroStat.SetBaseValue(StatKey.Hp, HpStatBaseFinal.Value);
            heroStat.SetBaseValue(StatKey.Dmg, DmgStatBaseFinal.Value);
        }
        HpModifierStatFinal.Value = Level * HpStatGrownFinal.Value;
        DmgModifierStatFinal.Value = Level * DmgStatGrownFinal.Value;

        // Remove all modifier before apply new stat
        heroStat.RemoveModifiersFromSource(sourceLevel);
        heroStat.RemoveModifiersFromSource(sourceAllHero);

        playerData.RemoveStatModifierAllHero(_modifierForAllHeroes);
        _modifierForAllHeroes.Clear();
        // mile stone level if possible
        for (int i = 0; i < Level; i++)
        {
            if (i >= heroEntity.LevelUpgrades.Count) break;
            var levelUpgradeE = heroEntity.LevelUpgrades[i];
            if (!levelUpgradeE.Milestone) continue;

            if (levelUpgradeE.IsUseForAllHero)
            {
                // store speacial stat (apply for all hero) to playerData and release them when apply
                _modifierForAllHeroes.Add(levelUpgradeE.Reward);
            }
            else
            {
                heroStat.AddModifier(levelUpgradeE.Reward.StatKey, levelUpgradeE.Reward.Modifier, sourceLevel);
            }
        }
        heroStat.CalculateStats();
    }

    public HeroStatData(HeroSave heroSave, PlayerData playerData)
    {
        this.playerData = playerData;
        this.heroSave = heroSave;
        this.heroType = this.heroSave.Type;
        heroStat = PlayerStat.Default();

        heroEntity = DataManager.Base.Hero.GetHero(heroType);
        heroEntity.SetBaseStat(heroStat, source);

        // Base
        HpStatBase = new StatModifier(EStatMod.Flat, heroEntity.BaseHp);
        DmgStatBase = new StatModifier(EStatMod.Flat, heroEntity.BaseDmg);

        // GrownBase
        HpStatGrownBase = new StatModifier(EStatMod.Flat, heroEntity.HpGrown);
        DmgStatGrownBase = new StatModifier(EStatMod.Flat, heroEntity.DmgGrown);

        // BaseFinal
        HpStatBaseFinal = new StatModifier(EStatMod.Flat, 0);
        DmgStatBaseFinal = new StatModifier(EStatMod.Flat, 0);

        // BaseGrown Final
        HpStatGrownFinal = new StatModifier(EStatMod.Flat, 0);
        DmgStatGrownFinal = new StatModifier(EStatMod.Flat, 0);

        // Final Modifier
        DmgModifierStatFinal = new StatModifier(EStatMod.Flat, 0);
        HpModifierStatFinal = new StatModifier(EStatMod.Flat, 0);

        heroStat.AddModifier(StatKey.Dmg, DmgModifierStatFinal, source);
        heroStat.AddModifier(StatKey.Hp, HpModifierStatFinal, source);

        _Apply();
    }

    public void ApplyLevelAndStar()
    {
        int Level = heroSave.Level;
        int Star = heroSave.Star;

        var allStarUpgradeData = heroEntity.StarUpgrades;
        var allLevelUpgradeData = heroEntity.LevelUpgrades;

        for (int i = 0; i <= Star; i++)
        {
            // Change base stat and grown stat
        }
        // Add modifier stat grown

      
    }

    public void _ApplyStatModifierAllHero(List<AttributeStatModifier> modifierApplyToAllHero, object source = null)
    {
        heroStat.RemoveModifiersFromSource(source == null ? sourceAllHero : source);
        foreach (var attr in modifierApplyToAllHero)
        {
            heroStat.AddModifier(attr.StatKey, attr.Modifier, source == null ? sourceAllHero : source);
        }
    }
    public void _ApplyStatModifierAllHero(AttributeStatModifier modifierApplyToAllHero, object source = null)
    {
        heroStat.AddModifier(modifierApplyToAllHero.StatKey, modifierApplyToAllHero.Modifier, source == null ? sourceAllHero : source);
    }
}