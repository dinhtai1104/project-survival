using Game.GameActor.Buff;
using UnityEngine;

public class GunHealBuff : AbstractBuff
{
    public override void Play()
    {
        //  Add Hp
        var hpHeal = GetValue(StatKey.Hp);

        var value = Caster.HealthHandler.GetMaxHP() * hpHeal * Caster.Stats.GetValue(StatKey.HealMul);
        Debug.Log("[Buff] Gun Heal added after caster: " + value);

        Caster.Heal((int)value, true);
        Debug.Log("Heal Current: " + Caster.HealthHandler.GetHealth());

        // Add Atk
        Caster.Stats.AddModifier(StatKey.Dmg, GetModifier(StatKey.Dmg), this);
    }
}
