using System.Collections.Generic;
using System.Linq;

public static class SkillTreeExtension
{
    public static List<LootParams> Merge(this List<SkillTreeStageEntity> list)
    {
        var result = new List<LootParams>();
        var dic = new Dictionary<StatKey, Dictionary<EStatMod, StatModifier>>();
        //foreach (var item in list)
        //{
        //    var att = item.Modifier;
        //    var statKey = att.StatKey;
        //    var modifier = att.Modifier;
        //    if (!dic.ContainsKey(statKey))
        //    {
        //        dic.Add(statKey, new Dictionary<EStatMod, StatModifier>());
        //    }
        //    var dicStatKey = dic[statKey];
        //    if (!dicStatKey.ContainsKey(modifier.Type))
        //    {
        //        dicStatKey.Add(modifier.Type, new StatModifier(modifier.Type, 0));
        //    }

        //    dicStatKey[modifier.Type].Value += modifier.Value;
        //}

        foreach (var item in list)
        {
            result.Add(new LootParams(ELootType.Stat, new SkillTreeLootData(item.Modifier.StatKey, item.Modifier.Modifier)));
        }

        return result;
    }

    public static List<AttributeStatModifier> MergeSkillTree(this List<SkillTreeStageEntity> list)
    {
        var result = new List<AttributeStatModifier>();
        var dic = new Dictionary<StatKey, Dictionary<EStatMod, StatModifier>>();
        foreach (var item in list)
        {
            var att = item.Modifier;
            var statKey = att.StatKey;
            var modifier = att.Modifier;
            if (!dic.ContainsKey(statKey))
            {
                dic.Add(statKey, new Dictionary<EStatMod, StatModifier>());
            }
            var dicStatKey = dic[statKey];
            if (!dicStatKey.ContainsKey(modifier.Type))
            {
                dicStatKey.Add(modifier.Type, new StatModifier(modifier.Type, 0));
            }

            dicStatKey[modifier.Type].Value += modifier.Value;
        }

        foreach (var item in dic)
        {
            foreach (var mod in item.Value) 
            {
                result.Add(new AttributeStatModifier(item.Key, mod.Value));
            }
        }

        return result;
    }
}