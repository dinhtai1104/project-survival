using Game.GameActor;
using UnityEngine;

public class EquipmentStealLifePassive : BaseEquipmentPassive
{
    public ValueConfigSearch legendary_stealHpMul;

    private void OnEnable()
    {
        Messenger.AddListener<ActorBase, ActorBase>(EventKey.KilledBy, OnKill);
    }
    private void OnDisable()
    {
        Messenger.RemoveListener<ActorBase, ActorBase>(EventKey.KilledBy, OnKill);
    }

    private void OnKill(ActorBase attacker, ActorBase defender)
    {
        if (Rarity >= ERarity.Legendary)
        {
            if (attacker == Caster && defender != Caster)
            {
                var hpPercentHeal = legendary_stealHpMul.FloatValue;
                var value = Caster.HealthHandler.GetMaxHP() * hpPercentHeal;
                Debug.Log("[Buff] Weapon Life steal added after caster: " + value);

                Caster.HealthHandler.AddHealth((int)value);
            }
        }
    }
}