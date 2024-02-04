using Game.Damage;
using Game.GameActor;

public class NullDamageCalculator : IDamageCalculator
{
    public bool CalculateDamage(ActorBase defender, ActorBase attacker, DamageSource source, out bool lastHit, out bool crit)
    {
        lastHit = false;
        crit = false;
        return false;
    }

    public void Initialize()
    {
    }
}