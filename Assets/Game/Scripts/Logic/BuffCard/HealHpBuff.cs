using UnityEngine;

namespace Game.GameActor.Buff
{
    public class HealHpBuff : AbstractBuff
    {
        public override void Play()
        {
            var hpHeal = GetValue(StatKey.Hp);

            var value = Caster.HealthHandler.GetMaxHP() * hpHeal * Caster.Stats.GetValue(StatKey.HealMul);
            Debug.Log("[Buff] Hp added after caster: " + value);

            Caster.Heal((int)value);
            Debug.Log("Heal Current: " + Caster.HealthHandler.GetHealth());
        }
    }
}