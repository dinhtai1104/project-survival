using UnityEngine;
using System;
using System.Collections.Generic;
using Sirenix.Utilities;

[System.Serializable]
public class BuffEntity : IWeightable
{
    public int Id;
    public EBuff Type;
    public Dictionary<int, BuffLevel> LevelCard = new Dictionary<int, BuffLevel>();
    public int LevelMaxCard;
    public EBuffClassify Classify;
    public EPickBuffType PickType;
    public string Icon;
    public float Point;
    public float Percentage;
    public int DungenUnlock;

    public List<EBuff> buffCondition = new List<EBuff>();

    public float Weight => Point;
    private DB_Buff entity;
    public BuffEntity(DB_Buff e)
    {
        this.entity = e;
        Id = e.Get<int>("Id");
        Enum.TryParse(e.Get<string>("Type"), out Type);
        Enum.TryParse(e.Get<string>("PickType"), out PickType);
        LevelMaxCard = e.Get<int>("MaxLevel");
        Point = e.Get<float>("Point");

        Enum.TryParse(e.Get<string>("Classify"), out Classify);
        Icon = e.Get<string>("Icon"); 
        DungenUnlock = e.Get<int>("DungeonUnlock");

        var dataBuff = e.Get<List<string>>("BuffCondition");
        if (dataBuff.IsNullOrEmpty() == false)
        {
            foreach (var buff in dataBuff)
            {
                try
                {
                    buffCondition.Add((EBuff)Enum.Parse(typeof(EBuff), buff));
                }
                catch (Exception ex)
                {

                }
            }
        }

        if (LevelMaxCard == 0) return;
        for (int i = 0; i < LevelMaxCard; i++)
        {
            AddLevelCard(i, e, "BaseBuff", i + 1);
        }
    }
    private void AddLevelCard(int level, DB_Buff e, string content, float mul = 1)
    {
        try
        {
            var levelOne = e.Get<List<string>>(content);
            if (levelOne == null) return;
            BuffLevel LevelOne = new BuffLevel();
            foreach (var data in levelOne)
            {
                var mod = new AttributeStatModifier(data, mul);
                LevelOne.Add(mod);
            }
            if (LevelOne.Count > 0)
            {
                LevelCard.Add(level, LevelOne);
            }
        }
        catch (Exception exeptionNull)
        {
            Debug.Log(Type + "-" + exeptionNull.Message);
        }
    }
    public string GetDescription()
    {
        return I2Localize.GetLocalize($"Buff_Description/{Type}");
    }
    public BuffEntity Clone()
    {
        return new BuffEntity(entity);
    }
}
[System.Serializable]
public class BuffLevel
{
    public List<AttributeStatModifier> Modifiers = new List<AttributeStatModifier>();
    public int Count => Modifiers.Count;

    public void Add(AttributeStatModifier mod)
    {
        Modifiers.Add(mod);
    }
    public void RemoveModiFromStatGroup(IStatGroup statGroup)
    {
        foreach (var i in Modifiers)
        {
            statGroup.RemoveModifier(i.StatKey, i.Modifier);
        }
    }
    public void Add(GroupModifier statGroup)
    {
        foreach (var i in Modifiers)
        {
            if (!statGroup.HasStat(i.StatKey))
            {
                statGroup.AddModifier(i.StatKey, i.Modifier);
            }
        }
    }
}