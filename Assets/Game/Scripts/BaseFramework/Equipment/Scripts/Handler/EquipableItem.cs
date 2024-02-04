using BehaviorDesigner.Runtime.Tasks.Unity.UnityPlayerPrefs;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;

[System.Serializable]
public class EquipableItem
{
    public string Id;
    public int IdSave => EquipmentSave.IdSave;
    public EEquipment EquipmentType;
    public ERarity EquipmentRarity => EquipmentSave.Rarity;
    public bool IsEquipped;

    private int itemLevel;
    public Stat BaseEquipmentValue;

    private StatAffix _baseStatAffix;
    [ShowInInspector]
    private EquipmentSave equipmentSave;
    [ShowInInspector]
    private List<BaseAffix> _baseAffixes = new List<BaseAffix>();
    [ShowInInspector]
    private Dictionary<ERarity, BaseAffix> _lineAffixes = new Dictionary<ERarity, BaseAffix>();

    private Dictionary<StatKey, BaseAffix> _statAffixes = new Dictionary<StatKey, BaseAffix>();

    [ShowInInspector]
    public float BaseStatValue
    {
        get
        {
            if (_baseStatAffix != null)
                return _baseStatAffix.Value;
            return 0f;
        }
    }
    public int ItemLevel => equipmentSave.Level;
    public List<BaseAffix> BaseAffixes { get => _baseAffixes; }
    public StatAffix BaseStatAffix { get => _baseStatAffix; set => _baseStatAffix = value; }
    public Dictionary<ERarity, BaseAffix> LineAffixes { get => _lineAffixes; set => _lineAffixes = value; }
    public EquipmentSave EquipmentSave { get => equipmentSave; set => equipmentSave = value; }
    public EquipmentEntity EquipmentEntity => _entity;

    // Base Grown Upgrade
    private Stat _baseStatGrown;
    private EquipmentEntity _entity;
    public Stat BaseStatGrown => _baseStatGrown;
    public Stat AddStat = new Stat();

    // Event Callback Enhance
    [ShowInInspector]
    public System.Action EnhanceEvent;
    [ShowInInspector]
    public System.Action UpRarityEvent;
    [ShowInInspector]
    public System.Action EquipEvent;
    [ShowInInspector]
    public System.Action UnEquipEvent;

    public EquipableItem() : base()
    {

    }

    public EquipableItem(EquipmentEntity entity) : this()
    {
        _entity = entity;
        _baseStatGrown = new Stat(entity.StatGrown, 0);
        Id = _entity.Id;
        this.EquipmentType = _entity.Type;
        BaseEquipmentValue = new Stat(entity.StatBase, 0);
    }
    
    public void ApplySaveData(EquipmentSave save)
    {
        this.EquipmentSave = save;
        itemLevel = equipmentSave.Level;

        AddAffixIfPossible();
    }

    public void SetBaseStat(StatAffix baseStat)
    {
        BaseStatAffix?.OnRemoveFromItem(this);
        BaseStatAffix = baseStat;
        baseStat.OnAddToItem(this);
    }

    public void ClearBaseAffix()
    {
        foreach (var affix in _baseAffixes)
        {
            affix.OnRemoveFromItem(this);
        }
    }
    public void AddBaseAffix(BaseAffix affix)
    {
        _baseAffixes.Add(affix);
        affix.OnAddToItem(this);
    }

    public void ClearBaseAffix(ERarity line)
    {
        LineAffixes[line].OnRemoveFromItem(this);
    }
    public void AddBaseAffix(ERarity line, BaseAffix affix)
    {
        if (!LineAffixes.ContainsKey(line))
        {
            LineAffixes.Add(line, affix);
        }
    }

    public void OnEquip(IStatGroup stats)
    {
        BaseStatAffix.OnEquip(stats);

        foreach (var affix in _baseAffixes)
        {
            affix.OnEquip(stats);
        }

        foreach (var line in LineAffixes)
        {
            line.Value.OnEquip(stats);
            line.Value.OnRemoveFromItem(this);
        }
        AddAffixIfPossible();
        EquipEvent?.Invoke();
    }

    public void OnUnequip()
    {
        BaseStatAffix.OnUnEquip();

        foreach (var affix in _baseAffixes)
        {
            affix.OnUnEquip();
        }
        foreach (var line in LineAffixes)
        {
            line.Value.OnRemoveFromItem(this);
            line.Value.OnUnEquip();
        }
        UnEquipEvent?.Invoke();
    }
    private void AddAffixIfPossible()
    {
        foreach (var line in LineAffixes)
        {
            line.Value.OnRemoveFromItem(this);
            if (line.Key <= EquipmentRarity)
            {
                line.Value.OnAddToItem(this);
            }
        }
    }
    public void Enhance()
    {
        itemLevel++;
        // save
        equipmentSave.Level = itemLevel;

        EnhanceEvent?.Invoke();
    }

    public void UpRarity()
    {
        if (EquipmentRarity < ERarity.Ultimate)
        {
            // save
            equipmentSave.RarityUp();
            UpRarityEvent?.Invoke();
        }
        AddAffixIfPossible();
    }

    public bool CanUpgradeSelf()
    {
        return false;
    }

    public void Dispose()
    {
        UpRarityEvent = null;
        EnhanceEvent = null;
    }

    public EquipableItem Clone()
    {
        var e = EquipmentSave.Clone();
        var item = e.CreateEquipableItem();
        return item;
    }
    public EquipableItem CloneShallow()
    {
        var e = EquipmentSave.Clone();
        var item = e.CreateInstanceEquipableItem();


        GameSceneManager.Instance.EquipmentFactory.ApplyLevelItem(item, e.Level);
        GameSceneManager.Instance.EquipmentFactory.ApplyRarityItem(item, e.Rarity);
        return item;
    }

    public override string ToString()
    {
        return $"-Id: {Id}-Type: {EquipmentType}-Level: {ItemLevel}-";
    }

    public bool TryGetLineAffix(ERarity rarity, out string hiddenLine)
    {
        if (LineAffixes.ContainsKey(rarity))
        {
            if (LineAffixes[rarity] == NullAffix.Null)
            {
                hiddenLine = null;
                return false;
            }
            if (EquipmentRarity == rarity)
            {
                hiddenLine = LineAffixes[rarity].GetDescription();
                return true;
            }
        }
        hiddenLine = null;
        return false;
    }

    public StatAffix GetStat(StatKey key)
    {
        if (_statAffixes.Count!=_baseAffixes.Count)
        {
            _statAffixes.Clear();
            foreach (StatAffix affix in _baseAffixes)
            {
                if (affix.StatName.Equals(key))
                {
                    _statAffixes.Add(key, affix);
                }
            }
        }
    
        return (StatAffix)_statAffixes[key];
    }
}