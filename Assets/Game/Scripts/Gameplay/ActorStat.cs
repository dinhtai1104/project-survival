using Engine;
using UnityEngine;

namespace Gameplay
{
    [System.Serializable]
    public class ActorStat : StatGroup
    {
        public static ActorStat Default()
        {
            var stats = new ActorStat();

            AddStat(StatKey.Hp, 100, 1);
            AddStat(StatKey.Damage, 1, 1);
            AddStat(StatKey.DamageBonus, 1, 1);
            AddStat(StatKey.RangeDamage, 1, 1);
            AddStat(StatKey.ElementDamage, 1, 1);
            AddStat(StatKey.EngineeringDamage, 1, 1);
            AddStat(StatKey.AttackSpeed, 0);
            AddStat(StatKey.HpRegeneration, 0, 0);
            AddStat(StatKey.CritChance, 0, 0, 1f);
            AddStat(StatKey.CritDamage, 0, 0);
            AddStat(StatKey.Speed, 6, 0);
            AddStat(StatKey.DodgeRate, 0, 0, 0.6f);
            AddStat(StatKey.LuckRate, 0, 0);
            AddStat(StatKey.InterestRate, 0, 0);
            AddStat(StatKey.Armor, 0, 0);
            AddStat(StatKey.LifestealRate, 0, 0);
            AddStat(StatKey.XpGain, 0, 0);
            AddStat(StatKey.PickupRange, 0, 0);
            AddStat(StatKey.Knockback, 0, 0);
            AddStat(StatKey.ConsumableHeal, 0, 0);
            AddStat(StatKey.PatrolEarn, 0, 0);
            AddStat(StatKey.BossDamage, 0, 0);

            stats.CalculateStats();
            return stats;

            void AddStat(string stat, float baseValue, float minValue = float.MinValue, float maxValue = float.MaxValue)
            {
                stats.AddStat(stat, baseValue, minValue, maxValue);
            }
        }
    }
}