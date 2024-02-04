using Game.GameActor;
using UnityEngine;

public class SamuraiNecklacePassive : BaseEquipmentPassive
{
    public ValueConfigSearch epicShotCount;
    public ValueConfigSearch legendaryShotCount;

    public ValueConfigSearch epicDmgExtra;
    public ValueConfigSearch legendaryDmgExtra;
    private int shotCurrent = 0;
    private int shotCount = 0;
    private float dmgExtra = 0;
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
        shotCurrent++;
        if (shotCurrent == shotCount)
        {
            Debug.Log("[Passive Samurai Necklace] Appied Increase Dmg each " + shotCount + " times shot");
            shotCurrent = 0;
            damageSource.AddModifier(new StatModifier(EStatMod.PercentAdd, dmgExtra));
            //*= (1 + dmgExtra);
        }
    }
    public override void Play()
    {
        base.Play();
        if (Rarity >= ERarity.Epic)
        {
            shotCount = epicShotCount.IntValue;
            dmgExtra = epicDmgExtra.FloatValue;
        }
        if (Rarity >= ERarity.Legendary)
        {
            shotCount = legendaryShotCount.IntValue;
            dmgExtra = legendaryDmgExtra.FloatValue;
        }
    }
}