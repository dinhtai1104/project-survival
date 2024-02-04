using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using GameUtility;
using System;
using static AnimData;

[System.Serializable]
public class BuffTable : DataTable<EBuff, BuffEntity>
{
    private float MaxPoint = 0;
    public override void GetDatabase()
    {
        MaxPoint = 0;
        DB_Buff.ForEachEntity(e => Get(e));
        RecalculatePercentage();
    }

    private void RecalculatePercentage()
    {
        foreach (var entity in Dictionary)
        {
            entity.Value.Percentage = entity.Value.Point * 1.0f / MaxPoint;
        }
    }

    private void Get(DB_Buff e)
    {
        var entity = new BuffEntity(e);
        if (!Dictionary.ContainsKey(entity.Type))
        {
            Dictionary.Add(entity.Type, entity);
            MaxPoint += entity.Point;
        }
    }

    public List<BuffEntity> GetBuffs(int amount, EPickBuffType _type, EHero hero)
    {
        var list = new List<BuffEntity>();
        //var buffDataPlayer = DataManager.Save.Buff.Dungeon;
        var buffDataPlayer = GameController.Instance.GetSession().buffSession.Dungeon;
        var collection2 = Dictionary.Values.ToList();
        var collection = new List<BuffEntity>();
        collection2.ForEach(t => collection.Add(t.Clone()));

        collection.RemoveAll(t => t.PickType == EPickBuffType.None);
        collection.RemoveAll(RemoveByDungeonUnlock);
        collection.RemoveAll(RemoveBuffHasNotBuffCondition);
        if (_type == EPickBuffType.Normal)
        {
            collection.RemoveAll(e => e.PickType != _type);
            for (int i = 0; i < amount; i++)
            {
                if (collection.Count == 0) break;
                collection.RemoveAll(e => buffDataPlayer.IsMaxLevel(e));
                RemoveBuffHero(hero, ref collection);
                var randomEntity = collection.RandomWeighted(out int Index);
                list.Add(randomEntity);
                collection.RemoveAt(Index);
            }

            int currentAmount = list.Count;

            for (int i = currentAmount; i < amount; i++)
            {
                list.Add(Dictionary[EBuff.AttackUp]);
            }
        }
        else
        {
            list.Add(Get(EBuff.Heal));
            collection.RemoveAll(t => t.Type == EBuff.Heal);

            for (int i = 1; i < amount; i++)
            {
                if (collection.Count == 0) break;
                collection.RemoveAll(e => buffDataPlayer.IsMaxLevel(e));
                RemoveBuffHero(hero, ref collection);
                var randomEntity = collection.RandomWeighted(out int Index);
                list.Add(randomEntity);
                collection.RemoveAt(Index);
            }

            int currentAmount = list.Count;

            for (int i = currentAmount; i < amount; i++)
            {
                list.Add(Dictionary[EBuff.AttackUp]);
            }
        }

        return list;
    }

    private bool RemoveBuffHasNotBuffCondition(BuffEntity buffEntity)
    {
        var buffSave = GameController.Instance.GetSession().buffSession;
        var buffConditionForThisBuff = buffEntity.buffCondition;

        if (buffSave.HasBuff(buffConditionForThisBuff))
        {
            return false;
        }

        return true;
    }

    public void RemoveBuffHero(EHero hero, ref List<BuffEntity> collection)
    {
        try
        {
            var value = ConstantValue.HeroExceptionBuff[hero];
            collection.RemoveAll(t => value.Contains(t.Type));
        }
        catch (Exception ex)
        {

        }
    }
    private bool RemoveByDungeonUnlock(BuffEntity buff)
    {
        var dungeonSave = DataManager.Save.Dungeon;
        if (dungeonSave.IsDungeonCleared(buff.DungenUnlock) || buff.DungenUnlock == -1)
        {
            return false;
        }
        return true;
    }

    private void RecalculatBuffPercentageChoice(List<BuffEntity> collection)
    {
        // Recalculate rate
        float MaxPoint = collection.Sum(t => t.Point);
        collection.ForEach(t =>
        {
            t.Percentage = t.Point * 1.0f / MaxPoint;
        });
    }

    public List<BuffEntity> BuffUnlockAtDungeon(int Dungeon)
    {
        var collection2 = Dictionary.Values.ToList();
        var collection = collection2.FindAll(t => t.DungenUnlock == Dungeon).ToList();

        return collection;
    }
}