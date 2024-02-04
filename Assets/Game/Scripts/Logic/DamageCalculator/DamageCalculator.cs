using Game.Damage;
using Game.GameActor;
using UnityEngine;

public class DamageCalculator : MonoBehaviour, IDamageCalculator
{
    public bool CalculateDamage(ActorBase defender, ActorBase attacker, DamageSource source, out bool lastHit, out bool crit)
    {
        bool success = true;
        crit = false;
        lastHit = false;
        float damage = CalculatePhysicalDamage(defender, attacker, source, out success, out crit);

        if (damage > 0f)
        {
            // marked attack
            Messenger.Broadcast(EventKey.DamageToHealth, attacker, defender, damage, source);

            if (defender.HealthHandler.GetPercentHealth() <= 0f)
            {
                //Debug.Log(attacker.gameObject.name + " Killed " + defender.gameObject.name + " Using Skill " + source.SourceSkillId);
                Messenger.Broadcast(EventKey.KilledBy, defender, attacker);
                lastHit = true;
            }
        }

        return true;
    }
    private float CalculatePhysicalDamage(ActorBase defender, ActorBase attacker, DamageSource source, out bool success, out bool crit)
    {
        success = true;
        crit = false;
        IStatGroup def = defender.Stats;

        // Calculate base damage
        float baseDamage = source.Value;
        float damage = baseDamage;

        if (attacker != null)
        {
            IStatGroup atk = attacker.Stats;
            float critChance = 0f;
            float critMul = 1.5f;
            float physicalDamageMul = 1f;

            critChance = atk.GetValue(StatKey.CritRate);
            critMul = atk.GetValue(StatKey.CritDmg);

            // Apply physical damage mul
            damage *= physicalDamageMul;

            // Apply crit
            crit = GameUtility.GameUtility.RandomBoolean(critChance);
            damage = crit ? critMul * damage : damage;
            if (crit)
            {
                // broadcast crit damage
                Messenger.Broadcast(EventKey.CritDamage, attacker, defender);
            }
        }
        return damage;
    }

    public void Initialize()
    {
    }
}