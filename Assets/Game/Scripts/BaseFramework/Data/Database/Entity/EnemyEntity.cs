using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] 
public class EnemyEntity
{
    public string Id;
    public float BaseHp;
    public float BaseDmg;
    public float BaseSpeed;
    public float BaseRangeAtk;
    public float BaseCooldownAtk;
    public float BodySize;
    public RigidbodyType2D BodyType;
    public float HpGrown;
    public float DmgGrown;

    public List<ETag> MonsterTags;

    public EnemyEntity() { }
    public EnemyEntity(DB_Enemy e)
    {
        MonsterTags = new List<ETag>();
        Id = e.Get<string>("Id");
        BaseHp = e.Get<float>("BaseHp");
        BaseDmg = e.Get<float>("BaseDmg");
        BaseSpeed = e.Get<float>("BaseSpeed");
        BaseRangeAtk = e.Get<float>("BaseRangeAtk");
        BaseCooldownAtk = e.Get<float>("BaseCooldownAtk");
        BodySize = e.Get<float>("BodySize");
        Enum.TryParse(e.Get<string>("BodyType"), out BodyType);
        HpGrown = e.Get<float>("HpGrown");
        DmgGrown = e.Get<float>("DmgGrown");

        foreach (var tag in e.Get<List<string>>("Tag"))
        {
            Enum.TryParse(tag, out ETag monsterTag);
            MonsterTags.Add(monsterTag);
        }
    }

    public void GetStats(IStatGroup Stat)
    {
        Stat.SetBaseValue(StatKey.Hp, BaseHp);
        Stat.SetBaseValue(StatKey.Dmg, BaseDmg);
        Stat.SetBaseValue(StatKey.SpeedMove, BaseSpeed);
        Stat.SetBaseValue(StatKey.Range, BaseRangeAtk);
        Stat.SetBaseValue(StatKey.Cooldown, BaseCooldownAtk);
    }

}