using Assets.Game.Scripts._Services;
using Assets.Game.Scripts.BaseFramework.Architecture;
using Assets.Game.Scripts.Talent;
using System.Collections.Generic;
using System.Diagnostics;

public class HeroFactory
{
    private static HeroFactory instance;
    public static HeroFactory Instance
    {
        get
        {
            if (instance == null) instance = new HeroFactory();
            return instance;
        }
    }


    public IStatGroup GetHeroStatGroup(IStatGroup origin, EHero hero, int Level, int Star)
    {
        var playerData = GameSceneManager.Instance.PlayerData;
        IStatGroup heroStat = new StatGroup();

        StatModifier HpStatBase;
        StatModifier DmgStatBase;

        StatModifier HpStatGrownBase;
        StatModifier DmgStatGrownBase;

        StatModifier HpStatBaseFinal;
        StatModifier DmgStatBaseFinal;

        StatModifier HpStatGrownFinal;
        StatModifier DmgStatGrownFinal;

        StatModifier HpModifierStatFinal;
        StatModifier DmgModifierStatFinal;

        heroStat = origin;

        var heroEntity = DataManager.Base.Hero.GetHero(hero);
        heroEntity.SetBaseStat(heroStat, EStatSource.Hero);

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

        heroStat.AddModifier(StatKey.Dmg, DmgModifierStatFinal, EStatSource.Hero);
        heroStat.AddModifier(StatKey.Hp, HpModifierStatFinal, EStatSource.Hero);

        HpStatGrownFinal.Value = HpStatGrownBase.Value;
        DmgStatGrownFinal.Value = DmgStatGrownBase.Value;

        HpStatBaseFinal.Value = HpStatBase.Value;
        DmgStatBaseFinal.Value = DmgStatBase.Value;

        // apply star
        var dbStar = DataManager.Base.HeroStarUpgrade.Dictionary[hero];

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

        // apply level
        HpModifierStatFinal.Value = Level * HpStatGrownFinal.Value;
        DmgModifierStatFinal.Value = Level * DmgStatGrownFinal.Value;

        var allHeroSave = DataManager.Save.User.HeroSaves;
        var statApplyForAllHero = new List<AttributeStatModifier>();
        foreach (var heroSave in allHeroSave)
        {
            if (!heroSave.Value.IsUnlocked) continue;
            var entityHeroSave = DataManager.Base.Hero.Get(heroSave.Key);
            // apply passive level
            for (int i = 0; i < heroSave.Value.Level; i++)
            {
                if (i >= entityHeroSave.LevelUpgrades.Count) break;
                var levelUpgradeE = entityHeroSave.LevelUpgrades[i];
                if (!levelUpgradeE.Milestone) continue;

                if (levelUpgradeE.IsUseForAllHero)
                {
                    // store speacial stat (apply for all hero) to playerData and release them when apply
                    //heroStat.AddModifier(levelUpgradeE.Reward.StatKey, levelUpgradeE.Reward.Modifier, EStatSource.HeroModifierAll);
                    statApplyForAllHero.Add(levelUpgradeE.Reward);
                }
            }
        }

        // Apply Stat Level Change Hero
        for (int i = 0; i < Level; i++)
        {
            if (i >= heroEntity.LevelUpgrades.Count) break;
            var levelUpgradeE = heroEntity.LevelUpgrades[i];
            if (!levelUpgradeE.Milestone) continue;

            if (levelUpgradeE.IsUseForAllHero)
            {
            }
            else
            {
                heroStat.AddModifier(levelUpgradeE.Reward.StatKey, levelUpgradeE.Reward.Modifier, EStatSource.HeroLevelModifier);
            }
        }

        // Apply Stat All Hero
        for (int i = 0; i < statApplyForAllHero.Count; i++)
        {
            var stat = statApplyForAllHero[i];
            heroStat.AddModifier(stat.StatKey, stat.Modifier, EStatSource.HeroLevelModifier);
        }

        heroStat.RemoveModifiersFromSource(EStatSource.SkillTree);
        heroStat.RemoveModifiersFromSource(EStatSource.Talent);
        // apply skill tree
        if (Architecture.ContainsService<SkillTreeService>())
        {
            Architecture.Get<SkillTreeService>().ApplySkillTree(heroStat);
        }

        if (Architecture.ContainsService<TalentService>())
        {
            Architecture.Get<TalentService>().ApplyTalent(heroStat);
        }
        return heroStat;
    }

