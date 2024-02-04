using Game.GameActor;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    [SerializeField]
    private PlayerStat _stats;

    /// <summary>
    /// Equipment for character
    /// </summary>

    public EquipmentHandler EquipmentHandler;
    public ExpHandler ExpHandler;
    public EHero HeroCurrent;
    [ShowInInspector]
    public Dictionary<EHero, HeroStatData> HeroDatas = new Dictionary<EHero, HeroStatData>();

    private List<AttributeStatModifier> _modifierApplyToAllHero = new List<AttributeStatModifier>();
    private List<AttributeStatModifier> _modifierApplySkillTree = new List<AttributeStatModifier>();

    public PlayerStat Stats { get => _stats; set => _stats = value; }

    public PlayerData(PlayerStat stats, EHero currentHero, EquipmentHandler equipmentHandler, ExpHandler expHandler)
    {
        this.HeroCurrent = currentHero;
        this.EquipmentHandler = equipmentHandler;
        this.ExpHandler = expHandler;

        var heroSaves = DataManager.Save.User.HeroSaves;
        foreach (var hero in EnumCollection.AllHeroes)
        {
            var heroStat = new HeroStatData(heroSaves[hero], this);
            HeroDatas.Add(hero, heroStat);
        }

        //ApplyStatModifierAffectToAllHeroes();
        RefreshStatHeroes();
        _stats = stats;
        _stats.ReplaceAllStatBySource(HeroDatas[HeroCurrent].heroStat, EStatSource.sourceHero);
    }

    /// <summary>
    /// Apply speacial stat (like stat for all heroes)
    /// </summary>

    public void ApplyStatModifierAffectToAllHeroes()
    {
        RemoveStatModifierAllHero(_modifierApplyToAllHero);
        foreach (var hero in HeroDatas)
        {
            hero.Value._ApplyStatModifierAllHero(_modifierApplyToAllHero, EStatSource.HeroModifierAll);
        }
    }

    /// <summary>
    /// Add special stat of one hero
    /// </summary>
    /// <param name="mods"></param>
    public void AddModifierForAllHero(List<AttributeStatModifier> mods)
    {
        foreach (var mod in mods)
        {
            _modifierApplyToAllHero.Add(mod);
        }
        //ApplyStatModifierAffectToAllHeroes();
    }
    public void AddModifierSkillTree(AttributeStatModifier mod)
    {
        _modifierApplySkillTree.Add(mod);
        foreach (var hero in HeroDatas)
        {
            hero.Value._ApplyStatModifierAllHero(mod, EStatSource.SkillTree);
        }
    }

    public void AddMondifierTalent(AttributeStatModifier mod)
    {
        foreach (var hero in HeroDatas)
        {
            hero.Value._ApplyStatModifierAllHero(mod, EStatSource.Talent);
        }
    }


    /// <summary>
    /// When one Hero change level or star may be affect to special stat like -Stat For All Hero
    /// We need to refresh _modifierApplyToAllHero by remove old all special stat of that hero 
    /// </summary>
    /// <param name="modifierForAllHeroes"></param>
    public void RemoveStatModifierAllHero(List<AttributeStatModifier> modifierForAllHeroes)
    {
        var mods = new List<AttributeStatModifier>(modifierForAllHeroes);
        foreach (var modall in mods)
        {
            for (int i = _modifierApplyToAllHero.Count - 1; i >= 0; i--)
            {
                var mod = _modifierApplyToAllHero[i];
                if (mod == modall)
                {
                    _modifierApplyToAllHero.RemoveAt(i);
                    break;
                }
            }
        }
    }
    /// <summary>
    /// When detect one change => apply base stat then apply special stat (stat for all)
    /// </summary>
    public void RefreshStatHeroes()
    {
        foreach (var hero in HeroDatas)
        {
            hero.Value._Apply();
        }

        var statAllHero = new List<AttributeStatModifier>();
        foreach (var hero in HeroDatas)
        {
            statAllHero.AddRange(hero.Value._modifierForAllHeroes);
            hero.Value.heroStat.RemoveModifiersFromSource(EStatSource.HeroModifierAll);
        }

        foreach (var hero in HeroDatas)
        {
            hero.Value._ApplyStatModifierAllHero(statAllHero, EStatSource.HeroModifierAll);
        }
        //ApplyStatModifierAffectToAllHeroes();

        GameSceneManager.Instance.PlayerData.Stats.GetStat(StatKey.Dmg)?.InvokeListeners();
        GameSceneManager.Instance.PlayerData.Stats.GetStat(StatKey.Hp)?.InvokeListeners();
    }

    public void LoadEquipmentPassive(ActorBase player)
    {
        EquipmentHandler.EquipPassive(player);
    }

    public void Init(EHero hero)
    {
        _stats.ReplaceAllStatBySource(HeroDatas[hero].heroStat, EStatSource.sourceHero);
    }
}