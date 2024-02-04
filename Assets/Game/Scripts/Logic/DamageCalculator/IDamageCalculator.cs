using Game.GameActor;

namespace Game.Damage
{
    public interface IDamageCalculator
    {
        void Initialize();
        bool CalculateDamage(ActorBase defender, ActorBase attacker, DamageSource source, out bool lastHit, out bool crit);
    }
}