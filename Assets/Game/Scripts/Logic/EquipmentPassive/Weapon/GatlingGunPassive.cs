using Game.GameActor;

public class GatlingGunPassive : WeaponPassive
{
    private Stat focusFireStat;
    public ValueConfigSearch stepIncreaseDmg;
    public ValueConfigSearch maxIncreaseDmg;

    private ActorBase lastActorDefender;
    private void OnEnable()
    {
        Messenger.AddListener<ActorBase, ActorBase, DamageSource>(EventKey.BeforeHit, OnBeforeHit);
    }
    private void OnDisable()
    {
        Messenger.RemoveListener<ActorBase, ActorBase, DamageSource>(EventKey.BeforeHit, OnBeforeHit);
    }
    private void OnBeforeHit(ActorBase attacker, ActorBase defender, DamageSource damageSource)
    {
        if (attacker != Caster) return;
        if (Rarity < ERarity.Epic) return;
        if (focusFireStat == null) return;
        if (lastActorDefender == null)
        {
            lastActorDefender = defender;
            focusFireStat.ClearModifiers();
            focusFireStat.BaseValue = 0;
            return;
        }

        if (lastActorDefender == defender)
        {
            focusFireStat.AddModifier(new StatModifier(EStatMod.Flat, stepIncreaseDmg.FloatValue));
            damageSource.Value *= (1 + focusFireStat.Value);
        }
        else
        {
            lastActorDefender = defender;
            focusFireStat.ClearModifiers();
            focusFireStat.BaseValue = 0;
        }
    }

    public override void Play()
    {
        focusFireStat = new Stat(0, 0, maxIncreaseDmg.FloatValue);
    }
}
