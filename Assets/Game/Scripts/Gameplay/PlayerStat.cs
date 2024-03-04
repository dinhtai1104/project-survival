using Engine;
using UnityEngine;

namespace Gameplay
{
    [System.Serializable]
    public class PlayerStat : StatGroup
    {
        public static PlayerStat Default()
        {
            var stats = new PlayerStat();

            stats.AddStat(StatKey.Hp, 0);
            stats.AddStat(StatKey.Damage, 0);
            stats.AddStat(StatKey.CritDamage, 0, 0f);
            stats.AddStat(StatKey.CritRate, 0, 0, 1f);
            stats.AddStat(StatKey.Speed, 0, 0f);

            stats.AddStat(StatKey.AttackSpeed, 0, 0.1f);
            // headshot rate -> one kill per shot
            stats.AddStat(StatKey.HeadshotRate, 0, 0, 1f);

            stats.AddStat(StatKey.DodgeRate, 0, 0, 1f);
            return stats;
        }
    }
}