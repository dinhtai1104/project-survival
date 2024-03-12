using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Framework
{
    public static class FormulaHelper
    {
        public static float CalculateDamageArmorTaken(Actor attacker, Actor defender, float damage)
        {
            var damageTaken = 1f;
            if (defender.Stats.HasStat(StatKey.Armor))
            {
                var armor = defender.Stats.GetValue(StatKey.Armor);
                if (armor >= 0)
                {
                    damageTaken = 15f / (15 + armor);
                }
                else
                {
                    damageTaken = (2 - 15f) / (15 + armor);
                }
            }
            return damage *= damageTaken;
        }
    }
}