    public IStatGroup GetHeroStat(EHero hero, int Level, int Star)
    {
        var playerData = GameSceneManager.Instance.PlayerData;
        var heroStat = new StatGroup();

        StatModifier HpStatBase;
        StatModifier DmgStatBase;

        StatModifier HpStatGrownBase;
        StatModifier DmgStatGrownBase;

        StatModifier HpStatBaseFinal;
        StatModifier DmgStatBaseFinal;

        StatModifier HpStatGrownFinal;
        StatModifier DmgStatGrownFinal;

        StatModifier HpModifierStatFinal;
        StatModifier DmgModifierStatFinal;

        heroStat = PlayerStat.Default();

        var heroEntity = DataManager.Base.Hero.GetHero(hero);
        heroEntity.SetBaseStat(heroStat, EStatSource.Hero);

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

        heroStat.AddModifier(StatKey.Dmg, DmgModifierStatFinal, EStatSource.Hero);
        heroStat.AddModifier(StatKey.Hp, HpModifierStatFinal, EStatSource.Hero);

        HpStatGrownFinal.Value = HpStatGrownBase.Value;
        DmgStatGrownFinal.Value = DmgStatGrownBase.Value;

        HpStatBaseFinal.Value = HpStatBase.Value;
        DmgStatBaseFinal.Value = DmgStatBase.Value;

        // apply star
        var dbStar = DataManager.Base.HeroStarUpgrade.Dictionary[hero];

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

        // apply level
        HpModifierStatFinal.Value = Level * HpStatGrownFinal.Value;
        DmgModifierStatFinal.Value = Level * DmgStatGrownFinal.Value;

        var allHeroSave = DataManager.Save.User.HeroSaves;
        var statApplyForAllHero = new List<AttributeStatModifier>();
        foreach (var heroSave in allHeroSave)
        {
            if (!heroSave.Value.IsUnlocked) continue;
            var entityHeroSave = DataManager.Base.Hero.Get(heroSave.Key);
            // apply passive level
            for (int i = 0; i < heroSave.Value.Level; i++)
            {
                if (i >= entityHeroSave.LevelUpgrades.Count) break;
                var levelUpgradeE = entityHeroSave.LevelUpgrades[i];
                if (!levelUpgradeE.Milestone) continue;

                if (levelUpgradeE.IsUseForAllHero)
                {
                    // store speacial stat (apply for all hero) to playerData and release them when apply
                    //heroStat.AddModifier(levelUpgradeE.Reward.StatKey, levelUpgradeE.Reward.Modifier, EStatSource.HeroModifierAll);
                    statApplyForAllHero.Add(levelUpgradeE.Reward);
                }
            }
        }

        // Apply Stat Level Change Hero
        for (int i = 0; i < Level; i++)
        {
            if (i >= heroEntity.LevelUpgrades.Count) break;
            var levelUpgradeE = heroEntity.LevelUpgrades[i];
            if (!levelUpgradeE.Milestone) continue;

            if (levelUpgradeE.IsUseForAllHero)
            {
            }
            else
            {
                heroStat.AddModifier(levelUpgradeE.Reward.StatKey, levelUpgradeE.Reward.Modifier, EStatSource.HeroLevelModifier);
            }
        }

        // Apply Stat All Hero
        for (int i = 0; i < statApplyForAllHero.Count; i++)
        {
            var stat = statApplyForAllHero[i];
            heroStat.AddModifier(stat.StatKey, stat.Modifier, EStatSource.HeroLevelModifier);
        }


        heroStat.RemoveModifiersFromSource(EStatSource.SkillTree);
        heroStat.RemoveModifiersFromSource(EStatSource.Talent);
        // apply skill tree
        Architecture.Get<SkillTreeService>().ApplySkillTree(heroStat);
        Architecture.Get<TalentService>().ApplyTalent(heroStat);

        return heroStat;
    }

    public IStatGroup ApplyEquipment(IStatGroup stat, EquipmentHandler equiment, out EquipmentHandler equipmentHandlerTrialHero)
    {
        var clone = equiment.Clone(stat);
        equipmentHandlerTrialHero = clone;
        return stat;
    }
}